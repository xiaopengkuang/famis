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
    public class ReturnController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController commonController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();
        // GET: Return
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Return_SelectingAsset()
        {
            return View();
        }

        public ActionResult Return_add()
        {
            return View();
        }

        public ActionResult LoadReturnList(int? page, int? rows, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_List dto_condition = serializer.Deserialize<dto_SC_List>(searchCondtiion);
            return loadList_Return(page, rows, dto_condition);
        }

        public ActionResult loadList_Return(int? page, int? rows, dto_SC_List cond)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? roleID = commonConversion.getRoleID();
            int? userID = commonConversion.getUSERID();

            //获取部门ID
            List<int?> idsRight_department = commonConversion.getids_departmentByRole(roleID);
            bool isAllUser = commonConversion.isSuperUser(roleID);
            //获取该用户可以去审核的单据
            var data_1 = from p in DB_C.tb_ReviewReminding
                         where p.flag == true && p.Type_Review_TB == SystemConfig.TB_Borrow
                         where p.ID_reviewer == userID
                         join tb_rt in DB_C.tb_Asset_Return on p.ID_review_TB equals tb_rt.ID
                         join tb_ST in DB_C.tb_State_List on tb_rt.state_list equals tb_ST.id into temp_ST
                         from ST in temp_ST.DefaultIfEmpty()
                         join tb_UOP in DB_C.tb_user on tb_rt.userID_operated equals tb_UOP.ID into temp_UOP
                         from UOP in temp_UOP.DefaultIfEmpty()
                         where ST.Name == SystemConfig.state_List_DSH
                         orderby tb_rt.date_operated descending
                         select new Json_Return
                         {
                             date_return = tb_rt.date_return,
                             date_operated = tb_rt.date_operated,
                             ID = tb_rt.ID,
                             reason_return = tb_rt.reason_return,
                             note_return=tb_rt.note_return,
                             serialNum = tb_rt.serialNum,
                             state = ST.Name,
                             user_operated = UOP.true_Name
                         };


            var data= from p in DB_C.tb_Asset_Return
                      where p.flag==true
                      where p.userID_operated!=null
                      where p.userID_operated==userID || isAllUser==true 
                      join tb_ST in DB_C.tb_State_List on p.state_list equals tb_ST.id into temp_ST
                      from ST in temp_ST.DefaultIfEmpty()
                      join tb_UOP in DB_C.tb_user on p.userID_operated equals tb_UOP.ID into temp_UOP
                      from UOP in temp_UOP.DefaultIfEmpty()
                      orderby p.date_operated descending
                      select new Json_Return
                      {
                          date_return = p.date_return,
                          date_operated = p.date_operated,
                          ID = p.ID,
                          reason_return = p.reason_return,
                          note_return = p.note_return,
                          serialNum = p.serialNum,
                          state = ST.Name,
                          user_operated = UOP.true_Name
                      };
            data = data.Union(data_1);
            data = data.OrderByDescending(a => a.date_operated);
            if (cond != null)
            {
                //TODO:  条件查询  留给研一
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
        /// 
        /// 同时排除已经被选择的资产
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="searchCondtiion"></param>
        /// <param name="selectedIDs"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadAsset_ByState(int? page, int? rows, String searchCondtiion, String selectedIDs, String stateID)
        {
            List<int?> ids_Gone = commonConversion.StringToIntList(selectedIDs);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            JsonResult json = new JsonResult();
            int? role = commonConversion.getRoleID();
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);

            List<int?> ids_state = commonConversion.StringToIntList(stateID);

            List<String> stateName_asset = commonConversion.getAssetStateNameListByJsonID(ids_state);
            if (dto_condition == null)
            {
                return loadAssetByDataDict(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateName_asset);
            }
            else
            {
                switch (dto_condition.typeFlag)
                {
                    case SystemConfig.searchPart_letf: json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateName_asset); break;
                    case SystemConfig.searchPart_right: json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateName_asset); break;
                    default: ; break;
                }
            }
            return json;
        }

        public JsonResult loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int?> selectedIDs, List<String> stateName_asset)
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
                           where stateName_asset.Contains(ST.name_para)
                           select p;

            if (data_ORG == null)
            {
                return NULL_dataGrid();
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
                    return NULL_dataGrid();
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

                                //获取其所有子节点
                                List<int?> ids_dic = commonController.GetSonIDs_dataDict_Para(dic_paraID);
                                data_ORG = from p in data_ORG
                                           where ids_dic.Contains(p.addressCF)
                                           select p;
                            }; break;

                        case SystemConfig.nameFlag_2_SYBM:
                            {
                                //获取部门所有节点
                                List<int?> ids_dic = commonController.GetSonIDs_Department(dic_paraID);
                                data_ORG = from p in data_ORG
                                           where ids_dic.Contains(p.department_Using)
                                           select p;
                            }; break;

                        case SystemConfig.nameFlag_2_ZCLB:
                            {

                                List<int?> ids_dic = commonController.GetSonID_AsseType(dic_paraID);
                                data_ORG = from p in data_ORG
                                           where ids_dic.Contains(p.type_Asset)
                                           select p;
                            }; break;
                        case SystemConfig.nameFlag_2_SYRY:
                            {
                                data_ORG = from p in data_ORG
                                           where p.Owener == dic_paraID
                                           select p;
                            }; break;
                        case SystemConfig.nameFlag_2_ZCZT:
                            {
                                data_ORG = from p in data_ORG
                                           where p.state_asset == dic_paraID
                                           select p;
                            }; break;
                        default: ; break;
                    }
                }
            }
            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                       from DZ in temp_DZ.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_borD in DB_C.tb_Asset_Borrow_detail on p.ID equals tb_borD.ID_Asset
                       where tb_borD.flag==true
                       join tb_bor in DB_C.tb_Asset_Borrow on  tb_borD.ID_borrow equals tb_bor.ID
                       where tb_bor.flag==true
                       join tb_SL_Bo in DB_C.tb_State_List on tb_bor.state_list equals tb_SL_Bo.id into temp_SL_bo
                       from SL_bo in temp_SL_bo.DefaultIfEmpty()
                       where SL_bo.Name==SystemConfig.state_List_YSH
                       join tb_DP_JC in DB_C.tb_department on tb_borD.departmentID_loan equals tb_DP_JC.ID into temp_DP_JC
                       from DP_JC in temp_DP_JC.DefaultIfEmpty()
                       join tb_U_JC in DB_C.tb_user on tb_borD.userID_loan equals tb_U_JC.ID into temp_U_JC
                       from U_JC in temp_U_JC.DefaultIfEmpty()
                       join tb_DP_BO in DB_C.tb_department on tb_bor.department_borrow equals tb_DP_BO.ID into temp_DP_BO
                       from DP_BO in temp_DP_BO.DefaultIfEmpty()
                       join tb_U_BO in DB_C.tb_user on tb_bor.userID_borrow equals tb_U_BO.ID into temp_U_BO
                       from U_BO in temp_U_BO.DefaultIfEmpty()
                       orderby p.Time_add descending
                       select new Json_Asset_Return
                       {
                           addressCF = DZ.name_para,
                           amount = p.amount,
                           ID = p.ID,
                           measurement = MM.name_para,
                           name_Asset = p.name_Asset,
                           Time_Operated = p.Time_add,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price,
                           value = p.value,
                           department_borrow=DP_BO.name_Department,
                           department_loan=DP_JC.name_Department,
                           user_borrow=U_BO.true_Name,
                           user_loan=U_JC.true_Name,
                           serialNum_JC=tb_bor.serialNum
                       };
            data = data.OrderByDescending(a => a.Time_Operated);
            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int?> selectedIDs, List<String> stateName_asset)
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
                           where stateName_asset.Contains(ST.name_para)
                           select p;
            if (data_ORG == null)
            {
                return NULL_dataGrid();
            }
            if (cond != null)
            {
                switch (cond.DataType)
                {
                    case SystemConfig.searchCondition_Date:
                        {
                            //TODO:异常处理
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
                       orderby p.Time_add descending
                       select new dto_Asset_Detail
                       {
                           addressCF = DZ.name_para,
                           amount = p.amount,
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol,
                           depreciation_Month = p.depreciation_Month,
                           ID = p.ID,
                           //measurement = tb_MM.name_para,
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
            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NULL_dataGrid()
        {
            var json = new
            {
                total = 0,
                rows = ""
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}