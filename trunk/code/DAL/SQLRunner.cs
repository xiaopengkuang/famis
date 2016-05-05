using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace FAMIS.DAL
{
    public class SQLRunner
    {
        SqlConnection cn = new SqlConnection(CommonConnecting.connectionstring);
        SqlDataAdapter da;
        DataSet ds = new DataSet();
       
        public int executesql(string sql)
        {
            int count_do = 0;
            SqlCommand cmd = new SqlCommand(sql, cn);
            cn.Open();
             count_do= cmd.ExecuteNonQuery();
            cn.Close();
            return count_do;
        }
    }
}