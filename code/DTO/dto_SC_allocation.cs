using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class dto_SC_List
    {
        public String serialNumber{get;set;}

        public String stateList { get; set; }

        public DateTime? begin { get; set; }

        public DateTime? end { get; set; }
    }
}