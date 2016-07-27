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
namespace FAMIS.Controllers
{
    
    public class AssetDeatailsController : Controller
    {
        // DictController dc = new DictController();
        CommonController comController = new CommonController();
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
                                    join t in db.tb_AssetType on a.type_Asset equals t.ID into temp_tt
                                    from tt in temp_tt.DefaultIfEmpty()
                                    join d in db.tb_dataDict_para on a.measurement equals d.ID into temp_d
                                    from dd in temp_d.DefaultIfEmpty()
                                    join j in db.tb_dataDict_para on a.addressCF equals j.ID into temp_j

                                    from jj in temp_j.DefaultIfEmpty()
                                    join s in db.tb_dataDict_para on a.state_asset equals s.ID into temp_s

                                    from ss in temp_s.DefaultIfEmpty()
                                    join p in db.tb_department on a.department_Using equals p.ID into temp_p
                                    from pp in temp_p.DefaultIfEmpty()
                                    join u in db.tb_user on a.Owener equals u.ID into temp_u
                                    from uu in temp_u.DefaultIfEmpty()
                                    join sp in db.tb_supplier on a.supplierID equals sp.ID into temp_sp
                                    from ssp in temp_sp.DefaultIfEmpty()
                                    join e in db.tb_dataDict_para on a.state_asset equals e.ID into temp_ee
                                    from ee in temp_ee.DefaultIfEmpty()
                                    join zj in db.tb_dataDict_para on a.Method_add equals zj.ID into temp_zj
                                    from zzj in temp_zj.DefaultIfEmpty()
                                    join fs in db.tb_dataDict_para on a.Method_depreciation equals fs.ID into temp_fs
                                     from ffs in temp_fs.DefaultIfEmpty()
                        

                        where a.serial_number==Json
                        select a.serial_number+","+t.name_Asset_Type+","+t.name_Asset_Type+","+a.name_Asset+","+a.specification+
                        ","+dd.name_para+","+pp.name_Department+","+uu.true_Name+","+ jj.name_para+","+zzj.name_para+","+ssp.name_supplier+
                         ","+ssp.linkman+","+ssp.address+","+a.Time_Purchase+","+ss.name_para+","+a.Time_add+","+a.YearService_month+","+
                        ffs.name_para+","+a.Net_residual_rate+","+a.unit_price+","+a.amount+","+a.value+","+a.depreciation_Month+","
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
             if (flagnum == 0)
                 flagnum = 11000000;
             int name_flag = flagnum /1000000;
             string name_flag_string = "";
             int item_id = flagnum % 1000000;

             List<int?> Depart_Asset_Type_Id = new List<int?>();
             List<int?> AssetassettypeId = new List<int?>();
             List<int?> ZTassettypeId = new List<int?>();

             IEnumerable<String> flags = from o in db.tb_dataDict
                                         where o.ID == name_flag
                                         select o.name_flag;
             foreach (string p in flags)
             {
                 name_flag_string = p;
             }
            
             if (name_flag_string == SystemConfig.nameFlag_2_SYBM)
             {

                 Depart_Asset_Type_Id = comController.GetSonIDs_Department(item_id);
                 IEnumerable<tb_Asset> q = from a in db.tb_Asset
                                           join t in db.tb_AssetType on a.type_Asset equals t.ID into temp_t
                                           from tt in temp_t.DefaultIfEmpty()
                                           join d in db.tb_dataDict_para on a.measurement equals d.ID into temp_d
                                           from dd in temp_d.DefaultIfEmpty()
                                           join j in db.tb_dataDict_para on a.addressCF equals j.ID into temp_j
                                           from jj in temp_j.DefaultIfEmpty()
                                           join p in db.tb_department on a.department_Using equals p.ID into temp_p
                                           from pp in temp_p.DefaultIfEmpty()
                                           join u in db.tb_user on a.Owener equals u.ID into temp_u
                                           from uu in temp_u.DefaultIfEmpty()
                                           join e in db.tb_dataDict_para on a.state_asset equals e.ID into temp_e
                                           from ee in temp_e.DefaultIfEmpty()
                                           join sp in db.tb_supplier on a.supplierID equals sp.ID into temp_sp
                                           from ssp in temp_sp.DefaultIfEmpty()
                                           where Depart_Asset_Type_Id.Contains(a.department_Using)||item_id==0
                                           select a;
                
                 foreach (tb_Asset asset in q)
                 {
                      
                     List<tb_Asset_inventory_Details> Deatail_List = db.tb_Asset_inventory_Details.Where(a => a.serial_number_Asset == asset.serial_number&&a.serial_number==pdsearial&&a.flag==true).ToList();
                     if (Deatail_List.Count > 0)
                         continue;
                     
                     var rule_tb = new tb_Asset_inventory_Details
                     {
                          serial_number = pdsearial,
                          state="盘亏",
                          amountOfSys=asset.amount,
                          amountOfInv=0,
                          difference=0-asset.amount,
                          serial_number_Asset=asset.serial_number,
                          
                         flag=true
                     };

                     db.tb_Asset_inventory_Details.Add(rule_tb);
                    
                 
                 }
                 db.SaveChanges();         
                            
            }
             if (name_flag_string == SystemConfig.nameFlag_2_ZCLB)
             {
                 AssetassettypeId=comController.GetSonID_AsseType(item_id);

                 IEnumerable<tb_Asset> q = from a in db.tb_Asset
                                           join t in db.tb_AssetType on a.type_Asset equals t.ID into temp_t
                                           from tt in temp_t.DefaultIfEmpty()
                                           join d in db.tb_dataDict_para on a.measurement equals d.ID into temp_d
                                           from dd in temp_d.DefaultIfEmpty()
                                           join j in db.tb_dataDict_para on a.addressCF equals j.ID into temp_j
                                           from jj in temp_j.DefaultIfEmpty()
                                           join p in db.tb_department on a.department_Using equals p.ID into temp_p
                                           from pp in temp_p.DefaultIfEmpty()
                                           join u in db.tb_user on a.Owener equals u.ID into temp_u
                                           from uu in temp_u.DefaultIfEmpty()
                                           join e in db.tb_dataDict_para on a.state_asset equals e.ID into temp_e
                                           from ee in temp_e.DefaultIfEmpty()
                                           join sp in db.tb_supplier on a.supplierID equals sp.ID into temp_sp
                                           from ssp in temp_sp.DefaultIfEmpty()
                                           where AssetassettypeId.Contains(a.type_Asset) || item_id == 0
                                           select a;
                 foreach (tb_Asset asset in q)
                 {
                     List<tb_Asset_inventory_Details> Deatail_List = db.tb_Asset_inventory_Details.Where(a => a.serial_number_Asset == asset.serial_number && a.serial_number == pdsearial && a.flag == true).ToList();
                     if (Deatail_List.Count > 0)
                         continue;
                      
                     var rule_tb = new tb_Asset_inventory_Details
                     {
                         serial_number = pdsearial,
                         state = "盘亏",
                         amountOfSys = asset.amount,
                         amountOfInv = 0,
                         difference = 0 - asset.amount,
                         serial_number_Asset = asset.serial_number,
                         flag=true
                     };
                     db.tb_Asset_inventory_Details.Add(rule_tb);
                    

                 }
                

             }
             db.SaveChanges();
             if (name_flag_string == SystemConfig.nameFlag_2_ZCZT)
             {



                 ZTassettypeId = comController.GetSonIDs_dataDict_Para(item_id);
                 IEnumerable<tb_Asset> q = from a in db.tb_Asset
                                           join t in db.tb_AssetType on a.type_Asset equals t.ID into temp_t
                                           from tt in temp_t.DefaultIfEmpty()
                                           join d in db.tb_dataDict_para on a.measurement equals d.ID into temp_d
                                           from dd in temp_d.DefaultIfEmpty()
                                           join j in db.tb_dataDict_para on a.addressCF equals j.ID into temp_j
                                           from jj in temp_j.DefaultIfEmpty()
                                           join p in db.tb_department on a.department_Using equals p.ID into temp_p
                                           from pp in temp_p.DefaultIfEmpty()
                                           join u in db.tb_user on a.Owener equals u.ID into temp_u
                                           from uu in temp_u.DefaultIfEmpty()
                                           join e in db.tb_dataDict_para on a.state_asset equals e.ID into temp_e
                                           from ee in temp_e.DefaultIfEmpty()
                                           join sp in db.tb_supplier on a.supplierID equals sp.ID into temp_sp
                                           from ssp in temp_sp.DefaultIfEmpty()
                                           where ZTassettypeId.Contains(a.state_asset)|| item_id == 0
                                           select a;
                 foreach (tb_Asset asset in q)
                 {
                     List<tb_Asset_inventory_Details> Deatail_List = db.tb_Asset_inventory_Details.Where(a => a.serial_number_Asset == asset.serial_number && a.serial_number == pdsearial && a.flag == true).ToList();
                     if (Deatail_List.Count > 0)
                         continue;
                     var rule_tb = new tb_Asset_inventory_Details
                     {
                         serial_number = pdsearial,
                         state = "盘亏",
                         amountOfSys = asset.amount,
                         amountOfInv = 0,
                         difference = 0 - asset.amount,
                         serial_number_Asset = asset.serial_number,
                         flag=true
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
       public string Get_Serial()
       {
           string json = "";

           json = Session["Searial"].ToString();


           return json;
       }
       [HttpPost]
       public string Get_Error_File()
       {
           string json = "";
           if (Session["ErrorFile"] == null)
               return "nullsession";
           json = Session["ErrorFile"].ToString();


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
        public JsonResult Load_Asset(int? page, int? rows, string JSdata)
        {
           page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;
            
            int flagnum = int.Parse(JSdata);
            if (flagnum == 0)
                flagnum = 11000000;
            int name_flag = flagnum /1000000;
            string name_flag_string = "";
            int item_id = flagnum %1000000;

            List<int?> Depart_Asset_Type_Id = new List<int?>();
            List<int?> AssetassettypeId = new List<int?>();
            List<int?> ZTassettypeId = new List<int?>();
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
                Depart_Asset_Type_Id = comController.GetSonIDs_Department(item_id);

                   var data= from a in db.tb_Asset

                             join t in db.tb_AssetType on a.type_Asset equals t.ID into temp_tt
                             from tt in temp_tt.DefaultIfEmpty()
                             join d in db.tb_dataDict_para on a.measurement equals d.ID into temp_d
                             from dd in temp_d.DefaultIfEmpty()
                             join j in db.tb_dataDict_para on a.addressCF equals j.ID into temp_j
                             from jj in temp_j.DefaultIfEmpty()
                             join p in db.tb_department on a.department_Using equals p.ID into temp_p
                             from pp in temp_p.DefaultIfEmpty()
                             join u in db.tb_user on a.Owener equals u.ID into temp_u
                             from uu in temp_u.DefaultIfEmpty()
                             join sp in db.tb_supplier on a.supplierID equals sp.ID into temp_sp
                             from ssp in temp_sp.DefaultIfEmpty()
                             join e in db.tb_dataDict_para on a.state_asset equals e.ID into temp_ee
                             from ee in temp_ee.DefaultIfEmpty()

                            where Depart_Asset_Type_Id.Contains(a.department_Using)||item_id == 0
                            select new
                            {
                                ID = a.ID,
                                serial_number = a.serial_number,
                                amountOfSys = a.amount,
                                type_Asset = tt.name_Asset_Type,
                                name_Asset = a.name_Asset,
                                specification = a.specification,
                                measurement = dd.name_para,
                                unit_price = a.unit_price,
                                amount = a.amount,
                                Total_price = a.value,
                                department_using = pp.name_Department,
                                address = jj.name_para,
                                owener = uu.true_Name,
                                state_asset = ee.name_para,
                                supllier = ssp.name_supplier

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

                AssetassettypeId = comController.GetSonID_AsseType(item_id);
                   var data =  from a in db.tb_Asset

                               join t in db.tb_AssetType on a.type_Asset equals t.ID into temp_tt
                               from tt in temp_tt.DefaultIfEmpty()
                               join d in db.tb_dataDict_para on a.measurement equals d.ID into temp_d
                               from dd in temp_d.DefaultIfEmpty()
                               join j in db.tb_dataDict_para on a.addressCF equals j.ID into temp_j
                               from jj in temp_j.DefaultIfEmpty()
                               join p in db.tb_department on a.department_Using equals p.ID into temp_p
                               from pp in temp_p.DefaultIfEmpty()
                               join u in db.tb_user on a.Owener equals u.ID into temp_u
                               from uu in temp_u.DefaultIfEmpty()
                               join sp in db.tb_supplier on a.supplierID equals sp.ID into temp_sp
                               from ssp in temp_sp.DefaultIfEmpty()
                               join e in db.tb_dataDict_para on a.state_asset equals e.ID into temp_ee
                               from ee in temp_ee.DefaultIfEmpty()

                            where AssetassettypeId.Contains(a.type_Asset)|| item_id == 0 
                            select new
                            {
                                ID = a.ID,
                                serial_number = a.serial_number,
                                amountOfSys = a.amount,
                                type_Asset = tt.name_Asset_Type,
                                name_Asset = a.name_Asset,
                                specification = a.specification,
                                measurement = dd.name_para,
                                unit_price = a.unit_price,
                                amount = a.amount,
                                Total_price = a.value,
                                department_using = pp.name_Department,
                                address = jj.name_para,
                                owener = uu.true_Name,
                              //  state_asset = e.name_para,
                                supllier = ssp.name_supplier

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
                 
 
            if (name_flag_string == SystemConfig.nameFlag_2_ZCZT)
            {

                ZTassettypeId = comController.GetSonIDs_dataDict_Para(item_id);
                    var data =  from a in db.tb_Asset
                   
                            join t in db.tb_AssetType on a.type_Asset equals t.ID into  temp_tt
                            from tt in temp_tt.DefaultIfEmpty()
                            join d in db.tb_dataDict_para on a.measurement equals d.ID into temp_d
                            from dd in temp_d.DefaultIfEmpty()
                            join j in db.tb_dataDict_para on a.addressCF equals j.ID into temp_j
                            from jj in temp_j.DefaultIfEmpty()
                            join p in db.tb_department on a.department_Using equals p.ID into temp_p
                            from pp in temp_p.DefaultIfEmpty()
                            join u in db.tb_user on a.Owener equals u.ID into temp_u
                            from uu in temp_u.DefaultIfEmpty()
                            join sp in db.tb_supplier on a.supplierID equals sp.ID into temp_sp
                            from ssp in temp_sp.DefaultIfEmpty()
                            join e in db.tb_dataDict_para on a.state_asset equals e.ID into temp_ee
                            from ee in temp_ee.DefaultIfEmpty()

                            where ZTassettypeId.Contains(a.state_asset) || item_id == 0
                            select new
                            {
                                ID = a.ID,
                                serial_number = a.serial_number,
                                amountOfSys = a.amount,
                                type_Asset = tt.name_Asset_Type,
                                name_Asset = a.name_Asset,
                                specification = a.specification,
                                measurement = dd.name_para,
                                unit_price = a.unit_price,
                                amount = a.amount,
                                Total_price = a.value,
                                department_using = pp.name_Department,
                                address = jj.name_para,
                                owener = uu.true_Name,
                                state_asset = ee.name_para,
                                supllier = ssp.name_supplier


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