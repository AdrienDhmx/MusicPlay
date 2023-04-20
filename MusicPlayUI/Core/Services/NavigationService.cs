using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.PopupViewModels;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using System;
using System.Collections.Generic;

namespace MusicPlayUI.Core.Services
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly Func<Type, ViewModel> _viewModelFactory;

        private bool _isFullScreen;
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set { 
                SetField(ref _isFullScreen, value);
                OnIsFullScreenChanged();
            }
        }

        private bool _isQueueDrawerOpen;
        public bool IsQueueDrawerOpen
        {
            get { return _isQueueDrawerOpen; }
            set
            {
                SetField(ref _isQueueDrawerOpen, value);
            }
        }

        public event Action IsFullScreenChanged;
        private void OnIsFullScreenChanged()
        {
            IsFullScreenChanged?.Invoke();
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set 
            {
                SetField(ref _isPopupOpen, value);
            }
        }

        private List<NavigationModel> FormerViews { get; set; } = new();

        private ViewModel _currentViewModel;
        public ViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                CurrentViewModel?.Dispose();
                SetField(ref _currentViewModel, value);
                CurrentViewChanged?.Invoke(CurrentViewName);
            }
        }

        public ViewNameEnum CurrentViewName { get; set; }

        private BaseModel _currentViewParameter;
        public BaseModel CurrentViewParameter
        {
            get => _currentViewParameter;
            set { SetField(ref _currentViewParameter, value); }
        }

        public event Action<ViewNameEnum> CurrentViewChanged;

        private ViewModel _secondaryViewModel = new();
        public ViewModel SecondaryViewModel
        {
            get { return _secondaryViewModel; }
            set
            {
                SecondaryViewModel?.Dispose();
                SetField(ref _secondaryViewModel, value);
            }
        }

        public ViewNameEnum ScdViewName { get; set; }

        private BaseModel _scdViewParameter;
        public BaseModel ScdViewParameter
        {
            get => _scdViewParameter;
            set { SetField(ref _scdViewParameter, value); }
        }

        private ViewModel _popupViewModel = new();
        public ViewModel PopupViewModel
        {
            get { return _popupViewModel; }
            private set
            {
                PopupViewModel?.Dispose();
                SetField(ref _popupViewModel, value);
            }
        }

        public ViewNameEnum PopupViewName { get; set; }


        private BaseModel _popupViewParameter;
        public BaseModel PopupViewParameter
        {
            get => _popupViewParameter;
            set { SetField(ref _popupViewParameter, value); }
        }

        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        private void NavigateTo<TViewModel>(BaseModel parameter = null, bool saveView = true) where TViewModel : ViewModel
        {
            // need to set the parameter first because the viewmodel gets created when _viewModelFactory is invoked
            CurrentViewParameter = parameter;
            CurrentViewModel = _viewModelFactory.Invoke(typeof(TViewModel));

            if(saveView)
                FormerViews.Insert(0, new(CurrentViewModel, parameter, CurrentViewName));
        }

        private void NavigateToSecondaryView<TViewModel>(BaseModel parameter = null) where TViewModel : ViewModel
        {
            ScdViewParameter = parameter;
            SecondaryViewModel = _viewModelFactory.Invoke(typeof(TViewModel));
        }

        public void NavigateTo(string viewName, BaseModel parameter = null)
        {
            NavigateTo(viewName.ToViewNameEnum(), parameter);
        }

        public void NavigateTo(ViewNameEnum viewName, BaseModel parameter = null, bool saveView = true)
        {
            switch (viewName)
            {
                case ViewNameEnum.Empty:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<EmptyViewModel>();
                    break;
                case ViewNameEnum.Home:
                    CurrentViewName = viewName;
                    NavigateTo<HomeViewModel>(saveView: saveView);
                    break;
                case ViewNameEnum.Albums:
                    CurrentViewName = viewName;
                    NavigateTo<AlbumLibraryViewModel>(saveView: saveView);
                    break;
                case ViewNameEnum.Artists:
                    CurrentViewName = viewName;
                    NavigateTo<ArtistLibraryViewModel>(saveView: saveView);
                    break;
                case ViewNameEnum.Playlists:
                    CurrentViewName = viewName;
                    NavigateTo<PlaylistLibraryViewModel>(saveView: saveView);
                    break;
                case ViewNameEnum.Import:
                    CurrentViewName = viewName;
                    NavigateTo<ImportLibraryViewModel>(saveView: saveView);
                    break;
                case ViewNameEnum.Settings:
                    CurrentViewName = viewName;
                    NavigateTo<SettingsViewModel>(saveView: saveView);
                    break;
                case ViewNameEnum.SpecificAlbum:
                    CurrentViewName = viewName;
                    NavigateTo<AlbumViewModel>(parameter, saveView);
                    break;
                case ViewNameEnum.SpecificArtist:
                    CurrentViewName = viewName;
                    NavigateTo<ArtistViewModel>(parameter, saveView);
                    break;
                case ViewNameEnum.SpecificPlaylist:
                    CurrentViewName = viewName;
                    NavigateTo<PlaylistViewModel>(parameter, saveView);
                    break;
                case ViewNameEnum.SpecificGenre:
                    CurrentViewName = viewName;
                    NavigateTo<GenreViewModel>(parameter, saveView);
                    break;
                case ViewNameEnum.NowPlaying:
                    CurrentViewName = viewName;
                    NavigateTo<NowPlayingViewModel>(saveView: saveView);
                    break;
                case ViewNameEnum.Queue:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<QueueViewModel>();
                    break;
                case ViewNameEnum.Lyrics:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<LyricsViewModel>();
                    break;
                case ViewNameEnum.TrackInfo:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<TrackInfoViewModel>();
                    break;
                case ViewNameEnum.General:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<GeneralSettingsViewModel>();
                    break;
                case ViewNameEnum.AppTheme:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<AppThemeSettingViewModel>();
                    break;
                case ViewNameEnum.Language:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<LanguageSettingViewModel>();
                    break;
                case ViewNameEnum.Visualizer:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<VisualizerSettingViewModel>();
                    break;
                case ViewNameEnum.Shortcuts:
                    ScdViewName = viewName;
                    NavigateToSecondaryView<ShortcutSettingViewModel>();
                    break;
                default:
                    ScdViewName = viewName;
                    NavigateTo<EmptyViewModel>(saveView: saveView);
                    break;
            }
        }

        public void NavigateBack()
        {
            if (FormerViews.Count > 1)
            {
                FormerViews.RemoveAt(0);

                NavigationModel navigationModel = FormerViews[0];
                NavigateTo(navigationModel.ViewName, navigationModel.Parameter, false);
            }
        }

        public void OpenPopup(ViewNameEnum viewName, BaseModel parameter)
        {
            if (parameter is not null)
            {
                bool open = true;

                PopupViewParameter = parameter;
                switch (viewName)
                {
                    case ViewNameEnum.TrackPopup:
                        PopupViewModel = _viewModelFactory?.Invoke(typeof(TrackPopupViewModel));
                        break;
                    case ViewNameEnum.AlbumPopup:
                        PopupViewModel = _viewModelFactory?.Invoke(typeof(AlbumPopupViewModel));
                        break;
                    case ViewNameEnum.ArtistPopup:
                        PopupViewModel = _viewModelFactory?.Invoke(typeof(ArtistPopupViewModel));
                        break;
                    case ViewNameEnum.PlaylistPopup:
                        PopupViewModel = _viewModelFactory?.Invoke(typeof(PlaylistPopupViewModel));
                        break;
                    default:
                        open = false;
                        break;
                }
                if (open)
                {
                    if (IsPopupOpen)
                    {
                        ClosePopup();
                    }
                    IsPopupOpen = true;
                }
            }
        }

        public void ClosePopup()
        {
            _isPopupOpen = false;
            OnPropertyChanged(nameof(IsPopupOpen));
        }

        public void ClosePopup(BaseModel parameter)
        {
            if (parameter.Equals(PopupViewParameter))
            {
                ClosePopup();
            }
        }

        public void ToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;
        }

        public void ToggleQueueDrawer()
        {
            IsQueueDrawerOpen = !IsQueueDrawerOpen;
        }
    }
}
