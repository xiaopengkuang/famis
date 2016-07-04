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
                          date_collar = p.date,
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

    }
}