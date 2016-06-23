using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Helper_Class;
using System.Collections;
using System.IO;
using System.Web.Script.Serialization;
using FAMIS.DAL;
using FAMIS.Models;
namespace FAMIS.Controllers.FAMIS.System_setup
{
    public class RuleController : Controller
    {
        // GET: Rule_
        private  string serial="";
        private FAMISDBTBModels mydb = new FAMISDBTBModels();
       
        
        public class GetRule// 绑定页面规则数据
         {
            
            public string ZC_serial{ get; set; }
           
            public string YH_serial { get; set; }
            public string ZC_Type_serial { get; set; }
            public string YH_Type_serial { get; set; }
    
            public string LY_serial { get; set; }
            public string DB_serial { get; set; }
            public string WX_serial { get; set; }
            public string JC_serial { get; set; }
            public string GH_serial { get; set; }
            public string JS_serial { get; set; }
            public string QL_serial { get; set; }
            public string PD_serial { get; set; }

            public int ZC_Bits { get; set; }
            public int YH_Bits { get; set; }
            public int QL_Bits { get; set; }

            public int PD_Bits { get; set; }

            public int LY_Bits { get; set; }
            public int DB_Bits { get; set; }
            public int WX_Bits { get; set; }

            public int JC_Bits { get; set; }

            public int GH_Bits { get; set; }

            public int JS_Bits { get; set; }

           



        
         }
        
       
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult AddNewRule()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddNewRule(string JSON)//string ZC,string YH,string LY,string WX,string JC,string GH,string JS,string QL,string PD){
        {
            
           // StreamWriter sw = new StreamWriter("D:\\jjjj.txt",true);
            GetRule model = new GetRule();

           // sw.WriteLine(JSON);
            string[] ruledetail = JSON.Split('o');
            string rule_head = null;
            string rule_full = null;
            string rule_bit =null;
            //sw.WriteLine("长度： —->"+" "+ruledetail.Length);
            for (int i = 0; i < ruledetail.Length;i++ )
            {
                string rule = ruledetail[i];
                if (rule.Length < 7)
                {
                  //  sw.WriteLine(rule);
                    break;

                }
              //  sw.WriteLine(rule);
                string[] details = rule.Split(',');
                rule_head = details[0];
                rule_full = details[1];
                rule_bit = details[2];

                this.UpdateRule(rule_head, rule_full, rule_bit);
            }
            //sw.Close();
            return this.Json(model);
        }
    
        
        public void UpdateRule(string rule_head, string rule, string bit)
        {
            var q = from p in mydb.tb_Rule_Generate
                    where p.Name_SeriaType == rule_head
                    select p;
            if (q.Count() == 0)
            {
                var rule_tb = new tb_Rule_Generate
                {
                    Name_SeriaType = rule_head,
                    Rule_Generate = rule,
                    serialNum_length= int.Parse(bit)
                   
                };
                mydb.tb_Rule_Generate.Add(rule_tb);
                mydb.SaveChanges();
            }
            foreach (var p in q)
            {
                p.serialNum_length = int.Parse(bit);
                p.Rule_Generate = rule;
            }
            mydb.SaveChanges();

        }
        public String GetRolename(int rid) {
            String name = "";
            IEnumerable<tb_role> mymenu = from o in mydb.tb_role
                                          where o.ID == rid
               
                                          select o;
            if (mymenu.Count() > 0)
            {
                foreach (tb_role o in mymenu)
                     name=o.name;

            }
            return name;


        }
        [HttpPost]
        public String GetRoleID()
        {
         /*   String json = "";
            var q = from c in mydb.tb_role
                    select new
                    {
                        c.ID,
                        c.name
                    };
            IEnumerable<tb_role> role= from o in mydb.tb_role
                                          
                                          select o;
            if (role.Count() > 0)
            {
                int temp = 0;
                foreach (tb_role o in role)
                {

                    if (temp != role.Count() - 1)

                        json += o.ID + ","+o.name+"o";
                    else
                        json += o.ID+","+o.name;
                    temp++;

                }
            }
            
            GetRule model = new GetRule();
            model.QL_serial = json;
            return json;
            List<tb_role> list = mydb.tb_role.ToList();
            var json = new
            {
                
                total = list.Count(),
                rows = (from r in list
                        select new tb_role()
                        {

                           ID= r.ID,
                           name=r.name

                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);*/
            List<tb_role> list = mydb.tb_role.OrderBy(a => a.ID).ToList();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new tb_role()
                          {
                              ID = r.ID,
                              name = r.name
                          }).ToList();

            String json = jss.Serialize(result).ToString().Replace("\\", "");


            return json;
        }

        [HttpPost]
        public ActionResult Add_Right(string JSON)
        {
            GetRule model = new GetRule();
            
            string[] Rightdetail = JSON.Split('o');
            
            int Roleid = int.Parse(Rightdetail[0]);

            
                  /* var q = from p in mydb.tb_Menu
                    where p.Role_ID == Roleid
                    select p;*/
            IEnumerable<tb_Menu> mymenu = from o in mydb.tb_Menu
                                          where o.Role_ID == Roleid
                                          select o;
                  if (mymenu.Count() > 0)
                 {
                     foreach (tb_Menu o in mymenu)
                         mydb.tb_Menu.Remove(o);
                        
                  }
                  mydb.SaveChanges();
                  if (JSON.Contains('o'))
                  {

                      for (int i = 1; i < Rightdetail.Count(); i++)
                      {
                          string[] name_id = Rightdetail[i].Split(',');
                          string name = name_id[0];
                          string id = name_id[1];

                          var menue_tb = new tb_Menu
                          {
                              Role_ID = Roleid,
                              name_Menu = name,
                              ID__Menu = id

                          };
                          mydb.tb_Menu.Add(menue_tb);
                      }
                      mydb.SaveChanges();
                  }
            
           
            
            //return View();
            return this.Json(model);
        }

        [HttpPost]
        public ActionResult Add_ATRight(string JSON)
        {
            GetRule model = new GetRule();

            string[] Rightdetail = JSON.Split('o');

            int Roleid = int.Parse(Rightdetail[0]);


            /* var q = from p in mydb.tb_Menu
              where p.Role_ID == Roleid
              select p;*/
            IEnumerable<tb_AssetType> mymenu = from o in mydb.tb_AssetType
                                          where o.RoleID == Roleid
                                          select o;
            if (mymenu.Count() > 0)
            {
                foreach (tb_AssetType o in mymenu)
                    mydb.tb_AssetType.Remove(o);

            }
            mydb.SaveChanges();
            if (JSON.Contains('o'))
            {

                for (int i = 1; i < Rightdetail.Count(); i++)
                {
                    string[] name_id = Rightdetail[i].Split(',');
                    string name = name_id[0];
                    string id = name_id[1];

                    var menue_tb = new tb_AssetType
                    {
                        RoleID = Roleid,
                        name_Asset_Type = name,
                        orderID= id

                    };
                    mydb.tb_AssetType.Add(menue_tb);
                }
                mydb.SaveChanges();
            }



            //return View();
            return this.Json(model);
        }
        [HttpPost]
        public ActionResult AddUser(string JSdata)
        {
            GetRule model = new GetRule();
           
            int id = 0;
            try
            {
                id = int.Parse(JSdata.Split(',')[0]);
            }
            catch(Exception e) {
                ;
            }
            string name = JSdata.Split(',')[1];
            string pwd = JSdata.Split(',')[2];
            string tname = JSdata.Split(',')[3];
            int rid = int.Parse(JSdata.Split(',')[4]);

            /* var q = from p in mydb.tb_Menu
              where p.Role_ID == Roleid
              select p;*/
            var q = from o in mydb.tb_user
                                          where o.ID == id
                                          select o;

            if (q.Count() == 0)
            {
                var rule_tb = new tb_user
                {
                    name_User = name,
                    password_User = pwd,
                    true_Name = tname,
                    time_LastLogined = DateTime.Now,
                    roleID_User=rid

                };
                mydb.tb_user.Add(rule_tb);
                mydb.SaveChanges();
            }
            else
            foreach (var p in q)
            {
                p.name_User = name;
                p.password_User = pwd;
                p.true_Name = tname;
                p.time_LastLogined = DateTime.Now;
                p.roleID_User =rid;
            }
            mydb.SaveChanges();


            //return View();
            return this.Json(model);
        }
        [HttpPost]
        public ActionResult SaveStaff(string JSdata)
        {
            GetRule model = new GetRule();

            int id = 0;
            try
            {
                id = int.Parse(JSdata.Split(',')[0]);
            }
            catch (Exception e)
            {
                ;
            }
            string staffid = JSdata.Split(',')[1];
            string codedepart = JSdata.Split(',')[2];
            string sex = JSdata.Split(',')[3];
            DateTime entrytime =DateTime.Parse(JSdata.Split(',')[4]);
            string phone = JSdata.Split(',')[5];
            string email = JSdata.Split(',')[6];
            Boolean flag = Boolean.Parse(JSdata.Split(',')[7]);
            DateTime creattime = DateTime.Now;
            DateTime ivalidtime = DateTime.Now;//不懂什么意思先标记为Now
            string operator_ = JSdata.Split(',')[8];
            string name = JSdata.Split(',')[9];

            /* var q = from p in mydb.tb_Menu
              where p.Role_ID == Roleid
              select p;*/
            var q = from o in mydb.tb_staff
                    where o.ID == id
                    select o;

            if (q.Count() == 0)
            {
                var rule_tb = new tb_staff
                {
                    ID_Staff=staffid,
                    code_Departmen=codedepart,
                    sex=sex,
                    entry_Time=entrytime,
                    phoneNumber=phone,
                    email=email,
                    effective_Flag=flag,
                    create_TIME=creattime,
                    invalid_TIME=ivalidtime,
                    _operator=operator_,
                    name=name

                };
                mydb.tb_staff.Add(rule_tb);
                mydb.SaveChanges();
            }
            else
                foreach (var p in q)
                {
                     p.ID_Staff=staffid;
                   p.code_Departmen=codedepart;
                   p.sex=sex;
                    p.entry_Time=entrytime;
                    p.phoneNumber=phone;
                    p.email=email;
                   p.effective_Flag=flag;
                    p.create_TIME=creattime;
                    p.invalid_TIME=ivalidtime;
                    p._operator=operator_;
                    p.name = name;

                }
            mydb.SaveChanges();


            //return View();
            return this.Json(model);
        }
        [HttpPost]
        public ActionResult AddRole(string JSdata)
        {
            GetRule model = new GetRule();
            int id = 0;
            try
            {
                id = int.Parse(JSdata.Split(',')[0]);
            }
            catch (Exception e)
            {
                ;
            }
            string name = JSdata.Split(',')[1];
            string des = JSdata.Split(',')[2];
             
            

            /* var q = from p in mydb.tb_Menu
              where p.Role_ID == Roleid
              select p;*/
            var q = from o in mydb.tb_role
                    where o.ID == id
                    select o;

            if (q.Count() == 0)
            {
                var rule_tb = new tb_role
                {
                    name = name,
                    description  = des
                   

                };
                mydb.tb_role.Add(rule_tb);
                mydb.SaveChanges();
            }
            else
                foreach (var p in q)
                {
                    p.name = name;
                    p.description = des;
                    
                }
            mydb.SaveChanges();


            //return View();
            return this.Json(model);
        }
        [HttpPost]
        public ActionResult Jugdement(string JSON,string name)
        {
           int len=JSON.Length;
            GetRule model = new GetRule();
            if (JSON.Contains(name))
            {
                if (JSON.Contains("o")){
                 JSON= JSON.Replace("o" + name, "");
                    if(JSON.Length==len)
                        JSON = JSON.Replace(name+"o", "");
                    }
                else
                  JSON= JSON.Replace(name, "");
                model.YH_serial = JSON;
            }
            else
                if(JSON=="")
                    JSON += name;
            else
                JSON += "o" + name;
            model.YH_serial = JSON;
            return this.Json(model);
        }
        [HttpPost]
        public String Get_Selected_Url(string JSON)
        { 
            String json="";
            int rid = int.Parse(JSON);
             IEnumerable<tb_Menu> mymenu = from o in mydb.tb_Menu
                                          where o.Role_ID == rid
                                          select o;
             if (mymenu.Count()>0)
                 {
                     int temp=0; 
                     foreach (tb_Menu o in mymenu)
                     {
                        if(temp!=mymenu.Count()-1)
                        json+=o.ID__Menu+","; 
                         else
                            json+=o.ID__Menu;
                         temp++;

                     }
                  }
             GetRule model = new GetRule();
             model.QL_serial = json;
             return json;
        
        }
        [HttpPost]
        public String Get_ATSelected_Url(string JSON)
        {
            String json = "";
            int rid = int.Parse(JSON);
            IEnumerable<tb_AssetType> mymenu = from o in mydb.tb_AssetType
                                          where o.RoleID== rid
                                          select o;
            if (mymenu.Count() > 0)
            {
                int temp = 0;
                foreach (tb_AssetType o in mymenu)
                {
                    if (temp != mymenu.Count() - 1)
                        json += o.orderID + ",";
                    else
                        json += o.orderID;
                    temp++;

                }
            }
            GetRule model = new GetRule();
            model.QL_serial = json;
            return json;

        }
        [HttpPost]
        public ActionResult Index(string serial,string bit)
        {
            StreamWriter sw = new StreamWriter("D:\\1.txt",true);
            sw.WriteLine(serial + " asdsad " + bit);
            GetRule model = new GetRule();
            Serial srl = new Serial();
            ArrayList al;

            al = srl.Serial_View(serial, 1, int.Parse(bit));
          // al = srl.Serial_View(da.YH_serial, 1, da.YH_Bits);
            foreach (string c in al)
            {
                
                sw.Write(c);
                sw.Close();
               serial = c;
               model.YH_serial = c;
                
            }

            return this.Json(model);
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
                mydb.tb_Rule_Generate.Add(rule);
               
                mydb.SaveChanges();

            }

            return View();


        }

       
        /*public ActionResult Index(FormCollection fc)
        {

            ViewBag.LoginState = fc["ZC_serial"];
          

            return View();

        }*/
    }
}