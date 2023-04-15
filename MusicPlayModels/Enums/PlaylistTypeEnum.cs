using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.Enums
{
    public enum PlaylistTypeEnum
    {
        // Playlists created automatically bu that are constant (Favorite, Last Played, Most Played...)
        ConstantAutoPlaylist = -1,

        // Playlists created automatically but taht are not constant
        // (playlists created by taking random tracks sharing some commun characteristics)
        Radio = 0,

        Favorite = 1,
        LastPlayed = 2,
        MostPlayed = 3,


        // Playlists created by the user
        UserPlaylist = 11,
    }
}
