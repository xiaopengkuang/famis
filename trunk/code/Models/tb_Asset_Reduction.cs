namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_Reduction
    {
        public int ID { get; set; }

        [StringLength(30)]
        public string Serial_number { get; set; }

        public DateTime? date_reduction { get; set; }

        public int? method_reduction { get; set; }

        public int? userID_apply { get; set; }

        public int? userID_approver { get; set; }

        public bool? flag { get; set; }

        public int? userID_operate { get; set; }

        public DateTime? date_Operated { get; set; }

        public int? state_List { get; set; }

        [StringLength(200)]
        public string reason_reduce { get; set; }

        [StringLength(200)]
        public string note_reduce { get; set; }

        public int? userID_reviewer { get; set; }

        [StringLength(200)]
        public string conten_review { get; set; }
    }
}
