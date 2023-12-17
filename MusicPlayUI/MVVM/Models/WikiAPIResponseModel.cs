using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.MVVM.Models
{
    public class NormalizedQuery
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class WikiPage
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public string extract { get; set; }
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
