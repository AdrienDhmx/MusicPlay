using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.StatsModels
{
    public class HistoryModel : BaseModel
    {
        private DateTime _date = DateTime.Today;
        private int _listenTime = 0;
        private int _playCount = 0;

        /// <summary>
        /// The model identifier
        /// </summary>
        public int Id { get; set; } = -1;

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
        public int ListenTime
        {
            get { return _listenTime; }
            set
            {
                _listenTime = value;
                OnPropertyChanged(nameof(ListenTime));
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

        public HistoryModel()
        {

        }

        public HistoryModel(DateTime date, int listenTime, int playCount)
        {
            Date = date;
            ListenTime = listenTime;
            PlayCount = playCount;
        }
    }
}
