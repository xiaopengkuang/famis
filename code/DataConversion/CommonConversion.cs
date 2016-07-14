using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.ControllerSQLs;
using FAMIS.DAL;
using FAMIS.DTO;
using System.Web.SessionState;
using FAMIS.Models;
namespace FAMIS.DataConversion
{
    public class CommonConversion
    {

        FAMISDBTBModels DB_C = new FAMISDBTBModels();



       


      
        public List<int?> StringToIntList(String idStr)
        {
            List<int?> results = new List<int?>();
            if (idStr == null || idStr == "")
            {
                return results;
            }
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
        /// <summary>
        /// 将list类型转换成String类型
        /// </summary>
        /// <param name="ids_list"></param>
        /// <returns></returns>
        public String IntListToString(List<int> ids_list)
        {
            String result="";
            for (int i = 0; i < ids_list.Count; i++)
            {
                if (i == 0) {
                    result = ids_list[i]+"";
                } else {
                    result += "_" + ids_list[i];
                }
            }
            return result;
        }


        public int getUniqueID()
        {
            string str = DateTime.Now.ToString("ddhhmmss");
            return int.Parse(str);
        }

        public String getUniqueIDString() 
        {
            return DateTime.Now.ToString("ddhhmmss");
        }


        public String getUniqueID_serialNum(String Type)
        {
            string str = Type + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            return str;
        }

        public String getUniqueID(String head)
        {
            string str = head + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            return str;
        }


        /// <summary>
        /// 默认url
        /// </summary>
        /// <returns></returns>
        public String getDefaultUrl()
        {
            return "javascript:void(0)";
        }



        public String getOperatorName()
        {
            HttpSessionState session = HttpContext.Current.Session;
            String userName = null;
            //先读取Session  判断Session是否存在
            if (session["userName"] != null)
            {
                userName = session["userName"].ToString();
            }

            return userName;
        }

        public int getDefaultFatherID()
        {
            return 0;
        }


        public int? getUSERID()
        {
            //TODO:

            HttpSessionState session = HttpContext.Current.Session;
            int? userID = null;
            //先读取Session  判断Session是否存在
            if (session["userID"] != null)
            {
                userID = int.Parse(session["userID"].ToString());
            }

            return userID;
        }

        /// <summary>
        /// 获取用户的角色ID
        /// </summary>
        /// <returns></returns>
        public int? getRoleID()
        {

            //TODO:
            
            HttpSessionState session = HttpContext.Current.Session;
            int? roleID=null;
            //先读取Session  判断Session是否存在
            if(session["userRole"]!=null)
            {
                 roleID = int.Parse(session["userRole"].ToString());
            }

            //校对数据库
            var data =from p in DB_C.tb_role
                      where p.flag==true
                      where p.ID==roleID
                      select p;
            if(data.Count()<1)
            {
                roleID=null;
            }

            return roleID;
        }


        /// <summary>
        /// 根据权限获取菜单列表ID
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<int?> getids_Menu_ByRole(int? roleID)
        {
            return getRightID_ByRole(roleID, SystemConfig.role_menu);
        }
        /// <summary>
        /// 根据角色获取资产类型权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<int?> getids_AssetTypeByRole(int? roleID)
        {
            return getRightID_ByRole(roleID,SystemConfig.role_assetType);
        }
        /// <summary>
        /// 根据角色ID获取部门权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<int?> getids_departmentByRole(int? roleID)
        {
            //List<int?> ids = getRightID_ByRole(roleID, SystemConfig.role_department);

            //var data =from p in DB_C.tb_department
            //          where p.effective_Flag==true
            //          where ids.Contains(p.ID)
            //          select p

            return getRightID_ByRole(roleID, SystemConfig.role_department);
        }


        public List<int?> getRightID_ByRole(int? roleID,String typeName)
        {
            List<int?> ids = new List<int?>();


            bool SuperUser = isSuperUser(roleID);
            //先判断其是否是超级管理员
            if (SuperUser)
            { 
                switch(typeName){
                    case SystemConfig.role_assetType: { ids = getfullRight_assetType(); }; break;
                    case SystemConfig.role_department: { ids = getfullRight_department(); }; break;
                    case SystemConfig.role_menu: { ids = getfullRight_menu(); }; break;
                }
                return ids;
            }
            else {
                var data = from p in DB_C.tb_role_authorization
                           where p.flag == true
                           where p.role_ID == roleID
                           where p.type == typeName
                           select p;
                foreach (var item in data)
                {
                    ids.Add(item.Right_ID);
                }


                //TODO:
                if (typeName == SystemConfig.role_department)
                {
                    var data_DP = from p in DB_C.tb_department
                                  where p.effective_Flag == true
                                  where ids.Contains(p.ID)
                                  select p;
                    List<int?> ids_DP = new List<int?>();
                    foreach (var it in data_DP)
                    {
                        ids_DP.Add(it.ID);
                    }
                    return ids_DP;
                }

                return ids;
            }
        }

        public List<int?> getfullRight_assetType()
        {
            List<int?> ids = new List<int?>();
            var data = from p in DB_C.tb_AssetType
                       where p.flag == true
                       select p;

            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            return ids;

        }
        public List<int?> getfullRight_department()
        {
            List<int?> ids = new List<int?>();
            var data = from p in DB_C.tb_department
                       where p.effective_Flag == true
                       select p;

            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            return ids;
        }
        public List<int?> getfullRight_menu()
        {
            List<int?> ids = new List<int?>();
            var data = from p in DB_C.tb_Menu
                       where p.isMenu==true
                       select p;

            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            return ids;
        }


        public bool isSuperUser(int? roleID)
        {
            bool flag = false;
            var ro = from p in DB_C.tb_role
                     where p.ID == roleID
                     where p.isSuperUser == true
                     select p;

            if (ro.Count() > 0)
            {
                flag = true;
            }
            return flag;
        }







        public bool isALL(String info)
        {
            return info.ToLower() == "all" ? true : false;
        }

        /// <summary>
        /// 获取资产状态ID
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int getStateIDByName(String name)
        {
            var data = from p in DB_C.tb_dataDict_para
                       where p.activeFlag == true
                       where p.name_para == name
                       join tb_DIC in DB_C.tb_dataDict on p.ID_dataDict equals tb_DIC.ID into tmp_DIC
                       from DIC in tmp_DIC.DefaultIfEmpty()
                       where DIC.name_flag==SystemConfig.nameFlag_2_ZCZT
                       select new { 
                       ID=p.ID
                       };
            if (data.Count() == 1)
            {
                foreach(var item in data)
                {
                    return item.ID;
                }
                return -1;
            }
            else { 
                return -1;
            }
        }

        /// <summary>
        /// 根据前台获取stateListID
        /// </summary>
        /// <returns></returns>
        public int getStateListID(int? jsonID)
        {
            //空字段默认是草稿类型
            jsonID = jsonID == null ? SystemConfig.state_List_CG_jsonID : jsonID;

            String stateName;
            switch(jsonID){
                case SystemConfig.state_List_CG_jsonID:{stateName=SystemConfig.state_List_CG;};break;
                case SystemConfig.state_List_DSH_jsonID:{stateName=SystemConfig.state_List_DSH;};break;
                case SystemConfig.state_List_YSH_jsonID:{stateName=SystemConfig.state_List_YSH;};break;
                case SystemConfig.state_List_TH_jsonID: { stateName = SystemConfig.state_List_TH; }; break;
                case SystemConfig.state_List_YGH_jsonID: { stateName = SystemConfig.state_List_YGH; }; break;
                default:{stateName=SystemConfig.state_List_CG;};break;
            }

            var data = from p in DB_C.tb_State_List
                       where p.Name == stateName
                       select p;

            int id = -1;
            foreach (var item in data)
            {
                id = item.id;
                break;
            }
            return id;
                     

        }
      


        /// <summary>
        /// 根据JsonID获取目标Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public String getTargetStateName(int? id)
        {
            String name = null;
            switch (id)
            {
                case SystemConfig.state_List_CG_jsonID: name = SystemConfig.state_List_CG; break;
                case SystemConfig.state_List_DSH_jsonID: name = SystemConfig.state_List_DSH; break;
                case SystemConfig.state_List_YSH_jsonID: name = SystemConfig.state_List_YSH; break;
                case SystemConfig.state_List_TH_jsonID: name = SystemConfig.state_List_TH; break;
                case SystemConfig.state_List_YGH_jsonID: name = SystemConfig.state_List_YGH; break;
                default: name = null; break;
            }
            return name;
        }
        public bool is_YSH(int? id_state_target)
        {
            if (id_state_target == null)
            {
                return false;
            }
            String nameState = getTargetStateName(id_state_target);
            if (nameState == SystemConfig.state_List_YSH)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 待审核模式
        /// </summary>
        /// <returns></returns>
        public bool is_DSH(int? id_state_target)
        {
            if (id_state_target == null)
            {
                return false;
            }
            String nameState = getTargetStateName(id_state_target);
            if (nameState == SystemConfig.state_List_DSH)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_state_target"></param>
        /// <returns></returns>
        public bool is_TH(int? id_state_target)
        {
            if (id_state_target == null)
            {
                return false;
            }
            String nameState = getTargetStateName(id_state_target);
            if (nameState == SystemConfig.state_List_TH)
            {
                return true;
            }
            return false;
        }


        public List<String> getSerialNumByID_Asset(List<int> ids)
        {
            List<String> serials = new List<String>();
            if (ids == null || ids.Count < 1)
            {
                return serials;
            }
            var data = from p in DB_C.tb_Asset
                       where ids.Contains(p.ID)
                       select new
                       {
                           serialNum=p.serial_number
                       };
            foreach (var item in data)
            {
                serials.Add(item.serialNum);
            }

            return serials;
        }

        /// <summary>
        /// 根据
        /// </summary>
        /// <param name="serialNum"></param>
        /// <returns></returns>
      


     



        /// <summary>
        /// 根据JsonID 获得名称Name
        /// </summary>
        /// <param name="id_ast"></param>
        /// <returns></returns>
        public String getAssetStateNameByJsonID(int? id_ast)
        {
            String result = null;
            switch (id_ast) {
                case SystemConfig.state_asset_bad_ID: result = SystemConfig.state_asset_bad; break;
                case SystemConfig.state_asset_free_ID: result = SystemConfig.state_asset_free; break;
                case SystemConfig.state_asset_loan_ID: result = SystemConfig.state_asset_loan; break;
                case SystemConfig.state_asset_using_ID: result = SystemConfig.state_asset_using; break;
                case SystemConfig.state_asset_fix_ID: result = SystemConfig.state_asset_fix; break;
                default: ; break;
            }
            return result;
        }


        public List<String> getAssetStateNameListByJsonID(List<int?> id_ast)
        {
            List<String> list = new List<String>();

            foreach (int id_st in id_ast)
            {
                String nameState = getAssetStateNameByJsonID(id_st);
                if (nameState != null)
                {
                    list.Add(nameState);
                }
            }

            return list;
        }
        



    }
}