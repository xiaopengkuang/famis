using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.DAL;
using FAMIS.Models;

namespace FAMIS.Controllers
{
    public class DictController : Controller
    {


        FAMISDBTBModels DB_Connecting = new FAMISDBTBModels();
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

        [HttpGet]
        public JsonResult load_GYS_add()
        {

            List<tb_supplier> list = DB_Connecting.tb_supplier.OrderBy(a => a.ID).ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new tb_supplier()
                        {
                            name_supplier = r.name_supplier,
                            linkman = r.linkman,
                            address = r.address
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }



        public JsonResult load_ZJFS_add()
        {
            List<tb_supplier> list = DB_Connecting.tb_supplier.OrderBy(a => a.ID).ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new tb_supplier()
                        {
                            name_supplier = r.name_supplier,
                            linkman = r.linkman,
                            address = r.address
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}