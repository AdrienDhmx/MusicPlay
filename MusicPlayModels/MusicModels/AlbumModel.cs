using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class AlbumModel : ArtistsRelation
    {
        private string _albumCover = "";

        private string _duration = "";

        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Copyright { get; set; } = "";

        public string AlbumCover
        {
            get => _albumCover;
            set
            {
                _albumCover = value;
                OnPropertyChanged(nameof(AlbumCover));
            }
        }

        public bool VariousArtists { get; set; }

        public int Year { get; set; }

        public bool IsSingle { get; set; } = false;

        public bool IsEP { get; set; } = false;

        public List<GenreModel> Genres { get; set; } = new();

        public string Duration 
        {
            get => _duration;
            set
            {
                SetField(ref _duration, value);
            }
        }

        public int Length { get; set; }

        public int PlayCount { get; set; } = 0;
        public DateTime LastPlayed { get; set; } = DateTime.MinValue;

        public AlbumModel(string title, string copyright, string cover, int year)
        {
            Name = title;
            Copyright = copyright;
            AlbumCover = cover;
            Year = year;
        }

        public AlbumModel()
        {

        }

        public bool IsAlbum()
        {
            return !IsEP && !IsSingle;
        }
    }
}
