using MessageControl;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Models;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.PopupViewModels;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Windows.Navigation;

namespace MusicPlayUI.Core.Services
{
    public class AppState(Func<Type, ViewModel> viewModelFactory) : ObservableObject, IAppState
    {
        private readonly Func<Type, ViewModel> _viewModelFactory = viewModelFactory;

        public event Action FullScreenChanged;
        public event Action CurrentViewChanged;

        private bool _isFullScreen;
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                SetField(ref _isFullScreen, value);
                FullScreenChanged?.Invoke();
            }
        }

        private bool _isQueueDrawerOpen;
        public bool IsQueueDrawerOpen
        {
            get => _isQueueDrawerOpen;
            set => SetField(ref _isQueueDrawerOpen, value);
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set => SetField(ref _isPopupOpen, value);
        }

        private NavigationModel _currentPopup;
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

        private NavigationModel _currentView;
        public NavigationModel CurrentView
        {
            get => _currentView;
            set
            {
                _currentView?.ViewModel?.Dispose();
                SetField(ref _currentView, value);
                CurrentViewChanged?.Invoke();
                CurrentView.ViewModel.Init();
            }
        }

        private bool _savePreviousViewModel = true;
        private readonly List<NavigationModel> _backNavigationHistory = [];
        private readonly List<NavigationModel> _forwardNavigationHistory = [];

        public NavigationModel CreateNavigationModel<TViewModel>(NavigationState state = null) where TViewModel : ViewModel
        {
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
            if (_savePreviousViewModel)
                _backNavigationHistory.Add(CurrentView);

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
                this.GetType().GetMethod("NavigateTo").MakeGenericMethod(viewModelType).Invoke(this, [parameter, saveView]);
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
            NavigateTo<TViewModel>(state ?? new NavigationState(null), saveView);
            CurrentView.State.ChildViewModel = CreateNavigationModel<TChildViewModel>(childState);
        }

        public void NavigateBack()
        {
            if (_backNavigationHistory.Count == 0)
                return;

            _forwardNavigationHistory.Add(CurrentView);
            CurrentView = _backNavigationHistory.Last();
            _backNavigationHistory.Remove(CurrentView);
        }

        public void NavigateForward()
        {
            if (_forwardNavigationHistory.Count == 0)
                return;

            _backNavigationHistory.Add(CurrentView);
            CurrentView = _forwardNavigationHistory.Last();
            _forwardNavigationHistory.Remove(CurrentView);
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

        public void ToggleQueueDrawer() { IsQueueDrawerOpen = !IsQueueDrawerOpen; }
    }

    //public class NavigationService : ObservableObject
    //{
    //    private readonly Func<Type, Type> _viewModelFactory;

    //    public event Action IsFullScreenChanged;
    //    private void OnIsFullScreenChanged()
    //    {
    //        IsFullScreenChanged?.Invoke();
    //    }

    //    private bool _isPopupOpen;
    //    public bool IsPopupOpen
    //    {
    //        get { return _isPopupOpen; }
    //        set 
    //        {
    //            SetField(ref _isPopupOpen, value);
    //        }
    //    }

    //    private List<NavigationModel> FormerViews { get; set; } = new();

    //    private Type _currentViewModel;
    //    public Type CurrentViewModel
    //    {
    //        get { return _currentViewModel; }
    //        set
    //        {
    //            CurrentViewModel?.Dispose();
    //            SetField(ref _currentViewModel, value);
    //            CurrentViewChanged?.Invoke(CurrentViewName);
    //        }
    //    }

    //    public ViewNameEnum CurrentViewName { get; set; }

    //    private BaseModel _currentViewParameter;
    //    public BaseModel CurrentViewParameter
    //    {
    //        get => _currentViewParameter;
    //        set { SetField(ref _currentViewParameter, value); }
    //    }

    //    public event Action<ViewNameEnum> CurrentViewChanged;

    //    private Type _secondaryViewModel = new();
    //    public Type SecondaryViewModel
    //    {
    //        get { return _secondaryViewModel; }
    //        set
    //        {
    //            SecondaryViewModel?.Dispose();
    //            SetField(ref _secondaryViewModel, value);
    //        }
    //    }

    //    public ViewNameEnum ScdViewName { get; set; }

    //    private BaseModel _scdViewParameter;
    //    public BaseModel ScdViewParameter
    //    {
    //        get => _scdViewParameter;
    //        set { SetField(ref _scdViewParameter, value); }
    //    }

    //    private Type _popupViewModel = new();
    //    public Type PopupViewModel
    //    {
    //        get { return _popupViewModel; }
    //        private set
    //        {
    //            PopupViewModel?.Dispose();
    //            SetField(ref _popupViewModel, value);
    //        }
    //    }

    //    public ViewNameEnum PopupViewName { get; set; }


    //    private BaseModel _popupViewParameter;
    //    public BaseModel PopupViewParameter
    //    {
    //        get => _popupViewParameter;
    //        set { SetField(ref _popupViewParameter, value); }
    //    }

    //    public NavigationService(Func<Type, Type> viewModelFactory)
    //    {
    //        _viewModelFactory = viewModelFactory;
    //        ConfigurationService.QueueCoversChange += OnCoverSettingsChange;
    //    }

    //    private void OnCoverSettingsChange()
    //    {
    //        if (CurrentViewName == ViewNameEnum.SpecificAlbum ||
    //            CurrentViewName == ViewNameEnum.SpecificArtist ||
    //            CurrentViewName == ViewNameEnum.SpecificPlaylist ||
    //            CurrentViewName == ViewNameEnum.SpecificGenre)
    //        {
    //            CurrentViewModel.Update();
    //        }
    //    }

    //    public void NavigateTo<TViewModel>(BaseModel parameter = null, bool saveView = true) where TViewModel : Type
    //    {
    //        // need to set the parameter first because the view model gets created when _viewModelFactory is invoked
    //        CurrentViewParameter = parameter;
    //        CurrentViewModel = _viewModelFactory.Invoke(typeof(TViewModel));

    //        if(saveView)
    //            FormerViews.Insert(0, new(CurrentViewModel, parameter, CurrentViewName));
    //    }

    //    public void NavigateToSecondaryView<TViewModel>(BaseModel parameter = null) where TViewModel : Type
    //    {
    //        ScdViewParameter = parameter;
    //        SecondaryViewModel = _viewModelFactory.Invoke(typeof(TViewModel));
    //    }

    //    public void NavigateTo(string viewName, BaseModel parameter = null)
    //    {
    //        NavigateTo(viewName.ToViewNameEnum(), parameter);
    //    }

    //    public void NavigateTo(ViewNameEnum viewName, BaseModel parameter = null, bool saveView = true)
    //    {
    //        switch (viewName)
    //        {
    //            case ViewNameEnum.Empty:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<EmptyViewModel>();
    //                break;
    //            case ViewNameEnum.Home:
    //                CurrentViewName = viewName;
    //                NavigateTo<HomeViewModel>(saveView: saveView);
    //                break;
    //            case ViewNameEnum.Albums:
    //                CurrentViewName = viewName;
    //                NavigateTo<AlbumLibraryViewModel>(saveView: saveView);
    //                break;
    //            case ViewNameEnum.Artists:
    //                CurrentViewName = viewName;
    //                NavigateTo<ArtistLibraryViewModel>(saveView: saveView);
    //                break;
    //            case ViewNameEnum.Playlists:
    //                CurrentViewName = viewName;
    //                NavigateTo<PlaylistLibraryViewModel>(saveView: saveView);
    //                break;
    //            case ViewNameEnum.Genres:
    //                CurrentViewName = viewName;
    //                NavigateTo<GenreLibraryViewModel>(saveView: saveView);
    //                break;
    //            case ViewNameEnum.Settings:
    //                CurrentViewName = viewName;
    //                NavigateTo<SettingsViewModel>(saveView: saveView);
    //                break;
    //            case ViewNameEnum.SpecificAlbum:
    //                CurrentViewName = viewName;
    //                NavigateTo<AlbumViewModel>(parameter, saveView);
    //                break;
    //            case ViewNameEnum.SpecificArtist:
    //                CurrentViewName = viewName;
    //                NavigateTo<ArtistViewModel>(parameter, saveView);
    //                break;
    //            case ViewNameEnum.SpecificPlaylist:
    //                CurrentViewName = viewName;
    //                NavigateTo<PlaylistViewModel>(parameter, saveView);
    //                break;
    //            case ViewNameEnum.SpecificGenre:
    //                CurrentViewName = viewName;
    //                NavigateTo<GenreViewModel>(parameter, saveView);
    //                break;
    //            case ViewNameEnum.NowPlaying:
    //                CurrentViewName = viewName;
    //                NavigateTo<NowPlayingViewModel>(saveView: saveView);
    //                break;
    //            case ViewNameEnum.Queue:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<QueueViewModel>();
    //                break;
    //            case ViewNameEnum.Lyrics:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<LyricsViewModel>();
    //                break;
    //            case ViewNameEnum.TrackInfo:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<TrackInfoViewModel>();
    //                break;
    //            case ViewNameEnum.General:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<GeneralSettingsViewModel>();
    //                break;
    //            case ViewNameEnum.Import:
    //                CurrentViewName = viewName;
    //                NavigateToSecondaryView<StorageSettingsViewModel>();
    //                break;
    //            case ViewNameEnum.AppTheme:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<AppThemeSettingViewModel>();
    //                break;
    //            case ViewNameEnum.Language:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<LanguageSettingViewModel>();
    //                break;
    //            case ViewNameEnum.DSP:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<DSPSettingsViewModels>();
    //                break;
    //            case ViewNameEnum.Visualizer:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<VisualizerSettingViewModel>();
    //                break;
    //            case ViewNameEnum.Shortcuts:
    //                ScdViewName = viewName;
    //                NavigateToSecondaryView<ShortcutSettingViewModel>();
    //                break;
    //            default:
    //                ScdViewName = viewName;
    //                NavigateTo<EmptyViewModel>(saveView: saveView);
    //                break;
    //        }
    //    }

    //    public void NavigateBack()
    //    {
    //        if (FormerViews.Count > 1)
    //        {
    //            FormerViews.RemoveAt(0);

    //            NavigationModel navigationModel = FormerViews[0];
    //            NavigateTo(navigationModel.ViewName, navigationModel.Parameter, false);
    //        }
    //    }

    //    public void OpenPopup(ViewNameEnum viewName, BaseModel parameter)
    //    {
    //        if (parameter is not null)
    //        {
    //            bool open = true;

    //            PopupViewParameter = parameter;
    //            PopupViewName = viewName;
    //            switch (viewName)
    //            {
    //                case ViewNameEnum.TrackPopup:
    //                    PopupViewModel = _viewModelFactory?.Invoke(typeof(TrackPopupViewModel));
    //                    break;
    //                case ViewNameEnum.AlbumPopup:
    //                    PopupViewModel = _viewModelFactory?.Invoke(typeof(AlbumPopupViewModel));
    //                    break;
    //                case ViewNameEnum.ArtistPopup:
    //                    PopupViewModel = _viewModelFactory?.Invoke(typeof(ArtistPopupViewModel));
    //                    break;
    //                case ViewNameEnum.PlaylistPopup:
    //                    PopupViewModel = _viewModelFactory?.Invoke(typeof(PlaylistPopupViewModel));
    //                    break;
    //                case ViewNameEnum.TagPopup:
    //                    PopupViewModel = _viewModelFactory?.Invoke(typeof(TagPopupViewModel));
    //                    break;
    //                default:
    //                    open = false;
    //                    break;
    //            }

    //            if (open)
    //            {
    //                if (IsPopupOpen)
    //                {
    //                    ClosePopup();
    //                }
    //                IsPopupOpen = true;
    //            }
    //        }
    //    }

    //    public void ClosePopup()
    //    {
    //        _isPopupOpen = false;
    //        OnPropertyChanged(nameof(IsPopupOpen));
    //    }

    //    public void ClosePopup(BaseModel parameter)
    //    {
    //        if (parameter.Equals(PopupViewParameter))
    //        {
    //            ClosePopup();
    //        }
    //    }
    //}
}
