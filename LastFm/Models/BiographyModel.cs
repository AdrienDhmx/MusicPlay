using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LastFmNamespace.Models
{
    public class BiographyModel
    {
        public DateTime Published { get; set; }

        public string Url { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}
