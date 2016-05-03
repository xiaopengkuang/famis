using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_CFDD_Asset
    {
        public int id { get; set; }


        [StringLength(20)]
        public string nameText { get; set; }

        public int fatherid { get; set; }

        [StringLength(30)]
        public string url { get; set; }

        [StringLength(20)]
        public string orderID { get; set; }
    }
}