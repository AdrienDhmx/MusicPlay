using MusicPlay.Database.Enums;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlay.Database.DatabaseAccess;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlay.Database.Models
{
    [Table("Album")]
    public class Album : PlayableModel
    {
        private string _name = "";
        private string _copyright = "";
        private string _albumCover = "";
        private int _releaseDate = 0;
        private bool _variousArtists = false;
        private int _type = 0; // Main album by default
        private bool _isLive = false;
        private bool _isCompilation = false;
        private string _information = "";

        private Label _label;
        private Artist _primaryArtist;

        private ObservableCollection<Track> _tracks = new();
        private List<AlbumTag> _albumTags = new();


        public int? LabelId { get; set; }
        [Required]
        public int PrimaryArtistId { get; set; }

        /// <summary>
        /// The name / title of the album
        /// </summary>
        [Required]
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        /// <summary>
        /// The copyright of the album
        /// </summary>
        public string Copyright
        {
            get => _copyright;
            set => SetField(ref _copyright, value);
        }

        /// <summary>
        /// The label under which the album is released
        /// </summary>
        public Label Label
        {
            get => _label;
            set => SetField(ref _label, value);
        }

        /// <summary>
        /// The path to the cover / artwork / jacket of the album
        /// </summary>
        public string AlbumCover
        {
            get => _albumCover;
            set => SetField(ref _albumCover, value);
        }

        /// <summary>
        /// If the album doesn't have a primary artists and is a compilation of tracks from different artist.
        /// Note: The <see cref="PrimaryArtist"/> is named "Various TrackArtistRole" when this is true
        /// </summary>
        public bool IsVariousArtists
        {
            get => _variousArtists;
            set => SetField(ref _variousArtists, value);
        }

        /// <summary>
        /// The release year or date of the album.
        /// </summary>
        public int ReleaseDate
        {
            get => _releaseDate;
            set => SetField(ref _releaseDate, value);
        }

        public override int Length
        {
            get
            {
                if(_length == 0)
                {
                    _length = Tracks.GetTotalLength();
                }
                return _length;
            }
            set
            {
                SetField(ref _length, value);
                OnPropertyChanged(nameof(Duration));
            }
        }

        /// <summary>
        /// The type of the album. 
        /// The type is deduced based on the number of tracks and the duration of the album.
        /// </summary>
        public AlbumTypeEnum Type
        {
            get => (AlbumTypeEnum)_type;
            set => SetField(ref _type, (int)value);
        }

        /// <summary>
        /// The main artist of the album, also called "Album Artist".
        /// </summary>
        public Artist PrimaryArtist
        {
            get
            {
                if(_primaryArtist.IsNull())
                {
                    using DatabaseContext context = new();
                    _primaryArtist = context.Artists.Find(PrimaryArtistId);
                }
                return _primaryArtist;
            }
            set => SetField(ref _primaryArtist, value);
        }

        /// <summary>
        /// The formatted string of the release year or date of the album 
        /// </summary>
        public string Release
        {
            get => TimestampToDateString(_releaseDate);
        }

        /// <summary>
        /// Whether the album is a recording of a concert / live show
        /// </summary>
        public bool IsLive
        {
            get => _isLive;
            set => SetField(ref _isLive, value);
        }

        /// <summary>
        /// Whether the album is a compilation
        /// </summary>
        public bool IsCompilation
        {
            get => _isCompilation;
            set => SetField(ref _isCompilation, value);
        }

        /// <summary>
        /// Information related to the album (Wikipedia Article, critics or user thoughts)
        /// </summary>
        public string Information
        {
            get => _information;
            set => SetField(ref _information, value);
        }

        /// <summary>
        /// The tracks in the album
        /// </summary>
        public ObservableCollection<Track> Tracks
        {
            get {
                if (_tracks.IsNotNullOrEmpty())
                {
                    return _tracks;
                }

                if (_tracks.IsNullOrEmpty()) // fetch
                {
                    using DatabaseContext context = new();
                    _tracks = [..
                        context.Tracks
                            .Where(t => t.AlbumId == Id)
                            .Include(t => t.TrackArtistRole)
                            .ThenInclude(tar => tar.ArtistRole)
                            .ThenInclude(ar => ar.Artist)
                            .OrderBy(t => t.TrackNumber)
                    ];
                }
                return _tracks;
            }
            set => SetField(ref _tracks, value);
        }

        public ObservableCollection<OrderedTrack> OrderedTracks
        {
            get => Tracks.ToOrderedTrack();
        }

        /// <summary>
        /// The tags of the album
        /// </summary>
        public List<AlbumTag> AlbumTags
        {
            get 
            {
                if(_albumTags.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _albumTags = [..context.AlbumTags.Where(at => at.AlbumId == Id)
                                                    .Include(at => at.Tag)];
                }
                return _albumTags;
            }
            set => SetField(ref _albumTags, value);
        }

        private ObservableCollection<Tag> _tags;
        [NotMapped]
        public ObservableCollection<Tag> Tags
        {
            get
            {
                if(_tags.IsNullOrEmpty())
                {
                    _tags = new(AlbumTags.Select(at => at.Tag));
                }
                return _tags;
            }
        }

        private ObservableCollection<Artist> _creditedArtists;
        [NotMapped]
        public ObservableCollection<Artist> CreditedArtists
        {
            get
            {
                if (_creditedArtists.IsNullOrEmpty()) // fetch
                {
                    List<int> artistRoleIds = Tracks.SelectMany(t => t.TrackArtistRole.Select(tar => tar.ArtistRoleId))
                                                    .ToList();
                    _creditedArtists = new(Artist.GetByArtistRoles(artistRoleIds));
                }
                return _creditedArtists;
            }
            set => SetField(ref _creditedArtists, value);
        }

        public Album(string title, Artist primaryArtist, string copyright, string cover, int length)
        {
            Name = title;
            _primaryArtist = primaryArtist;
            Copyright = copyright;
            AlbumCover = cover;
            Length = length;
        }

        public Album()
        {
        }

        public bool IsMainAlbum()
        {
            return Type == AlbumTypeEnum.Main;
        }

        public List<int> GetArtistsId()
        {
            List<int> ids = new List<int>();

            //foreach (var artist in Credits)
            //{
            //    ids.Add(artist.ArtistRole.ArtistId);
            //}

            return ids;
        }

        /// <summary>
        /// Insert an album and a new label if it doesn't exist
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public static async ValueTask Insert(Album album)
        {
            using DatabaseContext context = new();
            context.Albums.Add(album);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Get an album with all its tracks
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async ValueTask<Album?> Get(int id)
        {
            using DatabaseContext context = new();
            return await context.Albums.FindAsync(id);
        }

        /// <summary>
        /// Get all the albums in the database without their tracks
        /// </summary>
        /// <returns></returns>
        public static List<Album> GetAll()
        {
            using DatabaseContext context = new();
            return [.. context.Albums.Include(a => a.PrimaryArtist)];
        }
        /// <summary>
        /// Get all the album with its tracks where the artist is the primary artist
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public static List<Album> GetAllFromArtist(int artistId)
        {
            using DatabaseContext context = new();
            return context.Albums.Where(a => a.PrimaryArtistId == artistId).ToList();
        }

        public static List<Album> GetLastPlayed(int top)
            => GetLastPlayed<Album>(top);

        public static async Task<List<Album>> GetMostPlayed(int top)
            => await GetMostPlayed<Album>(top)
                        .Include(a => a.Label)
                        .Include(a => a.PrimaryArtist)
                        .ToListAsync();

        public static int Count()
        {
            return GetAll().Count;
        }

        public static async Task Update(Action<Album> update, Album album)
        {
            using DatabaseContext context = new();
            if(!context.Albums.Local.Any(a => a.Id == album.Id)) 
            {
                context.Albums.Attach(album);

            }
            update(album);
            await context.SaveChangesAsync();
        }

        public static async Task UpdateCover(Album album, string newCover)
            => await Update(a => a.AlbumCover = newCover, album);

        public static async Task Delete(Album album)
        {
            using DatabaseContext context = new();
            context.Albums.Remove(album);
            await context.SaveChangesAsync();
        }

    }
}
