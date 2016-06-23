using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.ControllerSQLs;
using FAMIS.DAL;
using FAMIS.DTO;
using FAMIS.Models;

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

                    Response.Redirect("/User/Index?" + userList[0].roleID_User + "");
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

        //==========================================================================================================================//







    }
}