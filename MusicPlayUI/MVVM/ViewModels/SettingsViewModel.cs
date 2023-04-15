using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.MVVM.Models;
using System.Collections.Generic;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private INavigationService _navigationService;
        public INavigationService NavigationService 
        {
            get => _navigationService;
            set { SetField(ref _navigationService, value); }
        }

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

        private List<SettingModel> _settings;
        public List<SettingModel> Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        private SettingModel _selectedSetting;
        private readonly IWindowService _windowService;

        public SettingModel SelectedSetting
        {
            get
            {
                if (_selectedSetting is null)
                    return Settings.Find(s => s.IsSelected);
                return _selectedSetting;
            }
            set
            {
                _selectedSetting = value;
                OnPropertyChanged(nameof(SelectedSetting));
            }
        }

        public ICommand NavigateToSettingCommand { get; }
        public SettingsViewModel(INavigationService navigationService, IWindowService windowService)
        {
            NavigationService = navigationService;
            _windowService = windowService;

            // navigate to general settings by default (sub view)
            NavigationService.NavigateTo(ViewNameEnum.General);

            NavigateToSettingCommand = new RelayCommand<SettingModel>((setting) =>
            {
                NavigateToSettings(setting);
            });

            Init();
        }

        public override void Dispose()
        {
            //NavigationService.DisposeSettingView();
        }

        private void Init()
        {
            Settings = SettingsModelFactory.GetSettings();
            NavigateToSettings(SelectedSetting);
            IsSettingsMenuOpen = true;
        }

        private void SetSelectedSetting(SettingModel setting)
        {
            if (setting is not null)
            {
                SelectedSetting = setting;
                Settings.ForEach(s => s.IsSelected = false);
                Settings.Find(s => s.SettingName == setting.SettingName).IsSelected = true;
                Settings = new(Settings);
            }
        }

        private void NavigateToSettings(SettingModel setting)
        {
            if (setting is null)
                return;

            SetSelectedSetting(setting);
            NavigationService.NavigateTo(setting.SettingViewEnum);
        }
    }
}
