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
namespace FAMIS.Controllers
{
    public class CollarController : Controller
    {
        // GET: Collar
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult add_collarView()
        {
            return View("add_collar");
        }

          public ActionResult add_collar()
        {
            return View();
        }
        public ActionResult collar()
        {
            return View();
        }
        public ActionResult edit_collarView(int? id)
        {
            if(id==null)
            {
                return View("Error");
            }

            ViewBag.id = id;
            return View("edit_collar");
        }

        public ActionResult collar_selectAsset()
        {
            return View("collar_selectAsset");
        }

        //======================================================================================//
        [HttpPost]
        public JsonResult LoadCollars(int? page, int? rows, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Allocation dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Allocation>(searchCondtiion);
            }
            return LoadCollarsList(page, rows, dto_condition);
        }


        /// <summary>
        /// 获取当前资产闲置的
        /// 同时排除已经被选择的资产
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="searchCondtiion"></param>
        /// <param name="selectedIDs"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadAsset_collor(int? page, int? rows, String searchCondtiion, String selectedIDs)
        {
            List<int> ids_Gone = commonConversion.StringToIntList(selectedIDs);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            JsonResult json = new JsonResult();

            int? role = commonConversion.getRole();
           
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);
            if (dto_condition == null)
            {
                json = loadAssetByDataDict_collar(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone);
            }
            else {
                switch (dto_condition.typeFlag)
                {
                    case SystemConfig.searchPart_letf: json = loadAssetByDataDict_collar(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone); break;
                    case SystemConfig.searchPart_right: json = loadAssetByLikeCondition_collar(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone); break;
                    default: ; break;
                }
            }

           
            return json;
        }

        public JsonResult loadAssetByDataDict_collar(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int> selectedIDs)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where p.department_Using == null || idsRight_deparment.Contains(p.department_Using)
                           where idsRight_assetType.Contains(p.type_Asset)
                           where !selectedIDs.Contains(p.ID)
                           join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                           from ST in temp_ST.DefaultIfEmpty()
                           where ST.name_para == SystemConfig.state_asset_free || ST.name_para == "" || ST.name_para == null
                           select p;
            if (data_ORG == null)
            {
                return null;
            }

            if (cond != null)
            {
                int nodeid = (int)cond.nodeID;
                int dicID = nodeid / SystemConfig.ratio_dictPara;
                int dic_paraID = nodeid - (SystemConfig.ratio_dictPara * dicID);
                //获取DicNameFlag
                var data_nameFlag = from p in DB_C.tb_dataDict
                                    where p.active_flag == true
                                    where p.ID == dicID
                                    where p.name_flag != null
                                    select new
                                    {
                                        nameFlag = p.name_flag
                                    };

                String nameFlag = null;
                foreach (var item in data_nameFlag)
                {
                    nameFlag = item.nameFlag;
                }

                if (nameFlag == null)
                {
                    return null;
                }



                if (commonConversion.isALL(cond.nodeText) || dic_paraID == 0)
                {
                }
                else
                {
                    switch (nameFlag)
                    {
                        case SystemConfig.nameFlag_2_CFDD:
                            {
                                data_ORG = from p in data_ORG
                                           where p.addressCF == dic_paraID
                                           select p;
                            }; break;

                        case SystemConfig.nameFlag_2_SYBM:
                            {
                                data_ORG = from p in data_ORG
                                           where p.department_Using == dic_paraID
                                           select p;
                            }; break;

                        case SystemConfig.nameFlag_2_ZCLB:
                            {
                                data_ORG = from p in data_ORG
                                           where p.type_Asset == dic_paraID
                                           select p;
                            }; break;
                    }
                }
            }
            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
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
                           amount = p.amount,
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol,
                           depreciation_Month = p.depreciation_Month,
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate,
                           Net_value = p.Net_value,
                           Time_Operated = p.Time_add,
                           //people_using = p.people_using,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price,
                           value = p.value,
                           YearService_month = p.YearService_month
                       };
            data = data.OrderByDescending(a => a.Time_Operated);

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


        public JsonResult loadAssetByLikeCondition_collar(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType,List<int?> idsRight_deparment, List<int> selectedIDs)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where p.department_Using == null || idsRight_deparment.Contains(p.department_Using)
                           where idsRight_assetType.Contains(p.type_Asset)
                           where !selectedIDs.Contains(p.ID)
                           join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                           from ST in temp_ST.DefaultIfEmpty()
                           where ST.name_para == SystemConfig.state_asset_free || ST.name_para == "" || ST.name_para == null
                           select p;
            if (data_ORG == null)
            {
                return null;
            }
            if (cond != null)
            {
                int nodeid = (int)cond.nodeID;
                int dicID = nodeid / SystemConfig.ratio_dictPara;
                int dic_paraID = nodeid - (SystemConfig.ratio_dictPara * dicID);
                //获取DicNameFlag
                var data_nameFlag = from p in DB_C.tb_dataDict
                                    where p.active_flag == true
                                    where p.ID == dicID
                                    where p.name_flag != null
                                    select new
                                    {
                                        nameFlag = p.name_flag
                                    };

                String nameFlag = null;
                foreach (var item in data_nameFlag)
                {
                    nameFlag = item.nameFlag;
                }

                if (nameFlag == null)
                {
                    return null;
                }


                switch (cond.DataType)
                {
                    case SystemConfig.searchCondition_Date:
                        {
                            DateTime beginTime = Convert.ToDateTime(((DateTime)cond.begin).ToString("yyyy-MM-dd") + " 00:00:00");
                            DateTime endTime = Convert.ToDateTime(((DateTime)cond.end).ToString("yyyy-MM-dd") + " 23:59:59");
                            switch (cond.dataName)
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
                        }; break;
                    case SystemConfig.searchCondition_Content:
                        {
                            switch (cond.dataName)
                            {
                                case SystemConfig.searchCondition_ZCBH:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.serial_number.Contains(cond.contentSC)
                                                   select p;
                                    }; break;

                                case SystemConfig.searchCondition_ZCMC:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.name_Asset.Contains(cond.contentSC)
                                                   select p;
                                    }; break;

                                case SystemConfig.searchCondition_ZCXH:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.specification.Contains(cond.contentSC)
                                                   select p;
                                    }; break;

                                default: ; break;
                            }
                        }; break;
                    default: ; break;
                }
            }
            
            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
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
                           amount = p.amount,
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol,
                           depreciation_Month = p.depreciation_Month,
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate,
                           Net_value = p.Net_value,
                           Time_Operated = p.Time_add,
                           //people_using = p.people_using,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price,
                           value = p.value,
                           YearService_month = p.YearService_month
                       };
            data = data.OrderByDescending(a => a.Time_Operated);

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


        public JsonResult LoadCollarsList(int? page, int? rows, dto_SC_Allocation cond)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? roleID = commonConversion.getRole();

            //获取部门ID
            List<int?> idsRight_department = commonConversion.getids_departmentByRole(roleID);


            var data = from p in DB_C.tb_Asset_collar
                       where p.flag == true
                       where p.department_collar == null || idsRight_department.Contains(p.department_collar)
                       join tb_DP in DB_C.tb_department on p.department_collar equals tb_DP.ID_Department into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_State_List on p.state_List equals tb_ST.id into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_AD in DB_C.tb_dataDict_para on p.addree_Storage equals tb_AD.ID into temp_AD
                       from AD in temp_AD.DefaultIfEmpty()
                       join tb_US in DB_C.tb_user on p._operator equals tb_US.ID into temp_US
                       from US in temp_US.DefaultIfEmpty()
                       orderby p.date_Operated descending
                       select new Json_collar { 
                            ID=p.ID,
                            address=AD.name_para,
                            date_Operated=p.date_Operated,
                            date_collar=p.date,
                            department=DP.name_Department,
                            operatorUser=US.name_User,
                            serialNumber=p.serial_number,
                            state=ST.Name
                       };
            
            if (cond != null)
            {

            }

            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.Count(),
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 领用只能针对未分配的部门进行领用
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="idStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Load_SelectedAsset(int? page, int? rows, String selectedIDs)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            //int? role = commonConversion.getRole();
            ////获取资产类别权限
            //List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
            ////获取部门权限
            //List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);

            List<int> ids_selected = commonConversion.StringToIntList(selectedIDs);

            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           //where p.department_Using == null || idsRight_deparment.Contains(p.department_Using)
                           //where idsRight_assetType.Contains(p.type_Asset)
                           where ids_selected.Contains(p.ID)
                           select p;
            if (data_ORG.Count()<1)
            {
                return null;
            }

            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
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
                           amount = p.amount,
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol,
                           depreciation_Month = p.depreciation_Month,
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate,
                           Net_value = p.Net_value,
                           Time_Operated = p.Time_add,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price,
                           value = p.value,
                           YearService_month = p.YearService_month
                       };
            data = data.OrderByDescending(a => a.Time_Operated);

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















        



      

        ////public JsonResult Load_Asset_By_CollarID(int? page, int? rows, int flag, int ID)
        ////{
        ////    page = page == null ? 1 : page;
        ////    rows = rows == null ? 15 : rows;
        ////    List<tb_Asset_collar> collar = DB_C.tb_Asset_collar.Where(a => a.ID == ID).ToList();
        ////    if (collar.Count != 1)
        ////    {
        ////        List<dto_Asset_Detail> list_null = new List<dto_Asset_Detail>();
        ////        var json_NULL = new
        ////        {
        ////            total = 0,
        ////            rows = list_null.ToArray()
        ////            //sql = selectSQL

        ////        };

        ////        return Json(json_NULL, JsonRequestBehavior.AllowGet);
        ////    }
        ////    String serNum = collar[0].serial_number;
        ////    List<tb_Asset_collar_detail> acd = DB_C.tb_Asset_collar_detail.Where(b => b.flag == true).Where(a => a.serial_number == serNum).ToList();

        ////    String cond = getSQLCond_SerialNumber(acd);

        ////    String selectSQL = getSelectAssets(flag, cond, rows, page);
        ////    String selectSQLCounter = getSelectAsset_Counter(flag, cond);


        ////    SQLRunner sqlRuner = new SQLRunner();
        ////    DataTable dt = sqlRuner.runSelectSQL_dto(selectSQL);
        ////    int resultCount = sqlRuner.runSelectSQL_Counter(selectSQLCounter, "total");
        ////    List<dto_Asset_Detail> list = new List<dto_Asset_Detail>();
        ////    for (int i = 0; i < dt.Rows.Count; i++)
        ////    {
        ////        dto_Asset_Detail tmp = new dto_Asset_Detail();

        ////        tmp.RowNo = int.Parse(dt.Rows[i]["RowNo"].ToString());
        ////        tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
        ////        tmp.serial_number = dt.Rows[i]["serial_number"].ToString();
        ////        tmp.name_Asset = dt.Rows[i]["name_Asset"].ToString();
        ////        tmp.type_Asset = dt.Rows[i]["type_Asset"].ToString();
        ////        tmp.specification = dt.Rows[i]["specification"].ToString();
        ////        tmp.people_using = dt.Rows[i]["specification"].ToString();
        ////        tmp.department_Using = dt.Rows[i]["department_Using"].ToString();
        ////        tmp.measurement = dt.Rows[i]["measurement"].ToString();
        ////        tmp.unit_price = double.Parse(dt.Rows[i]["unit_price"].ToString());
        ////        tmp.addressCF = dt.Rows[i]["addressCF"].ToString();
        ////        tmp.amount = int.Parse(dt.Rows[i]["amount"].ToString());
        ////        tmp.value = double.Parse(dt.Rows[i]["value"].ToString());
        ////        tmp.state_asset = dt.Rows[i]["state_asset"].ToString();
        ////        tmp.supplierID = dt.Rows[i]["supplierID"].ToString();
        ////        tmp.Method_add = dt.Rows[i]["Method_add"].ToString();
        ////        list.Add(tmp);
        ////    }
        ////    var json = new
        ////    {
        ////        total = resultCount,
        ////        rows = list.ToArray()
        ////        //sql = selectSQL

        ////    };

        ////    return Json(json, JsonRequestBehavior.AllowGet);

        ////}






        ////public List<dto_collar_detail> LoadCollars_By_ID_LIST(List<int> ids)
        ////{
        ////    SQLRunner sqlRuner = new SQLRunner();
        ////    String sql = GetCollarSelectSQL(ids);
        ////    DataTable dt = sqlRuner.runSelectSQL_dto(sql);
        ////    return converTo_dto_Collar_detail(dt);

        ////}

        ////public dto_collar_detail loadCollarByID(int id)
        ////{
        ////    List<int> ids = new List<int>();
        ////    ids.Add(id);
        ////    List<dto_collar_detail> list = new List<dto_collar_detail>();
        ////    list = LoadCollars_By_ID_LIST(ids);
        ////    if (list.Count == 1)
        ////    {
        ////        return list[0];
        ////    }
        ////    else
        ////    {
        ////        return new dto_collar_detail();
        ////    }

        ////}


        ////[HttpPost]
        ////public int deleteCollars(List<int> selectedIDs)
        ////{
        ////    String deleteSQL = getDeleteCollarSQL(selectedIDs);
        ////    SQLRunner sqlRunner = new SQLRunner();
        ////    int result = sqlRunner.executesql(deleteSQL);
        ////    return result;
        ////}

        ////public int InsertNewCollor(String collar_add)
        ////{
        ////    int insertNum = 0;
        ////    JavaScriptSerializer serializer = new JavaScriptSerializer();
        ////    Json_Collar_addNew Json_collar = serializer.Deserialize<Json_Collar_addNew>(collar_add);

        ////    CommonController cct = new CommonController();

        ////    String serialNumber_collar = cct.getLatestOneSerialNumber("LY");
        ////    DateTime dateNow = new DateTime();

        ////    int operatorID = 1;
        ////    int flag = Json_collar.flag == null ? 1 : (int)Json_collar.flag;

        ////    tb_Asset_collar collar_new = ConvertJsonTo_CollorTB(Json_collar, serialNumber_collar, true, dateNow, operatorID);
        ////    List<tb_Asset_collar> collarsList = new List<tb_Asset_collar>();
        ////    collarsList.Add(collar_new);

        ////    //添加明细
        ////    List<int> AssetIDs = ConvertStringToIntList(Json_collar.assetList);
        ////    List<tb_Asset_collar_detail> details = convertToList_CollarDetail(serialNumber_collar, getAssetByID(AssetIDs), 1);
        ////    String Insert_Collar_SQL = get_Insert_collar(collarsList);
        ////    String Insert_CollarDetail_SQL = get_Insert_collar_detail(details);
        ////    SQLRunner sqlRunner = new SQLRunner();
        ////    try
        ////    {
        ////        insertNum = sqlRunner.run_Insert_SQL(Insert_Collar_SQL);
        ////        if (insertNum != -1)
        ////        {
        ////            insertNum = sqlRunner.run_Insert_SQL(Insert_CollarDetail_SQL);
        ////        }

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        Console.WriteLine(e.ToString());
        ////    }

        ////    return insertNum;
        ////}


        ////public JsonResult Load_Asset_Collor(int? page, int? rows, int role, int? state, int flag, String searchCondtiion, String selectedIDs)
        ////{
        ////    page = page == null ? 1 : page;
        ////    rows = rows == null ? 15 : rows;
        ////    state = state == null ? 16 : state;
        ////    List<int> checkIDS = ConvertStringToIntList(selectedIDs);
        ////    JavaScriptSerializer serializer = new JavaScriptSerializer();
        ////    dto_SC_Asset dto_condition = null;
        ////    if (searchCondtiion != null)
        ////    {
        ////        dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
        ////    }

        ////    return loadAsset_By_Type(page, rows, role, dto_condition, checkIDS, SystemConfig.tableType_detail);

        ////}




        //public JsonResult Load_SelectedAsset(int? page, int? rows, int role, int? state, int flag, String selectedIDs)
        //{
        //    page = page == null ? 1 : page;
        //    rows = rows == null ? 15 : rows;
        //    state = state == null ? 16 : state;

        //    List<int> checkIDS = ConvertStringToIntList(selectedIDs);
        //    String cond = getSelectAsset_ID_cond(checkIDS);

        //    String selectSQL = getSelectAssets(flag, cond, rows, page);
        //    String selectSQLCounter = getSelectAsset_Counter(flag, cond);


        //    SQLRunner sqlRuner = new SQLRunner();
        //    DataTable dt = sqlRuner.runSelectSQL_dto(selectSQL);
        //    int resultCount = sqlRuner.runSelectSQL_Counter(selectSQLCounter, "total");

        //    List<dto_Asset_Detail> list = new List<dto_Asset_Detail>();

        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        dto_Asset_Detail tmp = new dto_Asset_Detail();
        //        tmp.RowNo = int.Parse(dt.Rows[i]["RowNo"].ToString());
        //        tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
        //        tmp.serial_number = dt.Rows[i]["serial_number"].ToString();
        //        tmp.name_Asset = dt.Rows[i]["name_Asset"].ToString();
        //        tmp.type_Asset = dt.Rows[i]["type_Asset"].ToString();
        //        tmp.specification = dt.Rows[i]["specification"].ToString();
        //        tmp.people_using = dt.Rows[i]["specification"].ToString();
        //        tmp.department_Using = dt.Rows[i]["department_Using"].ToString();
        //        tmp.measurement = dt.Rows[i]["measurement"].ToString();
        //        tmp.unit_price = double.Parse(dt.Rows[i]["unit_price"].ToString());
        //        tmp.addressCF = dt.Rows[i]["addressCF"].ToString();
        //        tmp.amount = int.Parse(dt.Rows[i]["amount"].ToString());
        //        tmp.value = double.Parse(dt.Rows[i]["value"].ToString());
        //        tmp.state_asset = dt.Rows[i]["state_asset"].ToString();
        //        tmp.supplierID = dt.Rows[i]["supplierID"].ToString();
        //        tmp.Method_add = dt.Rows[i]["Method_add"].ToString();
        //        list.Add(tmp);
        //    }
        //    var json = new
        //    {
        //        total = resultCount,
        //        rows = list.ToArray()
        //        //sql = selectSQL

        //    };

        //    return Json(json, JsonRequestBehavior.AllowGet);

        //}


        ////public JsonResult LoadCollarsList(int? page, int? rows, dto_SC_Allocation cond)
        ////{
        ////    page = page == null ? 1 : page;
        ////    rows = rows == null ? 15 : rows;

        ////    var data = from p in DB_C.tb_Asset_collar
        ////               join tb_DP in DB_C.tb_department on p.department_collar equals tb_DP.ID_Department into temp_DP
        ////               from DB in temp_DP.DefaultIfEmpty()
        ////               join tb_ST in DB_C.tb_State_List on p.state_List equals tb_ST.id into temp_ST
        ////               from ST in temp_ST.DefaultIfEmpty()
        ////               join tb_AD in DB_C.tb_dataDict_para on p.addree_Storage equals tb_AD.ID into temp_AD
        ////               from AD in temp_AD.DefaultIfEmpty()
        ////               where p.flag == true
        ////               select p;

        ////    if (cond != null)
        ////    {

        ////    }



        ////    String SQL_Str = GetCollarSelectSQL(page, rows, role, flag, cond);
        ////    String SQL_Counter_Str = GetCollarSelectSQL_Counter(page, rows, role, flag, cond);
        ////    SQLRunner sqlRuner = new SQLRunner();
        ////    DataTable dt = sqlRuner.runSelectSQL_dto(SQL_Str);
        ////    int resultCount = sqlRuner.runSelectSQL_Counter(SQL_Counter_Str, "total");
        ////    List<dto_Collar> list = new List<dto_Collar>();
        ////    for (int i = 0; i < dt.Rows.Count; i++)
        ////    {
        ////        dto_Collar tmp = new dto_Collar();
        ////        tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
        ////        tmp.serialNumber = dt.Rows[i]["serialNumber"].ToString();
        ////        tmp.operatorUser = dt.Rows[i]["operatorUser"].ToString();
        ////        tmp.staff = dt.Rows[i]["staff"].ToString();
        ////        tmp.state = dt.Rows[i]["state"].ToString();
        ////        tmp.address = dt.Rows[i]["address"].ToString();
        ////        tmp.department = dt.Rows[i]["department"].ToString();
        ////        tmp.data_collar = (DateTime)dt.Rows[i]["data_collar"];
        ////        tmp.date_Operated = (DateTime)dt.Rows[i]["date_Operated"];
        ////        list.Add(tmp);
        ////    }
        ////    var json = new
        ////    {
        ////        total = resultCount,
        ////        rows = list.ToArray()

        ////    };
        ////    return Json(json, JsonRequestBehavior.AllowGet);

        ////}


        ////public List<dto_collar_detail> converTo_dto_Collar_detail(DataTable dt)
        ////{
        ////    List<dto_collar_detail> list = new List<dto_collar_detail>();
        ////    for (int i = 0; i < dt.Rows.Count; i++)
        ////    {
        ////        dto_collar_detail tmp = new dto_collar_detail();
        ////        tmp.serialNumber = dt.Rows[i]["serialNumber"].ToString();
        ////        tmp.operatorUser = dt.Rows[i]["operatorUser"].ToString();
        ////        tmp.staff = dt.Rows[i]["staff"].ToString();
        ////        tmp.state = dt.Rows[i]["state"].ToString();
        ////        tmp.address = dt.Rows[i]["address"].ToString();
        ////        tmp.department = dt.Rows[i]["department"].ToString();
        ////        tmp.reason = dt.Rows[i]["reason"].ToString();
        ////        tmp.ps = dt.Rows[i]["ps"].ToString();
        ////        tmp.data_collar = (DateTime)dt.Rows[i]["data_collar"];
        ////        tmp.date_Operated = (DateTime)dt.Rows[i]["date_Operated"];
        ////        list.Add(tmp);
        ////    }
        ////    return list;
        ////}

        ////public String ConverToInsertSelectSQLString_CollarDetail(tb_Asset_collar_detail data)
        ////{
        ////    String dataStr = "select '" + data.serial_number + "','" + data.serial_number_Asset + "','" + data.flag + "'";
        ////    return dataStr;
        ////}
        ////public String ConverToInsertSelectSQLString_Collar(tb_Asset_collar data)
        ////{
        ////    //(addree_Storage,date,date_Operated,department_collar,flag,operator,person,reason,serial_number,state_List)
        ////    int flag = data.flag == true ? 1 : 0;

        ////    String dataStr = "select '" + data.addree_Storage + "','" + data.date + "','" +
        ////                      data.date_Operated + "','" + data.department_collar + "','" +
        ////                      flag + "','" + data._operator + "','" +
        ////                        data.person + "','" + data.reason + "','" +
        ////                        data.serial_number + "'," + data.state_List;

        ////    return dataStr;
        ////}


        ////public tb_Asset_collar ConvertJsonTo_CollorTB(Json_Collar_addNew json_collar, String SerialNumber, Boolean flag, DateTime dateNow, int operatorID)
        ////{
        ////    tb_Asset_collar collar = new tb_Asset_collar();

        ////    collar.serial_number = SerialNumber == null ? "" : SerialNumber;
        ////    collar.state_List = json_collar.statelist;
        ////    collar.person = json_collar.people_LY;
        ////    collar.reason = json_collar.reason_LY;
        ////    collar.flag = flag;
        ////    collar.department_collar = json_collar.department_LY;
        ////    collar.date = json_collar.date_LY;
        ////    collar.date_Operated = dateNow;
        ////    collar.addree_Storage = json_collar.address_LY;
        ////    collar._operator = operatorID;
        ////    return collar;

        ////}

        ////public ActionResult DetailCollar(int id)
        ////{

        ////    ViewBag.selectID = id;
        ////    //获取领用单信息
        ////    dto_collar_detail collar_detail = loadCollarByID(id);
        ////    ViewBag.serialNumber = collar_detail.serialNumber;
        ////    ViewBag.address = collar_detail.address;
        ////    ViewBag.data_collar = collar_detail.data_collar;
        ////    ViewBag.department = collar_detail.department;
        ////    ViewBag.operatorUser = collar_detail.operatorUser;
        ////    ViewBag.staff = collar_detail.staff;
        ////    ViewBag.reason = collar_detail.reason;
        ////    ViewBag.ps = collar_detail.ps;
        ////    return View();
        ////}
        ////public String updateCollarStateByID(int state, String idStr)
        ////{
        ////    String result = "";
        ////    //要通过的审核的单据 须审核其明细中的资产是否全是闲置
        ////    if (state == 3)
        ////    {
        ////        if (Check_AssetStateByCollarID(state, idStr))
        ////        {
        ////            String sql = getSQL_Update_Collar_State(state, idStr);
        ////            SQLRunner sqlRunner = new SQLRunner();
        ////            result = sqlRunner.executesql(sql) + "";
        ////        }
        ////        else
        ////        {
        ////            result = "-1";
        ////        }

        ////    }
        ////    else
        ////    {
        ////        //将collar设置为相关的状态
        ////        String sql = getSQL_Update_Collar_State(state, idStr);
        ////        SQLRunner sqlRunner = new SQLRunner();
        ////        result = sqlRunner.executesql(sql) + "";
        ////    }


        ////    //DO more option  
        ////    //State=3  

        ////    return result;
        ////}


        /////**
        //// * state 是单据state
        //// *判断单据中明细资产是否全部可用 
        //// * */
        ////public Boolean Check_AssetStateByCollarID(int state, String idStr)
        ////{
        ////    List<int> idList = ConvertStringToIntList(idStr);

        ////    if (state == 3)
        ////    {
        ////        if (idList.Count != 1)
        ////        {
        ////            return false;
        ////        }
        ////        List<dto_collar_detail> collars = LoadCollars_By_ID_LIST(idList);
        ////        if (collars.Count != 1)
        ////        {
        ////            return false;
        ////        }
        ////        String SerialNumber_LY_cond = getCond_SerialNumber(collars);

        ////        String Sql_ = getSQL_Select_CollarDetail_BySerial_LY(SerialNumber_LY_cond);

        ////        SQLRunner runner = new SQLRunner();

        ////        DataTable dt = runner.runSelectSQL_dto(Sql_);

        ////        List<dto_Asset_CollarDetail> list = ConvertDataTabelTo_dto_CollarDetail(dt);

        ////        String cond = getCond_SerialNum_By_CollarDetail(list);
        ////        String SQL_COUNTER = GetSQL_Select_Count_By_SeialNumberCond(cond, 16);
        ////        int counter = runner.runSelectSQL_Counter(SQL_COUNTER, "total");
        ////        if (counter > 0)
        ////        {
        ////            return false;
        ////        }
        ////        //更新Asset状态 再用:15
        ////        int affect_count = update_Asset_state_By_SerialNumber(cond, 15, 16);
        ////        if (affect_count != list.Count)
        ////        {
        ////            update_Asset_state_By_SerialNumber(cond, 16, 15);
        ////        }
        ////        return true;

        ////    }
        ////    else
        ////    {
        ////        //暂时不做操作
        ////        return false;
        ////    }
        ////}
        ////public int update_Asset_State(int flag, String idStr, int state)
        ////{

        ////    String UpdateSql = get_Update_Asset_State(flag, idStr, state);
        ////    SQLRunner SqlRunner = new SQLRunner();
        ////    return SqlRunner.run_Update_SQL(UpdateSql);
        ////}

        ////public int update_Asset_state_By_SerialNumber(String cond_SerialNumber, int state, int oldState)
        ////{
        ////    String UpdateSql = getSQL_Update_Asset_state_By_SerialNumber(state, oldState, cond_SerialNumber);
        ////    SQLRunner SqlRunner = new SQLRunner();
        ////    return SqlRunner.run_Update_SQL(UpdateSql);
        ////}

        ////public List<dto_Asset_CollarDetail> ConvertDataTabelTo_dto_CollarDetail(DataTable dt)
        ////{
        ////    List<dto_Asset_CollarDetail> list = new List<dto_Asset_CollarDetail>();

        ////    for (int i = 0; i < dt.Rows.Count; i++)
        ////    {

        ////        dto_Asset_CollarDetail tmp = new dto_Asset_CollarDetail();
        ////        tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
        ////        tmp.serial_number = dt.Rows[i]["serial_number"].ToString();
        ////        tmp.serial_number_Asset = dt.Rows[i]["serial_number_Asset"].ToString();
        ////        tmp.flag = dt.Rows[i]["flag"].ToString();

        ////        list.Add(tmp);

        ////    }
        ////    return list;

        ////}



        ////public List<tb_Asset_collar_detail> convertToList_CollarDetail(String serialNumber, List<dto_Asset_Detail> assets, int flag)
        ////{
        ////    List<tb_Asset_collar_detail> list = new List<tb_Asset_collar_detail>();
        ////    if (assets != null)
        ////    {
        ////        for (int i = 0; i < assets.Count; i++)
        ////        {
        ////            tb_Asset_collar_detail detail = new tb_Asset_collar_detail();
        ////            detail.serial_number = serialNumber == null ? "" : serialNumber;
        ////            detail.serial_number_Asset = assets[i].serial_number == null ? "" : assets[i].serial_number;
        ////            detail.flag = flag == 1 ? true : false;
        ////            list.Add(detail);
        ////        }
        ////    }
        ////    return list;

        ////}
        //////===============================================================Convert  Area===================================================================================//

        ////public String getSQL_Update_Collar_State(int state, String idStr)
        ////{
        ////    List<int> idList = ConvertStringToIntList(idStr);
        ////    String cond = "(";
        ////    if (idList == null || idList.Count == 0)
        ////    {
        ////        cond += "0";
        ////    }
        ////    else
        ////    {
        ////        for (int i = 0; i < idList.Count; i++)
        ////        {
        ////            if (i == 0)
        ////            {
        ////                cond += idList[i];
        ////            }
        ////            else
        ////            {
        ////                cond += "," + idList[i];
        ////            }
        ////        }
        ////    }
        ////    cond += ")";



        ////    String sql = "update tb_Asset_collar set state_List=" + state + " where ID in " + cond;
        ////    return sql;
        ////}

        ////public String get_Insert_collar_detail(List<tb_Asset_collar_detail> details)
        ////{
        ////    String insertSql = "insert into tb_Asset_collar_detail (serial_number,serial_number_Asset,flag) ";

        ////    if (details == null || details.Count == 0)
        ////    {
        ////        return "";
        ////    }

        ////    for (int i = 0; i < details.Count; i++)
        ////    {

        ////        if (i == 0)
        ////        {
        ////            insertSql += ConverToInsertSelectSQLString_CollarDetail(details[i]);
        ////        }
        ////        else
        ////        {
        ////            insertSql += " union all " + ConverToInsertSelectSQLString_CollarDetail(details[i]);
        ////        }
        ////    }
        ////    return insertSql;
        ////}
        ////public String getDeleteCollarSQL(List<int> SelectedIDS)
        ////{
        ////    if (SelectedIDS != null && SelectedIDS.Count > 0)
        ////    {
        ////        String IDcond = getSelect_ID_cond_WithOut_A(SelectedIDS);

        ////        String SQL = "update tb_Asset_collar set flag=0 where flag=1 " + IDcond;
        ////        return SQL;
        ////    }
        ////    else
        ////    {
        ////        return "";
        ////    }
        ////}
        ////public String getCond_SerialNum_By_CollarDetail(List<dto_Asset_CollarDetail> list)
        ////{

        ////    if (list.Count > 0)
        ////    {
        ////        String cond = "";
        ////        for (int i = 0; i < list.Count; i++)
        ////        {
        ////            if (i == 0)
        ////            {
        ////                cond = "'" + list[i].serial_number_Asset + "'";
        ////            }
        ////            else
        ////            {
        ////                cond += ",'" + list[i].serial_number_Asset + "'";
        ////            }
        ////        }
        ////        return cond;

        ////    }
        ////    else
        ////    {
        ////        return "0";
        ////    }
        ////}

        ////public String getSQL_Select_CollarDetail_BySerial_LY(String serialNum_LY_cond)
        ////{
        ////    String sql = "select acd.*  from tb_Asset_collar_detail  acd left join tb_Asset_collar ac on acd.serial_number=ac.serial_number where ac.serial_number in (" + serialNum_LY_cond + ")";
        ////    return sql;
        ////}

        ////public String get_Insert_collar(List<tb_Asset_collar> collar)
        ////{
        ////    String insertSql = "insert into tb_Asset_collar (addree_Storage,date,date_Operated,department_collar,flag,operator,person,reason,serial_number,state_List) ";
        ////    DateTime date_op = DateTime.Now;

        ////    if (collar.Count > 0)
        ////    {
        ////        for (int i = 0; i < collar.Count; i++)
        ////        {
        ////            collar[i].date_Operated = date_op;
        ////            collar[i].flag = true;
        ////            if (i == 0)
        ////            {
        ////                insertSql += ConverToInsertSelectSQLString_Collar(collar[i]);
        ////            }
        ////            else
        ////            {
        ////                insertSql += " union all " + ConverToInsertSelectSQLString_Collar(collar[i]);
        ////            }



        ////        }
        ////    }
        ////    else
        ////    {
        ////        return "";
        ////    }


        ////    return insertSql;

        ////}




        ////public String get_Update_Asset_State(int flag, String IdsStr, int state)
        ////{
        ////    List<int> idList = ConvertStringToIntList(IdsStr);
        ////    String cond = getSelect_ID_cond_WithOut_A(idList);
        ////    String SQL = "update tb_Asset set state_asset=" + state + " where flag=" + flag + " " + cond;
        ////    return SQL;
        ////}


        ////public String getSQL_Update_Asset_state_By_SerialNumber(int state, int oldState, String cond_serialNumber)
        ////{
        ////    String SQL = "update tb_Asset set state_asset=" + state + " where state_asset=" + oldState + " and serial_number in (" + cond_serialNumber + ")";
        ////    return SQL;
        ////}



        


        ////public String getCond_SerialNumber(List<dto_collar_detail> list)
        ////{
        ////    String cond = "";

        ////    if (list.Count > 0)
        ////    {
        ////        for (int i = 0; i < list.Count; i++)
        ////        {
        ////            if (i == 0)
        ////            {
        ////                cond = "'" + list[i].serialNumber + "'";
        ////            }
        ////            else
        ////            {
        ////                cond += ",'" + list[i].serialNumber + "'";
        ////            }
        ////        }
        ////    }
        ////    else
        ////    {
        ////        cond = "0";


        ////    }
        ////    return cond;

        ////}
      
      


        ////public String getSQLCond_SerialNumber(List<tb_Asset_collar_detail> list)
        ////{
        ////    String cond = "";
        ////    if (list != null && list.Count > 0)
        ////    {
        ////        cond = " and a.serial_number in ( ";
        ////        for (int i = 0; i < list.Count; i++)
        ////        {
        ////            if (i == 0)
        ////            {
        ////                cond += "'" + list[i].serial_number_Asset + "'";
        ////            }
        ////            else
        ////            {
        ////                cond += ",'" + list[i].serial_number_Asset + "'";
        ////            }
        ////        }
        ////        cond += " )";
        ////    }
        ////    else
        ////    {
        ////        cond = "and a.serial_number in ('0')";
        ////    }
        ////    return cond;

        ////}

    }
}