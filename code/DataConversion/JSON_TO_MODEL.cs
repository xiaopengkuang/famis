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
            tb.ID_Department = data.bmbh;
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

    }
}