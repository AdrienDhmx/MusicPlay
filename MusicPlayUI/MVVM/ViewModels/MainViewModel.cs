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
    public class MainViewModel : ViewModel
    {
        public IAudioPlayback AudioPlayback { get; }
        public IQueueService QueueService { get; }

        private IModalService _modalService;
        public IModalService ModalService
        {
            get { return _modalService; }
            set { SetField(ref _modalService, value); }
        }

        public static IAppState AppState
        {
            get => App.State;
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

        public ICommandsManager CommandsManager { get; }
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

        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand LeaveCommand { get; }
        public MainViewModel(IAudioPlayback audioPlayback, IQueueService queueService, IModalService modalService, MainMenuViewModel mainMenuViewModel, 
            QueueDrawerViewModel queueDrawerViewModel, PlayerControlViewModel playerControlViewModel, 
             ICommandsManager commandsManager)
        {
            AudioPlayback = audioPlayback;
            QueueService = queueService;
            CurrentMenu = mainMenuViewModel;
            QueueDrawer = queueDrawerViewModel;
            CurrentPlayerControl = playerControlViewModel;
            CommandsManager = commandsManager;
            ModalService = modalService;

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
