using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Collections.ObjectModel;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;
using MusicPlayUI.Core.Factories;


namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class AlbumPopupViewModel : TagTargetPopupViewModel
    {
        private readonly IQueueService _queueService;
        private readonly IPlaylistService _playlistService;
        private readonly ICommandsManager _commandsManager;
        private Album _album;
        public Album Album
        {
            get { return _album; }
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
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
        public ICommand RemoveAlbumTagCommand { get; }
        public ICommand ChangeCoverCommand { get; }
        public ICommand CreatePlaylistCommand { get; }
        public ICommand NavigateToArtistByIdCommand { get; }
        public ICommand CreateTagCommand { get; }
        public AlbumPopupViewModel(IQueueService queueService, IModalService modalService, IPlaylistService playlistService, ICommandsManager commandsManager) : base(modalService)
        {
            _queueService = queueService;
            _playlistService = playlistService;
            _commandsManager = commandsManager;

            PlayNextCommand = new RelayCommand(() => PlayNext());
            AddToQueueCommand = new RelayCommand(() => PlayNext(true));
            AddToPlaylistCommand = new RelayCommand<Playlist>(async (playlist) => await AddToPlaylist(playlist));
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            ChangeCoverCommand = new RelayCommand(async () => await ChangeCover());
            NavigateToArtistByIdCommand = _commandsManager.NavigateToArtistByIdCommand;

            CreateTagCommand = new RelayCommand(() => CreateTag(Album));
            AddToTagCommand = new RelayCommand<Tag>(async (tag) => await AddToTag(tag, Album));
        }

        private void PlayNext(bool end = false)
        {
            _queueService.AddTracks([..Album.Tracks], end, true, Album.Name);
            App.State.ClosePopup();
        }

        private async Task AddToPlaylist(Playlist playlist)
        {
            int tracksAdded = await Playlist.AddTracks(playlist, [.. Album.Tracks]);
            MessageFactory.TracksAddedToPlaylist(playlist.Name, tracksAdded).PublishWithAppDispatcher();
            Playlists.Remove(playlist);
        }

        private void CreatePlaylist()
        {
            _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistClosed);
        }

        private async void OnCreatePlaylistClosed(bool isCanceled)
        {
            if (!_modalService.IsModalOpen && !isCanceled)
            {
                await Task.Delay(500);
                var playlists = await Playlist.GetAll();
                playlists.ToList().OrderBy(p => p.Id);
                Playlist createdPlaylist = playlists.LastOrDefault();
                await AddToPlaylist(createdPlaylist);
            }
        }

        private async Task ChangeCover()
        {
            App.State.ClosePopup();
            bool result = await Album.ChangeCover();
            if (result)
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            App.State.UpdateCurrentViewIfIs([typeof(AlbumLibraryViewModel), typeof(AlbumViewModel)]);
        }

        private async Task GetUserPlaylists()
        {
            Playlists = new(await Playlist.GetAll());
        }

        public override void Init()
        {
            LoadData();
        }

        private async void LoadData()
        {
            Album = (Album)State.Parameter;

            if (App.State.CurrentView.ViewModel is GenreViewModel genreViewModel)
            {
                CanRemoveFromGenre = true;
                CurrentTagView = genreViewModel.Genre;
            }

            //Album.TrackTag = await DataAccess.Connection.GetAlbumTag(Album.Id);

            await GetUserPlaylists();
            GetTags(Album.AlbumTags.Select(g => g.TagId));
        }
    }
}
