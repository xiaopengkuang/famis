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
        public ActionResult Return_edit(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }


        public ActionResult Return_detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }
        public ActionResult Return_review(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
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
                         where  p.Type_Review_TB == SystemConfig.TB_Borrow
                         where p.ID_reviewer == userID
                         join tb_rt in DB_C.tb_Asset_Return on p.ID_review_TB equals tb_rt.ID
                         join tb_ST in DB_C.tb_State_List on tb_rt.state_list equals tb_ST.id into temp_ST
                         from ST in temp_ST.DefaultIfEmpty()
                         join tb_UOP in DB_C.tb_user on tb_rt.userID_operated equals tb_UOP.ID into temp_UOP
                         from UOP in temp_UOP.DefaultIfEmpty()
                         //where ST.Name == SystemConfig.state_List_DSH
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
                if (cond.serialNumber != null & cond.serialNumber != "")
                {
                    data = from p in data
                           where p.serialNum.Contains(cond.serialNumber)
                           select p;
                }


                if (cond.stateList != null && cond.stateList != "")
                {
                    //获取的ID
                    List<String> ids_state = commonConversion.getListStateBySearchState(cond.stateList);
                    data = from p in data
                           where ids_state.Contains(p.state)
                           select p;
                }

                //时间查询  先判断时间是否有效
                //TODO:时间格式化  begin+00:00:00    end+23:59:59
                if (cond.begin != null && cond.end != null)
                {
                    data = from p in data
                           where p.date_return >= cond.begin && p.date_return <= cond.end
                           select p;
                }
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
        public JsonResult LoadAsset_ByState(int? page, int? rows, String searchCondtiion, String selectedIDs, String stateID, String isall)
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
                return loadAssetByDataDict(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateName_asset,isall);
            }
            else
            {
                switch (dto_condition.typeFlag)
                {
                    case SystemConfig.searchPart_letf: json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateName_asset, isall); break;
                    case SystemConfig.searchPart_right: json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateName_asset, isall); break;
                    default: ; break;
                }
            }
            return json;
        }

        public JsonResult loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int?> selectedIDs, List<String> stateName_asset, String isall)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            bool allF = false;
            if (isall != null&&isall.ToLower()=="all")
            {
                allF = true;
            }

            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where p.department_Using == null || idsRight_deparment.Contains(p.department_Using) || allF==true
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
                       where tb_borD.flag==true && tb_borD.flag_return==false
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

        public JsonResult loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int?> selectedIDs, List<String> stateName_asset, String isall)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            bool allF = false;
            if (isall != null && isall.ToLower() == "all")
            {
                allF = true;
            }

            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where p.department_Using == null || idsRight_deparment.Contains(p.department_Using) ||allF==true
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
                       join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                       from DZ in temp_DZ.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_borD in DB_C.tb_Asset_Borrow_detail on p.ID equals tb_borD.ID_Asset
                       where tb_borD.flag == true && tb_borD.flag_return == false
                       join tb_bor in DB_C.tb_Asset_Borrow on tb_borD.ID_borrow equals tb_bor.ID
                       where tb_bor.flag == true
                       join tb_SL_Bo in DB_C.tb_State_List on tb_bor.state_list equals tb_SL_Bo.id into temp_SL_bo
                       from SL_bo in temp_SL_bo.DefaultIfEmpty()
                       where SL_bo.Name == SystemConfig.state_List_YSH
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
                           department_borrow = DP_BO.name_Department,
                           department_loan = DP_JC.name_Department,
                           user_borrow = U_BO.true_Name,
                           user_loan = U_JC.true_Name,
                           serialNum_JC = tb_bor.serialNum
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



        /// <summary>
        /// 根据选中ID 以及目标状态
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="idStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Load_SelectedAsset(int? page, int? rows, String selectedIDs, int? id_return)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            List<int?> ids_selected = commonConversion.StringToIntList(selectedIDs);
            return getAssetsByIDs(page, rows, ids_selected, id_return);
        }

        public JsonResult getAssetsByIDs(int? page, int? rows, List<int?> ids_selected, int? id_return)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where ids_selected.Contains(p.ID)
                           select p;
            if (data_ORG.Count() < 1)
            {
                return NULL_dataGrid();
            }
            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       //where ST.name_para==SystemConfig.state_asset_loan
                       join tb_borD in DB_C.tb_Asset_Borrow_detail on p.ID equals tb_borD.ID_Asset
                       where tb_borD.flag == true
                       join tb_bor in DB_C.tb_Asset_Borrow on tb_borD.ID_borrow equals tb_bor.ID
                       where tb_bor.flag == true
                       join tb_RTD in DB_C.tb_Asset_Return_detail on p.ID equals tb_RTD.ID_Asset into temp_RTD
                       from RTD in temp_RTD.DefaultIfEmpty()
                       where RTD.ID_Return==id_return || id_return==null
                       join tb_SL_Bo in DB_C.tb_State_List on tb_bor.state_list equals tb_SL_Bo.id into temp_SL_bo
                       from SL_bo in temp_SL_bo.DefaultIfEmpty()
                       where SL_bo.Name == SystemConfig.state_List_YSH
                       join tb_DP_JC in DB_C.tb_department on tb_borD.departmentID_loan equals tb_DP_JC.ID into temp_DP_JC
                       from DP_JC in temp_DP_JC.DefaultIfEmpty()
                       join tb_U_JC in DB_C.tb_user on tb_borD.userID_loan equals tb_U_JC.ID into temp_U_JC
                       from U_JC in temp_U_JC.DefaultIfEmpty()
                       join tb_DP_BO in DB_C.tb_department on tb_bor.department_borrow equals tb_DP_BO.ID into temp_DP_BO
                       from DP_BO in temp_DP_BO.DefaultIfEmpty()
                       join tb_U_BO in DB_C.tb_user on tb_bor.userID_borrow equals tb_U_BO.ID into temp_U_BO
                       from U_BO in temp_U_BO.DefaultIfEmpty()
                       select new Json_Asset_Selected_Return
                       {
                           amount = p.amount,
                           ID = p.ID,
                           measurement = MM.name_para,
                           name_Asset = p.name_Asset,
                           Time_Operated_JC = p.Time_add,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price,
                           value = p.value,
                           SerialNum_JC=tb_bor.serialNum,
                           department_loan=DP_JC.name_Department,
                           department_return=DP_BO.name_Department,
                           user_loan=U_JC.true_Name,
                           user_return=U_BO.true_Name
                       };
            data = data.Distinct();
            data = data.OrderByDescending(a => a.Time_Operated_JC);

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



        /// <summary>
        /// 添加归还单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int Handler_Return_insert(String data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCGH, SystemConfig.operation_add))
            {
                return -6;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Return_add json_data = serializer.Deserialize<Json_Return_add>(data);
            if (json_data == null)
            {
                return 0;
            }
            List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
            if (!commonController.checkAssetState_BySelectedAsset(selectedAssets, SystemConfig.state_asset_loan))
            {
                return -5;
            }

            //TODO:获取系列编号
            String seriaNumber = commonController.getLatestOneSerialNumber(SystemConfig.serialType_GH);

            int? userID = commonConversion.getUSERID();
            int state_list_ID = commonConversion.getStateListID(json_data.state_List);
            tb_Asset_Return newItem = JTM.ConverJsonToTable(json_data);
            //设置其他属性
            newItem.serialNum = seriaNumber;
            newItem.userID_operated = userID;
            newItem.flag = true;
            newItem.date_operated = DateTime.Now;
            newItem.state_list = state_list_ID;
            try {
                DB_C.tb_Asset_Return.Add(newItem);
                DB_C.SaveChanges();
                int? id_return = getIDBySerialNum(newItem.serialNum);
                List<tb_Asset_Return_detail> details = createReturnDetailList(id_return,selectedAssets);
                DB_C.tb_Asset_Return_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            
            }catch(Exception e){
                Console.WriteLine(e.Message);
                int? id_return = getIDBySerialNum(newItem.serialNum);
                if (id_return != null)
                {
                    try
                    {
                        tb_Asset_Return return_delete = DB_C.tb_Asset_Return.First(a => a.ID == id_return);
                        DB_C.tb_Asset_Return.Remove(return_delete);
                        DB_C.SaveChanges();
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine(e2.Message);
                        return -4;
                    }
                }
                return 0;
            
            }
        }


        /// <summary>
        /// 获取根据ID_return 获取Return详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Handler_Return_Get(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_Return
                       where p.flag == true
                       where p.ID == id
                       select new Json_Return_edit
                       {
                           ID = p.ID,
                           serialNum = p.serialNum,
                           date_Return=p.date_return,
                           note_return=p.note_return,
                           reason_return=p.reason_return
                       };
            if (data.Count() > 0)
            {
                Json_Return_edit result = data.First();
                result.ids_asset = getAssetIdsByReturnID(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;

        }


        [HttpPost]
        public int Handler_Return_Update(String data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCGH, SystemConfig.operation_edit))
            {
                return -6;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Return_add json_data = serializer.Deserialize<Json_Return_add>(data);
            if (json_data == null || json_data.ID == null)
            {
                return 0;
            }
            try
            {
                List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
                if (!commonController.checkAssetState_BySelectedAsset(selectedAssets,SystemConfig.state_asset_loan))
                {
                    return -5;
                }


                if (!RightToSubmit_return(json_data.state_List, json_data.ID))
                {
                    return -2;
                }
                var db_data = from p in DB_C.tb_Asset_Return
                              where p.ID == json_data.ID
                              select p;
                foreach (var item in db_data)
                {
                    item.date_return = json_data.date_return;
                    item.reason_return = json_data.note_Return;
                    item.note_return = json_data.note_Return;
                    item.state_list = commonConversion.getStateListID(json_data.state_List);
                    item.date_operated = DateTime.Now;
                    item.userID_operated = commonConversion.getUSERID();
                }

                var db_de = from p in DB_C.tb_Asset_Return_detail
                            where p.flag == true
                            where p.ID_Return == json_data.ID
                            select p;
                foreach (var item in db_de)
                {
                    item.flag = false;
                }
                //获取选中IDs
                List<tb_Asset_Return_detail> details = createReturnDetailList(json_data.ID, selectedAssets);
                DB_C.tb_Asset_Return_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }




        [HttpPost]
        public int updateReturnStateByID(String data)
        {
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_State_Update Json_data = serializer.Deserialize<Json_State_Update>(data);

            if (Json_data != null)
            {
                if (!RightToUpdateState(Json_data.id_state))
                {
                    return -6;
                }

                if (isOkToReview_return(Json_data.id_state, Json_data.id_Item))
                {
                    if (!RightToSubmit_return(Json_data.id_state, Json_data.id_Item))
                    {
                        return -4;
                    }
                    //获取数据库中信息
                    //获取数据库中的ID
                    int id_state_target = commonConversion.getStateListID(Json_data.id_state);
                    tb_Asset_Return ret = getReturnTBbyID(Json_data.id_Item);
                    if (ret == null)
                    {
                        return -3;
                    }
                    try {
                        int? userID = commonConversion.getUSERID();
                        var db_data = from p in DB_C.tb_Asset_Return
                                      where p.flag == true
                                      where p.ID == Json_data.id_Item
                                      select p;
                        foreach (var item in db_data)
                        {
                            item.state_list = id_state_target;
                            item.userID_review = userID;
                            item.date_review = DateTime.Now;
                            item.date_review = DateTime.Now;
                            item.content_review = Json_data.review;
                        }
                        if (commonConversion.is_YSH(Json_data.id_state))
                        {
                            List<int?> ids_asset = getAssetIdsByReturnID(Json_data.id_Item);


                            //找到那些已审核的单据
                            var  data_BROD=from p in DB_C.tb_Asset_Borrow_detail 
                                           where p.flag==true && p.flag_return==false
                                           join tb_bro in DB_C.tb_Asset_Borrow on p.ID_borrow equals tb_bro.ID
                                           join tb_ST in DB_C.tb_State_List on tb_bro.state_list equals tb_ST.id
                                           where tb_ST.Name==SystemConfig.state_List_YSH
                                           where ids_asset.Contains(p.ID_Asset)
                                           select p;

                            foreach(var item in data_BROD)
                            {
                                //获取相应的Asset   
                                //更新对应资产他们的使用者以及部门
                                var db_item_asset_temp = from p in DB_C.tb_Asset
                                                      where p.flag==true
                                                      join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID
                                                      where tb_ST.name_para==SystemConfig.state_asset_loan
                                                      where p.ID== item.ID_Asset
                                                      select p;

                                if (db_item_asset_temp.Count() < 1)
                                {
                                    return -7;
                                }

                                foreach(var item_asset in db_item_asset_temp)
                                {
                                     item_asset.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_using);
                                     item_asset.department_Using=item.departmentID_loan;
                                    item_asset.Owener=item.userID_loan;
                                }
                                item.flag_return=true;
                            }

                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Return
                                           where p.ID_review_TB == ret.ID
                                           select p;

                            foreach (var item in data_rem)
                            {
                                item.flag = false;
                                item.time_review = DateTime.Now;
                            }
                        } else if (commonConversion.is_TH(Json_data.id_state))
                        {
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Return
                                           where p.ID_review_TB == ret.ID
                                           select p;

                            foreach (var item in data_rem)
                            {
                                item.flag = false;
                                item.time_review = DateTime.Now;
                            }
                        }
                        else if (commonConversion.is_DSH(Json_data.id_state))
                        {

                            //往提醒表里面添加
                            tb_ReviewReminding tb = new tb_ReviewReminding();
                            tb.flag = true;
                            tb.Type_Review_TB = SystemConfig.TB_Return;
                            tb.ID_review_TB = ret.ID;
                            tb.ID_reviewer = Json_data.id_reviewer;
                            tb.time_add = DateTime.Now;
                            DB_C.tb_ReviewReminding.Add(tb);
                        }
                        DB_C.SaveChanges();
                        return 1;
                    }catch(Exception e){
                        Console.WriteLine(e.Message);
                        return -1;
                    }
                }
                else {
                    return -5;
                }

            }
            return 0;
 
        }

        /// <summary>
        /// 判断详细是否符合标准提交
        /// 不处于借出状态的无法提交
        /// </summary>
        /// <param name="id_state_Target"></param>
        /// <param name="id_return"></param>
        /// <returns></returns>
        public bool RightToSubmit_return(int? id_state_Target, int? id_return)
        {
            if (id_return == null || id_state_Target == null)
            {
                return false;
            }

            String NameTarget = commonConversion.getTargetStateName(id_state_Target);
            if (NameTarget == SystemConfig.state_List_YSH)
            {
                //获取AssetID
                List<int?> ids_asset = getAssetIdsByReturnID(id_return);
                //没有附加明细
                if (ids_asset.Count == 0)
                {
                    return false;
                }
                //检查里面是否还有不是在用状态状态的资产
                var checkData = from p in DB_C.tb_Asset
                                where p.flag == true
                                where ids_asset.Contains(p.ID)
                                join tb_AS in DB_C.tb_dataDict_para on p.state_asset equals tb_AS.ID
                                where tb_AS.name_para == SystemConfig.state_asset_loan
                                select p;
                if (checkData.Count() == ids_asset.Count)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool RightToUpdateState(int? id_json)
        {
            String operation = null;
            switch (id_json)
            {
                case SystemConfig.state_List_CG_jsonID: { operation = SystemConfig.operation_add; }; break;
                case SystemConfig.state_List_DSH_jsonID: { operation = SystemConfig.operation_edit; }; break;
                case SystemConfig.state_List_TH_jsonID: { operation = SystemConfig.operation_review; }; break;
                case SystemConfig.state_List_YSH_jsonID: { operation = SystemConfig.operation_review; }; break;
                default: { }; break;
            }

            if (commonController.isRightToOperate(SystemConfig.Menu_ZCGH, operation))
            {
                return true;
            }
            return false;
        }



        /// <summary>
        /// 判断状态转变逻辑是否合格
        /// </summary>
        /// <param name="id_stateTarget"></param>
        /// <param name="id_return"></param>
        /// <returns></returns>
        public bool isOkToReview_return(int? id_stateTarget, int? id_return)
        {
            if (id_return == null || id_stateTarget == null || !SystemConfig.state_List.Contains((int)id_stateTarget))
            {
                return false;
            }
            //获取当前状态
            var data = from p in DB_C.tb_Asset_Return
                       where p.flag == true
                       where p.ID == id_return
                       join tb_SL in DB_C.tb_State_List on p.state_list equals tb_SL.id
                       select new dto_state_List
                       {
                           id = tb_SL.id,
                           Name = tb_SL.Name,
                       };
            dto_state_List item = data.First();
            if (item != null)
            {
                String stateName = item.Name;
                String stateName_target = commonConversion.getTargetStateName(id_stateTarget);
                bool fs = false;
                switch (stateName_target)
                {
                    case SystemConfig.state_List_CG:
                        {
                            if (SystemConfig.state_List_CG_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_DSH:
                        {
                            if (SystemConfig.state_List_DSH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_YSH:
                        {
                            if (SystemConfig.state_List_YSH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_TH:
                        {
                            if (SystemConfig.state_List_TH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    default: { fs = false; }; break;
                }
                return fs;
            }
            return false;
        }


        public List<int?> getAssetIdsByReturnID(int? id)
        {
            var data = from p in DB_C.tb_Asset_Return_detail
                       where p.ID_Return == id
                       where p.flag == true
                       select p;

            List<int?> ids = new List<int?>();
            foreach (var item in data)
            {
                ids.Add((int)item.ID_Asset);
            }
            return ids;
        }


        public tb_Asset_Return getReturnTBbyID(int? id_return)
        {
            List<tb_Asset_Return> data = DB_C.tb_Asset_Return.Where(a => a.ID == id_return).ToList(); ;
            if (data.Count > 0)
            {
                return data.First();
            }
            return null;

        }







        /// <summary>
        /// 判断当前用户是否拥有该单据的编辑权
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int RightToEdit(int? id)
        {
            int? roleID = commonConversion.getRoleID();
            bool sup = commonConversion.isSuperUser(roleID);

            if (sup)
            {
                return 1;
            }
            //获取当前用户
            int? userID = commonConversion.getUSERID();
            if (id == null)
            {
                return 0;
            }
            tb_Asset_Return data = DB_C.tb_Asset_Return.Where(a => a.ID == id).First();

            if (data != null)
            {
                if (data.userID_operated == userID)
                {
                    //单据状态处于状态
                    var info = from p in DB_C.tb_State_List
                               where p.id == data.state_list
                               select p;
                    if (info.Count() == 1)
                    {
                        foreach (var item in info)
                        {
                            if (SystemConfig.state_List_CG_right.Contains(item.Name))
                            {
                                return 1;
                            }
                        }
                        return 0;
                    }


                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            return 0;

        }



        /// <summary>
        /// 根据serialNum获取ID
        /// </summary>
        /// <param name="serialNum"></param>
        /// <returns></returns>
        public int? getIDBySerialNum(String serialNum)
        {
            if (serialNum == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_Return
                       where p.serialNum == serialNum
                       select p;
            if (data.Count() != 1)
            {
                return null;
            }

            foreach (var item in data)
            {
                return item.ID;
            }

            return null;
        }



        public List<tb_Asset_Return_detail> createReturnDetailList(int? id_return, List<int?> ids_asset)
        {
            List<tb_Asset_Return_detail> list = new List<tb_Asset_Return_detail>();
            if (id_return == null || id_return < 1)
            {
                return list;
            }

            if (ids_asset.Count > 0)
            {
                foreach (int id_a in ids_asset)
                {
                    tb_Asset_Return_detail item = new tb_Asset_Return_detail();
                    item.ID_Asset = id_a;
                    item.ID_Return = id_return;
                    item.flag = true;
                    //借出单ID？要不要加进来
                    list.Add(item);
                }
                ////获取dataset
                //var data_asset = from p in DB_C.tb_Asset
                //                 where ids_asset.Contains(p.ID)
                //                 select p;
                //foreach (var item_asset in data_asset)
                //{
                //    tb_Asset_Return_detail item = new tb_Asset_Return_detail();
                //    item.ID_Asset = item_asset.ID;
                //    item.ID_Return = id_return;
                //    item.flag = true;
                //    //借出单ID？要不要加进来
                //    list.Add(item);
                //}
            }
            else
            {
                //return list;
            }
            return list;
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