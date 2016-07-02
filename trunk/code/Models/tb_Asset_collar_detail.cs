namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_collar_detail
    {
        public int ID { get; set; }

        public int? ID_collar { get; set; }

        public int? ID_asset { get; set; }

        public bool? flag { get; set; }
    }
}
