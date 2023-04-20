﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Resources = MusicPlay.Language.Resources;

namespace MusicPlayUI.Core.Enums
{
    public enum CommandEnums
    {
        PlayPause = 0,
        NexTrack,
        PreviousTrack,
        Shuffle,
        Repeat,
        DecreaseVolume,
        IncreaseVolume,
        MuteVolume,
        ToggleFavorite,
        Rating0,
        Rating1,
        Rating2,
        Rating3,
        Rating4,
        Rating5,
        Home,
        Albums,
        Artists,
        Playlists,
        NowPlaying,
        Import,
        Settings,
        NavigateBack,
        NavigateToAlbumById,
        NavigateToArtistById,
        NavigateToGenre,
        EscapeFullScreen,
        ToggleFullScreen,
        ToggleQueueDrawer,
    }

    public static class CommandEnumsExt 
    {
        public static string CommandToString(this CommandEnums command)
        {
            return command switch
            {
                CommandEnums.PlayPause => "Play / Pause",
                CommandEnums.NexTrack => "Next Track",
                CommandEnums.PreviousTrack => "Previous Tracks",
                CommandEnums.Shuffle => "Shuffle",
                CommandEnums.Repeat => "Repeat",
                CommandEnums.DecreaseVolume => "Decrease Volume",
                CommandEnums.IncreaseVolume => "Increase Volume",
                CommandEnums.MuteVolume => "Mute Volume",
                CommandEnums.ToggleFavorite => "Toggle Favorite",
                CommandEnums.Rating0 => "Remove Rating",
                CommandEnums.Rating1 => "Rating 1 Star",
                CommandEnums.Rating2 => "Rating 2 Stars",
                CommandEnums.Rating3 => "Rating 3 Stars",
                CommandEnums.Rating4 => "Rating 4 Stars",
                CommandEnums.Rating5 => "Rating 5 Stars",
                CommandEnums.Home => Resources.Home_View,
                CommandEnums.Albums => Resources.Albums_View,
                CommandEnums.Artists => Resources.Artists_View,
                CommandEnums.Playlists => Resources.Playlists_View,
                CommandEnums.NowPlaying => Resources.Now_Playing_View,
                CommandEnums.Import => Resources.Import_Library_View,
                CommandEnums.Settings => Resources.Settings_View,
                CommandEnums.NavigateBack => "Navigate Back",
                CommandEnums.EscapeFullScreen => "Escape Full Screen",
                CommandEnums.ToggleFullScreen => "Toggle Full Screen",
                CommandEnums.ToggleQueueDrawer => "Toggle Queue Drawer",
                _ => "",
            };
        }
    }
}
