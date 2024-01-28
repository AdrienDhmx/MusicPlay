

using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services
{
    public class HistoryServices : IHistoryServices
    {
        private PlayHistory _todayHistory = new();
        public PlayHistory TodayHistory
        {
            get { return _todayHistory; }
            set
            {
                _todayHistory = value;
            }
        }

        private TimeSpan _todayListenTime;
        public TimeSpan TodayListenTime
        {
            get => _todayListenTime;
            private set
            {
                _todayListenTime = value;
            }
        }

        public HistoryServices()
        {
            TodayHistory = PlayHistory.GetTodayHistory().Result;
            TodayListenTime = TimeSpan.FromMilliseconds(TodayHistory.PlayTime);
        }

        public void UpdateTodayHistory(Track track, int listenTimeIncrease)
        {
            if (listenTimeIncrease < 10000)
                return;

            UpdateTodayListenTime(listenTimeIncrease);
            PlayHistoryEntry entry = new PlayHistoryEntry()
            {
                TrackId = track.Id,
                HistoryId = TodayHistory.Id,
                PlayedLength = listenTimeIncrease,
                PlayTime = (int)TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan().TotalSeconds
            };
            PlayHistoryEntry.Insert(entry);
            entry.Track = track;
            TodayHistory.Entries.Add(entry);
        }

        private void UpdateTodayListenTime(int listenTimeIncrease)
        {
            TodayHistory.PlayTime += listenTimeIncrease;
            TodayListenTime = TimeSpan.FromMilliseconds(TodayHistory.PlayTime);
        }
    }

}
