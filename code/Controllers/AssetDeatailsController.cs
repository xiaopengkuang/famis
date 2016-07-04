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
        [HttpPost]
        public JsonResult Load_Asset(string JSdata)
        {
            string[] temp = JSdata.Split('o');
            int rid = int.Parse(temp[0]);
            string searchtype = temp[1];
            string searchmethod = temp[2];
            List<tb_Asset> list = db.tb_Asset.ToList();
            if (searchtype ==SystemConfig.nameFlag_2_SYBM)
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
                            where p.ID_Department.ToString() == searchmethod
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
            if (searchtype == SystemConfig.nameFlag_2_ZCLB)
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
                            where t.orderID == searchmethod
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
            if (searchtype == SystemConfig.nameFlag_2_ZCZT)
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
                            where e.ID.ToString() == searchmethod
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