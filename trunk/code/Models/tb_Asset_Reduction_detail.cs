namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_Reduction_detail
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        [StringLength(20)]
        public string serial_number_Asset { get; set; }
    }
}
