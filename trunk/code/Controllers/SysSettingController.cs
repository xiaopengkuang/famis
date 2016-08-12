using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.ViewCommon;
using FAMIS.DAL;
using FAMIS.Models;
using System.IO;
using FAMIS.Helper_Class;
namespace FAMIS.Controllers
{
    public class SysSettingController : Controller
    {
        // GET: SysSetting
      
        private FAMISDBTBModels db = new FAMISDBTBModels();
        Print_Helper print = new Print_Helper("test");
        public ActionResult RoleManage()
        {
            return View();
        }
        public ActionResult Edit_User()
        {
            return View();
        }
        public ActionResult EditRole()
        {
            return View();
        }
        public ActionResult SysConfig()
        {
            return View();
        }
        public ActionResult AddUser()
        {
            return View();
        }
        public ActionResult BarcodeCreating()
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
            sw.Write(staff.email + "\r\n")
             * 
             * 
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
        public string GetFormerRule() { 
        string rule_Json="";
        int temp=0;
        IEnumerable<tb_Rule_Generate> ruletable = from m in db.tb_Rule_Generate
                                                  select m;

        foreach (tb_Rule_Generate rule_row in ruletable)
        {
            if(temp!=ruletable.Count()-1)
            rule_Json += rule_row.Name_SeriaType+","+rule_row.Rule_Generate + ","+rule_row.serialNum_length+"o";
            else
                rule_Json += rule_row.Name_SeriaType + "," + rule_row.Rule_Generate + "," + rule_row.serialNum_length;
            temp++;

        }
        return rule_Json;
        
        }

       
        [HttpPost]
        public string Getbase64(string Asset_ID)
        {
            return print.Base_64(Asset_ID);
        }
        //[HttpPost]
        //public ActionResult AddStaff([Bind(Include = "ID_Staff,code_Departmen,sex,entry_Time,phoneNumber,email,effective_Flag,create_TIME,invalid_TIME,_operator,name")] tb_staff staff)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        //db.tb_staff.Add(staff);
        //        db.SaveChanges();

        //    }

        //    return View();


        //}

       
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
          // String jsonStr = 
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
        public string RoleDelete(string ID) {

            var q = from o in db.tb_role
                    where o.ID.ToString() == ID
                    select o;
            foreach (var p in q)
            {
                if (!(bool)p.isSuperUser)
                {
                    p.flag = false;
                    p.name = p.name + "(已删除)";

                }
                else
                    return "Supper";
            }
            db.SaveChanges();
            return "OK";
        }
        [HttpPost]
        public string UserDelete(string ID)
        {


            var q = from o in db.tb_user
                    where o.ID.ToString() == ID
                    select o;
            foreach (var p in q)
            {
                p.true_Name = p.true_Name + "(已删除)";

                   
                    p.flag = false;
           
            }
            db.SaveChanges();
            return "OK";
        }
        
       
        [HttpPost]
        public JsonResult getUser(int? page, int? rows, int? role, int? tableType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;



            // List<tb_role> list = db.tb_role.OrderBy(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();
 
            
                
                var data = from u in db.tb_user
                        join r in db.tb_role on u.roleID_User equals r.ID
                        join d in db.tb_department on u.ID_DepartMent equals d.ID
                        where u.flag==true&&r.flag==true&&d.effective_Flag==true
                        select new  
                        {
                             ID=u.ID,
                            name_User = u.name_User,
                            password_User=u.password_User,
                            true_Name=u.true_Name,
                            roleID_User = r.name,
                            ID_DepartMent=d.name_Department
                        };

            data = data.OrderByDescending(a => a.ID);
            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;

            var json = new
            {

                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                //rows = data.ToList().ToArray()
            };

            
            return Json(json, JsonRequestBehavior.AllowGet);

        }
       [HttpPost]
        public JsonResult getpageOrder(int? page, int? rows)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;



           

           
           
            
                var data =  from r in db.tb_role
                        where r.flag==true
                        select new  
                        {
                            ID = r.ID,
                            name = r.name,
                            description = r.description
                            
                        };
                data = data.OrderByDescending(a => a.ID);
                int skipindex = ((int)page - 1) * (int)rows;
                int rowsNeed = (int)rows;

                var json = new
                {

                    total = data.ToList().Count,
                    rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                    //rows = data.ToList().ToArray()
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