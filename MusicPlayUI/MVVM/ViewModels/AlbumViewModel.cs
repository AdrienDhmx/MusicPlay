using System.Windows.Input;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlayUI.Core.Services;
using DynamicScrollViewer;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class AlbumViewModel : ViewModel
    {
        private readonly ICommandsManager _commandsManager;
        public IQueueService QueueService { get; }

        private Album _album;
        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        public ICommand PlayAlbumCommand { get; }
        public ICommand PlayShuffledAlbumCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand NavigateToArtistByIdCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateToGenreCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand PlayArtistCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public AlbumViewModel(IQueueService queueService, ICommandsManager commandsManager)
        {
            this.QueueService = queueService;
            _commandsManager = commandsManager;

            // play
            PlayAlbumCommand = new RelayCommand(() =>
                this.QueueService.SetNewQueue(Album.Tracks, Album, Album.Name, Album.AlbumCover));
            PlayShuffledAlbumCommand = new RelayCommand(() =>
                this.QueueService.SetNewQueue(Album.Tracks, Album, Album.Name, Album.AlbumCover, isShuffled: true));
            PlayTrackCommand = new RelayCommand<OrderedTrack>((track) =>
                this.QueueService.SetNewQueue(Album.Tracks, Album, Album.Name, Album.AlbumCover, track.Track, false));
            PlayArtistCommand = new RelayCommand<Artist>((artist) =>
                this.QueueService.SetNewQueue(artist.Tracks, artist, artist.Name, artist.Cover));

            // navigate
            NavigateToArtistCommand = _commandsManager.NavigateToArtistCommand;
            NavigateToArtistByIdCommand = _commandsManager.NavigateToArtistByIdCommand;
            NavigateToAlbumByIdCommand = _commandsManager.NavigateToAlbumByIdCommand;
            NavigateBackCommand = _commandsManager.NavigateBackCommand;
            NavigateToGenreCommand = _commandsManager.NavigateToGenreCommand;

            // open popup
            OpenTrackPopupCommand = _commandsManager.OpenTrackPopupCommand;
            OpenAlbumPopupCommand = _commandsManager.OpenAlbumPopupCommand;
            OpenArtistPopupCommand = _commandsManager.OpenArtistPopupCommand;
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            AppBar.AnimateElevation(e.VerticalOffset);
            base.OnScrollEvent(e);
        }

        public override void UpdateAppBarStyle()
        {
            AppBar.SetStyle(AppTheme.Palette.PrimaryContainer, 0, 0, false);
            AppBar.SetForeground(AppTheme.Palette.OnPrimaryContainer);
        }

        public override void Init()
        {
            base.Init();
            Update();
        }

        public override void Update(BaseModel baseModel = null)
        {
            if (baseModel is null || baseModel is not MusicPlay.Database.Models.Album)
            {
                Album = (Album)State.Parameter;
            }
            else
            {
                Album = (Album)baseModel;
            }

            AppBar.SetData(Album.Name, Album.PrimaryArtist.Name);
        }
    }
}
