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

        ElementModel DB_Connecting = new ElementModel();
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

          

            List<tb_user> list = DB_Connecting.tb_user.OrderBy(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

         
            //List<tb_user> list = DB_Connecting.tb_user.ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new tb_user()
                        {
                            ID = r.ID,
                            name_User = r.name_User,
                            roleID_User = r.roleID_User,
                            time_LastLogined = r.time_LastLogined
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
            
        }


      
      
    }
}