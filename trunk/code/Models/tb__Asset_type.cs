namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tb_ Asset_type")]
    public partial class tb__Asset_type
    {
        public int ID { get; set; }

        [StringLength(8)]
        public string assetTypeCode { get; set; }

        [StringLength(20)]
        public string name_Asset_Type { get; set; }

        [StringLength(8)]
        public string father_MenuID_Type { get; set; }

        public int? treeLevel { get; set; }
    }
}
