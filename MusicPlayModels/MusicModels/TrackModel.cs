using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayModels.Interfaces;

namespace MusicPlayModels.MusicModels
{
    public class TrackModel : PlayableModel, ITaggable
    {
        private readonly int _folderId;
        private string _artwork = "";
        private string _path = "";
        private string _title = "";
        private int _trackNumber = 0;
        private int _discNumber = 1;
        private bool _isLive = false;
        private LyricsModel? _lyrics;

        private double _rating = 0;
        private bool _isFavorite = false;

        private List<TagModel> _tags = new();
        private AlbumModel _album;
        private List<TrackArtistsRoleModel> _artists = new();

        public string Path
        {
            get => _path;
            set => SetField(ref _path, value);
        }

        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
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

        public LyricsModel Lyrics
        {
            get => _lyrics ?? new();
            set => SetField(ref _lyrics, value);
        }

        public AlbumModel Album
        {
            get => _album;
            set => SetField(ref _album, value);
        }

        public List<TrackArtistsRoleModel> Artists
        {
            get => _artists;
            set => SetField(ref _artists, value);
        }

        public List<TagModel> Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }

        public bool HasLyrics => _lyrics != null;

        public TrackModel(int id, int folderId, string path, string title, List<TrackArtistsRoleModel> artists, AlbumModel album, List<TagModel> tags, string artwork, int trackNumber, int discNumber, int length, string duration, bool isLive, double rating, bool isFavorite)
        {
            Id = id;
            _folderId = folderId;
            _tags = tags;
            _album = album;
            Artists = artists;
            Path = path;
            Title = title;
            Artwork = artwork;
            TrackNumber = trackNumber;
            DiscNumber = discNumber;
            Length = length;
            Duration = duration;
            IsLive = isLive;
            Rating = rating;
            IsFavorite = isFavorite;
        }

        public TrackModel(int id, int folderId, string path, string title, List<TrackArtistsRoleModel> artists, AlbumModel album, List<TagModel> tags, LyricsModel lyrics, string artwork, int trackNumber, int discNumber, int length, string duration, bool isLive, double rating, bool isFavorite)
        {
            Id = id;
            _folderId = folderId;
            _tags = tags;
            _album = album;
            Artists = artists;
            Lyrics = lyrics;
            Path = path;
            Title = title;
            Artwork = artwork;
            TrackNumber = trackNumber;
            DiscNumber = discNumber;
            Length = length;
            Duration = duration;
            IsLive = isLive;
            Rating = rating;
            IsFavorite = isFavorite;
        }

        public TrackModel(TrackModel trackModel)
        {
            if(trackModel != null)
            {
                Id = trackModel.Id;
                Path = trackModel.Path;
                Title = trackModel.Title;
                Artists = trackModel.Artists;
                _album = trackModel.Album;
                Artwork = trackModel.Artwork;
                TrackNumber = trackModel.TrackNumber;
                DiscNumber = trackModel.DiscNumber;
                Length = trackModel.Length;
                Duration = trackModel.Duration;
                Album = trackModel.Album;
                PlayCount = trackModel.PlayCount;
                LastPlayed = trackModel.LastPlayed;
                Rating= trackModel.Rating;
                IsFavorite= trackModel.IsFavorite;
                IsLive= trackModel.IsLive;
                Lyrics = trackModel.Lyrics;
                Tags = trackModel.Tags;
            }
        }

        public TrackModel(OrderedTrackModel playlistTrackModel)
        {
            Id = playlistTrackModel.Id;
            Path = playlistTrackModel.Path;
            Title = playlistTrackModel.Title;
            Artists = playlistTrackModel.Artists;
            _album = playlistTrackModel.Album;
            Artwork = playlistTrackModel.Artwork;
            TrackNumber = playlistTrackModel.TrackNumber;
            DiscNumber = playlistTrackModel.DiscNumber;
            Length = playlistTrackModel.Length;
            Duration = playlistTrackModel.Duration;
            Album = playlistTrackModel.Album;
            PlayCount = playlistTrackModel.PlayCount;
            LastPlayed = playlistTrackModel.LastPlayed;
            Rating = playlistTrackModel.Rating;
            IsFavorite = playlistTrackModel.IsFavorite;
            IsLive = playlistTrackModel.IsLive;
            Lyrics= playlistTrackModel.Lyrics;
            Tags = playlistTrackModel.Tags;
        }

        public TrackModel()
        {

        }

        public override Dictionary<string, object> CreateTable()
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { nameof(Title), Title },
                { nameof(Artwork), Artwork },
                { nameof(Path), Path },
                { nameof(Length), Length },
                { nameof(Duration), Duration },
                { nameof(TrackNumber), TrackNumber },
                { nameof(DiscNumber), DiscNumber },
                { nameof(PlayCount), PlayCount },
                { DataBaseColumns.AlbumId, Album.Id },
                { nameof(IsFavorite), IsFavorite.ToInt() },
                { nameof(Rating), Rating },
                { DataBaseColumns.FolderId,  _folderId},
            };

            if(HasLyrics)
            {
                keyValues.Add(DataBaseColumns.LyricsId, Lyrics.Id);
            }

            return base.CreateTable().AddRange(keyValues);
        }
    }
}
