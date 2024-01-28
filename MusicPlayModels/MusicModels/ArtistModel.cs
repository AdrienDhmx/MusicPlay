using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayModels.Interfaces;

namespace MusicPlayModels.MusicModels
{
    public class ArtistModel : PlayableModel, ITaggable
    {
        private string _cover = string.Empty;
        private string _name = string.Empty;
        private string _biography = string.Empty;
        private string _realName = string.Empty;

        private int _birthDate = 0;
        private int _deathDate = 0;

        private string _country = string.Empty;
        private bool _isGroup = false;
        private List<ArtistModel> _groupMembers = new();

        private List<ArtistRoleModel> _roles = new List<ArtistRoleModel>();
        private List<AlbumModel> _albums = new List<AlbumModel>();
        private List<TrackModel> _tracks = new List<TrackModel>();
        private List<TagModel> _tags = new List<TagModel>();

        /// <summary>
        /// The artist name
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        /// <summary>
        /// The artist cover
        /// </summary>
        public string Cover
        {
            get => _cover;
            set
            {
                _cover = value;
                OnPropertyChanged(nameof(Cover));
            }
        }

        public string Biography
        {
            get => _biography;
            set => SetField(ref _biography, value);
        }

        public string RealName
        {
            get => _realName;
            set => SetField(ref _realName, value);
        }

        public string Country
        {
            get => _country;
            set => SetField(ref _country, value);
        }

        public int BirthDate
        {
            get => _birthDate;
            set 
            { 
                SetField(ref _birthDate, value);
                OnPropertyChanged(nameof(Birth));
            }
        }

        public int DeathDate
        {
            get => _deathDate;
            set
            {
                SetField(ref _deathDate, value);
                OnPropertyChanged(nameof(Death));
            }
        }

        public bool IsGroup
        {
            get => _isGroup;
            set => SetField(ref _isGroup, value);
        }

        public List<ArtistModel> GroupMembers
        {
            get => _groupMembers;
            set => SetField(ref _groupMembers, value);
        }

        public List<ArtistRoleModel> Roles
        {
            get => _roles;
            set => SetField(ref _roles, value);
        }

        // All the albums the artist is credited as the Primary Artist
        public List<AlbumModel> Albums
        {
            get => _albums;
            set => SetField(ref _albums, value);
        }

        // All the tracks the artists is credited on
        public List<TrackModel> Tracks
        {
            get => _tracks;
            set => SetField(ref _tracks, value);
        }

        public List<TagModel> Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }

        public string Birth
        {
            get => TimestampToDateString(_birthDate);
        }

        public string Death
        {
            get => TimestampToDateString(_deathDate);
        }

        public ArtistModel(int id, string name, string cover, string duration)
        {
            Id = id;
            Name = name;
            Cover = cover;
            Duration = duration;
        }

        public ArtistModel(string name)
        {
            Name = name;
            Cover = string.Empty;
            Duration = string.Empty;
        }

        public ArtistModel()
        {

        }

        public override Dictionary<string, object> CreateTable()
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { nameof(Name), Name },
                { nameof(Cover), Cover },
                { nameof(Biography), Biography },
                { nameof(RealName), RealName },
                { nameof(BirthDate), BirthDate },
                { nameof(DeathDate), DeathDate },
                { nameof(IsGroup), IsGroup.ToInt() },
            };
            return base.CreateTable().AddRange(keyValues);
        }
    }
}
