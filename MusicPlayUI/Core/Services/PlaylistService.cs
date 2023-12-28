using DataBaseConnection.DataAccess;
using GongSolutions.Wpf.DragDrop;
using MessageControl;
using MusicFilesProcessor;
using MusicFilesProcessor.Helpers;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Windows.Media.Playlists;

namespace MusicPlayUI.Core.Services
{
    public class PlaylistService : IPlaylistService
    {
        
        private readonly INavigationService _navigationService;

        public PlaylistService( INavigationService navigationService)
        {
            
            _navigationService = navigationService;
        }

        public void AddToPlaylist(List<TrackModel> tracks, PlaylistModel playlist)
        {
            AddtoPlaylistWihtoutMsg(tracks, playlist);
            MessageHelper.PublishMessage(MessageFactory.TracksAddedToPlaylist(playlist.Name, tracks.Count));
        }

        private static async void AddtoPlaylistWihtoutMsg(List<TrackModel> tracks, PlaylistModel playlist)
        {
            List<OrderedTrackModel> playlistTracks = await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id);

            tracks.AddRange(playlistTracks.ToTrackModel());
            tracks = tracks.DistinctBy(t => t.Id).ToList();
            tracks = tracks.Where(at => !playlistTracks.Select(t => t.Id).Contains(at.Id)).ToList();

            await DataAccess.Connection.AddTrackToPlaylist(playlist, tracks);
        }

        public void AddToPlaylist(List<OrderedTrackModel> tracks, PlaylistModel playlist)
        {
            AddtoPlaylistWihtoutMsg(tracks, playlist);
            MessageHelper.PublishMessage(MessageFactory.TracksAddedToPlaylist(playlist.Name, tracks.Count));
        }

        private static async void AddtoPlaylistWihtoutMsg(List<OrderedTrackModel> tracks, PlaylistModel playlist)
        {
            List<OrderedTrackModel> playlistTracks = await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id);

            tracks.AddRange(playlistTracks);
            tracks = tracks.DistinctBy(t => t.Id).ToList();
            tracks = tracks.Where(at => !playlistTracks.Select(t => t.Id).Contains(at.Id)).ToList();

            await DataAccess.Connection.AddTrackToPlaylist(playlist, tracks);
        }

        public async void SaveRadio(PlaylistModel radio, List<OrderedTrackModel> tracks)
        {
            radio.Id = DataAccess.Connection.InsertPlaylist(radio);
            AddToPlaylist(tracks, radio);

            if(_navigationService.CurrentViewName == ViewNameEnum.SpecificPlaylist)
                _navigationService.CurrentViewModel.Update(await DataAccess.Connection.GetPlaylist(radio.Id)); // update playlist view
        }

        public async void OnCreatePlaylistClosed(bool isCanceled, List<TrackModel> tracks)
        {
            if (!isCanceled)
            {
                var playlists = await DataAccess.Connection.GetAllPlaylists();

                playlists = playlists.ToList().OrderBy(t => t.CreationDate).ToList();
                PlaylistModel createdPlaylist = playlists.LastOrDefault();

                if (createdPlaylist is not null)
                {
                    AddToPlaylist(tracks, createdPlaylist);
                }

                UpdateView(false);
            }
        }

        public void UpdateView(bool back)
        {
            if (back && _navigationService.CurrentViewName == ViewNameEnum.SpecificPlaylist)
            {
                _navigationService.NavigateBack();
            }
            else if (_navigationService.CurrentViewName == ViewNameEnum.Playlists)
            {
                _navigationService.CurrentViewModel.Update();
            }
        }

        public async Task CreatePlaylistfromDirectory(string[] files)
        {
            var path = files[0];
            if (Directory.Exists(path))
            {
                (bool success, string playlistName, int tracksCount)= await Task.Run<(bool, string, int)>( async () =>
                {
                    ImportMusicLibrary importMusicLibrary = new(path);
                    importMusicLibrary.Import();

                    List<string> musicFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList().Where(s => ImportMusicLibrary.FilesExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();
                    TrackModel t = await DataAccess.Connection.GetTrackByPath(musicFiles[0]);
                    AlbumModel album = await DataAccess.Connection.GetAlbum(t.AlbumId);

                    int id  = PlaylistsFactory.CreatePlaylist(path.GetFolderName(), album.AlbumCover);
                    if (id >= 0)
                    {
                        PlaylistModel playlist = await DataAccess.Connection.GetPlaylist(id);
                        List<TrackModel> tracks = new();
                        foreach (string m in musicFiles)
                        {
                            TrackModel track = await DataAccess.Connection.GetTrackByPath(m);

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
