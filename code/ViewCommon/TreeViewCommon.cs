using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using FAMIS.DTO;
using FAMIS.DAL;
namespace FAMIS.ViewCommon
{
    public class TreeViewCommon
    {

        public string GetModule(String  role)
        {
            StreamWriter sw = new StreamWriter("D:\\ty111.txt");
           
            DataTable dt = createDT();
            string txt = "";
            for (int ii = 0; ii < dt.Rows.Count; ii++)
            {//对行循环
               
                for (int iii = 0; iii < dt.Columns.Count; iii++)
                {//对例循环

                    txt += dt.Rows[ii][iii].ToString();//某单元格的值

                }
                if (txt != null)
                {
                    sw.WriteLine(txt);
                    txt = "";
                }
            }
            sw.Close();
            string json = GetTreeJsonByTable(dt, "module_id", "module_name", "module_url", "module_fatherid", "0");
            return json;
        }

        public string GetAssetType(String role)
        {
            
           
            
            DataTable dt = createATDT();
            string txt = "";
            for (int ii = 0; ii < dt.Rows.Count; ii++)
            {//对行循环
               
                for (int iii = 0; iii < dt.Columns.Count; iii++)
                {//对例循环

                    txt += dt.Rows[ii][iii].ToString();//某单元格的值

                }
                
            }
             
           // orderID,name_Asset_Type,father_MenuID_Type,url,RoleID
            string json = GetTreeJsonByTable(dt, "ID", "name_Asset_Type", "url", "father_MenuID_Type", "0");
            return json;
        }

        public string GetDepartment(String role)
        {
          
            
           
            

            DataTable dt = createDPDT();
            string txt = "";
            for (int ii = 0; ii < dt.Rows.Count; ii++)
            {//对行循环

                for (int iii = 0; iii < dt.Columns.Count; iii++)
                {//对例循环

                    txt += dt.Rows[ii][iii].ToString();//某单元格的值

                }
                
            }
            
            // orderID,name_Asset_Type,father_MenuID_Type,url,RoleID
            string json = GetTreeJsonByTable(dt, "ID", "name_Department", "url", "ID_Father_Department", "0");
            return json;
        }
        public String getTreeJson(DataTable dt,string id,string name,string url ,string fatherid, string order)
        {
            result.Clear();
            sb.Clear();
            string json = GetTreeJsonByTable(dt, id,name,url, fatherid, order);
            return json;
        }



        
        /// <summary>
        /// 递归将DataTable转化为适合jquery easy ui 控件tree ,combotree 的 json
        /// 该方法最后还要 将结果稍微处理下,将最前面的,"children" 字符去掉.
        /// </summary>
        /// <param name="dt">要转化的表</param>
        /// <param name="pField">表中的父节点字段</param>
        /// <param name="pValue">表中顶层节点的值,没有 可以输入为0</param>
        /// <param name="kField">关键字字段名称</param>
        /// <param name="TextField">要显示的文本 对应的字段</param>
        /// <returns></returns>
        public static string TableToEasyUITreeJson(DataTable dt, string pField, string pValue, string kField, string TextField)
        {
            StringBuilder sb = new StringBuilder();
            string filter = String.Format(" {0}='{1}' ", pField, pValue);//获取顶级目录.
            DataRow[] drs = dt.Select(filter);
            if (drs.Length < 1)
                return "";
            sb.Append(",\"children\":[");
            foreach (DataRow dr in drs)
            {
                string pcv = dr[kField].ToString();
                sb.Append("{");
                sb.AppendFormat("\"id\":\"{0}\",", dr[kField].ToString());
                sb.AppendFormat("\"text\":\"{0}\"", dr[TextField].ToString());
                sb.Append(TableToEasyUITreeJson(dt, pField, pcv, kField, TextField).TrimEnd(','));
                sb.Append("},");
            }
            if (sb.ToString().EndsWith(","))
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            return sb.ToString();

        }

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
        #region 创建数据

        protected static DataTable createATDT() {


            DataTable dt = new DataTable();

          dt.Columns.Add("ID");
            dt.Columns.Add("name_Asset_Type");
            dt.Columns.Add("father_MenuID_Type");
            dt.Columns.Add("url");
            dt.Columns.Add("orderID");


            SqlConnection con = new SqlConnection(CommonConnecting.connectionstring);
            SqlDataAdapter sda = new SqlDataAdapter("select ID,name_Asset_Type,father_MenuID_Type,url,orderID from tb_AssetType  order by ID", con);
            DataTable dtt = new DataTable();
            sda.Fill(dtt);
            con.Close();
            string txt = "";
            for (int ii = 0; ii < dtt.Rows.Count; ii++)
            {//对行循环

                for (int iii = 0; iii < dtt.Columns.Count; iii++)
                {//对例循环
                    if (iii != dtt.Columns.Count - 1)
                    {
                        if (dtt.Rows[ii][iii].ToString()!=null)
                        txt += dtt.Rows[ii][iii].ToString() + ",";//某单元格的值
                    }
                    else
                        txt += dtt.Rows[ii][iii].ToString();
                }
                if (txt != null)
                {
                   
                    String[] temp=txt.Split(',');
                    string orderid = temp[0];
                    string name = temp[1];
                    string fid = temp[2];
                    string url = temp[3];
                    string rid = temp[4];

                    dt.Rows.Add(orderid, name, fid, "", rid);
                    txt = "";
                }
            }
            return dt;

           
        }
        protected static DataTable createDPDT()
        {


            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("name_Department");
            dt.Columns.Add("ID_Father_Department");
            dt.Columns.Add("url");
            dt.Columns.Add("Department_ID");

        
            SqlConnection con = new SqlConnection(CommonConnecting.connectionstring);
            SqlDataAdapter sda = new SqlDataAdapter("select ID,name_Department,ID_Father_Department,url,treeLevel from tb_department order by ID", con);
            DataTable dtt = new DataTable();
            sda.Fill(dtt);
            con.Close();
            string txt = "";
            for (int ii = 0; ii < dtt.Rows.Count; ii++)
            {//对行循环

                for (int iii = 0; iii < dtt.Columns.Count; iii++)
                {//对例循环
                    if (iii != dtt.Columns.Count - 1)
                    {
                        if (dtt.Rows[ii][iii].ToString() != null)
                            txt += dtt.Rows[ii][iii].ToString() + ",";//某单元格的值
                    }
                    else
                        txt += dtt.Rows[ii][iii].ToString();
                }
                if (txt != null)
                {

                    String[] temp = txt.Split(',');
                    string ID_Department = temp[0];
                    string name_Department = temp[1];
                    string fid = temp[2];
                    string url = temp[3];
                    string rid = temp[4];

                    dt.Rows.Add(ID_Department, name_Department, fid, "", rid);
                    txt = "";
                }
            }
            return dt;


        }
        protected static DataTable createDT()
        {
            DataTable dt = new DataTable();
            
            dt.Columns.Add("module_id");
            dt.Columns.Add("module_name");
            dt.Columns.Add("module_fatherid");
            dt.Columns.Add("module_url");
            dt.Columns.Add("module_order");


            SqlConnection con = new SqlConnection(CommonConnecting.connectionstring);
            SqlDataAdapter sda = new SqlDataAdapter("select ID,name_Menu,father_Menu_ID,url,ID from tb_Menu order by ID_Menu", con);
            DataTable dtt = new DataTable();
            sda.Fill(dtt);
            con.Close();
            string txt = "";
            for (int ii = 0; ii < dtt.Rows.Count; ii++)
            {//对行循环

                for (int iii = 0; iii < dtt.Columns.Count; iii++)
                {//对例循环
                    if (iii != dtt.Columns.Count - 1)
                    {
                        if (dtt.Rows[ii][iii].ToString() != null)
                            txt += dtt.Rows[ii][iii].ToString() + ",";//某单元格的值
                    }
                    else
                        txt += dtt.Rows[ii][iii].ToString();
                }
                if (txt != null)
                {

                    String[] temp = txt.Split(',');
                    string ID_Menu = temp[0];
                    string name_Menu = temp[1];
                    string fid = temp[2];
                    string url = temp[3];
                    string rid = temp[4];

                    dt.Rows.Add(ID_Menu, name_Menu, fid, "", rid);
                    txt = "";
                }
            }
            return dt;
        }
        #endregion  
    }
}