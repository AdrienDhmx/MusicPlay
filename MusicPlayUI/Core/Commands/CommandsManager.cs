using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using AudioHandler;
using MessageControl;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.PopupViewModels;

namespace MusicPlayUI.Core.Commands
{
    public class CommandsManager : ICommandsManager
    {
        private readonly IAudioPlayback _audioPlayback;
        private readonly IQueueService _queueService;
        private readonly IAudioTimeService _audioTimeService;

        // audio
        public ICommand PlayPauseCommand { get; }
        public ICommand NextTrackCommand { get; }
        public ICommand PreviousTrackCommand { get; }
        public ICommand DecreaseVolumeCommand { get; }
        public ICommand IncreaseVolumeCommand { get; }
        public ICommand MuteVolumeCommand { get; }

        // queue
        public ICommand ShuffleCommand { get; }
        public ICommand RepeatCommand { get; }
        public ICommand ToggleQueueDrawerCommand { get; }
        public ICommand PlayNewQueueCommand { get; }
        public ICommand PlayNewQueueShuffledCommand { get; }

        // navigation
        public ICommand NavigateCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateForwardCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public ICommand NavigateToArtistByIdCommand { get; }
        public ICommand NavigateToGenreCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToPlaylistCommand { get; }

        // popup
        public ICommand OpenAlbumPopupCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand OpenPlaylistPopupCommand { get; }
        public ICommand OpenTagPopupCommand { get; }
        public ICommand ClosePopupCommand { get; }

        // settings
        public ICommand ToggleThemeCommand { get; }

        //Covers
        public ICommand UpdateAlbumCover { get; }

        // others
        public ICommand FavoriteCommand { get; }
        public ICommand RatingCommand { get; }
        public ICommand ToggleMenuDrawerCommand { get; }
        public ICommand ToggleFullScreenCommand { get; }
        public ICommand EscapeFullScreenCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand LeaveCommand { get; }

        public CommandsManager(IAudioPlayback audioPlayback, IQueueService queueService, IAudioTimeService audioTimeService)
        {
            _audioPlayback = audioPlayback;
            _queueService = queueService;
            _audioTimeService = audioTimeService;

            PlayPauseCommand = new RelayCommand(_audioTimeService.PlayPause);

            NextTrackCommand = new RelayCommand(() =>
            {
                _queueService.NextTrack();
            });

            PreviousTrackCommand = new RelayCommand(() =>
            {
                _queueService.PreviousTrack();
            });

            DecreaseVolumeCommand = new RelayCommand<int>((int value) => _audioPlayback.DecreaseVolume(value));

            IncreaseVolumeCommand = new RelayCommand<int>((int value) => _audioPlayback.IncreaseVolume(value));

            MuteVolumeCommand = new RelayCommand(_audioPlayback.Mute);

            ShuffleCommand = new RelayCommand(() =>
            {
                Task.Run(queueService.Shuffle);
            });

            RepeatCommand = new RelayCommand(() =>
            {
                if (_audioPlayback.IsLooping)
                {
                    _audioTimeService.Loop(); // unloop
                }
                else
                {
                    if (queueService.Queue.IsOnRepeat)
                    {
                        _audioTimeService.Loop(); // loop
                    }
                    queueService.Repeat(); // repeat on/off
                }
            });

            PlayNewQueueCommand = new RelayCommand<PlayableModel>(model => SetNewQueue(model));

            PlayNewQueueShuffledCommand = new RelayCommand<PlayableModel>(model => SetNewQueue(model, true));

            FavoriteCommand = new RelayCommand(() =>
            {
                queueService.UpdateFavorite(!_queueService.Queue.PlayingTrack.IsFavorite);
            });

            RatingCommand = new RelayCommand<string>((value) =>
            {
                if (int.TryParse(value, out var rating))
                    queueService.UpdateRating(rating);
            });

            NavigateCommand = new RelayCommand<Type>(type =>
            {
                App.State.NavigateTo(type);
            });

            NavigateBackCommand = new RelayCommand(App.State.NavigateBack);
            NavigateForwardCommand = new RelayCommand(App.State.NavigateForward);

            NavigateToAlbumByIdCommand = new RelayCommand<int>(async (int id) =>
            {
                App.State.NavigateTo<AlbumViewModel>(await Album.Get(id));
            });

            NavigateToArtistByIdCommand = new RelayCommand<int>(async (int id) =>
            {
                App.State.NavigateTo<ArtistViewModel>(await Artist.Get(id));
            });

            NavigateToGenreCommand = new RelayCommand<Tag>((genre) =>
            {
                App.State.NavigateTo<GenreViewModel>(genre);
            });

            NavigateToAlbumCommand = new RelayCommand<Album>((album) =>
            {
                App.State.NavigateTo<AlbumViewModel>(album);
            });

            NavigateToArtistCommand = new RelayCommand<Artist>((artist) =>
            {
                App.State.NavigateTo<ArtistViewModel>(artist);
            });

            NavigateToPlaylistCommand = new RelayCommand<Playlist>((playlist) =>
            {
                App.State.NavigateTo<PlaylistViewModel>(playlist);
            });

            OpenAlbumPopupCommand = new RelayCommand<Album>(App.State.OpenPopup<AlbumPopupViewModel>);
            OpenTrackPopupCommand = new RelayCommand<Track>(App.State.OpenPopup<TrackPopupViewModel>);
            OpenArtistPopupCommand = new RelayCommand<Artist>(App.State.OpenPopup<ArtistPopupViewModel>);
            OpenPlaylistPopupCommand = new RelayCommand<Playlist>(App.State.OpenPopup<PlaylistPopupViewModel>);
            OpenTagPopupCommand = new RelayCommand<UITagModel>(App.State.OpenPopup<TagPopupViewModel>);

            ToggleMenuDrawerCommand = new RelayCommand(App.State.ToggleMenuDrawer);

            ToggleQueueDrawerCommand = new RelayCommand(App.State.ToggleQueueDrawer);

            ToggleFullScreenCommand = new RelayCommand(App.State.ToggleFullScreen);

            EscapeFullScreenCommand = new RelayCommand(() =>
            {
                if (App.State.IsFullScreen)
                    App.State.ToggleFullScreen();
            });

            ClosePopupCommand = new RelayCommand(App.State.ClosePopup);

            UpdateAlbumCover = new RelayCommand<Album>(async (album) =>
            {
                if (await CoverService.ChangeCover(album))
                {
                    App.State.CurrentView.ViewModel.Update();
                }
            });

            MinimizeCommand = new RelayCommand(() =>
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            });

            MaximizeCommand = new RelayCommand(() =>
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Normal)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                }
            });

            LeaveCommand = new RelayCommand(() =>
            {
                App.Current.Shutdown();
            });

            ToggleThemeCommand = new RelayCommand(() =>
            {
                static void ChangeTheme(bool changeTheme)
                {
                    if (changeTheme)
                    {
                        if (AppTheme.IsSystemSync)
                        {
                            ConfigurationService.SetPreference(SettingsEnum.SystemSyncTheme, "0");
                        }
                        else if (AppTheme.IsSunsetSunrise)
                        {
                            ConfigurationService.SetPreference(SettingsEnum.SunsetSunrise, "0");
                        }

                        // invert the theme in the config
                        ConfigurationService.SetPreference(SettingsEnum.LightTheme, AppTheme.IsLightTheme ? "0" : "1");

                        // reload app theme
                        AppTheme.InitializeAppTheme();
                    }
                }

                if (AppTheme.IsSystemSync)
                {
                    MessageHelper.PublishMessage(MessageFactory.CreateWraningMessageWithConfirmAction("The theme is based on the system theme. Changing it will disable this option.", 0, ChangeTheme, "Change Theme"));
                } 
                else if (AppTheme.IsSunsetSunrise)
                {
                    MessageHelper.PublishMessage(MessageFactory.CreateWraningMessageWithConfirmAction("The theme is based on the time of day. Changing it will disable this option.", 0, ChangeTheme, "Change Theme"));
                } else
                {
                    ChangeTheme(true);
                }
            });
        }

        private async void SetNewQueue(PlayableModel model, bool shuffled = false)
        {
            if (model.IsNull())
                return;

            await Task.Run(() =>
            {
                if (model is Artist artist)
                {
                    _queueService.SetNewQueue(artist.Tracks, artist, artist.Name, artist.Cover, null, shuffled);
                }
                else if (model is Album album)
                {
                    _queueService.SetNewQueue(album.Tracks, album, album.Name, album.AlbumCover, null, shuffled);
                }
                else if (model is Playlist playlist)
                {
                    _queueService.SetNewQueue(playlist.Tracks, playlist, playlist.Name, playlist.Cover, null, shuffled);
                }
                else if (model is Tag tag)
                {
                    _queueService.SetNewQueue(tag.Tracks, tag, tag.Name, "", null, shuffled);
                }
            });
        }

        public ICommand GetCommand(CommandEnums commandEnums)
        {
            return commandEnums switch
            {
                CommandEnums.PlayPause => PlayPauseCommand,
                CommandEnums.NexTrack => NextTrackCommand,
                CommandEnums.PreviousTrack => PreviousTrackCommand,
                CommandEnums.Shuffle => ShuffleCommand,
                CommandEnums.Repeat => RepeatCommand,
                CommandEnums.DecreaseVolume => DecreaseVolumeCommand,
                CommandEnums.IncreaseVolume => IncreaseVolumeCommand,
                CommandEnums.MuteVolume => MuteVolumeCommand,
                CommandEnums.ToggleFavorite => FavoriteCommand,
                CommandEnums.Rating0 => RatingCommand,
                CommandEnums.Rating1 => RatingCommand,
                CommandEnums.Rating2 => RatingCommand,
                CommandEnums.Rating3 => RatingCommand,
                CommandEnums.Rating4 => RatingCommand,
                CommandEnums.Rating5 => RatingCommand,
                CommandEnums.Home => NavigateCommand,
                CommandEnums.Albums => NavigateCommand,
                CommandEnums.Artists => NavigateCommand,
                CommandEnums.Playlists => NavigateCommand,
                CommandEnums.NowPlaying => NavigateCommand,
                CommandEnums.NavigateToAlbumById => NavigateToAlbumByIdCommand,
                CommandEnums.NavigateToArtistById => NavigateToArtistByIdCommand,
                CommandEnums.NavigateToGenre => NavigateToGenreCommand,
                CommandEnums.Settings => NavigateCommand,
                CommandEnums.NavigateBack => NavigateBackCommand,
                CommandEnums.EscapeFullScreen => EscapeFullScreenCommand,
                CommandEnums.ToggleFullScreen => ToggleFullScreenCommand,
                CommandEnums.ToggleQueueDrawer => ToggleQueueDrawerCommand,
                CommandEnums.ToggleTheme => ToggleThemeCommand,
                _ => null,
            };
        }
    }
}
