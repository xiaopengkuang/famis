using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class dto_SC_Asset
    {
       public String typeFlag { get; set; }
       public int? nodeID { get; set; }

       public String dataName { set; get; }
       public String nodeText { get; set; }
       public String DataType { get; set; }
       public DateTime? begin { get; set; }
       public DateTime? end { get; set; }
       public String TypeAsset { get; set; }
       public String contentSC { get; set; }

    }
}