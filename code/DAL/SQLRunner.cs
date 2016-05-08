using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using FAMIS.DTO;


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
            try
            {
                count_do = cmd.ExecuteNonQuery();
            }catch(Exception e)
            {
                count_do = 0;
            }
             
            cn.Close();
            return count_do;
        }


        public int runSelectSQLCounter(String sql)
        {
            int result = 0;
            SqlCommand cmd = new SqlCommand(sql, cn);
            cn.Open();
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            cn.Close();
            return result;
        }
        public DataTable runSelectSQL_dto_AssetSumm(String sql)
        {
            SqlCommand cmd = new SqlCommand(sql, cn);
            DataTable dt = new DataTable();
            cn.Open();
            try
            {

               
                SqlDataAdapter adp = new SqlDataAdapter(cmd); 
                adp.Fill(dt); 

               SqlDataReader sqldReader=cmd.ExecuteReader();

              
               if (!sqldReader.IsClosed)
               {
                   sqldReader.Close();
               }
            }
            catch
            {
               
            }
            cn.Close();
            return dt;

        }


        public int runSelectSQL_dto_AssetSumm_Counter(String sql)
        {
            int result = 0;
            SqlCommand cmd = new SqlCommand(sql, cn);
            cn.Open();
            try
            {
                SqlDataReader sqldReader = cmd.ExecuteReader();



                while (sqldReader.Read())
                {

                    result++;
                }
                if (!sqldReader.IsClosed)
                {
                    sqldReader.Close();
                }

            }
            catch
            {

            }
            cn.Close();
            return result;

        }

    }
}