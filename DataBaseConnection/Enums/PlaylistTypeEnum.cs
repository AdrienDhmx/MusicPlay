namespace MusicPlay.Database.Enums
{
    public enum PlaylistTypeEnum
    {
        // PlaylistTags created automatically bu that are constant (Favorite, Last Played, Most Played...)
        ConstantAutoPlaylist = -1,
            
        // PlaylistTags created automatically but that are not constant
        // (playlists created by taking random tracks sharing some common characteristics)
        Radio = 0,

        Favorite = 1,
        LastPlayed = 2,
        MostPlayed = 3,

        // PlaylistTags created by the user
        UserPlaylist = 11,
    }
}
