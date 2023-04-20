using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlay.Language;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using MusicFilesProcessor.Helpers;
using MusicPlayModels;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private readonly IRadioStationsService _radioStationsServices;
        private readonly IHistoryServices _historyServices;

        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get { return _welcomeMessage; }
            set
            {
                _welcomeMessage = value;
                OnPropertyChanged(nameof(WelcomeMessage));
            }
        }

        private bool _playHistory = false;
        public bool PlayHistory
        {
            get { return _playHistory; }
            set
            {
                _playHistory = value;
                OnPropertyChanged(nameof(PlayHistory));
            }
        }

        private string _todayStats;
        public string TodayStats
        {
            get { return _todayStats; }
            set
            {
                _todayStats = value;
                OnPropertyChanged(nameof(TodayStats));
            }
        }

        public XmlLanguage CurrentCulture
        {
            get => LanguageService.CurrentXamlCulture;
        }

        private string _numberOfArtists;
        public string NumberOfArtists
        {
            get { return _numberOfArtists; }
            set
            {
                _numberOfArtists = value;
                OnPropertyChanged(nameof(NumberOfArtists));
            }
        }

        private string _numberOfAlbums;
        public string NumberOfAlbums
        {
            get { return _numberOfAlbums; }
            set
            {
                _numberOfAlbums = value;
                OnPropertyChanged(nameof(NumberOfAlbums));
            }
        }

        private string _numberOfTracks;
        public string NumberOfTracks
        {
            get { return _numberOfTracks; }
            set
            {
                _numberOfTracks = value;
                OnPropertyChanged(nameof(NumberOfTracks));
            }
        }

        private ObservableCollection<ArtistModel> _bindedArtists = new();
        public ObservableCollection<ArtistModel> BindedArtists
        {
            get { return _bindedArtists; }
            set
            {
                _bindedArtists = value;
                OnPropertyChanged(nameof(BindedArtists));
            }
        }

        private ObservableCollection<AlbumModel> _bindedAlbums = new();
        public ObservableCollection<AlbumModel> BindedAlbums
        {
            get { return _bindedAlbums; }
            set
            {
                _bindedAlbums = value;
                OnPropertyChanged(nameof(BindedAlbums));
            }
        }

        private int _topLastPlayedArtists = 14;
        public int TopLastPlayedArtists
        {
            get { return _topLastPlayedArtists; }
            set
            {
                _topLastPlayedArtists = value;
                OnPropertyChanged(nameof(TopLastPlayedArtists));
            }
        }

        private int _topLastPlayedAlbums = 14;
        public int TopLastPlayedAlbums
        {
            get { return _topLastPlayedAlbums; }
            set
            {
                _topLastPlayedAlbums = value;
                OnPropertyChanged(nameof(TopLastPlayedAlbums));
            }
        }

        private List<PlaylistModel> _bindedPlaylists;
        public List<PlaylistModel> BindedPlaylists
        {
            get { return _bindedPlaylists; }
            set
            {
                _bindedPlaylists = value;
                OnPropertyChanged(nameof(BindedPlaylists));
            }
        }

        public ICommand NavigateToArtistsCommand { get; }
        public ICommand NavigateToAlbumsCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand PlayArtistCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand PlayAlbumCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public ICommand PlayPlaylistCommand { get; }
        public ICommand NavigateToPlaylistCommand { get; }
        public ICommand OpenPlaylistPopupCommand { get; }
        public HomeViewModel(INavigationService navigationService, IQueueService queueService, IRadioStationsService radioStationsServices, IHistoryServices historyServices)
        {
            _navigationService = navigationService;
            _queueService = queueService;
            _radioStationsServices = radioStationsServices;
            _historyServices = historyServices;

            // Cards navigation Commands
            NavigateToArtistsCommand = new RelayCommand(() => Navigate(ViewNameEnum.Artists));
            NavigateToAlbumsCommand = new RelayCommand(() => Navigate(ViewNameEnum.Albums));

            // navigation commands
            NavigateToArtistCommand = new RelayCommand<ArtistModel>((artist) => Navigate(ViewNameEnum.SpecificArtist, artist));
            NavigateToAlbumCommand = new RelayCommand<AlbumModel>((album) => Navigate(ViewNameEnum.SpecificAlbum, album));
            NavigateToPlaylistCommand = new RelayCommand<PlaylistModel>((playlist) => Navigate(ViewNameEnum.SpecificPlaylist, playlist));

            // play commands
            PlayArtistCommand = new RelayCommand<ArtistModel>((artist) => PlayArtist(artist));
            PlayAlbumCommand = new RelayCommand<AlbumModel>((album) => PlayAlbum(album));
            PlayPlaylistCommand = new RelayCommand<PlaylistModel>((playlist) => PlayPlaylist(playlist));

            // open popup commands
            OpenArtistPopupCommand = new RelayCommand<ArtistModel>((artist) => OpenPopup(artist, ViewNameEnum.ArtistPopup));
            OpenAlbumPopupCommand = new RelayCommand<AlbumModel>((album) => OpenPopup(album, ViewNameEnum.AlbumPopup));
            OpenPlaylistPopupCommand = new RelayCommand<PlaylistModel>((playlist) => OpenPopup(playlist, ViewNameEnum.PlaylistPopup));

            Task.Run(Init);
        }

        private async void Init()
        {
            WelcomeMessage = MessageService.GetWelcomeMessage();

            (int numberOfArtists, int numberOfAlbums, int numberOfTracks) = await DataAccess.Connection.GetNumberOfEntries();
            NumberOfArtists = numberOfArtists + " " + Resources.Artists_View;
            NumberOfAlbums = numberOfAlbums + " " + Resources.Albums_View;
            NumberOfTracks = numberOfTracks + " " + Resources.Tracks;

            int playCount = _historyServices.TodayHistory.PlayCount;
            if (playCount > 0)
            {
                string tracks = playCount > 1 ? Resources.Tracks : Resources.Track;
                TodayStats = Resources.Today + ": " + playCount.ToString() + " " + tracks + " - " + _historyServices.TodayListenTime.ToFormattedString(true);
            }
            else
            {
                TodayStats = "";
            }

            GetRecentData(numberOfTracks);

            if (BindedArtists.Count > 0)
            {
                PlayHistory = true;
                GetRadioStations();
            }
        }

        private async void GetRecentData(int totalTrackNumber)
        {
            BindedAlbums = new(await DataAccess.Connection.GetLastPlayedAlbums(TopLastPlayedAlbums));
            BindedArtists = new(await DataAccess.Connection.GetLastPlayedArtists(TopLastPlayedArtists));
        }

        private async void GetRadioStations()
        {
            BindedPlaylists = await _radioStationsServices.CreateRadioStations(4);
        }

        private void Navigate(ViewNameEnum viewNameEnum, BaseModel musicModel = null)
        {
            _navigationService.NavigateTo(viewNameEnum, musicModel);
        }

        private void OpenPopup(BaseModel musicModel, ViewNameEnum viewNameEnum)
        {
            _navigationService.OpenPopup(viewNameEnum, musicModel);
        }

        // Play methods
        private async void PlayArtist(ArtistModel artist)
        {
            _queueService.SetNewQueue(await ArtistServices.GetArtistTracks(artist.Id), new(artist.Name, ModelTypeEnum.Artist, artist.Id), artist.Cover, null, false, false, false);
        }
        private async void PlayAlbum(AlbumModel album)
        {
            List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbum(album.Id);
            _queueService.SetNewQueue(tracks, new(album.Name, ModelTypeEnum.Album, album.Id), album.AlbumCover, null, false, false, true);
        }
        private void PlayPlaylist(PlaylistModel playlist)
        {
            _queueService.SetNewQueue(playlist.Tracks.ToTrackModel(), new(playlist.Name, ModelTypeEnum.Playlist, playlist.Id), playlist.Cover, null);
        }
    }
}
