namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_ReviewReminding
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Type_Review_TB { get; set; }

        public int? ID_review_TB { get; set; }

        public bool? flag { get; set; }

        public int? ID_reviewer { get; set; }

        public DateTime? time_add { get; set; }

        public DateTime? time_review { get; set; }
    }
}
