using MessageControl;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Windows.UI.ViewManagement;

namespace MusicPlayUI.Core.Services
{
    public static class AppThemeService
    {
        private static Timer _appThemeTimer;
        private static UISettings _uiSettings;

        private const int _sunrise = 8;
        private const int _sunset = 20;

        public static bool IsLightTheme { get; private set; }

        public static void InitializeAppTheme()
        {
            SettingsValueEnum theme = (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.AppTheme);
            bool light = ConfigurationService.GetPreference(SettingsEnum.LightTheme) == 1;
            bool sunsetSunrise = ConfigurationService.GetPreference(SettingsEnum.SunsetSunrise) == 1;
            bool SystemSync = ConfigurationService.GetPreference(SettingsEnum.SystemSyncTheme) == 1; ;

            if (sunsetSunrise)
            {
                double timeUntilChange;
                double time = DateTime.Now.TimeOfDay.TotalHours;
                if (time < _sunrise || time >= _sunset)
                {
                    light = false;
                    timeUntilChange = time < _sunrise ? _sunrise - time : (24 - time) + _sunrise;
                }
                else
                {
                    light = true;
                    timeUntilChange = _sunset - time;
                }

                if (_appThemeTimer is null)
                {
                    _appThemeTimer = new();
                    _appThemeTimer.Elapsed += AppThemeSunsetSunriseElapsed;
                    _appThemeTimer.AutoReset = false;
                    _appThemeTimer.Start();
                }

                _appThemeTimer.Interval = TimeSpan.FromHours(timeUntilChange).TotalMilliseconds;

                // save for coherence
                ConfigurationService.SetPreference(SettingsEnum.LightTheme, light ? "1" : "0");
            }
            else if (SystemSync)
            {
                light = GetSystemTheme();
            }

            IsLightTheme = light;
            LoadAppTheme(theme, light);
        }

        private static void LoadAppTheme(SettingsValueEnum theme, bool light)
        {
            ResourceDictionary _themeDictionary = Application.Current.Resources.MergedDictionaries[0];
            _themeDictionary.MergedDictionaries.Clear();
            _themeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = GetThemeColorsResourceDictionary(theme, light) });
        }

        private static Uri GetThemeColorsResourceDictionary(SettingsValueEnum theme, bool light)
        {
            if (light)
            {
                return theme switch
                {
                    SettingsValueEnum.DefaultTheme => new Uri(@"\Resources\ThemeColors\MainLightThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.ForestTheme => new Uri(@"\Resources\ThemeColors\LightForestThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.WaterTheme => new Uri(@"\Resources\ThemeColors\LightWaterThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.FallenLeavesTheme => new Uri(@"\Resources\ThemeColors\LightFallenLeavesThemeColors.xaml", UriKind.Relative),
                    _ => new Uri(@"\Resources\ThemeColors\MainLightThemeColors.xaml", UriKind.Relative),
                };
            }
            else
            {
                return theme switch
                {
                    SettingsValueEnum.DefaultTheme => new Uri(@"\Resources\ThemeColors\MainDarkThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.ForestTheme => new Uri(@"\Resources\ThemeColors\DarkForestThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.WaterTheme => new Uri(@"\Resources\ThemeColors\DarkWaterThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.FallenLeavesTheme => new Uri(@"\Resources\ThemeColors\DarkFallenLeavesThemeColors.xaml", UriKind.Relative),
                    _ => new Uri(@"\Resources\ThemeColors\MainDarkThemeColors.xaml", UriKind.Relative),
                };
            }
        }

        private static bool GetSystemTheme()
        {
            _uiSettings ??= new UISettings();

            // unsubscribe in case it was already
            _uiSettings.ColorValuesChanged -= WindowsThemeColorChanged;
            _uiSettings.ColorValuesChanged += WindowsThemeColorChanged;

            var color = _uiSettings.GetColorValue(
                                    UIColorType.Background
            );

            return color == Windows.UI.Color.FromArgb(255, 255, 255, 255);
        }

        private static void WindowsThemeColorChanged(UISettings sender, object args)
        {
            bool isLightTheme = GetSystemTheme();
            bool light = ConfigurationService.GetPreference(SettingsEnum.LightTheme) == 1;

            if (light != isLightTheme)
            {
                // save for coherence and to compare again later if needed
                ConfigurationService.SetPreference(SettingsEnum.LightTheme, isLightTheme ? "1" : "0");

                SettingsValueEnum theme = (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.AppTheme);
                LoadAppTheme(theme, isLightTheme);
            }
        }

        private static void AppThemeSunsetSunriseElapsed(object sender, ElapsedEventArgs e)
        {
            // re-init theme
            InitializeAppTheme();

            App.Current.Dispatcher.Invoke(() => 
            {
                MessageHelper.PublishMessage(MessageFactory.AppThemeLightChange());
            });
        }
    }
}
