using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayModels.Enums;
using MusicPlayModels.Interfaces;

namespace MusicPlayModels.MusicModels
{
    public class AlbumModel : PlayableModel, ITaggable
    {
        private string _name = "";
        private string _copyright = "";
        private string _albumCover = "";
        private string _label = "";
        private int _releaseDate = 0;
        private bool _variousArtists = false;
        private int _type = 0; // Main album by default
        private bool _isLive = false;
        private bool _isCompilation = false;
        private string _information = "";

        private ArtistModel _primaryArtist;
        private List<TrackArtistsRoleModel> _credits = new();

        private List<TrackModel> _tracks = new ();
        private List<TagModel> _tags = new();

        /// <summary>
        /// The name / title of the album
        /// </summary>
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
        public string Label
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
        /// Note: The <see cref="PrimaryArtist"/> is named "Various Artists" when this is true
        /// </summary>
        public bool VariousArtists
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

        /// <summary>
        /// The type of the album. 
        /// The type is deduced based on the number of tracks and the duration of the album.
        /// </summary>
        public AlbumType Type
        {
            get => (AlbumType)_type;
            set => SetField(ref _type, (int)value);
        }

        /// <summary>
        /// The main artist of the album, also called "Album Artist".
        /// </summary>
        public ArtistModel PrimaryArtist
        {
            get => _primaryArtist;
            set => SetField(ref _primaryArtist, value);
        }

        /// <summary>
        /// All the artists credited on the album apart from the <see cref="PrimaryArtist"/>.
        /// </summary>
        public List<TrackArtistsRoleModel> Credits
        {
            get => _credits;
            set => SetField(ref _credits, value);
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
        public List<TrackModel> Tracks
        {
            get => _tracks;
            set => SetField(ref _tracks, value);
        }

        /// <summary>
        /// The tags of the album
        /// </summary>
        public List<TagModel> Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }

        public AlbumModel(string title, ArtistModel primaryArtist, string copyright, string cover, int length)
        {
            Name = title;
            _primaryArtist = primaryArtist;
            Copyright = copyright;
            AlbumCover = cover;
            Length = length;
        }

        public AlbumModel(ArtistModel primaryArtist)
        {
            _primaryArtist = PrimaryArtist;
        }

        public AlbumModel()
        {
        }

        public bool IsMainAlbum()
        {
            return Type == AlbumType.Main;
        }

        public List<int> GetArtistsId()
        {
            List<int> ids = new List<int>();

            foreach (var artist in Credits)
            {
                ids.Add(artist.Artist.Id);
            }

            return ids;
        }
    }
}
