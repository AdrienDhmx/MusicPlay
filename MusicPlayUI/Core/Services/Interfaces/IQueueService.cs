using GongSolutions.Wpf.DragDrop;
using MusicPlayModels.MusicModels;
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
        bool AlbumCoverOnly { get; set; }
        bool AreCoversEnabled { get; set; }
        bool ArtworkOnly { get; set; }
        bool AutoCover { get; set; }
        bool IsOnRepeat { get; }
        bool IsShuffled { get; }
        QueuePlayingFromModel PlayingFrom { get; set; }
        UIOrderedTrackModel PlayingTrack { get; set; }
        string QueueCover { get; set; }
        string QueueDuration { get; set; }
        int QueueLength { get; set; }
        ObservableCollection<UIOrderedTrackModel> QueueTracks { get; set; }

        event Action PlayingTrackChanged;
        event Action QueueChanged;
        event Action PlayingTrackInteractionChanged;

        void AddTrack(TrackModel track, bool end = false, bool showMsg = true);
        void AddTracks(List<TrackModel> tracks, bool end = false, bool album = true, string name = "");
        void DeleteQueue();
        void DragOver(IDropInfo dropInfo);
        void Drop(IDropInfo dropInfo);
        int GetPlayingTrackIndex();
        int GetTrackIndex(TrackModel track);
        void IncreasePlayCount(int increaseValue);
        void MoveTrack(int originalIndex, int targetIndex);
        bool NextTrack();
        void PlayTrack(TrackModel track);
        bool PreviousTrack();
        void RemoveTrack(TrackModel track);
        void Repeat();
        void SetNewQueue(List<UIOrderedTrackModel> tracks, QueuePlayingFromModel playingFrom, string cover, TrackModel playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false);
        void SetNewQueue(List<TrackModel> tracks, QueuePlayingFromModel playingFrom, string cover, TrackModel playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false);
        void Shuffle();
        Task UpdateFavorite(bool isFavorite);
        void UpdateRating(int rating);

        Task SaveQueue();
        void NavigateToPlayingFrom();
    }
}