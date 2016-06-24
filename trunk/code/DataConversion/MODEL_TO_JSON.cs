using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAMIS.Models;
using FAMIS.DTO;
namespace FAMIS.DataConversion
{
    public class MODEL_TO_JSON
    {

        public Json_AssetType_add ConverMdoelToJson(tb_AssetType tb)
        {
            Json_AssetType_add data = new Json_AssetType_add();
            data.id = tb.ID;
            data.lbbh = tb.assetTypeCode;
            data.sjlb=tb.father_MenuID_Type;
            data._parentId = tb.father_MenuID_Type;
            data.jldw=tb.measurement;
            data.zjfs= tb.method_Depreciation;
            data.lbmc=tb.name_Asset_Type;
            data.jczl = tb.Net_residual_rate;
            data.zjnx= tb.period_Depreciation;
            //data.level = tb.treeLevel;
            return data;
        }
        public List<Json_AssetType_add> ConverMdoelToJsonList(List<tb_AssetType> tb_list)
        {
            List<Json_AssetType_add> list = new List<Json_AssetType_add>();
            list.Clear();
            for (int i = 0; i < tb_list.Count; i++)
            {
                list.Add(ConverMdoelToJson(tb_list[i]));
            }
            return list;
        }
    }
}