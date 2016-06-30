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
namespace FAMIS.Controllers
{
    public class UserController : Controller
    {
        //
        FAMISDBTBModels DBConnecting = new FAMISDBTBModels();



        // GET: Login
        public ActionResult Index()
        {
            if(!IsUserLogined())
            {
                LoginOut();
            }


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
                List<tb_user> userList = DBConnecting.tb_user.Where(a => a.name_User == userName).Where(b => b.password_User == password).ToList();
                if (userList != null && userList.Count == 1)
                {

                    Session["Logined"] = "OK";
                    ViewBag.LoginUser = "欢迎您：" + userList[0].name_User;
                    //往Session里面保存用户信息
                    //用户名
                    Session["userName"] = userList[0].name_User;
                    //用户名
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
            
            return Session["userRole"].ToString();
           
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
                List<tb_user> list = DBConnecting.tb_user.Where(a => a.name_User == userName).Where(b => b.password_User == password).Where(c => c.roleID_User == userRole).ToList();
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
            StreamWriter sw = new StreamWriter("D:\\msda.txt");
            string Menu_JSON="";
            int rid = int.Parse(JSON);
            int indexof_menu_ID=1;
            IEnumerable<int?> menu_ID = from o in DBConnecting.tb_role_authorization
                                                         where o.role_ID == rid
                                                         && o.type=="menu"
                                                        
                                                         select o.Right_ID;
           
            
           foreach(int mid in menu_ID)
           {
              IEnumerable<tb_Menu> menue_details = from m in DBConnecting.tb_Menu
                              where m.ID == mid
                              orderby m.ID_Menu
                              select m;
              foreach (tb_Menu menu in menue_details)
              {
                  string menu_rankID=menu.ID_Menu;
                  if (!menu_rankID.Contains("_"))
                  {
                      if(indexof_menu_ID==1)
                          Menu_JSON += "{\r\"menus\":[{\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\",\r\"icon\":\"icon-sys\",\r \"menus\": [\r";
                      else
                          Menu_JSON += "]},\r{\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-sys\",\r \"menus\": [\r";

                  }

                  else
                  {
                      if (indexof_menu_ID != menu_ID.Count())
                      {
                          if (int.Parse(menu_rankID.Split('_')[1]) == 1)
                              Menu_JSON += "{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"\r}";
                          else
                              Menu_JSON += ",{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"\r}";

                      }
                      else
                      {
                          if (int.Parse(menu_rankID.Split('_')[1]) == 1)
                              Menu_JSON += "{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"}]\r}]\r}";
                          else
                              Menu_JSON += ",{\r\"menuid\":\"" + menu_rankID + "\",\r \"menuname\":\"" + menu.name_Menu + "\" ,\r\"icon\":\"icon-nav\",\r\"url\":\"" + menu.url + "\"}]\r}]\r}";
                      }
                  }
              }

              indexof_menu_ID++;
           }
           sw.Write(Menu_JSON);
           sw.Close();
           return Menu_JSON;


    }
        //==========================================================================================================================//







    }
}