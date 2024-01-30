using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.MVVM.Models;
using System.Collections.Generic;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicScrollViewer;
using MusicPlayUI.Core.Services;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private readonly IWindowService _windowService;

        private bool _isSettingsMenuOpen = false;
        public bool IsSettingsMenuOpen
        {
            get { return _isSettingsMenuOpen; }
            set
            {
                _isSettingsMenuOpen = value;
                OnPropertyChanged(nameof(IsSettingsMenuOpen));
            }
        }

        private ObservableCollection<SettingModel> _settings;
        public ObservableCollection<SettingModel> Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        private SettingModel _selectedSetting;
        public SettingModel SelectedSetting
        {
            get
            {
                return _selectedSetting;
            }
            set
            {
                _selectedSetting = value;
                OnPropertyChanged(nameof(SelectedSetting));
                AppBar.SetData(SelectedSetting.Name, string.Empty);
            }
        }

        public ICommand NavigateToSettingCommand { get; }
        public SettingsViewModel(IWindowService windowService)
        {
            _windowService = windowService;

            NavigateToSettingCommand = new RelayCommand<SettingModel>((setting) =>
            {
                NavigateToSettings(setting);
            });
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            AppBar.AnimateElevation(e.VerticalOffset, 
                                    10, 500,
                                    1000000, 0,
                                    100, 500,
                                    0.2, 0.8,
                                    1, 1,
                                    0.1, 0.8);
            base.OnScrollEvent(e);
        }

        public override void UpdateAppBarStyle()
        {
            AppBar.SetStyle(AppTheme.Palette.PrimaryContainer, 0.2, 1, false, 28);
            AppBar.SetDropShadow(0.1);
            AppBar.Foreground = AppTheme.Palette.OnBackground;
        }

        public override void Init()
        {
            Settings = new(SettingsModelFactory.GetSettings());

            if(State?.ChildViewModel?.ViewModel != null)
            {
                Type childViewModelType = State.ChildViewModel.ViewModel.GetType();
                //SettingModel setting = Settings.FirstOrDefault(s => s.Type == childViewModelType);
                //SetSelectedSetting(setting);
                SetSelectedSetting(s => s.Type == childViewModelType);
                IsSettingsMenuOpen = true;
                base.Init();
                return;
            }
            else if (State?.Parameter != null && State.Parameter is SettingModel setting)
            {
                SelectedSetting = setting;
            }

            if(SelectedSetting is null)
            {
                SetSelectedSetting(s => s.IsSelected);
            }

            base.Init();
            NavigateToSettings(SelectedSetting);
            IsSettingsMenuOpen = true;
        }

        private void SetSelectedSetting(Predicate<SettingModel> predicate)
        {
            foreach (SettingModel setting in Settings)
            {
                if (setting.IsSelected)
                {
                    setting.IsSelected = false;
                }
                if(predicate(setting))
                {
                    SelectedSetting = setting;
                    setting.IsSelected = true;
                }
            }
        }

        private void SetSelectedSetting(SettingModel setting)
        {
            if (setting is not null)
            {
                SelectedSetting = setting;

                foreach(SettingModel settings in Settings)
                {
                    if (settings.IsSelected)
                    {
                        settings.IsSelected = false;
                    } 
                    else if(settings.Name == setting.Name)
                    {
                        settings.IsSelected = true;
                    }
                }
            }
        }

        private void NavigateToSettings(SettingModel setting)
        {
            if (setting is null)
                return;

            SetSelectedSetting(setting);
            State.ChildViewModel = App.State.CreateNavigationModel(setting.Type);
        }
    }
}
