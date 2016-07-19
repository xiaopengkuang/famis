namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_sub_equiment
    {
        public int ID { get; set; }

        public int? ID_Asset { get; set; }

        [StringLength(20)]
        public string serialCode { get; set; }

        [StringLength(20)]
        public string name { get; set; }

        public int? measurement { get; set; }

        public double? value { get; set; }

        public int? supplyID { get; set; }

        [StringLength(20)]
        public string ggxh { get; set; }

        [StringLength(200)]
        public string note { get; set; }

        public DateTime? date_add { get; set; }

        public int? userID_add { get; set; }
    }
}
