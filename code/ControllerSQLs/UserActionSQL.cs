using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.ControllerSQLs
{
    public static class UserActionSQL
    {








        /// <summary>
        ///获取用户登录信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static String getSQL_Select_User_CheckCount(String userName,String password)
        {

            String sql = "select count(*) total from tb_user where name_User='"+userName+"' and password_User='"+password+"'";
            return sql;
        }

        /// <summary>
        /// 更新用户登录时间
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static String getSQL_Update_User_loginTime(String userName,String password)
        {
            String sql = "";
            return sql;

        }
    }
}