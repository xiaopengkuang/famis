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

    }
}