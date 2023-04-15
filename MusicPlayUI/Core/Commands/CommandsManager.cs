using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioEngine;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using static MusicPlayUI.Core.Commands.ShortcutsManager;

namespace MusicPlayUI.Core.Commands
{
    public class CommandsManager : ICommandsManager
    {
        private readonly IAudioPlayback _audioPlayback;
        private readonly IQueueService _queueService;
        private readonly INavigationService _navigationService;
        private readonly IAudioTimeService _audioTimeService;

        public ICommand PlayPauseCommand { get; }
        public ICommand NextTrackCommand { get; }
        public ICommand PreviousTrackCommand { get; }
        public ICommand DecreaseVolumeCommand { get; }
        public ICommand IncreaseVolumeCommand { get; }
        public ICommand MuteVolumeCommand { get; }
        public ICommand ToggleFullScreenCommand { get; }
        public ICommand EscapeFullScreenCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand OpenCloseMenuCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand RepeatCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand FavoriteCommand { get; }
        public ICommand RatingCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand LeaveCommand { get; }
        public CommandsManager(IAudioPlayback audioPlayback, IQueueService queueService, INavigationService navigationService, IAudioTimeService audioTimeService)
        {
            _audioPlayback = audioPlayback;
            _queueService = queueService;
            _navigationService = navigationService;
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

            DecreaseVolumeCommand = new RelayCommand(() =>
            {
                _audioPlayback.DecreaseVolume();
            });

            IncreaseVolumeCommand = new RelayCommand(() =>
            {
                _audioPlayback.IncreaseVolume();
            });

            MuteVolumeCommand = new RelayCommand(() =>
            {
                _audioPlayback.Mute();
            });

            ShuffleCommand = new RelayCommand(() =>
            {
                Task.Run(queueService.Shuffle);
            });

            RepeatCommand = new RelayCommand(() =>
            {
                if (_audioTimeService.IsLooping)
                {
                    _audioTimeService.Loop(); // unloop
                }
                else
                {
                    if (queueService.IsOnRepeat)
                    {
                        _audioTimeService.Loop(); // loop
                    }
                    queueService.Repeat(); // repeat on/off
                }
            });

            FavoriteCommand = new RelayCommand(() =>
            {
                queueService.UpdateFavorite(!queueService.PlayingTrack.IsFavorite); // invert favorite value of the playing track
            });

            RatingCommand = new RelayCommand<string>((value) =>
            {
                if (int.TryParse(value, out var rating))
                    queueService.UpdateRating(rating);
            });

            NavigateCommand = new RelayCommand<ViewNameEnum>((view) =>
            {
                _navigationService.NavigateTo(view);
            });

            ToggleFullScreenCommand = new RelayCommand(() =>
            {
                navigationService.SwitchFullScreen();
            });

            EscapeFullScreenCommand = new RelayCommand(() =>
            {
                if (_navigationService.IsFullScreen)
                    navigationService.SwitchFullScreen();
            });

            ClosePopupCommand = new RelayCommand(_navigationService.ClosePopup);

            MinimizeCommand = new RelayCommand(() =>
            {
                App.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
            });

            MaximizeCommand = new RelayCommand(() =>
            {
                if (App.Current.MainWindow.WindowState == System.Windows.WindowState.Normal)
                {
                    App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
                }
                else
                {
                    App.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
                }
            });

            LeaveCommand = new RelayCommand(() =>
            {
                App.Current.Shutdown();
            });
        }

        public ICommand GetCommand(CommandEnums commandEnums)
        {
            switch (commandEnums)
            {
                case CommandEnums.PlayPause:
                    return PlayPauseCommand;
                case CommandEnums.NexTrack:
                    return NextTrackCommand;
                case CommandEnums.PreviousTrack:
                    return PreviousTrackCommand;
                case CommandEnums.Shuffle:
                    return ShuffleCommand;
                case CommandEnums.Repeat:
                    return RepeatCommand;
                case CommandEnums.DecreaseVolume:
                    return DecreaseVolumeCommand;
                case CommandEnums.IncreaseVolume:
                    return IncreaseVolumeCommand;
                case CommandEnums.MuteVolume:
                    return MuteVolumeCommand;
                case CommandEnums.ToggleFavorite:
                    return FavoriteCommand;
                case CommandEnums.Rating0:
                    return RatingCommand;
                case CommandEnums.Rating1:
                    return RatingCommand;
                case CommandEnums.Rating2:
                    return RatingCommand;
                case CommandEnums.Rating3:
                    return RatingCommand;
                case CommandEnums.Rating4:
                    return RatingCommand;
                case CommandEnums.Rating5:
                    return RatingCommand;
                case CommandEnums.Home:
                    return NavigateCommand;
                case CommandEnums.Albums:
                    return NavigateCommand;
                case CommandEnums.Artists:
                    return NavigateCommand;
                case CommandEnums.Playlists:
                    return NavigateCommand;
                case CommandEnums.NowPlaying:
                    return NavigateCommand;
                case CommandEnums.Import:
                    return NavigateCommand;
                case CommandEnums.Settings:
                    return NavigateCommand;
                case CommandEnums.EscapeFullScreen:
                    return EscapeFullScreenCommand;
                case CommandEnums.ToggleFullScreen:
                    return ToggleFullScreenCommand;
                default:
                    return null;
            }
        }
    }
}
