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


        public List<int> StringToIntList(String idStr)
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


        public int? getRole()
        {

            return 1;
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
            return getRightID_ByRole(roleID, SystemConfig.role_department);
        }


        public List<int?> getRightID_ByRole(int? roleID,String typeName)
        {
            List<int?> ids = new List<int?>();


            var data = from p in DB_C.tb_role_authorization
                       where p.flag == true
                       where p.role_ID == roleID
                       where p.type == typeName
                       select p;

            foreach (var item in data)
            {
                ids.Add(item.Right_ID);
            }
            return ids;
        }

        public bool isALL(String info)
        {
            return info.ToLower() == "all" ? true : false;
        }

    }
}