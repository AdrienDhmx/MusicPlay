using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;

namespace MusicPlay.Database.Models
{
    [Table("PlayHistoryEntry")]
    public class PlayHistoryEntry : BaseModel
    {
        private Track _track;
        private int _playTime;
        private int _playedLength;

        [Required]
        public int HistoryId { get; set; }
        [Required]
        public int TrackId { get; set; }

        /// <summary>
        /// The track played in this entry
        /// </summary>
        public Track Track
        {
            get => _track;
            set => SetField(ref _track, value);
        }

        /// <summary>
        /// The length of the playback
        /// </summary>
        public int PlayedLength
        {
            get => _playedLength;
            set
            {
                if (Track.IsNotNull() && value > Track.Length)
                    value = Track.Length;
                SetField(ref _playedLength, value);
            }
        }

        /// <summary>
        /// The time of the day in seconds at which the track was played
        /// </summary>
        public int PlayTime
        {
            get => _playTime;
            set => SetField(ref _playTime, value);
        }

        [NotMapped]
        public string PlayedPercentage => (PlayedLength * 100 / Track.Length).ToString() + "%";

        public PlayHistoryEntry(Track track, int playedLength, int playTime, int historyId)
        {
            _track = track;
            _playTime = playTime;
            _playedLength = playedLength;
            HistoryId = historyId;
        }


        public PlayHistoryEntry(Track track, int playedLength, int historyId)
        {
            _track = track;
            _playedLength = playedLength;
            _playTime = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            HistoryId = historyId;
        }

        public PlayHistoryEntry()
        {
            
        }

        public static void Insert(PlayHistoryEntry historyEntry)
        {
            using DatabaseContext context = new();
            context.PlayHistoryEntries.Add(historyEntry);
            context.SaveChanges();
        }
    }
}
