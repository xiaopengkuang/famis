using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.ViewCommon;
using FAMIS.DAL;
using FAMIS.Models;
using System.IO;
namespace FAMIS.Controllers
{
    public class SysSettingController : Controller
    {
        // GET: SysSetting
        private ElementModel db = new ElementModel();
        public ActionResult RoleManage()
        {
            return View();
        }
        public ActionResult SysConfig()
        {
            return View();
        }
        
        public ActionResult AddRole()//([Bind(Include = "ID,name,description")] tb_role role)
        {
            /*if (ModelState.IsValid)
            {
                db.tb_role.Add(role);
                db.SaveChanges();
               
            }*/

            return View();
          
          
        }

       [HttpGet]
        public String loadSearchTreeByRole(String roleName)
        {


            TreeViewCommon treeviewCommon = new TreeViewCommon();
            String jsonStr = treeviewCommon.GetModule(roleName);
            return jsonStr;
        }
        [HttpPost]
        public ActionResult AddRole([Bind(Include = "name,description")] tb_role role)
        {
            if (ModelState.IsValid)
            {
                db.tb_role.Add(role);
                db.SaveChanges();
               
            }

            return View();


        }
        [HttpPost]
        public ActionResult RoleDelete(string ID) {
            StreamWriter sw = new StreamWriter("D:\\2.txt");
            sw.Write(ID);
            sw.Close();
            var Model = db.tb_role.Find(int.Parse(ID));
            db.tb_role.Remove(Model);
            db.SaveChanges();
            
            return this.Json(Model);
        }

       [HttpPost]
        public JsonResult getpageOrder(int? page, int? rows, int? role, int? tableType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;



           // List<tb_role> list = db.tb_role.OrderBy(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

            List<tb_role> list = db.tb_role.ToList();
            //List<tb_user> list = DB_Connecting.tb_user.ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new tb_role()
                        {
                            ID = r.ID,
                            name = r.name,
                            description = r.description
                            
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UserManage()
        {
            return View();
        }
        protected override void HandleUnknownAction(string actionName)
        {

            try
            {

                this.View(actionName).ExecuteResult(this.ControllerContext);

            }
            catch (InvalidOperationException ieox)
            {

                ViewData["error"] = "Unknown Action: \"" + Server.HtmlEncode(actionName) + "\"";

                ViewData["exMessage"] = ieox.Message;

                this.View("Error").ExecuteResult(this.ControllerContext);

            }

        }
    }
}