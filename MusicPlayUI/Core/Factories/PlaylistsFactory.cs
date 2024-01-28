using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicFilesProcessor.Helpers;
using MusicPlay.Language;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;

using MusicPlay.Database.Models.DataBaseModels;
using MusicPlay.Database.Helpers;

namespace MusicPlayUI.Core.Factories
{
    public static class PlaylistsFactory
    {
        public static async Task<Playlist> CreateFavoritePlaylist()
        {
            Playlist playlist = new Playlist();
            playlist.PlaylistType = PlaylistTypeEnum.Favorite;
            //playlist.Tracks = new((await Track.GetFavorites()).ToOrderedTrackModel());
            playlist.Name = Resources.Favorite;
            playlist.Description = Resources.Favorite_Dscpt;
            playlist.Cover = "m24 41.95-2.05-1.85q-5.3-4.85-8.75-8.375-3.45-3.525-5.5-6.3T4.825 20.4Q4 18.15 4 15.85q0-4.5 3.025-7.525Q10.05 5.3 14.5 5.3q2.85 0 5.275 1.35Q22.2 8 24 10.55q2.1-2.7 4.45-3.975T33.5 5.3q4.45 0 7.475 3.025Q44 11.35 44 15.85q0 2.3-.825 4.55T40.3 25.425q-2.05 2.775-5.5 6.3T26.05 40.1Z";
            return playlist;
        }

        public static async Task<Playlist> CreateLastPlayedPlaylist()
        {
            Playlist playlist = new Playlist();
            playlist.PlaylistType = PlaylistTypeEnum.LastPlayed;
            playlist.Name = Resources.Last_Played;
            //playlist.Tracks = new((await Track.GetLastPlayed(100)).ToOrderedTrackModel());
            playlist.Description = Resources.Last_Played_Dscpt;
            playlist.Cover = "m25.6 23.3 5.75 5.65q.45.45.45 1.075t-.45 1.075q-.45.45-1.05.45-.6 0-1.05-.45l-6.2-6.1q-.25-.25-.35-.525-.1-.275-.1-.575v-8.55q0-.65.425-1.075.425-.425 1.075-.425.65 0 1.075.425.425.425.425 1.075ZM23.85 42q-7 0-11.875-4.5T6.15 26.25q-.1-.7.275-1.2T7.5 24.5q.6-.05 1.05.375.45.425.55 1.025.85 5.6 4.9 9.35Q18.05 39 23.85 39q6.35 0 10.75-4.45t4.4-10.8q0-6.2-4.45-10.475Q30.1 9 23.85 9q-3.4 0-6.375 1.55t-5.175 4.1h3.75q.65 0 1.075.425.425.425.425 1.075 0 .65-.425 1.075-.425.425-1.075.425H8.6q-.65 0-1.075-.425Q7.1 16.8 7.1 16.15v-7.4q0-.65.425-1.075Q7.95 7.25 8.6 7.25q.65 0 1.075.425.425.425.425 1.075v3.8q2.6-3.05 6.175-4.8Q19.85 6 23.85 6q3.75 0 7.05 1.4t5.775 3.825q2.475 2.425 3.9 5.675Q42 20.15 42 23.9t-1.425 7.05q-1.425 3.3-3.9 5.75-2.475 2.45-5.775 3.875Q27.6 42 23.85 42Z";
            playlist.Tracks = new(); //(await DataAccess.Connection.GetLastPlayedTracks(100)).ToOrderedTrackModel();
            return playlist;
        }

        public static async Task<Playlist> CreateMostPlayedPlaylist()
        {
            Playlist playlist = new Playlist();
            playlist.PlaylistType = PlaylistTypeEnum.MostPlayed;
            playlist.Name = Resources.Most_Played;
            playlist.Description = Resources.Most_Played_Dscpt;
            playlist.Cover = "M13.05 23.05q-.45-.45-.45-1.05 0-.6.45-1.05l9.9-9.9q.25-.25.5-.35.25-.1.55-.1.3 0 .55.1.25.1.5.35l9.9 9.9q.45.45.45 1.05 0 .6-.45 1.05-.45.45-1.05.45-.6 0-1.05-.45L24 14.2l-8.85 8.85q-.45.45-1.05.45-.6 0-1.05-.45Zm0 12.65q-.45-.45-.45-1.05 0-.6.45-1.05l9.9-9.9q.25-.25.5-.35.25-.1.55-.1.3 0 .55.1.25.1.5.35l9.9 9.9q.45.45.45 1.05 0 .6-.45 1.05-.45.45-1.05.45-.6 0-1.05-.45L24 26.85l-8.85 8.85q-.45.45-1.05.45-.6 0-1.05-.45Z";
            playlist.Tracks = new(); //(await DataAccess.Connection.GetMostPlayedTracks(50)).ToOrderedTrackModel();
            return playlist;
        }

        public static Playlist CreateRadioPlaylist(string playlistName, string description, string cover,  List<Track> tracks)
        {
            return new()
            {
                Name = playlistName,
                Description = description,
                //Tracks = tracks.ToOrderedTrackModel(),
                Cover = cover,
                PlaylistType = PlaylistTypeEnum.Radio,
            };
        }

        /// <summary>
        /// Get all the auto playlists (favorite, last played and most played)
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Playlist>> GetConstAutoPlaylists()
        {
            List<Playlist> playlists = new()
            {
                await CreateFavoritePlaylist(),
                await CreateLastPlayedPlaylist(),
                await CreateMostPlayedPlaylist()
            };
            return playlists;
        }

        /// <summary>
        /// Try to create a playlist with the data passed in, if one of the data is incorrect the playlist is not created and 
        /// the corresponding bool will be returned false
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="cover"></param>
        public static async Task<Playlist> CreatePlaylist(this string name, string description, string cover)
        { 
            Playlist playlist = new()
            {
                Name = name,
                Description = description,
                PlaylistType = PlaylistTypeEnum.UserPlaylist
            };

            await Playlist.Insert(playlist);
            await playlist.UpdateCoverWithFile(cover);

            return playlist;
        }

        public static (bool, bool) IsPlaylistDataValid(this string name, string cover)
        {
            return (!string.IsNullOrWhiteSpace(name), cover.ValidFilePath(true));
        }

        /// <summary>
        /// Try to create a playlist with the data passed in, if one of the data is incorrect the playlist is not created and 
        /// the corresponding bool will be returned false
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="cover"></param>
        /// <returns>in order: valid, idr</returns>
        public static async Task<Playlist> CreatePlaylist(this string name, string cover)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            Playlist playlist = new()
            {
                Name = name,
                Description = "",
                PlaylistType = PlaylistTypeEnum.UserPlaylist
            };

            await Playlist.Insert(playlist);
            await playlist.UpdateCoverWithFile(cover);

            return playlist;
        }
    }
}
