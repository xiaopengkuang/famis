﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using FAMIS.DAL;
using System.Web.Script.Serialization;
using FAMIS.Models;
using System.Runtime.Serialization.Json;
using FAMIS.DTO;
using FAMIS.DataConversion;
using System.Threading;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using FAMIS.Helper_Class;
using System.Collections;

namespace FAMIS.Controllers
{
    public class DepreciationController : Controller
    {
        //
        AssetDeatailsController Asset_Deatial = new AssetDeatailsController();
        CommonController comController = new CommonController();
        DictController dc = new DictController();
        FAMISDBTBModels db = new FAMISDBTBModels();
        StringBuilder result_tree_SearchTree = new StringBuilder();
        StringBuilder sb_tree_SearchTree = new StringBuilder();
        Excel_Helper excel = new Excel_Helper();
        Serial serial = new Serial();
        Print_Helper print=new Print_Helper("");
        public ActionResult depreciation()
        {
            return View();
        }

        [HttpPost]
       public string Dep_JT()//保存折旧信息
        {
            
             
            List<String> list = Get_Depreciation_Data();
            if (list.Count() != 0)
            {
                
                UpdateAsset_table(list);
            }
        

            return "success!";
        }
        public void UpdateAsset_table(List<String> list)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                int id = int.Parse(list[i].Split('o')[0]);
                float tatol_price = float.Parse(list[i].Split('o')[1].Split(',')[0]);
                float month_depreciation = float.Parse(list[i].Split('o')[1].Split(',')[1]);
                float tatol_depreciation = float.Parse(list[i].Split('o')[1].Split(',')[2]);
                float net_value = float.Parse(list[i].Split('o')[1].Split(',')[3]);
                var q = from p in db.tb_Asset
                        where p.ID == id
                        select p;
                foreach (var p in q)
                {
                    p.value = tatol_price;
                    p.depreciation_Month = month_depreciation;
                    p.depreciation_tatol = tatol_depreciation;
                    p.Net_value = net_value;
                }


            }
            db.SaveChanges();
        }
        public List<String> Get_Depreciation_Data()//得到折旧计算所需要的信息
        {
         

            string para = "";

            List<String> list = new List<String>();
            IEnumerable<tb_Asset> asset = from o in db.tb_Asset
                                          select o;
            if (asset.Count() > 0)
            {

                int temp = 0;
                foreach (tb_Asset o in asset)//读取折旧信息并拼写格式
                {



                    if (temp != asset.Count() - 1)
                        para += o.ID + "," + o.Method_depreciation + "," + o.unit_price + "," + o.amount + "," + o.YearService_month + "," + o.Net_residual_rate + "," + o.Time_Purchase + "," + o.depreciation_tatol + "o";
                    else
                        para += o.ID + "," + o.Method_depreciation + "," + o.unit_price + "," + o.amount + "," + o.YearService_month + "," + o.Net_residual_rate + "," + o.Time_Purchase + "," + o.depreciation_tatol;
                    temp++;

                }
            }
            string[] Assetrows = para.Split('o');
            

            for (int i = 0; i < Assetrows.Length; i++)
            {
               
              
                string[] parameters = Assetrows[i].Split(',');
                if (parameters[1] == null || parameters[1]=="")
                    continue;
               ;
            
                switch (GetMethod_depreciation(int.Parse(parameters[1])))//目前只支持平均年限算法
                {

                    case SystemConfig.ZJ_Algorithm_PJNX:
                        {
                            

                            string data = Caculate_Depreciation_AverageYear(parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7]);

                            if (data == "")
                                continue;
                            else
                                list.Add(parameters[0] + "o" + data);

                            break;
                        }

                    default:
                        break;

                }

            }
            
            return list;
        }
        //保留小数点后两位

        //折旧计算
        public string Caculate_Depreciation_AverageYear(string unit, string amount, string year, string rate, string timepurchase, string Total_Depreciation_Amount)
        {


            if (timepurchase == null)
                return "";
            if (Total_Depreciation_Amount == "" || Total_Depreciation_Amount == null)
                Total_Depreciation_Amount = "0";
            DateTime now = DateTime.Now;
            int thismonth = now.Month;

            int purchasetime = int.Parse(timepurchase.Split('/')[1]);

            int month_passed = thismonth - purchasetime;

            string para = "";
            float Year_Depreciation_Rate = ((float)100 - float.Parse(rate)) / (float.Parse(year)/12)/100;//年折旧率
            float Month_Depreciation_Rate = Year_Depreciation_Rate / (float)12;
            float totalprice = float.Parse(amount) * float.Parse(unit);
            float Month_Depreciation_Amount = totalprice * Month_Depreciation_Rate;
            if (month_passed <= 0)
                return "";
            else
            {
                
                Total_Depreciation_Amount = (month_passed * Month_Depreciation_Amount).ToString();
                float net_value = totalprice - float.Parse(Total_Depreciation_Amount);
                para = totalprice.ToString() + "," + Month_Depreciation_Amount.ToString() + "," + Total_Depreciation_Amount.ToString()
                    + "," + net_value.ToString();
            }


            return para;
        }

        public string GetMethod_depreciation(int id)//获得折旧算法
        {
            IEnumerable<tb_dataDict_para> data = from o in db.tb_dataDict_para
                                                 where o.ID == id
                                                 select o;
            if (data.Count() > 0)
            {
                foreach (tb_dataDict_para o in data)
                {

                    return o.name_para;


                }
            }
            return "0";
        }

        [HttpPost]
        public JsonResult Load_Asset( int? page, int? rows, string JSdata)//加载资产折旧表
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            CommonConversion con=new CommonConversion();
            int? rrid = con.getRoleID();
            List<int?> DepIds = Asset_Deatial.IDs(rrid,"department");
            List<int?> ATIds = Asset_Deatial.IDs(rrid, "AssetType");
            int flagnum = int.Parse(JSdata);
            if (flagnum == 0)
                flagnum = 11000000;
            int name_flag = flagnum/1000000;
            string name_flag_string = "";
            int? item_id=flagnum%1000000;
              List<int?> Depart_Asset_Type_Id=new List<int?>();
              List<int?> AssetassettypeId = new List<int?>();
            
            
            IEnumerable<String> flags = from o in db.tb_dataDict
                                        where o.ID == name_flag
                                        select o.name_flag;
            foreach(string p in flags)
            {
                name_flag_string = p;
            }
            List<tb_Asset> list = db.tb_Asset.ToList();
            if (name_flag_string == SystemConfig.nameFlag_2_SYBM)
            {
                   Depart_Asset_Type_Id = comController.GetSonIDs_Department(item_id);
                   var data = from r in db.tb_Asset
                           join t in db.tb_AssetType on r.type_Asset equals t.ID into temp_t
                           from tt in temp_t.DefaultIfEmpty()
                           join D in db.tb_department on r.department_Using equals D.ID into temp_D
                           from DD in temp_D.DefaultIfEmpty()
                           join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID into temp_k
                           from kk in temp_k.DefaultIfEmpty()

                              where Depart_Asset_Type_Id.Contains(r.department_Using) && DepIds.Contains(r.department_Using) && ATIds.Contains(r.type_Asset) || item_id == 0 && DepIds.Contains(r.department_Using) && ATIds.Contains(r.type_Asset)
                              where r.flag == true
                              select new
                           {
                               ID = r.ID,
                               department_Using = DD.name_Department,
                               serial_number = r.serial_number,
                               name_Asset = tt.name_Asset_Type,
                               specification = r.specification,
                               unit_price = r.unit_price,
                               amount = r.amount,
                               Total_price = r.value,
                               depreciation_tatol = r.depreciation_tatol,
                               Method_depreciation = kk.name_para,
                               YearService_month = r.YearService_month,
                               Net_residual_rate = r.Net_residual_rate,
                               depreciation_Month = r.depreciation_Month,
                               Net_value = r.Net_value,
                               Time_Purchase = r.Time_Purchase,
                               Time_add = r.Time_add


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
            if (name_flag_string == SystemConfig.nameFlag_2_ZCLB)
            {
                  AssetassettypeId= comController.GetSonID_AsseType(item_id);

                var data = from r in db.tb_Asset
                           join t in db.tb_AssetType on r.type_Asset equals t.ID into temp_t
                           from tt in temp_t.DefaultIfEmpty()
                           join D in db.tb_department on r.department_Using equals D.ID into temp_D
                           from DD in temp_D.DefaultIfEmpty()
                           join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID into temp_k
                           from kk in temp_k.DefaultIfEmpty()

                           where AssetassettypeId.Contains(r.type_Asset) && DepIds.Contains(r.department_Using) && ATIds.Contains(r.type_Asset) || item_id == 0 && DepIds.Contains(r.department_Using) && ATIds.Contains(r.type_Asset)
                           where r.flag == true
                           select new
                           {
                               ID = r.ID,
                               department_Using = DD.name_Department,
                               serial_number = r.serial_number,
                               name_Asset = tt.name_Asset_Type,
                               specification = r.specification,
                               unit_price = r.unit_price,
                               amount = r.amount,
                               Total_price = r.value,
                               depreciation_tatol = r.depreciation_tatol,
                               Method_depreciation = kk.name_para,
                               YearService_month = r.YearService_month,
                               Net_residual_rate = r.Net_residual_rate,
                               depreciation_Month = r.depreciation_Month,
                               Net_value = r.Net_value,
                               Time_Purchase = r.Time_Purchase,
                               Time_add = r.Time_add


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
            return Null_dataGrid();

           

        }
        [HttpPost]
        public string Set_ErroFile(string Json)
        {

            Session["ErrorFile"] = Json;
            return "";
        }
        [HttpPost]
        public string SetEditID(string Json)
        {
            
            Session["EditID"] = Json;
            return "";
        }

        [HttpPost]
        public string Set_SearialNum(string Json)
        {

            Session["Searial"] = Json;
            return "";
        }
        [HttpPost]
        public string SetEditRow(string Json)
        {
            Session["EdtRow"] = Json;
            return "";
        }
          [HttpPost]
        public string SetPDsearialSession(string Json)
        {
            Session["Deatails_Searial"] = Json;
            return "";
        }
          [HttpPost]
         public string SetIsQueryied(string Json)
        {
            Session["IsQueried"] = Json;
            return "";
        }
          [HttpPost]
          public string GetIsQueryied(string Json)//检查用户是否进行过筛选查询
          {
              if (Session["IsQueried"] == null)
                  return "false";
             return Session["IsQueried"].ToString();
              
          }
          [HttpPost]
          public string GetEditID(string Json)//检查用户是否进行过筛选查询
          {
              if (Session["EditID"] == null)
                  return "false";
              return Session["EditID"].ToString();

          }
          [HttpPost]
          public string GetEditData(string Json)//检查用户是否进行过筛选查询
          {
              string type=Json.Split(',')[1];
              string ID=Json.Split(',')[0];
              string data = "";
              switch (type)
              {
                  case "PD":
                      {
                          var q = from o in db.tb_Asset_inventory
                                  where o.ID.ToString() == ID 
                                  select o;
                          foreach (var p in q)
                          {
                              
                              data += p.serial_number+","+p.date+","+ p.property + "," + p._operator + "," + p.ps;
                          }
                          return data; 
                      }
                  case "YH":
                      {
                          var q = from o in db.tb_user
                                  join r in db.tb_role on o.roleID_User equals r.ID  

                                  join d in db.tb_department on o.ID_DepartMent equals d.ID
                                  where o.ID.ToString() == ID where r.flag==true &&d.effective_Flag==true
                                  select new { 
                                   name=o.name_User,
                                   pwd=o.password_User,
                                   tname=o.true_Name,
                                   role=r.name,
                                   dpt=d.name_Department
                                  };
                          foreach (var p in q)
                          {

                              data += p.name + "," + p.pwd + "," + p.tname+","+p.role+","+p.dpt;
                          }
                          return data;
                      }
                  case "JS":
                      {
                          var q = from o in db.tb_role
                                  where o.ID.ToString() == ID
                                  select o;
                          foreach (var p in q)
                          {

                              data += p.name + "," + p.description;
                          }
                          return data;
                      }
              }
              return "false";

          }
         [HttpPost]
        public string SetCurrentRow(string Json)//用session记录一下当前用户所编辑的行
        {
           
            Session["CurrentRow"] = Json;
            return "";
        }
       
        
        [HttpPost]
        public ActionResult PDdelete(string ID)
        {

            var Model = "";
            var pdsearail=from o in db.tb_Asset_inventory
                          where o.serial_number==ID
                          select o;

            foreach(var p in pdsearail)
            {
                p.flag = false;
             
                var deatails = from o in db.tb_Asset_inventory_Details
                               where o.serial_number == p.serial_number
                               select o;

                foreach(var q in deatails)
                {
                    q.flag =false; 
                }
            }
            db.SaveChanges();

            return this.Json(Model);
        }
        [HttpPost]

        public String LoadPDstate()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var data =( from d in db.tb_dataDict
                       join dd in db.tb_dataDict_para on d.ID equals dd.ID_dataDict into temp_dd
                       from ddd in temp_dd.DefaultIfEmpty()
                       where d.name_dataDict==SystemConfig.Name_Dict_PDZT
                       select new {
                           ID = ddd.ID,
                           Name = ddd.name_para
                       
                       }).ToList();
            String json = jss.Serialize(data).ToString().Replace("\\", "");


            return json;

        }


       // [HttpPost]
        public string WX_Set_PD_Data(string pdserial,string asset_Serial,string num) {
            
           // string serialPD=Json.Split(',')[0];
           // string Asset_Serail = Json.Split(',')[1];
           // int number =int.Parse(Json.Split(',')[2]);
            string serialPD = pdserial;
            string Asset_Serail = asset_Serial;
            int number = int.Parse(num);
            string pdstate="";
            

                       var p = from o in db.tb_Asset_inventory_Details
                               where o.serial_number_Asset == Asset_Serail&&o.flag==true&&o.serial_number==serialPD
                               select o;

                       if (p.Count() > 0)
                       {
                           
                           //这里先禁掉，因为当时需求变为每个资产编号对应一个实物资产。
                           foreach (var q in p)
                           {
                               
                               if (q.amountOfInv + 1 - q.amountOfSys == 0)
                                   q.state = "持平";
                               if (q.amountOfInv + 1 - q.amountOfSys > 0)
                               {
                                   return "deny";
                                  // q.state = "盘盈";
                               }
                               if (q.amountOfInv + 1 - q.amountOfSys < 0)
                                   q.state = "盘亏";
                               q.difference = q.difference + 1;
                               q.amountOfInv = q.amountOfInv + 1;
                              
                           }

                       }
                       else
                       {
                           if (1 - number == 0)

                               pdstate = "持平";
                           if (1 - number < 0)
                               pdstate = "盘亏";
                           if (1 - number > 0)
                               pdstate = "盘赢";

                           var rule_tb = new tb_Asset_inventory_Details
                           {
                               serial_number = serialPD,

                               amountOfSys = number,
                               amountOfInv = 1,

                               state = pdstate,
                               difference = 1 - number,
                               serial_number_Asset = Asset_Serail,

                               flag = true
                           };

                           db.tb_Asset_inventory_Details.Add(rule_tb);
                       }
            db.SaveChanges();
            AddPDList(serialPD);
            ChangePDState(serialPD);
            return "success";
        }
        public void ChangePDState(string pdserial)
        {
            var q = from o in db.tb_Asset_inventory
                    

                    where o.serial_number==pdserial&&o.flag==true
                    select o;
            foreach (var p in q)
            {
                if (p.state == this.GetStateID("未盘点"))

                    p.state =this.GetStateID("盘点中");
                
                
            }
            db.SaveChanges();
        }
      
 
        [HttpPost]
        public String LoadPD(string Json)
        {
            String json ="";
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string uid = Json.Split(',')[0];
            string Asset_Serial = Json.Split(',')[1];
            List<string> tu = new List<string>();
            var temp_deatail = from o in db.tb_Asset_inventory_Details
                               where o.serial_number_Asset == Asset_Serial && o.difference >= 0
                               select o;
            foreach (var k in temp_deatail)
            {
                tu.Add(k.serial_number);
            }

            var q = from o in db.tb_Asset_inventory
                    join d in db.tb_dataDict_para on o.state equals d.ID.ToString()
                   
                    where o._operator == uid && d.name_para != "已盘点" && o.flag == true && !tu.Contains(o.serial_number)

                    orderby o.ID descending
                    select new
                    {

                        serial = o.serial_number,
                        ps = o.ps
                    };
            json = jss.Serialize(q).ToString().Replace("\\", "");

            return json;
        
        }
        [HttpPost]
        public string  WX_LoadPD(string openid)
        {
            string json = "";
            int temp=0;
         

           var q = from o in db.tb_Asset_inventory
                    join d in db.tb_dataDict_para on o.state equals d.ID.ToString()
                    join u in db.tb_user on o._operator equals  u.ID.ToString()
                    where u.openid_WX == openid && d.name_para != "已盘点" && o.flag == true 

                    orderby o.ID descending
                    select new
                    {

                        serial = o.serial_number,
                        ps = o.ps
                    };
            foreach (var p in q)
            {
                if (temp != q.Count() - 1)
                    json += p.serial + "," + p.ps + SystemConfig.NullString_Replace;
                else
                    json += p.serial + "," + p.ps;
            }

            return json;

        }
        [HttpPost]
        public string AddDP(string JSdata)//添加盘点单
        {
            ArrayList mysearial = serial.ReturnNewSearial("PD", 1);//生成一个盘点单
            Session["CurrentRow"] = 0;
            int id = 0;
            if (JSdata.Split(',')[0] == "update")
                id = int.Parse(Session["EditID"].ToString());
            try
            {
                id = int.Parse(JSdata.Split(',')[0]);
            }
            catch (Exception e)
            {
               
            }
            
            string oper= JSdata.Split(',')[1];
            string ps = JSdata.Split(',')[2];
            DateTime pddate = DateTime.Parse(JSdata.Split(',')[3].ToString());
            string type = JSdata.Split(',')[4];

            try
            {
             int temp=int.Parse(oper);
               
            }

            catch (Exception e)//如果不是ID
            {
                var qq = from o in db.tb_user
                         where o.true_Name == oper
                         select o;
                foreach (var p in qq)
                {
                    oper = p.ID.ToString();
                }
            };

            /* var q = from p in mydb.tb_Menu
              where p.Role_ID == Roleid
              select p;*/
            string wpdID = "";
            var q = from o in db.tb_Asset_inventory
                    where o.ID == id
                    select o;
            var GetWPD = from o in db.tb_dataDict_para
                         where o.name_para == "未盘点"
                         select o;
            foreach (var p in GetWPD)
            {
                wpdID = p.ID.ToString();
            }
                       
            if (q.Count() == 0)
            {
                var rule_tb = new tb_Asset_inventory
                {
                     serial_number=mysearial[0].ToString(),
                     _operator = oper,
                     state=wpdID,
                     date=pddate,
                     property=type,
                     date_Create=DateTime.Now,
                     ps= ps,
                     flag=true

                };
                db.tb_Asset_inventory.Add(rule_tb);
                db.SaveChanges();
            }
            else
                foreach (var p in q)
                {
                    p.date = pddate;
                    p._operator = oper;
                    p.ps = ps;
                    p.property = type;
                }
             db.SaveChanges();


            return "";
          
        }
        [HttpPost]
        public JsonResult Load_Inventory()
        {
            List<tb_Asset_inventory> list = db.tb_Asset_inventory.ToList();
            var json = new
            {
                total = list.Count(),

                rows = (from r in db.tb_Asset_inventory

                        select new
                        {
                            ID = r.ID,
                            serial_number = r.serial_number,
                            date = r.date,
                            _operator=r._operator,
                            amountOfSys = r.amountOfSys,
                            amountOfInv = r.amountOfInv,
                            difference = r.difference,
                            property = r.property,
                            state = r.state,
                            date_Create = r.date_Create,
                            ps = r.ps

                        }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);

        }

        public void TestWrite(string text)
        {
            StreamWriter sw = new StreamWriter("D:\\201608.txt", true);
            sw.WriteLine(text);
            sw.Close();
        }
        [HttpPost]
        public JsonResult Load_Inventory_details(int? page,int? rows, string JSdata)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            List<tb_Asset_inventory_Details> list = db.tb_Asset_inventory_Details.ToList();
           var data=   from r in db.tb_Asset_inventory_Details
                        join a in db.tb_Asset on r.serial_number_Asset equals a.serial_number into temp_a
                        from aa in temp_a.DefaultIfEmpty()

                        join t in db.tb_AssetType on aa.type_Asset equals t.ID into temp_t

                        from tt in temp_t.DefaultIfEmpty()

                        join d in db.tb_dataDict_para on aa.measurement equals d.ID into temp_d
                        from dd in temp_d.DefaultIfEmpty()
                        join j in db.tb_dataDict_para on aa.addressCF equals j.ID into temp_j
                        from jj in temp_j.DefaultIfEmpty()
                        join p in db.tb_department on aa.department_Using equals p.ID into temp_p
                        from pp in temp_p.DefaultIfEmpty()
                        join u in db.tb_user on aa.Owener equals u.ID into temp_u
                        from uu in temp_u.DefaultIfEmpty()
                        join s in db.tb_dataDict_para on aa.state_asset equals s.ID into temp_s
                        from ss in temp_s.DefaultIfEmpty()
                        join sp in db.tb_supplier on aa.supplierID equals sp.ID into temp_sp
                        from ssp in temp_sp.DefaultIfEmpty()
                        where r.serial_number == JSdata&&r.flag==true&&aa.flag==true
                        select new
                        {
                            ID = r.ID,
                            serial_number = r.serial_number,
                            state = r.state,
                            amountOfSys = r.amountOfSys,
                            amountOfInv = r.amountOfInv,
                            difference = r.difference,
                            serial_number_Asset = r.serial_number_Asset,
                            type_Asset = tt.name_Asset_Type,
                            name_Asset = aa.name_Asset,
                            specification = aa.specification,
                            measurement = dd.name_para,
                            unit_price = aa.unit_price,
                            amount = aa.amount,
                            Total_price = aa.value,
                            department_using = pp.name_Department,
                            address = jj.name_para,
                            owener = uu.true_Name,
                            state_asset = ss.name_para,
                            supllier = ssp.name_supplier


                        };
          // TestWrite("我的个数： " + data.Count());
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
       
        public void AddPDList(string pdsearial)
        {
            int? sys = 0;
            int? ivt = 0;
            int? def = 0;
            var q = from o in db.tb_Asset_inventory_Details
                    where o.serial_number == pdsearial&&o.flag==true
                    select o;
            foreach (var p in q)
            {
                sys += p.amountOfSys;
                ivt += p.amountOfInv;
                def += p.difference;
            }

            var z = from o in db.tb_Asset_inventory
                    where o.serial_number.ToString() == pdsearial&&o.flag==true
                    select o;
            foreach (var p in z)
            {
                p.amountOfSys = sys;
                p.amountOfInv = ivt;
                p.difference = def;
            }
            db.SaveChanges();


        }
        public bool Upadate_Inventory_Deatails(string serial, string para1, string para2)
        {
            if(!para1.Contains("ZC") && !para2.Contains("ZC")) 

                return false;
            
            string searailnum=Session["Deatails_Searial"].ToString();
            string Asset_serial = "";
            string inventory_amount = "";
            if (para1.Contains("ZC") || para2.Contains("YH"))
            {
                Asset_serial = para1;
                inventory_amount = para2;

            }
            else
            {
                Asset_serial = para2;
                inventory_amount = para1;
            }
            var q = from p in db.tb_Asset_inventory_Details
                    where p.serial_number == serial && p.serial_number_Asset == Asset_serial
                    select p;
            foreach (var p in q)
            {
                try
                {
                    p.amountOfInv = int.Parse(inventory_amount);
                    p.difference = int.Parse(inventory_amount) - p.amountOfSys;
                }
                catch  
                {
                    return false;
                  
                }
                if (int.Parse(inventory_amount) - p.amountOfSys < 0)
                    p.state = "盘亏";
                if (int.Parse(inventory_amount) - p.amountOfSys > 0)
                    p.state = "盘盈";
                if (int.Parse(inventory_amount) - p.amountOfSys==0)
                    p.state = "持平";
            }

            
            db.SaveChanges();
            AddPDList(searailnum);
            return true;

        }
        [HttpPost]
        public string ChangeState(string JSdata)
        {
            string searial = Session["Deatails_Searial"].ToString();
            var q = from o in db.tb_Asset_inventory
                    where searial == o.serial_number
                    select o;
            foreach (var p in q)
            { 
                if(JSdata=="2")

             p.state=GetStateID("盘点中");
                if (JSdata == "3")
                    p.state = GetStateID("已盘点");          
            }
            db.SaveChanges();
            return "";
        
        }
      public string GetStateID(string state)
      {
        var q=from o in db.tb_dataDict_para
              where o.name_para==state
              select o;
          foreach(var p in q)
              return p.ID.ToString();
          return "";
      }
         [HttpPost]
        public string GetForm()
        {
            
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)           
            {
                foreach (string str in FileCollect)
                {

                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                   // Response.Write("<script>alert('"+FileSave.FileName+"')</script>");
                    if (FileSave.FileName == "")
                    {
                        Session["ErrorFile"] = "nofile";
                        Response.Redirect("/Verify/AddExcel");

                        return "no";

                    }
                    else
                    {
                        Session["ErrorFile"] = "FileUploaded";

                        string imgName = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string imgPath =  "/"+imgName + FileSave.FileName;  
                        //通过此对象获取文件名
                        string AbsolutePath = Server.MapPath(imgPath);
                      
                        if (!AbsolutePath.Contains(".xls") && !AbsolutePath.Contains("csv"))
                        {
                            Session["ErrorFile"] = "wrongfile";
                            Response.Redirect("/Verify/AddExcel");
                            return "no";
                        }
                        
                        FileSave.SaveAs(AbsolutePath);
                        //将上传的东西保存
                        bool is_right=ReadExcel(Session["Deatails_Searial"].ToString(), AbsolutePath);
                        //删除
                        excel.FileDelete(AbsolutePath);
                        if (is_right)

                            Response.Redirect("/Verify/AddExcel");
                        else
                        {
                            Session["ErrorFile"] = "wrongcode";
                            Response.Redirect("/Verify/AddExcel");
                            return "no";
                        }

                        
                    }
                   
                    
                }
            }
           
            return "盘点数据提交成功！";
        }
         
        public bool ReadExcel(string pd,string path)
        {
           
            
           
           
            string temp = "";
            DataTable dt = excel.ImportExcelFile(path);
            if (dt == null)
            {
                return false;
            }
             for(int i = 0 ; i < dt.Rows.Count ; i++) 
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                   
                    if (j != dt.Columns.Count-1)
                    temp = dt.Rows[i][j].ToString();
                    else
                    {

                        bool is_right_uploaded = Upadate_Inventory_Deatails(pd, temp, dt.Rows[i][j].ToString());
                        if (!is_right_uploaded)
                        
                            return false;
                    };
                   
                } 
              
            }

             return true;
        }
       public JsonResult Null_dataGrid()
       {
           var json = new
           {
               total = 0,
               rows = ""

           };
          return Json(json, JsonRequestBehavior.AllowGet);
       
       }

       public JsonResult Query_By_Condition(int? page, int ? rows,String JSON) 
       {
           page = page == null ? 1 : page;
           rows = rows == null ? 15 : rows;

           string [] temp= JSON.Split(',');
           List<tb_Asset_inventory> list = db.tb_Asset_inventory.ToList();
           DateTime BeginDate = Convert.ToDateTime("8888-12-12"+ " 00:00:00");
           DateTime EndDate = Convert.ToDateTime("0001-01-01" + " 23:59:59");

           int Effective_Query_Num = 0;
           for (int i = 0; i < temp.Length; i++)
           {
               if (temp[i]!=SystemConfig.NullString_Replace)
                  
                   Effective_Query_Num++;
           }
           string searial = temp[0];
           if (temp[1] != SystemConfig.NullString_Replace)
            BeginDate = DateTime.Parse(temp[1]);

           if (temp[2] != SystemConfig.NullString_Replace)
                 EndDate = DateTime.Parse(temp[2]);
           
           string PDstate = temp[3];
           string PDperson = temp[4];
          

           switch(Effective_Query_Num)
           {
               case 0:
                   {
                       
                        
                           

                           var data = from r in db.tb_Asset_inventory
                                      join p in db.tb_user on r._operator equals p.ID.ToString() into temp_p
                                      from pp in temp_p.DefaultIfEmpty()
                                      join d in db.tb_dataDict_para on r.state equals d.ID.ToString() into temp_d

                                      from dd in temp_d.DefaultIfEmpty()
                                      where r.flag==true
                                   select new
                                   {
                                       ID = r.ID,
                                       serial_number = r.serial_number,
                                       date = r.date,
                                       _operator = pp.true_Name,
                                       amountOfSys = r.amountOfSys,
                                       amountOfInv = r.amountOfInv,
                                       difference = r.difference,
                                       property = r.property,
                                       state = dd.name_para,
                                       date_Create = r.date_Create,
                                       ps = r.ps
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
               case 1:
                   {
                       var data = from r in db.tb_Asset_inventory
                                  join p in db.tb_user on r._operator equals p.ID.ToString()  into temp_p
                                  from pp in temp_p.DefaultIfEmpty()
                                  join d in db.tb_dataDict_para on r.state equals d.ID.ToString() into temp_d

                                  from dd in temp_d.DefaultIfEmpty()
                                  where r.serial_number.Contains(searial) && r.flag == true || r.date >= BeginDate && r.flag == true || r.date <= EndDate && r.flag == true || r.state == PDstate && r.flag == true || pp.ID.ToString()== PDperson && r.flag == true
                                  select new
                                  {
                                      ID = r.ID,
                                      serial_number = r.serial_number,
                                      date = r.date,
                                      _operator = pp.true_Name,
                                      amountOfSys = r.amountOfSys,
                                      amountOfInv = r.amountOfInv,
                                      difference = r.difference,
                                      property = r.property,
                                      state = dd.name_para,
                                      date_Create = r.date_Create,
                                      ps = r.ps + " "

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
               case 2:
                   {
                      var data=from r in db.tb_Asset_inventory
                               join p in db.tb_user on r._operator equals p.ID.ToString() into temp_p
                               from pp in temp_p.DefaultIfEmpty()
                               join d in db.tb_dataDict_para on r.state equals d.ID.ToString() into temp_d

                               from dd in temp_d.DefaultIfEmpty()
                                   where (r.state==PDstate&&pp.ID.ToString()==PDperson&&r.flag==true)
                                   || (r.state == PDstate && r.serial_number.Contains(searial) && r.flag == true)
                                   || (r.state == PDstate && r.date >= BeginDate && r.flag == true)
                                   || (r.state == PDstate && r.date <= EndDate && r.flag == true)
                                   || (pp.ID.ToString() == PDperson && r.serial_number.Contains(searial) && r.flag == true)
                                   || (pp.ID.ToString() == PDperson && r.date >= BeginDate && r.flag == true)
                                   || (pp.ID.ToString() == PDperson && r.date <= EndDate && r.flag == true)
                                   || ( r.serial_number.Contains(searial)&&r.date >= BeginDate && r.flag == true)
                                   || (r.serial_number.Contains(searial) && r.date <= EndDate&& r.flag == true)
                                   || (r.date <= EndDate && r.date >= BeginDate && r.flag == true)
                                   select new
                                   {
                                       ID = r.ID,
                                       serial_number = r.serial_number,
                                       date = r.date,
                                       _operator = pp.true_Name,
                                       amountOfSys = r.amountOfSys,
                                       amountOfInv = r.amountOfInv,
                                       difference = r.difference,
                                       property = r.property,
                                       state = dd.name_para,
                                       date_Create = r.date_Create,
                                       ps = r.ps+" "

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
               case 3:
                 {
                    var data= from r in db.tb_Asset_inventory
                              join p in db.tb_user on r._operator equals p.ID.ToString() into temp_p
                              from pp in temp_p.DefaultIfEmpty()
                              join d in db.tb_dataDict_para on r.state equals d.ID.ToString() into temp_d

                              from dd in temp_d.DefaultIfEmpty()
                              where (r.serial_number.Contains(searial) && r.date <= EndDate && r.date >= BeginDate && r.flag == true)
                                   || ((pp.ID.ToString() == PDperson&&r.date <= EndDate && r.date >= BeginDate && r.flag == true)
                                   || (pp.ID.ToString() == PDperson&&r.serial_number.Contains(searial)&& r.date <= EndDate && r.flag == true)
                                   || (pp.ID.ToString() == PDperson&&r.serial_number.Contains(searial) && r.date >= BeginDate && r.flag == true)

                                   || (r.state==PDstate&&r.date <= EndDate && r.date >= BeginDate && r.flag == true)
                                   || (r.state==PDstate&&r.date <= EndDate && r.serial_number.Contains(searial) && r.flag == true)
                                   || (r.state==PDstate &&r.date >= BeginDate &&r.serial_number.Contains(searial)&& r.flag == true)

                                   || (r.state==PDstate&&r.date <= EndDate &&pp.ID.ToString()==PDperson&& r.flag == true)
                                   || (r.state == PDstate && r.date >= BeginDate && pp.ID.ToString() == PDperson && r.flag == true)
                                   || (r.state == PDstate && pp.ID.ToString() == PDperson && r.serial_number.Contains(searial) && r.flag == true))

                                 select new
                                 {
                                     ID = r.ID,
                                     serial_number = r.serial_number,
                                     date = r.date,
                                     _operator = pp.true_Name,
                                     amountOfSys = r.amountOfSys,
                                     amountOfInv = r.amountOfInv,
                                     difference = r.difference,
                                     property = r.property,
                                     state = dd.name_para,
                                     date_Create = r.date_Create,
                                     ps = r.ps

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
               case 4:
                 {
                    var data=from r in db.tb_Asset_inventory
                             join p in db.tb_user on r._operator equals p.ID.ToString() into temp_p
                             from pp in temp_p.DefaultIfEmpty()
                             join d in db.tb_dataDict_para on r.state equals d.ID.ToString() into temp_d

                             from dd in temp_d.DefaultIfEmpty()
                             where (r.state == PDstate && pp.ID.ToString() == PDperson && EndDate >= r.date && r.date >= BeginDate && r.flag == true)
                                 || (r.state == PDstate && pp.ID.ToString() == PDperson && EndDate >= r.date && searial.Contains(searial) && r.flag == true)
                                 || (r.state == PDstate && pp.ID.ToString() == PDperson && BeginDate <= r.date && r.serial_number.Contains(searial) && r.flag == true)
                                 || (EndDate >= r.date && pp.ID.ToString() == PDperson && BeginDate <= r.date && r.serial_number.Contains(searial) && r.flag == true)
                                  || (EndDate >= r.date && r.state == PDstate && BeginDate <= r.date && r.serial_number.Contains(searial) && r.flag == true)
                                 select new
                                 {
                                     ID = r.ID,
                                     serial_number = r.serial_number,
                                     date = r.date,
                                     _operator = pp.true_Name,
                                     amountOfSys = r.amountOfSys,
                                     amountOfInv = r.amountOfInv,
                                     difference = r.difference,
                                     property = r.property,
                                     state = dd.name_para,
                                     date_Create = r.date_Create,
                                     ps = r.ps

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
               case 5:
                 {
                    var data=from r in db.tb_Asset_inventory
                             join p in db.tb_user on r._operator equals p.ID.ToString() into temp_p
                             from pp in temp_p.DefaultIfEmpty()
                             join d in db.tb_dataDict_para on r.state equals d.ID.ToString() into temp_d

                             from dd in temp_d.DefaultIfEmpty()
                             where r.serial_number.Contains(searial) && BeginDate <= r.date && r.date <= EndDate && r.state == PDstate && pp.ID.ToString() == PDperson && r.flag == true
                                 select new
                                 {
                                     ID = r.ID,
                                     serial_number = r.serial_number,
                                     date = r.date,
                                     _operator =pp.true_Name,
                                     amountOfSys = r.amountOfSys,
                                     amountOfInv = r.amountOfInv,
                                     difference = r.difference,
                                     property = r.property,
                                     state = dd.name_para,
                                     date_Create = r.date_Create,
                                     ps = r.ps

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
             
                 
                
           }

         return  Null_dataGrid();

       
       }
       
      
    }
}