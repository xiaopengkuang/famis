using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_Asset_Detail
    {

        public int RowNo { get; set; }
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        [StringLength(20)]
        public string name_Asset { get; set; }

        [StringLength(8)]
        public string type_Asset { get; set; }

        [StringLength(20)]
        public string specification { get; set; }

        public String measurement { get; set; }

        public double? unit_price { get; set; }

        public int? amount { get; set; }

        public double? value { get; set; }

        [StringLength(20)]
        public string department_Using { get; set; }

        public String addressCF { get; set; }

        [StringLength(20)]
        public string people_using { get; set; }

        [StringLength(10)]
        public string state_asset { get; set; }

        [StringLength(20)]
        public string supplierID { get; set; }

        public DateTime? Time_Purchase { get; set; }

        public int? YearService_month { get; set; }

        public int? Method_depreciation { get; set; }

        public int? Net_residual_rate { get; set; }

        public double? depreciation_Month { get; set; }

        public double? depreciation_tatol { get; set; }

        public double? Net_value { get; set; }

        public String Method_add { get; set; }

    }
}