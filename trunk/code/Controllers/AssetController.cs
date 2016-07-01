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
    public class AssetController : Controller
    {

        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();

        // GET: Asset


        //===============================================================View  Area===================================================================================//
        public ActionResult Accounting()
        {
            return View();
        }
        public ActionResult allocation()
        {
            return View();
        }
        public ActionResult Collar_SelectAsset()
        {
            return View();
        }

        public ActionResult AddAsset()
        {
            return View();
        }

        public ActionResult AddCollar()
        {
            return View();
        }
        public ActionResult collar()
        {
            return View();
        }
        public ActionResult reduction()
        {
            return View();
        }

        public ActionResult DetailCollar(int id) {

            ViewBag.selectID = id;
            //获取领用单信息
            dto_collar_detail collar_detail = loadCollarByID(id);
            ViewBag.serialNumber = collar_detail.serialNumber;
            ViewBag.address = collar_detail.address;
            ViewBag.data_collar = collar_detail.data_collar;
            ViewBag.department = collar_detail.department;
            ViewBag.operatorUser = collar_detail.operatorUser;
            ViewBag.staff = collar_detail.staff;
            ViewBag.reason = collar_detail.reason;
            ViewBag.ps = collar_detail.ps;
            return View();
        }



        //===============================================================View  Area===================================================================================//


        //===============================================================Action  Area===================================================================================//
        //public JsonResult LoadCollars(int? page, int? rows, int role, int flag, String searchCondtiion)
        //{
        //    page = page == null ? 1 : page;
        //    rows = rows == null ? 15 : rows;
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    dto_SC_Allocation dto_condition = null;
        //    String condSC = "";
        //    if (searchCondtiion != null)
        //    {
        //        dto_condition = serializer.Deserialize<dto_SC_Allocation>(searchCondtiion);
        //        condSC = SC_To_String(dto_condition);
        //    }
        //    else
        //    {
        //        condSC = "";
        //    }
        //    return LoadCollarsList(page, rows, role, flag, condSC);
        //}

        public String updateCollarStateByID(int state,String idStr)
        {
              String result="";
            //要通过的审核的单据 须审核其明细中的资产是否全是闲置
            if (state == 3)
            {
                if(Check_AssetStateByCollarID(state, idStr))
                {
                      String sql = getSQL_Update_Collar_State(state, idStr);
                      SQLRunner sqlRunner = new SQLRunner();
                      result=sqlRunner.executesql(sql)+"";
                }else{
                    result="-1";
                }

            }else{
                 //将collar设置为相关的状态
                  String sql = getSQL_Update_Collar_State(state, idStr);
                  SQLRunner sqlRunner = new SQLRunner();
                  result=sqlRunner.executesql(sql)+"";
            }
           

            //DO more option  
            //State=3  

            return result;
        }


        /**
         * state 是单据state
         *判断单据中明细资产是否全部可用 
         * */
        public Boolean Check_AssetStateByCollarID(int state, String idStr)
        {
            List<int> idList=ConvertStringToIntList(idStr);

            if (state == 3)
            {
               if(idList.Count!=1)
               {
                   return false;
               }
                List<dto_collar_detail> collars=LoadCollars_By_ID_LIST(idList);
                if(collars.Count!=1)
                {
                    return false;
                }
                String SerialNumber_LY_cond = getCond_SerialNumber(collars);

                String Sql_ = getSQL_Select_CollarDetail_BySerial_LY(SerialNumber_LY_cond);
                
                SQLRunner runner=new SQLRunner();
                
                DataTable dt=runner.runSelectSQL_dto(Sql_);
                
                List<dto_Asset_CollarDetail> list = ConvertDataTabelTo_dto_CollarDetail(dt);

                String cond = getCond_SerialNum_By_CollarDetail(list);
                String SQL_COUNTER = GetSQL_Select_Count_By_SeialNumberCond(cond,16);
                int counter = runner.runSelectSQL_Counter(SQL_COUNTER, "total");
                if (counter > 0)
                {  
                    return false;
                }
                //更新Asset状态 再用:15
               int affect_count=update_Asset_state_By_SerialNumber(cond,15,16);
               if (affect_count!=list.Count)
               { 
                   update_Asset_state_By_SerialNumber(cond,16,15);
               }
               return true;
                
            }
            else
            {
                //暂时不做操作
                return false;
            }
        }

     



      





        public JsonResult LoadCollars(int? page, int? rows, int role, int flag, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Allocation dto_condition = null;
            String condSC = "";
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Allocation>(searchCondtiion);
                condSC = Convert_SC_To_CondString(dto_condition);
            }
            else
            {
                condSC = "";
            }

            return LoadCollarsList(page, rows, role, flag, condSC);
        }

        public JsonResult Load_Asset_By_CollarID(int? page, int? rows,int flag,int ID)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            List<tb_Asset_collar> collar = DB_C.tb_Asset_collar.Where(a => a.ID == ID).ToList();
            if (collar.Count!= 1)
            {
                List<dto_Asset_Detail> list_null = new List<dto_Asset_Detail>();
                var json_NULL = new
                {
                    total = 0,
                    rows = list_null.ToArray()
                    //sql = selectSQL

                };

                return Json(json_NULL, JsonRequestBehavior.AllowGet);
            }
            String serNum = collar[0].serial_number;
            List<tb_Asset_collar_detail> acd = DB_C.tb_Asset_collar_detail.Where(b=>b.flag==true).Where(a => a.serial_number==serNum).ToList();

            String cond = getSQLCond_SerialNumber(acd);

            String selectSQL = getSelectAssets(flag, cond, rows, page);
            String selectSQLCounter = getSelectAsset_Counter(flag, cond);


            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(selectSQL);
            int resultCount = sqlRuner.runSelectSQL_Counter(selectSQLCounter, "total");
            List<dto_Asset_Detail> list = new List<dto_Asset_Detail>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dto_Asset_Detail tmp = new dto_Asset_Detail();

                tmp.RowNo = int.Parse(dt.Rows[i]["RowNo"].ToString());
                tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                tmp.serial_number = dt.Rows[i]["serial_number"].ToString();
                tmp.name_Asset = dt.Rows[i]["name_Asset"].ToString();
                tmp.type_Asset = dt.Rows[i]["type_Asset"].ToString();
                tmp.specification = dt.Rows[i]["specification"].ToString();
                tmp.people_using = dt.Rows[i]["specification"].ToString();
                tmp.department_Using = dt.Rows[i]["department_Using"].ToString();
                tmp.measurement = dt.Rows[i]["measurement"].ToString();
                tmp.unit_price = double.Parse(dt.Rows[i]["unit_price"].ToString());
                tmp.addressCF = dt.Rows[i]["addressCF"].ToString();
                tmp.amount = int.Parse(dt.Rows[i]["amount"].ToString());
                tmp.value = double.Parse(dt.Rows[i]["value"].ToString());
                tmp.state_asset = dt.Rows[i]["state_asset"].ToString();
                tmp.supplierID = dt.Rows[i]["supplierID"].ToString();
                tmp.Method_add = dt.Rows[i]["Method_add"].ToString();
                list.Add(tmp);
            }
            var json = new
            {
                total = resultCount,
                rows = list.ToArray()
                //sql = selectSQL

            };

            return Json(json, JsonRequestBehavior.AllowGet);

        }



       


        public List<dto_collar_detail> LoadCollars_By_ID_LIST(List<int> ids)
        {
            SQLRunner sqlRuner = new SQLRunner();
            String sql = GetCollarSelectSQL(ids);
            DataTable dt= sqlRuner.runSelectSQL_dto(sql);
            return converTo_dto_Collar_detail(dt);
             
        }

        public dto_collar_detail loadCollarByID(int id)
        {
            List<int> ids = new List<int>();
            ids.Add(id);
            List<dto_collar_detail> list = new List<dto_collar_detail>();
            list = LoadCollars_By_ID_LIST(ids);
            if (list.Count == 1)
            {
                return list[0];
            }
            else {
                return new dto_collar_detail();
            }

        }


        [HttpPost]
        public int deleteCollars(List<int> selectedIDs)
        {
            String deleteSQL = getDeleteCollarSQL(selectedIDs); 
            SQLRunner sqlRunner = new SQLRunner();
            int result = sqlRunner.executesql(deleteSQL);
            return result;
        }

        public int InsertNewCollor(String collar_add)
        {
            int insertNum = 0;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Collar_addNew Json_collar = serializer.Deserialize<Json_Collar_addNew>(collar_add);

            CommonController cct = new CommonController();

            String serialNumber_collar = cct.getLatestOneSerialNumber("LY");
            DateTime dateNow = new DateTime();

            int operatorID = 1;
            int flag = Json_collar.flag == null ? 1 : (int)Json_collar.flag;

            tb_Asset_collar collar_new = ConvertJsonTo_CollorTB(Json_collar, serialNumber_collar, true, dateNow, operatorID);
            List<tb_Asset_collar> collarsList = new List<tb_Asset_collar>();
            collarsList.Add(collar_new);

            //添加明细
            List<int> AssetIDs = ConvertStringToIntList(Json_collar.assetList);
            List<tb_Asset_collar_detail> details = convertToList_CollarDetail(serialNumber_collar, getAssetByID(AssetIDs),1);
            String Insert_Collar_SQL = get_Insert_collar(collarsList);
            String Insert_CollarDetail_SQL = get_Insert_collar_detail(details);
            SQLRunner sqlRunner = new SQLRunner();
            try
            {
                insertNum = sqlRunner.run_Insert_SQL(Insert_Collar_SQL);
                if (insertNum != -1)
                {
                    insertNum = sqlRunner.run_Insert_SQL(Insert_CollarDetail_SQL);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return insertNum;
        }
        [HttpPost]
        public int Handler_addNewAsset(string Asset_add)
        {
            int info = 0;
            //插入对象方式
            //info = addNewAsset_hanlder_ByClass(Asset_add);
            info = Handler_addNewAsset_ByClass(Asset_add);

            return info;
        }
        public JsonResult Load_Asset_Collor(int? page, int? rows, int role, int? state, int flag, String searchCondtiion, String selectedIDs)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            state = state == null ? 16 : state;
            List<int> checkIDS = ConvertStringToIntList(selectedIDs);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            }

            return loadAsset_By_Type(page, rows, role, dto_condition, checkIDS,SystemConfig.tableType_detail);

        }

        public JsonResult Load_SelectedAsset(int? page, int? rows, int role, int? state, int flag, String selectedIDs)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            state = state == null ? 16 : state;

            List<int> checkIDS = ConvertStringToIntList(selectedIDs);
            String cond = getSelectAsset_ID_cond(checkIDS);

            String selectSQL = getSelectAssets(flag, cond, rows, page);
            String selectSQLCounter = getSelectAsset_Counter(flag, cond);


            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(selectSQL);
            int resultCount = sqlRuner.runSelectSQL_Counter(selectSQLCounter, "total");

            List<dto_Asset_Detail> list = new List<dto_Asset_Detail>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                dto_Asset_Detail tmp = new dto_Asset_Detail();


                tmp.RowNo = int.Parse(dt.Rows[i]["RowNo"].ToString());
                tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                tmp.serial_number = dt.Rows[i]["serial_number"].ToString();
                tmp.name_Asset = dt.Rows[i]["name_Asset"].ToString();
                tmp.type_Asset = dt.Rows[i]["type_Asset"].ToString();
                tmp.specification = dt.Rows[i]["specification"].ToString();
                tmp.people_using = dt.Rows[i]["specification"].ToString();
                tmp.department_Using = dt.Rows[i]["department_Using"].ToString();
                tmp.measurement = dt.Rows[i]["measurement"].ToString();
                tmp.unit_price = double.Parse(dt.Rows[i]["unit_price"].ToString());
                tmp.addressCF = dt.Rows[i]["addressCF"].ToString();
                tmp.amount = int.Parse(dt.Rows[i]["amount"].ToString());
                tmp.value = double.Parse(dt.Rows[i]["value"].ToString());
                tmp.state_asset = dt.Rows[i]["state_asset"].ToString();
                tmp.supplierID = dt.Rows[i]["supplierID"].ToString();
                tmp.Method_add = dt.Rows[i]["Method_add"].ToString();
                list.Add(tmp);
            }
            var json = new
            {
                total = resultCount,
                rows = list.ToArray()
                //sql = selectSQL

            };

            return Json(json, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult LoadAssets(int? page, int? rows,  int tableType, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? role=commonConversion.getRole();

            JsonResult result=new JsonResult();
             JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            }

            List<int> selectedIDs = new List<int>();
            result = loadAsset_By_Type(page, rows, role, dto_condition, selectedIDs, tableType);
            return result;
        }

       
       

        //===============================================================Action  Area===================================================================================//

        //===============================================================Action Function Area===================================================================================//

        public List<dto_Asset_CollarDetail> ConvertDataTabelTo_dto_CollarDetail(DataTable dt)
        {
            List<dto_Asset_CollarDetail> list = new List<dto_Asset_CollarDetail>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                dto_Asset_CollarDetail tmp = new dto_Asset_CollarDetail();
                tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                tmp.serial_number = dt.Rows[i]["serial_number"].ToString();
                tmp.serial_number_Asset = dt.Rows[i]["serial_number_Asset"].ToString();
                tmp.flag = dt.Rows[i]["flag"].ToString();

                list.Add(tmp);

            }
            return list;

        }





        public JsonResult loadAsset_By_Type(int? page, int? rows, int? role, dto_SC_Asset dto_condition, List<int> selectedIDs,int dataType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JsonResult json = new JsonResult();

            if (dto_condition == null)
            {
                return null;
            }

            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
            switch (dto_condition.typeFlag)
            {
                case SystemConfig.searchPart_letf: json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs,dataType); break;
                case SystemConfig.searchPart_right: json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType,selectedIDs,dataType); break;
                default: ; break;
            }
            return json;
        }


        public JsonResult loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs,int? dataType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset)
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);

            switch (cond.DataType)
            {
                case SystemConfig.searchCondition_Date:
                    {
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

             switch(dataType)
            {
                case SystemConfig.tableType_detail:{
                         //在进行数据绑定
                    var data = from p in data_ORG
                               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                               from AT in temp_AT.DefaultIfEmpty()
                               join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                               from MM in temp_MM.DefaultIfEmpty()
                               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
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
                               select new dto_Asset_Detail
                               {
                                   addressCF = DZ.name_para,
                                   amount = p.amount,
                                   department_Using = DP.name_Department,
                                   depreciation_tatol = p.depreciation_tatol,
                                   depreciation_Month = p.depreciation_Month,
                                   ID = p.ID,
                                   measurement = MM.name_para,
                                   Method_add = MA.name_para,
                                   Method_depreciation = MDP.name_para,
                                   Method_decrease = MDC.name_para,
                                   name_Asset = p.name_Asset,
                                   Net_residual_rate = p.Net_residual_rate,
                                   Net_value = p.Net_value,
                                   people_using = p.people_using,
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
                    data = data.OrderByDescending(a => a.Time_Purchase);

                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                case SystemConfig.tableType_summary:{
                               //在进行数据绑定
                       var data_ORG2 = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
                                   join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                                   from MM in temp_MM.DefaultIfEmpty()
                                   select new {
                                       name_Asset=p.name_Asset,
                                       type_Asset=p.type_Asset,
                                       measurement=p.measurement,
                                       specification=p.specification,
                                       type_Asset_name=AT.name_Asset_Type,
                                       measurement_name=MM.name_para,
                                       amount=p.amount,
                                       value=p.value
                                   };
                        //数据分组
                       var data = from a in data_ORG2
                                  group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                  select new
                                  {
                                      amount = b.Sum(a => a.amount),
                                      AssetName = b.Key.name_Asset,
                                      AssetType = b.Key.type_Asset_name,
                                      measurement = b.Key.measurement_name,
                                      specification = b.Key.specification,
                                      value = b.Sum(a => a.value)
                                  };
                        data = data.OrderByDescending(a => a.AssetName);
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                default:{
                return null;
                };break;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="cond"></param>
        /// <param name="idsRight_deparment"></param>
        /// <param name="idsRight_assetType"></param>
        /// <returns></returns>
        public JsonResult loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs,int? dataType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            //JsonResult json = new JsonResult();

            int nodeid = (int)cond.nodeID;
            int dicID = nodeid / SystemConfig.ratio_dictPara;
            int dic_paraID = nodeid - (SystemConfig.ratio_dictPara * dicID);
            //获取DicNameFlag
            var data_nameFlag=from p in DB_C.tb_dataDict
                                         where p.active_flag==true
                                         where p.ID==dicID
                                         where p.name_flag!=null
                                         select new{
                                             nameFlag=p.name_flag
                                         };
            
            String nameFlag=null;
            foreach (var item in data_nameFlag){
                nameFlag = item.nameFlag;
            }
            if (nameFlag==null){
                return null;
            }
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset)
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);
            
            if (commonConversion.isALL(cond.nodeText) || dic_paraID == 0)
            {
            }
            else
            {
                switch (nameFlag)
                {
                    case SystemConfig.nameFlag_2_ZJFS_JIA:
                        {
                            data_ORG = from p in data_ORG
                                       where p.Method_add == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_JSFS:
                        {
                            data_ORG = from p in data_ORG
                                       where p.Method_decrease == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCZT:
                        {
                            data_ORG = from p in data_ORG
                                       where p.state_asset == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_CFDD:
                        {
                            data_ORG = from p in data_ORG
                                       where p.addressCF == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_SYBM:
                        {
                            data_ORG = from p in data_ORG
                                       where p.department_Using == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCLB:
                        {
                            data_ORG = from p in data_ORG
                                       where p.type_Asset == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_GYS:
                        {
                            data_ORG = from p in data_ORG
                                       where p.supplierID == dic_paraID
                                       select p;
                        }; break;
                    default: ; break;
                }
            }
            switch(dataType)
            {
                case SystemConfig.tableType_detail:{
                         //在进行数据绑定
                    var data = from p in data_ORG
                               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                               from AT in temp_AT.DefaultIfEmpty()
                               join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                               from MM in temp_MM.DefaultIfEmpty()
                               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
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
                               select new dto_Asset_Detail
                               {
                                   addressCF = DZ.name_para,
                                   amount = p.amount,
                                   department_Using = DP.name_Department,
                                   depreciation_tatol = p.depreciation_tatol,
                                   depreciation_Month = p.depreciation_Month,
                                   ID = p.ID,
                                   measurement = MM.name_para,
                                   Method_add = MA.name_para,
                                   Method_depreciation = MDP.name_para,
                                   Method_decrease = MDC.name_para,
                                   name_Asset = p.name_Asset,
                                   Net_residual_rate = p.Net_residual_rate,
                                   Net_value = p.Net_value,
                                   people_using = p.people_using,
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
                    data = data.OrderByDescending(a => a.Time_Purchase);

                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                case SystemConfig.tableType_summary:{
                               //在进行数据绑定
                       var data_ORG2 = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
                                   join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                                   from MM in temp_MM.DefaultIfEmpty()
                                   select new {
                                       name_Asset=p.name_Asset,
                                       type_Asset=p.type_Asset,
                                       measurement=p.measurement,
                                       specification=p.specification,
                                       type_Asset_name=AT.name_Asset_Type,
                                       measurement_name=MM.name_para,
                                       amount=p.amount,
                                       value=p.value
                                   };
                        //数据分组
                       var data = from a in data_ORG2
                                  group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                  select new
                                  {
                                      amount = b.Sum(a => a.amount),
                                      AssetName = b.Key.name_Asset,
                                      AssetType = b.Key.type_Asset_name,
                                      measurement = b.Key.measurement_name,
                                      specification = b.Key.specification,
                                      value = b.Sum(a => a.value)
                                  };
                        data = data.OrderByDescending(a => a.AssetName);
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                default:{
                return null;
                };break;
            }
        }


        






        public int deleteAssets(List<int> selectedIDs)
        {
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where selectedIDs.Contains(p.ID)
                       select p;
            if (data.Count() < 1)
            {
                return 0;
            }

            try {


                foreach (var item in data)
                {
                    item.flag = false;
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                return 0;
            }
        }

        public int update_Asset_State(int flag, String idStr, int state)
        {

            String UpdateSql = get_Update_Asset_State(flag, idStr, state);
            SQLRunner SqlRunner = new SQLRunner();
            return SqlRunner.run_Update_SQL(UpdateSql);
        }

        public int update_Asset_state_By_SerialNumber(String cond_SerialNumber,int state,int oldState)
        {
            String UpdateSql = getSQL_Update_Asset_state_By_SerialNumber(state,oldState,cond_SerialNumber);
            SQLRunner SqlRunner = new SQLRunner();
            return SqlRunner.run_Update_SQL(UpdateSql);
        }

        public JsonResult LoadCollarsList(int? page, int? rows, int role, int flag, String cond)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            String SQL_Str = GetCollarSelectSQL(page, rows, role, flag, cond);
            String SQL_Counter_Str = GetCollarSelectSQL_Counter(page, rows, role, flag, cond);
            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(SQL_Str);
            int resultCount = sqlRuner.runSelectSQL_Counter(SQL_Counter_Str, "total");
            List<dto_Collar> list = new List<dto_Collar>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dto_Collar tmp = new dto_Collar();
                tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                tmp.serialNumber = dt.Rows[i]["serialNumber"].ToString();
                tmp.operatorUser = dt.Rows[i]["operatorUser"].ToString();
                tmp.staff = dt.Rows[i]["staff"].ToString();
                tmp.state = dt.Rows[i]["state"].ToString();
                tmp.address = dt.Rows[i]["address"].ToString();
                tmp.department = dt.Rows[i]["department"].ToString();
                tmp.data_collar = (DateTime)dt.Rows[i]["data_collar"];
                tmp.date_Operated = (DateTime)dt.Rows[i]["date_Operated"];
                list.Add(tmp);
            }
            var json = new
            {
                total = resultCount,
                rows = list.ToArray()

            };
            return Json(json, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public int Handler_addNewAsset_ByClass(string Asset_add)
        {
            int insertNum = 0;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Asset_add dto_aa = serializer.Deserialize<Json_Asset_add>(Asset_add);
            //先判断是添加单个函数批量添加
            if (dto_aa.d_Check_PLZJ_add == true)//单数添加
            {
                dto_aa.flag = true;
                dto_aa.OperateTime = DateTime.Now;
                DB_C.tb_Asset.Add(JTM.ConverJsonToTable(dto_aa)); 
            }
            else
            { //批量添加

                String ruleType = "ZC";
                int num = (int)dto_aa.d_Num_PLTJ_add;
                CommonController tmc = new CommonController();
                ArrayList serailNums = tmc.getNewSerialNumber(ruleType, num);
                List<tb_Asset> datasToadd = new List<tb_Asset>();
                for (int i = 0; i < serailNums.Count; i++)
                {
                    dto_aa.d_ZCBH_add = serailNums[i].ToString().Trim();
                    dto_aa.flag = true;
                    dto_aa.OperateTime =DateTime.Now;
                    datasToadd.Add(JTM.ConverJsonToTable(dto_aa));
                }
                DB_C.tb_Asset.AddRange(datasToadd);
            }

            try
            {
                insertNum = DB_C.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return insertNum;
        }

        //===============================================================Action Function Area===================================================================================//


      







       

      


        //===============================================================Convert  Area===================================================================================//


        /**
         *根据ID获取到AssetList 
         * */
        public List<dto_Asset_Detail> getAssetByID(List<int> IDS)
        {
            List<dto_Asset_Detail> list = new List<dto_Asset_Detail>();

            String sql = getSQL_SelectAsset_By_ID(IDS);

            SQLRunner sqlRunner = new SQLRunner();

            DataTable dt = sqlRunner.runSelectSQL_dto(sql);

            list = convertToList_asset(dt);
            return list;
        }


        /**
         * 完整Asset转换
         * */
        public List<dto_Asset_Detail> convertToList_asset(DataTable dt)
        {
            List<dto_Asset_Detail> list = new List<dto_Asset_Detail>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dto_Asset_Detail tmp = new dto_Asset_Detail();
                tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                tmp.serial_number = dt.Rows[i]["serial_number"].ToString();
                tmp.name_Asset = dt.Rows[i]["name_Asset"].ToString();
                tmp.type_Asset = dt.Rows[i]["type_Asset"].ToString();
                tmp.specification = dt.Rows[i]["specification"].ToString();
                tmp.people_using = dt.Rows[i]["specification"].ToString();
                tmp.department_Using = dt.Rows[i]["department_Using"].ToString();
                tmp.measurement = dt.Rows[i]["measurement"].ToString();
                tmp.unit_price = double.Parse(dt.Rows[i]["unit_price"].ToString());
                tmp.addressCF = dt.Rows[i]["addressCF"].ToString();
                tmp.amount = int.Parse(dt.Rows[i]["amount"].ToString());
                tmp.value = double.Parse(dt.Rows[i]["value"].ToString());
                tmp.state_asset = dt.Rows[i]["state_asset"].ToString();
                tmp.supplierID = dt.Rows[i]["supplierID"].ToString();
                tmp.Method_add = dt.Rows[i]["Method_add"].ToString();
                list.Add(tmp);
            }

            return list;
        }
        public List<dto_collar_detail> converTo_dto_Collar_detail(DataTable dt)
        {
            List<dto_collar_detail> list = new List<dto_collar_detail>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dto_collar_detail tmp = new dto_collar_detail();
                tmp.serialNumber = dt.Rows[i]["serialNumber"].ToString();
                tmp.operatorUser = dt.Rows[i]["operatorUser"].ToString();
                tmp.staff = dt.Rows[i]["staff"].ToString();
                tmp.state = dt.Rows[i]["state"].ToString();
                tmp.address = dt.Rows[i]["address"].ToString();
                tmp.department = dt.Rows[i]["department"].ToString();
                tmp.reason = dt.Rows[i]["reason"].ToString();
                tmp.ps = dt.Rows[i]["ps"].ToString();
                tmp.data_collar = (DateTime)dt.Rows[i]["data_collar"];
                tmp.date_Operated = (DateTime)dt.Rows[i]["date_Operated"];
                list.Add(tmp);
            }
            return list;
        }



        public String TranslateSearchConditionToString(dto_SC_Asset cond)
        {
            String condStr = "";
            if (cond == null)
            {
                condStr = "";
            }
            else
            {

                if (cond.typeFlag == "left" && cond.nodeText != null)
                {
                    int nodeid = (int)cond.nodeID;
                    //获取DictParaID
                    List<tb_dataDict> dic = DB_C.tb_dataDict.Where(a => a.flag_Search == true).Take(1).ToList();
                    int ratio = 100000;
                    if (dic.Count > 0)
                    {
                        ratio = (int)dic[0].ratio;
                    }
                    int dicID = nodeid / ratio;
                    int dic_paraID = nodeid - (ratio * dicID);
                    condStr = getSC_DicParaLeft(dicID, dic_paraID, cond.nodeText);
                }
                else if (cond.typeFlag == "right" && cond.DataType != null)
                {
                    if (cond.DataType == "Date")
                    {
                        condStr = getSC_Date(cond);
                    }
                    else if (cond.DataType == "content")
                    {
                        condStr = getSC_content(cond);
                    }
                    else
                    {

                    }
                }
                else
                {

                }


            }
            return condStr;
        }
        //public tb_Asset convertAssetTbByJson(Json_Asset_add data)
        //{
        //    tb_Asset tb_asset_add = new tb_Asset();
        //    tb_asset_add.serial_number = data.d_ZCBH_add;
        //    tb_asset_add.name_Asset = data.d_ZCMC_add;
        //    tb_asset_add.type_Asset = data.d_ZCLB_add;
        //    tb_asset_add.specification = data.d_ZCXH_add;
        //    tb_asset_add.measurement = data.d_JLDW_add;
        //    tb_asset_add.unit_price = data.d_Other_ZCDJ_add;
        //    tb_asset_add.amount = data.d_Other_ZCSL_add;
        //    tb_asset_add.value = data.d_Other_ZCJZ_add;
        //    tb_asset_add.department_Using = data.d_SZBM_add;
        //    tb_asset_add.addressCF = data.d_CFDD_add;
        //    tb_asset_add.people_using = data.d_SYR_add;
        //    tb_asset_add.flag = data.flag;
        //    tb_asset_add.Time_add = data.OperateTime;
        //    tb_asset_add.supplierID = data.d_GYS_add;
        //    tb_asset_add.Time_Purchase = data.d_GZRQ_add;
        //    tb_asset_add.YearService_month = data.d_Other_SYNX_add;
        //    tb_asset_add.Method_depreciation = data.d_Other_ZJFS_add;
        //    tb_asset_add.Net_residual_rate = data.d_Other_JCZL_add;
        //    tb_asset_add.depreciation_Month = data.d_Other_YTZJ_add;
        //    tb_asset_add.depreciation_tatol = data.d_Other_LJZJ_add;
        //    tb_asset_add.Net_value = data.d_Other_JZ_add;
        //    tb_asset_add.Method_add = data.d_ZJFS_add;
        //    return tb_asset_add;
        //}




        public List<int> ConvertStringToIntList(String idStr)
        {
            List<int> results = new List<int>();
            try
            {
                if (idStr != null && idStr != "")
                {
                    String[] ids = idStr.Split('_');
                    foreach (String i in ids)
                    {
                        results.Add(int.Parse(i));
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            return results;
        }
        public String ConverToInsertSelectSQLString_CollarDetail(tb_Asset_collar_detail data)
        {
            String dataStr = "select '" + data.serial_number + "','" + data.serial_number_Asset + "','"+data.flag+"'";
            return dataStr;
        }
        public String ConverToInsertSelectSQLString_Collar(tb_Asset_collar data)
        {
            //(addree_Storage,date,date_Operated,department_collar,flag,operator,person,reason,serial_number,state_List)
            int flag = data.flag == true ? 1 : 0;

            String dataStr = "select '" + data.addree_Storage + "','" + data.date + "','" +
                              data.date_Operated + "','" + data.department_collar + "','" +
                              flag + "','" +data._operator+"','"+
                                data.person + "','" +data.reason + "','" +
                                data.serial_number + "'," +data.state_List;

            return dataStr;
        }

        public String Convert_SC_To_CondString(dto_SC_Allocation cond)
        {
            String condStr = "";
            return condStr;
        }

        public tb_Asset_collar ConvertJsonTo_CollorTB(Json_Collar_addNew json_collar, String SerialNumber, Boolean flag, DateTime dateNow, int operatorID)
        {
            tb_Asset_collar collar = new tb_Asset_collar();

            collar.serial_number = SerialNumber == null ? "" : SerialNumber;
            collar.state_List = json_collar.statelist;
            collar.person = json_collar.people_LY;
            collar.reason = json_collar.reason_LY;
            collar.flag = flag;
            collar.department_collar = json_collar.department_LY;
            collar.date = json_collar.date_LY;
            collar.date_Operated = dateNow;
            collar.addree_Storage = json_collar.address_LY;
            collar._operator = operatorID;
            return collar;

        }


        

        public List<tb_Asset_collar_detail> convertToList_CollarDetail(String serialNumber, List<dto_Asset_Detail> assets,int flag)
        {
            List<tb_Asset_collar_detail> list = new List<tb_Asset_collar_detail>();
            if (assets != null)
            {
                for (int i = 0; i < assets.Count; i++)
                {
                    tb_Asset_collar_detail detail = new tb_Asset_collar_detail();
                    detail.serial_number = serialNumber == null ? "" : serialNumber;
                    detail.serial_number_Asset = assets[i].serial_number == null ? "" : assets[i].serial_number;
                    detail.flag = flag==1?true:false;
                    list.Add(detail);
                }
            }
            return list;

        }
        //===============================================================Convert  Area===================================================================================//



       


        //===============================================================SQL  Area===================================================================================//
        public String ConverJsonAssetToSelectSQLString(Json_Asset_add data)
        {
            String dataStr = "select '" + data.d_ZCBH_add + "','" + data.d_ZCMC_add + "','" +
                              data.d_ZCLB_add + "','" + data.d_ZCXH_add + "','" +
                              data.d_JLDW_add + "'," + data.d_Other_ZCDJ_add + "," +
                              data.d_Other_ZCSL_add + "," + data.d_Other_ZCJZ_add + ",'" +
                              data.d_SZBM_add + "'," + data.d_CFDD_add + ",'" +
                              data.d_SYR_add + "','" + data.d_GYS_add + "','" +
                              data.d_GZRQ_add + "'," + data.d_Other_SYNX_add + "," +
                              data.d_Other_ZJFS_add + "," + data.d_Other_JCZL_add + "," +
                              data.d_Other_YTZJ_add + "," + data.d_Other_LJZJ_add + "," +
                              data.d_Other_JZ_add + "," + data.d_ZJFS_add;

            return dataStr;
        }

        public String getSQL_SelectAsset_By_ID(List<int> IDS)
        {

            String cond = getSelectAsset_ID_cond(IDS);
            String sql = "select * from tb_Asset a where flag=1  " + cond;
            return sql;
        }

        public String getSelectAssets(int flag, String cond, int? rows, int? page)
        {
            int beginIndex = ((int)page - 1) * (int)rows + 1;
            int endIndex = (int)page * (int)rows;
            String sql = "select top " + rows + " * from (select  ROW_NUMBER() OVER (ORDER BY a.ID) as RowNo, a.ID,dd.name_para addressCF, a.name_Asset,t.name_Asset_Type type_Asset ,a.serial_number,dep.name_Department department_Using,a.unit_price,a.amount,a.supplierID,st.name,a.value,m.name_para measurement,zt.name_para state_asset,f.name_para Method_add,a.specification from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID left join tb_dataDict_para dd on a.addressCF=dd.ID where flag=" + flag + " " + cond + ") sList where sList.RowNo between " + beginIndex + " and " + endIndex;
            return sql;
        }

        public String getSelectAsset_Counter(int flag, String cond)
        {
            String sql = "select count(*) as total from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID left join tb_dataDict_para dd on a.addressCF=dd.ID where flag=" + flag + " " + cond;
            return sql;
        }

        public String getSelectAsset_ID_cond(List<int> selectedID)
        {
            String cond = "";
            if (selectedID != null && selectedID.Count > 0)
            {
                cond = " and a.ID in ( ";
                for (int i = 0; i < selectedID.Count; i++)
                {
                    if (i == 0)
                    {
                        cond += selectedID[i];
                    }
                    else
                    {
                        cond += "," + selectedID[i];
                    }
                }
                cond += " )";
            }
            else
            {
                cond = " and a.ID in (0) ";
            }
            return cond;
        }

        public String getSelect_ID_cond_WithOut_A(List<int> selectedID)
        {
            String cond = "";
            if (selectedID != null && selectedID.Count > 0)
            {
                cond = " and ID in ( ";
                for (int i = 0; i < selectedID.Count; i++)
                {
                    if (i == 0)
                    {
                        cond += selectedID[i];
                    }
                    else
                    {
                        cond += "," + selectedID[i];
                    }
                }
                cond += " )";
            }
            else
            {
                cond = " and ID in (0) ";
            }
            return cond;
        }

        public String GetCollarSelectSQL(List<int> ids)
        {
            if (ids!=null&&ids.Count > 0)
            {

            }
            else {
                return "";
            }

            String condID = getSelectAsset_ID_cond(ids);
            String sql = "select a.reason,a.ps,a.ID,a.serial_number serialNumber, de.name_Department department,st.name staff,dd.name_para address,us.name_User operatorUser,stL.Name state,a.date data_collar,a.date_Operated from tb_Asset_collar a left join tb_department de on a.department_collar=de.ID_Department left join tb_staff st on a.person=st.ID left join tb_dataDict_para dd on a.addree_Storage=dd.ID left join tb_user us on a.operator=us.ID left join tb_State_List stL on a.state_List=stl.id where a.flag=1 " + condID + " order by a.date_Operated desc";
            return sql;
        }

        public String GetCollarSelectSQL(int? page, int? rows, int role, int flag, String cond)
        {
            String sql = "select top " + rows + " a.ID,a.serial_number serialNumber, de.name_Department department,st.name staff,dd.name_para address,us.name_User operatorUser,stL.Name state,a.date data_collar,a.date_Operated from tb_Asset_collar a left join tb_department de on a.department_collar=de.ID_Department left join tb_staff st on a.person=st.ID left join tb_dataDict_para dd on a.addree_Storage=dd.ID left join tb_user us on a.operator=us.ID left join tb_State_List stL on a.state_List=stl.id where a.flag=1 " + cond + "order by a.date_Operated desc";
            return sql;
        }

        public String GetCollarSelectSQL_Counter(int? page, int? rows, int role, int flag, String cond)
        {
            String sql = "select count(*) as total from tb_Asset_collar a left join tb_department de on a.department_collar=de.ID_Department left join tb_staff st on a.person=st.ID left join tb_dataDict_para dd on a.addree_Storage=dd.ID left join tb_user us on a.operator=us.ID left join tb_State_List stL on a.state_List=stl.id where a.flag=1 " + cond;
            return sql;
        }


        public String GetSQL_Select_Count_By_SeialNumberCond(String cond,int tagretState)
        {
            String sql = "select count(*) as total from tb_Asset where flag=1 and state_asset !=" + tagretState + " and serial_number in(" + cond + ")";
            return sql;
        }


        public String get_Update_Asset_State(int flag, String IdsStr, int state)
        {
            List<int> idList = ConvertStringToIntList(IdsStr);
            String cond = getSelect_ID_cond_WithOut_A(idList);
            String SQL = "update tb_Asset set state_asset=" + state + " where flag=" + flag + " " + cond;
            return SQL;
        }


        public String getSQL_Update_Asset_state_By_SerialNumber(int state,int oldState,String cond_serialNumber)
        {
            String SQL = "update tb_Asset set state_asset=" + state + " where state_asset=" + oldState + " and serial_number in (" + cond_serialNumber + ")";
            return SQL;
        }


        public String get_Insert_collar(List<tb_Asset_collar> collar)
        {
            String insertSql = "insert into tb_Asset_collar (addree_Storage,date,date_Operated,department_collar,flag,operator,person,reason,serial_number,state_List) ";
            DateTime date_op = DateTime.Now;

            if (collar.Count > 0)
            {
                for (int i = 0; i < collar.Count; i++)
                {
                    collar[i].date_Operated = date_op;
                    collar[i].flag = true;
                    if (i == 0)
                    {
                        insertSql += ConverToInsertSelectSQLString_Collar(collar[i]);
                    }
                    else
                    {
                        insertSql += " union all " + ConverToInsertSelectSQLString_Collar(collar[i]);
                    }



                }
            }
            else {
                return "" ;
            }

          
            return insertSql;

        }

       

        public String getCond_SerialNumber(List<dto_collar_detail> list)
        {
            String cond = "";

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        cond = "'" + list[i].serialNumber + "'";
                    }
                    else
                    {
                        cond += ",'" + list[i].serialNumber + "'";
                    }
                }
            }
            else
            {
                cond = "0";
               

            }
            return cond;

        }
        public String getSQL_Update_Collar_State(int state,String idStr)
        {
            List<int> idList = ConvertStringToIntList(idStr);
            String cond = "(";
            if (idList == null || idList.Count == 0)
            {
                cond += "0";
            }
            else {
                for (int i = 0; i < idList.Count; i++)
                {
                    if (i == 0)
                    {
                        cond += idList[i];
                    }
                    else
                    {
                        cond +=","+ idList[i];
                    }
                }
            }
            cond += ")";



            String sql = "update tb_Asset_collar set state_List=" + state + " where ID in " + cond;
                return sql;
        }
        public String getAssetInertQueryMuit(Json_Asset_add data)
        {

            String insertSql = "insert into tb_Asset(serial_number,name_Asset," +
                "type_Asset,specification," +
                "measurement,unit_price," +
                "amount,value," +
                "department_Using," +
                "addressCF,people_using," +
                "supplierID,Time_Purchase," +
                "YearService_month,Method_depreciation," +
                "Net_residual_rate,depreciation_Month," +
                "depreciation_tatol,Net_value,Method_add) ";

            //true表示单个插入
            if (data.d_Check_PLZJ_add == true)
            {
                String dataStr = ConverJsonAssetToSelectSQLString(data);
                insertSql += dataStr;
            }
            else //false 表示批量插入
            {

                if (data.d_Num_PLTJ_add > 0)
                {
                    //获取编号
                    int num = (int)data.d_Num_PLTJ_add;
                    String ruleType = "ZC";
                    CommonController tmc = new CommonController();
                    ArrayList serailNums = tmc.getNewSerialNumber(ruleType, num);

                    for (int i = 0; i < data.d_Num_PLTJ_add; i++)
                    {
                        data.d_ZCBH_add = serailNums[i].ToString().Trim();
                        if (i == 0)
                        {
                            insertSql += ConverJsonAssetToSelectSQLString(data);
                        }
                        else
                        {
                            insertSql += " union all " + ConverJsonAssetToSelectSQLString(data);
                        }



                    }
                }
                else
                {
                    insertSql = "";
                }


            }
            return insertSql;
        }

        

        public String get_Insert_collar_detail(List<tb_Asset_collar_detail> details)
        {
            String insertSql = "insert into tb_Asset_collar_detail (serial_number,serial_number_Asset,flag) ";

            if (details == null || details.Count == 0)
            {
                return "";
            }

            for (int i = 0; i < details.Count; i++)
            {

                if (i == 0)
                {
                    insertSql += ConverToInsertSelectSQLString_CollarDetail(details[i]);
                }
                else
                {
                    insertSql += " union all " + ConverToInsertSelectSQLString_CollarDetail(details[i]);
                }
            }
            return insertSql;
        }

        //public String getDeleteAssetSQL(List<int> selectedIDs)
        //{
        //    String deleteIIIDDD = "";
        //    if (selectedIDs.Count > 0)
        //    {
        //        for (int i = 0; i < selectedIDs.Count; i++)
        //        {
        //            if (i == 0)
        //            {
        //                deleteIIIDDD = selectedIDs[i] + "";
        //            }
        //            else
        //            {
        //                deleteIIIDDD += "," + selectedIDs[i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        deleteIIIDDD = "0";
        //    }
            
        //    String deleteSQL = "update tb_Asset set flag=0 where ID in (" + deleteIIIDDD + ");";
        //    return deleteSQL;
        //}
        public String addAssetState(String Cond, int? state)
        {
            int id = state == null ? 15 : (int)state;
            return Cond + " and state_asset=" + id + " ";
        }



        public String getSC_Date(dto_SC_Asset cond)
        {
            string condStr = "";

            String beginTime = ((DateTime)cond.begin).ToString("yyyy-MM-dd") + " 00:00:00";
            String endTime = ((DateTime)cond.end).ToString("yyyy-MM-dd") + " 23:59:59";
            if (cond.dataName == "GZRQ")
            {
                condStr = " and Time_Purchase between '" + beginTime + "'and '" + endTime + "'";
            }
            else if (cond.dataName == "DJRQ")
            {

                condStr = " and Time_add between '" + beginTime + "'and '" + endTime + "'";
            }
            else
            {

            }
            return condStr;

        }
        public String getSC_content(dto_SC_Asset cond)
        {

            //like '%A%'包含A的字符串
            String condStr = "";
            switch (cond.dataName)
            {
                case "ZCBH": condStr = " and serial_number like'%" + cond.contentSC + "%'"; break;

                case "ZCMC": condStr = " and  name_Asset like'%" + cond.contentSC + "%'"; break;

                case "ZCXH": condStr = " and  specification like '%" + cond.contentSC + "%'"; break;

                default: condStr = ""; break;
            }
            return condStr;
        }
        public String getSC_DicParaLeft(int dicID, int dic_paraID, String nodetext)
        {
            String condStr = "";
            switch (dicID)
            {
                case 3: condStr = " and Method_add=" + dic_paraID; break;

                case 4: condStr = ""; break;

                case 5: condStr = " and  state_asset=" + dic_paraID; break;

                case 9: condStr = "  and addressCF=" + dic_paraID; break;

                case 11: condStr = "  and department_Using=" + dic_paraID; break;

                case 12: condStr = "  and type_Asset=" + dic_paraID; break;

                case 13: condStr = "  and people_using=" + dic_paraID; break;

                case 14: condStr = "  and supplierID='" + nodetext + "'"; break;

                default: condStr = ""; break;
            }

            return condStr;
        }

        public String getDeleteCollarSQL(List<int> SelectedIDS)
        {
            if (SelectedIDS!=null&&SelectedIDS.Count > 0)
            {
                String IDcond=getSelect_ID_cond_WithOut_A(SelectedIDS);

                String SQL = "update tb_Asset_collar set flag=0 where flag=1 " + IDcond;
                return SQL;
            }
            else {
                return "";
            }
        }

        public String getSQL_SelectAssetBySerialNumber(List<tb_Asset_collar_detail> list)
        {
            String sql="";
            if (list != null && list.Count > 0)
            {

            }
            else {
            }

            return sql;


        }

        public String getSQL_Select_CollarDetail_BySerial_LY(String serialNum_LY_cond)
        {
            String sql = "select acd.*  from tb_Asset_collar_detail  acd left join tb_Asset_collar ac on acd.serial_number=ac.serial_number where ac.serial_number in (" + serialNum_LY_cond + ")";
            return sql;
        }


      


        public String getSQLCond_SerialNumber(List<tb_Asset_collar_detail> list)
        {
            String cond = "";
            if (list != null && list.Count > 0)
            {
                cond = " and a.serial_number in ( ";
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        cond += "'"+list[i].serial_number_Asset+"'";
                    }
                    else
                    {
                        cond += ",'" + list[i].serial_number_Asset+"'";
                    }
                }
                cond += " )";
            }
            else
            {
                cond = "and a.serial_number in ('0')";
            }
            return cond;

        }
        public String getCond_SerialNum_By_CollarDetail(List<dto_Asset_CollarDetail> list)
        {

            if (list.Count > 0)
            {
                String cond = "";
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        cond = "'" + list[i].serial_number_Asset + "'";
                    }
                    else
                    {
                        cond+= ",'" + list[i].serial_number_Asset + "'";
                    }
                }
                return cond;

            }
            else
            {
                return "0";
            }
        }



        //===============================================================SQL  Area===================================================================================//

        //===============================================================OtherFunction  Area===================================================================================//
        public String InsertAssets(FormCollection fc)
        {
            return "InsetCorret";
        }

        protected override void HandleUnknownAction(string actionName)
        {

            try
            {

                this.View(actionName).ExecuteResult(this.ControllerContext);

            }
            catch (InvalidOperationException ieox)
            {

                ViewData["error"] = "Unknown Action: \"" + Server.HtmlEncode(actionName) + "\"";

                ViewData["exMessage"] = ieox.Message;

                this.View("Error").ExecuteResult(this.ControllerContext);

            }

        }
    }

    

}