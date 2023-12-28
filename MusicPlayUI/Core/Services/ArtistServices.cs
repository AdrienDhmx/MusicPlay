using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlay.Language;
using MusicPlayUI.Core.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services
{
    public static class ArtistServices
    {
        /// <summary>
        /// Get all the tracks the artist has made => all the tracks in his albums and the tracks he performed outside of those albums
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public static async Task<List<TrackModel>> GetArtistTracks(int artistId)
        {
            List<TrackModel> tracks = new();
            List<AlbumModel> albums = await DataAccess.Connection.GetAlbumsFromArtist(artistId);
            if (albums is not null || albums.Count != 0)
            {
                foreach (AlbumModel a in albums)
                {
                    if(a.IsAlbumArtist(artistId))
                        tracks.AddRange(await DataAccess.Connection.GetTracksFromAlbum(a.Id));
                }
            }
            tracks.AddRange(await DataAccess.Connection.GetTracksFromArtist(artistId));
            return tracks.DistinctBy(t => t.Id).ToList();
        }

        /// <summary>
        /// Only get the tracks the artist made but that are not in any of his albums (featured in another Album, or in a compilation)
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public static async Task<List<TrackModel>> GetArtistTracksNotInAlbums(int artistId)
        {
            var Albums = await DataAccess.Connection.GetAlbumsFromArtist(artistId);
            var Tracks = await DataAccess.Connection.GetTracksFromArtist(artistId);
            Tracks = Tracks.Where(t => !Albums.Any(a => a.Id == t.AlbumId)).ToList();
            List<TrackModel> albumsId = Tracks.DistinctBy(t => t.AlbumId).ToList();
            foreach (TrackModel track in albumsId)
            {
                AlbumModel album = await DataAccess.Connection.GetAlbum(track.AlbumId);
                foreach (TrackModel t in Tracks)
                {
                    if (t.AlbumId == album.Id)
                    {
                        t.AlbumCover = album.AlbumCover;
                        t.AlbumName = album.Name;
                    }
                }
            }
            return Tracks;
        }

        public static async void UpdateArtistBoolean(this ArtistModel originalArtist, ArtistModel newArtist, bool trackAdded = true)
        {
            if (originalArtist is null || newArtist is null) return;

            // performer and no tracks found in database
            if (originalArtist.IsPerformer && (await DataAccess.Connection.GetTracksFromArtist(originalArtist.Id))?.Count == 1)
            {
                // no album either => delete artist
                if (!originalArtist.IsAlbumArtist)
                {
                    await DataAccess.Connection.DeleteArtist(originalArtist.Id);
                    //MessageHelper.PublishMessage(DefaultMessageFactory.DataDeleted(originalArtist.Name));
                }
                // album => update performer to false
                else
                {
                    originalArtist.IsPerformer = false;
                    await DataAccess.Connection.UpdateArtist(originalArtist);
                }
            }
            // album artist and no album in database
            else if (originalArtist.IsAlbumArtist && (await DataAccess.Connection.GetAlbumsFromArtist(originalArtist.Id))?.Count == 1) // if only one left then it will change to 0
            {
                // not a performer either => delete artist
                if ((await DataAccess.Connection.GetTracksFromArtist(originalArtist.Id))?.Count == 0)
                {
                    await DataAccess.Connection.DeleteArtist(originalArtist.Id);
                    //MessageHelper.PublishMessage(DefaultMessageFactory.DataDeleted(originalArtist.Name));
                }
                // performer => update albumartist to false
                else
                {
                    originalArtist.IsAlbumArtist = false;
                    // the artist has tracks he performed
                    // the artist might not have isPerformer = true because the tracks he performed are from 'his'(not anymore his) album
                    originalArtist.IsPerformer = true;
                    await DataAccess.Connection.UpdateArtist(originalArtist);
                }
            }

            // track added to the newArtist and newArtist is not a performer => performer to true
            if (trackAdded && !newArtist.IsPerformer)
            {
                newArtist.IsPerformer = true;
                await DataAccess.Connection.UpdateArtist(newArtist);
            }
            // album added to the newArtist and newArtist is not an albumArtist => album artist to true
            else if (!newArtist.IsAlbumArtist)
            {
                newArtist.IsAlbumArtist = true;
                await DataAccess.Connection.UpdateArtist(newArtist);
            }
        }

        /// <summary>
        /// Insert the artist in the database
        /// </summary>
        /// <param name="artist"></param>
        /// <returns>the artist id</returns>
        public static int InsertNewArtistInDataBase(this ArtistModel artist, bool performer = true)
        {
            return  DataAccess.Connection.InsertArtist(artist);
        }
    }
}
