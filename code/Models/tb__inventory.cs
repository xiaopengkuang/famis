namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tb_ inventory")]
    public partial class tb__inventory
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        public DateTime? date { get; set; }

        public int? amountOfSys { get; set; }

        public int? amountOfInv { get; set; }

        public int? difference { get; set; }

        [StringLength(20)]
        public string property { get; set; }

        [Column("operator")]
        [StringLength(20)]
        public string _operator { get; set; }

        [StringLength(10)]
        public string state { get; set; }

        public DateTime? date_Create { get; set; }

        [StringLength(200)]
        public string ps { get; set; }
    }
}
