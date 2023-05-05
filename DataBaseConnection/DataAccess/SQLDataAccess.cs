using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using DataBaseConnection.Model;
using MusicPlayModels.Enums;
using MusicPlayModels.MusicModels;
using MusicPlayModels.StatsModels;
using SqlKata;

namespace DataBaseConnection.DataAccess
{
    public class SQLDataAccess : IDataBaseConnection
    {
        private readonly string connectionString;

        internal class Tables
        {
            public const string TAlbum = "Album";
            public const string TAlbumArtists = "AlbumArtists";
            public const string TAlbumGenres = "AlbumGenres";
            public const string TArtist = "Artist";
            public const string TArtistGenres = "ArtistGenres";
            public const string TGenre = "Genre";
            public const string THistory = "History";
            public const string TPlaylist = "Playlist";
            public const string TPlaylistTracks = "PlaylistTracks";
            public const string TQueue = "Queue";
            public const string TQueueTracks = "QueueTracks";
            public const string TTrack = "Track";
            public const string TTrackArtists = "TrackArtists";

            public static Dictionary<string, object> CreateTable(AlbumModel album)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.Name, album.Name },
                    { Columns.AlbumCover, album.AlbumCover },
                    { Columns.CopyRight, album.Copyright },
                    { Columns.Year, album.Year },
                    { Columns.IsSingle, album.IsSingle },
                    { Columns.IsEP, album.IsEP },
                    { Columns.PlayCount, album.PlayCount },
                    { Columns.LastPlayed, DateTime.MinValue.DateTimeToString() },
                    { Columns.variousArtists, album.VariousArtists ? 1 : 0 },
                    { Columns.CreationDate, DateTime.Now.DateTimeToString() },
                    { Columns.UpdateDate, DateTime.MinValue },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTable(ArtistModel artist)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.Name, artist.Name },
                    { Columns.Cover, artist.Cover },
                    { Columns.IsAlbumArtist, artist.IsAlbumArtist ? 1 : 0 },
                    { Columns.IsPerformer, artist.IsPerformer ? 1 : 0 },
                    { Columns.IsComposer, artist.IsComposer ? 1 : 0 },
                    { Columns.IsLyricist, artist.IsLyricist ? 1 : 0 },
                    { Columns.IsFeatured, artist.IsFeatured ? 1 : 0 },
                    { Columns.LastPlayed, DateTime.MinValue.DateTimeToString() },
                    { Columns.PlayCount, artist.PlayCount },
                    { Columns.Biography, artist.Biography },
                    { Columns.CreationDate, DateTime.Now.DateTimeToString() },
                    { Columns.UpdateDate, DateTime.MinValue },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTable(TrackModel track)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.Title, track.Title },
                    { Columns.Artwork, track.Artwork },
                    { Columns.Path, track.Path },
                    { Columns.Length, track.Length },
                    { Columns.Duration, track.Duration },
                    { Columns.TrackNumber, track.Tracknumber },
                    { Columns.DiscNumber, track.DiscNumber },
                    { Columns.LastPlayed, DateTime.MinValue.DateTimeToString() },
                    { Columns.PlayCount, track.PlayCount },
                    { Columns.AlbumId, track.AlbumId },
                    { Columns.IsFavorite, track.IsFavorite ? 1 : 0 },
                    { Columns.Rating, track.Rating },
                    { Columns.CreationDate, DateTime.Now.DateTimeToString() },
                    { Columns.UpdateDate, DateTime.MinValue },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTable(GenreModel genre)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.Name, genre.Name },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTable(PlaylistModel playlist)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.Name, playlist.Name },
                    { Columns.Description, playlist.Description },
                    { Columns.Cover, playlist.Cover },
                    { Columns.CreationDate, DateTime.Now.DateTimeToString() },
                    { Columns.UpdateDate, DateTime.MinValue },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTable(QueueModel queue)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.PlayingTrackId, queue.PlayingTrackId },
                    { Columns.IsShuffled, queue.IsShuffled ? 1 : 0 },
                    { Columns.Cover, queue.Cover },
                    { Columns.PlayingFrom, queue.PlayingFrom },
                    { Columns.IsOnRepeat, queue.IsOnRepeat ? 1 : 0 },
                    { Columns.Length, queue.Length },
                    { Columns.Duration, queue.Duration },
                    { Columns.ModelType, (int)queue.ModelType },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTrackArtistsTable(ArtistDataRelation artistRelation, int trackId)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.TrackId, trackId },
                    { Columns.ArtistId, artistRelation.ArtistId },
                    { Columns.IsPerformer, artistRelation.IsPerformer ? 1 : 0 },
                    { Columns.IsComposer, artistRelation.IsComposer ? 1 : 0 },
                    { Columns.IsLyricist, artistRelation.IsLyricist ? 1 : 0 },
                    { Columns.IsFeatured, artistRelation.IsFeatured ? 1 : 0 },
                    { Columns.IsAlbumArtist, artistRelation.IsAlbumArtist ? 1 : 0 },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateAlbumArtistsTable(ArtistDataRelation artistRelation, int albumId)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.AlbumId, albumId },
                    { Columns.ArtistId, artistRelation.ArtistId },
                    { Columns.IsPerformer, artistRelation.IsPerformer ? 1 : 0 },
                    { Columns.IsComposer, artistRelation.IsComposer ? 1 : 0 },
                    { Columns.IsLyricist, artistRelation.IsLyricist ? 1 : 0 },
                    { Columns.IsFeatured, artistRelation.IsFeatured ? 1 : 0 },
                    { Columns.IsAlbumArtist, artistRelation.IsAlbumArtist ? 1 : 0 },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTable(HistoryModel history)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.Date, history.Date.DateTimeToDateOnlyString() },
                    { Columns.listenTime, history.ListenTime },
                    { Columns.PlayCount, history.PlayCount },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateTable(int playlistId, int trackId, int trackIndex )
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.PlaylistId, playlistId },
                    { Columns.TrackId, trackId },
                    { Columns.Index, trackIndex },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateArtistGenresTable(int artistId, int genreId)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.ArtistId, artistId },
                    { Columns.GenreId, genreId },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateAlbumGenresTable(int albumId, int genreId)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.AlbumId, albumId },
                    { Columns.GenreId, genreId },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateQueueTracksTable(int queueId, int trackId, int trackIndex)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.QueueId, queueId },
                    { Columns.TrackId, trackId },
                    { Columns.Index, trackIndex },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateUpdateTable(ArtistModel artist)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.IsAlbumArtist, artist.IsAlbumArtist ? 1 : 0 },
                    { Columns.IsPerformer, artist.IsPerformer ? 1 : 0 },
                    { Columns.IsComposer, artist.IsComposer ? 1 : 0 },
                    { Columns.IsLyricist, artist.IsLyricist ? 1 : 0 },
                    { Columns.IsFeatured, artist.IsFeatured ? 1 : 0 },
                    { Columns.Biography, artist.Biography },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateUpdateTable(AlbumModel album)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.Name, album.Name },
                    { Columns.AlbumCover, album.AlbumCover },
                    { Columns.CopyRight, album.Copyright },
                    { Columns.Year, album.Year },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateUpdateInteractionTable(int playCount)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { Columns.PlayCount, playCount },
                    { Columns.LastPlayed, DateTime.Now.DateTimeToString() },
                };

                return keyValues;
            }

            public static Dictionary<string, object> CreateUpdateCover(string cover, bool albumCover = false, bool artwork = false)
            {
                string column = Columns.Cover;
                if (albumCover)
                    column = Columns.AlbumCover;
                else if (artwork)
                    column = Columns.Artwork;

                Dictionary<string, object> keyValues = new Dictionary<string, object>
                {
                    { column, cover },
                };

                return keyValues;
            }
        }

        internal class Columns
        {
            public const string Id = "Id";
            public const string ArtistId = "ArtistId";
            public const string AlbumId = "AlbumId";
            public const string TrackId = "TrackId";
            public const string GenreId = "GenreId";
            public const string QueueId = "QueueId";
            public const string PlaylistId = "PlaylistId";
            public const string Path = "Path";
            public const string Name = "Name";
            public const string AlbumCover = "AlbumCover";
            public const string Cover = "Cover";
            public const string CopyRight = "CopyRight";
            public const string Year = "Year";
            public const string IsSingle = "IsSingle";
            public const string IsEP = "IsEP"; 
            public const string PlayCount = "PlayCount";
            public const string LastPlayed = "LastPlayed";
            public const string variousArtists = "variousArtists";
            public const string IsAlbumArtist = "IsAlbumArtist";
            public const string IsComposer = "IsComposer";
            public const string IsPerformer = "IsPerformer";
            public const string IsLyricist = "IsLyricist";
            public const string IsFeatured = "IsFeatured";
            public const string Biography = "Biography";
            public const string Date = "Date";
            public const string listenTime = "listenTime";
            public const string Title = "Title";
            public const string Description = "Description";
            public const string CreationDate = "CreationDate";
            public const string UpdateDate = "UpdateDate";
            public const string Index = "TrackIndex";
            public const string PlayingTrackId = "PlayingTrackId";
            public const string IsShuffled = "IsShuffled";
            public const string PlayingFrom = "PlayingFrom";
            public const string IsOnRepeat = "IsOnRepeat";
            public const string Length = "Length";
            public const string Duration = "Duration";
            public const string ModelType = "ModelType";
            public const string Artwork = "Artwork";
            public const string TrackNumber = "TrackNumber";
            public const string DiscNumber = "DiscNumber";
            public const string IsFavorite = "IsFavorite";
            public const string Rating = "Rating";
        }

        public SQLDataAccess()
        {
            connectionString = DataAccess.GetConnectionString();
            Helper.Init(connectionString);
        }

        private void InsertAlbumArtists(List<ArtistDataRelation> ArtistDataRelations, int albumId)
        {
            foreach (ArtistDataRelation artist in ArtistDataRelations)
            {
                InsertAlbumArtists(artist, albumId);
            }
        }

        public void InsertAlbumArtists(ArtistDataRelation ArtistDataRelation, int albumId)
        {
            Query query = new Query(Tables.TAlbumArtists).AsInsert(Tables.CreateAlbumArtistsTable(ArtistDataRelation, albumId));
            query.Insert(false);
        }

        private void InsertTrackArtists(List<ArtistDataRelation> artistTrackRelations, int trackId)
        {
            foreach (ArtistDataRelation artist in artistTrackRelations)
            {
                InsertTrackArtists(artist, trackId);
            }
        }

        public void InsertTrackArtists(ArtistDataRelation artistTrack, int trackId)
        {
            Query query = new Query(Tables.TTrackArtists).AsInsert(Tables.CreateTrackArtistsTable(artistTrack, trackId));
            query.Insert(false);
        }

        public async Task<AlbumModel> GetAlbum(int id)
        {
            Query query = new Query(Tables.TAlbum).Select().Where(Columns.Id, id);
            AlbumModel album = await query.FirstAsync<AlbumModel>();
            album.Artists = await GetAlbumArtists(id);
            return album;
        }

        public async Task<int> InsertAlbum(AlbumModel album)
        {
            Query query = new Query(Tables.TAlbum).AsInsert(Tables.CreateTable(album));
            int id = await query.InsertAsync();
            InsertAlbumArtists(album.Artists, id);
            return id;
        }

        public Task InsertAlbumGenre(int albumId, int genreId)
        {
            Query query = new Query(Tables.TAlbumGenres).AsInsert(Tables.CreateAlbumGenresTable(albumId, genreId));
            return query.InsertAsync(false);
        }

        public async Task<int> InsertArtist(ArtistModel artist)
        {
            Query query =  new Query(Tables.TArtist).AsInsert(Tables.CreateTable(artist));
            int id = await query.InsertAsync();

            foreach (GenreModel genre in artist.Genres)
            {
                query = new Query(Tables.TArtistGenres).AsInsert(Tables.CreateArtistGenresTable(id, genre.Id));
                query.Insert(false);
            }

            return id;
        }

        public Task InsertArtistGenre(int artistId, int genreId)
        {
            Query query = new Query(Tables.TArtistGenres).AsInsert(Tables.CreateArtistGenresTable(artistId, genreId));
            return query.InsertAsync(false);
        }

        public Task<int> InsertGenre(GenreModel genre)
        {
            Query query =  new Query(Tables.TGenre).AsInsert(Tables.CreateTable(genre));
            return query.InsertAsync();
        }

        public HistoryModel InsertHistoryModel(HistoryModel historyModel)
        {
            Query query =  new Query(Tables.THistory).AsInsert(Tables.CreateTable(historyModel));
            historyModel.Id = query.Insert();
            return historyModel;
        }

        public Task<int> InsertPlaylist(PlaylistModel playlist)
        {
            Query query = new Query(Tables.TPlaylist).AsInsert(Tables.CreateTable(playlist));
            return query.InsertAsync();
        }

        public async Task InsertQueue(QueueModel queue)
        {
            new Query(Tables.TQueueTracks).AsDelete().Delete(); // delete tracks of last queue
            new Query(Tables.TQueue).AsDelete().Delete(); // delete last queue

            Query query = new Query(Tables.TQueue).AsInsert(Tables.CreateTable(queue));

            int id = await query.InsertAsync();

            List<Query> queries = new();
            foreach (OrderedTrackModel t in queue.Tracks)
            {
                queries.Add(new Query(Tables.TQueueTracks).AsInsert(Tables.CreateQueueTracksTable(id, t.Id, t.TrackIndex)));
            }
            queries.Insert();
        }

        public async Task<int> InsertTrack(TrackModel track)
        {
            Query query = new Query(Tables.TTrack).AsInsert(Tables.CreateTable(track));
            int id = await  query.InsertAsync();

            InsertTrackArtists(track.Artists, id);

            return id;
        }

        public Task AddTrackToPlaylist(PlaylistModel playlist, TrackModel track, int trackIndex)
        {
            if (trackIndex < 0)
                trackIndex = 0;

            return new Query(Tables.TPlaylistTracks).AsInsert(Tables.CreateTable(playlist.Id, track.Id, trackIndex)).InsertAsync(false);
        }

        public Task AddTrackToPlaylist(PlaylistModel playlist, List<OrderedTrackModel> tracks)
        {
            List<Query> queries = new List<Query>();
            foreach (OrderedTrackModel track in tracks)
            {
                queries.Add(new Query(Tables.TPlaylistTracks).AsInsert(Tables.CreateTable(playlist.Id, track.Id, track.TrackIndex)));
            }

            return queries.ExecuteAsync();
        }

        public async Task AddTrackToPlaylist(PlaylistModel playlist, List<TrackModel> tracks)
        {
            int index = (await GetTracksFromPlaylist(playlist.Id)).Count + 1;

            List<Query> queries = new List<Query>();
            foreach (TrackModel track in tracks)
            {
                queries.Add(new Query(Tables.TPlaylistTracks).AsInsert(Tables.CreateTable(playlist.Id, track.Id, index)));
                index++;
            }

            await queries.ExecuteAsync();
        }

        public void ClearDataBase()
        {
            // relations first
            new Query(Tables.TQueueTracks).AsDelete().Delete();
            new Query(Tables.TPlaylistTracks).AsDelete().Delete();
            new Query(Tables.TAlbumGenres).AsDelete().Delete();
            new Query(Tables.TArtistGenres).AsDelete().Delete();
            new Query(Tables.TAlbumArtists).AsDelete().Delete();
            new Query(Tables.TTrackArtists).AsDelete().Delete();

            new Query(Tables.TQueue).AsDelete().Delete(); // has a album id

            new Query(Tables.TTrack).AsDelete().Delete();
            new Query(Tables.TAlbum).AsDelete().Delete();
            new Query(Tables.TArtist).AsDelete().Delete();
            new Query(Tables.TPlaylist).AsDelete().Delete();
            new Query(Tables.TGenre).AsDelete().Delete();
            new Query(Tables.THistory).AsDelete().Delete();
        }

        public Task DeleQueue()
        {
            return new Query(Tables.TQueue).AsDelete().DeleteAsync();
        }

        public Task DeleteAlbum(int albumId)
        {
            return new Query(Tables.TAlbum).Where(Columns.Id, albumId).AsDelete().DeleteAsync();
        }

        public Task DeleteArtist(int artistId)
        {
            return new Query(Tables.TArtist).Where(Columns.Id, artistId).AsDelete().DeleteAsync();
        }

        public Task DeleteHistoryModel(DateTime date)
        {
            return new Query(Tables.THistory).Where(Columns.Date, date.DateTimeToDateOnlyString()).AsDelete().DeleteAsync();
        }

        public Task DeletePlaylist(int playlistId)
        {
            return new Query(Tables.TPlaylist).Where(Columns.Id, playlistId).AsDelete().DeleteAsync();
        }

        public Task DeleteTrack(int trackId)
        {
            return new Query(Tables.TTrack).Where(Columns.Id, trackId).AsDelete().DeleteAsync();
        }

        public async Task<List<AlbumModel>> GetAlbumFromGenre(int genreId)
        {
            Query query = new Query(Tables.TAlbumGenres).Select(Columns.AlbumId).Where(Columns.GenreId, genreId);
            List<AlbumModel> albums = await new Query(Tables.TAlbum).WhereIn(Columns.Id, query).GetAsync<AlbumModel>();
            return await GetAlbumArtists(albums);
        }

        public async Task<List<GenreModel>> GetAlbumGenre(int albumId)
        {
            Query query = new Query(Tables.TAlbumGenres).Select(Columns.GenreId).Where(Columns.AlbumId, albumId);
            return await new Query(Tables.TGenre).WhereIn(Columns.Id, query).GetAsync<GenreModel>();
        }

        public Task<int> GetAlbumId(string albumName)
        {
            return new Query(Tables.TAlbum).Select(Columns.Id).WhereLike(Columns.Name, albumName).FirstAsync<int>();
        }

        public Task<string> GetAlbumName(int id)
        {
            return new Query(Tables.TAlbum).Select(Columns.Name).Where(Columns.Id, id).FirstAsync<string>();
        }

        public async Task<List<AlbumModel>> GetAlbumsFromArtist(int artistId)
        {
            Query subQuery = new Query(Tables.TAlbumArtists).Select(Columns.AlbumId).Where(Columns.ArtistId, artistId);
            List<AlbumModel> albums = await new Query(Tables.TAlbum).WhereIn(Columns.Id, subQuery).GetAsync<AlbumModel>();
            return await GetAlbumArtists(albums);
        }

        public async Task<List<AlbumModel>> GetAllAlbums()
        {
            List<AlbumModel> albums = await new Query(Tables.TAlbum).GetAsync<AlbumModel>();
            return await GetAlbumArtists(albums);
        }

        public Task<List<ArtistModel>> GetAllArtists()
        {
            return new Query(Tables.TArtist).GetAsync<ArtistModel>();
        }

        public Task<List<GenreModel>> GetAllGenres()
        {
            return new Query(Tables.TGenre).GetAsync<GenreModel>();
        }

        public Task<List<PlaylistModel>> GetAllPlaylists()
        {
            return new Query(Tables.TPlaylist).GetAsync<PlaylistModel>();
        }

        public async Task<List<TrackModel>> GetAllTracks()
        {
            List<TrackModel> tracks = await new Query(Tables.TTrack).GetAsync<TrackModel>();
            return await GetTracksArtists(tracks);
        }

        public Task<ArtistModel> GetArtist(int id)
        {
            return new Query(Tables.TArtist).Where(Columns.Id, id).FirstAsync<ArtistModel>();
        }


        public Task<GenreModel> GetGenre(int id)
        {
            return new Query(Tables.TGenre).Where(Columns.Id, id).FirstAsync<GenreModel>();
        }
            
        public Task<List<ArtistModel>> GetArtists(IEnumerable<int> ids)
        {
            return new Query(Tables.TArtist).WhereIn(Columns.Id, ids).GetAsync<ArtistModel>();
        }

        public Task<List<ArtistModel>> GetArtistFromGenre(int genreId)
        {
            Query query = new Query(Tables.TArtistGenres).Select(Columns.ArtistId).Where(Columns.GenreId, genreId);
            return new Query(Tables.TArtist).WhereIn(Columns.Id, query).GetAsync<ArtistModel>();
        }

        public Task<List<GenreModel>> GetArtistGenre(int artistId)
        {
            Query query = new Query(Tables.TArtistGenres).Select(Columns.GenreId).Where(Columns.ArtistId, artistId);
            return new Query(Tables.TGenre).WhereIn(Columns.Id, query).GetAsync<GenreModel>();
        }

        public Task<int> GetArtistId(string artistName)
        {
            return new Query(Tables.TArtist).Select(Columns.Id).WhereLike(Columns.Name, artistName).FirstAsync<int>();
        }

        public Task<string> GetArtistName(int id)
        {
            return new Query(Tables.TArtist).Select(Columns.Name).Where(Columns.Id, id).FirstAsync<string>();
        }

        public async Task<List<TrackModel>> GetFavoriteTracks()
        {
            List <TrackModel> tracks = await new Query(Tables.TTrack).Where(Columns.IsFavorite, 1).GetAsync<TrackModel>();

            return await GetTracksArtists(tracks);
        }

        public Task<List<HistoryModel>> GetHistoryModels(DateTime date, int numberOfWeek)
        {
            DateTime endDate = date.AddDays(numberOfWeek * 7);
            return new Query(Tables.THistory).WhereBetween<DateTime>(Columns.Date, date, endDate).GetAsync<HistoryModel>();
        }

        public async Task<List<AlbumModel>> GetLastPlayedAlbums(int top)
        {
            Query query = new Query(Tables.TAlbum).WhereNot(Columns.LastPlayed, "=", DateTime.MinValue.DateTimeToString()).OrderByRaw($"datetime({Columns.LastPlayed}) DESC").Limit(top);
            List<AlbumModel> albums = await  query.GetAsync<AlbumModel>();
            return await GetAlbumArtists(albums);
        }

        public Task<List<ArtistModel>> GetLastPlayedArtists(int top)
        {
            return new Query(Tables.TArtist).WhereNot(Columns.LastPlayed, "=", DateTime.MinValue.DateTimeToString()).Where(Columns.IsAlbumArtist, 1).OrderByRaw($"datetime({Columns.LastPlayed}) DESC").Limit(top).GetAsync<ArtistModel>();
        }

        public async Task<List<TrackModel>> GetLastPlayedTracks(int top)
        {
            List<TrackModel> tracks = await new Query(Tables.TTrack).WhereNot(Columns.LastPlayed, "=", DateTime.MinValue.DateTimeToString()).OrderByRaw($"datetime({Columns.LastPlayed}) DESC").Limit(top).GetAsync<TrackModel>();
            return await GetTracksArtists(tracks);
        }

        public Task<List<AlbumModel>> GetMostPlayedAlbums(int top)
        {
            return new Query(Tables.TAlbum).WhereNot(Columns.PlayCount, "=", 0).OrderByDesc(Columns.PlayCount).Limit(top).GetAsync<AlbumModel>();
        }

        public Task<List<ArtistModel>> GetMostPlayedArtists(int top)
        {
            return new Query(Tables.TArtist).WhereNot(Columns.PlayCount, "=", 0).OrderByDesc(Columns.PlayCount).Limit(top).GetAsync<ArtistModel>();
        }

        public async Task<List<TrackModel>> GetMostPlayedTracks(int top)
        {
            List<TrackModel> tracks = await new Query(Tables.TTrack).WhereNot(Columns.PlayCount, "=", 0).OrderByDesc(Columns.PlayCount).Limit(top).GetAsync<TrackModel>();
            return await GetTracksArtists(tracks);
        }

        public async Task<(int, int, int)> GetNumberOfEntries()
        {
            int artist = await new Query(Tables.TArtist).AsCount().CountAsync();
            int album = await new Query(Tables.TAlbum).AsCount().CountAsync();
            int track = await new Query(Tables.TTrack).AsCount().CountAsync();

            return (artist, album, track);
        }

        public Task<PlaylistModel> GetPlaylist(int id)
        {
            Query query = new Query(Tables.TPlaylist).Where(Columns.Id, id);
            return Helper.FirstAsync<PlaylistModel>(query);
        }

        public Task<int> GetPlaylistId(string playlistName)
        {
            Query query = new Query(Tables.TPlaylist).Select(Columns.Id).WhereLike(Columns.Name, playlistName);
            return Helper.FirstAsync<int>(query);
        }

        public async Task<QueueModel> GetQueue()
        {
            QueueModel queueModel = await new Query(Tables.TQueue).FirstAsync<QueueModel>();
            
            if(queueModel != null && queueModel.Length != 0)
            {
                List<QueueTrackIdsModel> tracksIds = new Query(Tables.TQueueTracks).Select(Columns.TrackId, Columns.Index).Where(Columns.QueueId, queueModel.Id).Get<QueueTrackIdsModel>();
                List<int> ids = tracksIds.Select(t => t.TrackId).ToList();
                List<OrderedTrackModel> tracks = (await new Query(Tables.TTrack).WhereIn(Columns.Id, ids).GetAsync<OrderedTrackModel>());
                foreach (OrderedTrackModel t in tracks)
                {
                    t.TrackIndex = tracksIds.Find(tid => tid.TrackId == t.Id).TrackIndex;
                }

                queueModel.Tracks = await GetTracksArtists<OrderedTrackModel>(tracks);
                queueModel.PlayingTrack = tracks.Find(t => t.Id == queueModel.PlayingTrackId);
            }
            return queueModel;
        }

        public HistoryModel GetTodayHistoryModel()
        {
            return new Query(Tables.THistory).Where(Columns.Date, DateTime.Now.DateTimeToDateOnlyString()).First<HistoryModel>();
        }

        public async Task<TrackModel> GetTrack(int id)
        {
            TrackModel track = await new Query(Tables.TTrack).Where(Columns.Id, id).FirstAsync<TrackModel>();
            track.Artists = await GetTracksArtists(track.Id);
            return track;
        }

        private async Task<List<AlbumModel>> GetAlbumArtists(List<AlbumModel> albums)
        {
            foreach (AlbumModel album in albums)
            {
                album.Artists = await GetAlbumArtists(album.Id);
            }
            return albums;
        }

        private Task<List<ArtistDataRelation>> GetAlbumArtists(int id)
        {
            return  new Query(Tables.TAlbumArtists).Select(Tables.TAlbumArtists + ".*", Tables.TArtist + ".name").Where(Columns.AlbumId, id).Join(Tables.TArtist, Columns.Id, Columns.ArtistId).GetAsync<ArtistDataRelation>();
        }

        private async Task<List<T>> GetTracksArtists<T>(List<T> tracks) where T : TrackModel
        {
            foreach (T track in tracks)
            {
                track.Artists = await GetTracksArtists(track.Id);
            }
            return tracks;
        }

        private async Task<List<OrderedTrackModel>> GetTracksArtists(List<OrderedTrackModel> tracks)
        {
            foreach (OrderedTrackModel track in tracks)
            {
                track.Artists = await GetTracksArtists(track.Id);
            }
            return tracks;
        }

        private Task<List<ArtistDataRelation>> GetTracksArtists(int id)
        {
            return new Query(Tables.TTrackArtists).Select(Tables.TTrackArtists + ".*", Tables.TArtist + ".name").Where(Columns.TrackId, id).Join(Tables.TArtist, Columns.Id, Columns.ArtistId).GetAsync<ArtistDataRelation>();
        }

        public Task<List<TrackModel>> GetTrackBasedOnLastPlayedAndPlayCount(DateTime lowerDate, DateTime upperDate, int lowerPlayCount, int upperPlayCount)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TrackModel>> GetTracksFromAlbums(IEnumerable<int> albumsId)
        {
            List<TrackModel> tracks = await new Query(Tables.TTrack).WhereIn(Columns.AlbumId, albumsId).GetAsync<TrackModel>();
            return await GetTracksArtists(tracks);
        }

        public async Task<List<TrackModel>> GetTracksFromArtists(IEnumerable<int> artistsId)
        {
            Query query = new Query(Tables.TTrackArtists).Select(Columns.TrackId).WhereIn(Columns.ArtistId, artistsId);
            List<TrackModel> tracks = (await new Query(Tables.TTrack).WhereIn(Columns.Id, query).GetAsync<TrackModel>()).ToList();
            return await GetTracksArtists(tracks);

        }

        public async Task<TrackModel> GetTrackByPath(string path)
        {
            TrackModel track = await new Query(Tables.TTrack).Where(Columns.Path, path).FirstAsync<TrackModel>();
            track.Artists = await GetTracksArtists(track.Id);
            return track;
        }

        public Task<List<int>> GetTrackPlaylistsIds(int trackId)
        {
            return new Query(Tables.TPlaylistTracks).Select(Columns.PlaylistId).Where(Columns.TrackId, trackId).GetAsync();
        }

        public async Task<List<TrackModel>> GetTracksFromAlbum(int albumId)
        {
            List<TrackModel> tracks = await new Query(Tables.TTrack).Where(Columns.AlbumId, albumId).GetAsync<TrackModel>();
            return await GetTracksArtists(tracks);
        }

        public async Task<List<TrackModel>> GetTracksFromArtist(int artistId)
        {
            Query query = new Query(Tables.TTrackArtists).Select(Columns.TrackId).Where(Columns.ArtistId, artistId);
            List<TrackModel> tracks = (await new Query(Tables.TTrack).WhereIn(Columns.Id, query).GetAsync<TrackModel>()).ToList();
            return await GetTracksArtists(tracks);
        }

        public async Task<List<OrderedTrackModel>> GetTracksFromPlaylist(int playlistId)
        {
            List<PlaylistTrackIdsModel> tracksIds = new Query(Tables.TPlaylistTracks).Select(Columns.TrackId, Columns.Index).Where(Columns.PlaylistId, playlistId).Get<PlaylistTrackIdsModel>();
            List<OrderedTrackModel> tracks = (await new Query(Tables.TTrack).WhereIn(Columns.Id, tracksIds.Select(t => t.TrackId)).GetAsync<OrderedTrackModel>()).ToList();
            foreach (OrderedTrackModel t in tracks)
            {
                t.TrackIndex = tracksIds.Find(tid => tid.TrackId == t.Id).TrackIndex;
            }
            return await GetTracksArtists(tracks);
        }

        public async Task<List<TrackModel>> GetTracksFromQueue(int queueId)
        {
            Query query = new Query(Tables.TQueueTracks).Select(Columns.TrackId).Where(Columns.QueueId, queueId);
            List<TrackModel> tracks = (await new Query(Tables.TTrack).WhereIn(Columns.Id, query).GetAsync<TrackModel>()).ToList();
            return await GetTracksArtists(tracks);
        }

        public Task RemoveAlbumGenre(int albumId, int genreId)
        {
            return new Query(Tables.TAlbumGenres).Where(Tables.CreateAlbumGenresTable(albumId, genreId)).AsDelete().DeleteAsync();
        }

        public async Task RemoveArtistGenre(int artistId, int genreId)
        {
            await new Query(Tables.TArtistGenres).Where(Tables.CreateArtistGenresTable(artistId, genreId)).AsDelete().DeleteAsync();
        }

        public Task RemoveTrackFromPlaylist(PlaylistModel playlist, TrackModel track)
        {
            return new Query(Tables.TPlaylistTracks).Where(Columns.PlaylistId, playlist.Id).Where(Columns.TrackId, track.Id).AsDelete().DeleteAsync();
        }

        public async Task<List<AlbumModel>> SearchAlbums(List<int> genresId, List<int> artistsId, List<AlbumTypeEnum> albumTypes, string searchString, SortEnum sortEnum, bool ascending = false)
        {
            Query query = new Query(Tables.TAlbum); // main query

            if(genresId.Count > 0)
            {
                // selected genres query
                Query selectedGenre = new Query(Tables.TAlbumGenres).Select(Columns.AlbumId).OrWhereIn(Columns.GenreId, genresId);
                query.OrWhereIn(Columns.Id, selectedGenre);
            }

            if(artistsId.Count > 0)
            {
                // selected artists (filters) query
                Query selectedArtist = new Query(Tables.TAlbumArtists).Select(Columns.AlbumId).OrWhereIn(Columns.ArtistId, artistsId);
                query.OrWhereIn(Columns.Id, selectedArtist);
            }

            if(albumTypes.Count > 0)
            {
                bool isSingle = false;
                bool isEP = false;
                bool isCompilation = false;

                foreach (AlbumTypeEnum albumType in albumTypes)
                {
                    switch (albumType)
                    {
                        case AlbumTypeEnum.Single:
                            isSingle = true;
                            break;
                        case AlbumTypeEnum.EP:
                            isEP = true;
                            break;
                        case AlbumTypeEnum.Compilation:
                            isCompilation = true;
                            break;
                        default:
                            break;
                    }
                }

                Query selectedAlbumsType = new Query(Tables.TAlbum).Select(Columns.Id);

                if (isSingle)
                {
                    selectedAlbumsType.OrWhere(Columns.IsSingle, 1);
                }

                if(isEP)
                {
                    selectedAlbumsType.OrWhere(Columns.IsEP, 1);
                }

                if (isCompilation)
                {
                    selectedAlbumsType.OrWhere(Columns.variousArtists, 1);
                }

                query.WhereIn(Columns.Id, selectedAlbumsType);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                // searched artist query
                Query SearchArtist = new Query(Tables.TArtist).Select(Columns.Id).WhereStarts(Columns.Name, searchString);
                Query searchedArtists = new Query(Tables.TAlbumArtists).Select(Columns.AlbumId).WhereIn(Columns.ArtistId, SearchArtist);

                // sub query for search so that both the found artist and albums then select only among the already filtered albums (in the main query)
                // it is needed because of the orWhereIn that would select albums that do not follow previous filterings int the main query
                Query searchedAlbums = new Query(Tables.TAlbum).Select(Columns.Id).WhereContains(Columns.Name, searchString)
                                                                                 .OrWhereIn(Columns.Id, searchedArtists);

                query.WhereIn(Columns.Id, searchedAlbums);
            }

            // sorted column
            string[] sortColumn = Sort(sortEnum);

            List<AlbumModel> albums = await query.When(ascending, q => q.OrderBy(sortColumn), q => q.OrderByDesc(sortColumn))
                                                .GetAsync<AlbumModel>();
            return await GetAlbumArtists(albums);
        }

        public async Task<List<ArtistModel>> SearchArtists(List<int> artistTypes, string searchString, SortEnum sortEnum, bool ascending = false)
        {
            Query query = new Query(Tables.TArtist);

            bool isAlbumArtist = false;
            bool isComposer = false;
            bool isPerformer = false;
            bool isFeatured = false;
            bool isLyricist = false;

            foreach (int artistType in artistTypes)
            {
                switch ((ArtistTypeEnum)artistType)
                {
                    case ArtistTypeEnum.AlbumArtist:
                        isAlbumArtist = true;
                        break;
                    case ArtistTypeEnum.Composer:
                        isComposer = true;
                        break;
                    case ArtistTypeEnum.Performer:
                        isPerformer = true;
                        break;
                    case ArtistTypeEnum.Lyricist:
                        isLyricist = true;
                        break;
                    case ArtistTypeEnum.Featured:
                        isFeatured = true;
                        break;
                }
            }

            bool or = false;

            // need to group the filters in one query dif from the main
            // otherwise the search query only apply to the last filter
            Query filterQuery = new Query(Tables.TArtist).Select(Columns.Id);
            if(isAlbumArtist)
            {
                or = true;
                filterQuery.Where(Columns.IsAlbumArtist, 1);
            }

            if(isComposer)
            {
                filterQuery.When(or, q => q.OrWhere(Columns.IsComposer, 1), q => q.Where(Columns.IsComposer, 1));

                or = true;
            }

            if (isPerformer)
            {
                filterQuery.When(or, q => q.OrWhere(Columns.IsPerformer, 1), q => q.Where(Columns.IsPerformer, 1));

                or = true;
            }

            if (isFeatured)
            {
                filterQuery.When(or, q => q.OrWhere(Columns.IsFeatured, 1), q => q.Where(Columns.IsFeatured, 1));

                or = true;
            }

            if (isLyricist)
            {
                query.When(or, q => q.OrWhere(Columns.IsLyricist, 1), q => q.Where(Columns.IsLyricist, 1));

                or = true;
            }

            if(isAlbumArtist || or)
            {
                query.WhereIn(Columns.Id, filterQuery);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                Query SearchArtist = new Query(Tables.TArtist).Select(Columns.Id).WhereContains(Columns.Name, searchString);
                query.WhereIn(Columns.Id, SearchArtist);
            }

            string[] sortColumn = Sort(sortEnum);

            return await query.When(ascending, q => q.OrderBy(sortColumn), q => q.OrderByDesc(sortColumn))
                                .GetAsync<ArtistModel>();
        }

        private static string[] Sort(SortEnum sortEnum)
        {
            string[] sortColumn = new string[1];
            sortColumn[0] = Columns.Name; // A-Z
            switch (sortEnum)
            {
                case SortEnum.MostPlayed:
                    sortColumn[0] = Columns.PlayCount;
                    break;
                case SortEnum.LastPlayed:
                    sortColumn[0] = Columns.LastPlayed;
                    break;
                case SortEnum.AddedDate:
                    sortColumn[0] = Columns.CreationDate;
                    break;
                case SortEnum.UpdatedDate:
                    sortColumn[0] = Columns.UpdateDate;
                    break;
                default:
                    break;
            }
            return sortColumn;
        }

        public Task UpdateAlbum(AlbumModel album)
        {
            return new Query(Tables.TAlbum).Where(Columns.Id, album.Id).AsUpdate(Tables.CreateUpdateTable(album)).ExecuteAsync();
        }

        public Task UpdateAlbumCover(AlbumModel album)
        {
            return new Query(Tables.TAlbum).Where(Columns.Id, album.Id).AsUpdate(Tables.CreateUpdateCover(album.AlbumCover, true)).ExecuteAsync();
        }

        public void UpdateAlbumInteraction(int playCount, int id)
        {
            new Query(Tables.TAlbum).Where(Columns.Id, id).AsUpdate(Tables.CreateUpdateInteractionTable(playCount)).Execute();
        }

        public Task UpdateArtist(ArtistModel artist)
        {
            return new Query(Tables.TArtist).Where(Columns.Id, artist.Id).AsUpdate(Tables.CreateUpdateTable(artist)).ExecuteAsync();
        }

        public Task UpdateArtistCover(ArtistModel artist)
        {
            return new Query(Tables.TArtist).Where(Columns.Id, artist.Id).AsUpdate(Tables.CreateUpdateCover(artist.Cover)).ExecuteAsync();
        }

        public void UpdateArtistInteraction(int playCount, int id)
        {
            new Query(Tables.TArtist).Where(Columns.Id, id).AsUpdate(Tables.CreateUpdateInteractionTable(playCount)).Execute();
        }

        public void UpdateHistoryModel(HistoryModel historyModel)
        {
            new Query(Tables.THistory).Where(Columns.Id, historyModel.Id).AsUpdate(new Dictionary<string, object>() 
            { 
                { Columns.listenTime, historyModel.ListenTime },
                { Columns.PlayCount, historyModel.PlayCount },
            }).Execute();
        }

        public Task UpdatePlaylist(PlaylistModel playlist)
        {
            return new Query(Tables.TPlaylist).Where(Columns.Id, playlist.Id).AsUpdate(new Dictionary<string, object>()
            {
                { Columns.Name, playlist.Name },
                { Columns.Description, playlist.Description },
                { Columns.Cover, playlist.Cover },
                { Columns.UpdateDate, DateTime.Now.DateTimeToString() },
            }).ExecuteAsync();
        }

        public Task UpdatePlaylistTracks(int playlistId, List<OrderedTrackModel> tracks)
        {
            List<Query> queries = new List<Query>();
            foreach (OrderedTrackModel track in tracks)
            {
                queries.Add(new Query(Tables.TPlaylistTracks).Where(Columns.TrackId, track.Id).WhereNot(Columns.Index, track.TrackIndex).AsUpdate(new Dictionary<string, object>()
                {
                    { Columns.Index, track.TrackIndex },
                }));
            }
            return queries.ExecuteAsync();
        }

        public Task UpdateTrackArtist(TrackModel track)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTrackArtwork(TrackModel track)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTrackDiscTrackNumber(TrackModel track)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTrackIsFavorite(TrackModel track)
        {
            return new Query(Tables.TTrack).Where(Columns.Id, track.Id).AsUpdate(new Dictionary<string, object>()
            {
                { Columns.IsFavorite, track.IsFavorite ? 1 : 0 },
            }).ExecuteAsync();
        }

        public void UpdateTrackPlayCount(TrackModel track)
        {
            new Query(Tables.TTrack).Where(Columns.Id, track.Id).AsUpdate(Tables.CreateUpdateInteractionTable(track.PlayCount)).Execute();
        }

        public void UpdateTrackRating(TrackModel track)
        {
            new Query(Tables.TTrack).Where(Columns.Id, track.Id).AsUpdate(new Dictionary<string, object>()
            {
                { Columns.Rating, track.Rating },
            }).Execute();
        }
    }
}
