using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFilesProcessor.Models
{
    public class TimedLyricsLineModel : INotifyPropertyChanged
    {
        private string _time;

        public int index { get; set; } = 0;
        public int LengthInMilliseconds { get; set; } = -1;
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
        public string Lyrics { get; set; }

        private bool _isPlaying = false;
        public bool IsPlaying 
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
