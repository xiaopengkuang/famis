using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_Asset
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        [StringLength(20)]
        public string name_Asset { get; set; }

        [StringLength(20)]
        public string type_Asset { get; set; }

        [StringLength(20)]
        public string specification { get; set; }

        [StringLength(20)]
        public string measurement { get; set; }

        public double? unit_price { get; set; }

        public int? amount { get; set; }

        public double? value { get; set; }

        [StringLength(20)]
        public string department_Using { get; set; }

        [StringLength(200)]
        public string address { get; set; }

        [StringLength(20)]
        public string people_using { get; set; }

        [StringLength(10)]
        public string state_asset { get; set; }

        [StringLength(20)]
        public string supplierID { get; set; }

        public DateTime? Time_Purchase { get; set; }
    }
}