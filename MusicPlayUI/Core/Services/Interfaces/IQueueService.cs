using GongSolutions.Wpf.DragDrop;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IQueueService
    {
        Queue Queue { get; }

        event Action PlayingTrackChanged;
        event Action<int> PreviewPlayingTrackChanged;
        event Action QueueChanged;
        event Action PlayingTrackInteractionChanged;

        void SetNewQueue(IEnumerable<OrderedTrack> tracks, PlayableModel playingFrom, string playingFromName, string cover, Track playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false);
        void SetNewQueue(IEnumerable<Track> tracks, PlayableModel playingFrom, string playingFromName, string cover, Track playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false);
        void DeleteQueue();

        void PlayTrack(Track track);
        bool NextTrack();
        bool PreviousTrack();

        void Repeat();
        Task Shuffle();

        void AddTrack(Track track, bool end = false, bool showMsg = true);
        void AddTracks(List<Track> tracks, bool end = false, bool album = true, string name = "");
        void MoveTrack(int originalIndex, int targetIndex);
        void RemoveTrack(Track track);

        int GetPlayingTrackIndex();
        int GetTrackIndex(Track track);
        Task IncreasePlayCount(int increaseValue);
        Task UpdateFavorite(bool isFavorite);
        Task UpdateRating(int rating);

        void DragOver(IDropInfo dropInfo);
        void Drop(IDropInfo dropInfo);

        void ClearQueue();
        void SaveQueue();
        Task NavigateToPlayingFrom();
    }
}