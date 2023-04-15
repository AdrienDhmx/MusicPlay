﻿using System;
using System.Drawing;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MessageControl;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace MusicPlayUI.Core.Services
{
    public static class ConfigurationService
    {
        public static event Action QueueCoversChange;
        private static void OnQueueCoverChanged()
        {
            QueueCoversChange?.Invoke();
        }

        public static int GetPreference(SettingsEnum key)
        {
            string storedInt;
            storedInt = key switch
            {
                SettingsEnum.MainStartingView => Settings.Default.MainStartingView,
                SettingsEnum.AppTheme => Settings.Default.AppTheme,
                SettingsEnum.LightTheme => Settings.Default.LightTheme,
                SettingsEnum.SunsetSunrise => Settings.Default.SunsetSunrise,
                SettingsEnum.SystemSyncTheme => Settings.Default.SystemSyncTheme,
                SettingsEnum.Language => Settings.Default.Language,
                SettingsEnum.TimerInterval => Settings.Default.TimerInterval,
                SettingsEnum.NowPlayingStartingSubView => Settings.Default.NowPlayingStartingSubView,
                SettingsEnum.QueueCovers => Settings.Default.QueueCover,
                SettingsEnum.AutoChangeOutputdevice => Settings.Default.AutoChangeOutputDevice,
                SettingsEnum.ArtistFilter => Settings.Default.ArtistsFilter,
                SettingsEnum.ArtistOrder => Settings.Default.ArtistOrder,
                SettingsEnum.AlbumFilter => Settings.Default.AlbumFilter,
                SettingsEnum.AlbumOrder => Settings.Default.AlbumOrder,
                SettingsEnum.LyricsFontSize => Settings.Default.LyricsFontSize,
                SettingsEnum.Volume => Settings.Default.Volume,
                SettingsEnum.VDataQt => Settings.Default.VDataQt,
                SettingsEnum.VRepresentation => Settings.Default.VRepresentation,
                SettingsEnum.VGradient => Settings.Default.VGradient,
                SettingsEnum.VFill => Settings.Default.VFill,
                SettingsEnum.VStrokeThickness => Settings.Default.VStrokeThickness,
                SettingsEnum.VObjectLength => Settings.Default.VObjectLength,
                SettingsEnum.VCutHighFreq => Settings.Default.VCutHighFreq,
                SettingsEnum.VCutPercentage => Settings.Default.VCutPercentage,
                SettingsEnum.VisualizerVisibility => Settings.Default.VisualizerVisibility,
                SettingsEnum.NowPlayingCoverVisibility => Settings.Default.NowPlayingCoverVisibility,
                SettingsEnum.VRefreshRate => Settings.Default.VRefreshRate,
                SettingsEnum.VAutoColor => Settings.Default.VAutoColor,
                SettingsEnum.VCenterFreq => Settings.Default.VCenterFreq,
                _ => "",
            };
            return int.TryParse(storedInt, out int id) ? id : -2;
        }

        public static string GetStringPreference(SettingsEnum key)
        {
            return key switch
            {
                SettingsEnum.AlbumFilter => Settings.Default.AlbumFilter,
                SettingsEnum.ArtistFilter => Settings.Default.ArtistsFilter,
                SettingsEnum.AlbumOrder => Settings.Default.AlbumOrder,
                SettingsEnum.ArtistOrder => Settings.Default.ArtistOrder,
                SettingsEnum.UserName => Settings.Default.UserName,
                _ => "",
            };
        }

        public static SolidColorBrush GetVColor()
        {
            string value = Settings.Default.VColor;
            try
            {
                SolidColorBrush color = (SolidColorBrush)new BrushConverter().ConvertFrom(value);
                return color;
            }
            catch (Exception)
            {
                return new();
            }
        }

        /// <summary>
        /// Save the string as the value of the corresponding setting
        /// </summary>
        /// <param name="key"> the setting the value correspond to </param>
        /// <param name="value"> the value to save </param>
        /// <param name="force"> force the value even if it is valid (empty string or null value) </param>
        /// <returns></returns>
        public static bool SetPreference(SettingsEnum key, string value, bool force = false)
        {
            if (string.IsNullOrWhiteSpace(value) && !force)
                return false;

            switch (key)
            {
                case SettingsEnum.MainStartingView:
                    Settings.Default.MainStartingView = value;
                    break;
                case SettingsEnum.AppTheme:
                    Settings.Default.AppTheme = value;
                    break;
                case SettingsEnum.LightTheme:
                    Settings.Default.LightTheme = value;
                    break;
                case SettingsEnum.SunsetSunrise:
                    Settings.Default.SunsetSunrise = value;
                    break;
                case SettingsEnum.SystemSyncTheme:
                    Settings.Default.SystemSyncTheme = value;
                    break;
                case SettingsEnum.Language:
                    Settings.Default.Language = value;
                    break;
                case SettingsEnum.TimerInterval:
                    Settings.Default.TimerInterval = value;
                    break;
                case SettingsEnum.NowPlayingStartingSubView:
                    Settings.Default.NowPlayingStartingSubView = value;
                    break;
                case SettingsEnum.QueueCovers:
                    Settings.Default.QueueCover = value;
                    OnQueueCoverChanged();
                    break;
                case SettingsEnum.AutoChangeOutputdevice:
                    Settings.Default.AutoChangeOutputDevice = value;
                    break;
                case SettingsEnum.LyricsFontSize:
                    Settings.Default.LyricsFontSize = value;
                    break;
                case SettingsEnum.Volume:
                    Settings.Default.Volume = value;
                    break;
                case SettingsEnum.UserName:
                    Settings.Default.UserName = value;
                    break;
                case SettingsEnum.ArtistFilter:
                    Settings.Default.ArtistsFilter = value;
                    break;
                case SettingsEnum.ArtistOrder:
                    Settings.Default.ArtistOrder = value;
                    break;
                case SettingsEnum.AlbumFilter:
                    Settings.Default.AlbumFilter = value;
                    break;
                case SettingsEnum.AlbumOrder:
                    Settings.Default.AlbumOrder = value;
                    break;
                case SettingsEnum.VColor:
                    Settings.Default.VColor = value;
                    break;
                case SettingsEnum.VDataQt:
                    Settings.Default.VDataQt = value;
                    break;
                case SettingsEnum.VRepresentation:
                    Settings.Default.VRepresentation = value;
                    break;
                case SettingsEnum.VGradient:
                    Settings.Default.VGradient = value;
                    break;
                case SettingsEnum.VFill:
                    Settings.Default.VFill = value;
                    break;
                case SettingsEnum.VStrokeThickness:
                    Settings.Default.VStrokeThickness = value;
                    break;
                case SettingsEnum.VObjectLength:
                    Settings.Default.VObjectLength = value;
                    break;
                case SettingsEnum.VCutHighFreq:
                    Settings.Default.VCutHighFreq = value;
                    break;
                case SettingsEnum.VCutPercentage:
                    Settings.Default.VCutPercentage = value;
                    break;
                case SettingsEnum.VisualizerVisibility:
                    Settings.Default.VisualizerVisibility = value;
                    break;
                case SettingsEnum.NowPlayingCoverVisibility:
                    Settings.Default.NowPlayingCoverVisibility = value;
                    break;
                case SettingsEnum.VRefreshRate:
                    Settings.Default.VRefreshRate = value;
                    break;
                case SettingsEnum.VAutoColor:
                    Settings.Default.VAutoColor = value;
                    break;
                case SettingsEnum.VCenterFreq:
                    Settings.Default.VCenterFreq = value;
                    break;
                case SettingsEnum.UNKNOWN:
                    return false;
                default:
                    return false;
            }
            Settings.Default.Save();
            return true;
        }
    }
}
