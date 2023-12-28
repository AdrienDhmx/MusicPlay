using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels
{
    public abstract class PlayableModel : BaseModel
    {
        private DateTime _updateDate = DateTime.MinValue;

        private string _duration = "";
        private int _length = 0;

        private int _playCount = 0;
        private DateTime _lastPlayed = DateTime.MinValue;

        public DateTime CreationDate { get; init; }
        public DateTime UpdateDate
        {
            get => _updateDate;
            set => SetField(ref _updateDate, value);
        }

        /// <summary>
        /// The formatted string (hh:mm:ss) of the duration of the entity (artist, album, track, playlist, tag).
        /// </summary>
        public string Duration
        {
            get => _duration;
            set
            {
                SetField(ref _duration, value);
            }
        }

        /// <summary>
        /// The length (Duration) of the entity (artist, album, track, playlist, tag) in milliseconds
        /// </summary>
        public int Length
        {
            get => _length;
            set => SetField(ref _length, value);
        }

        /// <summary>
        /// The number of time the entity has been played
        /// </summary>
        public int PlayCount
        {
            get => _playCount;
            set => SetField(ref _playCount, value);
        }

        /// <summary>
        /// The last date at which the entity has been played
        /// </summary>
        public DateTime LastPlayed
        {
            get => _lastPlayed;
            set => SetField(ref _lastPlayed, value);
        }

        protected PlayableModel(DateTime creationDate, DateTime updateDate)
        {
            CreationDate = creationDate;
            UpdateDate = updateDate;
        }

        public PlayableModel()
        {
            
        }

        internal static string TimestampToDateString(int timestamp)
        {
            if (timestamp == 0)
            {
                return "";
            }
            else if (timestamp < 3000) // only the year is known...
            {
                return timestamp.ToString();
            }
            else
            {
                return DateOnly.FromDayNumber(timestamp).ToShortDateString();
            }
        }
    }
}
