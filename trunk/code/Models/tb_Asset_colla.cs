namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_colla
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        public DateTime? date { get; set; }

        [StringLength(20)]
        public string person { get; set; }

        [StringLength(200)]
        public string addree_Storage { get; set; }

        [StringLength(10)]
        public string state { get; set; }

        [StringLength(20)]
        public string person_Operator { get; set; }

        public DateTime? date_Operated { get; set; }
    }
}
