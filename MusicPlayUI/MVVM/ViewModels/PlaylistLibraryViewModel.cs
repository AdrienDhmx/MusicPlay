using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlayModels.Enums;
using MusicPlayUI.Core.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using System.Collections.ObjectModel;
using MusicPlayModels;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Threading.Tasks;
using System.Linq;
using MessageControl;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class PlaylistLibraryViewModel : ViewModel, IFileDragDropTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private readonly IRadioStationsService _radioStationsServices;
        
        private readonly IPlaylistService _playlistService;

        private List<PlaylistModel> _constAutoPlaylist = new();
        public List<PlaylistModel> ConstAutoPlaylist
        {
            get { return _constAutoPlaylist; }
            set
            {
                _constAutoPlaylist = value;
                OnPropertyChanged(nameof(ConstAutoPlaylist));
            }
        }

        private ObservableCollection<PlaylistModel> _bindedPlaylists = new ();
        public ObservableCollection<PlaylistModel> BindedPlaylists
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
        public PlaylistLibraryViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, 
            IRadioStationsService radioStationsServices, IPlaylistService playlistService)
        {
            _navigationService = navigationService;
            _queueService = queueService;
            _modalService = modalService;
            _radioStationsServices = radioStationsServices;
            
            _playlistService = playlistService;

            LoadData();

            CreatePlaylistCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(ViewNameEnum.CreatePlaylist, PlaylistCreated);
            });

            PlayPlaylistCommand = new RelayCommand<PlaylistModel>(async (playlist) =>
            {
                if (playlist is not null)
                {
                    if (playlist.PlaylistType == PlaylistTypeEnum.UserPlaylist)
                    {
                        List<OrderedTrackModel> tracks = await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id);
                        _queueService.SetNewQueue(tracks.ToTrackModel(), playlist.Name, ModelTypeEnum.Playlist, playlist.Cover, null, false, false, false);
                    }
                    else
                    {
                        _queueService.SetNewQueue(playlist.Tracks.ToTrackModel(), playlist.Name, ModelTypeEnum.Playlist, playlist.Cover, null, false, false, false);
                    }
                }
            });

            NavigateToPlaylistCommand = new RelayCommand<PlaylistModel>((playlist) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificPlaylist, playlist);
            });

            CreateAutoPlaylistCommand = new RelayCommand(async () =>
            {
                PlaylistModel playlist = await _radioStationsServices.CreateRadioStation();

                if(playlist is not null)
                    _navigationService.NavigateTo(ViewNameEnum.SpecificPlaylist, playlist);
                else // return null when not enough data
                    MessageHelper.PublishMessage(MessageFactory.ErrorMessage(ErrorEnum.NotEnoughDataForRadio));
            });

            OpenPlaylistPopupCommand = new RelayCommand<PlaylistModel>((playlist) =>
            {
                if (playlist is not null)
                {
                    _navigationService.OpenPopup(ViewNameEnum.PlaylistPopup, playlist);
                }
            });
        }
        
        private async void Search()
        {
            BindedPlaylists = new(await SearchHelper.FilterPlaylist(SearchText));
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
            List<PlaylistModel> playlists = await DataAccess.Connection.GetAllPlaylists();
            _totalPlaylistCount = playlists.Count;
            Search();
        }

        private async void LoadData()
        {
            ConstAutoPlaylist = await PlaylistsFactory.GetConstAutoPlaylists();
            BindedPlaylists = new(await DataAccess.Connection.GetAllPlaylists());
            _totalPlaylistCount = BindedPlaylists.Count;
            PlaylistCount = $"{BindedPlaylists.Count} of {_totalPlaylistCount}";
        }

        public async void OnFileDrop(string[] filepaths)
        {
            FilesDropped = true;
            await _playlistService.CreatePlaylistfromDirectory(filepaths);
            FilesDropped = false;
        }
    }
}
