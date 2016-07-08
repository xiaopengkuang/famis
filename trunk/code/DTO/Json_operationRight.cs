using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_operationRight
    {
        public bool? add { get; set; }
        public bool? edit { get; set; }
        public bool? view { get; set; }
        public bool? export { get; set; }
        public bool? submit { get; set; }
        public bool? review { get; set; }
        public bool? print { get; set; }
        public bool? delete { get; set; }
    }
}