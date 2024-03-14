using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("Track")]
    public class Track : PlayableModel
    {
        private string _artwork = "";
        private string _path = "";
        private string _title = "";
        private int _trackNumber = 0;
        private int _discNumber = 1;
        private bool _isLive = false;
        private double _rating = 0;
        private bool _isFavorite = false;

        // relations 
        private Lyrics _lyrics;
        private Album _album;
        private Folder _folder;
        private ObservableCollection<TrackArtistsRole> _trackArtistRole = [];
        private ObservableCollection<TrackTag> _trackTags = [];
        private ObservableCollection<PlaylistTrack> _playlistTracks = [];

        // relation ids
        [Required]
        public int AlbumId { get; set; }
        [Required]
        public int FolderId { get; set; }
        public int? LyricsId { get; set; }

        [Required]
        public string Path
        {
            get => _path;
            set => SetField(ref _path, value);
        }

        [Required]
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        [Required]
        public override int Length
        {
            get => _length;
            set => SetField(ref _length, value);
        }

        public string Artwork
        {
            get => _artwork;
            set
            {
                SetField(ref _artwork, value);
            }
        }

        public int TrackNumber
        {
            get => _trackNumber;
            set => SetField(ref _trackNumber, value);
        }

        public int DiscNumber
        {
            get => _discNumber;
            set => SetField(ref _discNumber, value);
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }

        public double Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public bool IsLive
        {
            get => _isLive;
            set => SetField(ref _isLive, value);
        }

        public Lyrics Lyrics
        {
            get => _lyrics ?? new();
            set => SetField(ref _lyrics, value);
        }

        public Album Album
        {
            get
            {
                if (_album.IsNull())
                {
                    using DatabaseContext context = new();
                    _album = context.Albums.Find(AlbumId);
                }
                return _album;
            }
            set => SetField(ref _album, value);
        }

        public ObservableCollection<TrackArtistsRole> TrackArtistRole
        {
            get
            {
                if(_trackArtistRole.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _trackArtistRole = [..context.TrackArtistRoles
                            .Where(tar => tar.TrackId == Id)
                            .Include(tar => tar.ArtistRole)
                            .ThenInclude(ar => ar.Artist)
                        ];
                }
                return _trackArtistRole;
            }
            set => SetField(ref _trackArtistRole, value);
        }

        public Folder Folder
        {
            get => _folder;
            set => SetField(ref _folder, value);
        }

        public ObservableCollection<TrackTag> TrackTags
        {
            get => _trackTags;
            set => SetField(ref _trackTags, value);
        }

        public ObservableCollection<PlaylistTrack> PlaylistTracks
        {
            get {
                if(_playlistTracks.IsNull())
                {
                    using DatabaseContext context = new();
                    _playlistTracks = [..context.PlaylistTracks.Where(pt => pt.TrackId == Id)];
                }
                return _playlistTracks;
            }
            set => SetField(ref _playlistTracks, value);
        }

        private ObservableCollection<Artist> _artists;
        [NotMapped]
        public ObservableCollection<Artist> Artists
        {
            get
            {
                if(_artists.IsNullOrEmpty())
                {
                    _artists = [];
                    _artists.Add(Album.PrimaryArtist);
                    foreach (TrackArtistsRole tar in TrackArtistRole)
                    {
                        _artists.Add(tar.ArtistRole.Artist);
                    }
                }
                return _artists;
            }
        }

        private ObservableCollection<Playlist> _playlists;
        [NotMapped]
        public ObservableCollection<Playlist> Playlists
        {
            get
            {
                if(_playlists.IsNull())
                {
                    _playlists = [];
                    foreach (PlaylistTrack playlistTrack in PlaylistTracks)
                    {
                        _playlists.Add(playlistTrack.Playlist);
                    }
                }
                return _playlists;
            }
        }

        public bool HasLyrics => _lyrics != null;

        public Track(int id, int folderId, string path, string title, List<TrackArtistsRole> artists, Album album, List<TrackTag> tags, string artwork, int trackNumber, int discNumber, int length, bool isLive, double rating, bool isFavorite)
        {
            Id = id;
            FolderId = folderId;
            _trackTags = new(tags);
            _album = album;
            TrackArtistRole = new(artists);
            Path = path;
            Title = title;
            Artwork = artwork;
            TrackNumber = trackNumber;
            DiscNumber = discNumber;
            Length = length;
            IsLive = isLive;
            Rating = rating;
            IsFavorite = isFavorite;
        }

        public Track(int id, int folderId, string path, string title, List<TrackArtistsRole> artists, Album album, List<TrackTag> tags, Lyrics lyrics, string artwork, int trackNumber, int discNumber, int length, bool isLive, double rating, bool isFavorite)
        {
            Id = id;
            FolderId = folderId;
            _trackTags = new(tags);
            _album = album;
            TrackArtistRole = new(artists);
            Lyrics = lyrics;
            Path = path;
            Title = title;
            Artwork = artwork;
            TrackNumber = trackNumber;
            DiscNumber = discNumber;
            Length = length;
            IsLive = isLive;
            Rating = rating;
            IsFavorite = isFavorite;
        }

        public Track(Track trackModel)
        {
            if (trackModel == null) return;

            Id = trackModel.Id;
            Path = trackModel.Path;
            Title = trackModel.Title;
            TrackArtistRole = trackModel.TrackArtistRole;
            _album = trackModel.Album;
            _folder = trackModel.Folder;
            Artwork = trackModel.Artwork;
            TrackNumber = trackModel.TrackNumber;
            DiscNumber = trackModel.DiscNumber;
            Length = trackModel.Length;
            PlayCount = trackModel.PlayCount;
            LastPlayed = trackModel.LastPlayed;
            Rating = trackModel.Rating;
            IsFavorite = trackModel.IsFavorite;
            IsLive = trackModel.IsLive;
            Lyrics = trackModel.Lyrics;
            TrackTags = trackModel.TrackTags;
        }

        public Track()
        {

        }

        public static async ValueTask Insert(Track track)
        {
            using DatabaseContext context = new();
            context.Tracks.Add(track);
            await context.SaveChangesAsync();
        }

        public static async ValueTask<Track?> Get(int id)
        {
            using DatabaseContext context = new();
            return await context.Tracks.FindAsync(id);
        }

        public static List<Track> GetAll()
        {
            using DatabaseContext context = new();
            return [.. context.Tracks];
        }

        public static async Task<List<Track>> GetAllFromAlbum(int albumId)
        {
            using DatabaseContext context = new();
            return await context.Tracks.Where(t => t.AlbumId == albumId).ToListAsync();
        }

        public static List<Track> GetAllFromArtist(int artistId)
        {
            using DatabaseContext context = new();
            return
            [
                .. context.Tracks.Include(t => t.TrackArtistRole)
                                .ThenInclude(tar => tar.ArtistRole)
                                .Where(t => t.TrackArtistRole.Any(tar => tar.ArtistRole.ArtistId == artistId))
,
            ];
        }

        public static List<Track> GetFavorites()
        {
            using DatabaseContext context = new();
            return [.. context.Tracks.Where(t => t.IsFavorite)];
        }

        public static List<Track> GetLastPlayed(int top)
            => GetLastPlayed<Track>(top);


        public static List<Track> GetMostPlayed(int top)
            => GetMostPlayed<Track>(top);

        public static int Count()
        {
            using DatabaseContext context = new();
            return context.Tracks.Count();
        }

        public static int CountFromFolder(int folderId)
        {
            using DatabaseContext context = new();
            return context.Tracks.Where(t => t.FolderId == folderId).Count();
        }

        public static async Task Update(Action<Track> update, int id)
        {
            using DatabaseContext context = new();

            Track track = context.Tracks.Find(id);
            if (track == null)
                return;
            update(track);
            context.Tracks.Update(track);
            await context.SaveChangesAsync();
        }

        public static async Task UpdateIsFavorite(Track track)
            => await Update(t => t.IsFavorite = !t.IsFavorite, track.Id);

        public static async Task UpdateRating(Track track, double newRating)
            => await Update(t => t.Rating = newRating, track.Id);

        public static async Task UpdateArtwork(Track track, string newArtwork)
            => await Update(t => t.Artwork = newArtwork, track.Id);

        public static async Task UpdatePlayCount(Track track)
            => await Update(t => PlayableModel.UpdatePlayCount(t), track.Id);


        public static async Task Delete(Track track)
        {
            using DatabaseContext context = new();
            context.Tracks.Remove(track);
            await context.SaveChangesAsync();
        }
    }
}
