namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        [StringLength(20)]
        public string name_Asset { get; set; }

        public int? type_Asset { get; set; }

        [StringLength(20)]
        public string specification { get; set; }

        public int? measurement { get; set; }

        public double? unit_price { get; set; }

        public int? amount { get; set; }

        public double? value { get; set; }

        public int? department_Using { get; set; }

        public int? addressCF { get; set; }

        [StringLength(20)]
        public string people_using { get; set; }

        public int? state_asset { get; set; }

        public int? supplierID { get; set; }

        public DateTime? Time_Purchase { get; set; }

        public int? YearService_month { get; set; }

        public int? Method_depreciation { get; set; }

        public int? Method_decrease { get; set; }

        public int? Method_add { get; set; }

        public int? Net_residual_rate { get; set; }

        public double? depreciation_Month { get; set; }

        public double? depreciation_tatol { get; set; }

        public double? Net_value { get; set; }

        public bool? flag { get; set; }

        public DateTime? Time_add { get; set; }

        public double? Total_price { get; set; }
    }
}
