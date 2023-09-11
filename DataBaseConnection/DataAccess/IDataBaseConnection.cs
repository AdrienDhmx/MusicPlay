using MusicPlayModels.MusicModels;
using MusicPlayModels.Enums;
using MusicPlayModels.StatsModels;
using AudioHandler.Models;

namespace DataBaseConnection.DataAccess
{
    public interface IDataBaseConnection
    {
        /// <summary>
        /// Get all the tracks in the database
        /// </summary>
        /// <returns></returns>
        public Task<List<TrackModel>> GetAllTracks();
        /// <summary>
        /// Get all the albums in the database
        /// </summary>
        /// <returns></returns>
        public Task<List<AlbumModel>> GetAllAlbums();
        /// <summary>
        /// Get all the artists in the database
        /// </summary>
        /// <returns></returns>
        public Task<List<ArtistModel>> GetAllArtists();
        /// <summary>
        /// Get all the playlists in the database
        /// </summary>
        /// <returns></returns>
        public Task<List<PlaylistModel>> GetAllPlaylists();
        /// <summary>
        /// Get all the tags in the database
        /// </summary>
        /// <returns></returns>
        public Task<List<TagModel>> GetAllTags();

        public Task<TagModel> GetTag(int id);

        /// <summary>
        /// Get the last queue playing stored in the database
        /// </summary>
        /// <returns></returns>
        public Task<QueueModel> GetQueue();

        public Task<TrackModel> GetTrackByPath(string path);

        /// <summary>
        /// Insert a track in the database
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public Task<int> InsertTrack(TrackModel track);
        /// <summary>
        /// Insert an album in the database
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public Task<int> InsertAlbum(AlbumModel album);
        /// <summary>
        /// Insert an artist in the database
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public Task<int> InsertArtist(ArtistModel artist);
        /// <summary>
        /// Insert a tag in the database
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Task<int> InserTag(TagModel tag);
        /// <summary>
        /// Insert a relation between an album and a tag in the database
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task InsertAlbumTag(int albumId, int tagId);

        /// <summary>
        /// Insert a relation between an artist and a tag in the database
        /// </summary>
        /// <param name="artistId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task InsertArtistTag(int artistId, int tagId);

        /// <summary>
        /// Insert a relation between a playlist and a tag in the database
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task<int> InsertPlaylistTag(int playlistId, int tagId);

        /// <summary>
        /// Insert a relation between a track and a tag in the database
        /// </summary>
        /// <param name="trackId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task InsertTrackTag(int trackId, int tagId);

        /// <summary>
        /// Insert a queue in the database
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public Task InsertQueue(QueueModel queue);

        /// <summary>
        /// Query the database to find the albums corresping to the criterias and sort the result
        /// </summary>
        /// <param name="tagsId"></param>
        /// <param name="artistsId"></param>
        /// <param name="searchString"></param>
        /// <param name="sortEnum"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public Task<List<AlbumModel>> SearchAlbums(List<int> tagsId, List<int> artistsId, List<AlbumTypeEnum> albumTypes, string searchString, SortEnum sortEnum, bool ascending = false);

        /// <summary>
        ///  Query the database to find the artists corresping to the criterias and sort the result
        /// </summary>
        /// <param name="artistTypes"></param>
        /// <param name="searchString"></param>
        /// <param name="sortEnum"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public Task<List<ArtistModel>> SearchArtists(List<int> tagsId, List<int> artistTypes, string searchString, SortEnum sortEnum, bool ascending = false);

        /// <summary>
        /// Update the artistId
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public Task UpdateTrackArtist(TrackModel track);
        
        public void UpdateTrackPlayCount(TrackModel track);

        public void UpdateTrackRating(TrackModel track);

        public Task UpdateTrackDiscTrackNumber(TrackModel track);

        public Task UpdateTrackIsFavorite(TrackModel track);

        /// <summary>
        /// Update the track Artwork
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public Task UpdateTrackArtwork(TrackModel track);

        /// <summary>
        /// Update the album cover
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public Task UpdateAlbumCover(AlbumModel album);

        /// <summary>
        /// Update the album artistId
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public Task UpdateAlbum(AlbumModel album);

        public void UpdateAlbumInteraction(int playCount, int id);

        public void UpdateArtistInteraction(int playCount, int id);

        /// <summary>
        /// Update the artist data (only the IsAlbumArtist and IsPerformer)
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public Task UpdateArtist(ArtistModel artist);

        /// <summary>
        /// Update the artist cover
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public Task UpdateArtistCover(ArtistModel artist);

        /// <summary>
        /// Update the playlist data, all the properties can be updated
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public Task UpdatePlaylist(PlaylistModel playlist);


        /// <summary>
        /// Update the name of the tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Task UpdateTag(TagModel tag);

        /// <summary>
        /// Insert a playlist in the database
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public Task<int> InsertPlaylist(PlaylistModel playlist);
        /// <summary>
        /// Delete a playlist from the database (no undo)
        /// </summary>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        public Task DeletePlaylist(int playlistId);
        /// <summary>
        /// Add a track to playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        public Task AddTrackToPlaylist(PlaylistModel playlist, TrackModel track, int trackIndex);
        /// <summary>
        /// Add relations btw a playlist a multiple tracks
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="tracks"></param>
        /// <returns></returns>
        public Task AddTrackToPlaylist(PlaylistModel playlist, List<OrderedTrackModel> tracks);
        /// <summary>
        /// Add relations btw a playlist a multiple tracks
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="tracks"></param>
        /// <returns></returns>
        public Task AddTrackToPlaylist(PlaylistModel playlist, List<TrackModel> tracks);
        /// <summary>
        /// Remove a relation btw a track and a playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        public Task RemoveTrackFromPlaylist(PlaylistModel playlist, TrackModel track);


        public Task UpdatePlaylistTracks(int playlistId, List<OrderedTrackModel> tracks);
        /// <summary>
        /// Delete the track from the database
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public Task DeleteTrack(int trackId);

        /// <summary>
        /// Delete the album from the database
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public Task DeleteAlbum(int albumId);

        /// <summary>
        /// Delete the tag and all its relation from the database
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task DeleteTag(int tagId);

        /// <summary>
        /// Get all the tracks belonging to the album
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public Task<List<TrackModel>> GetTracksFromAlbum(int albumId);
        /// <summary>
        /// Get all the tracks (not the tracks in albums) created by the artist
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public Task<List<TrackModel>> GetTracksFromArtist(int artistId);
        /// <summary>
        /// Get all the tracks in the playlist
        /// </summary>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        public Task<List<OrderedTrackModel>> GetTracksFromPlaylist(int playlistId);
        /// <summary>
        /// Get all the playlists the tracks is in
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public Task<List<int>> GetTrackPlaylistsIds(int trackId);
        /// <summary>
        /// Get all the tracks in the queue stored in the database
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        public Task<List<TrackModel>> GetTracksFromQueue(int queueId);
        /// <summary>
        /// Get all the albums released by the artist in the database
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public Task<List<AlbumModel>> GetAlbumsFromArtist(int artistId);
        /// <summary>
        /// Get all the favorites tracks
        /// </summary>
        /// <returns></returns>
        public Task<List<TrackModel>> GetFavoriteTracks();

        /// <summary>
        /// Get the top most played tracks (higher play count)
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public Task<List<TrackModel>> GetMostPlayedTracks(int top);

        /// <summary>
        /// Get the top most played albums (higher play count)
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public Task<List<AlbumModel>> GetMostPlayedAlbums(int top);

        /// <summary>
        /// Get the top most played artists (higher play count)
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public Task<List<ArtistModel>> GetMostPlayedArtists(int top);

        /// <summary>
        /// Get all the tags the album is labeled as
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public Task<List<TagModel>> GetAlbumTag(int albumId);

        /// <summary>
        /// Get all the albums labeled with the specified tag
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task<List<AlbumModel>> GetAlbumFromTag(int tagId);

        public Task<List<TagModel>> GetArtistTag(int artistId);

        public Task<List<ArtistModel>> GetArtistFromTag(int tagId);

        public Task<List<TrackModel>> GetTracksFromAlbums(IEnumerable<int> albumsId);

        public Task<List<TrackModel>> GetTracksFromArtists(IEnumerable<int> artistsId);

        public Task<List<TagModel>> GetTrackTag(int trackId);

        public Task<List<TagModel>> GetPlaylistTag(int playlistId);

        public Task<List<PlaylistModel>> GetPlaylistFromTag(int tagId);

        public Task<List<TrackModel>> GetTrackFromTag(int tagId);

        /// <summary>
        /// Get the album corresponding to the Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<AlbumModel> GetAlbum(int id);
        /// <summary>
        /// Get the artist corresponding to the Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ArtistModel> GetArtist(int id);

        /// <summary>
        /// Get all the artist corresponding to the ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<ArtistModel>> GetArtists(IEnumerable<int> ids);

        /// <summary>
        /// Get the playlist corresponding to the Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<PlaylistModel> GetPlaylist(int id);

        public Task<int> GetPlaylistId(string playlistName);
        /// <summary>
        /// Get the tracks corresponding to the Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TrackModel> GetTrack(int id);

        /// <summary>
        /// Get the artist name corresponding to the Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<string> GetArtistName(int id);
        /// <summary>
        /// Get the album name corresponding to the Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<string> GetAlbumName(int id);

        /// <summary>
        /// Try to find the Artist Id based on the artist name (case insensitive)
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns> -1 if none are found </returns>
        public Task<int> GetArtistId(string artistName);
        /// <summary>
        /// Try to found the Album Id based on the album name (case insensitive)
        /// </summary>
        /// <param name="albumName"></param>
        /// <returns>-1 if none are found </returns>
        public Task<int> GetAlbumId(string albumName);

        public Task DeleteArtist(int artistId);

        /// <summary>
        /// Remove the relation btw the album and the tag
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task RemoveAlbumTag(int albumId, int tagId);

        /// <summary>
        /// Remove the relation btw the artist and the tag
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task RemoveArtistTag(int artistId, int tagId);

        /// <summary>
        /// Remove the relation btw the playlist and the tag
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task RemovePlaylistTag(int playlistId, int tagId);

        /// <summary>
        /// Remove the relation btw the track and the tag
        /// </summary>
        /// <param name="trackId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Task RemoveTrackTag(int trackId, int tagId);

        /// <summary>
        /// Delete all the data in the database (no undo)
        /// </summary>
        /// <returns></returns>
        public void ClearDataBase();

        /// <summary>
        /// Get the top last played tracks
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public Task<List<TrackModel>> GetLastPlayedTracks(int top);

        /// <summary>
        /// Get the top last played albums
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public Task<List<AlbumModel>> GetLastPlayedAlbums(int top);

        /// <summary>
        /// Get the top last played artists
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public Task<List<ArtistModel>> GetLastPlayedArtists(int top);

        /// <summary>
        /// Get the number of Artists, Albums and Tracks
        /// </summary>
        /// <returns></returns>
        public Task<(int, int, int)> GetNumberOfEntries();

        /// <summary>
        /// Get all the tracks fitting the specified parameters in the database
        /// </summary>
        /// <param name="lowerDate">last played must be after that date (included)</param>
        /// <param name="upperDate">last played must be before that date (included)</param>
        /// <param name="lowerPlayCount">the minimum play count</param>
        /// <param name="upperPlayCount">the maximum play count</param>
        /// <returns></returns>
        public Task<List<TrackModel>> GetTrackBasedOnLastPlayedAndPlayCount(DateTime lowerDate, DateTime upperDate, int lowerPlayCount, int upperPlayCount);

        /// <summary>
        /// Retrieve all the data from the specified <paramref name="date"/> until <paramref name="numberOfWeek"/> or today if the current date is exceeded
        /// </summary>
        /// <param name="date"></param>
        /// <param name="numberOfWeek"></param>
        /// <returns></returns>
        public Task<List<HistoryModel>> GetHistoryModels(DateTime date, int numberOfWeek);

        /// <summary>
        /// Insert the history model in the database
        /// </summary>
        /// <param name="historyModel"></param>
        /// <returns></returns>
        public HistoryModel InsertHistoryModel(HistoryModel historyModel);

        /// <summary>
        /// Update all the data in the database for that history model
        /// </summary>
        /// <param name="historyModel"></param>
        public void UpdateHistoryModel(HistoryModel historyModel);

        /// <summary>
        /// Delete from the database all data with date older than <paramref name="date"/>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public Task DeleteHistoryModel(DateTime date);

        /// <summary>
        /// Try to retrieve the history model for the current date, if not found return the default value of the model
        /// </summary>
        /// <returns></returns>
        public HistoryModel GetTodayHistoryModel();

        /// <summary>
        /// Delete the queue and the queue track stored in tht database
        /// </summary>
        /// <returns></returns>
        public Task DeleQueue();

        /// <summary>
        /// Get all the preset
        /// </summary>
        /// <returns></returns>
        public Task<List<EQPresetModel>> GetAllEQPresets();

        /// <summary>
        /// Insert a new Equalizer Preset in the database with all its bands
        /// </summary>
        /// <param name="eqPreset"></param>
        /// <returns></returns>
        public Task<int> InsertEQPreset(EQPresetModel eqPreset);

        /// <summary>
        /// Update the Equalizer Preset name
        /// </summary>
        /// <param name="eqPreset"></param>
        /// <returns></returns>
        public Task UpdateEQPreset(EQPresetModel eqPreset);

        /// <summary>
        /// Update the Equalizer band
        /// </summary>
        /// <param name="eqBand"></param>
        /// <returns></returns>
        public Task UpdateEQBand(EQEffectModel eqBand);

        /// <summary>
        /// Delete the Equalizer preset and all its bands
        /// </summary>
        /// <param name="eqPreset"></param>
        /// <returns></returns>
        public Task DeleteEQPreset(EQPresetModel eqPreset);
    }
}
