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

        public int? measurement { get; set; }

        public int? method_Depreciation { get; set; }

        public int? period_Depreciation { get; set; }

        public double? Net_residual_rate { get; set; }

        [StringLength(100)]
        public string url { get; set; }

        [StringLength(20)]
        public string orderID { get; set; }
    }
}
