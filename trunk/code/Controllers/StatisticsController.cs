using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FAMIS.Models;
using FAMIS.DataConversion;
using FAMIS.DTO;

namespace FAMIS.Controllers
{
    public class StatisticsController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController comController = new CommonController();
        // GET: Statistics
        public ActionResult ST_assetType()
        {
            return View();
        }


        public ActionResult ST_department()
        {
            return View();
        }



        /// <summary>
        /// 根据查询条件加载资产数据接口
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="tableType"></param>
        /// <param name="searchCondtiion"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        [HttpPost]
        public String LoadAssets(int? page, int? rows, int tableType, String searchCondtiion, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? role = commonConversion.getRoleID();

            String result = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_ST dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_ST>(searchCondtiion);
            }

            List<int> selectedIDs = new List<int>();
            result = loadAsset_By_Type(page, rows, role, dto_condition, selectedIDs, tableType, exportFlag);
            return result;
        }

        /// <summary>
        /// 加载资产数据获取方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="dto_condition"></param>
        /// <param name="selectedIDs"></param>
        /// <param name="dataType"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        public String loadAsset_By_Type(int? page, int? rows, int? role, dto_SC_ST dto_condition, List<int> selectedIDs, int dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            String json = "";

            JavaScriptSerializer jss = new JavaScriptSerializer();


            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);

            json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs, dataType, exportFlag);
            return json;
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="cond"></param>
        /// <param name="idsRight_deparment"></param>
        /// <param name="idsRight_assetType"></param>
        /// <param name="selectedIDs"></param>
        /// <param name="dataType"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        public String loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_ST cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs, int? dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset)
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);
            if (cond == null)
            {

            }
            else
            {
                //资产性质过滤
                if (cond.Type_AssetType != null)
                {
                    //获取其子节点
                    List<int?> ids = comController.GetSonID_AsseType(cond.Type_AssetType);
                    if (!ids.Contains(cond.Type_AssetType))
                    {
                        ids.Add(cond.Type_AssetType);
                    }
                    data_ORG = from p in data_ORG
                               where ids.Contains(p.type_Asset)
                               select p;
                }

                //日期过滤
                if(cond.beginDate==null || cond.beginDate==""||cond.endDate==null||cond.endDate=="")
                {

                }else
                {
                    DateTime beginTime = new DateTime();
                    DateTime endTime = new DateTime();
                    try{
                         beginTime = Convert.ToDateTime((DateTime.Parse(cond.beginDate)).ToString("yyyy-MM-dd") + " 00:00:00");
                         endTime = Convert.ToDateTime((DateTime.Parse(cond.endDate)).ToString("yyyy-MM-dd") + " 23:59:59");
                    }catch(Exception e)
                    {

                    }
                    switch (cond.dateType)
                    {
                        case SystemConfig.searchCondition_DJRQ:
                            {
                                data_ORG = from p in data_ORG
                                           where p.Time_add > beginTime && p.Time_add < endTime
                                           select p;
                            }; break;

                        case SystemConfig.searchCondition_GZRQ:
                            {
                                data_ORG = from p in data_ORG
                                           where p.Time_Purchase > beginTime && p.Time_Purchase < endTime
                                           select p;
                            }; break;
                        default: ; break;
                    }
                }
                //部门过滤
                if (cond.department == null)
                {

                }
                else
                {
                    
                    data_ORG = from p in data_ORG
                               where p.department_Using == cond.department
                               select p;
                }
                //资产类别过滤
                if (cond.AssetType == null)
                {

                }
                else
                {
                   //TODO：现在不包含子节点

                    data_ORG = from p in data_ORG
                               where p.type_Asset == cond.AssetType
                               select p;
                }

                //供应商过滤
                if (cond.supplier == null)
                {
                    
                }
                else {
                    data_ORG = from p in data_ORG
                               where p.supplierID == cond.supplier
                               select p;
                }

                    

            }

            switch (dataType)
            {
                case SystemConfig.tableType_detail:
                    {
                        //在进行数据绑定
                        var data = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
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
                                   join tb_BC in DB_C.tb_Asset_code128 on p.ID equals tb_BC.ID_Asset into temp_BC
                                   from BC in temp_BC.DefaultIfEmpty()
                                   select new dto_Asset_Detail
                                   {
                                       addressCF = DZ.name_para,
                                       amount = p.amount.ToString(),
                                       department_Using = DP.name_Department.ToString(),
                                       depreciation_tatol = p.depreciation_tatol.ToString(),
                                       depreciation_Month = p.depreciation_Month.ToString(),
                                       ID = p.ID,
                                       Method_add = MA.name_para,
                                       Method_depreciation = MDP.name_para,
                                       Method_decrease = MDC.name_para,
                                       name_Asset = p.name_Asset,
                                       Net_residual_rate = p.Net_residual_rate.ToString(),
                                       Net_value = p.Net_value.ToString(),
                                       Time_Operated = p.Time_add,
                                       serial_number = p.serial_number,
                                       specification = p.specification,
                                       state_asset = ST.name_para,
                                       supplierID = SP.name_supplier,
                                       Time_Purchase = p.Time_Purchase,
                                       type_Asset = AT.name_Asset_Type,
                                       unit_price = p.unit_price,
                                       value = p.value.ToString(),
                                       YearService_month = p.YearService_month.ToString(),
                                       barcode = BC.code128,
                                       note = p.note
                                   };
                        data = data.OrderByDescending(a => a.Time_Operated);
                        if (exportFlag != null && exportFlag == true)
                        {
                            String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                            return json_result;
                        }
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        };
                        String result = jss.Serialize(json).ToString().Replace("\\", "");
                        return result;

                    }; break;
                case SystemConfig.tableType_summary:
                    {
                        //在进行数据绑定
                        var data_ORG2 = from p in data_ORG
                                        join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                        from AT in temp_AT.DefaultIfEmpty()
                                        join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID  into temp_MM
                                        from MM in temp_MM.DefaultIfEmpty()
                                        join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                                        from DP in temp_DP.DefaultIfEmpty()
                                        select new
                                        {
                                            name_Asset = p.name_Asset,
                                            type_Asset = p.type_Asset,
                                            measurement = p.measurement,
                                            department_Using=DP.name_Department==null?"":DP.name_Department,
                                            specification = p.specification,
                                            type_Asset_name = AT.name_Asset_Type,
                                            measurement_name = MM.name_para==null?"":MM.name_para,
                                            amount = p.amount,
                                            value = p.value
                                        };
                        //数据分组
                        if (cond==null||cond.orderType == null)
                        {
                            return NULL_dataGridSTring();
                        }
                        if ( cond.orderType == SystemConfig.ORDER_TYPE_DEPARTMENT)
                        {
                            var data = (from a in data_ORG2
                                    group a by new {a.department_Using, a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                    select new 
                                    {
                                        amount = b.Sum(a => a.amount).ToString(),
                                        AssetName = b.Key.name_Asset,
                                        department_Using=b.Key.department_Using,
                                        AssetType = b.Key.type_Asset_name,
                                        measurement = b.Key.measurement_name,
                                        specification = b.Key.specification,
                                        value = b.Sum(a => a.value).ToString()
                                    }).Distinct();
                            data = data.OrderByDescending(a => a.AssetName);
                            if (exportFlag != null && exportFlag == true)
                            {
                                String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                                return json_result;
                            }
                            int skipindex = ((int)page - 1) * (int)rows;
                            int rowsNeed = (int)rows;
                            var json = new
                            {
                                total = data.ToList().Count,
                                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            };
                            String result = jss.Serialize(json).ToString().Replace("\\", "");
                            return result;
                        }
                        else if (cond.orderType == SystemConfig.ORDER_TYPE_ASSETTYPE)
                        {
                            var data = (from a in data_ORG2
                                        group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                        select new
                                        {
                                            amount = b.Sum(a => a.amount).ToString(),
                                            AssetName = b.Key.name_Asset,
                                            AssetType = b.Key.type_Asset_name,
                                            measurement = b.Key.measurement_name,
                                            specification = b.Key.specification,
                                            value = b.Sum(a => a.value).ToString()
                                        }).Distinct();
                            data = data.OrderByDescending(a => a.AssetType);
                            if (exportFlag != null && exportFlag == true)
                            {
                                String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                                return json_result;
                            }
                            int skipindex = ((int)page - 1) * (int)rows;
                            int rowsNeed = (int)rows;
                            var json = new
                            {
                                total = data.ToList().Count,
                                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            };
                            String result = jss.Serialize(json).ToString().Replace("\\", "");
                            return result;
                        }else{
                            return NULL_dataGridSTring();
                        }
                        
                    }; break;
                default:
                    {
                        return NULL_dataGridSTring();
                    }; break;
            }
        }

        public String NULL_dataGridSTring()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var json = new
            {
                total = 0,
                rows = ""
            };
            String result = jss.Serialize(json).ToString().Replace("\\", "");
            return result;
        }

    }






}