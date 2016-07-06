using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace FAMIS.DTO
{
    public class Json_repair_add
    {

        public int? id { get; set; }
        public DateTime? dateToP { get; set; }

        public DateTime? dateToR { get; set; }

        [StringLength(200)]
        public string reason_add { get; set; }

        public int? supplier_repair { get; set; }

        public int? UAP_add { get; set; }

        public int? UAT_add { get; set; }

        [StringLength(200)]
        public string note_add { get; set; }

        public int? state_list { get; set; }

        public int? id_asset_repair { get; set; }

        public String serialNUM_Asset { get; set; }
        public String name_Asset { get; set; }
        public String name_Equipment { get; set; }
        public float? Cost_Repair { get; set; } 
    }
}