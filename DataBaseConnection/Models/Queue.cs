using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("Queue")]
    public class Queue : PlayableModel
    {
        private bool _isShuffled = false;
        private bool _isOnRepeat = false;
        private string _playingFromName = "";
        private string _cover = "";
        private ModelTypeEnum _playingFromModelType = ModelTypeEnum.UNKNOWN;
        private int _playingFromId = -1;
        private PlayableModel _playingFrom;
        private ObservableCollection<QueueTrack> _tracks = [];
        private Track _playingTrack;

        public bool IsShuffled
        {
            get { return _isShuffled; }
            set
            {
                SetField(ref _isShuffled, value);
            }
        }

        public bool IsOnRepeat
        {
            get { return _isOnRepeat; }
            set
            {
                SetField(ref _isOnRepeat, value);
            }
        }

        [NotMapped]
        public PlayableModel PlayingFrom
        {
            get 
            {
                if(PlayingFromId != 0)
                {
                    using DatabaseContext context = new();
                    switch(PlayingFromModelType)
                    {
                        case ModelTypeEnum.Album:
                            _playingFrom = context.Albums.Find(PlayingFromId);
                            break;
                        case ModelTypeEnum.Artist:
                            _playingFrom = context.Artists.Find(PlayingFromId);
                            break;
                        case ModelTypeEnum.Playlist:
                            _playingFrom = context.Playlists.Find(PlayingFromId);
                            break;
                        case ModelTypeEnum.Tag:
                            _playingFrom = context.Tags.Find(PlayingFromId);
                            break;
                        default:
                            return null;
                    }
                }

                return _playingFrom;
            }
            set
            {
                SetField(ref _playingFrom, value);
                if(_playingFrom is Album)
                {
                    PlayingFromModelType = ModelTypeEnum.Album;
                }
                else if(_playingFrom is Artist) 
                {
                    PlayingFromModelType = ModelTypeEnum.Artist;
                }
                else if (_playingFrom is Playlist)
                {
                    PlayingFromModelType = ModelTypeEnum.Playlist;
                }
                else if (_playingFrom is Tag)
                {
                    PlayingFromModelType = ModelTypeEnum.Tag;
                }
                else
                {
                    PlayingFromModelType = ModelTypeEnum.UNKNOWN;
                }
            }
        }

        public string PlayingFromName
        {
            get { return _playingFromName; }
            set
            {
                SetField(ref _playingFromName, value);
            }
        }

        public int PlayingFromId
        {
            get => _playingFromId;
            set
            {
                SetField(ref _playingFromId, value);
            }
        }

        public string Cover
        {
            get { return _cover; }
            set
            {
                SetField(ref _cover, value);
            }
        }

        public Track PlayingTrack
        {
            get => _playingTrack;
            set
            {
                SetField(ref _playingTrack, value);
            }
        }

        private QueueTrack _playingQueueTrack;
        [NotMapped]
        public QueueTrack PlayingQueueTrack
        {
            get
            {
                if(_playingQueueTrack == null && Tracks.IsNullOrEmpty())
                {
                    return new(new Track(), 0);
                }
                _playingQueueTrack ??= Tracks.First(t => t.Track.Id == PlayingTrack.Id);
                return _playingQueueTrack;
            }
            set
            {
                SetField(ref _playingQueueTrack, value);
                if (value.IsNotNull())
                    PlayingTrack = value.Track;
                else
                    PlayingTrack = null;
            }
        }

        public ObservableCollection<QueueTrack> Tracks
        {
            get
            {
                if(_tracks.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _tracks = [..context.QueueTracks.Where(qt => qt.QueueId == Id)
                                .Include(qt => qt.Track)];
                }
                return _tracks;
            }
            set
            {
                SetField(ref _tracks, value);
            }
        }

        public override int Length 
        {
            get
            {
                if(_length == 0)
                {
                    _length = Tracks.GetTracksTotalLength();
                    OnPropertyChanged(nameof(Duration));
                }
                return _length;
            }
            set => SetField(ref _length, value);
        }

        [Required]
        public int PlayingTrackId { get; set; }

        [EnumDataType(typeof(ModelTypeEnum))]
        public ModelTypeEnum PlayingFromModelType
        {
            get => _playingFromModelType;
            set => SetField(ref _playingFromModelType, value);
        }

        public Queue(int id, bool isShuffled, bool isOnRepeat, int length, int playingTrackId, string from, string cover)
        {
            Id = id;
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            PlayingTrackId = playingTrackId;
            PlayingFromName = from;
            Cover = cover;
        }

        public Queue(bool isShuffled, bool isOnRepeat, int length, int playingTrackId, string from, string cover)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            PlayingTrackId = playingTrackId;
            PlayingFromName = from;
            Cover = cover;
        }

        public Queue(bool isShuffled, bool isOnRepeat, int length, Track playingTrack)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            PlayingTrack = playingTrack;
        }

        public Queue()
        {

        }

        public static void Insert(Queue queue)
        {
            using DatabaseContext context = new();
            // Want only 1 queue saved in the database at all time
            context.QueueTracks.ExecuteDelete();
            context.Queues.ExecuteDelete();
            context.SaveChanges();

            // save a the tracks in a variable for saving them later
            ObservableCollection<QueueTrack> queueTracks = new(queue.Tracks);
            queue.Tracks = null;
            queue.PlayingTrackId = queue.PlayingQueueTrack.Track.Id;
            queue.PlayingTrack = null; // avoid EF trying to insert a relation of a track, or a conflict with already tracked relation
            context.Queues.Add(queue);
            context.SaveChanges(); // get and Id for the queue

            List<QueueTrack> tracks = [];
            foreach (QueueTrack track in queueTracks)
            {
                // avoid EF trying to insert a relation of a track, or a conflict with already tracked relation
                // by creating a new object with only the Ids
                tracks.Add(new(queue.Id, track.TrackId, track.TrackIndex));
            }

            context.QueueTracks.AddRange(tracks);
            context.SaveChanges();
        }

        public static Queue Get()
        {
            using DatabaseContext context = new();
            Queue queue = context.Queues.FirstOrDefault();

            if(queue.IsNotNull())
                queue.PlayingTrack = context.Tracks.Find(queue.PlayingTrackId);
            return queue;
        }

        public static async Task Delete(Queue queue)
        {
            using DatabaseContext context = new();
            context.Queues.Remove(queue);
            await context.SaveChangesAsync();
        }
    }
}
