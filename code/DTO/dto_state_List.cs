using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FAMIS.DTO
{
    public class dto_state_List
    {
        public int id { get; set; }

        [StringLength(20)]
        public string Name { get; set; }
    }
}