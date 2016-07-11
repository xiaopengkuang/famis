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

        public int? ID_reduction { get; set; }

        public int? ID_Asset { get; set; }

        public bool? flag { get; set; }
    }
}
