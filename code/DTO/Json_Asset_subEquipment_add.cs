using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Asset_subEquipment_add
    {
        public int? id_asset { get; set; }
        public String serialNum { get; set; }
        public String name { get; set; }
        public String specification { get; set; }
        public int? measurement { get; set; }
        public int? supplier { get; set; }
        public Double? value { get; set; }
        public String note { get; set; }
    }
}