using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.ViewCommon;

namespace FAMIS.Controllers
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

        public String loadSearchTreeByRole(String roleName)
        {


            TreeViewCommon treeviewCommon = new TreeViewCommon();
            String jsonStr = treeviewCommon.GetModule(roleName);
            return jsonStr;
        }


        public JsonResult getpageOrder(int? page, int? rows)
        {
            //page = page == null ? 1 : page;
            //rows = rows == null ? 1 : rows;
            //List<TSYS_USER> list = erp.TSYS_USER.OrderBy(a => a.PKID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

            //var json = new
            //{
            //    total = erp.TSYS_USER.Count(),
            //    rows = (from r in list
            //            select new TSYS_USER()
            //            {
            //                PKID = r.PKID,
            //                REAL_NAME = r.REAL_NAME,
            //                MOBILE = r.MOBILE,
            //                STAFF_CODE = r.STAFF_CODE
            //            }).ToArray()
            //};
            //return Json(json, JsonRequestBehavior.AllowGet);
            return null;
        }


      
      
    }
}