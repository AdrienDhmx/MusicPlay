using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicPlay.Database.Models;


namespace MusicPlayUI.Core.Services
{
    public static class ArtistServices
    {
        /// <summary>
        /// Get all the tracks the artist has made => all the tracks in his albums and the tracks he performed outside of those albums
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public static async Task<List<Track>> GetArtistTracks(int artistId)
        {
            List<Track> tracks = new();
            //List<Album> albums = await DataAccess.Connection.GetAlbumsFromArtist(ArtistId);
            //if (albums is not null || albums.Count != 0)
            //{
            //    foreach (Album a in albums)
            //    {
            //        if(a.IsAlbumArtist(ArtistId))
            //            tracks.AddRange(await DataAccess.Connection.GetTracksFromAlbum(a.Id));
            //    }
            //}
            //tracks.AddRange(await DataAccess.Connection.GetTracksFromArtist(ArtistId));
            return tracks.DistinctBy(t => t.Id).ToList();
        }

        /// <summary>
        /// Only get the tracks the artist made but that are not in any of his albums (featured in another Album, or in a compilation)
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public static async Task<List<Track>> GetArtistTracksNotInAlbums(int artistId)
        {
            //var AlbumTags = await DataAccess.Connection.GetAlbumsFromArtist(ArtistId);
            List<Track> Tracks = new(); // await DataAccess.Connection.GetTracksFromArtist(ArtistId);
            //TrackTags = TrackTags.ApplyWhere(t => !AlbumTags.Any(a => a.Id == t.AlbumId)).ToList();
            //List<Track> albumsId = TrackTags.DistinctBy(t => t.AlbumId).ToList();
            //foreach (Track track in albumsId)
            //{
            //    Album album = await DataAccess.Connection.GetAlbum(track.AlbumId);
            //    foreach (Track t in TrackTags)
            //    {
            //        if (t.AlbumId == album.Id)
            //        {
            //            t.AlbumCover = album.AlbumCover;
            //            t.AlbumName = album.Name;
            //        }
            //    }
            //}
            return Tracks;
        }
    }
}
