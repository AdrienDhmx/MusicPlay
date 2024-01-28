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


namespace MusicPlayUI.MVVM.ViewModels
{
    public class PlaylistLibraryViewModel : ViewModel, IFileDragDropTarget
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

        private ObservableCollection<Playlist> _bindedPlaylists = new ();
        public ObservableCollection<Playlist> BindedPlaylists
        {
            get { return _bindedPlaylists; }
            set
            {
                _bindedPlaylists = value;
                OnPropertyChanged(nameof(BindedPlaylists));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetField(ref _searchText, value);
                Task.Run(Search);
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

            LoadData();

            CreatePlaylistCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(ViewNameEnum.CreatePlaylist, PlaylistCreated);
            });

            PlayPlaylistCommand = new RelayCommand<Playlist>(async (playlist) =>
            {
                if (playlist is not null)
                {
                    if (playlist.PlaylistType == PlaylistTypeEnum.UserPlaylist)
                    {
                        //List<OrderedTrack> tracks = playlist.Tracks;
                        //_queueService.SetNewQueue(tracks.ToTrackModel(), new(playlist.Name, ModelTypeEnum.Playlist, playlist.Id), playlist.Cover, null, false, false, false);
                    }
                    else
                    {
                        //_queueService.SetNewQueue(playlist.Tracks.ToTrackModel(), new(playlist.Name, ModelTypeEnum.Playlist, playlist.Id), playlist.Cover, null, false, false, false);
                    }
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

        private async void Search()
        {
            BindedPlaylists = new ObservableCollection<Playlist>(await SearchHelper.FilterPlaylist(SearchText));
            PlaylistCount = $"{BindedPlaylists.Count} of {_totalPlaylistCount}";
        }

        private void PlaylistCreated(bool canceled)
        {
            if (!canceled)
            {
                Update();
            }
        }

        public async override void Update(BaseModel parameter = null)
        {
            // this update method is only called when a playlist has been deleted
            List<Playlist> playlists = await Playlist.GetAll();
            _totalPlaylistCount = playlists.Count;
            Search();
        }

        private async void LoadData()
        {
            ConstAutoPlaylist = await PlaylistsFactory.GetConstAutoPlaylists();
            BindedPlaylists = new(await Playlist.GetAll());
            _totalPlaylistCount = BindedPlaylists.Count;
            PlaylistCount = $"{BindedPlaylists.Count} of {_totalPlaylistCount}";
        }

        public async void OnFileDrop(string[] filePaths)
        {
            FilesDropped = true;
            await _playlistService.CreatePlaylistFromDirectory(filePaths);
            FilesDropped = false;
        }
    }
}
