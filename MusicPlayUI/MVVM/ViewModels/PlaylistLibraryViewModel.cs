using MusicPlayUI.Core.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using System.Collections.ObjectModel;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Threading.Tasks;
using System.Linq;
using MessageControl;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;
using MusicPlayUI.Core.Services;
using DynamicScrollViewer;
using MusicPlay.Database.Models.DataBaseModels;


namespace MusicPlayUI.MVVM.ViewModels
{
    public class PlaylistLibraryViewModel : LibraryViewModel, IFileDragDropTarget
    {
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private readonly ICommandsManager _commandsManager;
        private readonly IRadioStationsService _radioStationsServices;
        
        private readonly IPlaylistService _playlistService;

        private List<Playlist> _constAutoPlaylist = new();
        public List<Playlist> ConstAutoPlaylist
        {
            get { return _constAutoPlaylist; }
            set
            {
                _constAutoPlaylist = value;
                OnPropertyChanged(nameof(ConstAutoPlaylist));
            }
        }

        private ObservableCollection<Playlist> _playlists = new ();
        public ObservableCollection<Playlist> Playlists
        {
            get { return _playlists; }
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        private bool _filesDropped = false;
        public bool FilesDropped
        {
            get { return _filesDropped; }
            set 
            {
                SetField(ref _filesDropped, value);
            }
        }

        private string _playlistCount;
        public string PlaylistCount
        {
            get { return _playlistCount; }
            set
            {
                SetField(ref _playlistCount, value);
                AppBar.Subtitle = value;
            }
        }

        private int _totalPlaylistCount { get; set; }

        public ICommand CreatePlaylistCommand { get; }
        public ICommand PlayPlaylistCommand { get; }
        public ICommand NavigateToPlaylistCommand { get; }
        public ICommand CreateAutoPlaylistCommand { get; }
        public ICommand OpenPlaylistPopupCommand { get; }
        public PlaylistLibraryViewModel(IQueueService queueService, IModalService modalService, ICommandsManager commandsManager,
            IRadioStationsService radioStationsServices, IPlaylistService playlistService)
        {
            _queueService = queueService;
            _modalService = modalService;
            _commandsManager = commandsManager;
            _radioStationsServices = radioStationsServices;
            
            _playlistService = playlistService;

            CreatePlaylistCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(ViewNameEnum.CreatePlaylist, PlaylistCreated);
            });

            PlayPlaylistCommand = new RelayCommand<Playlist>((playlist) =>
            {
                if (playlist is not null)
                {
                    _queueService.SetNewQueue(playlist.PlaylistTracks, playlist, playlist.Name, playlist.Cover, null, false, false, false);
                }
            });

            NavigateToPlaylistCommand = _commandsManager.NavigateToPlaylistCommand;

            CreateAutoPlaylistCommand = new RelayCommand(async () =>
            {
                Playlist playlist = await _radioStationsServices.CreateRadioStation();

                if(playlist is not null)
                    App.State.NavigateTo<PlaylistViewModel>(playlist);
                else // return null when not enough data
                    MessageHelper.PublishMessage(MessageFactory.ErrorMessage(ErrorEnum.NotEnoughDataForRadio));
            });

            OpenPlaylistPopupCommand = _commandsManager.OpenPlaylistPopupCommand;
        }

        internal async override void FilterSearch()
        {
            Playlists = new(ConstAutoPlaylist);
            foreach(var playlist in await SearchHelper.FilterPlaylist(SearchText))
            {
                Playlists.Add(playlist);
            }
            PlaylistCount = $"{Playlists.Count - ConstAutoPlaylist.Count} of {_totalPlaylistCount}";
        }

        private void PlaylistCreated(bool canceled)
        {
            if (!canceled)
            {
                Update();
            }
        }

        public override void UpdateAppBarStyle()
        {
            AppBar.SetStyle(AppTheme.Palette.Background, 0, 0);
            AppBar.SetForeground(AppTheme.Palette.OnBackground);
        }

        public async override void Update(BaseModel parameter = null)
        {
            // this update method is only called when a playlist has been deleted
            List<Playlist> playlists = await Playlist.GetAll();
            _totalPlaylistCount = playlists.Count;
            FilterSearch();
        }

        public async override void Init()
        {
            AppBar.Title = "My Playlists";
            UpdateAppBarStyle();

            ConstAutoPlaylist = PlaylistsFactory.GetConstAutoPlaylists();
            Playlists = new(ConstAutoPlaylist);

            foreach (Playlist playlist in await Playlist.GetAll())
            {
                Playlists.Add(playlist);
            }

            _totalPlaylistCount = Playlists.Count - ConstAutoPlaylist.Count; // remove the 4 constante playlists
            PlaylistCount = $"{Playlists.Count - ConstAutoPlaylist.Count} of {_totalPlaylistCount}";
            IsLoading = false;
            base.Init();
        }

        public async void OnFileDrop(string[] filePaths)
        {
            FilesDropped = true;
            await _playlistService.CreatePlaylistFromDirectory(filePaths);
            FilesDropped = false;
        }
    }
}
