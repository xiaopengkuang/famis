using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOM.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SuperAdministrator()
        {
            return View();
        }
        public ActionResult SuperAdministrator_Test()
        {
            return View();
        }

        
    }
}