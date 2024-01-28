using MessageControl;
using MusicFilesProcessor;
using MusicFilesProcessor.Helpers;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services
{
    public class PlaylistService : IPlaylistService
    {
        public void AddToPlaylist(List<Track> tracks, Playlist playlist)
        {
            AddtoPlaylistWihtoutMsg(tracks, playlist);
            MessageHelper.PublishMessage(MessageFactory.TracksAddedToPlaylist(playlist.Name, tracks.Count));
        }

        private static void AddtoPlaylistWihtoutMsg(List<Track> tracks, Playlist playlist)
        {
            List<OrderedTrack> playlistTracks = new(); //playlist.Tracks;

            tracks.AddRange(playlistTracks.Select(pt => pt.Track));
            tracks = tracks.DistinctBy(t => t.Id).ToList();
            tracks = tracks.Where(at => !playlistTracks.Select(t => t.Track.Id).Contains(at.Id)).ToList();

            //await DataAccess.Connection.AddTrackToPlaylist(playlist, tracks);
        }

        public void AddToPlaylist(List<PlaylistTrack> tracks, Playlist playlist)
        {
            AddtoPlaylistWihtoutMsg(tracks.Select(t => t.Track).ToList(), playlist);
            MessageHelper.PublishMessage(MessageFactory.TracksAddedToPlaylist(playlist.Name, tracks.Count));
        }

        private static async void AddtoPlaylistWihtoutMsg(List<OrderedTrack> tracks, Playlist playlist)
        {
            List<OrderedTrack> playlistTracks = new(); //playlist.Tracks;

            tracks.AddRange(playlistTracks);
            tracks = tracks.DistinctBy(t => t.Track.Id).ToList();
            tracks = tracks.Where(at => !playlistTracks.Select(t => t.Track.Id).Contains(at.Track.Id)).ToList();

            //await DataAccess.Connection.AddTrackToPlaylist(playlist, tracks);
        }

        private static bool IsCurrentViewPlaylistViewModel()
        {
            return App.State.CurrentView.ViewModel.GetType() == typeof(PlaylistViewModel);
        }

        private static bool IsCurrentViewPlaylistLibraryViewModel()
        {
            return App.State.CurrentView.ViewModel.GetType() == typeof(PlaylistLibraryViewModel);
        }

        public async void SaveRadio(Playlist radio, List<PlaylistTrack> tracks)
        {
            await Playlist.Insert(radio);
            AddToPlaylist(tracks, radio);

            if(App.State.CurrentView.ViewModel.GetType() == typeof(PlaylistViewModel))
                App.State.CurrentView.ViewModel.Update(radio); // update playlist view
        }

        public async void OnCreatePlaylistClosed(bool isCanceled, List<Track> tracks)
        {
            if (!isCanceled)
            {
                var playlists = await Playlist.GetAll();

                playlists = playlists.ToList().OrderBy(t => t.CreationDate).ToList();
                Playlist createdPlaylist = playlists.LastOrDefault();

                if (createdPlaylist is not null)
                {
                    AddToPlaylist(tracks, createdPlaylist);
                }

                UpdateView(false);
            }
        }

        public void UpdateView(bool back)
        {
            if (back && IsCurrentViewPlaylistViewModel())
            {
                App.State.NavigateBack();
            }
            else if (IsCurrentViewPlaylistLibraryViewModel())
            {
                App.State.CurrentView.ViewModel.Update();
            }
        }

        public async Task CreatePlaylistFromDirectory(string[] files)
        {
            var path = files[0];
            if (Directory.Exists(path))
            {
                (bool success, string playlistName, int tracksCount) = await Task.Run<(bool, string, int)>( async () =>
                {
                    Folder newFolder = new(path);
                    await StorageService.Instance.AddFolder(newFolder);

                    ImportMusicLibrary importMusicLibrary = new(newFolder);
                    importMusicLibrary.Import();

                    List<string> musicFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList().Where(s => ImportMusicLibrary.FilesExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();
                    Track t = new(); //await DataAccess.Connection.GetTrackByPath(musicFiles[0]);
                    Album album = t.Album;

                    Playlist playlist = await PlaylistsFactory.CreatePlaylist(path.GetFolderName(), album.AlbumCover);
                    if (playlist.IsNotNull())
                    {
                        List<Track> tracks = new();
                        foreach (string m in musicFiles)
                        {
                            Track track = new(); //await DataAccess.Connection.GetTrackByPath(m);

                            if (track != null)
                            {
                                tracks.Add(track);
                            }
                        }

                        // no msg because the UI can't be updated in a tasks
                        AddtoPlaylistWihtoutMsg(tracks, playlist);
                        return (true, playlist.Name, tracks.Count);
                    }
                    return (false, string.Empty, 0);
                });

                if (success)
                {
                    MessageHelper.PublishMessage(MessageFactory.TracksAddedToPlaylist(playlistName, tracksCount));
                    UpdateView(false);
                }
            }
        }
    }
}
