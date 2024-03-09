using MessageControl;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Models;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.AppBars;
using MusicPlayUI.MVVM.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services
{
    public class AppState(Func<Type, ViewModel> viewModelFactory) : ObservableObject, IAppState
    {
        private readonly Func<Type, ViewModel> _viewModelFactory = viewModelFactory;
        private readonly int _maxHistoryCount = 20;

        public event Action FullScreenChanged;
        public event Action CurrentViewChanged;

        private bool _wasMenuOpen = false;
        private bool _isFullScreen;

        private bool _isMenuDrawerOpen = true;
        private bool _isQueueDrawerOpen;
        private bool _isPopupOpen;

        private AppBar _appBar;
        private NavigationModel _currentView;
        private NavigationModel _currentPopup;

        private bool _canNavigateBack = false;
        private bool _canNavigateForward = false;


        public void SetAppBar(AppBar appBar)
        {
            if(_appBar is null)
            {
                AppBar = appBar;
                this.CurrentView.ViewModel.UpdateAppBarStyle();
            }
        }

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                SetField(ref _isFullScreen, value);
                if(value)
                {
                    _wasMenuOpen = IsMenuDrawerOpen;    
                    IsMenuDrawerOpen = false;
                }
                else
                {
                    IsMenuDrawerOpen = _wasMenuOpen;
                }
                FullScreenChanged?.Invoke();
            }
        }

        public bool IsMenuDrawerOpen
        {
            get => _isMenuDrawerOpen;
            set => SetField(ref _isMenuDrawerOpen, value);
        }

        public bool IsQueueDrawerOpen
        {
            get => _isQueueDrawerOpen;
            set => SetField(ref _isQueueDrawerOpen, value);
        }

        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set => SetField(ref _isPopupOpen, value);
        }

        public AppBar AppBar
        {
            get => _appBar;
            private set
            {
                SetField(ref _appBar, value);
            }
        }

        public NavigationModel CurrentPopup
        {
            get => _currentPopup;
            set
            {
                _currentPopup?.ViewModel?.Dispose();
                SetField(ref _currentPopup, value);
                CurrentPopup.ViewModel.Init();
            }
        }

        public NavigationModel CurrentView
        {
            get => _currentView;
            set
            {
                _currentView?.ViewModel?.Dispose();
                SetField(ref _currentView, value);

                CanNavigateBack = _backNavigationHistory.Count > 0;
                CanNavigateForward = _forwardNavigationHistory.Count > 0;

                CurrentViewChanged?.Invoke();
                CurrentView.ViewModel.Init();
            }
        }

        public bool CanNavigateBack
        {
            get => _canNavigateBack;
            set => SetField(ref _canNavigateBack, value);
        }

        public bool CanNavigateForward
        {
            get => _canNavigateForward;
            set => SetField(ref _canNavigateForward, value);
        }

        private bool _savePreviousViewModel = true;
        private readonly List<NavigationModel> _backNavigationHistory = [];
        private readonly List<NavigationModel> _forwardNavigationHistory = [];

        public NavigationModel CreateNavigationModel<TViewModel>(NavigationState state = null) where TViewModel : ViewModel
        {
            state ??= new NavigationState(null);
            return new NavigationModel((TViewModel)_viewModelFactory.Invoke(typeof(TViewModel)), state);
        }

        public NavigationModel CreateNavigationModel(Type viewModelType, NavigationState state = null)
        {
            try
            {
                return (NavigationModel)this.GetType().GetMethod("CreateNavigationModel", [typeof(NavigationState)]).MakeGenericMethod(viewModelType).Invoke(this, [state]);
            }
            catch (Exception ex)
            {
                // Handle reflection-related exceptions or other unexpected errors
                $"Error creating the Navigation Model for {viewModelType}: {ex.Message}".CreateErrorMessage().PublishWithAppDispatcher();
                return null;
            }
        }

        public void NavigateTo<TViewModel>(NavigationState state, bool saveView = true) where TViewModel : ViewModel
        {
            if(CurrentView?.ViewModel.GetType() ==  typeof(TViewModel))
            {
                CurrentView.ViewModel.Update(); // refresh
                return;
            }

            if (_savePreviousViewModel && CurrentView is not null && _backNavigationHistory.Count < _maxHistoryCount)
            {
                _backNavigationHistory.Add(CurrentView);
            }

            _forwardNavigationHistory.Clear();
            CurrentView = CreateNavigationModel<TViewModel>(state);
            _savePreviousViewModel = saveView;
        }

        public void NavigateTo<TViewModel>(BaseModel parameter = null, bool saveView = true) where TViewModel : ViewModel
        {
            NavigateTo<TViewModel>(new NavigationState(parameter), saveView);
        }

        public void NavigateTo(Type viewModelType, BaseModel parameter = null, bool saveView = true)
        {
            try
            {
                this.GetType().GetMethod("NavigateTo", [typeof(BaseModel), typeof(bool)]).MakeGenericMethod(viewModelType).Invoke(this, [parameter, saveView]);
            }
            catch (Exception ex)
            {
                // Handle reflection-related exceptions or other unexpected errors
                $"Error navigating to {viewModelType}: {ex.Message}".CreateErrorMessage().PublishWithAppDispatcher();
            }
        }

        public void NavigateTo<TViewModel, TChildViewModel>(NavigationState state = null, NavigationState childState = null, bool saveView = true)
            where TViewModel : ViewModel
            where TChildViewModel : ViewModel
        {
            NavigationModel childViewModel = CreateNavigationModel<TChildViewModel>(childState);
            if (state == null)
            {
                state = new NavigationState(null, childViewModel);
            }
            else
            {
                state.ChildViewModel = childViewModel;
            }

            NavigateTo<TViewModel>(state, saveView);
        }

        public void NavigateBack()
        {
            if (!CanNavigateBack)
                return;

            _forwardNavigationHistory.Add(CurrentView);
            NavigationModel previousNavModel = _backNavigationHistory.Last();
            _backNavigationHistory.Remove(previousNavModel);

            CurrentView = CreateNavigationModel(previousNavModel.ViewModel.GetType(), previousNavModel.State);
        }

        public void NavigateForward()
        {
            if (!CanNavigateForward)
                return;

            _backNavigationHistory.Add(CurrentView);
            NavigationModel previousForwardedNavModel = _forwardNavigationHistory.Last();
            _forwardNavigationHistory.Remove(previousForwardedNavModel);

            CurrentView = CreateNavigationModel(previousForwardedNavModel.ViewModel.GetType(), previousForwardedNavModel.State);
        }

        public bool UpdateCurrentViewIfIs(List<Type> viewModels)
        {
            if(viewModels.Any(vm => vm == CurrentView.ViewModel.GetType()))
            {
                CurrentView.ViewModel.Update();
                return true;
            }
            return false;
        }

        public void OpenPopup<TPopupViewModel>(NavigationState state = null) where TPopupViewModel : ViewModel
        {
            CurrentPopup = CreateNavigationModel<TPopupViewModel>(state);
            IsPopupOpen = true;
        }

        public void OpenPopup<TPopupViewModel>(BaseModel parameter = null) where TPopupViewModel : ViewModel
        {
            OpenPopup<TPopupViewModel>(new NavigationState(parameter));
        }

        public void ClosePopup() { IsPopupOpen = false; }

        public void ToggleFullScreen() { IsFullScreen = !IsFullScreen; }

        public void ToggleMenuDrawer() { IsMenuDrawerOpen = !IsMenuDrawerOpen; }

        public void ToggleQueueDrawer() { IsQueueDrawerOpen = !IsQueueDrawerOpen; }
    }
}
