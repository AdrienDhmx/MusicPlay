using MusicPlay.Database.Models;
using System;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IHistoryServices
    {
        PlayHistory TodayHistory { get; set; }
        TimeSpan TodayListenTime { get; }

        void UpdateTodayHistory(Track track, int listenTimeIncrease);
    }
}