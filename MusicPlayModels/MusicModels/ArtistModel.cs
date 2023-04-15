using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class ArtistModel : DatedModel
    {
        private string _cover = "";
        private string _duration = "";

        /// <summary>
        /// The unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Thge artist name
        /// </summary>
        public string Name { get; set; } = "";

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

        /// <summary>
        /// The total duration of all the tracks the artist made and contributed to.
        /// </summary>
        public string Duration 
        {
            get => _duration;
            set
            {
                SetField(ref _duration, value);
            }
        }
        public string Biography { get; set; } = "";

        public List<GenreModel> Genres { get; set; }

        public int PlayCount { get; set; } = 0;
        public DateTime LastPlayed { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Wether the artist only appear as the main artist of 1 or more albums
        /// -- Allows to avoid database query when the artist has no individual tracks (performer) --
        /// </summary>
        public bool IsAlbumArtist { get; set; }

        /// <summary>
        /// Wether the artist is the main artist of an album or only appear on tracks as a performer
        /// true = no albums has the artist id in its table, only 1 or more tracks has.
        /// false = the artist has albums (his id is in the table of 1 or more albums).
        /// -- Allows to avoid database query when the artist has no album --
        /// </summary>
        public bool IsPerformer { get; set; }

        public bool IsComposer { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsLyricist { get; set; }

        public ArtistModel(int id, string name, string cover, string duration, bool isAlbumArtistOnly, bool isPerformerOnly)
        {
            Id = id;
            Name = name;
            Cover = cover;
            Duration = duration;
            IsAlbumArtist = isAlbumArtistOnly;
            IsPerformer = isPerformerOnly;
        }

        public ArtistModel(string name)
        {
            Name = name;
            Cover = "";
            Duration = "";
            IsAlbumArtist = false;
            IsPerformer = false;
        }

        public ArtistModel()
        {

        }
    }
}
