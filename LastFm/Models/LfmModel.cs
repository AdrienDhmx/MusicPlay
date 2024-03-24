using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LastFmNamespace.Models
{
    public class LfmModel
    {
        public string Status { get; set; } = string.Empty;

        public bool IsError => Status == "failed";
        public int ErrorCode { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
