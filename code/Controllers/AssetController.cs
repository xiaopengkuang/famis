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





         [HttpGet]
        public String loadSearchTreeByRole(String roleName)
        {


            TreeViewCommon treeviewCommon = new TreeViewCommon();
            String jsonStr = treeviewCommon.GetModule(roleName);
            return jsonStr;
        }




        [HttpGet]
         public JsonResult LoadAssets(int? page, int? rows, int role, int tableType,int flag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;

            if (tableType == 1)
            {
                return loadAsset_detail_1(page,rows,role,flag);
            }
            else if(tableType==0)
            {

                return loadAsset_Summary_0(page,rows,role,flag);
            }else{
                return null;
            }

            
            
            //page = page == null ? 1 : page;
            //rows = rows == null ? 1 : rows;
            //List<tb_Asset> list = DB_Connecting.tb_Asset.Where(b=>b.flag==flag).OrderByDescending(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();
            ////List<tb_user> list = DB_Connecting.tb_user.ToList();
            //var json = new
            //{
            //    total = DB_Connecting.tb_Asset.Where(b=>b.flag==flag).Count(),
            //    rows = (from r in list
            //            select new dto_Asset()
            //            {
            //                ID = r.ID,
            //                serial_number = r.serial_number,
            //                name_Asset = r.name_Asset,
            //                type_Asset = r.type_Asset,
            //                specification = r.specification,
            //                unit_price = r.unit_price,
            //                amount = r.amount,
            //                department_Using = r.department_Using,
            //                address = r.addressCF,
            //                state_asset = r.state_asset,
            //                value = r.value,
            //                supplierID = r.supplierID
            //            }).ToArray()
            //};
            //return Json(json, JsonRequestBehavior.AllowGet);
            
        }


        [HttpGet]
        public JsonResult loadAsset_Summary_0(int? page, int? rows, int role,int flag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;

            int beginIndex = ((int)page - 1) * (int)rows+1;

            int endIndex = (int)page*(int)rows;
            String loadAssetSummarySQL = "select top " + rows + " * from (select ROW_NUMBER() OVER (ORDER BY a.name_Asset) as RowNo,a.name_Asset AssetName,c.name_Asset_Type AssetType,b.name_para measurement,a.specification,SUM(a.amount) amount,Sum(a.value) value from tb_Asset as a left join tb_dataDict_para b on a.measurement=b.ID left join tb_AssetType c on a.type_Asset=c.assetTypeCode where a.flag=" + flag + "  group by a.name_Asset,a.specification,b.name_para,c.name_Asset_Type) s_tb where s_tb.RowNo between "+beginIndex+" and "+endIndex ;
            
            String loadAssetSumCounter = "select ROW_NUMBER() OVER (ORDER BY a.name_Asset) as RowNo,a.name_Asset AssetName,c.name_Asset_Type AssetType,b.name_para measurement,a.specification,SUM(a.amount) amount,Sum(a.value) value from tb_Asset as a left join tb_dataDict_para b on a.measurement=b.ID left join tb_AssetType c on a.type_Asset=c.assetTypeCode where a.flag=" + flag  + "  group by a.name_Asset,a.specification,b.name_para,c.name_Asset_Type";

            SQLRunner sqlRuner = new SQLRunner();

            DataTable dt = sqlRuner.runSelectSQL_dto_AssetSumm(loadAssetSummarySQL);
          

           int resultCount = sqlRuner.runSelectSQL_dto_AssetSumm_Counter(loadAssetSumCounter);

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
        public JsonResult loadAsset_detail_1(int? page, int? rows, int role,int flag)
        {
            //page = page == null ? 1 : page;
            //rows = rows == null ? 1 : rows;
            Boolean fllag=flag==1?true:false;
            List<tb_Asset> list = DB_Connecting.tb_Asset.Where(b => b.flag == fllag).OrderByDescending(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();
            var json = new
            {
                total = DB_Connecting.tb_Asset.Where(b => b.flag == fllag).Count(),
                rows = (from r in list
                        select new dto_Asset()
                        {
                            ID = r.ID,
                            serial_number = r.serial_number,
                            name_Asset = r.name_Asset,
                            type_Asset = r.type_Asset,
                            specification = r.specification,
                            unit_price = r.unit_price,
                            amount = r.amount,
                            department_Using = r.department_Using,
                            address = r.addressCF,
                            state_asset = r.state_asset,
                            value = r.value,
                            supplierID = r.supplierID
                        }).ToArray()
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
      
      
    }
}