using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MusicPlayModels.MusicModels;

namespace MusicPlayModels.StatsModels
{
    public class HistoryEntryModel : BaseModel
    {
        private readonly int _historyId;
        private TrackModel _track;
        private int _playTime;
        private int _playedLength;

        /// <summary>
        /// The track played in this entry
        /// </summary>
        public TrackModel Track
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
                if(value > Track.Length) 
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

        public string PlayedPercentage => (PlayedLength * 100 / Track.Length).ToString() + "%";

        public HistoryEntryModel(TrackModel track, int playedLength, int playTime, int historyId)
        {
            _track = track;
            _playTime = playTime;
            _playedLength = playedLength;
            _historyId = historyId;
        }


        public HistoryEntryModel(TrackModel track, int playedLength, int historyId)
        {
            _track = track;
            _playedLength = playedLength;
            _playTime = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            _historyId = historyId;
        }

        public override Dictionary<string, object> CreateTable()
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { DataBaseColumns.TrackId, Track.Id },
                { nameof(PlayTime), PlayTime },
                { nameof(PlayedLength), PlayedLength },
                { DataBaseColumns.HistoryId, _historyId },
            };
            return keyValues;
        }
    }
}
