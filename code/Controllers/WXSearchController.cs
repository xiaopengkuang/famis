using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAMIS.Controllers
{
    public class WXSearchController : Controller
    {
        // GET: WXSearch
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult WX_detail(String bh)
        {
            ViewBag.bh = bh;
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return View();
        }
    }
}