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
    public class BorrowController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController commonController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();
        // GET: Loan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Borrow()
        {

            return View();
        }

        public ActionResult Borrow_add()
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJC, SystemConfig.operation_add))
            {
                ViewBag.info = "暂无权限添加！";
                return View("Error");
            }
            return View();
        }

        public ActionResult Borrow_edit(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJC, SystemConfig.operation_edit))
            {
                ViewBag.info = "暂无权限编辑！";
                return View("Error");
            }
            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }
        public ActionResult Borrow_detail(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJC, SystemConfig.operation_view))
            {
                ViewBag.info = "暂无权限查看！";
                return View("Error");
            }
            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }
        public ActionResult Borrow_review(int? id) 
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJC, SystemConfig.operation_review))
            {
                ViewBag.info = "暂无权限审核！";
                return View("Error");
            }

            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }

        public ActionResult Borrow_SelectingAsset() 
        {
            return View();
        }
        


        /// <summary>
        /// 获取借用单据数据列表入口
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="searchCondtiion"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        [HttpPost]
        public String LoadBorrowList(int? page, int? rows, String searchCondtiion, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_List dto_condition = serializer.Deserialize<dto_SC_List>(searchCondtiion);
            return loadList_Borrow(page, rows, dto_condition,exportFlag);
        }

        /// <summary>
        /// 获取借用单据方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="cond"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        public String loadList_Borrow(int? page, int? rows, dto_SC_List cond, bool? exportFlag)
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
                         where p.Type_Review_TB == SystemConfig.TB_Borrow
                         where p.ID_reviewer == userID
                         join tb_bor in DB_C.tb_Asset_Borrow on p.ID_review_TB equals tb_bor.ID
                         join tb_UB in DB_C.tb_user on tb_bor.userID_borrow equals tb_UB.ID into temp_UB
                         from UB in temp_UB.DefaultIfEmpty()
                         join tb_DP in DB_C.tb_department on  tb_bor.department_borrow equals tb_DP.ID into temp_DP
                         from DP in temp_DP.DefaultIfEmpty()
                         join tb_ST in DB_C.tb_State_List on tb_bor.state_list equals tb_ST.id into temp_ST
                         from ST in temp_ST.DefaultIfEmpty()
                         join tb_UOP in DB_C.tb_user on tb_bor.userID_operated equals tb_UOP.ID into temp_UOP
                         from UOP in temp_UOP.DefaultIfEmpty()
                         //where ST.Name == SystemConfig.state_List_DSH
                         orderby tb_bor.date_operated descending
                         select new Json_borrow
                         {
                             date_borrow=tb_bor.date_borrow,
                             date_return=tb_bor.date_return,
                             date_operated = tb_bor.date_operated,
                             department_borrow=DP.name_Department,
                             ID=tb_bor.ID,
                             reason_borrow=tb_bor.reason_borrow,
                             serialNumber=tb_bor.serialNum,
                             state=ST.Name,
                             user_borrow=UB.true_Name,
                             user_operated=UOP.true_Name
                         };

            var data = from p in DB_C.tb_Asset_Borrow
                       where p.flag == true
                       where p.userID_operated != null
                       where p.userID_operated == userID || isAllUser == true
                       where idsRight_department.Contains(p.department_borrow) || p.userID_operated == userID 
                       join tb_UB in DB_C.tb_user on p.userID_borrow equals tb_UB.ID into temp_UB
                       from UB in temp_UB.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_borrow equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_State_List on p.state_list equals tb_ST.id into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_UOP in DB_C.tb_user on p.userID_operated equals tb_UOP.ID into temp_UOP
                       from UOP in temp_UOP.DefaultIfEmpty()
                       orderby p.date_operated descending
                       select new Json_borrow
                       {
                           date_borrow = p.date_borrow,
                           date_return = p.date_return,
                           date_operated = p.date_operated,
                           department_borrow = DP.name_Department,
                           ID = p.ID,
                           reason_borrow = p.reason_borrow,
                           serialNumber = p.serialNum,
                           state = ST.Name,
                           user_borrow = UB.true_Name,
                           user_operated = UOP.true_Name
                       };


            data = data.Union(data_1);
            data = data.OrderByDescending(a => a.date_operated);
            if (cond != null)
            {
                //TODO:  条件查询 
                if (cond.serialNumber != null & cond.serialNumber != "")
                {
                    data = from p in data
                           where p.serialNumber.Contains(cond.serialNumber)
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
                           where p.date_borrow >= cond.begin && p.date_borrow <= cond.end
                           select p;
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (exportFlag != null && exportFlag == true)
            {
                String json_result = serializer.Serialize(data).ToString().Replace("\\", "");
                return json_result;
            }
            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.Count(),
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            String json_result_2 = serializer.Serialize(json).ToString().Replace("\\", "");
            return json_result_2;
            //return Json(json, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 根据选中ID 以及目标状态
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="idStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Load_SelectedAsset(int? page, int? rows, String selectedIDs,int? id_borrow)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            List<int?> ids_selected = commonConversion.StringToIntList(selectedIDs);
            return getAssetsByIDs(page, rows, ids_selected,id_borrow);
        }


        /// <summary>
        /// 根据ＩＤ获取资产
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="ids_selected"></param>
        /// <param name="id_borrow"></param>
        /// <returns></returns>
        public JsonResult getAssetsByIDs(int? page, int? rows, List<int?> ids_selected, int? id_borrow)
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
                       join tb_UU in DB_C.tb_user on p.Owener equals tb_UU.ID into temp_UU
                       from UU in temp_UU.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_BDT in DB_C.tb_Asset_Borrow_detail on p.ID equals tb_BDT.ID_Asset into temp_BDT
                       from BDT in temp_BDT.DefaultIfEmpty()
                       where BDT.ID_borrow == id_borrow ||id_borrow==null ||id_borrow==-1
                       join tb_UL in DB_C.tb_user on BDT.userID_loan equals tb_UL.ID into temp_UL
                       from UL in temp_UL.DefaultIfEmpty()
                       join tb_DL in DB_C.tb_department on BDT.departmentID_loan equals tb_DL.ID into temp_DL
                       from DL in temp_DL.DefaultIfEmpty()
                       select new Json_Asset_Borrow_detail
                       {
                           ID = p.ID,
                           department_loan = ST.name_para == SystemConfig.state_asset_loan ? DL.name_Department : DP.name_Department,
                           user_loan = ST.name_para == SystemConfig.state_asset_loan ? UL.true_Name : UU.true_Name,
                           serial_number = p.serial_number,
                           name_Asset = p.name_Asset,
                           type_Asset = AT.name_Asset_Type,
                           specification = p.specification,
                           measurement = MM.name_para,
                           amount = p.amount,
                           value = p.value,
                           state_asset = ST.name_para,
                           Time_Operated = p.Time_add
                       };
            data = data.Distinct();
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

        /// <summary>
        /// 自定义空数据
        /// </summary>
        /// <returns></returns>
        public JsonResult NULL_dataGrid()
        {
            var json = new
            {
                total = 0,
                rows = ""
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 添加新的借用单据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int Handler_Borrow_insert(String data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJC, SystemConfig.operation_add))
            {
                return -6;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Borrow_add json_data = serializer.Deserialize<Json_Borrow_add>(data);
            if (json_data == null)
            {
                return 0;
            }
            List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
            if (!commonController.checkAssetState_BySelectedAsset(selectedAssets, SystemConfig.state_asset_using))
            {
                return -5;
            }

            //TODO:获取系列编号
            String seriaNumber = commonController.getLatestOneSerialNumber(SystemConfig.serialType_JC );

            int? userID = commonConversion.getUSERID();
            int state_list_ID = commonConversion.getStateListID(json_data.state_List);

            tb_Asset_Borrow newItem = JTM.ConverJsonToTable(json_data);
            //设置其他属性
            newItem.serialNum = seriaNumber;
            newItem.userID_operated = userID;
            newItem.flag = true;
            newItem.state_list = state_list_ID;
            newItem.date_operated = DateTime.Now;
            try
            {
                DB_C.tb_Asset_Borrow.Add(newItem);
                DB_C.SaveChanges();
                int? id_borrow = getIDBySerialNum(newItem.serialNum);
                //获取单据明细
                //获取选中的Ids
                //List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
                List<tb_Asset_Borrow_detail> details = createBorrowDetailList(id_borrow, selectedAssets);
                DB_C.tb_Asset_Borrow_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                int? id_borrow = getIDBySerialNum(newItem.serialNum);
                if (id_borrow != null)
                {
                    try
                    {
                        tb_Asset_Borrow borrow_delete = DB_C.tb_Asset_Borrow.First(a => a.ID == id_borrow);
                        DB_C.tb_Asset_Borrow.Remove(borrow_delete);
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
        /// 更新借用单据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int Handler_Borrow_Update(String data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJC, SystemConfig.operation_edit))
            {
                return -6;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Borrow_add json_data = serializer.Deserialize<Json_Borrow_add>(data);
            if (json_data == null||json_data.ID==null)
            {
                return 0;
            }

            List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
            if (!commonController.checkAssetState_BySelectedAsset(selectedAssets, SystemConfig.state_asset_using))
            {
                return -5;
            }


            try {
                if (!RightToSubmit_borrow(json_data.state_List, json_data.ID))
                {
                    return -2;
                }
                var db_data = from p in DB_C.tb_Asset_Borrow
                              where p.ID == json_data.ID
                              select p;
                foreach (var item in db_data)
                {
                    item.date_borrow = json_data.date_borrow;
                    item.date_return = json_data.date_return;
                    item.department_borrow = json_data.department_borrow;
                    item.userID_borrow = json_data.user_borrow;
                    item.reason_borrow = json_data.reason_Borrow;
                    item.note_borrow = json_data.note_Borrow;
                    item.state_list = commonConversion.getStateListID(json_data.state_List);
                }

                var db_de = from p in DB_C.tb_Asset_Borrow_detail
                            where p.flag == true
                            where p.ID_borrow == json_data.ID
                            select p;
                foreach(var item in db_de)
                {
                    item.flag = false;
                    item.flag_return = true;
                }
                //获取选中IDs
                //List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
                List<tb_Asset_Borrow_detail> details = createBorrowDetailList(json_data.ID, selectedAssets);
                DB_C.tb_Asset_Borrow_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }

        }


        /// <summary>
        /// 判断详细是否符合标准提交
        /// </summary>
        /// <param name="id_state_Target"></param>
        /// <param name="id_borrow"></param>
        /// <returns></returns>
        public bool RightToSubmit_borrow(int? id_state_Target, int? id_borrow)
        {
            if (id_borrow == null || id_state_Target == null)
            {
                return false;
            }

            String NameTarget = commonConversion.getTargetStateName(id_state_Target);
            if (NameTarget == SystemConfig.state_List_YSH)
            {
                //获取AssetID
                List<int> ids_asset = getAssetIdsByBorrowID(id_borrow);
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
                                where tb_AS.name_para == SystemConfig.state_asset_using
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



        /// <summary>
        /// 创建明细单据
        /// </summary>
        /// <param name="id_borrow"></param>
        /// <param name="ids_asset"></param>
        /// <returns></returns>
        public List<tb_Asset_Borrow_detail> createBorrowDetailList(int? id_borrow, List<int?> ids_asset)
        {
            List<tb_Asset_Borrow_detail> list = new List<tb_Asset_Borrow_detail>();
            if (id_borrow == null || id_borrow < 1)
            {
                return list;
            }

            if (ids_asset.Count > 0)
            {

                //获取dataset
                var data_asset = from p in DB_C.tb_Asset
                                 where ids_asset.Contains(p.ID)
                                 select p;
                foreach (var item_asset in data_asset)
                {
                    tb_Asset_Borrow_detail item = new tb_Asset_Borrow_detail();
                    item.ID_Asset = item_asset.ID;
                    item.ID_borrow = id_borrow;
                    item.userID_loan = item_asset.Owener;
                    item.departmentID_loan = item_asset.department_Using;
                    item.flag = true;
                    item.flag_return = false;
                    list.Add(item);
                }
            }
            else
            {
                //return list;
            }
            return list;
        }



        /// <summary>
        /// 获取单据信息  拥有编辑 数据绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Handler_Borrow_Get(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_Borrow
                       where p.flag == true
                       where p.ID == id
                       join tb_DP in DB_C.tb_department on p.department_borrow equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_USA in DB_C.tb_user on p.userID_borrow equals tb_USA.ID into temp_USA
                       from USA in temp_USA.DefaultIfEmpty()
                       select new Json_Borrow_edit
                       {
                           date_borrow=p.date_borrow,
                           date_return=p.date_return,
                           department_borrow=p.department_borrow,
                           departmentName_borrow=DP.name_Department,
                           ID=p.ID,
                           note_Borrow=p.note_borrow,
                           reason_Borrow=p.note_borrow,
                           serialNum=p.serialNum,
                           user_borrow=p.userID_borrow,
                           userName_borrow=USA.true_Name
                       };
            if (data.Count() > 0)
            {
                Json_Borrow_edit result = data.First();
                result.assetList = getAssetIdsByBorrowID(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;

        }


        /// <summary>
        /// 根据单据ID获取相应的资产ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> getAssetIdsByBorrowID(int? id)
        {
            var data = from p in DB_C.tb_Asset_Borrow_detail
                       where p.ID_borrow == id
                       where p.flag == true
                       select p;

            List<int> ids = new List<int>();
            foreach (var item in data)
            {
                ids.Add((int)item.ID_Asset);
            }
            return ids;
        }

        /// <summary>
        /// 根据单据编号获取ID
        /// </summary>
        /// <param name="serialNum"></param>
        /// <returns></returns>
        public int? getIDBySerialNum(String serialNum)
        {
            if (serialNum == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_Borrow
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

        /// <summary>
        /// 更新单据状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int updateBorrowStateByID(String data)
        {


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_State_Update Json_data = serializer.Deserialize<Json_State_Update>(data);

            if (Json_data != null)
            {
                if (!RightToUpdateState(Json_data.id_state))
                {
                    return -6;
                }
                if (isOkToReview_borrow(Json_data.id_state, Json_data.id_Item))
                {
                    if (!RightToSubmit_borrow(Json_data.id_state, Json_data.id_Item))
                    {
                        return -5;
                    }
                    //获取数据库中的ID
                    int id_state_target = commonConversion.getStateListID(Json_data.id_state);
                    tb_Asset_Borrow bro = getBorrowTBbyID(Json_data.id_Item);
                    if (bro == null)
                    {
                        return -4;
                    }
                    try
                    {
                        //获取用户ID
                        int? userID = commonConversion.getUSERID();
                        var db_data = from p in DB_C.tb_Asset_Borrow
                                      where p.flag == true
                                      where p.ID == Json_data.id_Item
                                      select p;
                        foreach (var item in db_data)
                        {
                            item.state_list = id_state_target;
                            item.userID_review = userID;
                            item.date_operated = DateTime.Now;
                            item.content_review = Json_data.review;
                        }
                        if (commonConversion.is_YSH(Json_data.id_state))
                        {
                            List<int> ids_asset = getAssetIdsByBorrowID(Json_data.id_Item);
                            var dataAsset = from p in DB_C.tb_Asset
                                            where p.flag == true
                                            where ids_asset.Contains(p.ID)
                                            select p;
                            if (dataAsset != null && dataAsset.Count() > 0 && dataAsset.Count() != ids_asset.Count)
                            {
                                return -3;
                            }
                            foreach (var item_as in dataAsset)
                            {
                                item_as.department_Using = bro.department_borrow;
                                item_as.Owener = bro.userID_review;
                                item_as.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_loan);
                            }
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Borrow
                                           where p.ID_review_TB == bro.ID
                                           select p;

                            foreach (var item in data_rem)
                            {
                                item.flag = false;
                                item.time_review = DateTime.Now;
                            }


                        }
                        else if (commonConversion.is_TH(Json_data.id_state))
                        {
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Borrow
                                           where p.ID_review_TB == bro.ID
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
                            tb.Type_Review_TB = SystemConfig.TB_Borrow;
                            tb.ID_review_TB = bro.ID;
                            tb.ID_reviewer = Json_data.id_reviewer;
                            tb.time_add = DateTime.Now;
                            DB_C.tb_ReviewReminding.Add(tb);
                        }

                        DB_C.SaveChanges();

                        return 1;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return 0;
                    }
                }
                else {
                    return -1;
                }

            }
            return 0;
        }



        /// <summary>
        /// 判断是否有权限
        /// </summary>
        /// <param name="id_json"></param>
        /// <returns></returns>
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

            if (commonController.isRightToOperate(SystemConfig.Menu_ZCJC, operation))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取Allocation TB
        /// </summary>
        /// <returns></returns>
        public tb_Asset_Borrow getBorrowTBbyID(int? id_borrow)
        {
            List<tb_Asset_Borrow> data = DB_C.tb_Asset_Borrow.Where(a => a.ID == id_borrow).ToList(); ;
            if (data.Count > 0)
            {
                return data.First();
            }
            return null;

        }


        /// <summary>
        /// 权限判断  单据逻辑判断
        /// </summary>
        /// <param name="id_stateTarget"></param>
        /// <param name="id_borrow"></param>
        /// <returns></returns>
        public bool isOkToReview_borrow(int? id_stateTarget, int? id_borrow)
        {
            if (id_borrow == null || id_stateTarget == null || !SystemConfig.state_List.Contains((int)id_stateTarget))
            {
                return false;
            }
            //获取当前状态
            var data = from p in DB_C.tb_Asset_Borrow
                       where p.flag == true
                       where p.ID == id_borrow
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

        /// <summary>
        /// 判断当前用户是否拥有该单据的编辑权
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int RightToEdit(int? id)
        {
            //获取当前用户
            int? userID = commonConversion.getUSERID();
            int? roleID = commonConversion.getRoleID();
            bool sup = commonConversion.isSuperUser(roleID);

            if (sup)
            {
                return 1;
            }
            if (id == null)
            {
                return 0;
            }
            tb_Asset_Borrow data = DB_C.tb_Asset_Borrow.Where(a => a.ID == id).First();

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
    }
}