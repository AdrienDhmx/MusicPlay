using System;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IAudioTimeService
    {
        string CurrentPosition { get; }
        int CurrentPositionMs { get; set; }
        string CurrentQueuePosition { get; }
        int CurrentQueuePositionMs { get; }
        int MaxPositionMs { get; }
        int MaxQueuePositionMs { get; }

        event Action CurrentPositionChanged;

        void SetPlayBackPosition(int position);
        void PlayPause();
        void Loop();
    }
}