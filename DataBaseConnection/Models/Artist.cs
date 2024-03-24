using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("Artist")]
    public class Artist : PlayableModel
    {
        private string _cover = string.Empty;
        private string _name = string.Empty;
        private string _biography = string.Empty;
        private string _realName = string.Empty;

        private int _birthDate = 0;
        private int _deathDate = 0;
        private bool _isGroup = false;
 
        private Country _country;

        private ObservableCollection<ArtistGroupMember> _groupMembers = [];

        private ObservableCollection<ArtistRole> _artistRole = [];
        private List<Album> _albums;
        private List<ArtistTag> _artistTags = [];
        private ObservableCollection<Track> _tracks = [];

        [NotMapped]
        public bool AreAlbumsLoaded { get; private set; } = false;

        public int? CountryId { get; set; }

        /// <summary>
        /// The artist name
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                SetField(ref _name, value);
                OnPropertyChanged(nameof(Acronyme));
            }
        }

        [NotMapped]
        public string Acronyme
        {
            get
            {
                string formatedName = Regex.Replace(Name, """[-,.&_':\\/;`~!?<>%*^$#@()\["'1-9]*""", "").ToUpper();
                string[] words = formatedName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if(words.Length > 1)
                {
                    return $"{words[0][0]}{words[^1][0]}";
                } 
                else if (words[0].Length > 1)
                {
                    return $"{words[0][0]}{words[0][1]}";
                }
                return words[0][0].ToString();
            }
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

        public Country Country
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

        public ObservableCollection<ArtistGroupMember> GroupMembers
        {
            get => _groupMembers;
            set => SetField(ref _groupMembers, value);
        }

        public ObservableCollection<ArtistRole> ArtistRoles
        {
            get 
            { 
                if(_artistRole.IsNullOrEmpty())
                {
                    DatabaseContext context = new();
                    _artistRole = [..context.ArtistRoles.Where(ar => ar.ArtistId == Id)];
                }
                return _artistRole;
            }
            set => SetField(ref _artistRole, value);
        }

        // All the albums the artist is credited as the Primary Artist
        public List<Album> Albums
        {
            get 
            {
                if(_albums.IsNull())
                {
                    _albums = Album.GetAllFromArtist(Id);
                    AreAlbumsLoaded = true;
                }
                return _albums;
            }
            set
            {
                SetField(ref _albums, value);
                AreAlbumsLoaded = value != null;
            }
        }

        // All the tracks the artists is credited on
        [NotMapped]
        public ObservableCollection<Track> Tracks
        {
            get
            {
                if(_tracks.IsNullOrEmpty()) // fetch
                {
                    DatabaseContext context = new();
                    _tracks = [.. context.Tracks.Where(t => t.TrackArtistRole
                                                    .Any(tar => tar.ArtistRole.ArtistId == Id)
                                                )
                                                .Where(t => t.Album.PrimaryArtistId == Id)
                                                .OrderBy(t => t.AlbumId)
                                                .ThenBy(t => t.TrackNumber)
                            ];
                }
                return _tracks;
            }
        }

        public List<ArtistTag> ArtistTags
        {
            get 
            { 
                if(_artistTags.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _artistTags = [..context.ArtistTags.Where(at => at.ArtistId == Id)
                                                        .Include(at => at.Tag)];
                }
                return _artistTags;
            }
            set => SetField(ref _artistTags, value);
        }

        private ObservableCollection<Tag> _tags;
        [NotMapped]
        public ObservableCollection<Tag> Tags
        {
            get
            {
                if(_tags.IsNullOrEmpty())
                {
                    _tags = new(ArtistTags.Select(t => t.Tag));
                }

                return _tags;
            }
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

        public Artist(int id, string name, string cover, string duration)
        {
            Id = id;
            Name = name;
            Cover = cover;
        }

        public Artist(string name)
        {
            Name = name;
            Cover = string.Empty;
        }

        public Artist()
        {

        }

        /// <summary>
        /// Insert an Artist and its roles to the database, if needed new ArtistRoles and Country will also be inserted
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public static async ValueTask Insert(Artist artist)
        {
            using DatabaseContext context = new();

            foreach (ArtistRole artistRole in artist.ArtistRoles)
            {
                context.ArtistRoles.Add(artistRole);
            }

            context.Artists.Add(artist);
            await context.SaveChangesAsync();
        }

        public static async ValueTask<Artist?> Get(int id)
        {
            using DatabaseContext context = new();
            return await context.Artists.FindAsync(id);
        }

        public static Artist? GetSync(int id)
        {
            using DatabaseContext context = new();
            return context.Artists.Find(id);
        }

        public static List<Artist> GetAll()
        {
            using DatabaseContext context = new();
            return
            [
                .. context.Artists
                        .Include(a => a.ArtistRoles)
                        .ThenInclude(ar => ar.Role)
                        .Include(a => a.Country)
,
            ];
        }

        public static List<Artist> GetByArtistRoles(List<int> artistRolesId)
        {
            using DatabaseContext context = new();
            return [..context.Artists.Where(a => a.ArtistRoles.Any(ar => artistRolesId.Any(arId => arId == ar.Id)))];
        }

        public static List<Artist> GetLastPlayed(int top)
            => GetLastPlayed<Artist>(top);

        public static List<Artist> GetMostPlayed(int top)
            => GetMostPlayed<Artist>(top);

        public static int Count()
        {
            using DatabaseContext context = new();
            return context.Artists.Count();
        }

        public static async Task Update(Action<Artist> update, Artist artist)
        {
            var roles = artist.ArtistRoles;
            artist.ArtistRoles = null;
            using DatabaseContext context = new();
            context.Artists.Update(artist);
            update(artist);
            await context.SaveChangesAsync();
            artist.ArtistRoles = roles;
        }

        public static async Task UpdateCover(Artist artist, string newCover)
            => await Update(a => a.Cover = newCover, artist);

        public static async Task UpdatePlayCount(Artist artist)
            => await Update(a => PlayableModel.UpdatePlayCount(a), artist);

        public static async Task Delete(Artist artist)
        {
            using DatabaseContext context = new();
            context.Artists.Remove(artist);
            await context.SaveChangesAsync();
        }

        public static async Task AddArtistRole(Artist artist, ArtistRole artistRole)
        {
            artistRole.Artist = null;
            Role role = artistRole.Role;
            artistRole.Role = null;

            using DatabaseContext context = new();
            context.ArtistRoles.Add(artistRole);
            await context.SaveChangesAsync();

            artistRole.Artist = artist;
            artistRole.Role = role;
            artist.ArtistRoles.Add(artistRole);
        }
    }
}
