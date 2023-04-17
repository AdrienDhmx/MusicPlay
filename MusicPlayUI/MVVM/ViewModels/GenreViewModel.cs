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
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class GenreViewModel : ViewModel
    {
        private readonly ICommandsManager _commandsManager;
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private const int _maxNumberOfCoversToDisplay = 14;

        private GenreModel _genre;
        public GenreModel Genre
        {
            get { return _genre ; }
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

        private List<string> _covers= new();
        public List<string> Covers
        {
            get => _covers;
            set
            {
                SetField(ref _covers, value);
            }
        }

        private ObservableCollection<TrackModel> _tracks;
        public ObservableCollection<TrackModel> Tracks
        {
            get => _tracks;
            set
            {
                SetField(ref _tracks, value);
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

        private Visibility _artistsVisibility = Visibility.Collapsed;
        public Visibility ArtistsVisibility
        {
            get => _artistsVisibility;
            set
            {
                SetField(ref _artistsVisibility, value);
            }
        }

        public ICommand PlayGenreCommand { get; }
        public ICommand OpenGenrePopupCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand ShowHideArtistsCommand { get; }
        public ICommand ShowHideAlbumsCommand { get; }
        public ICommand PlayArtistsOnlyCommand { get; }
        public ICommand PlayAlbumsOnlyCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand PlayAlbumCommand { get; }
        public ICommand PlayArtistCommand { get; }
        public GenreViewModel(ICommandsManager commandsManager, INavigationService navigationService, IQueueService queueService)
        {
            _commandsManager = commandsManager;
            _navigationService = navigationService;
            _queueService = queueService;

            // play commands
            PlayGenreCommand = new RelayCommand<string>((shuffle) =>
            {
                _queueService.SetNewQueue(Tracks.ToList(), Genre.Name, ModelTypeEnum.Genre, "", null, shuffle == "1");
            });

            PlayAlbumsOnlyCommand = new RelayCommand<string>(async (shuffle) =>
            {
                List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbums(Albums.Select(a => a.Id));
                _queueService.SetNewQueue(tracks, Genre.Name, ModelTypeEnum.Genre, "", null, shuffle == "1");
            });

            PlayArtistsOnlyCommand = new RelayCommand<string>(async (shuffle) =>
            {
                List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromArtists(Artists.Select(a => a.Id));
                _queueService.SetNewQueue(tracks, Genre.Name, ModelTypeEnum.Genre, "", null, shuffle == "1");
            });

            ShowHideArtistsCommand = new RelayCommand(() =>
            {
                ArtistsVisibility = ArtistsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });

            ShowHideAlbumsCommand = new RelayCommand(() =>
            {
                AlbumsVisibility = AlbumsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });

            // navigate commands
            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumCommand;
            NavigateToArtistCommand = _commandsManager.NavigateToArtistCommand;

            // open popup commands
            OpenAlbumPopupCommand = _commandsManager.OpenAlbumPopupCommand;
            OpenArtistPopupCommand = _commandsManager.OpenArtistPopupCommand;
            PlayAlbumCommand = _commandsManager.OpenTrackPopupCommand;

            Update();
        }

        public override async void Update(BaseModel parameter = null)
        {
            if(parameter != null && parameter is GenreModel genre)
            {
                Genre = genre;
            }
            else
            {
                Genre = _navigationService.CurrentViewParameter as GenreModel;
            }

            Artists = new(await DataAccess.Connection.GetArtistFromGenre(Genre.Id));
            Albums = new(await DataAccess.Connection.GetAlbumFromGenre(Genre.Id));

            int ar = 0;
            int al = 0;
            for (int i = 0; i < _maxNumberOfCoversToDisplay; i++)
            {
                if(al == Albums.Count && ar == Artists.Count)
                {
                    al = 0; // restart
                    ar = 0;
                }
                if(al < Albums.Count) 
                {
                    Covers.Add(Albums[al].AlbumCover);
                    al++;
                }
                else if(ar < Artists.Count)
                {
                    while (Covers.Contains(Artists[ar].Cover))
                    {
                        if (ar == Artists.Count - 1)
                            break;

                        ar++;
                    }
                    Covers.Add(Artists[ar].Cover);
                    ar++;
                }
            }

            List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbums(Albums.Select(a => a.Id));
            tracks = tracks.DistinctBy(t => t.Id).ToList();
            Tracks = new(tracks);

            Duration = tracks.GetTotalLength(out int _);
        }
    }
}
