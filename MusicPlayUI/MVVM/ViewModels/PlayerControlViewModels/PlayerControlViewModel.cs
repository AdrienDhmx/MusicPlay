using AudioHandler;
using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicFilesProcessor.Helpers;
using System;
using System.Timers;
using System.Windows.Input;
using Timer = System.Timers.Timer;
using System.Collections.ObjectModel;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using System.Threading.Tasks;
using System.Windows.Media;
using MusicFilesProcessor;
using System.Windows.Documents;

namespace MusicPlayUI.MVVM.ViewModels.PlayerControlViewModels
{
    public class PlayerControlViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;

        private IAudioTimeService _audioService;
        public IAudioTimeService AudioService
        {
            get => _audioService;
            set { SetField(ref _audioService, value); }
        }

        private IAudioPlayback _audioPlayback;
        public IAudioPlayback AudioPlayback
        {
            get { return _audioPlayback; }
            set { SetField(ref _audioPlayback, value); }
        }

        private IQueueService _queueService;
        public IQueueService QueueService
        {
            get { return _queueService; }
            set { SetField(ref _queueService, value); }
        }

        private bool _colorfulPlayerControl = ConfigurationService.GetPreference(SettingsEnum.ColorfulPlayerControl) == 1;
        public bool ColorfulPlayerControl
        {
            get { return _colorfulPlayerControl; }
            set
            {
                _colorfulPlayerControl = value;
                OnPropertyChanged(nameof(ColorfulPlayerControl));
            }
        }

        private RadialGradientBrush _radialGradientBrush;
        public RadialGradientBrush RadialGradientBrush
        {
            get => _radialGradientBrush;
            set
            {
                SetField(ref _radialGradientBrush, value);
            }
        }

        private SolidColorBrush _meanColor;
        public SolidColorBrush MeanColor
        {
            get => _meanColor;
            set
            {
                SetField(ref _meanColor, value);
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

        private bool _showMeanColor;
        public bool ShowMeanColor
        {
            get => _showMeanColor;
            set
            {
                SetField(ref _showMeanColor, value);
            }
        }

        private int _volume => _audioPlayback.Volume;
        public int Volume
        {
            get { return _volume; }
            set
            {
                VolumeIsChanging(value);
            }
        }

        public int VolumePercentage
        {
            get { return Volume * 100 / 10000; }
        }

        private FullDeviceModel _currentDevice;
        public FullDeviceModel CurrentDevice
        {
            get { return _currentDevice; }
            set
            {
                SetField(ref _currentDevice, value);
                OnPropertyChanged(nameof(CurrentDeviceIcon));
            }
        }

        public Geometry CurrentDeviceIcon => CurrentDevice.Icon;


        private ObservableCollection<FullDeviceModel> _allDevices = new(AudioOutput.GetAllDevices().CreateDeviceModel());
        public ObservableCollection<FullDeviceModel> AllDevices
        {
            get { return _allDevices; }
            set
            {
                _allDevices = value;
                OnPropertyChanged(nameof(AllDevices));
            }
        }

        private bool _isDevicesPopupOpen = false;
        public bool IsDevicesPopupOpen
        {
            get { return _isDevicesPopupOpen; }
            set
            {
                SetField(ref _isDevicesPopupOpen, value);
            }
        }

        public int Rating
        {
            get
            {
                if (QueueService.PlayingTrack != null)
                    return (int)QueueService.PlayingTrack.Rating;
                else
                    return 0;
            }
            set
            {
                SaveRating(value);
            }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }

        public ICommand ShuffleCommand { get; }
        public ICommand RepeatCommand { get; }
        public ICommand PlayPauseCommand { get; }
        public ICommand NextTrackCommand { get; }
        public ICommand PreviousTrackCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToNowPlayingCommand { get; }
        public ICommand IsFavoriteCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand ChangeOutputDeviceCommand { get; }
        public ICommand OpenDevicesPopupCommand { get; }
        public ICommand MuteCommand { get; }
        public ICommand OpenCloseQueuePopupCommand { get; }
        public ICommand NavigateToPlayingFromCommand { get; }
        public PlayerControlViewModel(INavigationService navigationService, IAudioTimeService audioService, IAudioPlayback audioPlayback, IQueueService queueService)
        {
            _navigationService = navigationService;
            AudioService = audioService;
            AudioPlayback = audioPlayback;
            QueueService = queueService;

            // same prop to update in both cases
            QueueService.PlayingTrackChanged += OnPlayingTrackChanged;
            QueueService.PlayingTrackInteractionChanged += OnPlayingTrackChanged;

            _audioPlayback.VolumeChanged += OnVolumeChanged;
            _audioPlayback.DeviceChanged += OnDeviceChanged;

            ConfigurationService.ColorfulPlayerControlChange += OnColorfulPlayerChanged;
            AppThemeService.ThemeChanged += UpdateBlurredCover;

            Volume = ConfigurationService.GetPreference(SettingsEnum.Volume);
            OnDeviceChanged();

            PlayPauseCommand = new RelayCommand(_audioService.PlayPause);

            PreviousTrackCommand = new RelayCommand(() =>
            {
                _queueService.PreviousTrack();
            });

            NextTrackCommand = new RelayCommand(() =>
            {
                _queueService.NextTrack();
            });

            ShuffleCommand = new RelayCommand(() => Task.Run(_queueService.Shuffle));

            RepeatCommand = new RelayCommand(() =>
            {
                if (_audioService.IsLooping)
                {
                    _audioService.Loop(); // Remove the loop
                }
                else
                {
                    if (QueueService.IsOnRepeat)
                    {
                        _audioService.Loop(); // set the loop
                    }
                    _queueService.Repeat(); // Remove or set the repeat
                }
            });

            NavigateToAlbumCommand = new RelayCommand(async () =>
            {
                AlbumModel Album = await DataAccess.Connection.GetAlbum(QueueService.PlayingTrack.AlbumId);
                _navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, Album);
            });

            NavigateToArtistCommand = new RelayCommand<int>(async (id) =>
            {
                ArtistModel Performer = await DataAccess.Connection.GetArtist(id);

                _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, Performer);
            });

            NavigateToNowPlayingCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo(ViewNameEnum.NowPlaying);
            });

            IsFavoriteCommand = new RelayCommand(async () =>
            {
                IsFavorite = !IsFavorite;
                await _queueService.UpdateFavorite(IsFavorite);
            });

            OpenTrackPopupCommand = new RelayCommand(OpenTrackPopup);

            ChangeOutputDeviceCommand = new RelayCommand<FullDeviceModel>((device) =>
            {
                _audioPlayback.ChangeOutputDevice(device.ToDeviceModel());
                foreach (FullDeviceModel d in AllDevices)
                {
                    if (d.Index == device.Index)
                    {
                        d.IsInitialized = true;
                    }
                    else
                    {
                        d.IsInitialized = false;
                    }
                }
            });

            OpenDevicesPopupCommand = new RelayCommand(OpenCloseDevicePopup);

            MuteCommand = new RelayCommand(_audioPlayback.Mute);

            OpenCloseQueuePopupCommand = new RelayCommand(() =>
            {
                _navigationService.ToggleQueueDrawer();
            });

            NavigateToPlayingFromCommand = new RelayCommand(_queueService.NavigateToPlayingFrom);

            UpdateBlurredCover();
        }

        private void UpdateRadialGradient()
        {
            RadialGradientBrush gradientBrush = new RadialGradientBrush()
            {
                GradientOrigin = new System.Windows.Point(0.1, 0.5),
                RadiusX = 0.8,
                RadiusY = 1.6,
            };
 
            GradientStopCollection offsets = new GradientStopCollection();
            MeanColor = BlurredCover.CalculateMeanColor();
            double meanChannelsValue = MeanColor.CalculateBrightness();
            ShowMeanColor = MeanColor.IsGrey(margin: 14);

            if(ShowMeanColor)
            {
                MeanColor = MeanColor.GetEmphasizedColor();
            }

            int modifier = (int)(20 * (1 - meanChannelsValue)); // invert the mean to make dark color more important

            if ((AppThemeService.IsLightTheme && meanChannelsValue < 0.65) // dark color on light color
                || (!AppThemeService.IsLightTheme && meanChannelsValue > 0.65)) // light color on dark color
            {
                if (AppThemeService.IsLightTheme && meanChannelsValue > 0.45)
                {
                    modifier = 20;
                }

                // not too visible
                offsets.Add(new GradientStop(Color.FromArgb((byte)(200 - modifier), 0, 0, 0), 0));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(220 - modifier), 0, 0, 0), 0.2));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(230 - modifier), 0, 0, 0), 0.4));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(240 - modifier), 0, 0, 0), 0.6));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(250 - modifier), 0, 0, 0), 0.8));
                offsets.Add(new GradientStop(Color.FromArgb(250, 0, 0, 0), 1));
            }
            else
            {
                if (!ShowMeanColor)
                {
                    if (AppThemeService.IsLightTheme && meanChannelsValue > 0.85)
                    {
                        modifier = -10;
                    } 
                    else if(!AppThemeService.IsLightTheme && meanChannelsValue < 0.28)
                    {
                        modifier = -20;
                    }
                }

                // make more visible
                offsets.Add(new GradientStop(Color.FromArgb((byte)(190 + modifier), 0, 0, 0), 0));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(200 + modifier), 0, 0, 0), 0.2));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(210 + modifier), 0, 0, 0), 0.4));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(220 + modifier), 0, 0, 0), 0.6));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(240 + modifier / 2), 0, 0, 0), 0.8));
                offsets.Add(new GradientStop(Color.FromArgb((byte)(250 + modifier / 2), 0, 0, 0), 1));
            }

            gradientBrush.GradientStops = offsets;
            RadialGradientBrush = gradientBrush;
        }

        private void UpdateBlurredCover()
        {
            if (QueueService.PlayingTrack != null && ColorfulPlayerControl)
            {
                BlurredCover = QueueService.PlayingTrack.Cover.GetBlurredImage(20);
                UpdateRadialGradient();
            }
        }

        private void OpenCloseDevicePopup()
        {
            IsDevicesPopupOpen = !IsDevicesPopupOpen;
            AllDevices = new(AudioOutput.GetAllDevices().CreateDeviceModel());
        }

        private void SaveRating(int value)
        {
            _queueService.UpdateRating(value);
        }

        public override void Dispose()
        {
            _audioPlayback.VolumeChanged -= OnVolumeChanged;
            _queueService.PlayingTrackChanged -= OnPlayingTrackChanged;

            QueueService.PlayingTrackChanged -= OnPlayingTrackChanged;
            QueueService.PlayingTrackInteractionChanged -= OnPlayingTrackChanged;

            ConfigurationService.ColorfulPlayerControlChange -= OnColorfulPlayerChanged;
            AppThemeService.ThemeChanged -= UpdateBlurredCover;

            _audioPlayback.Dispose(); // dispose of all audio resources
            GC.SuppressFinalize(this);
        }

        private void OpenTrackPopup()
        {
            if (!_navigationService.IsPopupOpen)
            {
                _navigationService.OpenPopup(ViewNameEnum.TrackPopup, QueueService.PlayingTrack);
            }
            else
            {
                _navigationService.ClosePopup();
            }
        }

        private void VolumeIsChanging(int volume)
        {
            if (volume > Volume)
            {
                _audioPlayback.IncreaseVolume(volume - Volume);
            }
            else
            {
                _audioPlayback.DecreaseVolume(Volume - volume);
            }
        }

        private void OnVolumeChanged(int volume)
        {
            OnPropertyChanged(nameof(Volume));
            OnPropertyChanged(nameof(VolumePercentage));
            ConfigurationService.SetPreference(SettingsEnum.Volume, Volume.ToString());
        }

        private void OnPlayingTrackChanged()
        {
            if (QueueService.PlayingTrack is not null) 
            {
                IsFavorite = QueueService.PlayingTrack.IsFavorite;
                OnPropertyChanged(nameof(Rating));
                UpdateBlurredCover();
            }
        }

        private void OnDeviceChanged()
        {
             CurrentDevice = _audioPlayback.Device.CreateDeviceModal();
        }

        private void OnColorfulPlayerChanged()
        {
            ColorfulPlayerControl = ConfigurationService.GetPreference(SettingsEnum.ColorfulPlayerControl) == 1;
            UpdateBlurredCover();
        }
    }
}
