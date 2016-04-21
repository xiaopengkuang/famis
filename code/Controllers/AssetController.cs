using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOM.Controllers
{
    public class AssetController : Controller
    {
        // GET: Asset
        public ActionResult Accounting()
        {
            return View();
        }
        public ActionResult allocation()
        {
            return View();
        }
        public ActionResult collar()
        {
            return View();
        }
        public ActionResult reduction()
        {
            return View();
        }
        
    }
}