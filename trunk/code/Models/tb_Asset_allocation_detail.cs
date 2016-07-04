namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_allocation_detail
    {
        public int ID { get; set; }

        public int? ID_allocation { get; set; }

        public int? ID_asset  { get; set; }

        public bool flag { get; set; }
    }
}
