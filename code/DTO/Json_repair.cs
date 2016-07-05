using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.Models
{
    public class Json_repair
    {
        public int ID { get; set; }

        public string serialNumber { get; set; }

        public DateTime? date_ToRepair { get; set; }

        public DateTime? date_ToReturn { get; set; }

        public string reason_ToRepair { get; set; }

        public string supplier_Name { get; set; }
        public string supplier_address { get; set; }
        public string supplier_LinkUser { get; set; }

        public string userName_applying { get; set; }

        public string userName_authorize { get; set; }

        public string note_repair { get; set; }

        public bool? flag { get; set; }

        public String state_list { get; set; }

        public double? cost_ToRepair { get; set; }

        public DateTime? date_review { get; set; }

        public string userName_create { get; set; }

        public string userName_review { get; set; }

        public DateTime? date_create { get; set; }

    }
}