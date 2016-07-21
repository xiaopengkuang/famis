using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Asset_sub_equipment
    {
        public int ID { get; set;}
        public String serialNum { get; set; }
        public String name { get; set; }
        public String specification { get; set; }
        public String measurement { get; set; }
        public String supplier  { get; set; }
        public DateTime? date_add { get; set; }
    }
}