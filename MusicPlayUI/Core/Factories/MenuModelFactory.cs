using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicPlayUI.Core.Factories
{
    public static class MenuModelFactory
    {
        public static List<MenuModel> CreateMenu()
        {
            List<MenuModel> menuList = new List<MenuModel>
            {
                new() { Icon = (PathGeometry)App.IconDic["HomeIcon"], Name = MusicPlay.Language.Resources.Home_View, Enum = ViewNameEnum.Home, IsSelected = false },
                new() { Icon = (PathGeometry)App.IconDic["AlbumLibraryIcon"], Name = MusicPlay.Language.Resources.Albums_View, Enum = ViewNameEnum.Albums, IsSelected = false },
                new() { Icon = (PathGeometry)App.IconDic["mic"], Name = MusicPlay.Language.Resources.Artists_View, Enum = ViewNameEnum.Artists, IsSelected = false },
                new() { Icon = (PathGeometry)App.IconDic["PlaylistLibrary"], Name = MusicPlay.Language.Resources.Playlists_View, Enum = ViewNameEnum.Playlists, IsSelected = false },
                new() { Icon = (PathGeometry)App.IconDic["Tags"], Name = MusicPlay.Language.Resources.Genre, Enum = ViewNameEnum.Genres, IsSelected = false },
                new() { Icon = (PathGeometry)App.IconDic["_music_icon"], Name = MusicPlay.Language.Resources.Now_Playing_View, Enum = ViewNameEnum.NowPlaying, IsSelected = false },
                new() { Icon = (PathGeometry)App.IconDic["DownloadsIcon"], Name = MusicPlay.Language.Resources.Import_Library_View, Enum = ViewNameEnum.Import, IsSelected = false },
                new() { Icon = (PathGeometry)App.IconDic["Settings"], Name = MusicPlay.Language.Resources.Settings_View, Enum = ViewNameEnum.Settings, IsSelected = false }
            };
            return menuList;
        }
    }
}
