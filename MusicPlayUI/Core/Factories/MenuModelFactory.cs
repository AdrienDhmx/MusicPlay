using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;
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
                new() { 
                    Icon = (PathGeometry)AppTheme.IconDic["HomeIcon"], 
                    Name = MusicPlay.Language.Resources.Home_View, 
                    Enum = ViewNameEnum.Home, 
                    IsSelected = false,
                    Type = typeof(HomeViewModel)
                },
                new() { 
                    Icon = (PathGeometry)AppTheme.IconDic["mic"], 
                    Name = MusicPlay.Language.Resources.Artists_View, 
                    Enum = ViewNameEnum.Artists, 
                    IsSelected = false,
                    Type = typeof(ArtistLibraryViewModel)
                },
                new() { 
                    Icon = (PathGeometry)AppTheme.IconDic["AlbumLibraryIcon"], 
                    Name = MusicPlay.Language.Resources.Albums_View, 
                    Enum = ViewNameEnum.Albums, 
                    IsSelected = false,
                    Type = typeof(AlbumLibraryViewModel)
                },
                new() { 
                    Icon = (PathGeometry)AppTheme.IconDic["PlaylistLibrary"], 
                    Name = MusicPlay.Language.Resources.Playlists_View, 
                    Enum = ViewNameEnum.Playlists, 
                    IsSelected = false,
                    Type = typeof(PlaylistLibraryViewModel)
                },
                new() { 
                    Icon = (PathGeometry)AppTheme.IconDic["Tags"], 
                    Name = MusicPlay.Language.Resources.Genre, 
                    Enum = ViewNameEnum.Genres, 
                    IsSelected = false,
                    Type = typeof(GenreLibraryViewModel)
                },
                new() { 
                    Icon = (PathGeometry)AppTheme.IconDic["_music_icon"], 
                    Name = MusicPlay.Language.Resources.Now_Playing_View, 
                    Enum = ViewNameEnum.NowPlaying, 
                    IsSelected = false,
                    Type = typeof(NowPlayingViewModel)
                },
                new() { 
                    Icon = (PathGeometry)AppTheme.IconDic["Settings"], 
                    Name = MusicPlay.Language.Resources.Settings_View, 
                    Enum = ViewNameEnum.Settings, 
                    IsSelected = false,
                    Type = typeof(SettingsViewModel)
                }
            };
            return menuList;
        }
    }
}
