namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_Repair
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serialNumber { get; set; }

        public DateTime? date_ToRepair { get; set; }

        public DateTime? date_ToReturn { get; set; }

        [StringLength(200)]
        public string reason_ToRepair { get; set; }

        public int? supplierID_Torepair { get; set; }

        public int? userID_applying { get; set; }

        public int? userID_authorize { get; set; }

        [StringLength(200)]
        public string note_repair { get; set; }

        public bool? flag { get; set; }

        public int? state_list { get; set; }

        public DateTime? date_review { get; set; }

        public int? userID_create { get; set; }

        public int? userID_review { get; set; }

        public DateTime? date_create { get; set; }

        public int? ID_Asset { get; set; }

        [StringLength(20)]
        public string Name_equipment { get; set; }

        public double? CostToRepair { get; set; }

        [StringLength(200)]
        public string content_Review { get; set; }

        [StringLength(200)]
        public string content_repairState { get; set; }
    }
}
