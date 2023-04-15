using MusicPlayModels.MusicModels;
using MusicPlayModels.StatsModels;
using System;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IHistoryServices
    {
        HistoryModel TodayHistory { get; set; }
        TimeSpan TodayListenTime { get; }

        void UpdateTodayHistory(int listenTimeIncrease);
    }
}