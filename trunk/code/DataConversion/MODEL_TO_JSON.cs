using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using FAMIS.ViewCommon;
using FAMIS.DAL;
using System.Web.Script.Serialization;
using FAMIS.Models;
using System.Runtime.Serialization.Json;
using FAMIS.DTO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using FAMIS.DataConversion;
namespace FAMIS.DataConversion
{
    public class MODEL_TO_JSON : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();

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
            data.lastEditTime = tb.lastEditTime;
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


        /// <summary>
        /// 将ID转换成相应的文本字段
        ///这个方法是不合适的  或者说还没完事好
        /// </summary>
        /// <param name="data_ORG"></param>
        /// <returns></returns>
        public JsonResult ConverMdoelToJsonList(IQueryable<tb_Asset> data_ORG, int skipindex, int rowNeed)
        {
            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                       from DZ in temp_DZ.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                       from SP in temp_SP.DefaultIfEmpty()
                       join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
                       from MDP in temp_MDP.DefaultIfEmpty()
                       join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
                       from MDC in temp_MDC.DefaultIfEmpty()
                       join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
                       from MA in temp_MA.DefaultIfEmpty()
                       select new dto_Asset_Detail
                       {
                           addressCF = DZ.name_para,
                           amount = p.amount.ToString(),
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol.ToString(),
                           depreciation_Month = p.depreciation_Month.ToString(),
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate.ToString(),
                           Net_value = p.Net_value.ToString(),
                           //people_using = p.people_using,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price.ToString(),
                           value = p.value.ToString(),
                           YearService_month = p.YearService_month.ToString()
                       };

            //int skipindex = ((int)page - 1) * (int)rows;

            //int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.ToList().ToArray()

                //rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}