using System;
using System.Windows.Media;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.Core.Services
{
    public static class ConfigurationService
    {
        public static event Action QueueCoversChange;
        private static void OnQueueCoverChanged()
        {
            UpdateCoverSettings();
            QueueCoversChange?.Invoke();
        }

        public static event Action ColorfulPlayerControlChange;
        private static void OnColorfulPlayerControlChanged()
        {
            ColorfulPlayerControlChange?.Invoke();
        }

        public static event Action ColorfulUIChange;
        private static void OnColorfulUIChanged()
        {
            ColorfulUIChange?.Invoke();
        }

        public static bool AreCoversEnabled { get; private set; } = true;

        public static bool AlbumCoverOnly { get; private set; } = false;

        public static bool ArtworkOnly { get; private set; } = false;

        public static bool AutoCover { get; private set; } = true;

        public static void Init()
        {
            UpdateCoverSettings();
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
                SettingsEnum.ColorfulPlayerControl => Settings.Default.ColorfulPlayerControl,
                SettingsEnum.ColorfulUI => Settings.Default.ColorfulUI,
                SettingsEnum.Language => Settings.Default.Language,
                SettingsEnum.TimerInterval => Settings.Default.TimerInterval,
                SettingsEnum.NowPlayingStartingSubView => Settings.Default.NowPlayingStartingSubView,
                SettingsEnum.QueueCovers => Settings.Default.QueueCover,
                SettingsEnum.AutoChangeOutputDevice => Settings.Default.AutoChangeOutputDevice,
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
                SettingsEnum.AutoForeground => Settings.Default.AutoForeground,
                SettingsEnum.EqualizerEnabled => Settings.Default.EqualizerEnabled,
                SettingsEnum.EqualizerPreset => Settings.Default.EqualizerPreset,
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
                SettingsEnum.PlayPause => Settings.Default.PlayPauseShortcut,
                SettingsEnum.NexTrack => Settings.Default.NextTrackShortcut,
                SettingsEnum.PreviousTrack => Settings.Default.PreviousTrackShortcut,
                SettingsEnum.Repeat => Settings.Default.RepeatShortcut,
                SettingsEnum.Shuffle => Settings.Default.ShuffleShortcut,
                SettingsEnum.ToggleFavorite => Settings.Default.ToggleFavShortcut,
                SettingsEnum.Rating0 => Settings.Default.Rating0Shortcut,
                SettingsEnum.Rating1 => Settings.Default.Rating1Shortcut,
                SettingsEnum.Rating2 => Settings.Default.Rating2Shortcut,
                SettingsEnum.Rating3 => Settings.Default.Rating3Shortcut,
                SettingsEnum.Rating4 => Settings.Default.Rating4Shortcut,
                SettingsEnum.Rating5 => Settings.Default.Rating5Shortcut,
                SettingsEnum.Home => Settings.Default.NavHomeShortcut,
                SettingsEnum.Albums => Settings.Default.NavAlbumsShortcut,
                SettingsEnum.Artists => Settings.Default.NavArtistsShortcut,
                SettingsEnum.Playlists => Settings.Default.NavPlaylistsShortcut,
                SettingsEnum.NowPlaying => Settings.Default.NavNowPlayingShortcut,
                SettingsEnum.Settings => Settings.Default.NavSettingsShortcut,
                SettingsEnum.Back => Settings.Default.NavBackShortcut,
                SettingsEnum.Forward => Settings.Default.NavForwardShortcut,
                SettingsEnum.ToggleFullScreen => Settings.Default.ToggleFullScreenShortcut,
                SettingsEnum.EscapeFullScreen => Settings.Default.EscapeFullScreenShortcut,
                SettingsEnum.ToggleTheme => Settings.Default.ToggleLightThemeShortcut,
                SettingsEnum.ToggleQueueDrawer => Settings.Default.ToggleQueueShortcut,
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
        /// <param name="force"> force the value even if it's not valid (empty string or null value) </param>
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
                case SettingsEnum.ColorfulPlayerControl:
                    Settings.Default.ColorfulPlayerControl = value;
                    OnColorfulPlayerControlChanged();
                    break;
                case SettingsEnum.ColorfulUI:
                    Settings.Default.ColorfulUI = value;
                    OnColorfulUIChanged();
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
                case SettingsEnum.AutoChangeOutputDevice:
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
                case SettingsEnum.AutoForeground:
                    Settings.Default.AutoForeground = value;
                    break;
                case SettingsEnum.UNKNOWN:
                    return false;
                case SettingsEnum.PlayPause:
                    Settings.Default.PlayPauseShortcut = value;
                    break;
                case SettingsEnum.NexTrack:
                    Settings.Default.NextTrackShortcut = value;
                    break;
                case SettingsEnum.PreviousTrack:
                    Settings.Default.PreviousTrackShortcut = value;
                    break;
                case SettingsEnum.Shuffle:
                    Settings.Default.ShuffleShortcut = value;
                    break;
                case SettingsEnum.Repeat:
                    Settings.Default.RepeatShortcut = value;
                    break;
                case SettingsEnum.DecreaseVolume:
                    Settings.Default.DecreaseVolShortcut = value;
                    break;
                case SettingsEnum.IncreaseVolume:
                    Settings.Default.IncreaseVolShortcut = value;
                    break;
                case SettingsEnum.MuteVolume:
                    Settings.Default.MuteVolShortcut = value;
                    break;
                case SettingsEnum.ToggleFavorite:
                    Settings.Default.ToggleFavShortcut = value;
                    break;
                case SettingsEnum.Rating0:
                    Settings.Default.Rating0Shortcut = value;
                    break;
                case SettingsEnum.Rating1:
                    Settings.Default.Rating1Shortcut = value;
                    break;
                case SettingsEnum.Rating2:
                    Settings.Default.Rating2Shortcut = value;
                    break;
                case SettingsEnum.Rating3:
                    Settings.Default.Rating3Shortcut = value;
                    break;
                case SettingsEnum.Rating4:
                    Settings.Default.Rating4Shortcut = value;
                    break;
                case SettingsEnum.Rating5:
                    Settings.Default.Rating5Shortcut = value;
                    break;
                case SettingsEnum.Home:
                    Settings.Default.NavHomeShortcut = value;
                    break;
                case SettingsEnum.Albums:
                    Settings.Default.NavAlbumsShortcut = value;
                    break;
                case SettingsEnum.Artists:
                    Settings.Default.NavArtistsShortcut = value;
                    break;
                case SettingsEnum.Playlists:
                    Settings.Default.NavPlaylistsShortcut = value;
                    break;
                case SettingsEnum.NowPlaying:
                    Settings.Default.NavNowPlayingShortcut = value;
                    break;
                case SettingsEnum.Settings:
                    Settings.Default.NavSettingsShortcut = value;
                    break;
                case SettingsEnum.Back:
                    Settings.Default.NavBackShortcut = value;
                    break;
                case SettingsEnum.ToggleQueueDrawer:
                    Settings.Default.ToggleQueueShortcut = value;
                    break;
                case SettingsEnum.EscapeFullScreen:
                    Settings.Default.EscapeFullScreenShortcut = value;
                    break;
                case SettingsEnum.ToggleFullScreen:
                    Settings.Default.ToggleFullScreenShortcut = value;
                    break;
                case SettingsEnum.ToggleTheme:
                    Settings.Default.ToggleLightThemeShortcut = value;
                    break;
                case SettingsEnum.EqualizerEnabled:
                    Settings.Default.EqualizerEnabled = value;
                    break;
                case SettingsEnum.EqualizerPreset:
                    Settings.Default.EqualizerPreset = value;
                    break;
                default:
                    return false;
            }
            Settings.Default.Save();
            return true;
        }

        private static void UpdateCoverSettings()
        {
            AreCoversEnabled = true;
            AlbumCoverOnly = false;
            ArtworkOnly = false;
            AutoCover = false;
            SettingsValueEnum settingsValue = (SettingsValueEnum)GetPreference(SettingsEnum.QueueCovers);
            switch (settingsValue)
            {
                case SettingsValueEnum.NoCovers:
                    AreCoversEnabled = false;
                    break;
                case SettingsValueEnum.AlbumCovers:
                    AlbumCoverOnly = true;
                    break;
                case SettingsValueEnum.ArtworkCovers:
                    ArtworkOnly = true;
                    break;
                case SettingsValueEnum.AutoCovers:
                    AutoCover = true;
                    break;
                default:
                    AutoCover = true;
                    break;
            }
        }
    }
}
