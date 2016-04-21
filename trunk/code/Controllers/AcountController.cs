using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.DAL;
namespace FAMIS.Controllers
{
    public class AcountController : Controller
    {
        // GET: Acount
        private AccountContext db = new AccountContext();
        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            ViewBag.LoginState = "登录前！";
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            /*"Cannot attach the file 'c:\\users\\administrator\\documents\\visual studio 2013
             * \\Projects\\FAMIS1\\FAMIS1\\App_Data\\FAMIS1.mdf' as database 'AccountInitializer'."}*/
            string email = fc["exampleInputEmail1"];
            string password = fc["exampleInputPassword1"];
            var user = db.Users.Where(b => b.User_Email == email & b.User_Password == password);
            if (user.Count() > 0)
            {
                ViewBag.LoginState = email + " " + "登录后！";
            }
            else
            {
                ViewBag.LoginState = "用户不存在";
            }
               
            return View();
         
        }

        /**
         *根据账号类型跳转到对应的页面
         * */
        public void JumpByAccount(FormCollection fc)
        {
            string email = fc["email"];
            string password = fc["password"];

        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection fc)
        {
            return View();
        }

        public object b { get; set; }
    }
}