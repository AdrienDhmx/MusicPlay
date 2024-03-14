using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
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
            get
            {
                if(_timedLines.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _timedLines = [..context.TimedLyricsLine.Where(tl => tl.LyricsId == Id)];
                }
                return _timedLines;
            }
            set => SetField(ref _timedLines, value);
        }

        public string WebsiteSource
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

        /// <summary>
        /// Insert the lyrics to the database, but don't link it the any track !
        /// </summary>
        /// <param name="lyrics">Thye lyrics to insert</param>
        public static void Insert(Lyrics lyrics)
        {
            using DatabaseContext context = new();
            context.Lyrics.Add(lyrics);
            context.SaveChanges();
        }

        /// <summary>
        /// Insert the lyrics to the database AND link it to the track
        /// </summary>
        /// <param name="lyrics">Thye lyrics to insert</param>
        /// <param name="track">The track to link to</param>
        public static void Insert(Lyrics lyrics, Track track)
        {
            using DatabaseContext context = new();
            context.Lyrics.Add(lyrics);
            context.SaveChanges();

            // get the track from the db with no relation to avoid tracking issues
            Track trackToLinkTo = context.Tracks.Find(track.Id);
            trackToLinkTo.Lyrics = lyrics;
            trackToLinkTo.LyricsId = lyrics.Id;
            context.SaveChanges();

            // update the passed track for UI or further processing with it 
            track.Lyrics = lyrics;
            track.LyricsId = lyrics.Id;
        }

        /// <summary>
        /// Insert all the timed lyrics
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns></returns>
        public static void InsertUpdateTimedLyrics(Lyrics lyrics)
        {
            if (!lyrics.HasTimedLyrics)
            {
                return;
            }

            using DatabaseContext context = new();

            foreach (TimedLyricsLine timedLine in lyrics.TimedLines)
            {
                timedLine.LyricsId = lyrics.Id;
                if(timedLine.Id == 0)
                {
                    context.TimedLyricsLine.Add(timedLine);
                }
                else
                {
                    context.Update(timedLine);
                }
            }

            context.SaveChanges(true);
        }

        public static void Update(Lyrics lyrics)
        {
            using DatabaseContext context = new();
            Lyrics savedLyrics = context.Lyrics.Find(lyrics.Id);
            savedLyrics.LyricsText = lyrics.LyricsText;
            context.SaveChanges(true);
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
