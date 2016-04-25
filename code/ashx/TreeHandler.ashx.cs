using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAMIS.ViewCommon;

namespace FAMIS.ashx
{
    /// <summary>
    /// TreeHandler 的摘要说明
    /// </summary>
    public class TreeHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
            string action = context.Request["action"];
            if (action != "" && action != null)
            {
                if (action.Equals("loadTree"))
                {
                    string role = context.Request["role"];
                   String resultTTResponse= loadTree(role);
                   context.Response.Write(resultTTResponse);
                   context.Response.End();  
                }
            }
        }



        public String  loadTree(String role)
        {
            TreeViewCommon treeviewCommon=new TreeViewCommon();
            String jsonStr = treeviewCommon.GetModule(role);
            return jsonStr;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}