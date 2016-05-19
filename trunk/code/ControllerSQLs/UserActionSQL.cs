using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.ControllerSQLs
{
    public static class UserActionSQL
    {









        public static String getSQL_Select_User_CheckCount(String userName,String password)
        {

            String sql = "select count(*) total from tb_user where name_User='"+userName+"' and password_User='"+password+"'";
            return sql;
        }

        public static String getSQL_Update_User_loginTime(String userName,String password)
        {
            String sql = "";
            return sql;

        }
    }
}