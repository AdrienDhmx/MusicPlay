using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Enums
{
    public enum SettingsEnum
    {
        // General
        MainStartingView = 0,
        NowPlayingStartingSubView = 1,

        TimerInterval = 2,
        DefaultBackgroundOpacity = 3,

        QueueCovers = 4,

        AutoChangeOutputdevice = 5,

        Volume = 6,

        VisualizerVisibility = 7,
        NowPlayingCoverVisibility = 8,

        LyricsFontSize = 9,

        AppTheme = 10, // app theme => accent color define a theme (default is purple)
        LightTheme =11, // theme is light or dark (bool)
        SunsetSunrise = 12, // theme is light or dark based on the time of the day (bool)
        SystemSyncTheme = 13, // theme is (light or dark) sync with the system (bool)

        Language = 20,
        UserName = 21,

        // visualizer
        VColor = 50,
        VDataQt = 51,
        VRepresentation = 52,
        VGradient = 53,
        VFill = 54,
        VStrokeThickness = 55,
        VObjectLength = 56,
        VCutHighFreq = 57,
        VCutPercentage = 58,
        VSmoothGraph = 59,
        VRefreshRate = 60,
        VAutoColor = 61,
        VCenterFreq = 62,

        AutoForeground = 63,

        // filters and orders
        ArtistFilter = 70,
        ArtistOrder = 71,
        AlbumFilter = 72,
        AlbumOrder = 73,

        PlayPause = 100,
        NexTrack = 101,
        PreviousTrack = 102,
        Shuffle = 103,
        Repeat = 104,
        DecreaseVolume = 105,
        IncreaseVolume = 106,
        MuteVolume = 107,
        ToggleFavorite = 108,
        Rating0 = 109,
        Rating1 = 110,
        Rating2 = 111,
        Rating3 = 112,
        Rating4 = 113,
        Rating5 = 114,
        Home = 115,
        Albums = 116,
        Artists = 117,
        Playlists = 118,
        NowPlaying = 119,
        Import = 120,
        Settings = 121,
        Back = 122,
        ToggleQueueDrawer = 123,
        EscapeFullScreen = 124,
        ToggleFullScreen = 125,
        ToggleTheme = 126,

        UNKNOWN = -1
    }

    public static class SettingsEnumExt
    {
        public static string SettingsEnumToString(this SettingsEnum preferences)
        {
            switch (preferences)
            {
                case SettingsEnum.MainStartingView:
                    return "MainStartingView";
                case SettingsEnum.AppTheme:
                    return "AppTheme";
                case SettingsEnum.Language:
                    return "Language";
                case SettingsEnum.TimerInterval:
                    return "TimerInterval";
                case SettingsEnum.NowPlayingStartingSubView:
                    return "NowPlayingStartingSubView";
                case SettingsEnum.DefaultBackgroundOpacity:
                    return "DefaultBackgroundOpacity";
                default:
                    return null;
            }
        }

        public static SettingsEnum StringToSettingsEnum(this string preference)
        {
            switch (preference)
            {
                case "MainStartingView":
                    return SettingsEnum.MainStartingView;
                case "AppTheme":
                    return SettingsEnum.AppTheme;
                case "Language":
                    return SettingsEnum.Language;
                case "TimerInterval":
                    return SettingsEnum.TimerInterval;
                case "NowPlayingStartingSubView":
                    return SettingsEnum.NowPlayingStartingSubView;
                case "DefaultBackgroundOpacity":
                    return SettingsEnum.DefaultBackgroundOpacity;
                default:
                    return SettingsEnum.UNKNOWN;
            }
        }
    }
}
