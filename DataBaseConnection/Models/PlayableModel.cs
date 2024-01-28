using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessageControl;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;

namespace MusicPlay.Database.Models
{
    public abstract class PlayableModel : BaseModel
    {
        private DateTime _updateDate = DateTime.MinValue;
        internal int _length = 0;
        private int _playCount = 0;
        private DateTime _lastPlayed = DateTime.MinValue;

        [Required]
        [Column(TypeName = "INTEGER")]
        public DateTime CreationDate { get; init; }

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
            return context.Set<T>().OrderByDescending(e => e.LastPlayed).Take(top).ToList();
        }

        public static IQueryable<T> GetMostPlayed<T>(int top) where T : PlayableModel
        {
            using DatabaseContext context = new();
            return context.Set<T>().OrderByDescending(e => e.PlayCount).Take(top);
        }

        public static async Task UpdatePlayCount<T>(T entity) where T : PlayableModel
        {
            using DatabaseContext context = new();
            T? dbEntity = await context.Set<T>().FindAsync(entity.Id);

            if(dbEntity.IsNotNull())
            {
                dbEntity!.PlayCount += 1;
                dbEntity.LastPlayed = DateTime.Now;
                await context.SaveChangesAsync();
            }
            else
            {
                $"The entity of type {typeof(T).Name} was not find in the database !".CreateErrorMessage().Publish();
            }
        }
    }
}
