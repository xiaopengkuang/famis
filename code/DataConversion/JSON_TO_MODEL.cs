using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAMIS.Models;
using FAMIS.DTO;

namespace FAMIS.DataConversion
{
    public class JSON_TO_MODEL
    {
        public tb_AssetType ConverJsonToTable(Json_AssetType_add data)
        {
            tb_AssetType tb=new tb_AssetType();
            tb.assetTypeCode = data.lbbh;
            tb.father_MenuID_Type = data.sjlb;
            tb.measurement = data.jldw;
            tb.method_Depreciation = data.zjfs;
            tb.name_Asset_Type = data.lbmc;
            tb.Net_residual_rate = data.jczl;
            tb.period_Depreciation = data.zjnx;
            tb.treeLevel = data.level;
            return tb;
        }

        public tb_department ConverJsonToTable(Json_department data)
        {
            tb_department tb=new tb_department();
            tb.CODE_Department=data.bmbh;
            tb.name_Department = data.bmmc;
            tb.treeLevel = data.level;
            tb.ID_Father_Department = data.sjbm;
            return tb;
        }

        public tb_customAttribute ConverJsonToTable(Json_customAttr data)
        {
            tb_customAttribute tb = new tb_customAttribute();
            tb.assetTypeID = data.zclb;
            tb.flag = data.flag;
            tb.length = data.zdcd;
            tb.necessary = data.sfbs;
            tb.SYSID = data.xtid;
            tb.title = data.sxbt;
            tb.type = data.sxlx;
            tb.type_value = data.glzd;
            return tb;
        }

        public tb_dataDict ConverJsonToTable(Json_dataDict data)
        {
            tb_dataDict tb = new tb_dataDict();
            tb.name_dataDict = data.csmc;
            tb.father_ID = data.cslx;
            tb.isTree = data.isTree;
            tb.treeLevel = data.level;
            return tb;
        }


        public tb_dataDict_para ConverJsonToTable(Json_dataDict_Para data)
        {
            tb_dataDict_para tb = new tb_dataDict_para();
            tb.name_para = data.csmc;
            tb.ID_dataDict = data.cslx;
            tb.description = data.csms;
            tb.fatherid = data.pid;
            return tb;
        }

        public tb_supplier ConverJsonToTable(Json_supplier data)
        {
            tb_supplier tb = new tb_supplier();
            tb.address = data.DZ;
            tb.email = data.YX;
            tb.fax = data.CZ;
            tb.linkman = data.LXR;
            tb.name_supplier = data.GYSMC;
            tb.phoneNumber = data.LXDH;
            return tb;
        }



        public tb_Asset ConverJsonToTable(Json_Asset_add data)
        {
            tb_Asset tb_asset_add = new tb_Asset();
            tb_asset_add.serial_number = data.d_ZCBH_add;
            tb_asset_add.name_Asset = data.d_ZCMC_add;
            tb_asset_add.type_Asset = data.d_ZCLB_add;
            tb_asset_add.specification = data.d_ZCXH_add;
            tb_asset_add.measurement = data.d_JLDW_add;
            tb_asset_add.unit_price = data.d_Other_ZCDJ_add;
            tb_asset_add.amount = data.d_Other_ZCSL_add;
            tb_asset_add.value = data.d_Other_ZCJZ_add;
            tb_asset_add.department_Using = data.d_SZBM_add;
            tb_asset_add.addressCF = data.d_CFDD_add;
            tb_asset_add.flag = data.flag;
            tb_asset_add.Time_add = data.OperateTime;
            tb_asset_add.supplierID = data.d_GYS_add;
            tb_asset_add.Time_Purchase = data.d_GZRQ_add;
            tb_asset_add.YearService_month = data.d_Other_SYNX_add;
            tb_asset_add.Method_depreciation = data.d_Other_ZJFS_add;
            tb_asset_add.Net_residual_rate = data.d_Other_JCZL_add;
            tb_asset_add.depreciation_Month = data.d_Other_YTZJ_add;
            tb_asset_add.depreciation_tatol = data.d_Other_LJZJ_add;
            tb_asset_add.Net_value = data.d_Other_JZ_add;
            tb_asset_add.Method_add = data.d_ZJFS_add;
            tb_asset_add.Owener = data.d_SYRY_add;
            return tb_asset_add;
        }


        public tb_Asset_collar ConverJsonToTable(Json_collar_add data)
        {
            tb_Asset_collar collar = new tb_Asset_collar();
            collar.reason = data.reason_LY;
            collar.department_collar = data.department_LY;
            collar.date = data.date_LY;
            collar.addree_Storage = data.address_LY;
            collar.ps = data.ps_LY;
            collar.user_collar = data.user_collar;
            return collar;

        }

        public tb_Asset_allocation ConverJsonToTable(Json_allocation_add data)
        {
            tb_Asset_allocation allocation = new tb_Asset_allocation();
            allocation.addree_Storage = data.address;
            allocation.date = data.date_allocation;
            allocation.department_allocation = data.department;
            allocation.ps = data.ps;
            allocation.reason = data.reason;
            allocation.user_allocation = data.user_allocation;
            return allocation;
        }



        public tb_Asset_Repair ConverJsonToTable(Json_repair_add data)
        {
            tb_Asset_Repair repair = new tb_Asset_Repair();
            repair.CostToRepair = data.Cost_Repair;
            repair.date_ToRepair = data.dateToP;
            repair.date_ToReturn = data.dateToR;
            repair.ID_Asset = data.id_asset_repair;
            repair.Name_equipment = data.name_Equipment;
            repair.note_repair = data.note_add;
            repair.reason_ToRepair = data.reason_add;
            repair.supplierID_Torepair = data.supplier_repair;
            repair.userID_applying = data.UAP_add;
            repair.userID_authorize = data.UAT_add;
            return repair;
        }


        public tb_Asset_Reduction ConverJsonToTable(Json_reduction_add data)
        {
            tb_Asset_Reduction tb = new tb_Asset_Reduction();
            tb.date_reduction = data.date_reduction;
            tb.method_reduction = data.method_reduction;
            tb.note_reduce = data.note;
            tb.reason_reduce = data.reason;
            tb.userID_apply = data.user_apply;
            tb.userID_approver = data.user_approve;
            return tb;
        }

    }
}