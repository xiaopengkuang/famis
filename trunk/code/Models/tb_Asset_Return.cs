namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_Return
    {
        public int ID { get; set; }

        [StringLength(30)]
        public string serialNum { get; set; }

        public DateTime? date_return { get; set; }

        [StringLength(200)]
        public string reason_return { get; set; }

        [StringLength(200)]
        public string note_return { get; set; }

        public int? userID_operated { get; set; }

        public DateTime? date_operated { get; set; }

        public bool? flag { get; set; }

        public int? state_list { get; set; }

        public int? userID_review { get; set; }

        [StringLength(200)]
        public string content_review { get; set; }

        public DateTime? date_review { get; set; }
    }
}
