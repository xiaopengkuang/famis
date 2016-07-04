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
            return View();
        }
        public ActionResult Allocation_SelectingAsset()
        {
            return View();
        }


        



        [HttpPost]
        public ActionResult LoadAllocation(int? page, int? rows, String searchCondtiion)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_List dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_List>(searchCondtiion);
            }
            return loadAllocationList(page, rows, dto_condition);
        }


        public JsonResult loadAllocationList(int? page, int? rows, dto_SC_List cond)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            int? roleID = commonConversion.getRoleID();
            int? userID = commonConversion.getUSERID();
            //获取部门ID
            List<int?> idsRight_department = commonConversion.getids_departmentByRole(roleID);
            bool isAllUser = commonConversion.isSuperUser(roleID);

            var data =from p in DB_C.tb_Asset_allocation
                       where p.flag == true
                       where p._operator!=null
                       where  p._operator==userID || isAllUser==true
                       where p.department_allocation == null || idsRight_department.Contains(p.department_allocation)
                       join tb_DP in DB_C.tb_department on p.department_allocation equals tb_DP.ID_Department into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_State_List on p.state_List equals tb_ST.id into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_AD in DB_C.tb_dataDict_para on p.addree_Storage equals tb_AD.ID into temp_AD
                       from AD in temp_AD.DefaultIfEmpty()
                       join tb_US in DB_C.tb_user on p._operator equals tb_US.ID into temp_US
                       from US in temp_US.DefaultIfEmpty()
                       orderby p.date_Operated descending
                       select new Json_allocation
                       {
                          ID = p.ID,
                          address = AD.name_para,
                          date_Operated = p.date_Operated,
                          date_allocation = p.date,
                          department = DP.name_Department,
                          operatorUser = US.name_User,
                          serialNumber = p.serial_number,
                          state = ST.Name
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
        [HttpPost]
        public int Handler_allocation_add(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_allocation_add json_data = serializer.Deserialize<Json_allocation_add>(data);
            if (json_data == null)
            {
                return 0;
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
                List<int> selectedAssets = commonConversion.StringToIntList(json_data.assetList);
                List<tb_Asset_allocation_detail> details = createAllocationDetailList(id_allocation, selectedAssets);
                DB_C.tb_Asset_allocation_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
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
                        return -4;
                    }
                }
                return 0;
            }


        }

        public List<tb_Asset_allocation_detail> createAllocationDetailList(int? id_collar, List<int> ids_asset)
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

    }
}