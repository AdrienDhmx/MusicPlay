using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesProcessor.Providers.Models;
using Newtonsoft.Json;

namespace FilesProcessor.Providers.Models
{
    public class NormalizedQuery
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class LangLink
    {
        public string lang { get; set; }
        [JsonProperty("*")]
        public string title { get; set; }
    }

    public class WikiPage
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public string extract { get; set; }
        public string contentmodel { get; set; }
        public string pagelanguage { get; set; }
        public string pagelanguagehtmlcode { get; set; }
        public string pagelanguagedir { get; set; }
        public string touched { get; set; }
        public int lastrevid { get; set; }
        public int length { get; set; }
        public string fullurl { get; set; }
        public string editurl { get; set; }
        public string canonicalurl { get; set; }
        public List<LangLink> langlinks { get; set; }
    }

    public class WikiQueryResult
    {
        public string batchcomplete { get; set; }
        public WikiQuery query { get; set; }
    }

    public class WikiQuery
    {
        public List<NormalizedQuery> normalized { get; set; }
        public Dictionary<string, WikiPage> pages { get; set; }
    }
}
