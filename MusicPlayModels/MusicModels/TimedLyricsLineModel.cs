using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayModels;

namespace MusicPlayModels.MusicModels
{
    public class TimedLyricsLineModel : BaseModel
    {
        private int _timestampMs = 0;
        private string _line = string.Empty;

        public int TimestampMs
        {
            get => _timestampMs;
            set => SetField(ref _timestampMs, value);
        }

        public string Line
        {
            get => _line;
            set => SetField(ref _line, value);
        }

        public TimedLyricsLineModel()
        {
              
        }
    }
}
