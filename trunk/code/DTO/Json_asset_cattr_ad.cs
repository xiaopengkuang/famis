using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace FAMIS.DTO
{
    public class Json_asset_cattr_ad
    {
        public int? ID_Asset { get; set; }

        public int? ID_customAttr { get; set; }

        [StringLength(20)]
        public string value { get; set; }
    }
}