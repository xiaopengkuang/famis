using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.ViewCommon;
using FAMIS.DAL;
using FAMIS.Models;

namespace FAMIS.Controllers
{
    public class AssetController : Controller
    {

        DAL_TBFamisModel DB_Connecting = new DAL_TBFamisModel();
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

         [HttpGet]
        public String loadSearchTreeByRole(String roleName)
        {


            TreeViewCommon treeviewCommon = new TreeViewCommon();
            String jsonStr = treeviewCommon.GetModule(roleName);
            return jsonStr;
        }

        [HttpGet]
        public JsonResult getpageOrder(int? page, int? rows, int? role,int? tableType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;



            List<tb_Asset> list = DB_Connecting.tb_Asset.OrderBy(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

         
            //List<tb_user> list = DB_Connecting.tb_user.ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new tb_Asset()
                        {
                            ID = r.ID,
                            serial_number = r.serial_number,
                            name_Asset = r.name_Asset,
                            type_Asset = r.type_Asset,
                            specification = r.specification,
                            unit_price = r.unit_price,
                            amount = r.amount,
                            department_Using = r.department_Using,
                            address = r.address,
                            state_asset = r.state_asset,
                            value = r.value,
                            supplierID = r.supplierID
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
            
        }


        public ActionResult AddAsset()
        {
            return View();
        }
      
      
    }
}