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
using System.Collections;

namespace FAMIS.Controllers
{
    public class DepreciationController : Controller
    {
        //
        DictController dc = new DictController();
        FAMISDBTBModels db = new FAMISDBTBModels();
        StringBuilder result_tree_SearchTree = new StringBuilder();
        StringBuilder sb_tree_SearchTree = new StringBuilder();
        Excel_Helper excel = new Excel_Helper();
        Serial serial = new Serial();
        public ActionResult depreciation()
        {
            return View();
        }

        [HttpPost]
        public string Dep_JT()
        {
            StreamWriter sw = new StreamWriter("D:\\qp0.txt", true);

            List<String> list = Get_Depreciation_Data();
            string ads = list[0];
            UpdateAsset_table(list);
            for (int i = 0; i < list.Count(); i++)
            {
                sw.WriteLine(list[i]);
            }
            sw.Close();

            return "sad";
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
                    p.Total_price = tatol_price;
                    p.depreciation_Month = month_depreciation;
                    p.depreciation_tatol = tatol_depreciation;
                    p.Net_value = net_value;
                }


            }
            db.SaveChanges();
        }
        public List<String> Get_Depreciation_Data()
        {
         

            string para = "";

            List<String> list = new List<String>();
            IEnumerable<tb_Asset> asset = from o in db.tb_Asset
                                          select o;
            if (asset.Count() > 0)
            {

                int temp = 0;
                foreach (tb_Asset o in asset)
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
               
               // StreamWriter sw = new StreamWriter("D:\\zima2.txt", true);
                string[] parameters = Assetrows[i].Split(',');
                if (parameters[1] == null || parameters[1]=="")
                    continue;
               //   sw.WriteLine(parameters[1] + ",");
               //  sw.WriteLine(GetMethod_depreciation(int.Parse(parameters[1])));
              //  sw.Close();
            
                switch (GetMethod_depreciation(int.Parse(parameters[1])))
                {

                    case "平均年限法":
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
            DateTime now = DateTime.Now;
            int thismonth = now.Month;

            int purchasetime = int.Parse(timepurchase.Split('/')[1]);

            int month_passed = thismonth - purchasetime;

            string para = "";
            float Year_Depreciation_Rate = ((float)100 - float.Parse(rate)) / float.Parse(year);//年折旧率
            float Month_Depreciation_Rate = Year_Depreciation_Rate / (float)12;
            float totalprice = float.Parse(amount) * float.Parse(unit);
            float Month_Depreciation_Amount = totalprice * Month_Depreciation_Rate;
            if (month_passed <= 0)
                return "";
            else
            {
                Total_Depreciation_Amount = (float.Parse(Total_Depreciation_Amount) + month_passed * Month_Depreciation_Amount).ToString();
                float net_value = totalprice - float.Parse(Total_Depreciation_Amount);
                para = totalprice.ToString() + "," + Month_Depreciation_Amount.ToString() + "," + Total_Depreciation_Amount.ToString()
                    + "," + net_value.ToString();
            }


            return para;
        }

        public string GetMethod_depreciation(int id)
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
        public JsonResult Load_Asset(string JSdata)
        {

            int flagnum = int.Parse(JSdata);
            int name_flag = flagnum/1000000;
            string name_flag_string = "";
            int item_id=flagnum%1000000;
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
                var json = new
                {
                    total = list.Count(),
                    rows = (from r in db.tb_Asset
                            join t in db.tb_AssetType on r.type_Asset equals t.ID
                            join D in db.tb_department on r.department_Using equals D.ID
                            join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID
                            where D.ID_Department == item_id
                            select new
                            {
                                ID = r.ID,
                                department_Using = D.name_Department,
                                serial_number = r.serial_number,
                                name_Asset = t.name_Asset_Type,
                                specification = r.specification,
                                unit_price = r.unit_price,
                                amount = r.amount,
                                Total_price = r.Total_price,
                                depreciation_tatol = r.depreciation_tatol,
                                Method_depreciation = k.name_para,
                                YearService_month = r.YearService_month,
                                Net_residual_rate = r.Net_residual_rate,
                                depreciation_Month = r.depreciation_Month,
                                Net_value = r.Net_value,
                                Time_Purchase = r.Time_Purchase,
                                Time_add = r.Time_add


                            }).ToArray()
                };


                return Json(json, JsonRequestBehavior.AllowGet);

            }
            if (name_flag_string == SystemConfig.nameFlag_2_ZCLB)
            {
                var json = new
                {
                    total = list.Count(),
                    rows = (from r in db.tb_Asset
                            join t in db.tb_AssetType on r.type_Asset equals t.ID
                            join D in db.tb_department on r.department_Using equals D.ID
                            join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID
                            where t.orderID == item_id.ToString()
                            select new
                            {
                                ID = r.ID,
                                department_Using = D.name_Department,
                                serial_number = r.serial_number,
                                name_Asset = t.name_Asset_Type,
                                specification = r.specification,
                                unit_price = r.unit_price,
                                amount = r.amount,
                                Total_price = r.Total_price,
                                depreciation_tatol = r.depreciation_tatol,
                                Method_depreciation = k.name_para,
                                YearService_month = r.YearService_month,
                                Net_residual_rate = r.Net_residual_rate,
                                depreciation_Month = r.depreciation_Month,
                                Net_value = r.Net_value,
                                Time_Purchase = r.Time_Purchase,
                                Time_add = r.Time_add


                            }).ToArray()
                };
                return Json(json, JsonRequestBehavior.AllowGet);

            }
            return Null_dataGrid();


        }
        [HttpPost]
        public string Set_SearialNum(string Json)
        {

            Session["Deatails_Searial"] = Json;
            return "";
        }
          [HttpPost]
        public string SetPDsearialSession(string Json)
        {
            Session["Deatails_Searial"] = Json;
            return "";
        }
         [HttpPost]
          public string SetCurrentRow(string Json)
        {
            Session["CurrentRow"] = Json;
            return "";
        }
       
        
        [HttpPost]
        public ActionResult PDdelete(string ID)
        {

            var Model = db.tb_Asset_inventory.Find(int.Parse(ID));
            db.tb_Asset_inventory.Remove(Model);
            db.SaveChanges();
            var pdsearail=from o in db.tb_Asset_inventory
                          where o.ID.ToString()==ID
                          select o;
            foreach(var p in pdsearail)
            {
                var deatails = from o in db.tb_Asset_inventory_Details
                               where o.serial_number == p.ID.ToString()
                               select o;

                db.tb_Asset_inventory_Details.Remove((tb_Asset_inventory_Details)deatails);
            }
            db.SaveChanges();

            return this.Json(Model);
        }

        [HttpPost]
        public string AddDP(string JSdata)
        {
            ArrayList mysearial = serial.ReturnNewSearial("PD", 1);
            
            int id = 0;
            try
            {
                id = int.Parse(JSdata.Split(',')[0]);
            }
            catch (Exception e)
            {
                ;
            }
            string oper= JSdata.Split(',')[1];
            string ps = JSdata.Split(',')[2];
            DateTime pddate = DateTime.Parse(JSdata.Split(',')[3].ToString());
            string type = JSdata.Split(',')[4];


            /* var q = from p in mydb.tb_Menu
              where p.Role_ID == Roleid
              select p;*/
            var q = from o in db.tb_Asset_inventory
                    where o.ID == id
                    select o;

            if (q.Count() == 0)
            {
                var rule_tb = new tb_Asset_inventory
                {
                     serial_number=mysearial[0].ToString(),
                     _operator = oper,
                     state="未盘点",
                     date=pddate,
                     property=type,
                     date_Create=DateTime.Now,
                     ps= ps,


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
        [HttpPost]
        public JsonResult Load_Inventory_details(string JSdata)
        {


            List<tb_Asset_inventory_Details> list = db.tb_Asset_inventory_Details.ToList();
            var json = new
            {
                total = list.Count(),

                rows = (from r in db.tb_Asset_inventory_Details
                        join a in db.tb_Asset on r.serial_number_Asset equals a.serial_number
                        join t in db.tb_AssetType on a.type_Asset equals t.ID
                        join d in db.tb_dataDict_para on a.measurement equals d.ID
                        join j in db.tb_dataDict_para on a.addressCF equals j.ID
                        join p in db.tb_department on a.department_Using equals p.ID
                        join u in db.tb_user on a.Owener equals u.ID
                        join s in db.tb_dataDict_para on a.state_asset equals s.ID
                        join sp in db.tb_supplier on a.supplierID equals sp.ID
                        where r.serial_number == JSdata
                        select new
                        {
                            ID = r.ID,
                            serial_number = r.serial_number,
                            state = r.state,
                            amountOfSys = r.amountOfSys,
                            amountOfInv = r.amountOfInv,
                            difference = r.difference,
                            serial_number_Asset = r.serial_number_Asset,
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
                            state_asset = s.name_para,
                            supllier = sp.name_supplier


                        }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);

        }
      
        public void Upadate_Inventory_Deatails(string serial, string Asset_serial, string inventory_amount)
        {

            var q = from p in db.tb_Asset_inventory_Details
                    where p.serial_number == serial && p.serial_number_Asset == Asset_serial
                    select p;
            foreach (var p in q)
            {
                p.amountOfInv = int.Parse(inventory_amount);
                p.difference = int.Parse(inventory_amount) - p.amountOfSys;
                if (int.Parse(inventory_amount) - p.amountOfSys < 0)
                    p.state = "盘亏";
                if (int.Parse(inventory_amount) - p.amountOfSys > 0)
                    p.state = "盘盈";
                else
                    p.state = "持平";
            }


            db.SaveChanges();

        }
        [HttpPost]
        public string ChangeState(string JSdata)
        {
            int searial = int.Parse(Session["Deatails_Searial"].ToString());
            var q = from o in db.tb_Asset_inventory
                    where searial == o.ID
                    select o;
            foreach (var p in q)
            { 
                if(JSdata=="2")
             p.state="盘点中";
                if (JSdata == "3")
                    p.state = "已盘点";          
            }
            db.SaveChanges();
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
                    string imgName = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string imgPath = "/" + imgName + FileSave.FileName;     //通过此对象获取文件名
                    string AbsolutePath = Server.MapPath(imgPath);
                    FileSave.SaveAs(AbsolutePath); 
                    //将上传的东西保存
                    ReadExcel( Session["Deatails_Searial"].ToString(), AbsolutePath);
                    
                }
            }
         return "盘点数据提交成功！";
        }
        
        public void ReadExcel(string pd,string path)
        {
           
            
           
           
            string temp = "";
            DataTable dt = excel.ImportExcelFile(path);
             for(int i = 0 ; i < dt.Rows.Count ; i++) 
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    StreamWriter sw = new StreamWriter("D:\\tet.txt");
                    sw.Write(dt.Rows.Count+","+dt.Columns.Count);
                    sw.Close();
                    if (j != dt.Columns.Count-1)
                    temp = dt.Rows[i][j].ToString();
                    else
                    Upadate_Inventory_Deatails(pd, temp, dt.Rows[i][j].ToString());

                } 
               
            }
           
          
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