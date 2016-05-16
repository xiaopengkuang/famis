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



namespace FAMIS.Controllers
{
    public class AssetController : Controller
    {

        FAMISDBTBModels DB_Connecting = new FAMISDBTBModels();
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
            List<tb_Asset_collar> collar = DB_Connecting.tb_Asset_collar.Where(a => a.ID == ID).ToList();
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
            List<tb_Asset_collar_detail> acd = DB_Connecting.tb_Asset_collar_detail.Where(b=>b.flag==true).Where(a => a.serial_number==serNum).ToList();

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
            tb_Asset_collar collar_new = ConvertJsonTo_CollorTB(Json_collar, serialNumber_collar, true, dateNow, operatorID);
            List<tb_Asset_collar> collarsList = new List<tb_Asset_collar>();
            collarsList.Add(collar_new);

            //添加明细
            List<int> AssetIDs = ConvertStringToIntList(Json_collar.assetList);
            List<tb_Asset_collar_detail> details = convertToList_CollarDetail(serialNumber_collar, getAssetByID(AssetIDs));
            String Insert_Collar_SQL = get_Insert_collar(collarsList);

            String Insert_CollarDetail_SQL = get_Insert_collar_detail(details);

            int flag = 1;
            int state = 15;
            String Update_Collar_Asset = get_Update_Asset_State(flag, Json_collar.assetList, state);

            List<String> SqlList = new List<String>();
            SqlList.Add(Insert_Collar_SQL);
            SqlList.Add(Insert_CollarDetail_SQL);
            SqlList.Add(Update_Collar_Asset);
            SQLRunner sqlRunner = new SQLRunner();

            try
            {
                insertNum = sqlRunner.run_Insert_SQL(Insert_Collar_SQL);
                if (insertNum != -1)
                {
                    insertNum = sqlRunner.run_Insert_SQL(Insert_CollarDetail_SQL);
                    if (insertNum != -1)
                    {
                        insertNum = sqlRunner.run_Update_SQL(Update_Collar_Asset);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return insertNum;
        }
        [HttpPost]
        public int addNewAsset_hanlder(string Asset_add)
        {
            int info = 0;
            //插入对象方式
            info = addNewAsset_hanlder_ByClass(Asset_add);


            //SQL语句注入方式
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //Json_Asset_add dto_aa = serializer.Deserialize<Json_Asset_add>(Asset_add);
            //String insertSQL = getAssetInertQueryMuit(dto_aa);
            //SQLRunner sqlRunner = new SQLRunner();



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
            String condSC = "";
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
                condSC = TranslateSearchConditionToString(dto_condition);
            }
            else
            {
                condSC = "";
            }


            condSC = addAssetState(condSC, state);


            return loadAsset_detail_1(page, rows, role, flag, condSC, checkIDS);

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
        public JsonResult LoadAssets(int? page, int? rows, int role, int tableType, int flag, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            String condSC = "";
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
                condSC = TranslateSearchConditionToString(dto_condition);
            }
            else
            {
                condSC = "";
            }



            if (tableType == 1)
            {

                return loadAsset_detail_1(page, rows, role, flag, condSC, null);
            }
            else if (tableType == 0)
            {
                return loadAsset_Summary_0(page, rows, role, flag, condSC);
            }
            else
            {
                return null;
            }
        }

       
       

        //===============================================================Action  Area===================================================================================//

        //===============================================================Action Function Area===================================================================================//
        [HttpPost]
        public JsonResult loadAsset_Summary_0(int? page, int? rows, int role, int flag, String condition)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            String loadAssetSummarySQL = getAssetSQL_Summary(page, rows, role, flag, condition);
            String loadAssetSumCounter = getAssetSQL_Summary_Counter(page, rows, role, flag, condition);

            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(loadAssetSummarySQL);
            int resultCount = sqlRuner.runSelectSQL_Counter(loadAssetSumCounter, "total");
            List<dto_Asset_Summary> list = new List<dto_Asset_Summary>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                dto_Asset_Summary tmp = new dto_Asset_Summary();

                tmp.RowNo = int.Parse(dt.Rows[i]["RowNo"].ToString());
                tmp.AssetName = dt.Rows[i]["AssetName"].ToString();
                tmp.AssetType = dt.Rows[i]["AssetType"].ToString();
                tmp.specification = dt.Rows[i]["specification"].ToString();
                tmp.measurement = dt.Rows[i]["measurement"].ToString();
                tmp.amount = int.Parse(dt.Rows[i]["amount"].ToString());
                tmp.value = double.Parse(dt.Rows[i]["value"].ToString());
                list.Add(tmp);
            }
            var json = new
            {
                total = resultCount,
                rows = list.ToArray()

            };
            return Json(json, JsonRequestBehavior.AllowGet);

        }


        /**
         * 加载详细列表
         * */
        public JsonResult loadAsset_detail_1(int? page, int? rows, int role, int flag, String condition, List<int> selectedIDs)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            String selectSQL = getAssetSQL_detail(page, rows, role, flag, condition);
            String selectSQLCounter = getAssetSQL_detail_Counter(page, rows, role, flag, condition);


            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(selectSQL);
            int resultCount = sqlRuner.runSelectSQL_Counter(selectSQLCounter, "total");

            List<dto_Asset_Detail> list = new List<dto_Asset_Detail>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                dto_Asset_Detail tmp = new dto_Asset_Detail();


                if (selectedIDs != null && selectedIDs.Count > 0)
                {
                    //过滤selectedIDs
                    if (selectedIDs.Contains(int.Parse(dt.Rows[i]["ID"].ToString())))
                    {
                        continue;
                    }
                }

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
                rows = list.ToArray(),
            };

            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public int deleteAssets(List<int> selectedIDs)
        {

            String deleteSQL = getDeleteAssetSQL(selectedIDs);
            SQLRunner sqlRunner = new SQLRunner();
            int result = sqlRunner.executesql(deleteSQL);
            return result;

        }

        public int update_Asset_State(int flag, String idStr, int state)
        {

            String UpdateSql = get_Update_Asset_State(flag, idStr, state);
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
        public int addNewAsset_hanlder_ByClass(string Asset_add)
        {
            int insertNum = 0;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Asset_add dto_aa = serializer.Deserialize<Json_Asset_add>(Asset_add);
            //先判断是添加单个函数批量添加
            if (dto_aa.d_Check_PLZJ_add == true)//单数添加
            {
                //

                //info += dto_aa.d_ZCBH_add;
                dto_aa.flag = true;
                dto_aa.OperateTime = new DateTime();
                DB_Connecting.tb_Asset.Add(convertAssetTbByJson(dto_aa));
                //DB_Connecting.tb_Asset.Add(dto_aa);

            }
            else
            { //批量添加

                String ruleType = "ZC";
                int num = dto_aa.d_Num_PLTJ_add;
                CommonController tmc = new CommonController();
                ArrayList serailNums = tmc.getNewSerialNumber(ruleType, num);
                List<tb_Asset> datasToadd = new List<tb_Asset>();
                for (int i = 0; i < serailNums.Count; i++)
                {

                    dto_aa.d_ZCBH_add = serailNums[i].ToString().Trim();
                    //info += dto_aa.d_ZCBH_add;
                    dto_aa.flag = true;
                    dto_aa.OperateTime = new DateTime();
                    datasToadd.Add(convertAssetTbByJson(dto_aa));

                }
                DB_Connecting.tb_Asset.AddRange(datasToadd);



            }

            //int a = 0;
            try
            {
                insertNum = DB_Connecting.SaveChanges();
                //info = "提交成功";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments.
                // ...
                // Try again.
                //DB_Connecting.SaveChanges();
                //info += "提交失败" + e.ToString();
            }
            //// TODO   存入到数据库中




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
                    List<tb_dataDict> dic = DB_Connecting.tb_dataDict.Where(a => a.flag_Search == true).Take(1).ToList();
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
        public tb_Asset convertAssetTbByJson(Json_Asset_add data)
        {
            tb_Asset tb_asset_add = new tb_Asset();
            tb_asset_add.serial_number = data.d_ZCBH_add;
            tb_asset_add.name_Asset = data.d_ZCMC_add;
            tb_asset_add.type_Asset = data.d_ZCLB_add;
            tb_asset_add.specification = data.d_ZCXH_add;
            tb_asset_add.measurement = data.d_JLDW_add;
            tb_asset_add.unit_price = data.d_Other_ZCDJ_add;
            tb_asset_add.amount = data.d_Other_ZCSL_add;
            tb_asset_add.value = data.d_Other_ZCJZ_add;
            tb_asset_add.department_Using = data.d_SZBM_add;
            tb_asset_add.addressCF = data.d_CFDD_add;
            tb_asset_add.people_using = data.d_SYR_add;
            tb_asset_add.flag = data.flag;
            tb_asset_add.Time_add = data.OperateTime;
            tb_asset_add.supplierID = data.d_GYS_add;
            tb_asset_add.Time_Purchase = data.d_GZRQ_add;
            tb_asset_add.YearService_month = data.d_Other_SYNX_add;
            tb_asset_add.Method_depreciation = data.d_Other_ZJFS_add;
            tb_asset_add.Net_residual_rate = data.d_Other_JCZL_add;
            tb_asset_add.depreciation_Month = data.d_Other_YTZJ_add;
            tb_asset_add.depreciation_tatol = data.d_Other_LJZJ_add;
            tb_asset_add.Net_value = data.d_Other_JZ_add;
            tb_asset_add.Method_add = data.d_ZJFS_add;
            return tb_asset_add;
        }




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
            String dataStr = "select '" + data.serial_number + "','" + data.serial_number_Asset + "'";
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


        

        public List<tb_Asset_collar_detail> convertToList_CollarDetail(String serialNumber, List<dto_Asset_Detail> assets)
        {
            List<tb_Asset_collar_detail> list = new List<tb_Asset_collar_detail>();
            if (assets != null)
            {
                for (int i = 0; i < assets.Count; i++)
                {
                    tb_Asset_collar_detail detail = new tb_Asset_collar_detail();
                    detail.serial_number = serialNumber == null ? "" : serialNumber;
                    detail.serial_number_Asset = assets[i].serial_number == null ? "" : assets[i].serial_number;
                    list.Add(detail);
                }
            }
            return list;

        }
        //===============================================================Convert  Area===================================================================================//



       


        //===============================================================SQL  Area===================================================================================//
        public String ConverJsonAssetToSelectSQLString(Json_Asset_add data)
        {
            //String insertSql = "insert into tb_Asset(serial_number,name_Asset," +
            //  "type_Asset,specification," +
            //  "measurement,unit_price," +
            //  "amount,value," +
            //  "department_Using," +
            //  "addressCF,people_using," +
            //  "supplierID,Time_Purchase," +
            //  "YearService_month,Method_depreciation," +
            //  "Net_residual_rate,depreciation_Month," +
            //  "depreciation_tatol,Net_value,Method_add)";


            //String dataStr = " select '" + data.d_ZCBH_add + "','" + data.d_ZCMC_add + "','" + data.d_ZCLB_add + "','" + data.d_ZCXH_add + "','" + data.d_JLDW_add + "'," + data.d_Other_ZCDJ_add + "," + data.d_Other_ZCSL_add + "," + data.d_Other_ZCJZ_add + ",'" + data.d_SZBM_add + "','','','','2014-06-21 00:00:00',60,'',20,20,20,20,''"; 

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


        public String get_Update_Asset_State(int flag, String IdsStr, int state)
        {
            List<int> idList = ConvertStringToIntList(IdsStr);
            String cond = getSelect_ID_cond_WithOut_A(idList);
            String SQL = "update tb_Asset set state_asset=" + state + " where flag=" + flag + " " + cond;
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

        public String getAssetSQL_detail(int? page, int? rows, int role, int flag, String condition)
        {
            int beginIndex = ((int)page - 1) * (int)rows + 1;
            int endIndex = (int)page * (int)rows;


            //String sqlStr = "select top " + rows + " * from (select  ROW_NUMBER() OVER (ORDER BY a.ID) as RowNo,a.ID,a.name_Asset,t.name_Asset_Type type_Asset,dd.name_para addressCF,a.serial_number,dep.name_Department department_Using,a.unit_price,a.amount,a.supplierID,st.name,a.value,m.name_para measurement,zt.name_para state_asset,f.name_para Method_add,a.specification from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID left join tb_dataDict_para dd on a.addressCF=dd.ID where flag=" + flag + ") sList where sList.RowNo between " + beginIndex + " and " + endIndex;
            String sqlStr = "select top " + rows + " * from (select  ROW_NUMBER() OVER (ORDER BY a.ID) as RowNo,a.ID,dd.name_para addressCF, a.name_Asset,t.name_Asset_Type type_Asset ,a.serial_number,dep.name_Department department_Using,a.unit_price,a.amount,a.supplierID,st.name,a.value,m.name_para measurement,zt.name_para state_asset,f.name_para Method_add,a.specification from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID left join tb_dataDict_para dd on a.addressCF=dd.ID where flag=" + flag + condition + ") sList where sList.RowNo between " + beginIndex + " and " + endIndex;
            return sqlStr;
        }


        public String getAssetSQL_detail_Counter(int? page, int? rows, int role, int flag, String condition)
        {
            String sqlStr = "select count(*) as total from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID where flag=" + flag + condition;
            return sqlStr;
        }
        public String getAssetSQL_Summary(int? page, int? rows, int role, int flag, String condition)
        {
            int beginIndex = ((int)page - 1) * (int)rows + 1;
            int endIndex = (int)page * (int)rows;
            String sqlStr = "select top " + rows + " * from (select ROW_NUMBER() OVER (ORDER BY a.name_Asset) as RowNo,a.name_Asset AssetName,c.name_Asset_Type AssetType,b.name_para measurement,a.specification,SUM(a.amount) amount,Sum(a.value) value from tb_Asset as a left join tb_dataDict_para b on a.measurement=b.ID left join tb_AssetType c on a.type_Asset=c.assetTypeCode where a.flag=" + flag + condition + "   group by a.name_Asset,a.specification,b.name_para,c.name_Asset_Type) s_tb where s_tb.RowNo between " + beginIndex + " and " + endIndex;
            return sqlStr;
        }


        public String getAssetSQL_Summary_Counter(int? page, int? rows, int role, int flag, String condition)
        {
            String sqlStr = "select  count(*) as total from tb_Asset as a left join tb_dataDict_para b on a.measurement=b.ID left join tb_AssetType c on a.type_Asset=c.assetTypeCode where a.flag=" + flag + condition + "  group by a.name_Asset,a.specification,b.name_para,c.name_Asset_Type";
            return sqlStr;
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
                    int num = data.d_Num_PLTJ_add;
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
            String insertSql = "insert into tb_Asset_collar_detail (serial_number,serial_number_Asset) ";

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

        public String getDeleteAssetSQL(List<int> selectedIDs)
        {
            String deleteIIIDDD = "";
            if (selectedIDs.Count > 0)
            {
                for (int i = 0; i < selectedIDs.Count; i++)
                {
                    if (i == 0)
                    {
                        deleteIIIDDD = selectedIDs[i] + "";
                    }
                    else
                    {
                        deleteIIIDDD += "," + selectedIDs[i];
                    }
                }
            }
            else
            {
                deleteIIIDDD = "0";
            }
            
            String deleteSQL = "update tb_Asset set flag=0 where ID in (" + deleteIIIDDD + ");";
            return deleteSQL;
        }
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