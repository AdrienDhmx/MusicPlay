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

            Init();
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void Init()
        {
            Settings = new(SettingsModelFactory.GetSettings());

            if(State?.Parameter != null)
            {
                SelectedSetting = (SettingModel)State.Parameter;
            }
            else
            {
                SetSelectedSetting(Settings.FirstOrDefault(s => s.IsSelected));
            }

            NavigateToSettings(SelectedSetting);
            IsSettingsMenuOpen = true;
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
