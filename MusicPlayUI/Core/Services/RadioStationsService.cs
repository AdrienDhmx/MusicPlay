using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicFilesProcessor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;

namespace MusicPlayUI.Core.Services
{
    public class RadioStationsService : IRadioStationsService
    {
        private List<PlaylistModel> _todayRadioStations;
        public List<PlaylistModel> TodayRadioStations
        {
            get { return _todayRadioStations; }
            set { _todayRadioStations = value; }
        }

        private readonly Random rng = new();

        /// <summary>
        /// The number of tracks a radio has.
        /// </summary>
        public int RadioStationTrackNumber { get; set; } = 20;

        /// <summary>
        /// The min percentage of tracks included in the radio created by the same artist/Performer as the track the radio is based on
        /// </summary>
        public int ArtistInfluence { get; set; } = 40;

        /// <summary>
        /// The min percentage of tracks included the radio having the same Chips as the track the radio is based on.
        /// </summary>
        public int GenreInfluence { get; set; } = 60;


        public RadioStationsService()
        {
        }

        public async Task<List<PlaylistModel>> CreateRadioStations(int number)
        {
            if (TodayRadioStations != null)
                return TodayRadioStations;
            return await Task.Run(async () =>
            {
                List<PlaylistModel> radioStations = new List<PlaylistModel>();
                List<TrackModel> Tracks = await DataAccess.Connection.GetLastPlayedTracks(100);

                if (Tracks.Count > RadioStationTrackNumber * 2)
                {
                    for (int i = 0; i < number; i++)
                    {
                        int index = rng.Next(Tracks.Count - 1);
                        PlaylistModel playlist = await CreateRadioStation(Tracks.ElementAt(index));
                        if (playlist != null)
                            radioStations.Add(playlist);
                        Tracks.RemoveAt(index);
                    }
                    TodayRadioStations = radioStations;
                }

                return radioStations;
            });
        }

        public async Task<PlaylistModel> CreateRadioStation()
        {
            List<TrackModel> Tracks = await DataAccess.Connection.GetLastPlayedTracks(100);
            if (Tracks.Count > RadioStationTrackNumber * 2)
            {
                int index = rng.Next(0, Tracks.Count);
                return await CreateRadioStation(Tracks.ElementAt(index));
            }
            return null;
        }


        public async Task<PlaylistModel> CreateRadioStation(TrackModel track)
        {
            AlbumModel album = await DataAccess.Connection.GetAlbum(track.AlbumId);
            track.AlbumName = album.Name;
            track.AlbumCover = album.AlbumCover;

            ArtistDataRelation ArtistDataRelation = album.GetAlbumArtist();
            if(ArtistDataRelation == null)
            {
                ArtistDataRelation = album.Artists[0];
            }
            ArtistModel artist = await DataAccess.Connection.GetArtist(ArtistDataRelation.ArtistId);

            List<GenreModel> genres = await DataAccess.Connection.GetAlbumGenre(album.Id);
            List<TrackModel> RadioTracks = new();
            List<TrackModel> Tracks = new();
            List<AlbumModel> albums = new();

            PlaylistModel radio = new();
            radio.Name = $"{track.Title} Radio";
            radio.PlaylistType = MusicPlayModels.Enums.PlaylistTypeEnum.Radio;
            radio.Cover = string.IsNullOrWhiteSpace(track.Artwork) ? track.AlbumCover : track.Artwork;

            // add all tracks with the same genre
            foreach (GenreModel genreModel in genres)
            {
                if (genreModel is null) continue;

                albums = await DataAccess.Connection.GetAlbumFromGenre(genreModel.Id);

                Tracks.AddRange(await GetTracksFromAlbums(albums));
            }

            // add tracks from the same performer
            Tracks.AddRange(await DataAccess.Connection.GetTracksFromArtist(artist.Id));
            albums = await DataAccess.Connection.GetAlbumsFromArtist(artist.Id);
            Tracks.AddRange(await GetTracksFromAlbums(albums));

            if(Tracks.Count < RadioStationTrackNumber)
            {
                ArtistModel albumArtist = await DataAccess.Connection.GetArtist(album.GetAlbumArtist().ArtistId);
                if (albumArtist != null)
                {
                    Tracks.AddRange(await DataAccess.Connection.GetTracksFromArtist(albumArtist.Id));
                    albums = await DataAccess.Connection.GetAlbumsFromArtist(albumArtist.Id);
                    Tracks.AddRange(await GetTracksFromAlbums(albums));
                }
            }

            Tracks = TrackListHelper.Shuffle(Tracks).ToList();

            if(Tracks.Count >= RadioStationTrackNumber)
            {
                RadioTracks = Tracks.GetRange(0, RadioStationTrackNumber);
            }
            else
            {
                RadioTracks = Tracks;
            }

            RadioTracks.Insert(0, track);
            RadioTracks = RadioTracks.DistinctBy(t => t?.Id).ToList();

            radio.Tracks = RadioTracks.ToOrderedTrackModel();
            radio.Duration = radio.Tracks.GetTotalLength(out int _);
            return radio;
        }

        private async static Task<List<TrackModel>> GetTracksFromAlbums(List<AlbumModel> albums)
        {
            if (albums is null) return new();

            List<TrackModel> tracks = new List<TrackModel>();
            foreach (AlbumModel a in albums)
            {
                if (a is null) continue;

                tracks.AddRange(await DataAccess.Connection.GetTracksFromAlbum(a.Id));
            }
            return tracks;
        }
    }
}
