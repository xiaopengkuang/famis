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
    public class ReductionController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController commonController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();
        // GET: Reduction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Reduction()
        {
            return View();
        }
        public ActionResult Reduction_add()
        {
            return View();
        }
        public ActionResult Reduction_edit(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }

        public ActionResult Reduction_detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }

        public ActionResult Reduction_review(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }

        public ActionResult Reduction_SelectingAsset()
        {
            return View();
        }


        public JsonResult LoadReduction(int? page,int? rows,String  searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_List dto_condition = serializer.Deserialize<dto_SC_List>(searchCondtiion);
            return loadList_Reduction(page, rows, dto_condition);
        }


        public JsonResult loadList_Reduction(int? page, int? rows, dto_SC_List cond)
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
                         where p.Type_Review_TB == SystemConfig.TB_Reduction
                         where p.ID_reviewer == userID
                         join tb_red in DB_C.tb_Asset_Reduction on p.ID_review_TB equals tb_red.ID
                         join tb_UAP in DB_C.tb_user on tb_red.userID_apply equals tb_UAP.ID into temp_UAP
                         from UAP in temp_UAP.DefaultIfEmpty()
                         join tb_UAV in DB_C.tb_user on tb_red.userID_approver equals tb_UAV.ID into temp_UAV
                         from UAV in temp_UAV.DefaultIfEmpty()
                         join tb_Dic in DB_C.tb_dataDict_para on  tb_red.method_reduction equals tb_Dic.ID into temp_dic
                         from dic in temp_dic.DefaultIfEmpty()
                         join tb_ST in DB_C.tb_State_List on tb_red.state_List equals tb_ST.id into temp_ST
                         from ST in temp_ST.DefaultIfEmpty()
                         join tb_UOP in DB_C.tb_user on tb_red.userID_operate equals tb_UOP.ID into temp_UOP
                         from UOP in temp_UOP.DefaultIfEmpty()
                         //where ST.Name==SystemConfig.state_List_DSH
                         orderby tb_red.date_Operated descending
                         select new Json_reduction
                         {
                             date_reduction=tb_red.date_reduction,
                             dateTime_add=tb_red.date_Operated,
                             ID=tb_red.ID,
                             method_reduction=dic.name_para,
                             serialNumber=tb_red.Serial_number,
                             state_list=ST.Name,
                             user_apply=UAP.true_Name,
                             user_operate=UOP.true_Name,
                             user_approve=UAV.true_Name
                         };

            var data=from p in DB_C.tb_Asset_Reduction
                     where p.flag == true
                     where p.userID_operate!=null
                     where  p.userID_operate==userID || isAllUser==true
                     join tb_UAP in DB_C.tb_user on p.userID_apply equals tb_UAP.ID into temp_UAP
                     from UAP in temp_UAP.DefaultIfEmpty()
                     join tb_UAV in DB_C.tb_user on p.userID_approver equals tb_UAV.ID into temp_UAV
                     from UAV in temp_UAV.DefaultIfEmpty()
                     join tb_Dic in DB_C.tb_dataDict_para on  p.method_reduction equals tb_Dic.ID into temp_dic
                     from dic in temp_dic.DefaultIfEmpty()
                     join tb_ST in DB_C.tb_State_List on p.state_List equals tb_ST.id into temp_ST
                     from ST in temp_ST.DefaultIfEmpty()
                     join tb_UOP in DB_C.tb_user on p.userID_operate equals tb_UOP.ID into temp_UOP
                     from UOP in temp_UOP.DefaultIfEmpty()
                     orderby p.date_Operated descending
                     select new Json_reduction
                     {
                         date_reduction = p.date_reduction,
                         dateTime_add = p.date_Operated,
                         ID = p.ID,
                         method_reduction = dic.name_para,
                         serialNumber = p.Serial_number,
                         state_list = ST.Name,
                         user_apply = UAP.true_Name,
                         user_operate = UOP.true_Name,
                         user_approve = UAV.true_Name,
                     };
           

            data = data.Union(data_1);
            data = data.OrderByDescending(a => a.dateTime_add);
            if (cond != null)
            {
                //TODO:  条件查询  留给研一
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
                           where ids_state.Contains(p.state_list)
                           select p;
                }

                //时间查询  先判断时间是否有效
                //TODO:时间格式化  begin+00:00:00    end+23:59:59
                if (cond.begin != null && cond.end != null)
                {
                    data = from p in data
                           where p.date_reduction >= cond.begin && p.date_reduction <= cond.end
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



        [HttpPost]
        public int Handler_reduction_add(String data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJS, SystemConfig.operation_add))
            {
                return -6;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_reduction_add json_data = serializer.Deserialize<Json_reduction_add>(data);
            if (json_data == null)
            {
                return 0;
            }
            //TODO:获取系列编号
            String seriaNumber = commonController.getLatestOneSerialNumber(SystemConfig.serialType_JS);
            int? userID = commonConversion.getUSERID();
            int state_list_ID = commonConversion.getStateListID(json_data.statelist);
            tb_Asset_Reduction newItem = JTM.ConverJsonToTable(json_data);

            //设置其他属性
            newItem.Serial_number = seriaNumber;
            newItem.userID_operate = userID;
            newItem.flag = true;
            newItem.state_List = state_list_ID;
            newItem.date_Operated = DateTime.Now;
            try {
                DB_C.tb_Asset_Reduction.Add(newItem);
                DB_C.SaveChanges();
                int? id_reduction = getIDBySerialNum(newItem.Serial_number);
                //获取单据明细
                //获取选中的Ids
                List<int?> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
                List<tb_Asset_Reduction_detail> details = createReductionDetailList(id_reduction, selectedAssets);
                DB_C.tb_Asset_Reduction_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
                int? id_reduction = getIDBySerialNum(newItem.Serial_number);
                if (id_reduction != null)
                {
                    try
                    {
                        tb_Asset_Reduction reduction_delete = DB_C.tb_Asset_Reduction.First(a => a.ID == id_reduction);
                        DB_C.tb_Asset_Reduction.Remove(reduction_delete);
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


        [HttpPost]
        public int Handler_reduction_update(String data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCJS, SystemConfig.operation_edit))
            {
                return -6;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_reduction_add Json_data = serializer.Deserialize<Json_reduction_add>(data);
            if (Json_data == null || Json_data.ID == null)
            {
                return 0;
            }
            try {
                if (!RightToSubmit_reduction(Json_data.statelist, Json_data.ID))
                {
                    return -5;
                }
                int? userID=commonConversion.getUSERID();
                var db_data = from p in DB_C.tb_Asset_Reduction
                              where p.ID == Json_data.ID
                              select p;
                foreach(var item in db_data)
                {
                    item.date_Operated = DateTime.Now;
                    item.method_reduction = Json_data.method_reduction;
                    item.note_reduce = Json_data.note;
                    item.reason_reduce = Json_data.reason;
                    item.state_List = Json_data.statelist;
                    item.userID_apply = Json_data.user_apply;
                    item.userID_approver = Json_data.user_approve;
                    item.userID_operate = userID;
                }
                var data_delete = from p in DB_C.tb_Asset_Reduction_detail
                                  where p.flag == true
                                  where p.ID_reduction == Json_data.ID
                                  select p;
                foreach (var item in data_delete)
                {
                    item.flag = false;
                }
                List<int?> ids_asset_selected = commonConversion.StringToIntList(Json_data.assetList);
                List<tb_Asset_Reduction_detail> details = createReductionDetailList(Json_data.ID,ids_asset_selected);
                DB_C.tb_Asset_Reduction_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public int? getIDBySerialNum(String serialNum)
        {
            if (serialNum == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_Reduction
                       where p.Serial_number == serialNum
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
        public List<tb_Asset_Reduction_detail> createReductionDetailList(int? id_reduction, List<int?> ids_asset)
        {
            List<tb_Asset_Reduction_detail> list = new List<tb_Asset_Reduction_detail>();
            if (id_reduction == null || id_reduction < 1)
            {
                return list;
            }

            if (ids_asset.Count > 0)
            {
                foreach (int id in ids_asset)
                {
                    tb_Asset_Reduction_detail item = new tb_Asset_Reduction_detail();
                    item.ID_Asset = id;
                    item.ID_reduction = id_reduction;
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
            tb_Asset_Reduction data = DB_C.tb_Asset_Reduction.Where(a => a.ID == id).First();

            if (data != null)
            {
                if (data.userID_operate == userID)
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


        [HttpPost]
        public ActionResult loadReductionByID(int? id)
        {
            var data = from p in DB_C.tb_Asset_Reduction
                       where p.flag == true && p.ID == id
                       join tb_dic in DB_C.tb_dataDict_para on p.method_reduction equals tb_dic.ID into  temp_dic
                       from dic in temp_dic.DefaultIfEmpty()
                       join tb_UAP in DB_C.tb_user on p.userID_apply equals tb_UAP.ID into temp_UAP
                       from UAP in temp_UAP.DefaultIfEmpty()
                       join tb_UAT in DB_C.tb_user on p.userID_approver equals tb_UAT.ID into temp_UAT
                       from UAT in temp_UAT.DefaultIfEmpty()
                       select new Json_redution_edit
                       {
                            date_reduction=p.date_reduction,
                            ID=p.ID,
                            note_reduce=p.note_reduce,
                            reason_reduce=p.reason_reduce,
                            Serial_number=p.Serial_number,
                            method_reduction=p.method_reduction,
                            methodName_reduction=dic.name_para,
                            user_apply=UAP.true_Name,
                            user_approver=UAT.true_Name,
                            userID_apply=p.userID_apply,
                            userID_approver=p.userID_approver
                       };
            Json_redution_edit result = data.First();
            List<int?> ids_select = getAssetIdsByReductionID(id);
            result.ids_asset = ids_select;
            return Json(result, JsonRequestBehavior.AllowGet);
                        
        }


        public List<int?> getAssetIdsByReductionID(int? id)
        {
            var data = from p in DB_C.tb_Asset_Reduction_detail
                       where p.ID_reduction == id
                       where p.flag == true
                       select p;

            List<int?> ids = new List<int?>();
            foreach (var item in data)
            {
                ids.Add((int)item.ID_Asset);
            }
            return ids;
        }

        /// <summary>
        /// 判断详细是否符合标准提交
        /// 存在报废不需要报废
        /// </summary>
        /// <param name="id_state_Target"></param>
        /// <param name="id_allocation"></param>
        /// <returns></returns>
        public bool RightToSubmit_reduction(int? id_state_Target, int? id_reduction)
        {
            if (id_reduction == null || id_state_Target == null)
            {
                return false;
            }

            String NameTarget = commonConversion.getTargetStateName(id_state_Target);
            if (NameTarget == SystemConfig.state_List_YSH)
            {
                //获取AssetID
                List<int?> ids_asset = getAssetIdsByReductionID(id_reduction);

                //没有附加明细
                if (ids_asset.Count == 0)
                {
                    return false;
                }

                //检查里面是否还有是否存在报废资产
                var checkData = from p in DB_C.tb_Asset
                                where p.flag == true
                                where ids_asset.Contains(p.ID)
                                join tb_AS in DB_C.tb_dataDict_para on p.state_asset equals tb_AS.ID
                                where tb_AS.name_para != SystemConfig.state_asset_bad
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



        [HttpPost]
        public int updateReductionStateByID(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_State_Update Json_data = serializer.Deserialize<Json_State_Update>(data);
            if (Json_data != null)
            {
                if (!RightToUpdateState(Json_data.id_state))
                {
                    return -6;
                }
                ///状态前后保证
                if (isOkToReview_reduction(Json_data.id_state, Json_data.id_Item))
                {
                    if (!RightToSubmit_reduction(Json_data.id_state, Json_data.id_Item))
                    {
                        return -5;
                    }

                    int id_state_target = commonConversion.getStateListID(Json_data.id_state);
                    tb_Asset_Reduction reduc = getReductionTBbyID(Json_data.id_Item);
                    if (reduc == null || reduc.ID < -4)
                    {
                        return -4;
                    }
                    try {
                        //获取用户ID
                        int? userID = commonConversion.getUSERID();
                        var db_data = from p in DB_C.tb_Asset_Reduction
                                      where p.flag == true
                                      where p.ID == Json_data.id_Item
                                      select p;
                        foreach (var item in db_data)
                        {
                            item.state_List = id_state_target;
                            item.userID_reviewer = userID;
                            item.date_Operated = DateTime.Now;
                            item.conten_review = Json_data.review;
                        }

                        if (commonConversion.is_YSH(Json_data.id_state))
                        {

                            //修改
                            List<int?> ids_asset = getAssetIdsByReductionID(Json_data.id_Item);
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
                                //TODO:这里需要将Asset设置为False吗
                                //item_as.flag = false;
                                item_as.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_bad);
                            }
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Reduction
                                           where p.ID_review_TB == reduc.ID
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
                            tb.Type_Review_TB = SystemConfig.TB_Reduction;
                            tb.ID_review_TB = reduc.ID;
                            tb.ID_reviewer = Json_data.id_reviewer;
                            tb.time_add = DateTime.Now;
                            DB_C.tb_ReviewReminding.Add(tb);
                        }
                        else if (commonConversion.is_TH(Json_data.id_state))
                        {
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Reduction
                                           where p.ID_review_TB == reduc.ID
                                           select p;

                            foreach (var item in data_rem)
                            {
                                item.flag = false;
                                item.time_review = DateTime.Now;
                            }
                        }


                        DB_C.SaveChanges();
                        return 1;
                    }catch(Exception e){
                        Console.WriteLine(e.Message);
                        return 0;
                    }




                }
            } 
            return 0;
        }
        public tb_Asset_Reduction getReductionTBbyID(int? id_reduction)
        {
            List<tb_Asset_Reduction> data = DB_C.tb_Asset_Reduction.Where(a => a.ID == id_reduction).ToList(); ;
            if (data.Count > 0)
            {
                return data.First();
            }
            return null;

        }


        /// <summary>
        ///判断是否有权限去更新
        /// 
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

            if (commonController.isRightToOperate(SystemConfig.Menu_ZCJS, operation))
            {
                return true;
            }
            return false;
        }


        public bool isOkToReview_reduction(int? id_stateTarget, int? id_reduction)
        {
            if (id_reduction == null || id_stateTarget == null || !SystemConfig.state_List.Contains((int)id_stateTarget))
            {
                return false;
            }
            //获取当前状态
            var data = from p in DB_C.tb_Asset_Reduction
                       where p.flag == true
                       where p.ID == id_reduction
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


    }
}