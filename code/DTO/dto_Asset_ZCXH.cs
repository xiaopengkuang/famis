using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace FAMIS.DTO
{
    public class dto_Asset_ZCXH
    {
        [StringLength(20)]
        public string ZCXH { get; set; }

   
    }
}