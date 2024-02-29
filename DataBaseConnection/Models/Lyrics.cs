using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("Lyrics")]
    public class Lyrics : BaseModel
    {
        private string _lyrics = string.Empty;
        public string _url = string.Empty;
        private ObservableCollection<TimedLyricsLine> _timedLines = new();
        private string _webSiteSource = string.Empty;
        public bool _isSaved = false;

        [Column("Lyrics")]
        public string LyricsText
        {
            get => _lyrics;
            set => SetField(ref _lyrics, value);
        }

        [System.ComponentModel.DataAnnotations.Schema.Index(IsUnique=true)]
        public string Url
        {
            get => _url;
            set => SetField(ref _url, value);
        }

        public ObservableCollection<TimedLyricsLine> TimedLines
        {
            get => _timedLines;
            set => SetField(ref _timedLines, value);
        }

        [NotMapped]
        public string WebSiteSource
        {
            get => _webSiteSource;
            set => SetField(ref _webSiteSource, value);
        }

        [NotMapped]
        public bool IsSaved
        {
            get => _isSaved;
            set => SetField(ref _isSaved, value);
        }

        [NotMapped]
        public bool HasTimedLyrics => _timedLines.Count > 0;

        public Lyrics()
        {

        }

        public static async Task Insert(Lyrics lyrics)
        {
            using DatabaseContext context = new();
            context.Lyrics.Add(lyrics);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Insert all the timed lyrics
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns></returns>
        public static async Task InsertTimedLyrics(Lyrics lyrics)
        {
            if (lyrics.HasTimedLyrics)
            {
                await Task.Run(() =>
                {
                    foreach (TimedLyricsLine timedLine in lyrics.TimedLines)
                    {
                        timedLine.LyricsId = lyrics.Id;
                        //timedLine.Id = DataAccess.Connection.InsertOne(timedLine);
                    }
                });
            }
        }

        /// <summary>
        /// Try to get lyrics with this url from the database
        /// </summary>
        /// <param name="url">The url of the lyrics</param>
        /// <returns>The Lyrics if found, otherwise null</returns>
        public static Task<Lyrics?> TryToGetByUrl(string url)
        {
            using DatabaseContext context = new();
            return context.Lyrics.Where(p => p.Url.ToLower() == url.ToLower()).FirstOrDefaultAsync();
        }

        public static async Task Delete(Lyrics lyrics)
        {
            //if(lyrics.HasTimedLyrics)
            //{
            //    await DataAccess.Connection.DeleteAllWhere<TimedLyricsLine>(new Where(DataBaseColumns.LyricsId, lyrics.Id.ToString()));
            //}

            //await DataAccess.Connection.DeleteOneAsync(lyrics);
        }
    }
}
