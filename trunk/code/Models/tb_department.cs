namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_department
    {
        public int ID { get; set; }

        public int? ID_Department { get; set; }

        public int? ID_Father_Department { get; set; }

        [StringLength(20)]
        public string name_Department { get; set; }

        public int? treeLevel { get; set; }

        public bool? effective_Flag { get; set; }

        public DateTime? create_TIME { get; set; }

        public DateTime? invalid_TIME { get; set; }

        [Column("operator")]
        [StringLength(20)]
        public string _operator { get; set; }

        [StringLength(100)]
        public string url { get; set; }

        [StringLength(20)]
        public string orderNum { get; set; }

        [StringLength(20)]
        public string Department_ID { get; set; }
    }
}
