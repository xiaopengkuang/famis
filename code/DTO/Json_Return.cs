using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FAMIS.DTO
{
    public class Json_Return
    {
        public int ID { get; set; }
        [StringLength(20)]
        public string serialNum { get; set; }
        public DateTime? date_return { get; set; }

        [StringLength(200)]
        public string reason_return { get; set; }

        [StringLength(200)]
        public string note_return { get; set; }

        public String user_operated { get; set; }
        public DateTime? date_operated { get; set; }
        public String state { get; set; }

    }
}