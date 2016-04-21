using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOM.Controllers
{
    public class DictController : Controller
    {
        // GET: Dict
        public ActionResult staff()
        {
            return View();
        }
        public ActionResult dataDict()
        {
            return View();
        }
        public ActionResult Asset_type()
        {
            return View();
        }
        public ActionResult supplier()
        {
            return View();
        }
    }
}