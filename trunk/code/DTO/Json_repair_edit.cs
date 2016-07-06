using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace FAMIS.DTO
{
    public class Json_repair_edit
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serialNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_ToRepair { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_ToReturn { get; set; }

        [StringLength(200)]
        public string reason_ToRepair { get; set; }

        public int? supplierID_Torepair { get; set; }
        public String supplierName_Torepair { get; set; }

        public String Address_supplier { get; set; }
        public String linkMan_supplier { get; set; }

        public int? userID_applying { get; set; }

        public int? userID_authorize { get; set; }

        [StringLength(200)]
        public string note_repair { get; set; }


        public int? ID_Asset { get; set; }

        [StringLength(20)]
        public string Name_equipment { get; set; }

        public double? CostToRepair { get; set; }

        public String Name_asset { get; set; }

        public String serial_asset { get; set; }
    }
}