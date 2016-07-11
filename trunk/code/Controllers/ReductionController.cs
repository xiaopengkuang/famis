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
                         where p.flag == true && p.Type_Review_TB == SystemConfig.TB_Reduction
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
                         user_operate = UOP.true_Name,
                         user_apply = UAP.true_Name,
                         user_approve = UAV.true_Name,
                     };
            data = data.Union(data_1).OrderByDescending(a=>a.dateTime_add);
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
                List<tb_Asset_Reduction_detail> details = createAllocationDetailList(id_reduction, selectedAssets);
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
        public List<tb_Asset_Reduction_detail> createAllocationDetailList(int? id_reduction, List<int?> ids_asset)
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


    }
}