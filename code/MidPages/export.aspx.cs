using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FAMIS.MidPages
{
    public partial class export : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "utf-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AppendHeader("content-disposition", "attachment;filename=\"" + HttpUtility.HtmlEncode(Request["txtName"] ?? DateTime.Now.ToString("yyyyMMdd")) + ".xls\"");
            Response.ContentType = "Application/ms-excel";
            Response.Write("<html>\n<head>\n");
            Response.Write("<style type=\"text/css\">\n.pb{font-size:13px;border-collapse:collapse;} " +
                           "\n.pb th{font-weight:bold;text-align:center;border:0.5pt solid windowtext;padding:2px;} " +
                           "\n.pb td{border:0.5pt solid windowtext;padding:2px;}\n</style>\n</head>\n");
            Response.Write("<body>\n" + Request["txtContent"] + "\n</body>\n</html>");
            Response.Flush();
            Response.End();
        }  
    }
}
