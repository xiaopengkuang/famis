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
namespace FAMIS.Controllers
{
    public class DepreciationController : Controller
    {
        //
      
        DictController dc = new DictController();
        FAMISDBTBModels db = new FAMISDBTBModels();
        StringBuilder result_tree_SearchTree = new StringBuilder();
        StringBuilder sb_tree_SearchTree = new StringBuilder();
        public ActionResult depreciation()
        {
            return View();
        }
        [HttpPost]
        public string Depreciation()
        {
            StreamWriter sw = new StreamWriter("D:\\qp0.txt", true);

            List<String> list = Get_Depreciation_Data();
            UpdateAsset_table(list);
            for (int i = 0; i < list.Count(); i++)
            {
                sw.WriteLine(list[i]);
            }
            sw.Close();

                return "sad";
        }
        public void UpdateAsset_table( List<String> list)
        {
            for(int i=0;i<list.Count();i++)
            { 
                int id= int.Parse(list[i].Split('o')[0]);
                float tatol_price=float.Parse(list[i].Split('o')[1].Split(',')[0]);
                float month_depreciation=float.Parse(list[i].Split('o')[1].Split(',')[1]);
                float tatol_depreciation=float.Parse(list[i].Split('o')[1].Split(',')[2]);
                 float net_value=float.Parse(list[i].Split('o')[1].Split(',')[3]);
                 var q = from p in db.tb_Asset
                    where p.ID ==id
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
            StreamWriter sw = new StreamWriter("D:\\qp2.txt", true);
            
           string para="";
            
            List<String> list = new List<String>();
            IEnumerable<tb_Asset> asset = from o in db.tb_Asset
                                          select o;
            if (asset.Count() > 0)
            {

                int temp = 0;
                foreach (tb_Asset o in asset)
                {



                    if (temp != asset.Count() - 1)
                        para += o.ID + "," + o.Method_depreciation + "," + o.unit_price + "," + o.amount + "," + o.YearService_month + "," + o.Net_residual_rate + "," + o.Time_Purchase + "," + o.depreciation_tatol+"o";
                    else
                        para += o.ID + "," + o.Method_depreciation + "," + o.unit_price + "," + o.amount + "," + o.YearService_month + "," + o.Net_residual_rate + "," + o.Time_Purchase+","+o.depreciation_tatol;
                    temp++; 

                }
            }
            string[] Assetrows = para.Split('o');
            sw.WriteLine(para);
            
            for (int i = 0; i < Assetrows.Length; i++)
            {
                string[] parameters = Assetrows[i].Split(',');
                sw.WriteLine(GetMethod_depreciation(int.Parse(parameters[1])));
                switch (GetMethod_depreciation(int.Parse(parameters[1])))
                {

                    case "平均年限法":
                        {
                            sw.Write("执行了！");
                            
                           string data= Caculate_Depreciation_AverageYear(parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7]);
                            
                           if (data == "")
                               continue;
                            else
                               list.Add(parameters[0]+"o"+data);

                            break;
                        }

                    default:
                        break;

                }

            }
            sw.Close();
            return list;
        }
        //保留小数点后两位
    
        //折旧计算
        public string Caculate_Depreciation_AverageYear(string unit, string amount, string year, string rate, string timepurchase, string Total_Depreciation_Amount)
        {
            if (timepurchase == null)
                return "";
            DateTime now=DateTime.Now;
            int thismonth=now.Month;
           
            int purchasetime= int.Parse(timepurchase.Split('/')[1]);

            int month_passed=thismonth-purchasetime;
           
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
                para = totalprice.ToString()+ "," + Month_Depreciation_Amount.ToString() + "," + Total_Depreciation_Amount.ToString()
                    + "," + net_value.ToString();
            }

           
            return para;
        }
         
        public string GetMethod_depreciation(int id)
        {
            IEnumerable<tb_dataDict_para> data = from o in db.tb_dataDict_para
                                                  where o.ID==id
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
            
            string[] temp = JSdata.Split('o');
            int rid = int.Parse(temp[0]);
            string searchtype = temp[1];
            string searchmethod = temp[2];
            List<tb_Asset> list = db.tb_Asset.ToList();
            if (searchtype == "使用部门")
            {
                var json = new
                {
                    total = list.Count(),
                    rows = (from r in db.tb_Asset
                            join t in db.tb_AssetType on r.type_Asset equals t.assetTypeCode.ToString()
                            join D in db.tb_department on r.department_Using equals D.ID_Department.ToString()
                            join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID
                            where D.name_Department == searchmethod
                            select new 
                            {
                                ID = r.ID,
                                department_Using = D.name_Department,
                                serial_number = r.serial_number,
                                name_Asset = t.name_Asset_Type,
                                specification = r.specification,
                                unit_price = r.unit_price,
                                amount = r.amount,
                                Total_price=r.Total_price,
                                depreciation_tatol=r.depreciation_tatol,
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
            if (searchtype == "资产类别")
            {
                var json = new
                {
                    total = list.Count(),
                    rows = (from r in db.tb_Asset
                            join t in db.tb_AssetType on r.type_Asset equals t.assetTypeCode.ToString()
                            join D in db.tb_department on r.department_Using equals D.ID_Department.ToString()
                            join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID
                            where t.name_Asset_Type== searchmethod
                            select new
                            {
                                ID = r.ID,
                                department_Using =D.name_Department,
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
            return Json("Error", JsonRequestBehavior.AllowGet);
           
                         
        }
      
       
        
	}
}