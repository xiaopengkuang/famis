using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.ashx
{
    /// <summary>
    /// loadInfo 的摘要说明
    /// </summary>
    public class loadInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            string callback = context.Request["callback"];
            string openid = context.Request["openid"];
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            string response = string.Format("\"value1\":\"{"+openid+"}\",\"value2\":\"{1}\"");
            string call = callback + "({" + response + "})";
            context.Response.Write(call);
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