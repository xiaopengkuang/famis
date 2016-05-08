using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using FAMIS.DAL;
using System.Web.Script.Serialization;
using FAMIS.Models;
using System.Runtime.Serialization.Json;
using FAMIS.DTO;

namespace FAMIS.Controllers
{
    public class DictController : Controller
    {


        FAMISDBTBModels DB_Connecting = new FAMISDBTBModels();
        StringBuilder result_tree_department = new StringBuilder();
        StringBuilder sb_tree_department = new StringBuilder();

        StringBuilder result_tree_Address = new StringBuilder();
        StringBuilder sb_tree_Address = new StringBuilder();
        // GET: Dict
        public ActionResult staff()
        {
            return View();
        }
        public ActionResult dataDict()
        {
            return View();
        }
        public ActionResult Asset_type()
        {
            return View();
        }
        public ActionResult supplier()
        {
            return View();
        }

        [HttpGet]
        public JsonResult load_GYS_add()
        {

            List<tb_supplier> list = DB_Connecting.tb_supplier.OrderBy(a => a.ID).ToList();
            var json = new
            {
                total = list.Count(),
                rows = (from r in list
                        select new dto_supplier()
                        {
                            name_supplier = r.name_supplier,
                            linkman = r.linkman,
                            address = r.address
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public String load_ZCXH_add(String assetType)
        {
             List<tb_Asset> list;
             if (assetType != "" && assetType != "all")
             {
                 list = DB_Connecting.tb_Asset.Where(a => a.type_Asset == assetType).Distinct().ToList() ;
             }
             else {
                 list =  DB_Connecting.tb_Asset.Distinct().ToList() ;
             }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new dto_Asset_ZCXH()
                          {
                              ZCXH = r.specification
                             
                          }).Distinct().ToList(); ;

            String json = jss.Serialize(result).ToString().Replace("\\", "");
            return json;
        }


      

        [HttpGet]
        /**
         * 加载增加方式
         * */
        public String load_FS_add(int? TypeID)
        {

            List<tb_dataDict_para> list = DB_Connecting.tb_dataDict_para.Where(a => a.ID_dataDict == TypeID).OrderBy(a => a.ID).ToList();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new dto_DataDict_para()
                          {
                              ID = r.ID,
                              name_para = r.name_para
                          }).ToList(); 

            String json = jss.Serialize(result).ToString().Replace("\\", "");
            return json;
        }

        [HttpGet]
        public String load_SYR_add(String SZBM_ID)
        {
            List<tb_staff> list = DB_Connecting.tb_staff.Where(a => a.code_Departmen == SZBM_ID).OrderBy(a => a.ID).ToList();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = (from r in list
                          select new dto_Staff()
                          {
                              id = r.ID_Staff,
                              name = r.name
                          }).ToList(); ;

            String json = jss.Serialize(result).ToString().Replace("\\", "");
            return json;
        }




         [HttpGet]
        public String load_SZBM()
        {
            return GenerateTree_Department();
        }

         public String load_ZCLB()
         {
           return   GenerateTree_AssetType();
         }




         public String GenerateTree_AssetType()
         {
             List<tb_AssetType> list_de_AT = DB_Connecting.tb_AssetType.ToList();
             result_tree_department.Clear();
             sb_tree_department.Clear();
             List<dto_AssetType> list_dto_AT = new List<dto_AssetType>();
             list_de_AT.ForEach(item =>
             {
                 dto_AssetType tmp_dto_AT = new dto_AssetType();

                 tmp_dto_AT.id = item.assetTypeCode.Trim();
                 tmp_dto_AT.fatherID = item.father_MenuID_Type.Trim();
                 tmp_dto_AT.nameText = item.name_Asset_Type.Trim();
                 tmp_dto_AT.url = item.url.Trim();
                 tmp_dto_AT.orderID = item.orderID;
                 list_dto_AT.Add(tmp_dto_AT);
             });
             DataSet ds_de = ConvertToDataSet(list_dto_AT);
             DataTable dt_de = new DataTable();
             dt_de = ds_de.Tables[0];

             string json = GetTreeJsonByTable_Department(dt_de, "id", "nameText", "url", "fatherID", "0");
             return json;
         }


         [HttpGet]
         public String load_CFDD_add(int id_di)
         {

             List<tb_dataDict_para> list_ad_AT = DB_Connecting.tb_dataDict_para.Where(a => a.ID_dataDict == id_di).ToList();
             result_tree_department.Clear();
             sb_tree_department.Clear();
             List<dto_CFDD_Asset> list_dto_ad_AT = new List<dto_CFDD_Asset>();
             list_ad_AT.ForEach(item =>
             {
                 dto_CFDD_Asset tmp_dto_AT = new dto_CFDD_Asset();

                 tmp_dto_AT.id = item.ID;
                 tmp_dto_AT.fatherid = item.fatherid;
                 tmp_dto_AT.url = item.url;
                 tmp_dto_AT.nameText = item.name_para;
                 tmp_dto_AT.orderID = item.orderID;
                 list_dto_ad_AT.Add(tmp_dto_AT);
             });
             DataSet ds_ad = ConvertToDataSet(list_dto_ad_AT);
             DataTable dt_ad = new DataTable();
             dt_ad = ds_ad.Tables[0];

             string json = GetTreeJsonByTable_Address(dt_ad, "id", "nameText", "url", "fatherID", "0");
             return json;
         }






       
         public String GenerateTree_Department()
         {
             List<tb_department> list_de = DB_Connecting.tb_department.ToList();
             result_tree_department.Clear();
             sb_tree_department.Clear();
             List<dto_department> list_dto=new List<dto_department>();
             list_de.ForEach(item =>
             {
                 dto_department tmp_dto=new dto_department();

                 tmp_dto.id = item.ID_Department;
                 tmp_dto.fatherID = item.ID_Father_Department;
                 tmp_dto.nameText = item.name_Department;
                 tmp_dto.url = item.url;
                 tmp_dto.orderNum = item.orderNum;
                 list_dto.Add(tmp_dto);
             });




             DataSet ds_de = ConvertToDataSet(list_dto); 
             DataTable dt_de = new DataTable();
             dt_de = ds_de.Tables[0];

             string json = GetTreeJsonByTable_Department(dt_de, "id", "nameText", "url", "fatherID", "0");
             return json;
  
         }





         public DataSet ConvertToDataSet<T>(IList<T> list)
         {
             if (list == null || list.Count <= 0)
             {
                 return null;
             }

             DataSet ds = new DataSet();
             DataTable dt = new DataTable(typeof(T).Name);
             DataColumn column;
             DataRow row;

             System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

             foreach (T t in list)
             {
                 if (t == null)
                 {
                     continue;
                 }

                 row = dt.NewRow();

                 for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                 {
                     System.Reflection.PropertyInfo pi = myPropertyInfo[i];

                     string name = pi.Name;

                     if (dt.Columns[name] == null)
                     {
                         column = new DataColumn(name, pi.PropertyType);
                         dt.Columns.Add(column);
                     }

                     row[name] = pi.GetValue(t, null);
                 }

                 dt.Rows.Add(row);
             }

             ds.Tables.Add(dt);

             return ds;
         }

				


        public String GetTreeJsonByTable_Department(DataTable tabel, string idCol, string txtCol, string url, string rela, object pId)
        {
            result_tree_department.Append(sb_tree_department.ToString());
            sb_tree_department.Clear();
            if (tabel.Rows.Count > 0)
            {
                sb_tree_department.Append("[");
                string filer = string.Format("{0}='{1}'", rela, pId);
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        sb_tree_department.Append("{\"id\":\"" + row[idCol] + "\",\"text\":\"" + row[txtCol] + "\",\"attributes\":\"" + row[url] + "\",\"state\":\"open\"");
                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            sb_tree_department.Append(",\"children\":");
                            GetTreeJsonByTable_Department(tabel, idCol, txtCol, url, rela, row[idCol]);
                            result_tree_department.Append(sb_tree_department.ToString());
                            sb_tree_department.Clear();
                        }
                        result_tree_department.Append(sb_tree_department.ToString());
                        sb_tree_department.Clear();
                        sb_tree_department.Append("},");
                    }
                    sb_tree_department = sb_tree_department.Remove(sb_tree_department.Length - 1, 1);
                }
                sb_tree_department.Append("]");
                result_tree_department.Append(sb_tree_department.ToString());
                sb_tree_department.Clear();
            }
            return result_tree_department.ToString();
        }



        public String GetTreeJsonByTable_Address(DataTable tabel, string idCol, string txtCol, string url, string rela, object pId)
        {
            result_tree_Address.Append(sb_tree_Address.ToString());
            sb_tree_Address.Clear();
            if (tabel.Rows.Count > 0)
            {
                sb_tree_Address.Append("[");
                string filer = string.Format("{0}='{1}'", rela, pId);
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        sb_tree_Address.Append("{\"id\":\"" + row[idCol] + "\",\"text\":\"" + row[txtCol] + "\",\"attributes\":\"" + row[url] + "\",\"state\":\"open\"");
                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            sb_tree_Address.Append(",\"children\":");
                            GetTreeJsonByTable_Address(tabel, idCol, txtCol, url, rela, row[idCol]);
                            result_tree_Address.Append(sb_tree_Address.ToString());
                            sb_tree_Address.Clear();
                        }
                        result_tree_Address.Append(sb_tree_Address.ToString());
                        sb_tree_Address.Clear();
                        sb_tree_Address.Append("},");
                    }
                    sb_tree_Address = sb_tree_Address.Remove(sb_tree_Address.Length - 1, 1);
                }
                sb_tree_Address.Append("]");
                result_tree_Address.Append(sb_tree_Address.ToString());
                sb_tree_Address.Clear();
            }
            return result_tree_Address.ToString();
        }


    }
}