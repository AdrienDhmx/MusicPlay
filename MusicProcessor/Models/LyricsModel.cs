using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFilesProcessor.Models
{
    public class LyricsModel
    {
        public string Lyrics { get; set; }
        public bool IsTimed { get; set; } = false;
        public List<TimedLyricsLineModel> TimedLyrics { get; set; } = new();
        public string WebSiteSource { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public bool IsFromUser { get; set; } = false;
        public string FileName { get; set; }
        public bool IsSaved { get; set; } = false;
    }
}
