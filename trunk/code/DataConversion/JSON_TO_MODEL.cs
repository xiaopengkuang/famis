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

    }
}