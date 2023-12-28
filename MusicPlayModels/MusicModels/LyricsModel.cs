using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class LyricsModel : BaseModel
    {
        private string _lyrics = string.Empty;
        private bool _isTimed = false;
        private List<TimedLyricsLineModel> _timedLines = new ();

        private string _webSiteSource = string.Empty;
        public string _url = string.Empty;
        public bool _isFromUser = false;
        public bool _isSaved = false;

        public string Lyrics
        {
            get => _lyrics;
            set => SetField(ref _lyrics, value);
        }

        public bool IsTimed
        {
            get => _isTimed;
            set => SetField(ref _isTimed, value);
        }

        public List<TimedLyricsLineModel> TimedLines
        {
            get => _timedLines;
            set => SetField(ref _timedLines, value);
        }

        public string WebSiteSource
        {
            get => _webSiteSource;
            set => SetField(ref _webSiteSource, value);
        }

        public string Url
        {
            get => _url; 
            set => SetField(ref _url, value);
        }

        public bool IsFromUser
        {
            get => _isFromUser;
            set => SetField(ref _isFromUser, value);
        }

        public bool IsSaved
        {
            get => _isSaved; 
            set => SetField(ref _isSaved, value);
        }
    }
}
