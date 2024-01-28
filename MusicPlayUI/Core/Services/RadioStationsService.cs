
using MusicFilesProcessor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;

namespace MusicPlayUI.Core.Services
{
    public class RadioStationsService : IRadioStationsService
    {
        private List<Playlist> _todayRadioStations;
        public List<Playlist> TodayRadioStations
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
        /// The min percentage of tracks included the radio having the same CurrentTagView as the track the radio is based on.
        /// </summary>
        public int GenreInfluence { get; set; } = 60;


        public RadioStationsService()
        {
        }

        public async Task<List<Playlist>> CreateRadioStations(int number)
        {
            if (TodayRadioStations != null)
                return TodayRadioStations;
            return await Task.Run(async () =>
            {
                List<Playlist> radioStations = new List<Playlist>();
                List<Track> Tracks = new(); //await DataAccess.Connection.GetLastPlayedTracks(100);

                if (Tracks.Count > RadioStationTrackNumber * 2)
                {
                    for (int i = 0; i < number; i++)
                    {
                        int index = rng.Next(Tracks.Count - 1);
                        Playlist playlist = await CreateRadioStation(Tracks.ElementAt(index));
                        if (playlist != null)
                            radioStations.Add(playlist);
                        Tracks.RemoveAt(index);
                    }
                    TodayRadioStations = radioStations;
                }

                return radioStations;
            });
        }

        public async Task<Playlist> CreateRadioStation()
        {
            List<Track> Tracks = new();//await DataAccess.Connection.GetLastPlayedTracks(100);
            if (Tracks.Count > RadioStationTrackNumber * 2)
            {
                int index = rng.Next(0, Tracks.Count);
                return await CreateRadioStation(Tracks.ElementAt(index));
            }
            return null;
        }


        public async Task<Playlist> CreateRadioStation(Track track)
        {
            Album album = track.Album;

            Artist primaryArtist = album.PrimaryArtist;

            List<Tag> genres = new();//await DataAccess.Connection.GetAlbumTag(album.Id);
            //genres.AddRange(await DataAccess.Connection.GetTrackTag(track.Id));

            List<Track> RadioTracks = new();
            List<Track> Tracks = new();
            List<Album> albums = new();
            List<Playlist> playlists = new();

            Playlist radio = new();
            radio.Name = $"{track.Title} - Radio";
            radio.Description = $"A radio based on the track '{track.Title}' by {album.PrimaryArtist.Name}.";
            radio.PlaylistType = PlaylistTypeEnum.Radio;
            radio.Cover = string.IsNullOrWhiteSpace(track.Artwork) ? track.Album.AlbumCover : track.Artwork;

            // add all tracks with the same genre
            foreach (Tag genreModel in genres)
            {
                if (genreModel is null) continue;

                //albums = await DataAccess.Connection.GetAlbumFromTag(genreModel.Id);
                //playlists = await DataAccess.Connection.GetPlaylistFromTag(genreModel.Id);

                //foreach (var playlist in playlists)
                //{

                //    TrackTags.AddRange(await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id));
                //}

 
                //TrackTags.AddRange(await GetTracksFromAlbums(albums));
                //TrackTags.AddRange(await DataAccess.Connection.GetTrackFromTag(genreModel.Id));
            }

            // add tracks from the same performer
            //TrackTags.AddRange(await DataAccess.Connection.GetTracksFromArtist(artist.Id));
            //albums = await DataAccess.Connection.GetAlbumsFromArtist(artist.Id);
            //TrackTags.AddRange(await GetTracksFromAlbums(albums));

            //if(TrackTags.Count < RadioStationTrackNumber)
            //{
            //    Artist albumArtist = await DataAccess.Connection.GetArtist(album.GetAlbumArtist().ArtistId);
            //    if (albumArtist != null)
            //    {
            //        TrackTags.AddRange(await DataAccess.Connection.GetTracksFromArtist(albumArtist.Id));
            //        albums = await DataAccess.Connection.GetAlbumsFromArtist(albumArtist.Id);
            //        TrackTags.AddRange(await GetTracksFromAlbums(albums));
            //    }
            //}

            Tracks = Tracks.DistinctBy(t => t?.Id).Shuffle().ToList();
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

            radio.Tracks = new(); //RadioTracks.ToOrderedTrackModel();
            return radio;
        }

        private async static Task<List<Track>> GetTracksFromAlbums(List<Album> albums)
        {
            if (albums is null) return new();

            List<Track> tracks = new List<Track>();
            foreach (Album a in albums)
            {
                if (a is null) continue;
                tracks.AddRange(a.Tracks);
            }
            return tracks;
        }
    }
}
