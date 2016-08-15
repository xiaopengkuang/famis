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
using System.Data.Entity;
namespace FAMIS.Controllers
{
    public class NoticeController : Controller
    {
        FAMISDBTBModels db = new FAMISDBTBModels();
        AssetDeatailsController adc = new AssetDeatailsController();
        CommonConversion com = new CommonConversion();
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
        public JsonResult Invalidate_Notice(int? page,int? rows)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            List<tb_Asset> asset = new List<tb_Asset>();
            int ? rid=com.getRoleID();
            List<int?> dids = adc.IDs(rid, "department"); 
            List<int?> ATids = adc.IDs(rid, "Assettype");
            var data = from r in db.tb_Asset

                       join t in db.tb_AssetType on r.type_Asset equals t.ID into temp_t
                       from tt in temp_t.DefaultIfEmpty()
                       join d in db.tb_dataDict_para on r.measurement equals d.ID into temp_d
                       from dd in temp_d.DefaultIfEmpty()
                       join j in db.tb_dataDict_para on r.addressCF equals j.ID into temp_j
                       from jj in temp_j.DefaultIfEmpty()
                       join p in db.tb_department on r.department_Using equals p.ID into temp_p
                       from pp in temp_p.DefaultIfEmpty()
                       join u in db.tb_user on r.Owener equals u.ID into temp_u
                       from uu in temp_u.DefaultIfEmpty()
                       join s in db.tb_dataDict_para on r.state_asset equals s.ID into temp_s
                       from ss in temp_s.DefaultIfEmpty()
                       join sp in db.tb_supplier on r.supplierID equals sp.ID into temp_sp
                       from ssp in temp_sp.DefaultIfEmpty()
                       where DbFunctions.DiffDays(DateTime.Now, r.Time_Purchase) + 30 * r.YearService_month <= SystemConfig.Days_To_Notice&&ATids.Contains(r.type_Asset)&&dids.Contains(r.department_Using)
                       select new
                       {

                           ID = r.ID,
                           DaysNotice = DbFunctions.DiffDays(DateTime.Now, r.Time_Purchase) + 30 * r.YearService_month,
                           serial_number = r.serial_number,

                           type_Asset = tt.name_Asset_Type,
                           name_Asset = r.name_Asset,
                           specification = r.specification,
                           measurement = dd.name_para,
                           unit_price = r.unit_price,
                           amount = r.amount,
                           Total_price = r.value,
                           department_using = pp.name_Department,
                           address = jj.name_para,
                           owener = uu.true_Name,
                           state_asset = ss.name_para,
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
        [HttpPost]
        public JsonResult Repair_Notice(int? page, int? rows)
        {
            List<tb_Asset_Repair> repair = new List<tb_Asset_Repair>();
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            int? rid = com.getRoleID();
            List<int?> dids = adc.IDs(rid, "department");
            List<int?> ATids = adc.IDs(rid, "Assettype");
            var data = from r in db.tb_Asset_Repair
                        join s in db.tb_State_List on r.state_list equals s.id into temp_s
                        from ss in temp_s.DefaultIfEmpty()
                        join spl in db.tb_supplier on r.supplierID_Torepair equals spl.ID into temp_spl
                        from sspl in temp_spl.DefaultIfEmpty()
                        join ass in db.tb_Asset on r.ID_Asset equals ass.ID into temp_ass
                        from aass in temp_ass.DefaultIfEmpty()
                        join uapp in db.tb_user on r.userID_applying equals uapp.ID into temp_uapp
                        from uuapp in temp_uapp.DefaultIfEmpty()
                        join uaut in db.tb_user on r.userID_authorize equals uaut.ID into temp_uaut
                        from uuaut in temp_uaut.DefaultIfEmpty()
                        join ucreat in db.tb_user on r.userID_create equals ucreat.ID into temp_ucreat
                        from uucreat in temp_ucreat.DefaultIfEmpty()
                        join uview in db.tb_user on r.userID_review equals uview.ID into temp_uview
                        from uuview in temp_uview.DefaultIfEmpty()

                       where DbFunctions.DiffDays(DateTime.Now, r.date_ToReturn) <= SystemConfig.Days_To_Notice && r.flag == true && ss.Name == SystemConfig.state_List_YSH && ATids.Contains(aass.type_Asset) && dids.Contains(aass.department_Using)
                        //&& ss.Name!=SystemConfig.state_List_TH
                        select new
                        {

                            ID = r.ID,
                            DaysNotice = DbFunctions.DiffDays(DateTime.Now, r.date_ToReturn),
                            serial= r.serialNumber,
                            RepairDate=r.date_ToRepair,
                            Returndate=r.date_ToReturn,
                            Reason=r.reason_ToRepair,
                            RepairNote=r.note_repair,
                            uapp=uuapp.true_Name,
                            uaut=uuaut.true_Name,
                            uview =uuview.true_Name,
                            dateview=r.date_review,
                            usercreat=uucreat.true_Name,
                            nameqment=r.Name_equipment,
                            costrepair=r.CostToRepair,
                            createdate=r.date_create,
                            content_Review=r.content_Review,
                            serial_number = r.serialNumber,
                            state=ss.Name,
                             
                            supplier = sspl.name_supplier,
                            assetname=aass.name_Asset,
                            



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

        [HttpPost]
        public JsonResult Return_Notice(int? page, int? rows)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            List<tb_Asset_Borrow> repair = new List<tb_Asset_Borrow>();
            int? rid = com.getRoleID();
            List<int?> dids = adc.IDs(rid, "department");
            List<int?> ATids = adc.IDs(rid, "Assettype");
            var data = from r in db.tb_Asset_Borrow
                       join s in db.tb_State_List on r.state_list equals s.id into temp_s
                       from ss in temp_s.DefaultIfEmpty()
                       join dp in db.tb_department on r.department_borrow equals dp.ID into temp_dp
                       from ddp in temp_dp.DefaultIfEmpty()
                       join ub in db.tb_user on r.userID_borrow equals ub.ID into temp_ub
                       from uub in temp_ub.DefaultIfEmpty()
                       join uo in db.tb_user on r.userID_operated equals uo.ID into temp_uo
                       from uuo in temp_uo.DefaultIfEmpty()
                       join ur in db.tb_user on r.userID_review equals ur.ID into temp_ur
                       from uur in temp_ur.DefaultIfEmpty()
                      
                       where DbFunctions.DiffDays(DateTime.Now, r.date_return) <= SystemConfig.Days_To_Notice && r.flag == true && ss.Name == SystemConfig.state_List_YSH
                       select new
                       {

                           ID = r.ID,
                           DaysNotice = DbFunctions.DiffDays(DateTime.Now, r.date_return),
                           serial = r.serialNum,
                           RepairDate = r.date_borrow,
                           Returndate = r.date_return,
                           Reason = r.reason_borrow,
                           RepairNote = r.note_borrow,
                           Borrow_Depart = ddp.name_Department,
                           uapp = uub.true_Name,
                           uaut = uuo.true_Name,
                           uview = uub.true_Name,




                           content_Review = r.content_review,
                           datecreat = r.date_operated,
                           state = ss.Name







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
}