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



namespace FAMIS.Controllers
{
    public class AssetController : Controller
    {

        FAMISDBTBModels DB_Connecting = new FAMISDBTBModels();
        // GET: Asset
        public ActionResult Accounting()
        {
            return View();
        }
        public ActionResult allocation()
        {
            return View();
        }


        public JsonResult LoadAllocation(int? page, int? rows, int role, int flag, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Allocation dto_condition = null;
            String condSC = "";
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Allocation>(searchCondtiion);
                condSC = SC_To_String(dto_condition);
            }
            else
            {
                condSC = "";
            }

            return loadAllocationList(page, rows, role, flag, condSC);


        }

        public JsonResult loadAllocationList(int? page, int? rows, int role, int flag,String cond)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            String SQL_Str = GetAllocationSelectSQL(page, rows, role, flag, cond);
            String SQL_Counter_Str = GetAllocationSelectSQL_Counter(page, rows, role, flag, cond);
            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(SQL_Str);
            int resultCount = sqlRuner.runSelectSQL_dto_Counter(SQL_Counter_Str);
            List<dto_allocation> list = new List<dto_allocation>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dto_allocation tmp = new dto_allocation();
                tmp.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                tmp.serialNumber = dt.Rows[i]["serialNumber"].ToString();
                tmp.operatorUser = dt.Rows[i]["operatorUser"].ToString();
                tmp.staff = dt.Rows[i]["staff"].ToString();
                tmp.state = dt.Rows[i]["state"].ToString();
                tmp.address = dt.Rows[i]["address"].ToString();
                tmp.department = dt.Rows[i]["department"].ToString();
                //tmp.date_allocation =(DateTime)dt.Rows[i]["date_allocation"];
                //tmp.date_Operated = (DateTime)dt.Rows[i]["date_Operated"];
                list.Add(tmp);
            }
            var json = new
            {
                total = resultCount,
                rows = list.ToArray()

            };
            return Json(json, JsonRequestBehavior.AllowGet);
            
        }

        public String GetAllocationSelectSQL(int? page, int? rows, int role, int flag, String cond)
        {
            String sql = "select a.ID,a.serial_number serialNumber, de.name_Department department,st.name staff,dd.name_para address,us.name_User operatorUser,stL.Name state,a.date date_allocation,a.date_Operated from tb_Asset_allocation a left join tb_department de on a.department_allocation=de.ID left join tb_staff st on a.person=st.ID left join tb_dataDict_para dd on a.addree_Storage=dd.ID left join tb_user us on a.operator=us.ID left join tb_State_List stL on a.state_List=stl.id where a.flag=1 " + cond + " order by a.date_Operated desc";
            return sql;
        }

        public String GetAllocationSelectSQL_Counter(int? page, int? rows, int role, int flag, String cond)
        {
            String sql = "select a.ID,a.serial_number serialNumber, de.name_Department department,st.name staff,dd.name_para address,us.name_User operatorUser,stL.Name state,a.date date_allocation,a.date_Operated from tb_Asset_allocation a left join tb_department de on a.department_allocation=de.ID left join tb_staff st on a.person=st.ID left join tb_dataDict_para dd on a.addree_Storage=dd.ID left join tb_user us on a.operator=us.ID left join tb_State_List stL on a.state_List=stl.id where a.flag=1 " + cond + " order by a.date_Operated desc";
            return sql;
        }

        public String SC_To_String(dto_SC_Allocation cond)
        {
            String condStr = "";
            return condStr;
        }

        public ActionResult collar()
        {
            return View();
        }
        public ActionResult reduction()
        {
            return View();
        }







        [HttpPost]
        public int addNewAsset_hanlder(string Asset_add)
        {
            int info =0 ;
            //插入对象方式
            info = addNewAsset_hanlder_ByClass(Asset_add);


            //SQL语句注入方式
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //Json_Asset_add dto_aa = serializer.Deserialize<Json_Asset_add>(Asset_add);
            //String insertSQL = getAssetInertQueryMuit(dto_aa);
            //SQLRunner sqlRunner = new SQLRunner();



            return info;
        }


        public String getAssetInertQueryMuit(Json_Asset_add data) {

            String insertSql = "insert into tb_Asset(serial_number,name_Asset,"+
                "type_Asset,specification,"+
                "measurement,unit_price,"+
                "amount,value,"+
                "department_Using,"+
                "addressCF,people_using,"+
                "supplierID,Time_Purchase,"+
                "YearService_month,Method_depreciation,"+
                "Net_residual_rate,depreciation_Month,"+
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

                    for (int i = 0; i < data.d_Num_PLTJ_add;i++ )
                    {
                        data.d_ZCBH_add = serailNums[i].ToString().Trim();
                        if (i == 0)
                        {
                            insertSql += ConverJsonAssetToSelectSQLString(data);
                        }
                        else {
                            insertSql += " union all " + ConverJsonAssetToSelectSQLString(data);
                        }



                    }
                }else{
                    insertSql="";
                }

                    
            }
            return insertSql;
        }


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
                insertNum=DB_Connecting.SaveChanges();
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


        public tb_Asset convertAssetTbByJson(Json_Asset_add data)
        {
            tb_Asset tb_asset_add = new tb_Asset();
            tb_asset_add.serial_number = data.d_ZCBH_add;
            tb_asset_add.name_Asset = data.d_ZCMC_add;
            tb_asset_add.type_Asset = data.d_ZCLB_add;
            tb_asset_add.specification = data.d_ZCXH_add;
            tb_asset_add.measurement = data.d_JLDW_add;
            tb_asset_add.unit_price = data.d_Other_ZCDJ_add;
            tb_asset_add.amount =data.d_Other_ZCSL_add;
            tb_asset_add.value =data.d_Other_ZCJZ_add;
            tb_asset_add.department_Using = data.d_SZBM_add;
            tb_asset_add.addressCF = data.d_CFDD_add;
            tb_asset_add.people_using = data.d_SYR_add;
            tb_asset_add.flag =data.flag ;
            tb_asset_add.Time_add = data.OperateTime;
            tb_asset_add.supplierID = data.d_GYS_add;
            tb_asset_add.Time_Purchase = data.d_GZRQ_add;
            tb_asset_add.YearService_month = data.d_Other_SYNX_add;
            tb_asset_add.Method_depreciation = data.d_Other_ZJFS_add;
            tb_asset_add.Net_residual_rate = data.d_Other_JCZL_add;
            tb_asset_add.depreciation_Month = data.d_Other_YTZJ_add;
            tb_asset_add.depreciation_tatol = data.d_Other_LJZJ_add;
            tb_asset_add.Net_value =data.d_Other_JZ_add;
            tb_asset_add.Method_add = data.d_ZJFS_add;
            return tb_asset_add;
        }





        // [HttpPost]
        //public String loadSearchTreeByRole(String roleName)
        //{
        //     //Do something with RoleName
        //    // Str
        //    //SQLRunner sqlRuner = new SQLRunner();
        //    //DataTable dt = sqlRuner.runSelectSQL_dto_AssetSumm(loadAssetSummarySQL);
        //    TreeViewCommon treeviewCommon = new TreeViewCommon();
        //    String jsonStr = treeviewCommon.GetModule(roleName);
        //    return jsonStr;
        //}
        // public String getSearchTreeSQL(String roleName)
        // {
        //     String sql = "select ID,name_para,ID_dataDict fatherid,url,orderID from  tb_dataDict_para  dic_PL where dic_PL.ID_dataDict in (select ID from tb_dataDict dic where dic.flag_search=1)"
        //            +" union all "
        //            +" select di_L.ID,di_L.name_dataDict name_para,0 fatherid,'' url,di_L.ID orederID from tb_dataDict di_L where di_L.flag_search=1 "
        //            +" union all "
        //            +" select deTB.ID_Department ID,deTB.name_Department name_para,deTB.ID_Father_Department fatherid,'' url,deTB.ID orderID from tb_department deTB "
        //            +" union all "
        //            +" select stf.ID,stf.name name_para,13 fatherid,'' url,stf.ID orderID from tb_staff stf";
        //     return sql;
        // }




        [HttpPost]
         public JsonResult LoadAssets(int? page, int? rows, int role, int tableType, int flag, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            String condSC = "";
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
                 condSC = TranslateSearchConditionToString(dto_condition);
            }
            else {
                condSC = "";
            }

          

            if (tableType == 1)
            {
                
                return loadAsset_detail_1(page, rows, role, flag, condSC);
            }
            else if(tableType==0)
            {
                return loadAsset_Summary_0(page, rows, role, flag, condSC);
            }else{
                return null;
            }
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

                if (cond.typeFlag=="left"&&cond.nodeText!=null)
                {
                    int nodeid = (int)cond.nodeID;
                    //获取DictParaID
                    List<tb_dataDict> dic = DB_Connecting.tb_dataDict.Where(a => a.flag_Search == true).Take(1).ToList();
                    int ratio=100000;
                    if (dic.Count > 0)
                    {
                        ratio =(int)dic[0].ratio;
                    }
                    int dicID = nodeid / ratio;
                    int dic_paraID = nodeid - (ratio * dicID);
                    condStr = getSC_DicParaLeft(dicID, dic_paraID,cond.nodeText);
                }
                else if (cond.typeFlag == "right"&&cond.DataType!=null)
                {
                    if (cond.DataType == "Date")
                    {
                        condStr = getSC_Date(cond);
                    }
                    else if (cond.DataType == "content")
                    {
                        condStr = getSC_content(cond);
                    }
                    else { 

                    }
                }
                else { 

                }


            }
            return condStr;
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
            else { 

            }
            return condStr;
            
        }
        public String getSC_content(dto_SC_Asset cond)
        {

            //like '%A%'包含A的字符串
            String condStr = "";
            switch (cond.dataName)
            {
                case "ZCBH": condStr = " and serial_number like'%" + cond.contentSC+"%'"; break;

                case "ZCMC": condStr = " and  name_Asset like'%" + cond.contentSC + "%'"; break;

                case "ZCXH": condStr = " and  specification like '%" + cond.contentSC+"%'"; break;

                default :condStr=""; break; 
            }

            return condStr;
            

        }


        public String getSC_DicParaLeft(int dicID,int dic_paraID,String nodetext)
        {
            String condStr = "";
            switch (dicID){ 
            case 3 : condStr=" and Method_add="+dic_paraID; break;

            case 4: condStr =""; break;

            case 5: condStr = " and  state_asset=" + dic_paraID; break;

            case 9: condStr = "  and addressCF=" + dic_paraID; break;

            case 11: condStr = "  and department_Using=" + dic_paraID; break;

            case 12: condStr = "  and type_Asset=" + dic_paraID; break;

            case 13: condStr = "  and people_using=" + dic_paraID; break;

            case 14: condStr = "  and supplierID='" + nodetext+"'"; break; 

            default :condStr=""; break; 
            }

            return condStr;
        }




        [HttpPost]
        public JsonResult loadAsset_Summary_0(int? page, int? rows, int role,int flag,String condition)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;

            String loadAssetSummarySQL = getAssetSQL_Summary(page, rows, role, flag,condition);
            String loadAssetSumCounter = getAssetSQL_Summary_Counter(page, rows, role, flag,condition);

            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(loadAssetSummarySQL);
           int resultCount = sqlRuner.runSelectSQL_dto_Counter(loadAssetSumCounter);
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
        public JsonResult loadAsset_detail_1(int? page, int? rows, int role, int flag, String condition)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;
            String selectSQL = getAssetSQL_detail(page, rows, role, flag, condition);
            String selectSQLCounter = getAssetSQL_detail_Counter(page, rows, role, flag, condition);


            SQLRunner sqlRuner = new SQLRunner();
            DataTable dt = sqlRuner.runSelectSQL_dto(selectSQL);
            int resultCount = sqlRuner.runSelectSQL_dto_Counter(selectSQLCounter);

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
                rows = list.ToArray(),
            };

            return Json(json, JsonRequestBehavior.AllowGet);
            
        }

        public String getAssetSQL_detail(int? page, int? rows, int role, int flag, String condition)
        {
            int beginIndex = ((int)page - 1) * (int)rows + 1;
            int endIndex = (int)page * (int)rows;


            //String sqlStr = "select top " + rows + " * from (select  ROW_NUMBER() OVER (ORDER BY a.ID) as RowNo,a.ID,a.name_Asset,t.name_Asset_Type type_Asset,dd.name_para addressCF,a.serial_number,dep.name_Department department_Using,a.unit_price,a.amount,a.supplierID,st.name,a.value,m.name_para measurement,zt.name_para state_asset,f.name_para Method_add,a.specification from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID left join tb_dataDict_para dd on a.addressCF=dd.ID where flag=" + flag + ") sList where sList.RowNo between " + beginIndex + " and " + endIndex;
            String sqlStr = "select top " + rows + " * from (select  ROW_NUMBER() OVER (ORDER BY a.ID) as RowNo,a.ID,dd.name_para addressCF, a.name_Asset,t.name_Asset_Type type_Asset ,a.serial_number,dep.name_Department department_Using,a.unit_price,a.amount,a.supplierID,st.name,a.value,m.name_para measurement,zt.name_para state_asset,f.name_para Method_add,a.specification from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID left join tb_dataDict_para dd on a.addressCF=dd.ID where flag=" + flag + condition+") sList where sList.RowNo between " + beginIndex + " and " + endIndex;
            return sqlStr;
        }


        public String getAssetSQL_detail_Counter(int? page, int? rows, int role, int flag, String condition)
        {
            String sqlStr = "select  ROW_NUMBER() OVER (ORDER BY a.ID) as RowNo,a.ID,t.name_Asset_Type name_Asset,a.serial_number,dep.name_Department department_Using,a.unit_price,a.amount,a.supplierID,st.name department_Using,a.value,m.name_para measurement,zt.name_para state_asset,f.name_para Method_add,a.specification from tb_Asset a left join tb_AssetType t on a.type_Asset=t.assetTypeCode left join tb_dataDict_para m on a.measurement=m.ID left join tb_dataDict_para f on a.Method_add=f.ID left join tb_staff st on a.people_using=st.ID left join tb_department dep on a.department_Using=dep.ID_Department left join tb_dataDict_para zt on a.state_asset=zt.ID where flag=" + flag +condition;
            return sqlStr;
        }
        public String getAssetSQL_Summary(int? page, int? rows, int role, int flag, String condition)
        {
            int beginIndex = ((int)page - 1) * (int)rows + 1;
            int endIndex = (int)page * (int)rows;
            String sqlStr = "select top " + rows + " * from (select ROW_NUMBER() OVER (ORDER BY a.name_Asset) as RowNo,a.name_Asset AssetName,c.name_Asset_Type AssetType,b.name_para measurement,a.specification,SUM(a.amount) amount,Sum(a.value) value from tb_Asset as a left join tb_dataDict_para b on a.measurement=b.ID left join tb_AssetType c on a.type_Asset=c.assetTypeCode where a.flag=" + flag + condition+"   group by a.name_Asset,a.specification,b.name_para,c.name_Asset_Type) s_tb where s_tb.RowNo between " + beginIndex + " and " + endIndex;
            return sqlStr;
        }


        public String getAssetSQL_Summary_Counter(int? page, int? rows, int role, int flag, String condition)
        {
            String sqlStr = "select ROW_NUMBER() OVER (ORDER BY a.name_Asset) as RowNo,a.name_Asset AssetName,c.name_Asset_Type AssetType,b.name_para measurement,a.specification,SUM(a.amount) amount,Sum(a.value) value from tb_Asset as a left join tb_dataDict_para b on a.measurement=b.ID left join tb_AssetType c on a.type_Asset=c.assetTypeCode where a.flag=" + flag +condition+ "  group by a.name_Asset,a.specification,b.name_para,c.name_Asset_Type";
            return sqlStr;
        }

        public int deleteAssets(List<int> selectedIDs)
        {

            String deleteSQL = getDeleteAssetSQL(selectedIDs);
            SQLRunner sqlRunner = new SQLRunner();
            int result = sqlRunner.executesql(deleteSQL);
            return result;

        }

        public String getDeleteAssetSQL(List<int> selectedIDs)
        {
            String deleteIIIDDD="";
            for (int i = 0; i < selectedIDs.Count; i++)
            {
                if (i == 0)
                {
                    deleteIIIDDD = selectedIDs[i]+"";
                }
                else {
                    deleteIIIDDD += "," + selectedIDs[i];
                }
            }
            String deleteSQL = "update tb_Asset set flag=0 where ID in (" + deleteIIIDDD + ");";
            return deleteSQL;
        }




        public ActionResult AddAsset()
        {
            return View();
        }

       

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