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
    public class RepairController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController commonController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();
        // GET: Repair
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Repair()
        {
            return View();
        }

        public ActionResult Repair_add()
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_add))
            {
                return View("Error");
            }
            return View();
        }

        public ActionResult Repair_SelectingAsset()
        {
            return View();
        }

        public ActionResult Repair_edit(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_edit))
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
        public ActionResult Repair_detail(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_view))
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
        public ActionResult Repair_review(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_review))
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

        public ActionResult Repair_return(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_return))
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
        public JsonResult LoadRepair(int? page, int? rows, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_List dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_List>(searchCondtiion);
            }
            return loadRepairList(page, rows, dto_condition);
        }

        public JsonResult loadRepairList(int? page, int? rows, dto_SC_List cond)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            int? roleID = commonConversion.getRoleID();
            int? userID = commonConversion.getUSERID();
            //获取部门ID
            List<int?> idsRight_department = commonConversion.getids_departmentByRole(roleID);
            bool isAllUser = commonConversion.isSuperUser(roleID);

              var data_1= from p in DB_C.tb_ReviewReminding
                        where p.flag==true && p.Type_Review_TB==SystemConfig.TB_Repair
                        where p.ID_reviewer==userID
                        join tb_rep in DB_C.tb_Asset_Repair on p.ID_review_TB equals tb_rep.ID
                          join tb_ST in DB_C.tb_State_List on tb_rep.state_list equals tb_ST.id into temp_ST
                        from ST in temp_ST.DefaultIfEmpty()
                          join tb_SP in DB_C.tb_supplier on tb_rep.supplierID_Torepair equals tb_SP.ID into temp_SP
                        from SP in temp_SP.DefaultIfEmpty()
                          join tb_UAP in DB_C.tb_user on tb_rep.userID_applying equals tb_UAP.ID into temp_UAP
                        from UAP in temp_UAP.DefaultIfEmpty()
                          join tb_UAT in DB_C.tb_user on tb_rep.userID_authorize equals tb_UAT.ID into temp_UAT
                        from UAT in temp_UAT.DefaultIfEmpty()
                          join tb_UCT in DB_C.tb_user on tb_rep.userID_create equals tb_UCT.ID into temp_UCT
                        from UCT in temp_UCT.DefaultIfEmpty()
                        where ST.Name==SystemConfig.state_List_DSH
                          orderby tb_rep.date_create descending
                        select new Json_repair
                        {
                            cost_ToRepair = tb_rep.CostToRepair == null ? 0 : tb_rep.CostToRepair,
                            date_create = tb_rep.date_create,
                            date_ToRepair = tb_rep.date_ToRepair,
                            date_ToReturn = tb_rep.date_ToReturn,
                            ID = tb_rep.ID,
                            serialNumber = tb_rep.serialNumber,
                            state_list = ST.Name,
                            supplier_Name = SP.name_supplier,
                            userName_applying = UAP.true_Name,
                            userName_authorize = UAT.true_Name,
                            userName_create = UCT.true_Name,
                        };


            var data = from p in DB_C.tb_Asset_Repair
                       where p.flag == true
                       where p.userID_create != null
                       where p.userID_create == userID || isAllUser == true
                       join tb_ST in DB_C.tb_State_List on p.state_list equals tb_ST.id into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_SP in DB_C.tb_supplier on p.supplierID_Torepair equals tb_SP.ID into temp_SP
                       from SP in temp_SP.DefaultIfEmpty()
                       join tb_UAP in DB_C.tb_user on p.userID_applying equals tb_UAP.ID into  temp_UAP
                       from UAP in temp_UAP.DefaultIfEmpty()
                       join tb_UAT in DB_C.tb_user on p.userID_authorize equals tb_UAT.ID into  temp_UAT
                       from UAT in temp_UAT.DefaultIfEmpty()
                       join tb_UCT in DB_C.tb_user on p.userID_create equals tb_UCT.ID into  temp_UCT
                       from UCT in temp_UCT.DefaultIfEmpty() 
                       orderby p.date_create descending
                       select new Json_repair 
                       {
                           cost_ToRepair = p.CostToRepair==null?0:p.CostToRepair,
                           date_create=p.date_create,
                           date_ToRepair=p.date_ToRepair,
                           date_ToReturn=p.date_ToReturn,
                           ID=p.ID,
                           serialNumber=p.serialNumber,
                           state_list=ST.Name,
                           supplier_Name=SP.name_supplier,
                           userName_applying=UAP.true_Name,
                           userName_authorize=UAT.true_Name,
                           userName_create=UCT.true_Name,
                       };

            data = data.Union(data_1).OrderByDescending(a => a.date_create);

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
                           where ids_state.Contains(p.state_list)
                           select p;
                }

                //时间查询  先判断时间是否有效
                //TODO:时间格式化  begin+00:00:00    end+23:59:59
                if (cond.begin != null && cond.end != null)
                {
                    data = from p in data
                           where p.date_ToRepair >= cond.begin && p.date_ToRepair <= cond.end
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
        /// 添加维修单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int Handler_InsertRepairList(String data)
        {

            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_add))
            {
                return -6;
            }
            if (data == null)
            {
                return -5;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_repair_add data_json = serializer.Deserialize<Json_repair_add>(data);
            if (data_json == null)
            {
                return 0;
            }
            //TODO:获取系列编号
            String seriaNumber =commonController.getLatestOneSerialNumber(SystemConfig.serialType_WX);
            int? userID = commonConversion.getUSERID();
            
            int state_list_ID = commonConversion.getStateListID(data_json.state_list);

            tb_Asset_Repair newItem = JTM.ConverJsonToTable(data_json);
            //设置其他属性
            newItem.date_create = DateTime.Now;
            newItem.userID_create = userID;
            newItem.flag = true;
            newItem.state_list = state_list_ID;
            try
            {
                DB_C.tb_Asset_Repair.Add(newItem);
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

        }

        [HttpPost]
        public JsonResult getRepairByID(int? id)
        {

            var data = from p in DB_C.tb_Asset_Repair
                       where p.ID == id
                       where p.flag == true
                       join tb_SP in DB_C.tb_supplier on p.supplierID_Torepair equals tb_SP.ID into temp_SP
                       from SP in temp_SP.DefaultIfEmpty()
                       join tb_AS in DB_C.tb_Asset on p.ID_Asset equals tb_AS.ID into temp_AS
                       from AS in temp_AS.DefaultIfEmpty()
                       select new Json_repair_edit
                       {
                           Address_supplier=SP.address,
                           CostToRepair=p.CostToRepair==null?0:p.CostToRepair,
                           date_ToRepair=p.date_create,
                           date_ToReturn=p.date_ToReturn,
                           ID=p.ID,
                           ID_Asset=p.ID_Asset,
                           linkMan_supplier=SP.linkman,
                           Name_asset=AS.name_Asset,
                           Name_equipment=p.Name_equipment,
                           note_repair=p.note_repair,
                           reason_ToRepair=p.reason_ToRepair,
                           serial_asset=AS.serial_number,
                           serialNumber=p.serialNumber,
                           supplierID_Torepair=p.supplierID_Torepair,
                           supplierName_Torepair=SP.name_supplier,
                           userID_applying=p.userID_applying,
                           userID_authorize=p.userID_authorize
                       };
            Json_repair_edit result = data.First();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int updateRepairStateByID(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_State_Update Json_data = serializer.Deserialize<Json_State_Update>(data);
            if (Json_data != null)
            {
                if (!RightToUpdateState(Json_data.id_state))
                {
                    return -6;
                }

                 if(isOkToReview_Repair(Json_data.id_state,Json_data.id_Item))
                 {
                     if (!RightToSubmit_Repair(Json_data.id_state, Json_data.id_Item))
                     {
                        return -2;
                     }
                     int id_state_target = commonConversion.getStateListID(Json_data.id_state);
                     try {
                         //获取用户ID
                         int? userID = commonConversion.getUSERID();
                         var db_data = from p in DB_C.tb_Asset_Repair
                                       where p.flag == true
                                       where p.ID == Json_data.id_Item
                                       select p;
                         foreach (var item in db_data)
                         {
                             item.state_list = id_state_target;
                             item.date_create = DateTime.Now;
                         }
                         if (commonConversion.is_YSH(Json_data.id_state))
                         {

                             foreach (var item in db_data)
                             {
                                 item.userID_review = userID;
                                 item.date_review = DateTime.Now;
                                 item.content_Review = Json_data.review;
                             }

                             List<int> ids_asset = getAssetIdsByRepairID(Json_data.id_Item);
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
                                 item_as.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_fix);
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

        [HttpPost]
        public int Repair_returnByID(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_State_Update Json_data = serializer.Deserialize<Json_State_Update>(data);
            if (Json_data != null)
            {
                if (!rightToReturn(Json_data))
                {
                    return -2;
                }
              
                try
                {
                    var data_r = from p in DB_C.tb_Asset_Repair
                                 where p.flag == true && p.ID == Json_data.id_Item
                                 select p;
                    foreach (var item in data_r)
                    {
                        item.state_list = commonConversion.getStateListID(Json_data.id_state);
                        item.content_repairState = Json_data.review;
                    }
                    DB_C.SaveChanges();
                    //获取
                    var data_a = from p in DB_C.tb_Asset
                                 join tb_rep in DB_C.tb_Asset_Repair on p.ID equals tb_rep.ID_Asset
                                 where p.flag == true && tb_rep.ID == Json_data.id_Item && tb_rep.flag == true
                                 select p;

                    foreach (var item in data_a)
                    {
                        item.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_using);
                    }
                    DB_C.SaveChanges();

                }
                catch (Exception e)
                {
                    var data_r = from p in DB_C.tb_Asset_Repair
                                 where p.flag == true && p.ID == Json_data.id_Item
                                 select p;
                    foreach (var item in data_r)
                    {
                        item.state_list = commonConversion.getStateListID(SystemConfig.state_List_YGH_jsonID);
                        item.content_repairState = null;
                    }
                    DB_C.SaveChanges();
                       //还原状态
                }


            }
            return 0;
        }





        public bool rightToReturn(Json_State_Update data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_return))
            {
                return false;
            }
            if(data==null||data.id_Item==null)
            {
                return false;
            }
            if(data.id_state!=SystemConfig.state_List_YGH_jsonID)
            {
                return false;
            }
            //获取asset状态
            var data_DB = from p in DB_C.tb_Asset_Repair
                          where p.flag == true && p.ID == data.id_Item
                          join tb_asset in DB_C.tb_Asset on p.ID_Asset equals tb_asset.ID
                          join tb_ST in DB_C.tb_dataDict_para on tb_asset.state_asset equals tb_ST.ID
                          where tb_ST.name_para == SystemConfig.state_asset_fix
                          select p;

            return data_DB.Count() > 0 ? true : false;

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

            if (commonController.isRightToOperate(SystemConfig.Menu_ZCWX, operation))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 判断单据是否有权限去操作
        /// </summary>
        /// <param name="id_stateTarget"></param>
        /// <param name="id_Repair"></param>
        /// <returns></returns>
        public bool isOkToReview_Repair(int? id_stateTarget, int? id_Repair)
        {
            if (id_Repair == null || id_stateTarget == null || !SystemConfig.state_List.Contains((int)id_stateTarget))
            {
                return false;
            }
            //获取当前状态
            var data = from p in DB_C.tb_Asset_Repair
                       where p.flag == true
                       where p.ID == id_Repair
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






        [HttpPost]
        public int Handler_UpdateRepairList(String data)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_edit))
            {
                return -6;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_repair_add Json_data = serializer.Deserialize<Json_repair_add>(data);
            if (Json_data == null || Json_data.id == null)
            {
                return 0;
            }
            try {
                if (!RightToSubmit_Repair(Json_data.state_list, Json_data.id))
                {
                    return -2;
                }
                var db_data = from p in DB_C.tb_Asset_Repair
                              where p.ID == Json_data.id
                              select p;
                foreach(var item in db_data)
                {
                    item.CostToRepair = Json_data.Cost_Repair;
                    item.date_ToRepair = Json_data.dateToP;
                    item.date_ToReturn = Json_data.dateToR;
                    item.ID_Asset = Json_data.id_asset_repair;
                    item.Name_equipment = Json_data.name_Equipment;
                    item.note_repair = Json_data.note_add;
                    item.reason_ToRepair = Json_data.reason_add;
                    item.state_list = commonConversion.getStateListID(Json_data.state_list);
                    item.supplierID_Torepair = Json_data.supplier_repair;
                    item.userID_applying = Json_data.UAP_add;
                    item.userID_authorize = Json_data.UAT_add;
                    item.date_create = DateTime.Now;
                }
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return -3;
            }
        }



        /// <summary>
        /// 判断选中资产是否满足条件
        /// </summary>
        /// <param name="id_state_target"></param>
        /// <param name="id_repair"></param>
        /// <returns></returns>
        public bool RightToSubmit_Repair(int? id_state_target, int? id_repair)
        {
            if (id_repair == null || id_state_target==null)
            {
                return false;
            }
            String NameTarget = commonConversion.getTargetStateName(id_state_target);
            if (NameTarget == SystemConfig.state_List_YSH)
            {
                //获取AssetID
                List<int> ids_asset = getAssetIdsByRepairID(id_repair);

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

        public List<int> getAssetIdsByRepairID(int? id)
        {
            var data = from p in DB_C.tb_Asset_Repair
                       where p.ID == id
                       where p.flag == true || p.flag == null
                       select p;

            List<int> ids = new List<int>();
            foreach (var item in data)
            {
                ids.Add((int)item.ID_Asset);
            }
            return ids;
        }


        /// <summary>
        /// 判断当前用户是否拥有该单据的编辑权
        /// 默认只有创建者才拥有编辑权限 超级管理员也不行
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
            tb_Asset_Repair data = DB_C.tb_Asset_Repair.Where(a => a.ID == id).First();

            if (data != null)
            {

                if (data.userID_create == userID)
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