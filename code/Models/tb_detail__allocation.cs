namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tb_detail_ allocation")]
    public partial class tb_detail__allocation
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        [StringLength(20)]
        public string serial_number_Asset { get; set; }

        [StringLength(20)]
        public string department_allocation { get; set; }

        [StringLength(20)]
        public string user_allocation { get; set; }

        [StringLength(200)]
        public string address_allocation { get; set; }
    }
}
