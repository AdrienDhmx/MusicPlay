using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("PlayHistory")]
    public class PlayHistory : BaseModel
    {
        private DateTime _date = DateTime.Today;
        private int _playTime = 0;
        private ObservableCollection<PlayHistoryEntry> _entries = [];


        /// <summary>
        /// The total playback time in milliseconds
        /// </summary>
        public int PlayTime
        {
            get 
            { 
                if(_playTime == 0 && Entries.Count > 0)
                {
                    foreach (PlayHistoryEntry entry in Entries)
                    {
                        _playTime += entry.PlayedLength;
                    }
                }
                return _playTime; 
            }
            set
            {
                _playTime = value;
                OnPropertyChanged(nameof(PlayTime));
            }
        }

        /// <summary>
        /// The date of the record
        /// </summary>
        [Required]
        [Column(TypeName = "INTEGER")]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public ObservableCollection<PlayHistoryEntry> Entries
        {
            get => _entries;
            set => SetField(ref _entries, value);
        }

        /// <summary>
        /// The total number of playbacks
        /// </summary>
        [NotMapped]
        public int PlayCount => _entries.IsNotNull() ? _entries.Count : 0;

        public PlayHistory()
        {

        }

        public PlayHistory(DateTime date, int listenTime)
        {
            Date = date;
            PlayTime = listenTime;
        }

        /// <summary>
        /// Get the existing play history for today or create it
        /// </summary>
        /// <returns></returns>
        public static async Task<PlayHistory?> GetTodayHistory()
        {
            using DatabaseContext context = new();
            PlayHistory? mostRecent = await context.PlayHistories.OrderByDescending(x => x.Date).FirstOrDefaultAsync();

            if(mostRecent?.Date == DateTime.Now.Date)
            {
                return mostRecent;
            }

            // create one for today
            PlayHistory todayHistory = new();
            context.PlayHistories.Add(todayHistory);
            await context.SaveChangesAsync();
            return todayHistory;
        }

        public static List<PlayHistory> GetRecentHistory(int top)
        {
            using DatabaseContext context = new();
            return context.PlayHistories.OrderByDescending(x => x.Date).Take(top).ToList();
        }

        /// <summary>
        /// Get the history between the 2 dates excluded
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<PlayHistory> GetHistoryBetween(DateTime start, DateTime end)
        {
            if(start > end)
            {
                throw new ArgumentException("Start date cannot be greater than end date.");
            }

            using DatabaseContext context = new();
            return context.PlayHistories.Where(pl => pl.Date > start && pl.Date < end)
                    .Include(h => h.Entries)
                    .ToList();
        }
    }
}
