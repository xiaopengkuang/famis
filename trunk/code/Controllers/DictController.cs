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

        FAMISDBTBModels db = new FAMISDBTBModels();
        
        StringBuilder sb_tree_SearchTree = new StringBuilder();
        StringBuilder result_tree_SearchTree = new StringBuilder();
        
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

        public ActionResult Error()
        {
            return View();
        }
        public ActionResult add_AssetType(int? pid,String pname)
        {
            if (pname == null || pname == "" || pid == null)
            {
                ViewBag.info = pname;
                return View("Error");
            }
            //return View("Error");

            if (pid!=null&&pname!="")
            {
                ViewBag.fatherID = pid;
                ViewBag.fatherName = pname;
                return View();
            }
            else
            {
                ViewBag.info = pname+"\tss";
                return View("Error");
            }

            
             
            
        }

        public ActionResult edit_AssetType(int? id,String name)
        {
            if (name == null || name == ""||id==null)
            {
                ViewBag.info = name;
                return View("Error");
            }
            ViewBag.name = name;

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
                        }).ToArray(),

                        sql=getSearchTreeSQL(""),
                countre=couterTest() 
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


        [HttpPost]
        public ActionResult AddStaff([Bind(Include = "ID_Staff,code_Departmen,sex,entry_Time,phoneNumber,email,effective_Flag,create_TIME,invalid_TIME,_operator,name")] tb_staff staff)
        {
            /*StreamWriter sw = new StreamWriter("D:\\123456.txt");
            sw.Write(staff.ID_Staff + "\r\n");
            sw.Write(staff.code_Departmen + "\r\n");
            sw.Write(staff.sex+ "\r\n");
            sw.Write(staff.entry_Time + "\r\n");
            sw.Write(staff.phoneNumber + "\r\n");
            sw.Write(staff.email + "\r\n");
            sw.Write(staff.effective_Flag+"\r\n");
            sw.Write(staff.create_TIME + "\r\n");
            sw.Write(staff.invalid_TIME + "\r\n");
            sw.Write(staff._operator + "\r\n");
            sw.Write(staff.name + "\r\n");
            sw.Close();*/

            if (ModelState.IsValid)
            {
                db.tb_staff.Add(staff);
                db.SaveChanges();

            }

            return View();


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

                 tmp_dto_AT.id = item.assetTypeCode.ToString();
                 tmp_dto_AT.fatherID = item.father_MenuID_Type.ToString();
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
         [HttpPost]
         public String loadSearchTreeByRole(String roleName)
         {
             List<dto_TreeNode> tree = new List<dto_TreeNode>();
            
             tree = getTreeSearchNodes(roleName);
             DataSet ds_tree = ConvertToDataSet(tree);
             DataTable dt_tree = new DataTable();
             dt_tree = ds_tree.Tables[0];
             result_tree_SearchTree.Clear();
             sb_tree_SearchTree.Clear();
             string json = GetTreeJsonByTable_TreeSearch(dt_tree, "id", "nameText", "url", "fatherID", "0");
             return json;
         }

         public List<dto_TreeNode> getTreeSearchNodes(String role)
         {
             List<tb_dataDict> dic = DB_Connecting.tb_dataDict.Where(a => a.flag_Search == true).ToList();
             List<dto_TreeNode> nodesAll = new List<dto_TreeNode>();
             for (int i = 0; i < dic.Count; i++)
             {
                 dto_TreeNode fathernode = new dto_TreeNode();
                 fathernode.id = (dic[i].ID*dic[i].ratio).ToString();
                 fathernode.nameText = dic[i].name_dataDict;
                 fathernode.url = "";
                 fathernode.orderID = dic[i].ID.ToString();
                 fathernode.fatherID = "0";
                 
                 if (dic[i].tb_Ref != null && dic[i].tb_Ref != "")
                 {

                    
                     List<dto_TreeNode> tmp = new List<dto_TreeNode>();
                     if (dic[i].tb_Ref == "tb_department")
                     {
                         tmp = getSZBMNodes(fathernode);
                     }
                     else if (dic[i].tb_Ref == "tb_supplier")
                     {
                         tmp = getGYSNodes(fathernode);
                     }
                     else if (dic[i].tb_Ref == "tb_AssetType")
                     {
                         tmp = getZCLBNodes(fathernode);
                     }
                     else if (dic[i].tb_Ref == "tb_staff")
                     {
                         tmp = getSYRNodes(fathernode);
                     }
                     else{

                      }

                  
                     if (tmp.Count > 0)
                     {
                         nodesAll.AddRange(tmp);
                     }
                 }
                 else
                 {
                     List<dto_TreeNode> tmp = new List<dto_TreeNode>();
                     tmp = getDictNodes(dic[i].ID,fathernode);
                     if (tmp.Count > 0)
                     {
                         nodesAll.AddRange(tmp);
                     }
                 }
                    
             }
             return nodesAll;
         }
         public List<dto_TreeNode> getZCLBNodes(dto_TreeNode fathernode)
         {
             List<tb_AssetType> list_ORG = DB_Connecting.tb_AssetType.ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 node.id = (int.Parse(fathernode.id) + list_ORG[i].assetTypeCode).ToString(); ;
                 node.nameText = list_ORG[i].name_Asset_Type;
                 node.url = "";
                 node.orderID = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
                 node.fatherID = (int.Parse(fathernode.id) + list_ORG[i].father_MenuID_Type).ToString();
                 list.Add(node);
             }

             list.Add(fathernode);
             return list;
         }


         public List<dto_TreeNode> getSYRNodes(dto_TreeNode fathernode)
         {
             List<tb_staff> list_ORG = DB_Connecting.tb_staff.ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 node.id = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString(); ;
                 node.nameText = list_ORG[i].name;
                 node.url = "";
                 node.orderID = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
                 node.fatherID = fathernode.id;
                 list.Add(node);
             }

             list.Add(fathernode);
             return list;
         }
         public List<dto_TreeNode> getSZBMNodes(dto_TreeNode fathernode)
         {
             List<tb_department> list_ORG = DB_Connecting.tb_department.ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 int idYY = int.Parse(fathernode.id) + (int)list_ORG[i].ID_Department;
                 node.id = (idYY).ToString();
                 node.nameText = list_ORG[i].name_Department;
                 node.url = "";
                 idYY=int.Parse(fathernode.id) + (int)list_ORG[i].ID_Father_Department;
                 node.orderID = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
                 node.fatherID = (idYY).ToString();
                 list.Add(node);
             }

             list.Add(fathernode);
             return list;
         }


         public List<dto_TreeNode> getGYSNodes(dto_TreeNode fathernode)
         {
             List<tb_supplier> list_ORG = DB_Connecting.tb_supplier.ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 node.id = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString(); ;
                 node.nameText = list_ORG[i].name_supplier;
                 node.url = "";
                 node.orderID = (int.Parse(fathernode.id)+list_ORG[i].ID).ToString();
                 node.fatherID =fathernode.id;
                 list.Add(node);
             }

             list.Add(fathernode);
             return list;
         }

         public List<dto_TreeNode> getDictNodes(int id_dic, dto_TreeNode fathernode)
         {
             List<tb_dataDict_para> list_ORG = DB_Connecting.tb_dataDict_para.Where(a => a.ID_dataDict == id_dic).ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 node.id = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
                 node.nameText = list_ORG[i].name_para;
                 node.url = "";
                 node.orderID = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
                 node.fatherID = (int.Parse(fathernode.id) + list_ORG[i].fatherid).ToString();
                 list.Add(node);

             }

                
             //list.Clear();
             list.Add(fathernode);
             return list;
         }
            


         public int couterTest()
         {
             String sql = getSearchTreeSQL("");
             SQLRunner sqlRuner = new SQLRunner();
             //DataTable dt = sqlRuner.runSelectSQL_dto_AssetSumm(sql);
             return 5;
         }
         public String getSearchTreeSQL(String roleName)
         {
             String sql =
                 "select ID,name_para,ID_dataDict fatherid,0 url,0 orderID from  tb_dataDict_para  dic_PL where dic_PL.ID_dataDict in (select ID from tb_dataDict dic where dic.flag_search=1)"
                    + " union all "
                    +" select di_L.ID,di_L.name_dataDict name_para,0 fatherid,0 url,di_L.ID orderID from tb_dataDict di_L where di_L.flag_search=1 ";
                    //+ " union all "
                    //+ " select deTB.ID_Department ID,deTB.name_Department name_para,deTB.ID_Father_Department fatherid,0 url,deTB.ID orderID from tb_department deTB "
                    //+ " union all "
                    //+ " select stf.ID,stf.name name_para,13 fatherid,0 url,stf.ID orderID from tb_staff stf";

             return sql;
         }





         #region 创建数据
         public DataTable createTreeDT(List<dto_TreeNode> list)
         {
             DataTable dt = new DataTable();
             dt.Columns.Add("module_id");
             dt.Columns.Add("module_name");
             dt.Columns.Add("module_fatherid");
             dt.Columns.Add("module_url");
             dt.Columns.Add("module_order");
             dt.Rows.Add("0", "全部", "-1", "", "1");
             for (int i = 0; i < list.Count; i++)
             {
                 dt.Rows.Add(list[i].id,list[i].nameText, list[i].fatherID, "", list[i].orderID);
             }
             return dt;
         }
         #endregion  



         #region 根据DataTable生成EasyUI Tree Json树结构
        public StringBuilder result = new StringBuilder();
        public StringBuilder sb = new StringBuilder();
        /// <summary>  
        /// 根据DataTable生成EasyUI Tree Json树结构  
        /// </summary>  
        /// <param name="tabel">数据源</param>  
        /// <param name="idCol">ID列</param>  
        /// <param name="txtCol">Text列</param>  
        /// <param name="url">节点Url</param>  
        /// <param name="rela">关系字段</param>  
        /// <param name="pId">父ID</param>  
        private string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string url, string rela, object pId)
        {
            result.Append(sb.ToString());
            sb.Clear();
            if (tabel.Rows.Count > 0)
            {
                sb.Append("[");
                string filer = string.Format("{0}='{1}'", rela, pId);
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        sb.Append("{\"id\":\"" + row[idCol] + "\",\"text\":\"" + row[txtCol] + "\",\"attributes\":\"" + row[url] + "\",\"state\":\"open\"");
                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            sb.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, idCol, txtCol, url, rela, row[idCol]);
                            result.Append(sb.ToString());
                            sb.Clear();
                        }
                        result.Append(sb.ToString());
                        sb.Clear();
                        sb.Append("},");
                    }
                    sb = sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("]");
                result.Append(sb.ToString());
                sb.Clear();
            }
            return result.ToString();
        }
        #endregion 

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
                 tmp_dto_AT.fatherid = (int)item.fatherid;
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

                 tmp_dto.id = item.ID_Department.ToString();
                 tmp_dto.fatherID = item.ID_Father_Department.ToString();
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


        public String GetTreeJsonByTable_TreeSearch(DataTable tabel, string idCol, string txtCol, string url, string rela, object pId)
        {
            result_tree_SearchTree.Append(sb_tree_SearchTree.ToString());
            sb_tree_SearchTree.Clear();
            if (tabel.Rows.Count > 0)
            {
                sb_tree_SearchTree.Append("[");
                string filer = string.Format("{0}='{1}'", rela, pId);
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        sb_tree_SearchTree.Append("{\"id\":\"" + row[idCol] + "\",\"text\":\"" + row[txtCol] + "\",\"attributes\":\"" + row[url] + "\",\"state\":\"open\"");
                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            sb_tree_SearchTree.Append(",\"children\":");
                            GetTreeJsonByTable_TreeSearch(tabel, idCol, txtCol, url, rela, row[idCol]);
                            result_tree_SearchTree.Append(sb_tree_SearchTree.ToString());
                            sb_tree_SearchTree.Clear();
                        }
                        result_tree_SearchTree.Append(sb_tree_SearchTree.ToString());
                        sb_tree_SearchTree.Clear();
                        sb_tree_SearchTree.Append("},");
                    }
                    sb_tree_SearchTree = sb_tree_SearchTree.Remove(sb_tree_SearchTree.Length - 1, 1);
                }
                sb_tree_SearchTree.Append("]");
                result_tree_SearchTree.Append(sb_tree_SearchTree.ToString());
                sb_tree_SearchTree.Clear();
            }
            return result_tree_SearchTree.ToString();
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


        public String loadTreeGrid_AssetType()
        {
                //读取数据
            return "";
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