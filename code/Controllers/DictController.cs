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
using FAMIS.DataConversion;
using System.Threading;

namespace FAMIS.Controllers
{
    public class DictController : Controller
    {


        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        JSON_TO_MODEL convertHandler = new JSON_TO_MODEL();
        CommonConversion commonConversion = new CommonConversion();


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
        public ActionResult add_AssetType(int? pid,String pname,String level)
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
                ViewBag.level = level;
                //获取  获取新的资产编号
                //TODO: 
                //DateTime dt = DateTime.Now;
                String h = DateTime.Now.Hour.ToString().PadLeft(2, '0');      //获取当前时间的小时部分
                String m = DateTime.Now.Minute.ToString().PadLeft(2, '0');    //获取当前时间的分钟部分
                String s = DateTime.Now.Second.ToString().PadLeft(2, '0');    //获取当前时间的秒部分
                //23.ToString().PadLeft(6, '0');

                ViewBag.CodeAssetType = h + m + s;

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
            if (id==null)
            {
                ViewBag.info = name;
                return View("Error");
            }
            ViewBag.name = name;
            ViewBag.id = id;
            return View();
        }

        public ActionResult supplier()
        {
            return View();
        }

        public ActionResult add_departmentView(int? pid, String pname, String level)
        {
            if (pname == null || pname == "" || pid == null)
            {
                ViewBag.info = pname;
                return View("Error");
            }
            //return View("Error");

            if (pid != null && pname != "")
            {
                ViewBag.fatherID = pid;
                ViewBag.fatherName = pname;
                ViewBag.level = level;
                //获取  获取新的资产编号
                //TODO: 
                //DateTime dt = DateTime.Now;
                String h = DateTime.Now.Hour.ToString().PadLeft(2, '0');      //获取当前时间的小时部分
                String m = DateTime.Now.Minute.ToString().PadLeft(2, '0');    //获取当前时间的分钟部分
                String s = DateTime.Now.Second.ToString().PadLeft(2, '0');    //获取当前时间的秒部分
                //23.ToString().PadLeft(6, '0');

                ViewBag.CodeAssetType = h + m + s;

                return View("add_department");
            }
            else
            {
                ViewBag.info = pname + "\tss";
                return View("Error");
            }
        }




        public ActionResult edit_departmentView(int? id, String name)
        {
            if (id == null)
            {
                ViewBag.info = name;
                return View("Error");
            }
            ViewBag.name = name;
            ViewBag.id = id;
            return View("edit_department");
        }



        public ActionResult add_customAttrView(int? id,String name)
        {
            if (name==null||name==""||id == null || id <= 0)
            {
                ViewBag.info = "add_customAttrView";
                return View("Error");
            }
            ViewBag.id_assetType=id;

            return View("add_customAttr");
        }

        


        [HttpGet]
        public JsonResult load_GYS_add()
        {

            List<tb_supplier> list = DB_C.tb_supplier.OrderBy(a => a.ID).ToList();
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
                 list = DB_C.tb_Asset.Where(a => a.type_Asset == assetType).Distinct().ToList() ;
             }
             else {
                 list =  DB_C.tb_Asset.Distinct().ToList() ;
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

            List<tb_dataDict_para> list = DB_C.tb_dataDict_para.Where(a => a.ID_dataDict == TypeID).OrderBy(a => a.ID).ToList();
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
            List<tb_staff> list = DB_C.tb_staff.Where(a => a.code_Departmen == SZBM_ID).OrderBy(a => a.ID).ToList();
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
             List<tb_AssetType> list_de_AT = DB_C.tb_AssetType.ToList();
             if (!result_tree_department.Equals(""))
             {
                 Thread.Sleep(1000);
             }
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
             result_tree_department.Clear();
             sb_tree_department.Clear();
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


         [HttpPost]
         public String loadTree(String name)
         {

             if (name == null || name == "")
             {
                 return "";
             }

             String tree="";
             switch (name)
             {
                 case "departmentTree": tree=loadDepartment(); break;
                 case "assetType": tree = loadAssetType(); ; break;
                 case "2": ; break;
                 default: tree= "" ; break;
             }
             return tree;
         }

         [HttpPost]
         public JsonResult load_attrs_current(int? page, int? rows, int? assetTypeID)
         {
             page = page == null ? 1 : page;
             rows = rows == null ? 15 : rows;

             if (assetTypeID == null)
             {
                 return NULL_DATA();

             }
             //获取当前属性

             var data = (from a in DB_C.tb_customAttribute
                         where a.assetTypeID == assetTypeID
                         where a.flag==true
                         join b in DB_C.tb_customAttribute_Type on a.type equals b.ID into temp
                         from tt in temp.DefaultIfEmpty()
                         join c in DB_C.tb_dataDict on a.type_value equals c.ID into temp2
                         from tt2 in temp2.DefaultIfEmpty()
                         select new
                         {
                             id = a.ID,
                             xtID=a.SYSID,
                             sxbt = a.title,
                             zdcd=a.length,
                             sfbx=a.necessary,
                             sxlx=tt.name==null?"":tt.name,
                             glzdlx = tt2.name_dataDict == null ? "" : tt2.name_dataDict//这里主要第二个集合有可能为空。需要判断
                         }).OrderByDescending(a=>a.id);



             int count = data.Count();

             int skipindex = ((int)page - 1) * (int)rows;
             int rowsNeed = (int)rows;

              var json_data = new
             {
                 total = count,
                 rows =  data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()

             };
              return Json(json_data, JsonRequestBehavior.AllowGet);


         }


         [HttpPost]
         public JsonResult load_attrs_inhert(int? page, int? rows, int? assetTypeID)
         {
             page = page == null ? 1 : page;
             rows = rows == null ? 15 : rows;

             if (assetTypeID == null)
             {
                 return NULL_DATA();
             }
            //迭代获取数据
             //获取其所有父节点
             List<int?> ids=new List<int?>();
             var ids_data = GetParents_AsseType(assetTypeID);
             foreach (var q in ids_data)
             {
                 ids.Add(q.ID);
             }
             ids.Remove(assetTypeID);

             //int selectID_base = (int)assetTypeID;
             //根据父节点获取相应的属性
             var data = (from a in DB_C.tb_customAttribute
                         where ids.Contains(a.assetTypeID)
                         where a.flag == true
                         join b in DB_C.tb_customAttribute_Type on a.type equals b.ID into temp
                         from tt in temp.DefaultIfEmpty()
                         join c in DB_C.tb_dataDict on a.type_value equals c.ID into temp2
                         from tt2 in temp2.DefaultIfEmpty()
                         select new
                         {
                             id = a.ID,
                             xtID = a.SYSID,
                             sxbt = a.title,
                             zdcd = a.length,
                             sfbx = a.necessary,
                             sxlx = tt.name == null ? "" : tt.name,
                             glzdlx = tt2.name_dataDict == null ? "" : tt2.name_dataDict//这里主要第二个集合有可能为空。需要判断
                         }).OrderByDescending(a => a.id);

             int count = data.Count();

             int skipindex = ((int)page - 1) * (int)rows;
             int rowsNeed = (int)rows;

             var json_data = new
             {
                 total = count,
                 rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()

             };
             return Json(json_data, JsonRequestBehavior.AllowGet);

         }


         public IEnumerable<tb_AssetType> GetSonID_AsseType(int? p_id)
         {
             var query = from c in DB_C.tb_AssetType
                         where c.father_MenuID_Type == p_id
                         where c.flag==true
                         select c;

             return query.ToList().Concat(query.ToList().SelectMany(t => GetSonID_AsseType(t.ID)));
         }
         public IEnumerable<tb_AssetType> GetParents_AsseType(int? id)
         {
             var query = from c in DB_C.tb_AssetType
                         where c.ID == id
                         where c.flag == true
                         select c;

             return query.ToList().Concat(query.ToList().SelectMany(t => GetParents_AsseType(t.father_MenuID_Type)));
         }  
         public String loadDepartment()
         {
             List<tb_department> list_ORG = DB_C.tb_department.Where(a=>a.effective_Flag==true).ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 int idYY = Convert.ToInt32(list_ORG[i].ID_Department);
                 node.id = (idYY).ToString();
                 node.nameText = list_ORG[i].name_Department;
                 node.url = "javascript:void(0)";
                 node.orderID=list_ORG[i].ID.ToString();
                 node.fatherID = list_ORG[i].ID_Father_Department.ToString();
                 list.Add(node);
             }
             return TreeListToString(list);
             

         }

         public String loadAssetType()
         {
               var q = from p in DB_C.tb_AssetType
                     where p.flag == true
                     select new
                     {
                         id=p.ID,
                         nameText=p.name_Asset_Type,
                         url=p.url,
                         orderID=p.orderID,
                         fatherID=p.father_MenuID_Type
                     };
               List<dto_TreeNode> list = new List<dto_TreeNode>();
               foreach (var p in q)
               {
                   dto_TreeNode node = new dto_TreeNode();
                   node.id = p.id.ToString();
                   node.nameText = p.nameText;
                   node.orderID = p.orderID;
                   node.fatherID = p.fatherID.ToString();
                   node.url = p.url;
                   list.Add(node);
               }

               return TreeListToString(list);
         }

        



         


         public String TreeListToString(List<dto_TreeNode> list)
         {
             if (list.Count > 0)
             {
                 DataSet ds_tree = ConvertToDataSet(list);
                 DataTable dt_tree = new DataTable();
                 dt_tree = ds_tree.Tables[0];

                 if (result_tree_SearchTree.Equals(""))
                 {
                     Thread.Sleep(1000);
                 }
                 result_tree_SearchTree.Clear();
                 sb_tree_SearchTree.Clear();
                 string json = GetTreeJsonByTable_TreeSearch(dt_tree, "id", "nameText", "url", "fatherID", "0");
                 result_tree_SearchTree.Clear();
                 sb_tree_SearchTree.Clear();
                 return json;
             }
             else {
                 return "";
             }

            
         }

         public List<dto_TreeNode> getTreeSearchNodes(String role)
         {
             List<tb_dataDict> dic = DB_C.tb_dataDict.Where(a => a.flag_Search == true).ToList();
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
             List<tb_AssetType> list_ORG = DB_C.tb_AssetType.ToList();
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
             List<tb_staff> list_ORG = DB_C.tb_staff.ToList();
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
             List<tb_department> list_ORG = DB_C.tb_department.ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 int idYY = int.Parse(fathernode.id) + Convert.ToInt32(list_ORG[i].ID_Department);
                 node.id = (idYY).ToString();
                 node.nameText = list_ORG[i].name_Department;
                 node.url = "";
                 idYY = int.Parse(fathernode.id) + Convert.ToInt32(list_ORG[i].ID_Father_Department);
                 node.orderID = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
                 node.fatherID = (idYY).ToString();
                 list.Add(node);
             }

             list.Add(fathernode);
             return list;
         }


         public List<dto_TreeNode> getGYSNodes(dto_TreeNode fathernode)
         {
             List<tb_supplier> list_ORG = DB_C.tb_supplier.ToList();
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

        [HttpPost]
         public int Handler_addNewAssetType(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_AssetType_add json_data = serializer.Deserialize<Json_AssetType_add>(data);
            if (json_data != null)
            {
                try {
                    
                    tb_AssetType at = convertHandler.ConverJsonToTable(json_data);
                    //设置默认url和orderID
                    at.url = "javascript:void(0)";
                    at.orderID = at.assetTypeCode.ToString();
                    at.flag = true;
                    at.lastEditTime = DateTime.Now;

                    DB_C.tb_AssetType.Add(at);
                    DB_C.SaveChanges();
                    return 1;
                }catch(Exception e){
                    return 0;
                }
            }
            return 0;
        }

        [HttpPost]
        public int Handler_InsertDepartmen(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_department dp = serializer.Deserialize<Json_department>(data);
            if(dp!=null)
            {
                try {
                    tb_department tb_dp = new tb_department();
                    tb_dp =convertHandler.ConverJsonToTable(dp);
                    //获取用户信息以及插入时间
                    //获取操作用户
                    tb_dp._operator = "KXP";
                    //获取插入时间
                    tb_dp.create_TIME = DateTime.Now;
                    tb_dp.effective_Flag = true;
                    tb_dp.url = "javascript:void(0)";
                    tb_dp.orderNum = tb_dp.ID_Department.ToString();

                    DB_C.tb_department.Add(tb_dp);
                    DB_C.SaveChanges();
                    return 1;
                }
                catch(Exception e){
                }
            }
            return 0;

        }


        [HttpPost]
        public int Handler_InsertCAttr(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_customAttr json_data = serializer.Deserialize<Json_customAttr>(data);
            if (json_data != null && json_data.zclb != null)
            {
                //TODO：优化 先判断 是否存在该资产类型
                
                //
                try
                {
                    tb_customAttribute tb_CAttr = convertHandler.ConverJsonToTable(json_data);

                    //初始化相应的数据
                    tb_CAttr.time = DateTime.Now;
                    //TODO:
                    tb_CAttr.operatorName = "KXP";
                    DB_C.tb_customAttribute.Add(tb_CAttr);
                    DB_C.SaveChanges();
                    return 1;
                }
                catch (Exception e)
                {
                }
            }
            return 0;

        }

        [HttpPost]
        public int Handler_deleteCAttr(String ids)
        {
            if(ids==null)
            {
                return 0;
            }

            List<int> id_list = commonConversion.StringToIntList(ids);

            var target = from p in DB_C.tb_customAttribute
                         where p.flag == true
                         where id_list.Contains(p.ID) 
                          select p;
             if (target.Count()< 1)
            {
                return 0;
            }
            try{

                foreach(var q in target)
                {
                    q.flag = false;
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
            }

            return 0;

        }

        [HttpPost]
        public int Handler_UpdateDepartmen(String data, int id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_department json_data = serializer.Deserialize<Json_department>(data);

            var q=from p in DB_C.tb_department where p.ID_Department == id select p;
            if (q.Count() != 1)
            {
                return 0;
            }

            try
            {
                foreach (var p in q)
                {

                    //TODO
                    //p._operator="KXP";
                    p.name_Department = json_data.bmmc;
                    p.create_TIME = DateTime.Now;
                }
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        [HttpPost]
        public int Handler_updateAssetType(String data,int id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_AssetType_add json_data = serializer.Deserialize<Json_AssetType_add>(data);
            

            var q = from p in DB_C.tb_AssetType
                    where p.ID == id
                    select p;
            if (q.Count() != 1)
            {
                return 0;
            }
            try { 
                 foreach (var p in q)
                {
                    p.name_Asset_Type = json_data.lbmc;
                    p.measurement = json_data.jldw;
                    p.period_Depreciation = json_data.zjnx;
                    p.method_Depreciation = json_data.zjfs;
                    p.Net_residual_rate = json_data.jczl;
                    p.lastEditTime = DateTime.Now;
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                return 0;
            }
           
        }

        [HttpPost]
        public JsonResult Handler_getDepartment(int? bmbh)
        {
            if (bmbh == null)
            {
                return null;
            }

            var data = from p in DB_C.tb_department
                       join q in DB_C.tb_department on p.ID_Father_Department equals q.ID_Department
                       where p.ID_Department == bmbh
                       select new { 
                       bmbh=p.ID_Department,
                       bmmc=p.name_Department,
                       sjbm=p.ID_Father_Department,
                       sjbm_Name=q.name_Department
                       };
            if (data.ToList().Count < 1)
            {
                var data2 = from p in DB_C.tb_department
                           where p.ID_Department == bmbh
                           select new
                           {
                               bmbh = p.ID_Department,
                               bmmc = p.name_Department,
                               sjbm = p.ID_Father_Department,
                               sjbm_Name = ""
                           };
                return Json(data2.ToList().Take(1), JsonRequestBehavior.AllowGet);

            }
            else {
                return Json(data.ToList().Take(1), JsonRequestBehavior.AllowGet);
            }
           
        }


        [HttpPost]
        public JsonResult Handler_GetAssetType(int? id)
        {
            if(id==null)
            {
                return null;
            }
            //
            var data = from a in DB_C.tb_AssetType
                       join b in DB_C.tb_AssetType on a.father_MenuID_Type equals b.ID
                       //join c in DB_C.tb_dataDict_para on a.measurement equals c.ID
                       //join d in DB_C.tb_dataDict_para on a.method_Depreciation equals d.ID
                       where a.ID==id
                       select new {
                           id=a.ID,
                           lbbh=a.assetTypeCode,
                           lbmc=a.name_Asset_Type,
                           sjlb=b.name_Asset_Type,
                           zjfs = a.method_Depreciation,
                           jldw = a.measurement,
                           zjnx=a.period_Depreciation,
                           jczl=a.Net_residual_rate
                       };
            
            if (data.ToList().Count<1)
            {
               var data2 = from a in DB_C.tb_AssetType
                       //join c in DB_C.tb_dataDict_para on a.measurement equals c.ID
                           //join d in DB_C.tb_dataDict_para on a.method_Depreciation equals d.ID
                       where a.ID == id
                       select new
                       {
                           id = a.ID,
                           lbbh = a.assetTypeCode,
                           lbmc = a.name_Asset_Type,
                           sjlb = a.father_MenuID_Type,
                           zjfs = a.method_Depreciation,
                           jldw = a.measurement,
                           zjnx = a.period_Depreciation,
                           jczl = a.Net_residual_rate
                       };
               return Json(data2.ToList().Take(1), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(data.ToList().Take(1), JsonRequestBehavior.AllowGet);
            }

         //return  Json(data.ToList().Take(1), JsonRequestBehavior.AllowGet);

        }

        public List<dto_TreeNode> getDictNodes(int id_dic, dto_TreeNode fathernode)
         {
             List<tb_dataDict_para> list_ORG = DB_C.tb_dataDict_para.Where(a => a.ID_dataDict == id_dic).ToList();
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

             List<tb_dataDict_para> list_ad_AT = DB_C.tb_dataDict_para.Where(a => a.ID_dataDict == id_di).ToList();
             if (!result_tree_Address.Equals(""))
             {
                 Thread.Sleep(1000);
             }
             result_tree_Address.Clear();
             sb_tree_Address.Clear();
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

             result_tree_Address.Clear();
             sb_tree_Address.Clear();
             return json;
         }






       
         public String GenerateTree_Department()
         {
             List<tb_department> list_de = DB_C.tb_department.ToList();
             if (!result_tree_department.Equals(""))
             {
                 Thread.Sleep(1000);
             }

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
             result_tree_department.Clear();
             sb_tree_department.Clear();
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


        public JsonResult loadTreeGrid_AssetType()
        {
                //读取数据
            List<tb_AssetType> list = DB_C.tb_AssetType.Where(a => a.flag == true).ToList();
            
            MODEL_TO_JSON co=new MODEL_TO_JSON();
            List<Json_AssetType_add> data = co.ConverMdoelToJsonList(list);
            var json_NULL = new
                {
                    total = data.Count,
                    rows = data.ToArray()
                    //sql = selectSQL

                };

            return Json(json_NULL, JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadTreeGrid_Department()
        {
            var data = from p in DB_C.tb_department where p.effective_Flag == true select new {
            id=p.ID_Department,
            _parentId=p.ID_Father_Department,
            name=p.name_Department,
            time=p.create_TIME,
            operatorName=p._operator
            };

            var json_NULL = new
            {
                total = data.ToList().Count,
                rows = data.ToArray()
                //sql = selectSQL

            };
            return Json(json_NULL, JsonRequestBehavior.AllowGet);



        }


        [HttpPost]
        public JsonResult loadTreeGrid(String name)
        {
            if (name == null || name == "")
            {
                return null;
            }
            JsonResult jsR = new JsonResult();
            switch(name){
                case "assetType": jsR = loadTreeGrid_AssetType(); break;
                case "department": jsR = loadTreeGrid_Department(); break;
                default: jsR= null; break;
            }
            return jsR;
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


        /// <summary>
        /// 加载自定义属性的类别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public String load_CAttrType()
        {

            var data = from q in DB_C.tb_customAttribute_Type
                       select new { 
                           id=q.ID,
                           text = q.name,
                           description=q.description
                       };
            JavaScriptSerializer jss = new JavaScriptSerializer();

            String json = jss.Serialize(data).ToString().Replace("\\", "");
            return json;
        }






        public JsonResult NULL_DATA()
        {
            List<dto_null> list_null = new List<dto_null>();
            var json_NULL = new
            {
                total = 0,
                rows = list_null.ToArray()

            };
            return Json(json_NULL, JsonRequestBehavior.AllowGet);
        }
    }
}