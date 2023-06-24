using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AudioHandler;
using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.Core.Commands
{
    public class CommandsManager : ICommandsManager
    {
        private readonly IAudioPlayback _audioPlayback;
        private readonly IQueueService _queueService;
        private readonly INavigationService _navigationService;
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

        // navigation
        public ICommand NavigateCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public ICommand NavigateToArtistByIdCommand { get; }
        public ICommand NavigateToGenreCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand NavigateToArtistCommand { get; }

        // popup
        public ICommand OpenAlbumPopupCommand { get;}
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand ClosePopupCommand { get; }

        //Covers
        public ICommand UpdateAlbumCover { get; }

        // others
        public ICommand FavoriteCommand { get; }
        public ICommand RatingCommand { get; }
        public ICommand ToggleFullScreenCommand { get; }
        public ICommand EscapeFullScreenCommand { get; }
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

            DecreaseVolumeCommand = new RelayCommand(_audioPlayback.DecreaseVolume);

            IncreaseVolumeCommand = new RelayCommand(_audioPlayback.IncreaseVolume);

            MuteVolumeCommand = new RelayCommand(_audioPlayback.Mute);

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
                queueService.UpdateFavorite(!_queueService.PlayingTrack.IsFavorite);
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

            NavigateBackCommand = new RelayCommand(_navigationService.NavigateBack);

            NavigateToAlbumByIdCommand = new RelayCommand<int>(async (int id) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, await DataAccess.Connection.GetAlbum(id));
            });

            NavigateToArtistByIdCommand = new RelayCommand<int>(async (int id) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, await DataAccess.Connection.GetArtist(id));
            });

            NavigateToGenreCommand = new RelayCommand<GenreModel>((genre) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificGenre, genre);
            });

            NavigateToAlbumCommand = new RelayCommand<AlbumModel>((album) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, album);
            });

            NavigateToArtistCommand = new RelayCommand<ArtistModel>((artist) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, artist);
            });

            OpenAlbumPopupCommand = new RelayCommand<AlbumModel>((album) => _navigationService.OpenPopup(ViewNameEnum.AlbumPopup, album));
            OpenTrackPopupCommand = new RelayCommand<UIOrderedTrackModel>((track) => _navigationService.OpenPopup(ViewNameEnum.TrackPopup, track));
            OpenArtistPopupCommand = new RelayCommand<ArtistModel>((artist) => _navigationService.OpenPopup(ViewNameEnum.ArtistPopup, artist));

            ToggleQueueDrawerCommand = new RelayCommand(_navigationService.ToggleQueueDrawer);

            ToggleFullScreenCommand = new RelayCommand(_navigationService.ToggleFullScreen);

            EscapeFullScreenCommand = new RelayCommand(() =>
            {
                if (_navigationService.IsFullScreen)
                    _navigationService.ToggleFullScreen();
            });

            ClosePopupCommand = new RelayCommand(_navigationService.ClosePopup);

            UpdateAlbumCover = new RelayCommand<AlbumModel>((album) =>
            {
                if (CoverService.ChangeCover(album))
                {
                    if (_navigationService.CurrentViewName == ViewNameEnum.Albums || _navigationService.CurrentViewName == ViewNameEnum.SpecificAlbum)
                    {
                        _navigationService.CurrentViewModel.Update();
                    }
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
                CommandEnums.Import => NavigateCommand,
                CommandEnums.Settings => NavigateCommand,
                CommandEnums.NavigateBack => NavigateBackCommand,
                CommandEnums.EscapeFullScreen => EscapeFullScreenCommand,
                CommandEnums.ToggleFullScreen => ToggleFullScreenCommand,
                _ => null,
            };
        }
    }
}
