using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessageControl;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;

namespace MusicPlay.Database.Models
{
    public class PlayableModel : BaseModel
    {
        private DateTime _updateDate = DateTime.MinValue;
        internal int _length = 0;
        private int _playCount = 0;
        private DateTime _lastPlayed = DateTime.MinValue;
        private DateTime _creationDate;

        [Required]
        [Column(TypeName = "INTEGER")]
        public DateTime CreationDate
        {
            get
            {
                if(_creationDate.IsNull())
                {
                    _creationDate = DateTime.Now;
                }
                return _creationDate;
            }
            set => SetField(ref _creationDate, value);
        }

        [Column(TypeName = "INTEGER")]
        public DateTime UpdateDate
        {
            get => _updateDate;
            set => SetField(ref _updateDate, value);
        }

        /// <summary>
        /// The length (Duration) of the entity (artist, album, track, playlist, tag) in milliseconds
        /// </summary>
        [NotMapped]
        public virtual int Length
        {
            get => _length;
            set
            {
                SetField(ref _length, value);
                OnPropertyChanged(nameof(Duration));
            }
        }

        /// <summary>
        /// The number of time the entity has been played
        /// </summary>
        [Required]
        public int PlayCount
        {
            get => _playCount;
            set => SetField(ref _playCount, value);
        }

        /// <summary>
        /// The last date at which the entity has been played
        /// </summary>
        [Column(TypeName = "INTEGER")]
        public DateTime LastPlayed
        {
            get => _lastPlayed;
            set => SetField(ref _lastPlayed, value);
        }

        /// <summary>
        /// The formatted string (hh:mm:ss) of the duration of the entity (artist, album, track, playlist, tag).
        /// </summary>
        [NotMapped]
        public string Duration => TimeSpan.FromMilliseconds(Length).ToShortString();

        /// <summary>
        /// The formatted string (x days y hours z minutes k seconds) of the duration of the entity (artist, album, track, playlist, tag).
        /// </summary>
        [NotMapped]
        public string WrittenDuration => TimeSpan.FromMilliseconds(Length).ToFullString();

        protected PlayableModel(DateTime creationDate, DateTime updateDate)
        {
            CreationDate = creationDate;
            UpdateDate = updateDate;
        }

        public PlayableModel(PlayableModel playableModel)
        {
            CreationDate  = playableModel.CreationDate;
            UpdateDate = playableModel.UpdateDate;
            PlayCount = playableModel.PlayCount;
            LastPlayed = playableModel.LastPlayed;
        }

        public PlayableModel()
        {
            
        }

        internal static string TimestampToDateString(int timestamp)
        {
            if (timestamp == 0)
            {
                return "";
            }
            else if (timestamp < 3000) // only the year is known...
            {
                return timestamp.ToString();
            }
            else
            {
                return DateOnly.FromDayNumber(timestamp).ToShortDateString();
            }
        }

        public static List<T> GetLastPlayed<T>(int top) where T : PlayableModel
        {
            using DatabaseContext context = new();
            return [.. context.Set<T>().Where(e => e.LastPlayed != DateTime.MinValue).OrderByDescending(e => e.LastPlayed).Take(top)];
        }

        public static List<T> GetMostPlayed<T>(int top) where T : PlayableModel   
        {
            using DatabaseContext context = new();
            return [.. context.Set<T>().Where(e => e.PlayCount != 0).OrderByDescending(e => e.PlayCount).Take(top)];
        }

        public static void UpdatePlayCount<T>(T entity) where T : PlayableModel
        {
            entity!.PlayCount += 1;
            entity.LastPlayed = DateTime.Now;
        }
    }
}
