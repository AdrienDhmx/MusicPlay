using AudioHandler;
using GongSolutions.Wpf.DragDrop.Utilities;
using Microsoft.Extensions.DependencyInjection;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels.PlayerControlViewModels;
using MusicPlayUI.MVVM.ViewModels.PopupViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class MainViewModel : ViewModel, IOnMouseDownListener
    {
        private readonly IAudioTimeService _audioTimeService;
        public IAudioPlayback AudioPlayback { get; }
        public IQueueService QueueService { get; }

        private IModalService _modalService;
        public IModalService ModalService
        {
            get { return _modalService; }
            set { SetField(ref _modalService, value); }
        }

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get => _navigationService;
            set { SetField(ref _navigationService, value); }
        }


        private ViewModel _currentPlayerControl;
        public ViewModel CurrentPlayerControl
        {
            get => _currentPlayerControl;
            set
            {
                if (_currentPlayerControl is not null)
                    _currentPlayerControl.Dispose();
                _currentPlayerControl = value;
                OnPropertyChanged(nameof(CurrentPlayerControl));
            }
        }

        public bool IsModalOpen
        {
            get => _modalService.IsModalOpen;
        }

        public ViewModel ModalContent
        {
            get => _modalService.Modal;
        }

        private ViewModel _currentMenu;
        public ViewModel CurrentMenu
        {
            get => _currentMenu;
            set
            {
                _currentMenu?.Dispose();
                _currentMenu = value;
                OnPropertyChanged(nameof(CurrentMenu));
            }
        }

        private ViewModel _queueDrawer;
        public ViewModel QueueDrawer
        {
            get => _queueDrawer;
            set { SetField(ref _queueDrawer, value); }
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand NextTrackCommand { get; }
        public ICommand PreviousTrackCommand { get; }
        public ICommand DecreaseVolumeCommand { get; set; }
        public ICommand IncreaseVolumeCommand { get; set; }
        public ICommand SwitchFullScreenCommand { get; }
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
        public MainViewModel(INavigationService navigationService, IAudioPlayback audioPlayback, IQueueService queueService, IModalService modalService, IAudioTimeService audioTimeService,
             MainMenuViewModel mainMenuViewModel, QueueDrawerViewModel queueDrawerViewModel, PlayerControlViewModel playerControlViewModel)
        {
            NavigationService = navigationService;
            AudioPlayback = audioPlayback;
            QueueService = queueService;
            CurrentMenu = mainMenuViewModel;
            QueueDrawer = queueDrawerViewModel;
            CurrentPlayerControl = playerControlViewModel;
            ModalService = modalService;
            _audioTimeService = audioTimeService;

            PlayPauseCommand = new RelayCommand(_audioTimeService.PlayPause);

            NextTrackCommand = new RelayCommand(() =>
            {
                queueService.NextTrack();
            });

            PreviousTrackCommand = new RelayCommand(() =>
            {
                queueService.PreviousTrack();
            });

            DecreaseVolumeCommand = new RelayCommand(() =>
            {
                audioPlayback.DecreaseVolume();
            });

            IncreaseVolumeCommand = new RelayCommand(() =>
            {
                audioPlayback.IncreaseVolume();
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

            NavigateCommand = new RelayCommand<string>((view) =>
            {
                NavigationService.NavigateTo(view);
            });

            SwitchFullScreenCommand = new RelayCommand(() =>
            {
                navigationService.SwitchFullScreen();
            });

            EscapeFullScreenCommand = new RelayCommand(() =>
            {
                if (NavigationService.IsFullScreen)
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

        public void OnClick(MouseButtonEventArgs e)
        {
            if (NavigationService.IsPopupOpen)
            {
                UIElement element = Mouse.DirectlyOver as UIElement;

                Popup ancestor = FindParent<Popup>(element);
                // if the click is not on the popup close popup
                if(element is not null && ancestor is null)
                {
                   NavigationService.ClosePopup();
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = GetParentObject(child);

            // end of the tree
            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return FindParent<T>(parentObject);
            }
        }

        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;

            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }
    }
}
