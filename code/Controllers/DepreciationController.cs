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
                            join D in db.tb_department on r.department_Using equals D.ID_Department.ToString()
                            where D.name_Department == searchmethod
                            select new 
                            {
                                ID = r.ID,
                                department_Using = D.name_Department,
                                serial_number = r.serial_number,
                                name_Asset = r.name_Asset,
                                specification = r.specification,
                                unit_price = r.unit_price,
                                amount = r.amount,

                                Method_depreciation = r.Method_depreciation,
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

                                Method_depreciation = r.Method_depreciation,
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
      
        [HttpPost]
        public JsonResult LoadAsset(string JSdata)
        { 
            int rows = 1;
             
            string[] temp = JSdata.Split('o');
            int rid = int.Parse(temp[0]);
            string searchtype =temp[1];
            string searchmethod = temp[2];
            

            List<tb_Asset> list = db.tb_Asset.ToList();
            if (searchtype == "使用部门")
            {

                
               var json = new
                {
                    total = list.Count(),
                    rows = (from r in list
                            where r.department_Using == searchmethod
                            select new tb_Asset()
                            {
                                ID = r.ID,
                                department_Using = r.department_Using,
                                serial_number = r.serial_number,
                                name_Asset = r.name_Asset,
                                specification = r.specification,
                                unit_price = r.unit_price,
                                amount = r.amount,

                                Method_depreciation = r.Method_depreciation,
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

                    rows = (from r in list
                            where r.name_Asset== searchmethod
                            select new tb_Asset()
                            {
                                ID = r.ID,
                                department_Using=r.department_Using,
                                serial_number=r.serial_number,
                                name_Asset=r.name_Asset,
                                specification=r.specification,
                                unit_price=r.unit_price,
                                amount=r.amount,
                                Total_price=r.Total_price,
                                Method_depreciation=r.Method_depreciation,
                                YearService_month=r.YearService_month,
                                Net_residual_rate=r.Net_residual_rate,
                                depreciation_Month=r.depreciation_Month,
                                Net_value=r.Net_value,
                                Time_Purchase=r.Time_Purchase,
                                Time_add=r.Time_add
                               
                            }).ToArray()
                };
               return Json(json, JsonRequestBehavior.AllowGet);

            }
            
            
            return Json("Error", JsonRequestBehavior.AllowGet);
           
        }
        
	}
}