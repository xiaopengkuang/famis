namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_AssetType
    {
        public int ID { get; set; }

        public int? assetTypeCode { get; set; }

        [StringLength(20)]
        public string name_Asset_Type { get; set; }

        public int? father_MenuID_Type { get; set; }

        public int? treeLevel { get; set; }

        public int? measurement { get; set; }

        public int? method_Depreciation { get; set; }

        public int? period_Depreciation { get; set; }

        public double? Net_residual_rate { get; set; }

        [StringLength(100)]
        public string url { get; set; }

        [StringLength(20)]
        public string orderID { get; set; }
        public int RoleID { get; set; }
       
    }
}
