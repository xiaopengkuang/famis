using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FAMIS.DTO
{
    public class Json_asset_edit
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

        public int? supplierID { get; set; }
        public String supplierName { get; set; }

        public String address_supplier { get; set; }
        
        public String linkMan_supplier { get; set; }


        [Column(TypeName = "date")]
        public DateTime? Time_Purchase { get; set; }

        public int? YearService_month { get; set; }

        public int? Method_depreciation { get; set; }

        public int? Method_add { get; set; }

        public int? Net_residual_rate { get; set; }

        public double? depreciation_Month { get; set; }

        public double? depreciation_tatol { get; set; }


        public double? Net_value { get; set; }


        public double? Total_price { get; set; }

        /// <summary>
        /// 当前拥有者
        /// </summary>
        public int? Owener { get; set; }
        public String name_owner { get; set; }
        
        public List<Json_asset_cattr_ad> cattrs { get; set; }
        public String note { get; set; }
        public String code_oldSYS { get; set; }
    }
}