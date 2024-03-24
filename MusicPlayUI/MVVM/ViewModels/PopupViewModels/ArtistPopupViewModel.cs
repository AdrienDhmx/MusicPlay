
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Collections.ObjectModel;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;
using MusicFilesProcessor.Helpers;
using MusicPlayUI.Core.Factories;


namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class ArtistPopupViewModel : TagTargetPopupViewModel
    {
        private readonly IQueueService _queueService;
        private readonly IPlaylistService _playlistService;
        private readonly IWindowService _windowService;
        private Artist _artist;
        public Artist Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        private ObservableCollection<Playlist> _playlists;
        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        public ICommand PlayNextCommand { get; }
        public ICommand AddToQueueCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand AddToTagCommand { get; }
        public ICommand ChangeCoverCommand { get; }
        public ICommand CreatePlaylistCommand { get; }
        public ICommand RemoveArtistGenreCommand { get; }
        public ICommand OpenEditArtistWindow { get; }
        public ICommand CreateTagCommand { get; }
        public ArtistPopupViewModel(IQueueService queueService, IModalService modalService, IPlaylistService playlistService, IWindowService windowService) : base(modalService)
        {
            _queueService = queueService;
            _playlistService = playlistService;
            _windowService = windowService;

            PlayNextCommand = new RelayCommand(() => PlayNext());
            AddToQueueCommand = new RelayCommand(() => PlayNext(true));
            AddToPlaylistCommand = new RelayCommand<Playlist>((playlist) => AddToPlaylist(playlist));
            ChangeCoverCommand = new RelayCommand(async () => await ChangeCover());
            CreatePlaylistCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistClosed));
            AddToTagCommand = new RelayCommand<Tag>(async (tag) => await AddToTag(tag, Artist));
            CreateTagCommand = new RelayCommand(() => CreateTag(Artist));
            OpenEditArtistWindow = new RelayCommand(() => {
                ClosePopup();
                _windowService.OpenWindow(ViewNameEnum.ArtistProperties, Artist);
            });
        }

        private void PlayNext(bool end = false)
        {
            _queueService.AddTracks(Artist.GetAllArtistTracks(), end, false, Artist.Name);
            ClosePopup();
        }

        private async void AddToPlaylist(Playlist playlist)
        {
            int tracksAdded = await Playlist.AddTracks(playlist, Artist.GetAllArtistTracks());
            MessageFactory.TracksAddedToPlaylist(playlist.Name, tracksAdded).PublishWithAppDispatcher();
            Playlists.Remove(playlist);
        }

        private async void OnCreatePlaylistClosed(bool isCanceled)
        {
            if (!_modalService.IsModalOpen && !isCanceled)
            {
                await Task.Delay(500);

                var playlists = await Playlist.GetAll();
                playlists.ToList().Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));
                Playlist createdPlaylist = playlists.LastOrDefault();
                AddToPlaylist(createdPlaylist);
            }
        }

        private async Task ChangeCover()
        {
            ClosePopup();
            bool result = await Artist.ChangeCover();
            if (result)
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            App.State.UpdateCurrentViewIfIs([typeof(ArtistLibraryViewModel), typeof(ArtistViewModel)]);
        }

        public override void Init()
        {
            LoadData();
        }

        private async void LoadData()
        {
            Artist = (Artist)State.Parameter;
            //Artist.TrackTag = await DataAccess.Connection.GetArtistTag(Artist.Id);

            Playlists = new(await Playlist.GetAll());
            GetTags(Artist.ArtistTags.Select(g => g.TagId));
        }
    }
}
