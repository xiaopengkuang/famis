using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_WXSearch_AssetList
    {

        public int ID { get; set; }
        public String name { get; set; }
        public String serialNum { get; set; }
        public String AssetType { get; set; }
        public String state { get; set; }
    }
}