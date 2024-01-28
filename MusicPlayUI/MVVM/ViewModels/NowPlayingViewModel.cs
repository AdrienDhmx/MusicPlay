﻿using AudioHandler;
using MusicFilesProcessor;
using SpectrumVisualizer.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using MusicPlayUI.Core.Commands;
using MusicFilesProcessor.Helpers;
using MusicPlayUI.MVVM.ViewModels.PlayerControlViewModels;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using System.Windows;
using MessageControl;

using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class NowPlayingViewModel : ViewModel
    {
        private readonly IAudioPlayback _audioPlayback;
        private readonly IWindowService _windowService;
        private readonly ICommandsManager _commandsManager;
        private IQueueService _queueService;
        public IQueueService QueueService
        {
            get { return _queueService; }
            private set 
            { 
                SetField(ref _queueService, value);
            }
        }

        public static IAppState AppState
        {
            get => App.State;
        }

        private IVisualizerParameterStore _visualizerParameterService;
        public IVisualizerParameterStore VisualizerParameterService
        {
            get => _visualizerParameterService;
            set { SetField(ref _visualizerParameterService, value); }
        }

        private NowPlayingPlayerControlViewModel _playerViewModel;
        public NowPlayingPlayerControlViewModel PlayerViewModel
        {
            get { return _playerViewModel; }
            set
            {
                _playerViewModel = value;
                OnPropertyChanged(nameof(PlayerViewModel));
            }
        }

        private bool _spectrumvisibility = ConfigurationService.GetPreference(SettingsEnum.VisualizerVisibility) == 1;
        public bool SpectrumVisibility
        {
            get { return _spectrumvisibility; }
            set
            {
                _spectrumvisibility = value;
                OnPropertyChanged(nameof(SpectrumVisibility));
                ConfigurationService.SetPreference(SettingsEnum.VisualizerVisibility, (value ? 1 : 0).ToString());
            }
        }

        private bool _isFullScreen;
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                _isFullScreen = value;
                OnPropertyChanged(nameof(IsFullScreen));
            }
        }

        private int _stream = 0;
        public int Stream
        {
            get { return _stream; }
            set
            {
                _stream = value;
                OnPropertyChanged(nameof(Stream));
            }
        }

        private string _selectedCover = "";
        public string SelectedCover
        {
            get { return _selectedCover; }
            set
            {
                SetField(ref _selectedCover, value);

            }
        }

        private int _selectedCoverIndex = 0;
        public int SelectedCoverIndex
        {
            get => _selectedCoverIndex;
            set
            {
                SetField(ref _selectedCoverIndex, value);
            }
        }

        private List<string> _covers;
        public List<string> Covers
        {
            get { return _covers; }
            set
            {
                _covers = value;
                OnPropertyChanged(nameof(Covers));
            }
        }

        private bool _userChoseCover = false;
        public bool UserChoseCover
        {
            get { return _userChoseCover; }
            set
            {
                _userChoseCover = value;
                OnPropertyChanged(nameof(UserChoseCover));
            }
        }

        private string _blurredCover;
        public string BlurredCover
        {
            get { return _blurredCover; }
            set
            {
                _blurredCover = value;
                OnPropertyChanged(nameof(BlurredCover));
            }
        }

        private bool _isQueueOpen = false;
        public bool IsQueueOpen
        {
            get { return _isQueueOpen; }
            set
            {
                _isQueueOpen = value;
                OnPropertyChanged(nameof(IsQueueOpen));
            }
        }

        private bool _isLyricsOpen = false;
        public bool IsLyricsOpen
        {
            get { return _isLyricsOpen; }
            set
            {
                _isLyricsOpen = value;
                OnPropertyChanged(nameof(IsLyricsOpen));
            }
        }

        private bool _isTrackInfoOpen = false;
        public bool IsTrackInfoOpen
        {
            get { return _isTrackInfoOpen; }
            set
            {
                _isTrackInfoOpen = value;
                OnPropertyChanged(nameof(IsTrackInfoOpen));
            }
        }

        private bool _isSubViewOpen = false;
        public bool IsSubViewOpen
        {
            get { return _isSubViewOpen; }
            set
            {
                _isSubViewOpen = value;
                OnPropertyChanged(nameof(IsSubViewOpen));
            }
        }

        private bool _isCoverOpen = ConfigurationService.GetPreference(SettingsEnum.NowPlayingCoverVisibility) == 1;
        public bool IsCoverOpen
        {
            get { return _isCoverOpen; }
            set
            {
                _isCoverOpen = value;
                OnPropertyChanged(nameof(IsCoverOpen));
                ConfigurationService.SetPreference(SettingsEnum.NowPlayingCoverVisibility, (value ? 1 : 0).ToString());
            }
        }

        private int _spectrumColumn;
        public int SpectrumColumn
        {
            get => _spectrumColumn;
            set
            {
                SetField(ref _spectrumColumn, value);
            }
        }

        private double _widthValue;
        public double WidthValue
        {
            get { return _widthValue; }
            set
            {
                _widthValue = value;
                OnPropertyChanged(nameof(WidthValue));
            }
        }

        public ICommand NextCoverCommand { get; }
        public ICommand PreviousCoverCommand { get; }
        public ICommand OpenCloseSubViewCommand { get; }
        public ICommand SwitchFullScreenCommand { get; }
        public ICommand OpenCloseCoverCommand { get; }
        public ICommand ShowPlayerControlCommand { get; }
        public ICommand ChangeIsAutoForegroundCommand { get; }
        public ICommand InvertForegroundCommand { get; }
        public ICommand ShowHideSpectrumCommand { get; }
        public ICommand OpenVisualizerSettingCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public NowPlayingViewModel(IQueueService queueService,  IAudioPlayback audioPlayback, IVisualizerParameterStore visualizerParameterStore, 
            IWindowService windowService, NowPlayingPlayerControlViewModel nowPlayingPlayerControlViewModel, ICommandsManager commandsManager)
        {
            QueueService = queueService;
            _audioPlayback = audioPlayback;
            VisualizerParameterService = visualizerParameterStore;
            _windowService = windowService;
            PlayerViewModel = nowPlayingPlayerControlViewModel;
            _commandsManager = commandsManager;

            AppState.FullScreenChanged += IsFullScreenChanged;
            _visualizerParameterService.AutoColorChanged += VAutoColorChanged;
            _visualizerParameterService.RepresentationChanged += VRepresentationChanged;
            _audioPlayback.StreamChanged += OnStreamChanged;
            _queueService.PlayingTrackChanged += OnPlayingTrackChanged;

            NextCoverCommand = new RelayCommand(() =>
            {
                UserChoseCover = true;
                SelectedCoverIndex++;
                if (SelectedCoverIndex >= Covers.Count)
                    SelectedCoverIndex = 0;
                SetSelectedCover(SelectedCoverIndex);
            });

            PreviousCoverCommand = new RelayCommand(() =>
            {
                UserChoseCover = true;
                SelectedCoverIndex--;
                if (SelectedCoverIndex < 0)
                    SelectedCoverIndex = Covers.Count - 1;
                SetSelectedCover(SelectedCoverIndex);
            });

            OpenCloseSubViewCommand = new RelayCommand<string>((value) =>
            {
                if(int.TryParse(value, out int view))
                {
                    OpenCloseSubView(view);
                }
            });

            SwitchFullScreenCommand = new RelayCommand(AppState.ToggleFullScreen);

            OpenCloseCoverCommand = new RelayCommand(() =>
            {
                IsCoverOpen = !IsCoverOpen;

                SetWidthValue();
            });

            ShowHideSpectrumCommand = new RelayCommand(() =>
            {
                SpectrumVisibility = !SpectrumVisibility;
                SetWidthValue();
            });

            OpenVisualizerSettingCommand = new RelayCommand(() =>
            {
                _windowService.OpenWindow(ViewNameEnum.Visualizer);
            });

            NavigateToArtistCommand = _commandsManager.NavigateToArtistByIdCommand;

            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumByIdCommand;

            LoadData();
            Stream = _audioPlayback.Stream;
            InitSubView();
        }

        private void IsFullScreenChanged()
        {
            IsFullScreen = AppState.IsFullScreen;
        }

        public override void Dispose()
        {
            _queueService.PlayingTrackChanged -= OnPlayingTrackChanged;
            _visualizerParameterService.AutoColorChanged -= VAutoColorChanged;
            _visualizerParameterService.RepresentationChanged -= VRepresentationChanged;
            _audioPlayback.StreamChanged -= OnStreamChanged;
            AppState.FullScreenChanged -= IsFullScreenChanged;
            PlayerViewModel.Dispose();

            // dispose of the subview if opened
            if (IsSubViewOpen)
            {
                AppState.CurrentView.State.ChildViewModel.ViewModel?.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        private void InitSubView()
        {
            OpenCloseSubView(ConfigurationService.GetPreference(SettingsEnum.NowPlayingStartingSubView));
        }

        private void OpenCloseSubView(int view)
        {
            ViewNameEnum selectedView = ViewNameEnum.Empty;

            if (view == 41) // queue
            {
                IsQueueOpen = !IsQueueOpen;
                IsLyricsOpen = false;
                IsTrackInfoOpen = false;

                if (IsQueueOpen)
                    selectedView = ViewNameEnum.Queue;
            }
            else if (view == 42) // lyrics
            {
                IsQueueOpen = false;
                IsLyricsOpen = !IsLyricsOpen;
                IsTrackInfoOpen = false;

                if (IsLyricsOpen)
                {
                    selectedView = ViewNameEnum.Lyrics;
                }
            }
            else if (view == 43) // track info
            {
                IsQueueOpen = false;
                IsLyricsOpen = false;
                IsTrackInfoOpen = !IsTrackInfoOpen;

                if (IsTrackInfoOpen) 
                    selectedView = ViewNameEnum.TrackInfo;
            }

            if (selectedView == ViewNameEnum.Empty)
            {
                IsSubViewOpen = false;
                AppState.CurrentView.State.ChildViewModel = new(new EmptyViewModel(), null);
                ConfigurationService.SetPreference(SettingsEnum.NowPlayingStartingSubView, "0");
            }
            else
            {
                IsSubViewOpen = true;
                //NavigationService.NavigateTo(selectedView);
                ConfigurationService.SetPreference(SettingsEnum.NowPlayingStartingSubView, view.ToString());
            }
            SetWidthValue();
        }

        private void SetWidthValue()
        {
            if (IsCoverOpen && (IsSubViewOpen || SpectrumVisibility))
            {
                if(IsSubViewOpen)
                {
                    WidthValue = 30;

                    if(IsLyricsOpen)
                    {
                        VisualizerParameterService.TextAlignment = TextAlignment.Left;
                    }
                }
                else
                {
                    SpectrumColumn = 0;
                    WidthValue = 100; // take all width
                }

                if (_visualizerParameterService.Representation == DataRepresentationTypeEnum.CircledPoints || 
                    _visualizerParameterService.Representation == DataRepresentationTypeEnum.CircledBar) 
                {
                    SpectrumColumn = 1;

                    if(!IsSubViewOpen)
                        WidthValue = 40; // need less width for circle representations
                }
                else if(_visualizerParameterService.Representation == DataRepresentationTypeEnum.LinearMirroredPoints ||
                    _visualizerParameterService.Representation == DataRepresentationTypeEnum.LinearMirroredBar)
                {
                    SpectrumColumn = 1;
                    WidthValue = 30;
                }
                else
                {
                    SpectrumColumn = 0; // spectrum can take all width available since it will not be hidden (either top or bottom => above or under cover)
                }
            }
            else if (IsSubViewOpen)
            {
                SpectrumColumn = 0; // cover not open
                WidthValue = 0;

                if(IsLyricsOpen)
                {
                    VisualizerParameterService.TextAlignment = TextAlignment.Center;
                }
            }
            else
            {
                if(!IsCoverOpen)
                {
                    SpectrumColumn = 0;
                }

                WidthValue = 100;
            }            
        }

        private void LoadData()
        {
            if(_queueService.Queue.PlayingTrack is not null)
            {
                IsFullScreen = AppState.IsFullScreen;
                SetCovers();
                SetWidthValue();
            }
        }

        private void SetCovers()
        {
            List<string> covers = new();
            int selectedCover = SelectedCoverIndex;

            if (_queueService.Queue.Cover.ValidFilePath())
            {
                covers.Add(_queueService.Queue.Cover);
                selectedCover = 0;
            }

            if (_queueService.Queue.PlayingTrack.Album.AlbumCover.ValidFilePath() && 
                _queueService.Queue.PlayingTrack.Album.AlbumCover != _queueService.Queue.Cover)
            {
                covers.Add(_queueService.Queue.PlayingTrack.Album.AlbumCover);
                selectedCover = covers.Count - 1;
            }

            if (_queueService.Queue.PlayingTrack.Artwork.ValidFilePath() && 
                _queueService.Queue.PlayingTrack.Artwork != _queueService.Queue.PlayingTrack.Album.AlbumCover &&
                _queueService.Queue.PlayingTrack.Artwork != _queueService.Queue.Cover)
            {
                covers.Add(_queueService.Queue.PlayingTrack.Artwork);
                selectedCover = covers.Count - 1;
            }

            if (UserChoseCover) // try to keep the selected cover
            {
                int index = covers.IndexOf(SelectedCover);
                if (index != -1) // found
                {
                    selectedCover = index; // keep the selected cover
                }
                else
                {
                    UserChoseCover = false; // cover chosen not available anymore
                }
            }

            Covers = covers;
            SetSelectedCover(selectedCover);
        }

        private void SetSelectedCover(int index)
        {
            if(index < Covers.Count && index >=0)
            {
                // if cover is not the current one
                if (SelectedCover != Covers[index])
                {
                    SelectedCover = Covers[index];
                    SelectedCoverIndex = index;
                    try
                    {
                        GetBlurredCover(); // also update the spectrum color if it's in auto
                    }
                    catch (Exception)
                    {
                        // when skipping tracks too fast an error happens sometimes when getting the blurred cover:
                        // ImageSource Failed to convert value 'C:\Users\adrie\Music\MusicPlay\Covers\Blurred\Two Of Us_AlbumCover_blurred.png'(type 'String')
                        // to the target type using converter 'DynamicValueConverter'.The fallback value will be used if it's available.
                        // IOException:'System.IO.IOException: The process cannot access the file 'C:\Users\adrie\Music\MusicPlay\Covers\Blurred\Two Of Us_AlbumCover_blurred.png'
                        // because it is being used by another process.
                        MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error while getting the cover."));
                    }
                }
            }
            else if(index > 0)
            {
                SetSelectedCover(index - 1);
            }
        }
        private void GetBlurredCover()
        {
            if (SelectedCover.ValidFilePath())
            {
                BlurredCover = SelectedCover.GetBlurredImage(20);

                if(BlurredCover is not null)
                    SetSpectrumColor();
            }
        }

        private void SetSpectrumColor()
        {
            if (BlurredCover.ValidFilePath() && VisualizerParameterService.AutoColor)
            {
                SolidColorBrush meanColor = ImageProcessor.CalculateMeanColor(BlurredCover);

                SolidColorBrush emphasizedColor = meanColor.GetEmphasizedColor();
                VisualizerParameterService.ObjectColor = emphasizedColor;
            }
        }

        private void OnPlayingTrackChanged()
        {
            SetCovers();
        }

        private void OnStreamChanged()
        {
            Stream = _audioPlayback.Stream;
        }

        private void VAutoColorChanged()
        {
            if (VisualizerParameterService.AutoColor)
            {
                SetSpectrumColor();
            }
        }

        private void VRepresentationChanged()
        {
            // if the cover and the visualizer are opened but not the sub view
            // then the representation type can change the width 
            if(IsCoverOpen && SpectrumVisibility)
            {
                SetWidthValue();
            }
        }

    }
}
