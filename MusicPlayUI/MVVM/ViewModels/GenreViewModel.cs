using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ABI.System.Collections.Generic;
using DataBaseConnection.DataAccess;
using MusicFilesProcessor.Helpers;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class GenreViewModel : ViewModel
    {
        private readonly ICommandsManager _commandsManager;
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private const int _maxNumberOfCoversToDisplay = 14;

        private UITagModel _genre;
        public UITagModel Genre
        {
            get { return _genre; }
            set
            {
                SetField(ref _genre, value);
            }
        }

        private ObservableCollection<ArtistModel> _artists;
        public ObservableCollection<ArtistModel> Artists
        {
            get => _artists;
            set
            {
                SetField(ref _artists, value);
            }
        }

        private ObservableCollection<AlbumModel> _albums;
        public ObservableCollection<AlbumModel> Albums
        {
            get => _albums;
            set
            {
                SetField(ref _albums, value);
            }
        }

        private ObservableCollection<PlaylistModel> _playlist;
        public ObservableCollection<PlaylistModel> BindedPlaylists
        {
            get => _playlist;
            set
            {
                SetField(ref _playlist, value);
            }
        }

        private ObservableCollection<UIOrderedTrackModel> _tracks;
        public ObservableCollection<UIOrderedTrackModel> Tracks
        {
            get => _tracks;
            set
            {
                SetField(ref _tracks, value);
            }
        }

        private ObservableCollection<TrackModel> _allTracks;
        public ObservableCollection<TrackModel> AllTracks
        {
            get => _allTracks;
            set
            {
                SetField(ref _allTracks, value);
            }
        }

        private string _duration = "";
        public string Duration
        {
            get => _duration;
            set
            {
                SetField(ref _duration, value);
            }
        }

        private Visibility _albumsVisibility = Visibility.Visible;
        public Visibility AlbumsVisibility
        {
            get => _albumsVisibility;
            set
            {
                SetField(ref _albumsVisibility, value);
            }
        }

        private Visibility _artistsVisibility = Visibility.Visible;
        public Visibility ArtistsVisibility
        {
            get => _artistsVisibility;
            set
            {
                SetField(ref _artistsVisibility, value);
            }
        }

        private Visibility _playlistsVisibility = Visibility.Visible;
        public Visibility PlaylistsVisibility
        {
            get => _playlistsVisibility;
            set
            {
                SetField(ref _playlistsVisibility, value);
            }
        }

        private Visibility _tracksVisibility = Visibility.Visible;
        public Visibility TracksVisibility
        {
            get => _tracksVisibility;
            set
            {
                SetField(ref _tracksVisibility, value);
            }
        }

        public ICommand PlayGenreCommand { get; }
        public ICommand OpenGenrePopupCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public ICommand NavigateToArtistByIdCommand { get; }
        public ICommand NavigateToPlaylistCommand { get; }

        public ICommand ShowHideArtistsCommand { get; }
        public ICommand ShowHideAlbumsCommand { get; }
        public ICommand ShowHidePlaylistsCommand { get; }
        public ICommand ShowHideTracksCommand { get; }

        public ICommand PlayArtistsOnlyCommand { get; }
        public ICommand PlayAlbumsOnlyCommand { get; }
        public ICommand PlayPlaylistsOnlyCommand { get; }
        public ICommand PlayTracksOnlyCommand { get; }

        public ICommand OpenTagPopupCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand OpenPlaylistPopupCommand { get;  }
        public ICommand OpenTrackPopupCommand { get;  }

        public ICommand PlayAlbumCommand { get; }
        public ICommand PlayArtistCommand { get; }
        public ICommand PlayPlaylistCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand EditTagCommand { get; }
        public GenreViewModel(ICommandsManager commandsManager, INavigationService navigationService, IQueueService queueService, IModalService modalService)
        {
            _commandsManager = commandsManager;
            _navigationService = navigationService;
            _queueService = queueService;
            _modalService = modalService;

            // play commands
            PlayGenreCommand = new RelayCommand<string>((shuffle) =>
            {
                _queueService.SetNewQueue(AllTracks.ToList(), new(Genre.Name, ModelTypeEnum.Genre, Genre.Id), "", null, shuffle == "1");
            });

            PlayAlbumsOnlyCommand = new RelayCommand<string>(async (shuffle) =>
            {
                List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbums(Albums.Select(a => a.Id));
                _queueService.SetNewQueue(tracks, new(Genre.Name, ModelTypeEnum.Genre, Genre.Id), "", null, shuffle == "1");
            });

            PlayArtistsOnlyCommand = new RelayCommand<string>(async (shuffle) =>
            {
                List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromArtists(Artists.Select(a => a.Id));
                _queueService.SetNewQueue(tracks, new(Genre.Name, ModelTypeEnum.Genre, Genre.Id), "", null, shuffle == "1");
            });

            PlayPlaylistsOnlyCommand = new RelayCommand<string>(async (shuffle) =>
            {
                _queueService.SetNewQueue(await GetPlaylistsTracks(), new(Genre.Name, ModelTypeEnum.Genre, Genre.Id), "", null, shuffle == "1");
            });

            PlayTracksOnlyCommand = new RelayCommand<string>((shuffle) =>
            {
                _queueService.SetNewQueue(Tracks.ToList(), new(Genre.Name, ModelTypeEnum.Genre, Genre.Id), "", null, shuffle == "1");
            });

            ShowHideArtistsCommand = new RelayCommand(() =>
            {
                ArtistsVisibility = ArtistsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });

            ShowHideAlbumsCommand = new RelayCommand(() =>
            {
                AlbumsVisibility = AlbumsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });

            ShowHidePlaylistsCommand = new RelayCommand(() =>
            {
                PlaylistsVisibility = PlaylistsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });

            ShowHideTracksCommand = new RelayCommand(() =>
            {
                TracksVisibility = TracksVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });

            PlayAlbumCommand = new RelayCommand<AlbumModel>(async (album) =>
            {
                List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbum(album.Id);
                _queueService.SetNewQueue(tracks, new(album.Name, ModelTypeEnum.Album, album.Id), album.AlbumCover);
            });

            PlayArtistCommand = new RelayCommand<ArtistModel>(async (artist) =>
            {
                List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromArtist(artist.Id);
                _queueService.SetNewQueue(tracks, new(artist.Name, ModelTypeEnum.Artist, artist.Id), artist.Cover);
            });

            PlayPlaylistCommand = new RelayCommand<PlaylistModel>(async (playlist) =>
            {
                List<OrderedTrackModel> tracks = await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id);
                _queueService.SetNewQueue(tracks.ToTrackModel(), new(playlist.Name, ModelTypeEnum.Artist, playlist.Id), playlist.Cover);
            });

            PlayTrackCommand = new RelayCommand<TrackModel>((track) =>
            {
                _queueService.SetNewQueue(Tracks.ToList(), new(Genre.Name, ModelTypeEnum.Genre, Genre.Id), track.AlbumCover, track);
            });

            EditTagCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(ViewNameEnum.CreateTag, (bool canceled) => { }, Genre);
            });

            // navigate commands
            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumCommand;
            NavigateToArtistCommand = _commandsManager.NavigateToArtistCommand;
            NavigateToPlaylistCommand = _commandsManager.NavigateToPlaylistCommand;
            NavigateToAlbumByIdCommand = _commandsManager.NavigateToAlbumByIdCommand;
            NavigateToArtistByIdCommand = _commandsManager.NavigateToArtistByIdCommand;


            // open popup commands
            OpenAlbumPopupCommand = _commandsManager.OpenAlbumPopupCommand;
            OpenArtistPopupCommand = _commandsManager.OpenArtistPopupCommand;
            OpenPlaylistPopupCommand = _commandsManager.OpenPlaylistPopupCommand;
            OpenTrackPopupCommand = _commandsManager.OpenTrackPopupCommand;
            OpenTagPopupCommand = _commandsManager.OpenTagPopupCommand;

            Update();
        }

        public async Task<List<TrackModel>> GetPlaylistsTracks()
        {
            List<TrackModel> tracks = new();

            foreach (PlaylistModel playlist in BindedPlaylists)
            {
                tracks.AddRange(await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id));
            }
            return tracks;
        }

        public override async void Update(BaseModel parameter = null)
        {
            if(parameter != null && parameter is UITagModel genre)
            {
                Genre = genre;
            }
            else
            {
                Genre = _navigationService.CurrentViewParameter as UITagModel;
            }

            Artists = new(Genre.Artists);
            Albums = new(Genre.Albums);
            BindedPlaylists = new(Genre.Playlists);
            Tracks = new(Genre.Tracks.ToUIOrderedTrackModel(_queueService.AlbumCoverOnly, _queueService.AutoCover));

            List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbums(Albums.Select(a => a.Id));
            tracks.AddRange(await DataAccess.Connection.GetTracksFromArtists(Artists.Select(a => a.Id)));
            tracks.AddRange(await GetPlaylistsTracks());
            tracks.AddRange(Tracks);
            tracks = tracks.DistinctBy(t => t.Id).ToList();
            AllTracks = new(tracks);

            Duration = tracks.GetTotalLength(out int _);
        }

        public int RemoveAlbum(int albumId)
        {
            for (int i = 0; i < Albums.Count; i++)
            {
                if (Albums[i].Id == albumId)
                {
                    Albums.RemoveAt(i);
                    return i;
                }
            }
            return -1;
        }

        public int RemoveArtist(int artistId)
        {
            for (int i = 0; i < Artists.Count; i++)
            {
                if (Artists[i].Id == artistId)
                {
                    Artists.RemoveAt(i);
                    return i;
                }
            }
            return -1;
        }

        public int RemovePlaylist(int playlistId)
        {
            for (int i = 0; i < BindedPlaylists.Count; i++)
            {
                if (BindedPlaylists[i].Id == playlistId)
                {
                    BindedPlaylists.RemoveAt(i);
                    return i;
                }
            }
            return -1;
        }

        public int RemoveTrack(int trackId)
        {
            for (int i = 0; i < Tracks.Count; i++)
            {
                if (Tracks[i].Id == trackId)
                {
                    Tracks.RemoveAt(i);
                    return i;
                }
            }
            return -1;
        }
    }
}
