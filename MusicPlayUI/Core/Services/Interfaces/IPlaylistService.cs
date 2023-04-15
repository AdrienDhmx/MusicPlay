using MusicPlayModels.MusicModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IPlaylistService
    {
        void AddToPlaylist(List<TrackModel> tracks, PlaylistModel playlist);
        void AddToPlaylist(List<OrderedTrackModel> tracks, PlaylistModel playlist);
        void OnCreatePlaylistClosed(bool isCanceled, List<TrackModel> tracks);
        void UpdateView(bool back);
        void SaveRadio(PlaylistModel radio, List<OrderedTrackModel> tracks);
        Task CreatePlaylistfromDirectory(string[] files);
    }
}