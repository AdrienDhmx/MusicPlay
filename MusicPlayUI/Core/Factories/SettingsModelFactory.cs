using MusicPlayUI.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using MusicPlay.Language;
using System.Text;
using System.Threading.Tasks;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using System.Windows.Media;

namespace MusicPlayUI.Core.Factories
{
    public static class SettingsModelFactory
    {
        private static readonly SolidColorBrush LightDefaultAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#5954a8");
        private static readonly SolidColorBrush LightWaterAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#365bad");
        private static readonly SolidColorBrush LightTurquoiseAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#006b5f");
        private static readonly SolidColorBrush LightForestAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#426915");
        private static readonly SolidColorBrush LightFallenLeavesAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#855300");
        private static readonly SolidColorBrush LightRedWineAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#984061");


        private static readonly SolidColorBrush DarkDefaultAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#c5c0ff");
        private static readonly SolidColorBrush DarkWaterAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#b2c5ff");
        private static readonly SolidColorBrush DarkTurquoiseAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#54dbc7");
        private static readonly SolidColorBrush DarkForestAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#a7d474");
        private static readonly SolidColorBrush DarkFallenLeavesAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffb95f");
        private static readonly SolidColorBrush DarkRedWineAccentColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffb1c8");

        public static List<AppThemeModel> GetExistingThemes()
        {
            if (AppThemeService.IsLightTheme)
            {
                return new()
                {
                    new("Default Theme", "", SettingsValueEnum.DefaultTheme, LightDefaultAccentColor),
                    new("Water Theme", "", SettingsValueEnum.WaterTheme, LightWaterAccentColor),
                    new("Turquoise Theme", "", SettingsValueEnum.TurquoiseTheme, LightTurquoiseAccentColor),
                    new("Forest Theme", "", SettingsValueEnum.ForestTheme, LightForestAccentColor),
                    new("Fallen Leaves Theme", "", SettingsValueEnum.FallenLeavesTheme, LightFallenLeavesAccentColor),
                    new("Red Wine Theme", "", SettingsValueEnum.RedWineTheme, LightRedWineAccentColor),
                };
            }
            else
            {
                return new()
                {
                    new("Default Theme", "", SettingsValueEnum.DefaultTheme, DarkDefaultAccentColor),
                    new("Water Theme", "", SettingsValueEnum.WaterTheme, DarkWaterAccentColor),
                    new("Turquoise Theme", "", SettingsValueEnum.TurquoiseTheme, DarkTurquoiseAccentColor),
                    new("Forest Theme", "", SettingsValueEnum.ForestTheme, DarkForestAccentColor),
                    new("Fallen Leaves Theme", "", SettingsValueEnum.FallenLeavesTheme, DarkFallenLeavesAccentColor),
                    new("Red Wine Theme", "", SettingsValueEnum.RedWineTheme, DarkRedWineAccentColor),
                };
            }
        }

        public static List<SettingValueModel<ViewNameEnum>> GetStartingViews()
        {
            List<SettingValueModel<ViewNameEnum>> startingViews = new()
            {
                new(Resources.Home_View, "", ViewNameEnum.Home),
                new(Resources.Albums_View, "", ViewNameEnum.Albums),
                new(Resources.Artists_View, "", ViewNameEnum.Artists),
                new(Resources.Playlists_View, "", ViewNameEnum.Playlists),
                new(Resources.Now_Playing_View, "", ViewNameEnum.NowPlaying)
            };
            return startingViews;
        }

        public static List<SettingValueModel<SettingsValueEnum>> GetLanguages()
        {
            List<SettingValueModel<SettingsValueEnum>> languages = new()
            {
                new(Resources.English_Lang, Resources.English_Lang, SettingsValueEnum.English),
                new(Resources.French_Lang, Resources.French_Lang, SettingsValueEnum.French)
            };
            return languages;
        }

        public static List<SettingModel> GetSettings()
        {
            List<SettingModel> settings = new()
            {
                new(ViewNameEnum.General, Resources.General_Setting, "", true),
                new(ViewNameEnum.Import, Resources.Storage, "", false),
                new(ViewNameEnum.AppTheme, Resources.Themes_Setting, ""),
                new(ViewNameEnum.Language, Resources.Language_Setting, ""),
                new(ViewNameEnum.DSP, "Equalizer", ""), 
                new(ViewNameEnum.Visualizer, Resources.Visualizer, ""),
                new(ViewNameEnum.Shortcuts, "Shortcuts", ""),
            };
            return settings;
        }

        public static List<SettingValueModel<SettingsValueEnum>> GetQueueCovers()
        {
            List<SettingValueModel<SettingsValueEnum>> covers = new()
            {
                new(Resources.AutoCover, "", SettingsValueEnum.AutoCovers),
                new(Resources.AlbumCover, "", SettingsValueEnum.AlbumCovers),
                new(Resources.Artwork, "", SettingsValueEnum.ArtworkCovers),
                new(Resources.None, "", SettingsValueEnum.NoCovers)
            };
            return covers;
        }

        public static List<SettingValueModel<int>> GetVisualizerDataQuantities()
        {
            List<SettingValueModel<int>> dataQuantities = new()
            {
                new("64", "", 64),
                new("128", "", 128),
                new("256", "", 256),
                new("512", "", 512),
                new("1024", "", 1024),
                new("2048", "", 2048),
                new("4096", "", 4096)
            };
            return dataQuantities;
        }

        public static List<SettingValueModel<int>> GetVisualizerRepresentations()
        {
            List<SettingValueModel<int>> representations = new()
            {
                new(Resources.LinearUpwardBar, "", 1),
                new(Resources.LinearDownwardBar, "", 2),
                new(Resources.CircledBar, "", 3),
                new(Resources.LinearMirroredBar, "", 4),
                new(Resources.LinearUpwardPoints, "", 11),
                new(Resources.LinearDownwardPoints, "", 12),
                new(Resources.CircledPoints, "", 13),
                new(Resources.LinearMirroredPoints, "", 14)
            };
            return representations;
        }

        public static List<SettingValueModel<int>> GetVisualizerRefreshRates()
        {
            List<SettingValueModel<int>> refreshRates = new()
            {
                new(Resources.Low + " (30fps)", "", 1),
                new(Resources.High + " (60fps)", "", 2),
                new(Resources.VeryHigh + " (120fps)", "", 3),
            };
            return refreshRates;
        }

        public static List<SettingValueModel<int>> GetVisualizerObjectLengths()
        {
            List<SettingValueModel<int>> refreshRates = new()
            {
                new(Resources.ExtraLarge, "", 100),
                new(Resources.Large, "", 90),
                new(Resources.Normal, "", 75),
                new(Resources.Thin, "", 60),
                new(Resources.ExtraThin, "", 40)
            };
            return refreshRates;
        }

    }
}
