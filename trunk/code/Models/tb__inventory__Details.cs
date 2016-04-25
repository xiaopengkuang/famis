namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tb_ inventory_ Details")]
    public partial class tb__inventory__Details
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        [StringLength(10)]
        public string state { get; set; }

        public int? amountOfSys { get; set; }

        public int? amountOfInv { get; set; }

        public int? difference { get; set; }

        [StringLength(20)]
        public string serial_number_Asset { get; set; }
    }
}
