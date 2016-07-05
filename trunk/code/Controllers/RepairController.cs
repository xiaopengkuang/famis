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
            return View();
        }

        public ActionResult Repair_SelectingAsset()
        {
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
        /// 添加维修单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int Handler_InsertRepairList(String data)
        {
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
            String seriaNumber = commonConversion.getUnqiIDString();
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
                return -1;
            }

        }


    }
}