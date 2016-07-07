using System;
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
using FAMIS.DataConversion;
namespace FAMIS.Controllers
{
    
    public class AssetDeatailsController : Controller
    {
        // DictController dc = new DictController();
        FAMISDBTBModels db = new FAMISDBTBModels();
        StringBuilder result_tree_SearchTree = new StringBuilder();
        StringBuilder sb_tree_SearchTree = new StringBuilder();
        Excel_Helper excel = new Excel_Helper();
        
        CommonConversion commonConversion = new DataConversion.CommonConversion();
        // GET: /AssetDeatails/
        public ActionResult Index()
        {
            return View();
        }

        public class GetAsset// 绑定页面规则数据
        {

            public string ZCBH{ get; set; }

            public string ZCXZ { get; set; }
            public string ZCLB { get; set; }
            public string ZCMC { get; set; }

            public string GGXH { get; set; }
            public string JLSW { get; set; }
            public string SZBM { get; set; }
            public string SYR { get; set; }
            public string CFDD { get; set; }
            public string ZJFS { get; set; }
            public string JZCL { get; set; }
            public string ZCDJ { get; set; }

            public string ZCSL { get; set; }
            public string ZCJZ { get; set; }
            public string YTZJ { get; set; }

            public string LJZJ { get; set; }

            public string  JZ { get; set; }
           //自定义属性不在其中！






        }
         [HttpPost]
        public string Get_Asset_Deatail_BySearial(string Json)
        {
            string json="";
           IEnumerable<String>  q = from a in db.tb_Asset
                     join t in db.tb_AssetType on a.type_Asset equals t.ID
                        join d in db.tb_dataDict_para on a.measurement equals d.ID
                        join j in db.tb_dataDict_para on a.addressCF equals j.ID
                        join p in db.tb_department on a.department_Using equals p.ID
                        join u in db.tb_user on a.Owener equals u.ID
                        join s in db.tb_dataDict_para on a.state_asset equals s.ID
                        join zj in db.tb_dataDict_para on a.Method_add equals zj.ID
                        join fs in db.tb_dataDict_para on a.Method_depreciation equals fs.ID
                        join sp in db.tb_supplier on a.supplierID equals sp.ID
                        where a.serial_number==Json
                        select a.serial_number+","+t.name_Asset_Type+","+t.name_Asset_Type+","+a.name_Asset+","+a.specification+
                        ","+d.name_para+","+p.name_Department+","+u.true_Name+","+ j.name_para+","+zj.name_para+","+sp.name_supplier+
                         ","+sp.linkman+","+sp.address+","+a.Time_Purchase+","+s.name_para+","+a.Time_add+","+a.YearService_month+","+
                        fs.name_para+","+a.Net_residual_rate+","+a.unit_price+","+a.amount+","+a.Total_price+","+a.depreciation_Month+","
                        +a.depreciation_tatol+","+a.Net_value;
            
             foreach(String a in q)
                 json=a;
             return json.ToString();    
        }
         [HttpPost]
         public string Add_InventDeatails(string Json)
         {
             string[] temp = Json.Split('o');
             string condiation = temp[0];
             string pdsearial = temp[1];
             int flagnum = int.Parse(condiation);
             int name_flag = flagnum / 1000000;
             string name_flag_string = "";
             int item_id = flagnum % 1000000;
             IEnumerable<String> flags = from o in db.tb_dataDict
                                         where o.ID == name_flag
                                         select o.name_flag;
             foreach (string p in flags)
             {
                 name_flag_string = p;
             }
            
             if (name_flag_string == SystemConfig.nameFlag_2_SYBM)
             {


                 IEnumerable<tb_Asset> q = from a in db.tb_Asset
                                           join t in db.tb_AssetType on a.type_Asset equals t.ID
                                           join d in db.tb_dataDict_para on a.measurement equals d.ID
                                           join j in db.tb_dataDict_para on a.addressCF equals j.ID
                                           join p in db.tb_department on a.department_Using equals p.ID
                                           join u in db.tb_user on a.Owener equals u.ID
                                           join e in db.tb_dataDict_para on a.state_asset equals e.ID
                                           join sp in db.tb_supplier on a.supplierID equals sp.ID
                                           where p.ID_Department == item_id
                                           select a;
                 foreach (tb_Asset asset in q)
                 {
                     var rule_tb = new tb_Asset_inventory_Details
                     {
                          serial_number = pdsearial,
                          state="盘亏",
                          amountOfSys=asset.amount,
                          amountOfInv=0,
                          difference=0-asset.amount,
                          serial_number_Asset=asset.serial_number

                     };

                     db.tb_Asset_inventory_Details.Add(rule_tb);
                    
                 
                 }
                 db.SaveChanges();         
                            
            }
             if (name_flag_string == SystemConfig.nameFlag_2_ZCLB)
             {
               

                 IEnumerable<tb_Asset> q = from a in db.tb_Asset
                                           join t in db.tb_AssetType on a.type_Asset equals t.ID
                                           join d in db.tb_dataDict_para on a.measurement equals d.ID
                                           join j in db.tb_dataDict_para on a.addressCF equals j.ID
                                           join p in db.tb_department on a.department_Using equals p.ID
                                           join u in db.tb_user on a.Owener equals u.ID
                                           join e in db.tb_dataDict_para on a.state_asset equals e.ID
                                           join sp in db.tb_supplier on a.supplierID equals sp.ID
                                           where t.orderID== item_id.ToString()
                                           select a;
                 foreach (tb_Asset asset in q)
                 {
                      
                     var rule_tb = new tb_Asset_inventory_Details
                     {
                         serial_number = pdsearial,
                         state = "盘亏",
                         amountOfSys = asset.amount,
                         amountOfInv = 0,
                         difference = 0 - asset.amount,
                         serial_number_Asset = asset.serial_number

                     };
                     db.tb_Asset_inventory_Details.Add(rule_tb);
                    

                 }
                

             }
             db.SaveChanges();
             if (name_flag_string == SystemConfig.nameFlag_2_ZCZT)
             {


                 IEnumerable<tb_Asset> q = from a in db.tb_Asset
                                           join t in db.tb_AssetType on a.type_Asset equals t.ID
                                           join d in db.tb_dataDict_para on a.measurement equals d.ID
                                           join j in db.tb_dataDict_para on a.addressCF equals j.ID
                                           join p in db.tb_department on a.department_Using equals p.ID
                                           join u in db.tb_user on a.Owener equals u.ID
                                           join e in db.tb_dataDict_para on a.state_asset equals e.ID
                                           join sp in db.tb_supplier on a.supplierID equals sp.ID
                                           where e.ID == item_id
                                           select a;
                 foreach (tb_Asset asset in q)
                 {
                     var rule_tb = new tb_Asset_inventory_Details
                     {
                         serial_number = pdsearial,
                         state = "盘亏",
                         amountOfSys = asset.amount,
                         amountOfInv = 0,
                         difference = 0 - asset.amount,
                         serial_number_Asset = asset.serial_number

                     };
                     db.tb_Asset_inventory_Details.Add(rule_tb);
                   

                 }
                

             }
              db.SaveChanges();
             AddPDList(pdsearial);//添加盘点明细汇总数据到盘点表。

             return "";
            
         
         }
         public void AddPDList(string pdsearial)
         {
             int? sys = 0;
             int? ivt = 0;
             int? def = 0;
             var q = from o in db.tb_Asset_inventory_Details
                     where o.serial_number == pdsearial
                     select o;
             foreach(var p in q)
             {
                 sys += p.amountOfSys;
                 ivt += p.amountOfInv;
                 def += p.difference;
             }
             
             var z = from o in db.tb_Asset_inventory
                     where o.serial_number.ToString() == pdsearial
                     select o;
             foreach (var p in z)
             {
                 p.amountOfSys = sys;
                 p.amountOfInv = ivt;
                 p.difference = def;
             }
             db.SaveChanges();

         
         }
       [HttpPost]
        public string Get_Serial_Deatails()
         {
             string json = "";
              
                 json = Session["Deatails_Searial"].ToString();
             

            return json;
        }
       [HttpPost]
       public string Get_Current_Row()
       {
           string json = "";
           try
           {
               json = Session["CurrentRow"].ToString();
           }
           catch 
           {
               return json;
           }
           return json;
       }
        [HttpPost]
        public JsonResult Load_Asset(string JSdata)
        {
            int flagnum = int.Parse(JSdata);
            int name_flag = flagnum / 1000000;
            string name_flag_string = "";
            int item_id = flagnum % 1000000;
            IEnumerable<String> flags = from o in db.tb_dataDict
                                        where o.ID == name_flag
                                        select o.name_flag;
            foreach (string p in flags)
            {
                name_flag_string = p;
            }
            List<tb_Asset> list = db.tb_Asset.ToList();
            if (name_flag_string == SystemConfig.nameFlag_2_SYBM)
            {
                var json = new
                {
                    total = list.Count(),
                    rows = (from a in db.tb_Asset
                            join t in db.tb_AssetType on a.type_Asset equals t.ID
                            join d in db.tb_dataDict_para on a.measurement equals d.ID
                            join j in db.tb_dataDict_para on a.addressCF equals j.ID
                            join p in db.tb_department on a.department_Using equals p.ID
                            join u in db.tb_user on a.Owener equals u.ID
                            join e in db.tb_dataDict_para on a.state_asset equals e.ID
                            join sp in db.tb_supplier on a.supplierID equals sp.ID
                            where p.ID_Department == item_id
                            select new
                            {
                                ID = a.ID,
                                serial_number = a.serial_number,
                                amountOfSys = a.amount,
                                type_Asset = t.name_Asset_Type,
                                name_Asset = a.name_Asset,
                                specification = a.specification,
                                measurement = d.name_para,
                                unit_price = a.unit_price,
                                amount = a.amount,
                                Total_price = a.Total_price,
                                department_using = p.name_Department,
                                address = j.name_para,
                                owener = u.true_Name,
                               // state_asset = e.name_para,
                                supllier = sp.name_supplier

                            }).ToArray()
                };


                return Json(json, JsonRequestBehavior.AllowGet);

            }
            if (name_flag_string == SystemConfig.nameFlag_2_ZCLB)
            {
                var json = new
                {
                    total = list.Count(),
                    rows = (from a in db.tb_Asset
                            join t in db.tb_AssetType on a.type_Asset equals t.ID
                            join d in db.tb_dataDict_para on a.measurement equals d.ID
                            join j in db.tb_dataDict_para on a.addressCF equals j.ID
                            join p in db.tb_department on a.department_Using equals p.ID
                            join u in db.tb_user on a.Owener equals u.ID
                            join e in db.tb_dataDict_para on a.state_asset equals e.ID 
                            join sp in db.tb_supplier on a.supplierID equals sp.ID
                            where t.orderID == item_id.ToString()
                            select new
                            {
                                ID = a.ID,
                                serial_number = a.serial_number,
                                amountOfSys = a.amount,
                                type_Asset = t.name_Asset_Type,
                                name_Asset = a.name_Asset,
                                specification = a.specification,
                                measurement = d.name_para,
                                unit_price = a.unit_price,
                                amount = a.amount,
                                Total_price = a.Total_price,
                                department_using = p.name_Department,
                                address = j.name_para,
                                owener = u.true_Name,
                              //  state_asset = e.name_para,
                                supllier = sp.name_supplier

                            }).ToArray()
                };
                return Json(json, JsonRequestBehavior.AllowGet);

            }
            if (name_flag_string == SystemConfig.nameFlag_2_ZCZT)
            {
                var json = new
                {
                    total = list.Count(),
                    rows = (from a in db.tb_Asset
                   
                            join t in db.tb_AssetType on a.type_Asset equals t.ID
                            join d in db.tb_dataDict_para on a.measurement equals d.ID
                            join j in db.tb_dataDict_para on a.addressCF equals j.ID
                            join p in db.tb_department on a.department_Using equals p.ID
                            join u in db.tb_user on a.Owener equals u.ID
                            join sp in db.tb_supplier on a.supplierID equals sp.ID
                            join e in db.tb_dataDict_para on a.state_asset equals e.ID
                            where e.ID == item_id
                            select new
                            {
                                ID = a.ID,
                                serial_number = a.serial_number,
                                amountOfSys = a.amount,
                                type_Asset = t.name_Asset_Type,
                                name_Asset = a.name_Asset,
                                specification = a.specification,
                                measurement = d.name_para,
                                unit_price = a.unit_price,
                                amount = a.amount,
                                Total_price = a.Total_price,
                                department_using = p.name_Department,
                                address = j.name_para,
                                owener = u.true_Name,
                                state_asset = e.name_para,
                                supllier = sp.name_supplier


                            }).ToArray()
                };
                return Json(json, JsonRequestBehavior.AllowGet);

            }
            return Null_dataGrid();
          

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
	}
}