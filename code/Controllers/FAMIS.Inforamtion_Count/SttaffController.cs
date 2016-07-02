using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.DAL;
using FAMIS.DTO;
using FAMIS.ViewCommon;
using FAMIS.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Web.Mvc;
namespace FAMIS.Controllers.FAMIS.ASSET_TYPE
{
    public class SttaffController : Controller
    {

        private dto_Staff dto_db = new dto_Staff();
         
        private FAMISDBTBModels db = new FAMISDBTBModels();
        public ActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult RoleDelete(string ID)
        {
            
             
            var Model = db.tb_role.Find(int.Parse(ID));
            db.tb_role.Remove(Model);
            db.SaveChanges();

            return this.Json(Model);
        }
        [HttpPost]
        public ActionResult StaffDelete( string ID) {
           /* StreamWriter sw = new StreamWriter("D:\\staffdelete.txt");
            sw.Write(index);
            sw.Close();*/
            var Model = db.tb_staff.Find(int.Parse(ID));
            db.tb_staff.Remove(Model);
            db.SaveChanges();

            return this.Json(Model);
        
        }
        [HttpPost]
        public JsonResult getpageOrder(int? page, int? rows, int? role, int? tableType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;



            // List<tb_role> list = db.tb_role.OrderBy(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

            List<tb_staff> list = db.tb_staff.ToList();
            //List<tb_user> list = DB_Connecting.tb_user.ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new tb_staff
                        {
                            ID = r.ID,
                            ID_Staff=r.ID_Staff,
                            code_Departmen=r.code_Departmen,
                            sex=r.sex,
                            entry_Time=r.entry_Time,
                            phoneNumber=r.phoneNumber,
                            email=r.email,
                            effective_Flag=r.effective_Flag,
                            create_TIME=r.create_TIME,
                            invalid_TIME=r.invalid_TIME,
                            _operator=r._operator,

                            name = r.name,
                           

                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Display_Staff()
        {

            return View();
        }
      
        [HttpGet]
        /**
         * 加载增加方式
         * */
        public String load_Operator()
        {

            List<tb_user> list = db.tb_user.OrderBy(a => a.ID).ToList();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new tb_user()
                          {
                              ID = r.ID,
                              true_Name=r.true_Name
                          }).ToList();

            String json = jss.Serialize(result).ToString().Replace("\\", "");

            
            return json;
        }
       
        public void load_StaffID()
        {

            List<tb_user> list = db.tb_user.OrderBy(a => a.ID).ToList();
            var q = db.tb_user.Select(e => e.ID).Max();
            JavaScriptSerializer jss = new JavaScriptSerializer();

            
            
                Session["id"] = q+1;
             
            
        }
        public String load_Depart()
        {

            List<tb_department> list = db.tb_department.OrderBy(a => a.ID).ToList();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new tb_department()
                          {
                              ID = r.ID,
                              ID_Department = r.ID_Department
                          }).ToList();

            String json = jss.Serialize(result).ToString().Replace("\\", "");

            this.load_StaffID();
            return json;
        }

        [HttpPost]
        public ActionResult test()
        {
            StreamWriter sw = new StreamWriter("D:\\123456.txt");
            sw.Write("这是我！"+ "\r\n");
            sw.Close();
            return View();
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
        [HttpGet]
        public ActionResult AddStaff([Bind(Include = "ID_Staff,code_Departmen,sex,entry_Time,phoneNumber,email,effective_Flag,create_TIME,invalid_TIME,_operator,name")] tb_staff staff)
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
                db.tb_staff.Add(staff);
                db.SaveChanges();

            }
            
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