using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlayModels.StatsModels;
using MusicPlayUI.Core.Services.Interfaces;
using System;

namespace MusicPlayUI.Core.Services
{
    public class HistoryServices : IHistoryServices
    {
        private HistoryModel _todayHistory = new();
        public HistoryModel TodayHistory
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
            TodayHistory = DataAccess.Connection.GetTodayHistoryModel();

            if (TodayHistory is null || TodayHistory.Id == -1)
                TodayHistory = new();

            TodayListenTime = TimeSpan.FromMilliseconds(TodayHistory.ListenTime);
        }

        public void UpdateTodayHistory(int listenTimeIncrease)
        {
            TodayHistory.PlayCount++;
            UpdateTodayListenTime(listenTimeIncrease);
            if (TodayHistory.Id == -1)
            {
                TodayHistory = DataAccess.Connection.InsertHistoryModel(TodayHistory);
            }
            else
            {
                DataAccess.Connection.UpdateHistoryModel(TodayHistory);
            }
        }

        private void UpdateTodayListenTime(int listenTimeIncrease)
        {
            TodayHistory.ListenTime += listenTimeIncrease;
            TodayListenTime = TimeSpan.FromMilliseconds(TodayHistory.ListenTime);
        }
    }
}
