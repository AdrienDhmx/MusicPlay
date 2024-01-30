using System.Collections.Generic;
using System.Windows.Input;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Commands;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class AppThemeSettingViewModel : BaseSettingViewModel
    {
        private string _appliedTheme;
        public string AppliedTheme
        {
            get
            {
                return _appliedTheme;
            }
            set
            {
                _appliedTheme = AppliedSetting(value);
                OnPropertyChanged(nameof(AppliedTheme));
            }
        }

        private List<AppThemeModel> _themes;
        public List<AppThemeModel> Themes
        {
            get { return _themes; }
            set
            {
                _themes = value;
                OnPropertyChanged(nameof(Themes));
            }
        }

        private bool _lightTheme;
        public bool LightTheme
        {
            get => _lightTheme;
            set
            {
                SetField(ref _lightTheme, value);
                _systemSync = false;
                _sunsetSunrise = false;

                // set the private values to avoid infinite loop
                // need to raise the OnPropertyChanged event manually
                OnPropertyChanged(nameof(SystemSync));
                OnPropertyChanged(nameof(SunsetSunrise));

                UpdateBool(LightTheme, SunsetSunrise, SystemSync);
            }
        }

        private bool _sunsetSunrise;
        public bool SunsetSunrise
        {
            get => _sunsetSunrise;
            set
            {
                SetField(ref _sunsetSunrise, value);
                _systemSync = false;

                // set the private values to avoid infinite loop
                // need to raise the OnPropertyChanged event manually
                OnPropertyChanged(nameof(SystemSync));

                UpdateBool(LightTheme, SunsetSunrise, SystemSync);
            }
        }

        private bool _systemSync;
        public bool SystemSync
        {
            get => _systemSync;
            set
            {
                SetField(ref  _systemSync, value);
                _sunsetSunrise = false;

                // set the private values to avoid infinite loop
                // need to raise the OnPropertyChanged event manually
                OnPropertyChanged(nameof(SunsetSunrise));

                UpdateBool(LightTheme, SunsetSunrise, SystemSync);
            }
        }

        private bool _colorfulPlayerControl;
        public bool ColorfulPlayerControl
        {
            get => _colorfulPlayerControl;
            set
            {
                SetField(ref _colorfulPlayerControl, value);
                SetPreference(SettingsEnum.ColorfulPlayerControl, BoolToString(_colorfulPlayerControl));
            }
        }

        public ICommand SetThemeCommand { get; }
        public AppThemeSettingViewModel()
        {            
            SetThemeCommand = new RelayCommand<SettingValueModel<SettingsValueEnum>>((theme) => 
            {
                SetNewTheme(theme);
            });

            AppTheme.ThemeChanged += Load;
            Load();
        }

        public override void Dispose()
        {
            Themes.Clear();
            AppTheme.ThemeChanged -= Load;
        }

        private void Load()
        {
            _lightTheme = AppTheme.IsLightTheme;
            _sunsetSunrise = ConfigurationService.GetPreference(SettingsEnum.SunsetSunrise) == 1;
            _systemSync = ConfigurationService.GetPreference(SettingsEnum.SystemSyncTheme) == 1;
            _colorfulPlayerControl = ConfigurationService.GetPreference(SettingsEnum.ColorfulPlayerControl) == 1;

            OnPropertyChanged(nameof(LightTheme));
            OnPropertyChanged(nameof(SunsetSunrise));
            OnPropertyChanged(nameof(SystemSync));
            OnPropertyChanged(nameof(ColorfulPlayerControl));

            LoadThemes();
        }

        private void LoadThemes()
        {
            SettingsValueEnum theme = (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.AppTheme);

            Themes = new(SettingsModelFactory.GetExistingThemes());
            UpdateSelectedTheme(theme);
        }

        private void UpdateBool(bool light, bool sunsetSunrise, bool systemSync)
        {
            SetPreference(SettingsEnum.LightTheme, BoolToString(light));
            SetPreference(SettingsEnum.SunsetSunrise, BoolToString(sunsetSunrise));
            SetPreference(SettingsEnum.SystemSyncTheme, BoolToString(systemSync));

            AppTheme.InitializeAppTheme();

            _lightTheme = AppTheme.IsLightTheme;
            OnPropertyChanged(nameof(LightTheme));

            LoadThemes();
        }

        private void SetNewTheme(SettingValueModel<SettingsValueEnum> theme, bool message = true)
        {
            UpdateSelectedTheme(theme.Value);

            SetPreference(SettingsEnum.AppTheme, ((int)theme.Value).ToString());
            AppliedTheme = theme.Name;
            // init new theme
            AppTheme.InitializeAppTheme();
        }

        private void UpdateSelectedTheme(SettingsValueEnum theme)
        {
            foreach (SettingValueModel<SettingsValueEnum> t in Themes)
            {
                if (t.Value == theme)
                {
                    t.IsSelected = true;
                    AppliedTheme = t.Name;
                }
                else
                    t.IsSelected = false;
            }
        }
    }
}
