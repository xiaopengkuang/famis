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
using System.Data.Entity;
namespace FAMIS.Controllers
{
    public class NoticeController : Controller
    {
        FAMISDBTBModels db = new FAMISDBTBModels();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Invalidated()
        {
            return View();
        }
        public ActionResult RepairNotice()
        {
            return View();
        }
        public ActionResult ReturnNotice()
        {
            return View();
        }
        public bool IsNear_Invalidate(DateTime? TimePurchase,int? MonthService)
        {
            DateTime date = DateTime.Parse(TimePurchase.ToString().Trim());
            int year_purchase=  int.Parse(date.ToString("YYYY"));
            int month_purchase= int.Parse(date.ToString("MM"));
            int yearnow = int.Parse(DateTime.Now.ToString("yyyy"));
            int monthnow = int.Parse(DateTime.Now.ToString("yyyy"));
            int totalmonth = (yearnow - year_purchase) * 12 + monthnow - month_purchase;
            StreamWriter sw = new StreamWriter("D:\\sw.txt",true);
            sw.WriteLine(year_purchase+","+month_purchase+","+yearnow+","+monthnow+","+totalmonth+","+MonthService+","+(MonthService-totalmonth));
            sw.Close();
            if ((MonthService - totalmonth) <= 1)
                return true;
            else return false;
            
          
        }

        public DateTime  Date_Add(DateTime? TimePurchase, int? MonthService)
        {
            DateTime date = DateTime.Parse(TimePurchase.ToString().Trim());
            string mydate;
            int year_purchase = int.Parse(date.ToString("YYYY"));
            int month_purchase = int.Parse(date.ToString("MM"));
            string day_purchase=date.ToString("DD");
            int? month_remian = MonthService % 12;
            int? year_remain = MonthService / 12;
            int?  year_out= (month_remian+month_purchase)/12;
            int? month_twice_remain=(month_remian+month_purchase)%12;
           if((month_remian+month_purchase)>=12)

            mydate=(year_purchase+year_remain+year_out).ToString()+"-"+month_twice_remain+"-"+day_purchase+" "+"09:09:09";
            else
               mydate=(year_purchase+year_remain+year_out).ToString()+"-"+month_remian+month_purchase+"-"+day_purchase+" "+"09:09:09";
            return DateTime.Parse(mydate);

        }
        public int Get_Days_Remaining(DateTime? Date_Supposed)
        {
            DateTime date = DateTime.Parse(Date_Supposed.ToString().Trim());
            int daysremaining=0;
          
            TimeSpan distance = date.Subtract(DateTime.Now);
           daysremaining=distance.Days;
           return daysremaining;
        }
        public ArrayList Get_Near_Invalidated()
        {
            ArrayList Invalidating = new ArrayList();
            var q = from o in db.tb_Asset
                    select o;
            foreach (var p in q)
            {
                if (IsNear_Invalidate(p.Time_Purchase, p.YearService_month))
                {
                    int? remainingdays = 30*p.YearService_month+Get_Days_Remaining(p.Time_Purchase);
                    Invalidating.Add(p.serial_number+","+remainingdays);  
                }   
                   

            
            }

            return Invalidating;
          
        
        }
        [HttpPost]
        public JsonResult Invalidate_Notice()
        {
            List<tb_Asset> asset = new List<tb_Asset>();

            var json = new
            {
                total = asset.Count(),
                rows = (from r in db.tb_Asset
                         
                        join t in db.tb_AssetType on r.type_Asset equals t.ID
                        join d in db.tb_dataDict_para on r.measurement equals d.ID
                        join j in db.tb_dataDict_para on r.addressCF equals j.ID
                        join p in db.tb_department on r.department_Using equals p.ID
                        join u in db.tb_user on r.Owener equals u.ID
                        join s in db.tb_dataDict_para on r.state_asset equals s.ID
                        join sp in db.tb_supplier on r.supplierID equals sp.ID
                        where DbFunctions.DiffDays(DateTime.Now, r.Time_Purchase) + 30 * r.YearService_month<=SystemConfig.Days_To_Notice
                        select new
                        {

                            ID = r.ID,
                            DaysNotice = DbFunctions.DiffDays(DateTime.Now, r.Time_Purchase) + 30 * r.YearService_month,
                            serial_number = r.serial_number,
                          
                            
                            
                            
                            type_Asset = t.name_Asset_Type,
                            name_Asset = r.name_Asset,
                            specification = r.specification,
                            measurement = d.name_para,
                            unit_price = r.unit_price,
                            amount = r.amount,
                            Total_price = r.value,
                            department_using = p.name_Department,
                            address = j.name_para,
                            owener = u.true_Name,
                            state_asset = s.name_para,
                            supllier = sp.name_supplier



                        }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Repair_Notice()
        {
            List<tb_Asset_Repair> repair = new List<tb_Asset_Repair>();

            var json = new
            {
                total = repair.Count(),
                rows = (from r in db.tb_Asset_Repair
                        join s in db.tb_State_List on r.state_list equals s.id
                        join spl in db.tb_supplier on r.supplierID_Torepair equals spl.ID
                        join ass in db.tb_Asset on r.ID_Asset equals ass.ID
                        join uapp in db.tb_user on r.userID_applying equals uapp.ID
                        join uaut in db.tb_user on r.userID_authorize equals uaut.ID
                        join ucreat in db.tb_user on r.userID_create equals ucreat.ID
                        join uview in db.tb_user on r.userID_review equals uview.ID
                        where DbFunctions.DiffDays(DateTime.Now, r.date_ToReturn) <= SystemConfig.Days_To_Notice&&r.flag==true
                        select new
                        {

                            ID = r.ID,
                            DaysNotice = DbFunctions.DiffDays(DateTime.Now, r.date_ToReturn),
                           serial= r.serialNumber,
                            RepairDate=r.date_ToRepair,
                            Returndate=r.date_ToReturn,
                            Reason=r.reason_ToRepair,
                            RepairNote=r.note_repair,
                            uapp=uapp.true_Name,
                            uaut=uaut.true_Name,
                            uview =uview.true_Name,
                            dateview=r.date_review,
                            usercreat=ucreat.true_Name,
                            nameqment=r.Name_equipment,
                            costrepair=r.CostToRepair,
                            createdate=r.date_create,
                            content_Review=r.content_Review,
                            serial_number = r.serialNumber,
                            state=s.Name,
                             
                            supplier = spl.name_supplier,
                            assetname=ass.name_Asset,
                            



                        }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Return_Notice()
        {
            List<tb_Asset_Borrow> repair = new List<tb_Asset_Borrow>();

            var json = new
            {
                total = repair.Count(),
                rows = (from r in db.tb_Asset_Borrow
                        join s in db.tb_State_List on r.state_list equals s.id
                        
                        join dp in db.tb_department on r.department_borrow equals dp.ID
                        join ub in db.tb_user on r.userID_borrow equals ub.ID
                        join uo in db.tb_user on r.userID_operated equals uo.ID
                        join ur in db.tb_user on r.userID_review equals ur.ID
                        
                        where DbFunctions.DiffDays(DateTime.Now, r.date_return) <= SystemConfig.Days_To_Notice && r.flag == true
                        select new
                        {

                            ID = r.ID,
                            DaysNotice = DbFunctions.DiffDays(DateTime.Now, r.date_return),
                            serial = r.serialNum,
                            RepairDate = r.date_borrow,
                            Returndate = r.date_return,
                            Reason = r.reason_borrow,
                            RepairNote = r.note_borrow,
                            Borrow_Depart=dp.name_Department,
                            uapp = ub.true_Name,
                            uaut = uo.true_Name,
                            uview = ub.true_Name,
                            
                           
                            
                           
                            content_Review = r.content_review,
                            datecreat=r.date_operated,
                            state = s.Name

                            
                           




                        }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

	}
}