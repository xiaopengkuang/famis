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
    public class HomeController : Controller
    {
        FAMISDBTBModels DBC= new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();


        // GET: Login
        public ActionResult Index()
        {
            if (!IsUserLogined())
            {
                LoginOut();
            }


            return View();
        }

        public ActionResult actions()
        {
            return View();
        }
        public ActionResult Test()
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
        
            var data = from p in DBC.tb_user
                       where p.flag == true
                       where p.name_User == userName && p.password_User == password
                       select p;


            if (data.Count()== 1)//存在用户
            {
                List<tb_user> userList = DBC.tb_user.Where(a => a.name_User == userName).Where(b => b.password_User == password).Where(f => f.flag == true).ToList();
                if (userList != null && userList.Count == 1)
                {

                    Session["Logined"] = "OK";
                    ViewBag.LoginUser = "欢迎您：" + userList[0].name_User;
                    //往Session里面保存用户信息
                    //用户名
                    Session["userName"] = userList[0].name_User;

                    Session["userID"] = userList[0].ID;
                    ////用户名
                    Session["password"] = userList[0].password_User;
                    //用户角色
                    Session["userRole"] = userList[0].roleID_User;

                    //更新用户登录时间

                    Response.Redirect("/Home/index");
                }
                else
                {
                    ViewBag.LoginUser = "";
                    Session.Remove("Logined");
                    Session.RemoveAll();
                    Response.Redirect("/Home/ErrorLogin");
                }
            }
            else
            {
                ViewBag.LoginUser = "";
                Session.Remove("Logined");
                Session.RemoveAll();
                Response.Redirect("/Home/ErrorLogin");
            }
        }

        public void LoginOut()
        {
            //清除登录信息再跳转
            Session.Remove("Logined");
            ViewBag.LoginUser = "";
            Session.RemoveAll();
            //jump
            Response.Redirect("/Home/Login");
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
            return "/Home/Login";
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

            if (Session["Logined"].ToString() == "OK")
            {
                String userName = Session["userName"] == null ? "" : Session["userName"].ToString();
                String password = Session["password"] == null ? "" : Session["password"].ToString();
                int userRole = int.Parse(Session["userRole"] == null ? "0" : Session["userRole"].ToString());
                List<tb_user> list = DBC.tb_user.Where(f => f.flag == true).Where(a => a.name_User == userName).Where(b => b.password_User == password).Where(c => c.roleID_User == userRole).ToList();
                if (list.Count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


        }

        [HttpPost]
        public String Get_Json_Memu(string JSON)
        {
            StreamWriter sw = new StreamWriter("D:\\msda.txt");
            string Menu_JSON = "";
            int rid = int.Parse(JSON);
            int indexof_menu_ID = 1;
            IEnumerable<int?> menu_ID = from o in DBC.tb_role_authorization
                                        where o.role_ID == rid
                                        && o.type == "menu"

                                        select o.Right_ID;


            foreach (int mid in menu_ID)
            {
                IEnumerable<tb_Menu> menue_details = from m in DBC.tb_Menu
                                                     where m.ID == mid
                                                     orderby m.ID_Menu
                                                     select m;
                foreach (tb_Menu menu in menue_details)
                {
                    string menu_rankID = menu.ID_Menu;
                    if (!menu_rankID.Contains("_"))
                    {
                        if (indexof_menu_ID == 1)
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
    }
}