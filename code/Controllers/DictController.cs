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
        CommonController commonController = new CommonController();


        //TODO:
        //多个定义是为了防止异步加载时 发生冲突(有必要吗？)
        StringBuilder result_tree_department = new StringBuilder();
        StringBuilder sb_tree_department = new StringBuilder();

        //FAMISDBTBModels db = new FAMISDBTBModels();
        
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

        [HttpPost]
        public JsonResult load_supplier(int? page, int? rows)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;


            var data = (from p in DB_C.tb_supplier
                        where p.flag == true
                        select new
                        {
                            id = p.ID,
                            name = p.name_supplier,
                            addree = p.address,
                            lineMan = p.linkman,
                            editTime=p.editTime

                        }).OrderByDescending(a=>a.editTime);

            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;

            var json_data = new
            {
                total = data.Count(),
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            return Json(json_data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult add_supplierView()
        {
            return View("add_supplier");
        }

        public ActionResult edit_supplierView(int? id)
        {
            if (id == null)
            {
                ViewBag.info = "edit_supplierView";
                return View("Error");
            }

            ViewBag.id = id;
            return View("edit_supplier");
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
                ViewBag.CodeAssetType =commonConversion.getUniqueID(SystemConfig.serialType_BM);
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
            if (name==null||id == null || id <= 0)
            {
                ViewBag.info = "add_customAttrView";
                return View("Error");
            }
            ViewBag.id_assetType=id;

            return View("add_customAttr");
        }

        public ActionResult add_dataDictParaView(int? id_Dict, String name_Dict,int? pid)
        {
            if (name_Dict == null || id_Dict == null || id_Dict <= 0)
            {
                ViewBag.info = "add_dataDictParaView";
                return View("Error");
            }

            ViewBag.id_Dict = id_Dict;
            ViewBag.name_Dict = name_Dict;
            ViewBag.pid = pid==null?0:pid;

            return View("add_dataDictPara");
        }

        public ActionResult edit_dataDictParaView(int? id_Dict, String name_Dict, int? id)
        {
            if (name_Dict == null || id_Dict == null || id_Dict <= 0||id==null)
            {
                ViewBag.info = "edit_dataDictParaView";
                return View("Error");
            }

            ViewBag.id_Dict = id_Dict;
            ViewBag.name_Dict = name_Dict;
            ViewBag.id = id;

            return View("edit_dataDictPara");
        }


        //
        public ActionResult add_dataDictView(int? pid, String pname,String level)
        {
            if (pname == null || pid == null || pid <= 0)
            {
                ViewBag.info = "add_dataDictView";
                return View("Error");
            }
            ////获取参数类别
            //var flag = from p in DB_C.tb_dataDict
            //           where p.active_flag == true
            //           where p.ID == pid
            //           select p;
            ViewBag.id_dataDict = pid;
            ViewBag.name_dataDict = pname;
            ViewBag.level=level==null?"2":level;
            return View("add_dataDict");
        }

        public ActionResult edit_dataDictView(int? id)
        {
            if (id == null || id <= 0)
            {
                ViewBag.info = "edit_dataDictView";
                return View("Error");
            }

            ViewBag.id_dataDict = id;
            return View("edit_dataDict");
        }



        [HttpPost]
        public JsonResult load_GYS_add()
        {

            var data = from p in DB_C.tb_supplier
                       where p.flag == true
                       select new dto_supplier { 
                            ID=p.ID,
                            name_supplier=p.name_supplier,
                            linkman=p.linkman,
                            address=p.address
                       };
            var json = new
            {
                total = data.Count(),
                rows= data.ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public String load_ZCXH_add(int? assetType)
        {
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where p.type_Asset == assetType
                       where p.specification!="" && p.specification!=null
                       select new { 
                       ZCXH=p.specification
                       };
            data = data.Distinct();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            String json = jss.Serialize(data.ToList()).ToString().Replace("\\", "");
            return json;
        }


       

        [HttpPost]
        /**
         * 加载增加方式
         * */
        public String load_FS_add(String nameFlag)
        {
            if (nameFlag == null || nameFlag == "")
            {
                return "";
            }

            var data = from p in DB_C.tb_dataDict_para
                       where p.activeFlag == true
                       join type in DB_C.tb_dataDict on p.ID_dataDict equals type.ID
                       where type.name_flag == nameFlag
                       select new dto_DataDict_para()
                       {
                           ID = p.ID,
                           name_para = p.name_para
                       };
            JavaScriptSerializer jss = new JavaScriptSerializer();
            String json = jss.Serialize(data).ToString().Replace("\\", "");
            return json;
        }

        [HttpPost]
        public JsonResult load_User_add(int? id_DP)
        {
            if (id_DP != null)
            {

                List<int?> ids_DP = commonController.GetSonIDs_Department(id_DP);
                var data = from p in DB_C.tb_user
                           where p.flag == true
                           where ids_DP.Contains(p.ID_DepartMent)
                           select new
                           {
                               id = p.ID,
                               name = p.true_Name
                           };
                return Json(data.ToList(), JsonRequestBehavior.AllowGet);


            }else{

                var data = from p in DB_C.tb_user
                       where p.flag == true
                       select new
                       { 
                       id=p.ID,
                       name=p.true_Name
                       };
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
            }

            
        }

     



        /// <summary>
        /// 根据资产类型返回数据的自定义属性
        /// 继承其父节点所有属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Handler_loadCAttr(int? id)
        {
            if (id == null)
            {
                return NULL_DATAList();
            }
            List<int?> ids = new List<int?>();
            ids = commonController.GetParentID_AsseType(id);

            //获取自定义属性

            var data =from p in DB_C.tb_customAttribute
                      where p.flag==true
                      where ids.Contains(p.assetTypeID)
                      join tb_ATT in DB_C.tb_customAttribute_Type on p.type equals tb_ATT.ID into temp_ATT
                      from ATT in temp_ATT.DefaultIfEmpty()
                      join tb_dic in DB_C.tb_dataDict on p.type_value equals tb_dic.ID into temp_dic
                      from dic in temp_dic.DefaultIfEmpty()
                      orderby p.assetTypeID 
                      select new Json_CAtrr{
                        ID=p.ID,
                        length=p.length,
                        necessary=p.necessary,
                        title=p.title,
                        type=p.type,
                        type_Name=ATT.name,
                        isTree=dic==null?false:dic.isTree,
                        type_value=p.type_value==null?-1:p.type_value
                      };
            if (data.Count()>0)
            {
                return Json(data.ToList(), JsonRequestBehavior.AllowGet);
            }
            return NULL_DATAList() ;
        }


         [HttpPost]
        public String load_SZBM()
        {
            return GenerateTree_Department();
        }

        [HttpPost]
         public String load_ZCLB()
         {
           return   GenerateTree_AssetType();
         }




         public String GenerateTree_AssetType()
         {

             if (!result_tree_department.Equals(""))
             {
                 Thread.Sleep(1000);
             }
             result_tree_department.Clear();
             sb_tree_department.Clear();

             int? roleID = commonConversion.getRoleID();
             List<int?> ids_at = commonConversion.getids_AssetTypeByRole(roleID);


             var data = from p in DB_C.tb_AssetType
                        where p.flag == true
                        where ids_at.Contains(p.ID)
                        select new dto_TreeNode
                        {
                            id=p.ID,
                            fatherID=(int)p.father_MenuID_Type,
                            nameText=p.name_Asset_Type,
                            url=p.url,
                            orderID=p.ID
                        };

             return TreeListToString(data.ToList());
         }
         [HttpPost]
         public String loadSearchTreeByRole(String treeType)
         {
             //获取用户权限
             int? roleID = commonConversion.getRoleID();
             if (roleID == null)
             {
                 return "{}";
             }
             List<dto_TreeNode> tree = new List<dto_TreeNode>();
             tree = getTreeSearchNodes(roleID,treeType);
             return TreeListToString(tree);
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
                 case "Dict":tree=loadTree_Dict() ; break;
                 default: tree= "" ; break;
             }
             return tree;
         }

        /// <summary>
        /// 加载没有外接表的
         ///  where p.tb_Ref==null
        /// </summary>
        /// <returns></returns>
         public String loadTree_Dict()
         {
             var data = from p in DB_C.tb_dataDict 
                        where p.active_flag==true 
                        where p.tb_Ref==null
                        select p;
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             foreach (var item in data)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 node.fatherID = (int)item.father_ID;
                 node.id = item.ID;
                 node.nameText = item.name_dataDict;
                 node.orderID =(int)(item.orderID==null?item.ID:item.orderID);
                 node.url = item.url==null?"javascript:void(0)":item.url;
                 list.Add(node);
             }
             //string result = TreeListToString(list);
             //int aaa = 0;

             //return result;
             return TreeListToString(list);
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
             ids = commonController.GetParentID_AsseType(assetTypeID);
             //var ids_data = GetParents_AsseType(assetTypeID);
             //foreach (var q in ids_data)
             //{
             //    ids.Add(q.ID);
             //}
             //ids.Remove(assetTypeID);

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



   

     
         public String loadDepartment()
         {
             List<tb_department> list_ORG = DB_C.tb_department.Where(a=>a.effective_Flag==true).ToList();
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             for (int i = 0; i < list_ORG.Count; i++)
             {
                 dto_TreeNode node = new dto_TreeNode();
                 int idYY = Convert.ToInt32(list_ORG[i].ID);
                 node.id = idYY;
                 node.nameText = list_ORG[i].name_Department;
                 node.url = "javascript:void(0)";
                 node.orderID=list_ORG[i].ID;
                 node.fatherID = (int)list_ORG[i].ID_Father_Department;
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
                   node.id = p.id;
                   node.nameText = p.nameText;
                   node.orderID = p.id;
                   node.fatherID = (int)p.fatherID;
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

         public List<dto_TreeNode> getTreeSearchNodes(int? roleID,String treeType)
         {
             var idList = from p in DB_C.tb_dataDict
                        where p.active_flag == true
                        where p.isSysSet == true
                        where p.flag_Search == true
                        select p;
             List<dto_TreeNode> nodesAll = new List<dto_TreeNode>();

             //    int? role = commonConversion.getRoleID();

             //    //获取资产类别权限
             //    List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
             //    //获取部门权限
             //    List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);

             foreach (var item in idList)
             {
                 if (treeType == SystemConfig.treeType_Accounting)
                 {
                     if (SystemConfig.treeType_Accounting_Menu.Contains(item.name_flag)){}
                     else { continue; }

                 }else if (treeType == SystemConfig.treeType_collarSearch){
                     if (SystemConfig.treeType_collarSearch_Menu.Contains(item.name_flag)){}
                     else {continue;}
                 }
                 else if (treeType == SystemConfig.treeType_allocationSearch)
                 {
                     if (SystemConfig.treeType_allocation_Search_Menu.Contains(item.name_flag)) { }
                     else { continue; }
                 }
                 else if (treeType == SystemConfig.treeType_repairSearch)
                 {
                     if (SystemConfig.treeType_repair_Search_Menu.Contains(item.name_flag)) { }
                     else { continue; }
                 }
                 else if (treeType == SystemConfig.treeType_depreciationSearch)
                 {
                     if (SystemConfig.treeType_depreciation_Search_Menu.Contains(item.name_flag))
                     { }
                     else { continue;}
                 }
                 else if (treeType == SystemConfig.treeType_newdeatails)
                 {
                     if (SystemConfig.treeType_newdeatails_Search_Menu.Contains(item.name_flag))
                     { }
                     else { continue; }
                 }
                 else if (treeType == SystemConfig.treeType_Reduction)
                 {
                     if (SystemConfig.treeType_reduction_Search_Menu.Contains(item.name_flag))
                     { }
                     else { continue; }
                 }
                 else
                 {
                     continue;
                 }
                 dto_TreeNode fathernode = new dto_TreeNode();
                 fathernode.id = (int)(item.ID * SystemConfig.ratio_dictPara);
                 fathernode.nameText = item.name_dataDict;
                 fathernode.url ="javascript:void(0)";
                 fathernode.orderID = item.ID;
                 fathernode.fatherID =0;
                
                 if (item.tb_Ref != null && item.tb_Ref != "")
                 {
                     List<dto_TreeNode> tmp = new List<dto_TreeNode>();
                     switch(item.tb_Ref){
                         case SystemConfig.treeTB_deparment: {
                             tmp = getSZBMNodes(fathernode, roleID);
                         }; break;
                         case SystemConfig.treeTB_AssetType:
                         {
                                 tmp = getZCLBNodes(fathernode, roleID);
                         }; break;
                         case SystemConfig.treeTB_supplier:{ 
                             tmp = getGYSNodes(fathernode);
                         }; break;
                         case SystemConfig.treeTB_user:
                             {
                                 tmp = getUserNodes(fathernode);
                             }; break;
                         default: ; break;
                     }
                     if (tmp.Count > 0){
                         nodesAll.AddRange(tmp);
                     }
                 }else {
                     List<dto_TreeNode> tmp = new List<dto_TreeNode>();
                     tmp = getDictNodes(item.ID, fathernode);
                     if (tmp.Count > 0){
                         nodesAll.AddRange(tmp);
                     }
                 }
                
             }
             return nodesAll;
         }




        /// <summary>
        /// 根据用户角色获取其资产类别
        /// </summary>
        /// <param name="fathernode"></param>
        /// <returns></returns>
         public List<dto_TreeNode> getZCLBNodes(dto_TreeNode fathernode,int? roleID)
         {
             //获取获取有权限的ids
             List<int?> ids=commonConversion.getids_AssetTypeByRole(roleID);
             var data = from p in DB_C.tb_AssetType
                        where p.flag == true
                        where ids.Contains(p.ID)
                        select new dto_TreeNode
                        {
                            fatherID =(int) (p.father_MenuID_Type+fathernode.id),
                            id = fathernode.id+ p.ID,
                            nameText = p.name_Asset_Type,
                            url = "javascript:void(0)",
                            orderID = p.ID
                        };
             List<dto_TreeNode> list = new List<dto_TreeNode>();


             if (data.Count() > 0)
             {
                 list = data.ToList();
                 list.Add(fathernode);
             }

             return list;

         }

       
         //public List<dto_TreeNode> getSYRNodes(dto_TreeNode fathernode)
         //{
         //    List<tb_staff> list_ORG = DB_C.tb_staff.ToList();
         //    List<dto_TreeNode> list = new List<dto_TreeNode>();
         //    for (int i = 0; i < list_ORG.Count; i++)
         //    {
         //        dto_TreeNode node = new dto_TreeNode();
         //        node.id = fathernode.id + list_ORG[i].ID ;
         //        node.nameText = list_ORG[i].name;
         //        node.url = "";
         //        node.orderID = fathernode.id + list_ORG[i].ID;
         //        node.fatherID = fathernode.id;
         //        list.Add(node);
         //    }

         //    list.Add(fathernode);
         //    return list;
         //}




        /// <summary>
        /// 根据用户权限获取部门节点
        /// </summary>
        /// <param name="fathernode"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
         public List<dto_TreeNode> getSZBMNodes(dto_TreeNode fathernode,int? roleID)
         {

             //获取部门权限ID_List
             List<int?> ids = commonConversion.getids_departmentByRole(roleID);


             var data = from p in DB_C.tb_department
                        where p.effective_Flag == true
                        where ids.Contains(p.ID)
                        select new dto_TreeNode {
                            id =(int)(fathernode.id+p.ID),
                            nameText=p.name_Department,
                            url = "javascript:void(0)",
                            fatherID=(int)(fathernode.id+p.ID_Father_Department),
                            orderID = (int)(fathernode.id + p.ID_Father_Department)
                        };
             List<dto_TreeNode> list = new List<dto_TreeNode>(); 
             if (data.Count() > 0)
             {
                 list = data.ToList();
                 list.Add(fathernode);
             }
             return list;
             
         }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="fathernode"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
         public List<dto_TreeNode> getGYSNodes(dto_TreeNode fathernode)
         {

             var data = from p in DB_C.tb_supplier
                        where p.flag == true
                        select new dto_TreeNode
                        {
                            id = fathernode.id+p.ID,
                            nameText = p.name_supplier,
                            url = "javascript:void(0)",
                            fatherID =fathernode.id,
                            orderID = fathernode.id + p.ID
                        };
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             if (data.Count() > 0)
             {
                 list = data.ToList();
                 list.Add(fathernode);
             }
             return list;
         }


         [HttpPost]
         public List<dto_TreeNode> getUserNodes(dto_TreeNode fathernode)
         {
             var data = from p in DB_C.tb_user
                        where p.flag == true
                        join tb_RO in DB_C.tb_role on p.roleID_User equals tb_RO.ID
                        where tb_RO.flag == true
                        select new dto_TreeNode
                        {
                            id=fathernode.id+p.ID,
                            nameText=p.true_Name,
                            fatherID=fathernode.id,
                            orderID=p.ID
                        };
             List<dto_TreeNode> list = new List<dto_TreeNode>();
             if (data.Count() > 0)
             {
                 list = data.ToList();
                 list.Add(fathernode);
             }
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
                    Console.WriteLine(e.Message);
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
                    tb_dp._operator = commonConversion.getOperatorName(); ;
                    //获取插入时间
                    tb_dp.create_TIME = DateTime.Now;
                    tb_dp.effective_Flag = true;
                    tb_dp.url = "javascript:void(0)";
                    tb_dp.orderNum = commonConversion.getUniqueID().ToString() ;

                    DB_C.tb_department.Add(tb_dp);
                    DB_C.SaveChanges();
                    return 1;
                }
                catch(Exception e){
                    Console.WriteLine(e.Message);
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
                    tb_CAttr.operatorName = commonConversion.getOperatorName();
                    DB_C.tb_customAttribute.Add(tb_CAttr);
                    DB_C.SaveChanges();
                    return 1;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return 0;

        }

        [HttpPost]
        public int Handler_InsertdataDic(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_dataDict json_data = serializer.Deserialize<Json_dataDict>(data);
            if (json_data != null && json_data.cslx != null)
            {
                try {
                    tb_dataDict tb_data = convertHandler.ConverJsonToTable(json_data); 
                   
                    //其他默认数据填充
                    tb_data.active_flag = true;
                    tb_data.flag_Search = false;
                    tb_data.isSysSet = false;
                    tb_data.orderID=commonConversion.getUniqueID();
                    tb_data.url = "javascript:void(0)";
                    DB_C.tb_dataDict.Add(tb_data);
                    DB_C.SaveChanges();
                    return 1;
                }catch(Exception e){
                    Console.WriteLine(e.Message);

                }
            }
            return 0;
        }

        [HttpPost]
        public int Handler_InsertDictPara(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_dataDict_Para json_data = serializer.Deserialize<Json_dataDict_Para>(data);
            if (json_data != null && json_data.cslx != null)
            {
                try
                {
                    tb_dataDict_para tb_data = convertHandler.ConverJsonToTable(json_data);
                    //其他默认数据填充
                    tb_data.activeFlag = true;
                    tb_data.create_Time = DateTime.Now;
                    tb_data.orderID = commonConversion.getUniqueIDString();
                    tb_data.url = commonConversion.getDefaultUrl();

                    DB_C.tb_dataDict_para.Add(tb_data);
                    DB_C.SaveChanges();
                    return 1;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return 0;
                }
            }
            return 0;
        }

        [HttpPost]
        public int Handler_InsertSupplier(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_supplier json_data = serializer.Deserialize<Json_supplier>(data);
            if (json_data != null && json_data.GYSMC != null && json_data.GYSMC != "")
            {
                try {
                    tb_supplier tb_data = convertHandler.ConverJsonToTable(json_data);
                    //定义默认参数
                    tb_data.operatorname = commonConversion.getOperatorName();
                    tb_data.flag = true;
                    tb_data.editTime = DateTime.Now;
                    DB_C.tb_supplier.Add(tb_data);
                    DB_C.SaveChanges();
                    return 1;

                }catch(Exception e){
                    Console.WriteLine(e.Message);
                    return 0;
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

            List<int?> id_list = commonConversion.StringToIntList(ids);

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
                Console.WriteLine(e.Message);
            }

            return 0;

        }


        [HttpPost]
        public int Handler_deletedataDict(int? id)
        {
            if (id == null)
            {
                return 0;
            }

            var target = from p in DB_C.tb_dataDict
                         where p.active_flag == true
                         where p.ID == id
                         where p.isSysSet==false
                         select p;
            if (target.Count() < 1)
            {
                return 0;
            }
            try {

                foreach (var q in target)
                {
                    q.active_flag = false;
                }
                DB_C.SaveChanges();
                return 1;

            }catch(Exception e){

                Console.WriteLine(e.Message);
            }
            return 0;

        }

        [HttpPost]
        public int Handler_deleteDictPara(String ids,bool? isTree)
        {
            if (ids == null)
            {
                return 0;
            }
            List<int?> ids_dict = commonConversion.StringToIntList(ids);

            isTree = isTree == null ? false : isTree;

            if (isTree == true)
            {
                if (ids_dict.Count != 1)
                {
                    return 0;
                }

                //找到其父节点
                for (int i = 0; i < ids_dict.Count; i++)
                {
                    ids_dict.AddRange(commonController.GetSonIDs_dataDict_Para(ids_dict[i]));
                }
            }

            var target = from p in DB_C.tb_dataDict_para
                         where p.activeFlag==true
                         where ids_dict.Contains(p.ID)
                         select p;
            if (target.Count() < 1)
            {
                return 0;
            }
            try
            {

                foreach (var q in target)
                {
                    q.activeFlag = false;
                }
                DB_C.SaveChanges();
                return 1;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            return 0;
        }


        [HttpPost]
        public int Handler_deleteAssetType(int? id)
        {
            if (id == null)
            {
                return 0;
            }

            List<int?> ids = commonController.GetSonID_AsseType(id);
            var data = from p in DB_C.tb_AssetType
                       where p.flag == true
                       where ids.Contains(p.ID)
                       select p;
            try {
                foreach (var item in data)
                {
                    item.flag = false;
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){

                Console.WriteLine(e.Message);
                return 0;
            }
        }

        [HttpPost]
        public int Handler_deleteGYS(String ids)
        {
            if (ids == null)
            {
                return 0;
            }
            List<int?> id_list = commonConversion.StringToIntList(ids);
            var data = from p in DB_C.tb_supplier
                       where p.flag == true
                       where id_list.Contains(p.ID)
                       select p;

            try { 
                foreach(var item in data)
                {
                    item.flag = false;
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                Console.WriteLine(e.Message);
                return 0;
            }
        }



        /// <summary>
        /// 默认是系统设置 防止恶意删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public int Check_IsSysSet(String table, int? id)
        {
            if(table == null || id == null)
            {
                return 1;     
            }
            int flagRe = 1;
            switch(table){
                case "dataDict": flagRe= check_IsSysSet_DataDict(id); break;
                default: flagRe= 1; break;
            }

            return flagRe;
        }



        public int check_IsSysSet_DataDict(int? id)
        {
            var data = from p in DB_C.tb_dataDict
                       where p.active_flag == true
                       where p.isSysSet == true
                       where p.ID==id
                       select p;
            return data.Count();
        }

        [HttpPost]
        public int Handler_UpdateDepartmen(String data, int id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_department json_data = serializer.Deserialize<Json_department>(data);

            var q=from p in DB_C.tb_department where p.ID == id select p;
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
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        [HttpPost]
        public JsonResult load_dict_combobox(int? id)
        {
            //
            var data = from p in DB_C.tb_dataDict_para
                       where p.activeFlag == true
                       join tb_DIC in DB_C.tb_dataDict on p.ID_dataDict equals tb_DIC.ID
                       where tb_DIC.isSysSet == false
                       where p.ID_dataDict==id
                       select new dto_DataDict_para
                       { 
                         ID=p.ID,
                         name_para=p.name_para
                       };
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public String load_dict_combotree(int? id)
        {
            //
            var data = from p in DB_C.tb_dataDict_para
                       where p.activeFlag == true
                       join tb_DIC in DB_C.tb_dataDict on p.ID_dataDict equals tb_DIC.ID
                       where tb_DIC.isSysSet == false
                       where p.ID_dataDict==id
                       select new dto_TreeNode
                       {
                          id=p.ID,
                          fatherID=(int)p.fatherid,
                           nameText=p.name_para,
                           orderID=p.ID,
                           url="javascript:void(0)"

                       };
            return  TreeListToString(data.ToList());
        }

        [HttpPost]
        public JsonResult load_dict_CAttr()
        {
            //
            var data = from p in DB_C.tb_dataDict
                       where p.active_flag == true
                       where p.isSysSet==false
                       where p.father_ID!=0
                       select new dto_DataDict_para
                       {
                           ID = p.ID,
                           name_para = p.name_dataDict
                       };
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public int Handler_updateAssetType(String data,int? id)
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
                Console.WriteLine(e.Message);
                return 0;
            }
           
        }

        [HttpPost]
        public int Handler_UpdatedataDic(String data,int? id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_dataDict json_data = serializer.Deserialize<Json_dataDict>(data);

            var q = from p in DB_C.tb_dataDict
                    where p.active_flag == true
                    where p.ID == id
                    select p;
            if (q.Count() != 1)
            {
                return 0;
            }

            try
            {
                foreach (var p in q)
                {
                    p.isTree = json_data.isTree;
                    p.name_dataDict = json_data.csmc;
                    if (json_data.cslx != null)
                    {
                        p.father_ID = json_data.cslx;
                    }
                }
                DB_C.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }


        [HttpPost]
        public int Handler_UpdateDictPara(String data, int? id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_dataDict_Para json_data = serializer.Deserialize<Json_dataDict_Para>(data);
            if (json_data == null||id==null)
            {
                return 0;
            }
            var q = from p in DB_C.tb_dataDict_para
                    where p.activeFlag == true
                    where p.ID == id
                    select p;
            if (q.Count() !=1)
            {
                return 1;
            }
            try{
                foreach (var item in q)
                {
                    item.name_para = json_data.csmc;
                    item.description = json_data.csms;
                    //item.ID_dataDict = json_data.cslx;  这个字段应该不用更新
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                Console.WriteLine(e.Message);
                return 0;
            }


        }

        [HttpPost]
        public int Handler_UpdateSupplier(String data,int? id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_supplier json_data = serializer.Deserialize<Json_supplier>(data);
            if (json_data == null || id == null)
            {
                return 0;
            }
            var q = from p in DB_C.tb_supplier
                    where p.flag == true
                    where p.ID == id
                    select p;
            if (q.Count() != 1)
            {
                return 0;
            }
            try {
                foreach (var item in q)
                {
                    item.address = json_data.DZ;
                    item.editTime = DateTime.Now;
                    item.email = json_data.YX;
                    item.fax = json_data.CZ;
                    item.linkman = json_data.LXR;
                    item.name_supplier = json_data.GYSMC;
                    item.operatorname = commonConversion.getOperatorName();
                    item.phoneNumber = json_data.LXDH;
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                Console.WriteLine(e.Message);
                return 0;
            }
            return 0;
        }



        [HttpPost]
        public JsonResult Handler_getDepartment(int? bmbh)
        {
            if (bmbh == null)
            {
                return NULL_DATAList();
            }

            var data = from p in DB_C.tb_department
                       join q in DB_C.tb_department on p.ID_Father_Department equals q.ID
                       where p.ID == bmbh
                       select new { 
                       bmbh=p.ID,
                       bmmc=p.name_Department,
                       sjbm=p.ID_Father_Department,
                       sjbm_Name=q.name_Department
                       };
            if (data.ToList().Count < 1)
            {
                var data2 = from p in DB_C.tb_department
                           where p.ID == bmbh
                           select new
                           {
                               bmbh = p.ID,
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

        public JsonResult Handler_getDataDict(int? id)
        {
            if (id == null)
            {
                return NULL_DATA();
            }

            var data = from a in DB_C.tb_dataDict
                       where a.active_flag == true
                       where a.ID == id
                       where a.isSysSet == false
                       join b in DB_C.tb_dataDict on a.father_ID equals b.ID into tmp_b
                       from jb in tmp_b.DefaultIfEmpty()
                       select new {
                           csmc = a.name_dataDict,
                           cslx = jb.ID,
                           cslx_name = jb.name_dataDict == null ? "" : jb.name_dataDict,
                           isTree = a.isTree
                       };
            return Json(data.ToList().Take(1), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Handler_getDictPara(int? id)
        {
            if (id == null)
            {
                return NULL_DATAList();
            }
            var data = from a in DB_C.tb_dataDict_para
                       join b in DB_C.tb_dataDict on a.ID_dataDict equals b.ID
                       where a.ID==id
                       where a.activeFlag==true
                       select new { 
                       cslx=a.ID_dataDict,
                       cslx_name=b.name_dataDict,
                       csmc=a.name_para,
                       csms=a.description,
                       id=a.ID
                       };
            if (data.Count() < 1)
            {
                return NULL_DATAList();
            }
            return Json(data.ToList().Take(1), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Handler_getSupplier(int? id)
        {
            if (id == null)
            {
                return NULL_DATAList();
            }
            var data = from p in DB_C.tb_supplier
                       where p.flag == true
                       where p.ID == id
                       select new {
                           GYSMC=p.name_supplier,
                           LXR=p.linkman,
                           LXDH=p.phoneNumber,
                           YX=p.email,
                           CZ=p.fax,
                           DZ=p.address,
                           ID=p.ID

                       };
            if (data.Count() < 1)
            {
                return NULL_DATAList(); ;
            }
            return Json(data.ToList().Take(1), JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult Handler_GetAssetType(int? id)
        {
            if(id==null)
            {
                return NULL_DATA();
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

             var data = from p in DB_C.tb_dataDict_para
                        where p.activeFlag == true
                        where p.ID_dataDict == id_dic
                        select new dto_TreeNode
                        {
                                id = fathernode.id + p.ID,
                                nameText =p.name_para,
                                url = "javascript:void(0)",
                                orderID = fathernode.id + p.ID,
                                fatherID = (int)(fathernode.id + p.fatherid)
                        };
           

             List<dto_TreeNode> list = new List<dto_TreeNode>();
             if (data.Count() > 0)
             {
                 list = data.ToList();
                 list.Add(fathernode);
             }
             return list;

             //List<tb_dataDict_para> list_ORG = DB_C.tb_dataDict_para.Where(a => a.ID_dataDict == id_dic).ToList();
             //List<dto_TreeNode> list = new List<dto_TreeNode>();
             //for (int i = 0; i < list_ORG.Count; i++)
             //{
             //    dto_TreeNode node = new dto_TreeNode();
             //    node.id = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
             //    node.nameText = list_ORG[i].name_para;
             //    node.url = "";
             //    node.orderID = (int.Parse(fathernode.id) + list_ORG[i].ID).ToString();
             //    node.fatherID = (int.Parse(fathernode.id) + list_ORG[i].fatherid).ToString();
             //    list.Add(node);

             //}
             //list.Clear();
             //list.Add(fathernode);
             //return list;
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

         [HttpPost]
         public String load_DictTree(String nameFlag)
         {

             if(nameFlag==null||nameFlag=="")
             {
                 return "";
             }
             var data = from p in DB_C.tb_dataDict_para
                        where p.activeFlag == true
                        join type in DB_C.tb_dataDict on p.ID_dataDict equals type.ID
                        where type.name_flag == nameFlag
                        select new dto_TreeNode
                        {
                            id=p.ID,
                            fatherID=(int)p.fatherid,
                            nameText=p.name_para,
                            orderID=p.ID,
                            url=p.url
                        };

             return TreeListToString(data.ToList());
         }






       
         public String GenerateTree_Department()
         {
             if (!result_tree_department.Equals(""))
             {
                 Thread.Sleep(1000);
             }
             //获取用户角色ＩＤ
             int? roleID = commonConversion.getRoleID();
             List<int?> ids_de = commonConversion.getids_departmentByRole(roleID);

             result_tree_department.Clear();
             sb_tree_department.Clear();
             var data = from p in DB_C.tb_department
                        where p.effective_Flag == true
                        where ids_de.Contains(p.ID)
                        select new dto_TreeNode { 
                        id=(int)p.ID,
                        fatherID=(int)p.ID_Father_Department,
                        nameText=p.name_Department,
                        orderID = (int)p.ID,
                        url=p.url
                        };

             return TreeListToString(data.ToList());
         }




         public JsonResult NULL_dataGrid()
         {
             var json = new
             {
                 total = 0,
                 rows = ""
             };
             return Json(json, JsonRequestBehavior.AllowGet);
         }

         public JsonResult NULL_TreeGrid()
         {
             var json = new
             {
                 total = 0,
                 rows = ""
             };
             return Json(json, JsonRequestBehavior.AllowGet);
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

            var data =from p in DB_C.tb_AssetType
                      where p.flag==true
                      join tb_dic in DB_C.tb_dataDict_para on p.measurement equals tb_dic.ID into temp_dic
                      from dic in temp_dic.DefaultIfEmpty()
                      join tb_dic in DB_C.tb_dataDict_para on p.method_Depreciation equals tb_dic.ID into temp_dic2
                      from dic2 in temp_dic2.DefaultIfEmpty()
                      select new 
                       {
                           id=p.ID,
                           lbmc = p.name_Asset_Type,
                           zjfs = dic2.name_para,
                           zjnx = p.period_Depreciation,
                           jczl = p.Net_residual_rate,
                           lastEditTime = p.lastEditTime,
                           jldw = dic.name_para,
                           _parentId=p.father_MenuID_Type,
                           level=p.treeLevel
                       };


            
            var json_NULL = new
                {
                    total = data.Count(),
                    rows = data.ToArray()
                };
            return Json(json_NULL, JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadTreeGrid_Department()
        {
            var data = from p in DB_C.tb_department where p.effective_Flag == true select new {
            id=p.ID,
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
        public JsonResult loadTreeGrid_dictPara(int id)
        {
            //判断其是否是外接表

            var data = from p in DB_C.tb_dataDict_para
                       where p.activeFlag == true
                       where p.ID_dataDict == id
                       select new { 
                       id=p.ID,
                       name=p.name_para,
                       _parentId=p.fatherid,
                       description = p.description
                       };

            var json = new
            {
                total = data.Count(),
                rows = data.ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult loadTreeGrid(String name,int? dictID)
        {
            if (name == null || name == "")
            {
                return NULL_TreeGrid();
            }

            JsonResult jsR = new JsonResult();
            
            switch(name){
                case "assetType": jsR = loadTreeGrid_AssetType(); break;
                case "department": jsR = loadTreeGrid_Department(); break;
                case "dictPara": {
                    if (dictID == null)
                    {
                        return jsR;
                    }
                    jsR = loadTreeGrid_dictPara((int)dictID);
                }
                    ; break;
                default: ; break;
            }
            return jsR;
        }

        [HttpPost]
        public JsonResult loadDataGrid(int? page, int? rows,String name, int? dictID)
        {
            if (name == null || name == "")
            {
                return NULL_dataGrid();
            }

            JsonResult jsR = new JsonResult();
            if (dictID == null)
            {
                return jsR;
            }
            switch (name)
            {
                //case "assetType": jsR = loadTreeGrid_AssetType(); break;
                //case "department": jsR = loadTreeGrid_Department(); break;
                case "dictPara": jsR = loadDataGrid_dictPara(page,rows,dictID); break;
                default: ; break;
            }
            return jsR;
        }


        public JsonResult loadDataGrid_dictPara(int? page, int? rows,int? dictID)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            if (dictID == null)
            {
                return NULL_DATA();
            }

            var data = (from p in DB_C.tb_dataDict_para
                       where p.activeFlag == true
                       where p.ID_dataDict == dictID
                       select new { 
                       id=p.ID,
                       name=p.name_para,
                       description = p.description
                       }).OrderByDescending(a=>a.id);

            int count = data.Count();

            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;

            var json_data = new
            {
                total = count,
                //rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                rows = data.ToList().ToArray()


            };
            return Json(json_data, JsonRequestBehavior.AllowGet);


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

        [HttpPost]
        public JsonResult isTreeType_DictData(int? id)
        {
            if (id == null)
            {
                return null;
            }

            //从DictTable中读书数据
            var data = from p in DB_C.tb_dataDict
                       where p.active_flag == true
                       where p.ID == id
                       select p;
            if (data.Count() != 1)
            {
                return null;
            }

            foreach (var item in data)
            {
                var result = new {
                    isTree = item.isTree == true ? true : false,
                    editAble = item.editAble == true ? true : false
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;

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


        public JsonResult NULL_DATAList()
        {

            JsonResult re = new JsonResult();
            return re;
        }

    }
}