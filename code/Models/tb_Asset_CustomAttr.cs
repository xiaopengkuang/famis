namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_CustomAttr
    {
        public int ID { get; set; }

        public int? ID_Asset { get; set; }

        public int? ID_customAttr { get; set; }

        [StringLength(100)]
        public string value { get; set; }

        public bool? flag { get; set; }

        public int? ID_AssetType { get; set; }
    }
}
