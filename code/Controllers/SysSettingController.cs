using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOM.Controllers
{
    public class SysSettingController : Controller
    {
        // GET: SysSetting
        public ActionResult RoleManage()
        {
            return View();
        }
        public ActionResult SysConfig()
        {
            return View();
        }
        public ActionResult UserManage()
        {
            return View();
        }
    }
}