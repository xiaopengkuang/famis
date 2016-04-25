using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAMIS.Controllers
{
    public class UserController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
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
            if(userName.Equals("BYRD")&&password.Equals("123")){

                //pass
                Session["LoginState"] = "Logined";
                ViewBag.LoginUser = "欢迎您：" + userName;
                Response.Redirect("/Manage/Index");
            }
            else{
                ViewBag.LoginUser = "";
                Session.Remove("LoginState");
                Response.Redirect("/User/ErrorLogin");
            }

        }

        public void LoginOut()
        {
            //清除登录信息再跳转
            Session.Remove("LoginState");
            Session.RemoveAll();
            ViewBag.LoginUser = "未登录";
            //jump
            Response.Redirect("/User/Login");
        }

        public ActionResult ErrorLogin()
        {

            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
    }
}