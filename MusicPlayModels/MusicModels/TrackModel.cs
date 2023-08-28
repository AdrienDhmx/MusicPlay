using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class TrackModel : ArtistsRelation
    {
        private string _artwork = "";
        public string Path { get; set; } = "";
        public string Title { get; set; } = "";
        public int AlbumId { get; set; }

        public string Artwork
        {
            get => _artwork;
            set
            {
                _artwork = value;
                OnPropertyChanged(nameof(Artwork));
            }
        }

        public List<TagModel> Tags { get; set; } = new();

        public int Tracknumber { get; set; } = 0;

        public int DiscNumber { get; set; } = 1;

        /// <summary>
        /// The track length in seconds
        /// </summary>
        public int Length { get; set; } = 0;

        /// <summary>
        /// The track duration in the following format: HH:MM:SS
        /// </summary>
        public string Duration { get; set; } = "";

        private DateTime _lastPlayed = DateTime.MinValue;
        private int _playCount = 0;
        private double _rating = 0;
        private bool _isFavorite = false;

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
        public int PlayCount
        {
            get => _playCount;
            set
            {
                _playCount = value;
                OnPropertyChanged(nameof(PlayCount));
            }
        }
        public DateTime LastPlayed
        {
            get => _lastPlayed;
            set
            {
                _lastPlayed = value;
                OnPropertyChanged(nameof(LastPlayed));
            }
        }

        public string AlbumName { get; set; } = "";
        public string AlbumCover { get; set; } = "";

        public TrackModel(int id, string path, string title, int album, List<ArtistDataRelation> artists, string artwork, int tracknumber, int discNumber, int length, string duration) : base(artists)
        {
            Id = id;
            Path = path;
            Title = title;
            AlbumId = album;
            Artwork = artwork;
            Tracknumber = tracknumber;
            DiscNumber = discNumber;
            Length = length;
            Duration = duration;
        }

        public TrackModel(string path, string title, int album, List<ArtistDataRelation> artists, string artwork, int tracknumber, int discNumber, int length, string duration) : base(artists)
        {
            Path = path;
            Title = title;
            AlbumId = album;
            Artwork = artwork;
            Tracknumber = tracknumber;
            DiscNumber = discNumber;
            Length = length;
            Duration = duration;
        }

        public TrackModel(TrackModel trackModel)
        {
            if(trackModel != null)
            {
                Id = trackModel.Id;
                Path = trackModel.Path;
                Title = trackModel.Title;
                AlbumId = trackModel.AlbumId;
                Artists = trackModel.Artists;
                Artwork = trackModel.Artwork;
                Tracknumber = trackModel.Tracknumber;
                DiscNumber = trackModel.DiscNumber;
                Length = trackModel.Length;
                Duration = trackModel.Duration;
                AlbumCover = trackModel.AlbumCover;
                AlbumName = trackModel.AlbumName;
                PlayCount = trackModel.PlayCount;
                LastPlayed = trackModel.LastPlayed;
                Rating= trackModel.Rating;
                IsFavorite= trackModel.IsFavorite;
            }
        }

        public TrackModel(OrderedTrackModel playlistTrackModel)
        {
            Id = playlistTrackModel.Id;
            Path = playlistTrackModel.Path;
            Title = playlistTrackModel.Title;
            AlbumId = playlistTrackModel.AlbumId;
            Artists = playlistTrackModel.Artists;
            Artwork = playlistTrackModel.Artwork;
            Tracknumber = playlistTrackModel.Tracknumber;
            DiscNumber = playlistTrackModel.DiscNumber;
            Length = playlistTrackModel.Length;
            Duration = playlistTrackModel.Duration;
            AlbumCover = playlistTrackModel.AlbumCover;
            AlbumName = playlistTrackModel.AlbumName;
            PlayCount = playlistTrackModel.PlayCount;
            LastPlayed = playlistTrackModel.LastPlayed;
            Rating = playlistTrackModel.Rating;
            IsFavorite = playlistTrackModel.IsFavorite;
        }

        public TrackModel()
        {

        }
    }
}
