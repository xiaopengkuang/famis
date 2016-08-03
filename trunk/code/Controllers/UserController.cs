using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.ControllerSQLs;
using FAMIS.DAL;
using FAMIS.DTO;
using FAMIS.Models;
using System.IO;
using FAMIS.DataConversion;

namespace FAMIS.Controllers
{
    public class UserController : Controller
    {
        //
        FAMISDBTBModels DBConnecting = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();


        // GET: Login
        public ActionResult Index()
        {
            if(!IsUserLogined())
            {
                LoginOut();
            }

            ViewBag.LoginUser = Session["userName"] == null ? "未知用户" : Session["userName"].ToString();

            return View();
        }




        public ActionResult actions()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public void LoginCheck(FormCollection fc)
        {
            String userName = fc["UserName"];
            String password = fc["password"];

            //验证处理
            String sql_check = UserActionSQL.getSQL_Select_User_CheckCount(userName, password);
            SQLRunner runner = new SQLRunner();
            int count = runner.runSelectSQL_Counter(sql_check, "total");
            


            if (count == 1)//存在用户
            {
                List<tb_user> userList = DBConnecting.tb_user.Where(a => a.name_User == userName).Where(b => b.password_User == password).Where(f=>f.flag==true).ToList();
                if (userList != null && userList.Count == 1)
                {

                    Session["Logined"] = "OK";
                    ViewBag.LoginUser = userList[0].true_Name;
                    //往Session里面保存用户信息
                    //用户名
                    Session["userName"] = userList[0].name_User;

                    Session["userID"] = userList[0].ID;
                    ////用户名
                    Session["password"] = userList[0].password_User;
                    //用户角色
                    Session["userRole"] = userList[0].roleID_User;
                    
                    //更新用户登录时间

                    Response.Redirect("/User/Index");
                }
                else {
                    ViewBag.LoginUser = "";
                    Session.Remove("Logined");
                    Session.RemoveAll();
                    Response.Redirect("/User/ErrorLogin");
                }
            }
            else {
                ViewBag.LoginUser = "";
                Session.Remove("Logined");
                Session.RemoveAll();
                Response.Redirect("/User/ErrorLogin");
            }
        }

        public void LoginOut()
        {
            //清除登录信息再跳转
            Session.Remove("Logined");
            ViewBag.LoginUser = "";
            Session.RemoveAll();
            //jump
            Response.Redirect("/User/Login");
        }
       [HttpPost]
        public string GetRole()
        {
            return commonConversion.getRoleID().ToString();
        }

        public String signOut()
        {
            ViewBag.LoginUser = "";
            Session.RemoveAll();
            return "/User/Login";
        }

        public ActionResult ErrorLogin()
        {

            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult EditPassWord()
        {
            //if (!IsUserLogined())
            //{
            //    LoginOut();
            //}
            return View();
        }

        public int redefinePWD(String pwd_old, String pwd_new)
        {
            if (pwd_new == null || pwd_old == null || pwd_new == "" || pwd_old == "")
            {
                return -1;
            }
            //判断密码是否正确
            if (isPasswordRight(pwd_old) < 1)
            {
                return -2;
            }
            //修改
            int? userID = commonConversion.getUSERID();
            var data = from p in DBConnecting.tb_user
                       where p.flag == true
                       where p.ID == userID
                       select p;
            if (data.Count() < 1)
            {
                return -3;
            }
            bool flagRemove = false;
            try {
                foreach (var item in data)
                {
                    item.password_User = pwd_new;
                }
                DBConnecting.SaveChanges();
                flagRemove = true;
            }
            catch (Exception e) {
                return -4;
            }
            if (flagRemove)
            {
                ViewBag.LoginUser = "";
                Session.RemoveAll();
                return 1;
            }
            return -5;



            

        }

        public int isPasswordRight(String pwd)
        {

            int? userID = commonConversion.getUSERID();
            String userName = commonConversion.getOperatorName();
            var data = from p in DBConnecting.tb_user
                       where p.flag == true
                       where p.ID == userID && p.name_User == userName && p.password_User == pwd
                       select p;
            return data.Count();
        }




        //==========================================================================================================================//
        //判断用户是否登录  同时也得保证数据库中存在该用户
        public Boolean IsUserLogined()
        {
            if (Session["Logined"] == null)
            {
                return false;
            }

            if(Session["Logined"].ToString()=="OK")
            {
                String userName = Session["userName"] == null ? "" : Session["userName"].ToString();
                String password = Session["password"] == null ? "" : Session["password"].ToString();
                int userRole = int.Parse(Session["userRole"] == null ? "0" : Session["userRole"].ToString());
                List<tb_user> list = DBConnecting.tb_user.Where(f=>f.flag==true).Where(a => a.name_User == userName).Where(b => b.password_User == password).Where(c => c.roleID_User == userRole).ToList();
                if (list.Count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }else{
                return false;
            }


        }
        

        [HttpPost]
        public String Get_Json_Memu(string JSON)
        {
            int? roleID = commonConversion.getRoleID();
            bool supU = commonConversion.isSuperUser(roleID);
           // StreamWriter sw = new StreamWriter("D:\\msda.txt");
            string Menu_JSON = "";
            int rid = int.Parse(JSON);
            int indexof_menu_ID = 1;
            int ArgeFlag = 0;
             
              

            
            
                var menu_ID = !supU? from o in DBConnecting.tb_role_authorization
                              join m in DBConnecting.tb_Menu on o.Right_ID equals m.ID
                                     where o.role_ID == rid && o.type == "menu" && m.isMenu == true
                              orderby m.ID_Menu
                                    select m : from o in DBConnecting.tb_Menu
                                               where o.isMenu==true
                                               orderby o.ID_Menu
                                               select o;
                foreach (var menu in menu_ID)
                {


                    string menu_rankID = menu.ID_Menu;
                    if (!menu_rankID.Contains("_"))
                    {
                        if (indexof_menu_ID == 1)
                            Menu_JSON += "{\r\"menus\":[{\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\",\r\"icon\":\"icon-sys\",\r \"menus\": [\r";
                        else
                            Menu_JSON += "]},\r{\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-sys\",\r \"menus\": [\r";
                        ArgeFlag = 1;
                    }

                    else
                    {
                        if (indexof_menu_ID != menu_ID.Count())
                        {
                            if (ArgeFlag == 1)
                                Menu_JSON += "{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"\r}";
                            else
                                Menu_JSON += ",{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"\r}";
                            ArgeFlag = 0;
                        }
                        else
                        {
                            if (ArgeFlag ==1)
                                Menu_JSON += "{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"}]\r}]\r}";
                            else
                                Menu_JSON += ",{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"}]\r}]\r}";
                            ArgeFlag =0;
                        }
                    }


                    indexof_menu_ID++;
                }
           
            
         
         //  sw.Write(Menu_JSON);
          // sw.Close();
           return Menu_JSON;


    }
        //==========================================================================================================================//







    }
}