using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_Staff
    {


        [StringLength(8)]
        //StaffID
        public string id { get; set; }

        [StringLength(8)]
        public string code_Departmen { get; set; }
        [StringLength(20)]
        public string name { get; set; }
    }
}