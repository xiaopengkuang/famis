namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_Borrow
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serialNum { get; set; }

        public DateTime? date_borrow { get; set; }

        public DateTime? date_return { get; set; }

        public int? state_list { get; set; }

        public int? department_borrow { get; set; }

        public int? userID_borrow { get; set; }

        [StringLength(200)]
        public string reason_borrow { get; set; }

        [StringLength(200)]
        public string note_borrow { get; set; }

        public DateTime? date_operated { get; set; }

        public int? userID_operated { get; set; }

        public int? userID_review { get; set; }

        [StringLength(200)]
        public string content_review { get; set; }

        public bool? flag { get; set; }
    }
}
