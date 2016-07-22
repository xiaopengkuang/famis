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
using FAMIS.DataConversion;
namespace FAMIS.Controllers.FAMIS.System_setup
{
    public class RuleController : Controller
    {
        // GET: Rule_
        //private  string serial="";
        private FAMISDBTBModels mydb = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
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
        public String GetUserID()
        {

            List<tb_user> list = mydb.tb_user.OrderBy(a => a.ID).ToList();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new tb_user()
                          {
                              ID = r.ID,
                              true_Name = r.true_Name
                          }).ToList();

            String json = jss.Serialize(result).ToString().Replace("\\", "");


            return json;
        }
        [HttpPost]
        public String GetDepID()
        {

            List<tb_department> list = mydb.tb_department.OrderBy(a => a.ID).ToList();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new tb_role()
                          {
                              ID = r.ID,
                              name = r.name_Department
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
            IEnumerable<tb_role_authorization> role_au = from o in mydb.tb_role_authorization
                                                         where o.role_ID == Roleid
                                                          && o.type == "menu"
                                                         select o;
            if (role_au.Count() > 0)
            {
                foreach (tb_role_authorization o in role_au)
                    mydb.tb_role_authorization.Remove(o);

            }
            mydb.SaveChanges();
            if (JSON.Contains('o'))
            {

                for (int i = 1; i < Rightdetail.Count(); i++)
                {


                    int Stored_ID = int.Parse(Rightdetail[i]);
                   
                
                    var role_au_tb = new tb_role_authorization
                    {
                        role_ID = Roleid,
                        type = "menu",
                        Right_ID = Stored_ID,
                        flag = true
                    };
                    mydb.tb_role_authorization.Add(role_au_tb);
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
            IEnumerable<tb_role_authorization> role_au = from o in mydb.tb_role_authorization
                                          where o.role_ID == Roleid
                                           && o.type=="AssetType"
                                          select o;
            if (role_au.Count() > 0)
            {
                foreach (tb_role_authorization o in role_au)
                    mydb.tb_role_authorization.Remove(o);

            }
            mydb.SaveChanges();
            if (JSON.Contains('o'))
            {

                for (int i = 1; i < Rightdetail.Count(); i++)
                {
                    string[] name_id = Rightdetail[i].Split(',');
                    string name = name_id[0];
                    string id = name_id[1];
                    int Stored_ID = 0;
                    bool mflag = false;
                    IEnumerable<tb_AssetType> Assettype = from f in mydb.tb_AssetType
                                                            where f.ID.ToString() == id
                                                            select f;
                    if (Assettype.Count() > 0)
                    {
                        foreach (tb_AssetType q in Assettype)
                        {
                            mflag = (bool)q.flag;
                            Stored_ID = q.ID;
                        }
                    }
                    var role_au_tb = new tb_role_authorization
                    {
                        role_ID = Roleid,
                        type="AssetType",
                        Right_ID = Stored_ID,
                        flag=mflag
                    };
                    mydb.tb_role_authorization.Add(role_au_tb);
                }
                mydb.SaveChanges();
            }



            //return View();
            return this.Json(model);
        }

        [HttpPost]
        public ActionResult Add_DPRight(string JSON)
        {
            GetRule model = new GetRule();

            string[] Rightdetail = JSON.Split('o');

            int Roleid = int.Parse(Rightdetail[0]);
            

            IEnumerable<tb_role_authorization> role_au = from o in mydb.tb_role_authorization
                                                         where o.role_ID == Roleid
                                                          && o.type == "department"
                                                         select o;
            if (role_au.Count() > 0)
            {
                foreach (tb_role_authorization o in role_au)
                    mydb.tb_role_authorization.Remove(o);

            }
            mydb.SaveChanges();
            if (JSON.Contains('o'))
            {

                for (int i = 1; i < Rightdetail.Count(); i++)
                {
                    string[] name_id = Rightdetail[i].Split(',');
                    string name = name_id[0];
                    string id = name_id[1];
                    int Stored_ID = 0;
                    bool mflag = false;
                    IEnumerable<tb_department> department= from f in mydb.tb_department
                                   where f.ID.ToString() == id
                                   select f;
                    if (department.Count() > 0)
                    {
                        foreach (tb_department q in department)
                        {
                            mflag = (bool)q.effective_Flag;
                            Stored_ID = q.ID;
                        }
                    }
                    var role_au_tb = new tb_role_authorization
                    {
                        role_ID = Roleid,
                        type="department",
                        Right_ID = Stored_ID,
                        flag=mflag
                    };
                    mydb.tb_role_authorization.Add(role_au_tb);
                }
                mydb.SaveChanges();
            }



            //return View();
            return this.Json(model);
        }
        [HttpPost]
        public string AddUser(string JSdata)
        {
            GetRule model = new GetRule();
           
            int id = 0;
            try
            {
                id = int.Parse(JSdata.Split(',')[0]);
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            int rid=0;
            int did=0; 
             string ridtemp=JSdata.Split(',')[4];
             string deptemp = JSdata.Split(',')[5];
            string name = JSdata.Split(',')[1];
            string pwd = JSdata.Split(',')[2];
            string tname = JSdata.Split(',')[3];
            string isText =JSdata.Split(',')[6];
            IEnumerable<int> ri = from o in mydb.tb_role
                     where o.name == ridtemp
                     select o.ID;
            foreach (int roids in ri)
            {
                rid = roids;
            }

            if (isText == "True")
            {
                var z = from o in mydb.tb_department
                        where o.name_Department == deptemp
                        select o.ID;
                foreach (int a in z)
                {
                    did = a;
                }
            }
            else
            {
                did = int.Parse(deptemp);
            }
            var q = from o in mydb.tb_user
                                          where o.ID == id
                                          select o;

            if (q.Count() == 0)
            {
                var validate = from o in mydb.tb_user
                               where o.name_User == name
                               select o;
                if (validate.Count() > 0)
                    return "name_exist";
                var rule_tb = new tb_user
                {
                    name_User = name,
                    password_User = pwd,
                    true_Name = tname,
                    flag=true,
                    time_LastLogined = DateTime.Now,
                    roleID_User=rid,
                    ID_DepartMent=did
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
                p.ID_DepartMent = did;
            }
            mydb.SaveChanges();


            //return View();
            return "";
        }
       
        [HttpPost]
        public string AddRole(string JSdata)
        {
            GetRule model = new GetRule();
            bool? flag=false;
            int id = 0;
            try
            {
                id = int.Parse(JSdata.Split(',')[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
                var hasexist = from o in mydb.tb_role
                               where o.name == name
                               select o;
                if (hasexist.Count() > 0)
                    return "has_exist";
                var rule_tb = new tb_role
                {
                    name = name,
                    description  = des,
                    flag=true,
                    isSuperUser=false
                };
                mydb.tb_role.Add(rule_tb);
                mydb.SaveChanges();
            }
            else
                foreach (var p in q)
                {
                    p.name = name;
                    p.description = des;
                   
                    flag = p.isSuperUser;
                }
            if (!(bool)flag || commonConversion.isSuperUser((commonConversion.getRoleID())))
                mydb.SaveChanges();

            else
                return "Supper";

            return "Saved";
           
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
             IEnumerable<tb_role_authorization> role_au = from o in mydb.tb_role_authorization
                                          where o.role_ID == rid
                                          && o.type=="menu"
                                          select o;
             if (role_au.Count() > 0)
                 {
                     int temp=0;
                     foreach (tb_role_authorization o in role_au)
                     {
                         IEnumerable<tb_Menu>  me = from a in mydb.tb_Menu
                                                           where a.ID == o.Right_ID && a.isleafnode==true
                                                           select a;
                         foreach (tb_Menu tb in me)
                         {
                             if (temp != role_au.Count() - 1)

                                 json += tb.ID+ ",";
                             else
                                 json += tb.ID;
                             temp++;
                         
                         }
                       

                     }
                  }
           
             return json;
        
        }
        [HttpPost]
        public String Get_ATSelected_Url(string JSON)
        {
           // StreamWriter sw = new StreamWriter("D:\\jjjj.txt", true);
            
            String json = "";
            int rid = int.Parse(JSON);
            IEnumerable<tb_role_authorization> role_au= from o in mydb.tb_role_authorization
                                          where o.role_ID== rid 
                                         && o.type=="AssetType"
                                          select o;
            if (role_au.Count() > 0)
            {
                int temp = 0;
                foreach (tb_role_authorization o in role_au)
                {
                    IEnumerable<tb_AssetType> asset = from a in mydb.tb_AssetType
                                                      where a.ID == o.Right_ID
                                                      select a;
                    foreach (tb_AssetType at in asset)
                    {
                        if (temp != role_au.Count() - 1)
                            json += at.ID+ ",";
                        else
                            json += at.ID;
                        temp++;
                    }
                   

                }
            }
           GetRule model = new GetRule();
            model.QL_serial = json;
           
            return json;

        }
        [HttpPost]
        public String Get_DPSelected_Url(string JSON)
        {
            // StreamWriter sw = new StreamWriter("D:\\jjjj.txt", true);

            String json = "";
            int rid = int.Parse(JSON);
            IEnumerable<tb_role_authorization> role_au = from o in mydb.tb_role_authorization
                                                         where o.role_ID == rid
                                                        && o.type=="department"
                                                         select o;
            if (role_au.Count() > 0)
            {
                int temp = 0;
                foreach (tb_role_authorization o in role_au)
                {
                    IEnumerable<tb_department> depart = from a in mydb.tb_department
                                                        where a.ID == o.Right_ID
                                                      select a;
                    foreach (tb_department d in depart)
                    {

                        if (temp != role_au.Count() - 1)
                            json += d.ID + ",";
                        else
                            json += d.ID;
                        temp++;
                    }
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