using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MusicPlayModels.MusicModels;

namespace MusicPlayModels.StatsModels
{
    public class HistoryModel : BaseModel
    {
        private DateTime _date = DateTime.Today;
        private int _playTime = 0;
        private int _playCount = 0;
        private List<HistoryEntryModel> _entries = new List<HistoryEntryModel>();

        /// <summary>
        /// The total number of playbacks
        /// </summary>
        public int PlayCount
        {
            get { return _playCount; }
            set
            {
                _playCount = value;
                OnPropertyChanged(nameof(PlayCount));
            }
        }

        /// <summary>
        /// The total playback time in milliseconds
        /// </summary>
        public int PlayTime
        {
            get { return _playTime; }
            set
            {
                _playTime = value;
                OnPropertyChanged(nameof(PlayTime));
            }
        }

        /// <summary>
        /// The date of the record
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public List<HistoryEntryModel> Entries
        {
            get => _entries;
            set => SetField(ref _entries, value);
        }

        public HistoryModel()
        {

        }

        public HistoryModel(DateTime date, int listenTime, int playCount)
        {
            Date = date;
            PlayTime = listenTime;
            PlayCount = playCount;
        }

        public override Dictionary<string, object> CreateTable()
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { nameof(Date), Date },
                { nameof(PlayTime), PlayTime },
                { nameof(PlayCount), PlayCount },
            };

            return keyValues;
        }
    }
}
