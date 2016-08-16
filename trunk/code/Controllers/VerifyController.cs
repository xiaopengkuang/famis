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
    public class VerifyController : Controller
    {
        DictController dc = new DictController();
        FAMISDBTBModels db = new FAMISDBTBModels();
        StringBuilder result_tree_SearchTree = new StringBuilder();
        StringBuilder sb_tree_SearchTree = new StringBuilder();
        Excel_Helper excel = new Excel_Helper();
        CommonController comController = new CommonController();
        Serial serial = new Serial();
        Print_Helper print = new Print_Helper("test");
        public ActionResult Depreciation()
        {
            return View();
        }
      
        public ActionResult Add_Inventory()
        {
            return View();
        }
        public ActionResult Edit_Inventory()
        {
            return View();
        }
        public ActionResult WX_Load(string openid)
        {
            Response.Write("<script>" + openid + "</script>");
            return View();
        }
        [HttpPost]
        public ActionResult Export(string serial, string DJ_type)
        {
            switch(DJ_type)
            {
                case "LY": 
                    {
                        var data = from o in db.tb_Asset_collar
                                   join od in db.tb_Asset_collar_detail on o.ID equals od.ID_collar
                                   join depp in db.tb_department on o.department_collar equals depp.ID  
                                   join asset in db.tb_Asset on od.ID_asset equals asset.ID
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join op in db.tb_user on o._operator equals op.ID
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID
                                   join gys in db.tb_dataDict_para on asset.supplierID equals gys.ID
                                   where o.serial_number == serial && o.flag == true
                                   select new
                                   {
                                       单据号 = o.serial_number,
                                       领用日期 = o.date,
                                       领用原因 = o.reason,
                                       领用部门 = depp.name_Department,
                                       领用人 = op.true_Name,
                                       存放地点 = o.addree_Storage,
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = cf.name_para,
                                       添加方式 = zjfs.name_para,
                                       资产状态 = zczt.name_para,
                                       供应商 = gys.name_para,
                                       备注 = o.ps


                                   };

                        DataTable data_table = Excel_Helper.ToDataTable(data);
                        return Excel_Exp(data_table, "领用明细");    
                              
                              
                     }

                case "JC":
                    {
                        var data = from o in db.tb_Asset_Borrow
                                   join depp in db.tb_department on o.department_borrow equals depp.ID  
                                    
                                   join od in db.tb_Asset_Borrow_detail on o.ID equals od.ID_borrow
                                   join asset in db.tb_Asset on od.ID_Asset equals asset.ID
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join op in db.tb_user on o.userID_operated equals op.ID
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID
                                   join gys in db.tb_dataDict_para on asset.supplierID equals gys.ID
                                   where o.serialNum == serial && o.flag == true
                                   select new
                                   {
                                       单据号 = o.serialNum,
                                       借出日期 = o.date_borrow,
                                       借出原因 = o.reason_borrow,
                                       预计归还时间=o.date_return,
                                       借出部门 = depp.name_Department,
                                       借出人 = op.true_Name,
                                       
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = cf.name_para,
                                       添加方式 = zjfs.name_para,
                                       资产状态 = zczt.name_para,
                                       供应商 = gys.name_para,
                                       备注=o.note_borrow


                                   };

                        DataTable data_table = Excel_Helper.ToDataTable(data);
                        return Excel_Exp(data_table, "借出明细");


                    }


                case "DB":
                    {
                        var data = from o in db.tb_Asset_allocation
                                   join depp in db.tb_department on o.department_allocation equals depp.ID  
                                   join od in db.tb_Asset_allocation_detail on o.ID equals od.ID_allocation
                                   join asset in db.tb_Asset on od.ID_asset equals asset.ID
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join op in db.tb_user on o._operator equals op.ID
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID
                                   join gys in db.tb_dataDict_para on asset.supplierID equals gys.ID
                                   join dz in db.tb_dataDict_para on o.addree_Storage equals dz.ID
                                   where o.serial_number == serial && o.flag == true
                                   select new
                                   {
                                       单据号 = o.serial_number,
                                       调拨日期 = o.date,
                                       调拨原因 = o.reason,
                                       调入地址 = dz.name_para,
                                       调入部门 = depp.name_Department,
                                       操作人 = op.true_Name,

                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = cf.name_para,
                                       添加方式 = zjfs.name_para,
                                       资产状态 = zczt.name_para,
                                       供应商 = gys.name_para,
                                       备注=o.ps


                                   };

                        DataTable data_table = Excel_Helper.ToDataTable(data);
                        return Excel_Exp(data_table, "调拨明细");


                    }

                case "WX":
                    {
                        var data = from o in db.tb_Asset_Repair
                                   
                                  
                                   join asset in db.tb_Asset on o.ID_Asset equals asset.ID
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                  
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID
                                   join gys in db.tb_dataDict_para on asset.supplierID equals gys.ID
                                    
                                   where o.serialNumber == serial && o.flag == true
                                   select new
                                   {
                                       单据号 = o.serialNumber,
                                       维修日期 = o.date_ToRepair,
                                       归还日期 = o.date_ToReturn,

                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = cf.name_para,
                                       添加方式 = zjfs.name_para,
                                       资产状态 = zczt.name_para,
                                       供应商 = gys.name_para,
                                       备注=o.note_repair


                                   };

                        DataTable data_table = Excel_Helper.ToDataTable(data);
                        return Excel_Exp(data_table, "维修明细");


                    }

                case "GH":
                    {
                        var data = from o in db.tb_Asset_Return

                                   join od in db.tb_Asset_Return_detail  on o.ID equals od.ID_Return
                                   join asset in db.tb_Asset on od.ID_Asset equals asset.ID
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()

                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID
                                   join gys in db.tb_dataDict_para on asset.supplierID equals gys.ID

                                   where o.serialNum == serial && o.flag == true
                                   select new
                                   {
                                       单据号 = o.serialNum,
                                     
                                       预计归还日期 = o.date_return,
                                       原因=o.reason_return,
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = cf.name_para,
                                       添加方式 = zjfs.name_para,
                                       资产状态 = zczt.name_para,
                                       供应商 = gys.name_para,
                                       备注=o.note_return


                                   };

                        DataTable data_table = Excel_Helper.ToDataTable(data);
                        return Excel_Exp(data_table, "归还明细");


                    }
                case "JS":
                    {
                        var data = from o in db.tb_Asset_Reduction
                                   join u in db.tb_user on  o.userID_apply equals u.ID
                                   join uu in db.tb_user on  o.userID_approver equals uu.ID
                                   join od in db.tb_Asset_Reduction_detail on o.ID equals od.ID_reduction
                                   join asset in db.tb_Asset on od.ID_Asset equals asset.ID
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join jsfs in db.tb_dataDict_para on o.method_reduction equals jsfs.ID
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID
                                   join gys in db.tb_dataDict_para on asset.supplierID equals gys.ID

                                   where o.Serial_number == serial && o.flag == true
                                   select new
                                   {
                                       单据号 = o.Serial_number,
                                       减少日期 = o.date_reduction,
                                       减少方式 = jsfs.name_para,
                                       申请人=u.true_Name,
                                       批准人=uu.true_Name,
                                       减少原因=o.reason_reduce,
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = cf.name_para,
                                       添加方式 = zjfs.name_para,
                                       资产状态 = zczt.name_para,
                                       供应商 = gys.name_para,
                                       备注=o.note_reduce


                                   };

                        DataTable data_table = Excel_Helper.ToDataTable(data);
                        return Excel_Exp(data_table, "减少明细");


                    }          
            }

            return View();
        }
        [HttpPost]
        public ActionResult ExportStu2(string JSdata)
        {
            int flagnum = int.Parse(JSdata);
            if (flagnum == 0)
                flagnum = 11000000;
            int name_flag = flagnum / 1000000;
            string name_flag_string = "";
            int? item_id = flagnum % 1000000;
            List<int?> Depart_Asset_Type_Id = new List<int?>();
            List<int?> AssetassettypeId = new List<int?>();


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
                var data = from r in db.tb_Asset
                           join t in db.tb_AssetType on r.type_Asset equals t.ID into temp_t
                           from tt in temp_t.DefaultIfEmpty()
                           join D in db.tb_department on r.department_Using equals D.ID into temp_D
                           from DD in temp_D.DefaultIfEmpty()
                           join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID into temp_k
                           from kk in temp_k.DefaultIfEmpty()

                           where Depart_Asset_Type_Id.Contains(r.department_Using) || item_id == 0
                           select new
                           {
                               序号 = r.ID,
                               使用部门 = DD.name_Department,
                               资产编号 = r.serial_number,
                               资产名称 = tt.name_Asset_Type,
                               规格型号 = r.specification,
                               单价 = r.unit_price,
                               数量 = r.amount,
                               总价 = r.value,
                               折旧总额 = r.depreciation_tatol,
                               折旧方式 = kk.name_para,
                               平均使用年限 = r.YearService_month,
                               净值残率 = r.Net_residual_rate,
                               月提折旧 = r.depreciation_Month,
                               净值 = r.Net_value,
                               购置日期 = r.Time_Purchase,
                               添加日期 = r.Time_add

                           };

                DataTable data_table = Excel_Helper.ToDataTable(data);
                return Excel_Exp(data_table, SystemConfig.Export_File_Name_ZJ);


            }
            if (name_flag_string == SystemConfig.nameFlag_2_ZCLB)
            {
                AssetassettypeId = comController.GetSonID_AsseType(item_id);

                var data = from r in db.tb_Asset
                           join t in db.tb_AssetType on r.type_Asset equals t.ID into temp_t
                           from tt in temp_t.DefaultIfEmpty()
                           join D in db.tb_department on r.department_Using equals D.ID into temp_D
                           from DD in temp_D.DefaultIfEmpty()
                           join k in db.tb_dataDict_para on r.Method_depreciation equals k.ID into temp_k
                           from kk in temp_k.DefaultIfEmpty()

                           where AssetassettypeId.Contains(r.type_Asset) || item_id == 0
                           select new
                           {
                               序号 = r.ID,
                               使用部门 = DD.name_Department,
                               资产编号 = r.serial_number,
                               资产名称 = tt.name_Asset_Type,
                               规格型号 = r.specification,
                               单价 = r.unit_price,
                               数量 = r.amount,
                               总价 = r.value,
                               折旧总额 = r.depreciation_tatol,
                               折旧方式 = kk.name_para,
                               平均使用年限 = r.YearService_month,
                                净值残率 = r.Net_residual_rate,
                                月提折旧 = r.depreciation_Month,
                               净值= r.Net_value,
                               购置日期 = r.Time_Purchase,
                               添加日期= r.Time_add


                           };
                DataTable data_table = Excel_Helper.ToDataTable(data);
                return Excel_Exp(data_table, SystemConfig.Export_File_Name_ZJ);
               
            }
            return View();
            
          
        }
        [HttpPost]
        public ActionResult ExportRT_Notice()
        {

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

                           序号 = r.ID,
                           剩余日期 = DbFunctions.DiffDays(DateTime.Now, r.date_return),
                           借出单号 = r.serialNum,
                           借出日期 = r.date_borrow,
                           归还日期 = r.date_return,
                           借出原因 = r.reason_borrow,
                           借出小计 = r.note_borrow,
                           借出部门 = ddp.name_Department,
                           申请人 = uub.true_Name,
                           授权人 = uuo.true_Name,
                           审核人 = uub.true_Name,




                           审核内容= r.content_review,
                           创建日期 = r.date_operated,
                           状态 = ss.Name







                       };
            DataTable data_table = Excel_Helper.ToDataTable(data);
            return Excel_Exp(data_table, SystemConfig.Export_File_Name_GH);
        }

     
        [HttpPost]
        public ActionResult ExportIV_Notice()
        {

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
                       where DbFunctions.DiffDays(DateTime.Now, r.Time_Purchase) + 30 * r.YearService_month <= SystemConfig.Days_To_Notice
                       select new
                       {

                           序号 = r.ID,
                           剩余日期 = DbFunctions.DiffDays(DateTime.Now, r.Time_Purchase) + 30 * r.YearService_month,
                           资产编号 = r.serial_number,

                           资产类别 = tt.name_Asset_Type,
                           资产名称 = r.name_Asset,
                           规格型号 = r.specification,
                           计量单位 = dd.name_para,
                           单价 = r.unit_price,
                           数量 = r.amount,
                           总价 = r.value,
                           使用部门 = pp.name_Department,
                           存放地址 = jj.name_para,
                           使用人 = uu.true_Name,
                           资产状态 = ss.name_para,
                           供应商 = ssp.name_supplier



                       };
            DataTable data_table = Excel_Helper.ToDataTable(data);
            return Excel_Exp(data_table, SystemConfig.Export_File_Name_BF);
        }

        [HttpPost]
        public ActionResult ExportRP_Notice()
        {

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

                       where DbFunctions.DiffDays(DateTime.Now, r.date_ToReturn) <= SystemConfig.Days_To_Notice && r.flag == true && ss.Name == SystemConfig.state_List_YSH
                       //&& ss.Name!=SystemConfig.state_List_TH
                       select new
                       {

                           序号 = r.ID,
                           剩余日期 = DbFunctions.DiffDays(DateTime.Now, r.date_ToReturn),
                           维修单号 = r.serialNumber,
                           维修日期 = r.date_ToRepair,
                           归还日期 = r.date_ToReturn,
                           维修原因 = r.reason_ToRepair,
                           维修小计 = r.note_repair,
                           申请人 = uuapp.true_Name,
                           授权人 = uuaut.true_Name,
                           审核人 = uuview.true_Name,
                           审核日期 = r.date_review,
                           创建人 = uucreat.true_Name,
                           装备名称 = r.Name_equipment,
                           维修费用 = r.CostToRepair,
                           创建日期 = r.date_create,
                           审核内容 = r.content_Review,
                           
                           状态 = ss.Name,

                           维修供应商 = sspl.name_supplier,
                           资产名称 = aass.name_Asset,




                       };

            DataTable data_table = Excel_Helper.ToDataTable(data);
            return Excel_Exp(data_table, SystemConfig.Export_File_Name_WX);
        }

        [HttpPost]
        public ActionResult ExportPD_Details(string JSON)
        {
            string pdsearial = JSON.Trim();
            var data = from r in db.tb_Asset_inventory_Details
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
                       where r.serial_number ==pdsearial && r.flag == true
                       select new
                       {
                           序号 = r.ID,
                           盘点编号 = r.serial_number,
                           盘点状态 = r.state,
                           系统数量 = r.amountOfSys,
                           盘点数量 = r.amountOfInv,
                           差异 = r.difference,
                           资产编号 = r.serial_number_Asset,
                           资产类型 = tt.name_Asset_Type,
                           资产名称 = aa.name_Asset,
                           规格型号 = aa.specification,
                           计量单位 = dd.name_para,
                           单价 = aa.unit_price,
                           数量 = aa.amount,
                           总价 = aa.value,
                           使用部门 = pp.name_Department,
                           存放地址 = jj.name_para,
                            使用人= uu.true_Name,
                           资产状态 = ss.name_para,
                           供应商 = ssp.name_supplier


                       };

            DataTable data_table = Excel_Helper.ToDataTable(data);
            return Excel_Exp(data_table, SystemConfig.Export_File_Name_PDDetails);
        }
        [HttpPost]
        public ActionResult Exportmodel()
        {
            string imgPath = "/SystemFile/" + "模板Excel.xlsx";
            //通过此对象获取文件名
            string AbsolutePath = Server.MapPath(imgPath);
           DataTable data= excel.ImportExcelFile(AbsolutePath);

           return Excel_Exp(data, "模板Excel");
        }
        [HttpPost]
        public ActionResult ExportStuPDMain(string JSON)
        {
            JSON = JSON.Trim();

            string[] temp = JSON.Split(',');
            List<tb_Asset_inventory> list = db.tb_Asset_inventory.ToList();
            DateTime BeginDate = Convert.ToDateTime("8888-12-12" + " 00:00:00");
            DateTime EndDate = Convert.ToDateTime("0001-01-01" + " 23:59:59");

            int Effective_Query_Num = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != SystemConfig.NullString_Replace)
                {
                   
                    Effective_Query_Num++;
                }
            }
            string searial = temp[0];
            if (temp[1] != SystemConfig.NullString_Replace)
                BeginDate = DateTime.Parse(temp[1]);

            if (temp[2] != SystemConfig.NullString_Replace)
                EndDate = DateTime.Parse(temp[2]);

            string PDstate = temp[3];
            string PDperson = temp[4]; 

           switch (Effective_Query_Num)
           {
               case 0:
                   {




                       var data = from r in db.tb_Asset_inventory
                                  where r.flag == true
                                  select new
                                  {
                                      序号 = r.ID,
                                      盘点单号 = r.serial_number,
                                      日期 = r.date,
                                      盘点人 = r._operator,
                                      系统数量 = r.amountOfSys,
                                      盘点数量 = r.amountOfInv,
                                      差异 = r.difference,
                                      资产性质 = r.property,
                                      资产状态 = r.state,
                                      创建日期 = r.date_Create,
                                      备注 = r.ps
                                  };
                       DataTable data_table = Excel_Helper.ToDataTable(data);
                       return Excel_Exp(data_table, SystemConfig.Export_File_Name_PDMain);




                   }
               case 2:
                   {
                       var data = from r in db.tb_Asset_inventory
                                  where r.state == PDstate && r._operator == PDperson && r.flag == true
                                  select new
                                  {

                                      序号 = r.ID,
                                      盘点单号 = r.serial_number,
                                      日期 = r.date,
                                      盘点人 = r._operator,
                                      系统数量 = r.amountOfSys,
                                      盘点数量 = r.amountOfInv,
                                      差异 = r.difference,
                                      资产性质 = r.property,
                                      资产状态 = r.state,
                                      创建日期 = r.date_Create,
                                      备注 = r.ps
                                  };

                       DataTable data_table = Excel_Helper.ToDataTable(data);
                       return Excel_Exp(data_table, SystemConfig.Export_File_Name_PDMain);


                   }
               case 3:
                   {
                       var data = from r in db.tb_Asset_inventory
                                  where (r.state == PDstate && r._operator == PDperson && BeginDate <= r.date && r.flag == true)
                                  || (r.state == PDstate && r._operator == PDperson && EndDate >= r.date && r.flag == true)
                                  || (r.state == PDstate && r._operator == PDperson && r.serial_number.Contains(searial) && r.flag == true)

                                  select new
                                  {
                                      序号 = r.ID,
                                      盘点单号 = r.serial_number,
                                      日期 = r.date,
                                      盘点人 = r._operator,
                                      系统数量 = r.amountOfSys,
                                      盘点数量 = r.amountOfInv,
                                      差异 = r.difference,
                                      资产性质 = r.property,
                                      资产状态 = r.state,
                                      创建日期 = r.date_Create,
                                      备注 = r.ps

                                  };
                       DataTable data_table = Excel_Helper.ToDataTable(data);
                       return Excel_Exp(data_table, SystemConfig.Export_File_Name_PDMain);

                   }
               case 4:
                   {
                       var data = from r in db.tb_Asset_inventory
                                  where (r.state == PDstate && r._operator == PDperson && EndDate >= r.date && r.date >= BeginDate && r.flag == true)
                                  || (r.state == PDstate && r._operator == PDperson && EndDate >= r.date && searial.Contains(searial) && r.flag == true)
                                  || (r.state == PDstate && r._operator == PDperson && BeginDate <= r.date && r.serial_number.Contains(searial) && r.flag == true)
                                  select new
                                  {

                                      序号 = r.ID,
                                      盘点单号 = r.serial_number,
                                      日期 = r.date,
                                      盘点人 = r._operator,
                                      系统数量 = r.amountOfSys,
                                      盘点数量 = r.amountOfInv,
                                      差异 = r.difference,
                                      资产性质 = r.property,
                                      资产状态 = r.state,
                                      创建日期 = r.date_Create,
                                      备注 = r.ps

                                  };
                       DataTable data_table = Excel_Helper.ToDataTable(data);
                       return Excel_Exp(data_table, SystemConfig.Export_File_Name_PDMain);



                   }
               case 5:
                   {
                       var data = from r in db.tb_Asset_inventory
                                  where r.serial_number.Contains(searial) && BeginDate <= r.date && r.date <= EndDate && r.state == PDstate && r._operator == PDperson && r.flag == true
                                  select new
                                  {

                                      序号 = r.ID,
                                      盘点单号 = r.serial_number,
                                      日期 = r.date,
                                      盘点人 = r._operator,
                                      系统数量 = r.amountOfSys,
                                      盘点数量 = r.amountOfInv,
                                      差异 = r.difference,
                                      资产性质 = r.property,
                                      资产状态 = r.state,
                                      创建日期 = r.date_Create,
                                      备注 = r.ps

                                  };
                       DataTable data_table = Excel_Helper.ToDataTable(data);
                       return Excel_Exp(data_table,SystemConfig.Export_File_Name_PDMain);



                   }

                  


           }

           return View();

        }
        public ActionResult Excel_Exp(DataTable data,string name_file)
        {

            
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("Sheet1");

            //貌似这里可以设置各种样式字体颜色背景等，但是不是很方便，这里就不设置了

            //给sheet1添加第一行的头部标题
            int count = 0;
            try
            {


                if (true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (int j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }


                for (int i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (int j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }

            }
            catch (Exception ex)
            {
                ;

            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            DateTime dt = DateTime.Now;
            string dateTime = dt.ToString("yyMMddHHmmssfff");
            string fileName = name_file + dateTime + ".xls";
            
           // print.Print(Encoding.UTF8.GetBytes(ms.ToArray()));
            return File(ms, "application/vnd.ms-excel", fileName);
        
        }
        public ActionResult Inventory()
        {
            return View();
        }
        public ActionResult New_Deatails()
        {
            return View();
        }
        public ActionResult Deatails_View()
        {
            return View();
        }
        public ActionResult AddExcel()
        {
            return View();
        }
       
    }
}