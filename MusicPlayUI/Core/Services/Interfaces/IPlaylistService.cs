using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IPlaylistService
    {
        void AddToPlaylist(List<Track> tracks, Playlist playlist);
        void AddToPlaylist(List<PlaylistTrack> tracks, Playlist playlist);
        void OnCreatePlaylistClosed(bool isCanceled, List<Track> tracks);
        void UpdateView(bool back);
        void SaveRadio(Playlist radio, List<PlaylistTrack> tracks);
        Task CreatePlaylistFromDirectory(string[] files);
    }
}