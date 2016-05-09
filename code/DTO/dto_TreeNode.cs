using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_TreeNode
    {
        [StringLength(8)]
        public string id { get; set; }

        [StringLength(20)]
        public string nameText { get; set; }

        [StringLength(8)]
        public string fatherID { get; set; }

        [StringLength(100)]
        public string url { get; set; }

        [StringLength(20)]
        public string orderID { get; set; }
    }
}