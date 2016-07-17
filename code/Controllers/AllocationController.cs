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
    public class AllocationController : Controller
    {

        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController commonController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();
        // GET: Allocation
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Allocation()
        {
            return View();
        }
        public ActionResult Allocation_add()
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCDB, SystemConfig.operation_add))
            {
                return View("Error");
            }
            return View();
        }
        public ActionResult Allocation_SelectingAsset()
        {
            return View();
        }

        public ActionResult Allocation_edit(int? id)
        {

            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCDB, SystemConfig.operation_edit))
            {
                return View("Error");
            }
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;

            return View();
        }

        public ActionResult Allocation_detail(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCDB, SystemConfig.operation_view))
            {
                return View("Error");
            }

            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }



        public ActionResult Allocation_review(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCDB, SystemConfig.operation_review))
            {
                return View("Error");
            }
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }

        



        [HttpPost]
        public String LoadAllocation(int? page, int? rows, String searchCondtiion, bool? exportFlag)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_List dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_List>(searchCondtiion);
            }
            return loadAllocationList(page, rows, dto_condition, exportFlag);
        }


        public String loadAllocationList(int? page, int? rows, dto_SC_List cond, bool? exportFlag)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            int? roleID = commonConversion.getRoleID();
            int? userID = commonConversion.getUSERID();
            //获取部门ID
            List<int?> idsRight_department = commonConversion.getids_departmentByRole(roleID);
            bool isAllUser = commonConversion.isSuperUser(roleID);


            //获取该用户可以去审核的单据
            var data_1= from p in DB_C.tb_ReviewReminding
                        where p.flag==true && p.Type_Review_TB==SystemConfig.TB_Allocation
                        where p.ID_reviewer==userID
                        join tb_allocation in DB_C.tb_Asset_allocation on p.ID_review_TB equals tb_allocation.ID
                        join tb_DP in DB_C.tb_department on tb_allocation.department_allocation equals tb_DP.ID into temp_DP
                        from DP in temp_DP.DefaultIfEmpty()
                        join tb_ST in DB_C.tb_State_List on tb_allocation.state_List equals tb_ST.id into temp_ST
                        from ST in temp_ST.DefaultIfEmpty()
                        join tb_AD in DB_C.tb_dataDict_para on tb_allocation.addree_Storage equals tb_AD.ID into temp_AD
                        from AD in temp_AD.DefaultIfEmpty()
                        join tb_US in DB_C.tb_user on tb_allocation._operator equals tb_US.ID into temp_US
                        from US in temp_US.DefaultIfEmpty()
                        join tb_USC in DB_C.tb_user on tb_allocation.user_allocation equals tb_USC.ID into temp_USC
                        from USC in temp_USC.DefaultIfEmpty()
                        where ST.Name==SystemConfig.state_List_DSH
                        orderby tb_allocation.date_Operated descending
                        select new Json_allocation
                        {
                            ID = tb_allocation.ID,
                            address = AD.name_para,
                            date_Operated = tb_allocation.date_Operated,
                            date_allocation = tb_allocation.date,
                            department = DP.name_Department,
                            operatorUser = US.name_User,
                            user_allocation = USC.true_Name,
                            serialNumber = tb_allocation.serial_number,
                            state = ST.Name
                        };
            var data =from p in DB_C.tb_Asset_allocation
                       where p.flag == true
                       where p._operator!=null
                       where  p._operator==userID || isAllUser==true
                       where p.department_allocation == null || idsRight_department.Contains(p.department_allocation)
                       join tb_DP in DB_C.tb_department on p.department_allocation equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_State_List on p.state_List equals tb_ST.id into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_AD in DB_C.tb_dataDict_para on p.addree_Storage equals tb_AD.ID into temp_AD
                       from AD in temp_AD.DefaultIfEmpty()
                       join tb_US in DB_C.tb_user on p._operator equals tb_US.ID into temp_US
                       from US in temp_US.DefaultIfEmpty()
                      join tb_USC in DB_C.tb_user on p.user_allocation equals tb_USC.ID into temp_USC
                      from USC in temp_USC.DefaultIfEmpty()
                       orderby p.date_Operated descending
                       select new Json_allocation
                       {
                          ID = p.ID,
                          address = AD.name_para,
                          date_Operated = p.date_Operated,
                          date_allocation = p.date,
                          department = DP.name_Department,
                          operatorUser = US.name_User,
                          user_allocation=USC.true_Name,
                          serialNumber = p.serial_number,
                          state = ST.Name
                       };
            data = data.Union(data_1).OrderByDescending(a=>a.date_Operated);


            if (cond != null)
            {
                //TODO:  条件查询  留给研一
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
                           where p.date_allocation >= cond.begin && p.date_allocation <= cond.end
                           select p;
                }
            }



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
            //return Json(json, JsonRequestBehavior.AllowGet);
            String json_result_2 = serializer.Serialize(json).ToString().Replace("\\", "");
            return json_result_2;

        }
        [HttpPost]
        public int Handler_allocation_add(String data)
        {

            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCDB,SystemConfig.operation_add))
            {
                return -6;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_allocation_add json_data = serializer.Deserialize<Json_allocation_add>(data);
            if (json_data == null)
            {
                return 0;
            }


            List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
            if (commonController.checkAssetState_BySelectedAsset(selectedAssets, SystemConfig.state_asset_using))
            {
                return -5;
            }
            //TODO:获取系列编号
            String seriaNumber = commonController.getLatestOneSerialNumber(SystemConfig.serialType_DB);

            int? userID = commonConversion.getUSERID();
            int state_list_ID = commonConversion.getStateListID(json_data.statelist);

            tb_Asset_allocation newItem = JTM.ConverJsonToTable(json_data);
            //设置其他属性
            newItem.serial_number = seriaNumber;
            newItem._operator = userID;
            newItem.flag = true;
            newItem.state_List = state_list_ID;
            newItem.date_Operated = DateTime.Now;
            try {
                DB_C.tb_Asset_allocation.Add(newItem);
                DB_C.SaveChanges();
                int? id_allocation = getIDBySerialNum(newItem.serial_number);
                //获取单据明细
                //获取选中的Ids
                //List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
                List<tb_Asset_allocation_detail> details = createAllocationDetailList(id_allocation, selectedAssets);
                DB_C.tb_Asset_allocation_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                Console.WriteLine(e.Message);
                int? id_allocation = getIDBySerialNum(newItem.serial_number);
                if (id_allocation != null)
                {
                    try
                    {
                        tb_Asset_allocation allocation_delete = DB_C.tb_Asset_allocation.First(a => a.ID == id_allocation);
                        DB_C.tb_Asset_allocation.Remove(allocation_delete);
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

        public List<tb_Asset_allocation_detail> createAllocationDetailList(int? id_collar, List<int?> ids_asset)
        {
            List<tb_Asset_allocation_detail> list = new List<tb_Asset_allocation_detail>();
            if (id_collar == null || id_collar < 1)
            {
                return list;
            }

            if (ids_asset.Count > 0)
            {
                foreach (int id in ids_asset)
                {
                    tb_Asset_allocation_detail item = new tb_Asset_allocation_detail();
                    item.ID_asset = id;
                    item.ID_allocation = id_collar;
                    item.flag = true;
                    list.Add(item);
                }
            }
            else
            {
                //return list;
            }
            return list;
        }

        [HttpPost]
        public JsonResult Allocation_getWithID(int? id)
        {
            var data = from p in DB_C.tb_Asset_allocation
                       where p.flag == true
                       where p.ID == id
                       join tb_AD in DB_C.tb_dataDict_para on p.addree_Storage equals tb_AD.ID into temp_AD
                       from AD in temp_AD.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_allocation equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_USA in DB_C.tb_user on p.user_allocation equals tb_USA.ID into temp_USA
                       from USA in temp_USA.DefaultIfEmpty()
                       select new dto_allocation_edit
                       {
                           address = p.addree_Storage,
                           address_name=AD.name_para,
                           date = p.date,
                           department = p.department_allocation,
                           department_name=DP.name_Department,
                           ID = p.ID,
                           user_allocation=USA.ID,
                           ps = p.ps,
                           reason = p.reason,
                           serial_number = p.serial_number
                       };
            dto_allocation_edit result = data.First();
            List<int> ids_select = getAssetIdsByAllocationID(id);
            result.idsList = ids_select;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public int Handler_allocation_update(String data)
        {

            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCDB, SystemConfig.operation_edit))
            {
                return -6;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_allocation_add Json_data = serializer.Deserialize<Json_allocation_add>(data);
            List<int?> selectedAssets = commonConversion.StringToIntList(Json_data.assetList);
            if (commonController.checkAssetState_BySelectedAsset(selectedAssets,SystemConfig.state_asset_using))
            {
                return -5;
            }


            if (Json_data == null || Json_data.id == null)
            {
                return 0;
            }
            try {
                if (!RightToSubmit_allocation(Json_data.statelist, Json_data.id))
                {
                    return -2;
                }
                var db_data = from p in DB_C.tb_Asset_allocation
                              where p.ID == Json_data.id
                              select p;
                foreach (var item in db_data)
                {
                    item.addree_Storage = Json_data.address;
                    item.date = Json_data.date_allocation;
                    item.date_Operated = DateTime.Now;
                    item.department_allocation = Json_data.department;
                    item.ps = Json_data.ps;
                    item.user_allocation = Json_data.user_allocation;
                    item.reason = Json_data.reason;
                    item.state_List = commonConversion.getStateListID(Json_data.statelist);
                }


                var db_de = from p in DB_C.tb_Asset_allocation_detail
                            where p.flag == true
                            where p.ID_allocation == Json_data.id
                            select p;
                foreach (var item in db_de)
                {
                    item.flag = false;
                }
                //获取选中IDs
                //List<int?> selectedAssets = commonConversion.StringToIntList(Json_data.assetList);
                List<tb_Asset_allocation_detail> details = createAllocationDetailList(Json_data.id, selectedAssets);
                DB_C.tb_Asset_allocation_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                Console.WriteLine(e.Message);
                return 0;
            }

        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int updateAllocationrStateByID(String data)
        {


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_State_Update Json_data = serializer.Deserialize<Json_State_Update>(data);
           
            if (Json_data != null)
            {
                if (!RightToUpdateState(Json_data.id_state))
                {
                    return -6;
                }

                //判断是否有权限
                if (isOkToReview_allocation(Json_data.id_state, Json_data.id_Item))
                {
                    if (!RightToSubmit_allocation(Json_data.id_state, Json_data.id_Item))
                    {
                        return -2;
                    }
                    //获取数据库中的ID
                    int id_state_target = commonConversion.getStateListID(Json_data.id_state);
                    tb_Asset_allocation co = getAllocationTBbyID(Json_data.id_Item);
                    if (co == null)
                    {
                        return -1;
                    }
                    try
                    {

                        //获取用户ID
                        int? userID = commonConversion.getUSERID();
                        var db_data = from p in DB_C.tb_Asset_allocation
                                      where p.flag == true
                                      where p.ID == Json_data.id_Item
                                      select p;



                        foreach (var item in db_data)
                        {
                            item.state_List = id_state_target;
                            item.userID_reView = userID;
                            item.date_Operated = DateTime.Now;
                            item.info_review = Json_data.review;
                        }
                        if (commonConversion.is_YSH(Json_data.id_state))
                        {
                            List<int> ids_asset = getAssetIdsByAllocationID(Json_data.id_Item);
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
                                item_as.addressCF = co.addree_Storage;
                                item_as.department_Using = co.department_allocation;
                                //可有可无
                                //item_as.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_using);
                            }
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Allocation
                                           where p.ID_review_TB == co.ID
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
                                           where p.Type_Review_TB == SystemConfig.TB_Allocation
                                           where p.ID_review_TB == co.ID
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
                            tb.Type_Review_TB = SystemConfig.TB_Allocation;
                            tb.ID_review_TB = co.ID;
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
                else
                {
                    return -1;
                }
            }
            return 0;

        }




        public bool RightToUpdateState(int? id_json)
        {
            String operation = null;
            switch (id_json) {
                case SystemConfig.state_List_CG_jsonID: { operation = SystemConfig.operation_add; }; break;
                case SystemConfig.state_List_DSH_jsonID: { operation = SystemConfig.operation_edit; }; break;
                case SystemConfig.state_List_TH_jsonID: { operation = SystemConfig.operation_review; }; break;
                case SystemConfig.state_List_YSH_jsonID: { operation = SystemConfig.operation_review; }; break;
                default: { }; break;
            }

            if (commonController.isRightToOperate(SystemConfig.Menu_ZCDB, operation))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取Allocation TB
        /// </summary>
        /// <returns></returns>
        public tb_Asset_allocation getAllocationTBbyID(int? id_collar)
        {
            List<tb_Asset_allocation> data = DB_C.tb_Asset_allocation.Where(a => a.ID == id_collar).ToList(); ;
            if (data.Count > 0)
            {
                return data.First();
            }
            return null;

        }

        public List<int> getAssetIdsByAllocationID(int? id)
        {
            var data = from p in DB_C.tb_Asset_allocation_detail
                       where p.ID_allocation == id
                       where p.flag==true
                       select p;

            List<int> ids = new List<int>();
            foreach (var item in data)
            {
                ids.Add((int)item.ID_asset);
            }
            return ids;
        }


        /// <summary>
        /// 判断详细是否符合标准提交
        /// </summary>
        /// <param name="id_state_Target"></param>
        /// <param name="id_allocation"></param>
        /// <returns></returns>
        public bool RightToSubmit_allocation(int? id_state_Target, int? id_allocation)
        {
            if (id_allocation == null || id_state_Target == null)
            {
                return false;
            }

            String NameTarget = commonConversion.getTargetStateName(id_state_Target);
            if (NameTarget == SystemConfig.state_List_YSH)
            {
                //获取AssetID
                List<int> ids_asset = getAssetIdsByAllocationID(id_allocation);

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




        public int? getIDBySerialNum(String serialNum)
        {
            if (serialNum == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_allocation
                       where p.serial_number == serialNum
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


        public bool isOkToReview_allocation(int? id_stateTarget, int? id_allocation)
        {
            if (id_allocation == null || id_stateTarget == null || !SystemConfig.state_List.Contains((int)id_stateTarget))
            {
                return false;
            }
            //获取当前状态
            var data = from p in DB_C.tb_Asset_allocation
                       where p.flag == true
                       where p.ID == id_allocation
                       join tb_SL in DB_C.tb_State_List on p.state_List equals tb_SL.id
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
            tb_Asset_allocation data = DB_C.tb_Asset_allocation.Where(a => a.ID == id).First();

            if (data != null)
            {
                if (data._operator == userID)
                {
                    //单据状态处于状态
                    var info = from p in DB_C.tb_State_List
                               where p.id == data.state_List
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