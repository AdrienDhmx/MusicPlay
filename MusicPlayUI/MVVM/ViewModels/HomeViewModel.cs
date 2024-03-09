using MusicPlay.Language;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using System.Collections.ObjectModel;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Helpers;
using DynamicScrollViewer;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        private readonly IQueueService _queueService;
        private readonly IRadioStationsService _radioStationsServices;
        private readonly IHistoryServices _historyServices;
        private readonly ICommandsManager _commandsManager;
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

        private ObservableCollection<Artist> _bindedArtists = new();
        public ObservableCollection<Artist> BindedArtists
        {
            get { return _bindedArtists; }
            set
            {
                _bindedArtists = value;
                OnPropertyChanged(nameof(BindedArtists));
            }
        }

        private ObservableCollection<Album> _bindedAlbums = new();
        public ObservableCollection<Album> BindedAlbums
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

        private List<Playlist> _bindedPlaylists;
        public List<Playlist> BindedPlaylists
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
        public HomeViewModel(IQueueService queueService, IRadioStationsService radioStationsServices, IHistoryServices historyServices, ICommandsManager commandsManager)
        {
            _queueService = queueService;
            _radioStationsServices = radioStationsServices;
            _historyServices = historyServices;
            _commandsManager = commandsManager;

            // Cards navigation Commands
            NavigateToArtistsCommand = new RelayCommand(() => App.State.NavigateTo<ArtistLibraryViewModel>());
            NavigateToAlbumsCommand = new RelayCommand(() => App.State.NavigateTo<AlbumLibraryViewModel>());

            // navigation commands
            NavigateToArtistCommand = _commandsManager.NavigateToArtistCommand;
            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumCommand;
            NavigateToPlaylistCommand = _commandsManager.NavigateToPlaylistCommand;

            // play commands
            PlayArtistCommand = _commandsManager.PlayNewQueueCommand;
            PlayAlbumCommand = _commandsManager.PlayNewQueueCommand;
            PlayPlaylistCommand = _commandsManager.PlayNewQueueCommand;

            // open popup commands
            OpenArtistPopupCommand = _commandsManager.OpenArtistPopupCommand;
            OpenAlbumPopupCommand = _commandsManager.OpenAlbumPopupCommand;
            OpenPlaylistPopupCommand = _commandsManager.OpenPlaylistPopupCommand;
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            AppBar.AnimateElevation(e.VerticalOffset, true);
            base.OnScrollEvent(e);
        }

        public override void UpdateAppBarStyle()
        {
            if (AppBar is not null)
            {
                AppBar.Reset();
                AppBar.ApplyDropShadow = false;
                AppBar.BackgroundOpacity = 0;
                AppBar.ContentOpacity = 0;
                AppBar.Title = WelcomeMessage;
            }
        }

        public override void Init()
        {
            WelcomeMessage = MessageService.GetWelcomeMessage();

            base.Init();

            NumberOfArtists = Artist.Count() + " " + Resources.Artists_View;
            NumberOfAlbums = Album.Count() + " " + Resources.Albums_View;
            NumberOfTracks = Track.Count() + " " + Resources.Tracks;

            int playCount = 0; //_historyServices.TodayHistory.PlayCount;
            if (playCount > 0)
            {
                string tracks = playCount > 1 ? Resources.Tracks : Resources.Track;
                TodayStats = Resources.Today + ": " + playCount.ToString() + " " + tracks + " - " + _historyServices.TodayListenTime.ToFullString();
            }
            else
            {
                TodayStats = "";
            }

            GetRecentData();

            if (BindedArtists.Count > 0)
            {
                PlayHistory = true;
                //GetRadioStations();
            }
        }

        private void GetRecentData()
        {
            BindedAlbums = new(Album.GetLastPlayed(TopLastPlayedAlbums));
            BindedArtists = new(Artist.GetLastPlayed(TopLastPlayedArtists));
        }

        private async void GetRadioStations()
        {
            BindedPlaylists = await _radioStationsServices.CreateRadioStations(4);
        }
    }
}
