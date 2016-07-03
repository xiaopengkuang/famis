﻿using System;
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



       



        public List<int> StringToIntList(String idStr)
        {
            List<int> results = new List<int>();
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

        public int getUnqiID()
        {
            string str = DateTime.Now.ToString("ddhhmmss");
            return int.Parse(str);
        }

        public String getUnqiIDString() 
        {
            return DateTime.Now.ToString("ddhhmmss");
        }

        public String getDefaultUrl()
        {
            return "javascript:void(0)";
        }

        public String getOperatorName()
        {
            return "KXP";
        }

        public int getDefaultFatherID()
        {
            return 0;
        }


        public int? getUSERID()
        {
            //return 1;
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


        public int? getRoleID()
        {

            //return 1;
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


                if (typeName == SystemConfig.role_department)
                {
                    var data_DP = from p in DB_C.tb_department
                                  where p.effective_Flag == true
                                  where ids.Contains(p.ID)
                                  select p;
                    List<int?> ids_DP = new List<int?>();
                    foreach (var it in data_DP)
                    {
                        ids_DP.Add(it.ID_Department);
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
                ids.Add(item.ID_Department);
            }
            return ids;
        }
        public List<int?> getfullRight_menu()
        {
            List<int?> ids = new List<int?>();
            var data = from p in DB_C.tb_Menu
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
                case SystemConfig.state_List_TH_jsonID:{stateName=SystemConfig.state_List_TH;};break;
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
        public bool isOkToReview(int? id_stateTarget, int? id_collar)
        {
            if (id_collar == null || id_stateTarget == null || !SystemConfig.state_List.Contains((int)id_stateTarget))
            {
                return false;
            }
            //获取当前状态
            var data = from p in DB_C.tb_Asset_collar
                       where p.flag == true
                       where p.ID == id_collar
                       join tb_SL in DB_C.tb_State_List on p.state_List equals tb_SL.id
                       select new dto_state_List
                       {
                           id = tb_SL.id,
                           Name = tb_SL.Name,
                       };
            int a;
            dto_state_List item = data.First();
            int b;

            if (item != null)
            {
                String stateName = item.Name;
                String stateName_target = getTargetStateName(id_stateTarget);
                bool fs = false;
                switch (stateName_target)
                {
                    case SystemConfig.state_List_CG:
                        {
                            if (SystemConfig.state_List_CG_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_DSH:
                        {
                            if (SystemConfig.state_List_DSH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_YSH:
                        {
                            if (SystemConfig.state_List_YSH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_TH:
                        {
                            if (SystemConfig.state_List_TH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    default: { fs = false; }; break;
                }
                return fs;

            }
            return false;
        }



        public String getTargetStateName(int? id)
        {
            String name = null;
            switch (id)
            {
                case SystemConfig.state_List_CG_jsonID: name = SystemConfig.state_List_CG; break;
                case SystemConfig.state_List_DSH_jsonID: name = SystemConfig.state_List_DSH; break;
                case SystemConfig.state_List_YSH_jsonID: name = SystemConfig.state_List_YSH; break;
                case SystemConfig.state_List_TH_jsonID: name = SystemConfig.state_List_TH; break;
                default: name = null; break;
            }
            return name;
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

        public int? getIDBySerialNum(String serialNum)
        {
            if (serialNum == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_collar
                       where p.serial_number == serialNum
                       select p;
            if (data.Count() != 1)
            {
                return null;
            }

            foreach (var item in data)
            {
                return item.ID;
            }

            return null;
        }



    }
}