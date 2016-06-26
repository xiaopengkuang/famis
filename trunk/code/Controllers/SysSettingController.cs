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
      
        private FAMISDBTBModels db = new FAMISDBTBModels();
       
        public ActionResult RoleManage()
        {
            return View();
        }
        public ActionResult SysConfig()
        {
            return View();
        }
        [HttpPost]
        
        public ActionResult AddRule([Bind(Include = "Name_SeriaType,Rule_Generate,serialNum_length")] tb_Rule_Generate rule)
        {
            /*StreamWriter sw = new StreamWriter("D:\\123456.txt");
            sw.Write(staff.ID_Staff + "\r\n");
            sw.Write(staff.code_Departmen + "\r\n");
            sw.Write(staff.sex+ "\r\n");
            sw.Write(staff.entry_Time + "\r\n");
            sw.Write(staff.phoneNumber + "\r\n");
            sw.Write(staff.email + "\r\n");
            sw.Write(staff.effective_Flag+"\r\n");
            sw.Write(staff.create_TIME + "\r\n");
            sw.Write(staff.invalid_TIME + "\r\n");
            sw.Write(staff._operator + "\r\n");
            sw.Write(staff.name + "\r\n");
            sw.Close();*/

            if (ModelState.IsValid)
            {
                db.tb_Rule_Generate.Add(rule);

                db.SaveChanges();

            }

            return View();


        }
        [HttpPost]
        public ActionResult AddStaff([Bind(Include = "ID_Staff,code_Departmen,sex,entry_Time,phoneNumber,email,effective_Flag,create_TIME,invalid_TIME,_operator,name")] tb_staff staff)
        {

            if (ModelState.IsValid)
            {
                db.tb_staff.Add(staff);
                db.SaveChanges();

            }

            return View();


        }

       
        public ActionResult AddStaff()//([Bind(Include = "ID,name,description")] tb_role role)
        {
            /*if (ModelState.IsValid)
            {
                db.tb_role.Add(role);
                db.SaveChanges();
               
            }*/

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
           //String jsonStr = treeviewCommon.GetAssetType(roleName);
           String jsonStr = treeviewCommon.GetModule(roleName);
            return jsonStr;
        }
       [HttpGet]
       public String loadATByRole(String roleName)
       {


           TreeViewCommon treeviewCommon = new TreeViewCommon();
           String jsonStr = treeviewCommon.GetAssetType(roleName);
           //String jsonStr = treeviewCommon.GetModule(roleName);
           return jsonStr;
       }
       [HttpGet]
       public String loadDPByRole(String roleName)
       {


           TreeViewCommon treeviewCommon = new TreeViewCommon();
           String jsonStr = treeviewCommon.GetDepartment(roleName);
           //String jsonStr = treeviewCommon.GetModule(roleName);
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
            
            var Model = db.tb_role.Find(int.Parse(ID));
            db.tb_role.Remove(Model);
            db.SaveChanges();
            
            return this.Json(Model);
        }
        [HttpPost]
        public ActionResult UserDelete(string ID)
        {
             
            var Model = db.tb_user.Find(int.Parse(ID));
            db.tb_user.Remove(Model);
            db.SaveChanges();

            return this.Json(Model);
        }
        
       
        [HttpPost]
        public JsonResult getUser(int? page, int? rows, int? role, int? tableType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;



            // List<tb_role> list = db.tb_role.OrderBy(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

            List<tb_user> list = db.tb_user.ToList();
            //List<tb_user> list = DB_Connecting.tb_user.ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new tb_user()
                        {
                             ID=r.ID,
                            name_User = r.name_User,
                            password_User=r.password_User,
                            true_Name=r.true_Name,
                            roleID_User = r.roleID_User

                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);

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