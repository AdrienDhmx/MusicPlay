using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MusicPlay.Database.Helpers;

namespace MusicPlay.Database.Models
{
    [Table("TimedLyricsLine")]
    public class TimedLyricsLine : BaseModel
    {
        private int _timestampMs = 0;
        private string _line = string.Empty;
        private int _index = 0;
        private bool _isPlaying = false;

        [Required]
        public int LyricsId { get; set; }

        [Required]
        public int TimestampMs
        {
            get => _timestampMs;
            set => SetField(ref _timestampMs, value);
        }

        [Required]
        public string Line
        {
            get => _line;
            set => SetField(ref _line, value);
        }

        [NotMapped]
        public int Index
        {
            get => _index;
            set => SetField(ref _index, value);
        }

        [NotMapped]
        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetField(ref _isPlaying, value);
        }
    }
}
