using MessageControl;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Windows.UI.ViewManagement;

namespace MusicPlayUI.Core.Services
{
    public static class AppTheme
    {
        private static ResourceDictionary _appThemeDic = App.Current.Resources.MergedDictionaries[0];
        public static ResourceDictionary AppThemeDic
        {
            get => _appThemeDic;
            private set => _appThemeDic = value;
        }
        public static readonly ResourceDictionary IconDic = new() { Source = new Uri("Resources\\Icons.xaml", UriKind.Relative) };


        private static Timer _appThemeTimer;
        private static UISettings _uiSettings;

        public static class Palette
        {
            public static Brush Background => (Brush)AppThemeDic["Background"];
            public static Brush OnBackground => (Brush)AppThemeDic["OnBackground"];

            public static Brush SurfaceVariant => (Brush)AppThemeDic["SurfaceVariant"];
            public static Brush OnSurfaceVariant => (Brush)AppThemeDic["OnSurfaceVariant"];

            public static Brush Outline => (Brush)AppThemeDic["Outline"];

            public static Brush Primary => (Brush)AppThemeDic["Primary"];
            public static Brush OnPrimary => (Brush)AppThemeDic["OnPrimary"];
            public static Brush PrimaryContainer => (Brush)AppThemeDic["PrimaryContainer"];
            public static Brush OnPrimaryContainer => (Brush)AppThemeDic["OnPrimaryContainer"];
            public static Brush PrimaryHover => (Brush)AppThemeDic["PrimaryHover"];

            public static Brush Secondary => (Brush)AppThemeDic["Secondary"];
            public static Brush OnSecondary => (Brush)AppThemeDic["OnSecondary"];
            public static Brush SecondaryContainer => (Brush)AppThemeDic["SecondaryContainer"];
            public static Brush OnSecondaryContainer => (Brush)AppThemeDic["OnSecondaryContainer"];
            public static Brush SecondaryHover => (Brush)AppThemeDic["SecondaryHover"];

            public static Brush Tertiary => (Brush)AppThemeDic["Tertiary"];
            public static Brush OnTertiary => (Brush)AppThemeDic["OnTertiary"];
            public static Brush TertiaryContainer => (Brush)AppThemeDic["TertiaryContainer"];
            public static Brush OnTertiaryContainer => (Brush)AppThemeDic["OnTertiaryContainer"];
            public static Brush TertiaryHover => (Brush)AppThemeDic["TertiaryHover"];

            public static Brush Error => (Brush)AppThemeDic["Error"];
            public static Brush OnError => (Brush)AppThemeDic["OnError"];
            public static Brush ErrorContainer => (Brush)AppThemeDic["ErrorContainer"];
            public static Brush OnErrorContainer => (Brush)AppThemeDic["OnErrorContainer"];
            public static Brush ErrorHover => (Brush)AppThemeDic["ErrorHover"];

            public static Brush BlackForeground => (Brush)AppThemeDic["BlackForeground"];
            public static Brush WhiteForeground => (Brush)AppThemeDic["WhiteForeground"];
        }

        private const int _sunrise = 8;
        private const int _sunset = 20;

        public static bool IsLightTheme { get; private set; }
        public static bool IsSunsetSunrise { get; private set; }
        public static bool IsSystemSync { get; private set; }

        public static event Action ThemeChanged;
        private static void OnThemeChanged()
        {
            ThemeChanged?.Invoke();
        }

        public static void InitializeAppTheme()
        {
            SettingsValueEnum theme = (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.AppTheme);
            bool light = ConfigurationService.GetPreference(SettingsEnum.LightTheme) == 1;
            bool sunsetSunrise = ConfigurationService.GetPreference(SettingsEnum.SunsetSunrise) == 1;
            bool SystemSync = ConfigurationService.GetPreference(SettingsEnum.SystemSyncTheme) == 1;

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
            IsSunsetSunrise = sunsetSunrise;
            IsSystemSync = SystemSync;
            LoadAppTheme(theme, light);
        }

        private static void LoadAppTheme(SettingsValueEnum theme, bool light)
        {
            AppThemeDic = new ResourceDictionary() { Source = GetThemeColorsResourceDictionary(theme, light) };
            ResourceDictionary _themeDictionary = Application.Current.Resources.MergedDictionaries[0];
            _themeDictionary.MergedDictionaries.Clear();
            _themeDictionary.MergedDictionaries.Add(AppThemeDic);
            OnThemeChanged();
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
                    SettingsValueEnum.TurquoiseTheme => new Uri(@"\Resources\ThemeColors\LightTurquoiseThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.RedWineTheme => new Uri(@"\Resources\ThemeColors\LightRedWineThemeColors.xaml", UriKind.Relative),
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
                    SettingsValueEnum.TurquoiseTheme => new Uri(@"\Resources\ThemeColors\DarkTurquoiseThemeColors.xaml", UriKind.Relative),
                    SettingsValueEnum.RedWineTheme => new Uri(@"\Resources\ThemeColors\DarkRedWineThemeColors.xaml", UriKind.Relative),
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
