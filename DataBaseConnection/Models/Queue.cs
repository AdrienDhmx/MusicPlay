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
            get => _playingFrom;
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
                _playingQueueTrack ??= Tracks.First(t => t.Track.Id == PlayingTrack.Id);
                return _playingQueueTrack;
            }
            set
            {
                SetField(ref _playingQueueTrack, value);
                PlayingTrack = value.Track;
            }
        }

        public ObservableCollection<QueueTrack> Tracks
        {
            get => _tracks;
            set
            {
                SetField(ref _tracks, value);
            }
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

        public Queue(bool isShuffled, bool isOnRepeat, int length, Track playingTrack, List<Track> tracks)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            PlayingTrack = playingTrack;
        }

        public Queue()
        {

        }

        public static async Task Insert(Queue queue)
        {
            using DatabaseContext context = new();
            context.Queues.FromSqlRaw("DELETE FROM Queue"); // delete all other queues before inserting the new one
            context.Queues.Add(queue); // that way there are always only 1 queue in the db => the most recent one (currently being played)
            await context.SaveChangesAsync();
        }

        public static Queue? Get()
        {
            using DatabaseContext context = new();
            Queue queue = context.Queues
                    .Include(q => q.Tracks)
                    .FirstOrDefault();

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
