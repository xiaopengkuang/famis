using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;

namespace FAMIS.ViewCommon
{
    public class TreeViewCommon
    {

        public string GetModule(String  role)
        {
            DataTable dt = createDT();

            string json = GetTreeJsonByTable(dt, "module_id", "module_name", "module_url", "module_fatherid", "0");
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
        StringBuilder result = new StringBuilder();
        StringBuilder sb = new StringBuilder();
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
        protected static DataTable createDT()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("module_id");
            dt.Columns.Add("module_name");
            dt.Columns.Add("module_fatherid");
            dt.Columns.Add("module_url");
            dt.Columns.Add("module_order");

            dt.Rows.Add("ALL", "全部", "0", "", "1");
            dt.Rows.Add("BM1", "使用部门", "ALL", "", "1");
            dt.Rows.Add("ZC1", "资产类型", "ALL", "", "1");
            dt.Rows.Add("DZ1", "存放地址", "ALL", "", "1");
            dt.Rows.Add("JSFS1", "减少方式", "ALL", "", "1");
            dt.Rows.Add("GYS1", "供应商", "ALL", "", "1");
            dt.Rows.Add("ZJFS1", "增加方式", "ALL", "", "1");

            //dt.Rows.Add("M0101", "深圳", "M01", "", "100");
            //dt.Rows.Add("M010101", "南山区", "M0101", "", "1000");
            //dt.Rows.Add("M010102", "罗湖区", "M0101", "", "1001");
            //dt.Rows.Add("M010103", "福田区", "M0101", "", "1002");
            //dt.Rows.Add("M010104", "宝安区", "M0101", "", "1003");
            //dt.Rows.Add("M010105", "龙岗区", "M0101", "", "1004");

            //dt.Rows.Add("M01010301", "上梅林", "M010103", "", "1002001");
            //dt.Rows.Add("M01010302", "下梅林", "M010103", "", "1002002");
            //dt.Rows.Add("M01010303", "车公庙", "M010103", "", "1002003");
            //dt.Rows.Add("M01010304", "竹子林", "M010103", "", "1002004");
            //dt.Rows.Add("M01010305", "八卦岭", "M010103", "", "1002005");
            //dt.Rows.Add("M01010306", "华强北", "M010103", "", "1002006");

            //dt.Rows.Add("M0102", "广州", "M01", "", "101");
            //dt.Rows.Add("M010201", "越秀区", "M0102", "", "1105");
            //dt.Rows.Add("M010202", "海珠区", "M0102", "", "1106");
            //dt.Rows.Add("M010203", "天河区", "M0102", "", "1107");
            //dt.Rows.Add("M010204", "白云区", "M0102", "", "1108");
            //dt.Rows.Add("M010205", "黄埔区", "M0102", "", "1109");
            //dt.Rows.Add("M010206", "荔湾区", "M0102", "", "1110");
            //dt.Rows.Add("M010207", "罗岗区", "M0102", "", "1111");
            //dt.Rows.Add("M010208", "南沙区", "M0102", "", "1112");
            return dt;
        }
        #endregion  
    }
}