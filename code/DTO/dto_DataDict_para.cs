using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_DataDict_para
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string name_para { get; set; }
    }
}