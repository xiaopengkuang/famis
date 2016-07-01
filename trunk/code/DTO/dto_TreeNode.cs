using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_TreeNode
    {
        public int id { get; set; }

        [StringLength(20)]
        public string nameText { get; set; }

        public int fatherID { get; set; }

        [StringLength(100)]
        public string url { get; set; }

        public int orderID { get; set; }
    }
}