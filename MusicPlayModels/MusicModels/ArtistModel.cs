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
        private string _name = "";
        private string _biography = "";

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

        public string Biography
        {
            get => _biography;
            set => SetField(ref _biography, value);
        }

        public List<TagModel> Tags { get; set; }

        public int PlayCount { get; set; } = 0;
        public DateTime LastPlayed { get; set; } = DateTime.MinValue;

        public bool IsAlbumArtist { get; set; }

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
