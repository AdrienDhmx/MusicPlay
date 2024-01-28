using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LastFmNamespace.Models;
using Newtonsoft.Json;

namespace LastFmNamespace.Models
{
    public class ArtistInfoModel
    {
        public string Name { get; set; } = string.Empty;
        public string Mbid { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Streamable { get; set; } = string.Empty;
        public string Ontour { get; set; } = string.Empty;
        [JsonIgnore]
        public List<string> Images { get; set; } = [];
        public Stats Stats { get; set; }
        public Similar Similar { get; set; }
        [JsonProperty("tags")]
        public Tags Tags { get; set; }
        [JsonProperty("bio")]
        public Bio Bio { get; set; }
    }

    public class Stats
    {
        public string Listeners { get; set; } = string.Empty;
        public string Playcount { get; set; } = string.Empty;
    }

    public class Similar
    {
        public List<ArtistInfoModel> Artist { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class Tags
    {
        public List<Tag> Tag { get; set; }
    }

    public class Bio
    {
        public Links Links { get; set; }
        [JsonProperty("published")]
        public string Published { get; set; } = string.Empty;
        [JsonProperty("summary")]
        public string Summary { get; set; } = string.Empty;
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class Links
    {
        public Link Link { get; set; }
    }

    public class Link
    {
        public string Text { get; set; } = string.Empty;
        public string Rel { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
    }

    public class Root
    {
        public string Method { get; set; } = string.Empty;

        [JsonProperty("Artist")]
        public ArtistInfoModel Artist { get; set; }
    }
}
