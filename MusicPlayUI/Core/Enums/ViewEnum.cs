using MusicPlayUI.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Enums
{
    public enum ViewNameEnum
    {
        // An empty View that can be used for views that doesn't exists yet
        Empty = 0,

        // Login, create account and Profil view are all under the User Account
        Profil = -1,
        Login = -2,
        CreateAccount = -3,

        // Main Menu Views
        Home = 1,
        Albums = 2,
        Artists = 3,
        Playlists = 4,
        Import = 5,
        Settings = 6,
        NowPlaying = 7, // now in menu

        // Main Views (not in menu)
        SpecificAlbum = 10,
        SpecificArtist = 11,
        SpecificPlaylist = 12,
        SpecificGenre = 13,

        // Secondary Views
        Queue = 20,
        Lyrics = 21,
        TrackInfo = 22,

        // Quick Views
        QuickQueue = 30,
        QuickLyrics = 31,
        QuickTrackInfo = 32,

        // Modal Views
        CreatePlaylist = 40,
        UpdatePlaylist = 41,
        ConfirmAction = 42,

        // PopupViewModel Views
        TrackPopup = 50,
        AlbumPopup = 51,
        ArtistPopup = 52,
        PlaylistPopup = 53,

        // Settings Views
        General = 60,
        AppTheme = 61,
        Language = 62,

        // Windows
        TrackProperties = 70,
        Visualizer = 71
    }

    public static class ViewNameEnumExt
    {
        public static string ViewEnumToString(this ViewNameEnum viewNameEnum)
        {
            switch (viewNameEnum)
            {
                case ViewNameEnum.Empty:
                    return "Empty";
                case ViewNameEnum.Profil:
                    return "Profil";
                case ViewNameEnum.Login:
                    return "Login";
                case ViewNameEnum.CreateAccount:
                    return "CreateAccount";
                case ViewNameEnum.Home:
                    return "Home";
                case ViewNameEnum.Albums:
                    return "Albums";
                case ViewNameEnum.Artists:
                    return "Artists";
                case ViewNameEnum.Playlists:
                    return "Playlists";
                case ViewNameEnum.Import:
                    return "Import";
                case ViewNameEnum.Settings:
                    return "Settings";
                case ViewNameEnum.SpecificAlbum:
                    return "SpecificAlbum";
                case ViewNameEnum.SpecificArtist:
                    return "SpecificArtist";
                case ViewNameEnum.SpecificPlaylist:
                    return "SpecificPlaylist";
                case ViewNameEnum.NowPlaying:
                    return "NowPlaying";
                case ViewNameEnum.Queue:
                    return "Queue";
                case ViewNameEnum.Lyrics:
                    return "Lyrics";
                case ViewNameEnum.TrackInfo:
                    return "TrackInfo";
                case ViewNameEnum.QuickQueue:
                    return "QuickQueue";
                case ViewNameEnum.QuickLyrics:
                    return "QuickLyrics";
                case ViewNameEnum.QuickTrackInfo:
                    return "QuickTrackInfo";
                case ViewNameEnum.General:
                    return "General";
                case ViewNameEnum.AppTheme:
                    return "AppTheme";
                case ViewNameEnum.Language:
                    return "Language";
                case ViewNameEnum.Visualizer:
                    return "Visualizer";
                default:
                    return "Empty";
            }
        }

        public static ViewNameEnum ToViewNameEnum(this string viewName)
        {
            switch (viewName)
            {
                case "Empty":
                    return ViewNameEnum.Empty;
                case "Profil":
                    return ViewNameEnum.Profil;
                case "Login":
                    return ViewNameEnum.Login;
                case "CreatAccount":
                    return ViewNameEnum.CreateAccount;
                case "Home":
                    return ViewNameEnum.Home;
                case "Albums":
                    return ViewNameEnum.Albums;
                case "Artists":
                    return ViewNameEnum.Artists;
                case "Playlists":
                    return ViewNameEnum.Playlists;
                case "Import":
                    return ViewNameEnum.Import;
                case "Settings":
                    return ViewNameEnum.Settings;
                case "SpecificAlbum":
                    return ViewNameEnum.SpecificAlbum;
                case "SpecificArtsit":
                    return ViewNameEnum.SpecificArtist;
                case "SpecificPlaylist":
                    return ViewNameEnum.SpecificPlaylist;
                case "NowPlaying" or "Now Playing":
                    return ViewNameEnum.NowPlaying;
                case "Queue":
                    return ViewNameEnum.Queue;
                case "Lyrics":
                    return ViewNameEnum.Lyrics;
                case "TrackInfo":
                    return ViewNameEnum.TrackInfo;
                case "QuickQueue":
                    return ViewNameEnum.QuickQueue;
                case "QuickLyrics":
                    return ViewNameEnum.QuickLyrics;
                case "QuickTrackInfo":
                    return ViewNameEnum.QuickTrackInfo;
                case "General":
                    return ViewNameEnum.General;
                case "AppTheme":
                    return ViewNameEnum.AppTheme;
                case "Language":
                    return ViewNameEnum.Language;
                case "Visualizer":
                    return ViewNameEnum.Visualizer;
                case "TrackProperties":
                    return ViewNameEnum.TrackProperties;
                default:
                    return ViewNameEnum.Empty;
            }
        }
    }
}
