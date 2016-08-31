using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Asset_History_Operate
    {
        public String OperateName { get; set; }
        public String OperateUser { get; set; }
        public String serialNum { get; set; }
        public String OperateTime_Type { get; set; }
        public DateTime? OperateTime { get; set; } 
    }
}