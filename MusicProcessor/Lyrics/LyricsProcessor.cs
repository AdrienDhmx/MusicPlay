using MusicFilesProcessor.Enums;
using MusicFilesProcessor.Helpers;
using MusicFilesProcessor.Lyrics.Helper;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlay.Language;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;

namespace MusicFilesProcessor.Lyrics
{
    public class LyricsProcessor
    {
        private readonly ConnectivityHelper _connectivityHelper = ConnectivityHelper.Instance;

        private ILyricsHelper _lyricsHelper = new AZLyricsHelper();
        private ILyricsFileHelper _lyricsFileHelper = new TimedLyricsFileHelper();
        private bool _isTimed = true;

        public string CurrentWebSite { get; private set; } = "AZLyrics";
        public string CurrentURL { get; private  set; } = "";

        private static readonly LyricsProcessor _instance;
        public static LyricsProcessor Instance
        {
            get => _instance ?? new LyricsProcessor();
        }

        private LyricsProcessor()
        {
        }

        public void ChangeLyricsWebSource(LyricsWebsiteEnum webSite)
        {
            switch (webSite)
            {
                case LyricsWebsiteEnum.AZLyrics:
                    _lyricsHelper = new AZLyricsHelper();
                    CurrentWebSite = "AZLyrics";
                    break;
                default:
                    _lyricsHelper = new AZLyricsHelper();
                    CurrentWebSite = "AZLyrics";
                    break;
            }
        }

        /// <summary>
        /// Change the file helper based on the parameter if needed.
        /// </summary>
        /// <param name="istimed"></param>
        public void ChangeLyricsFileHelper(bool istimed = false)
        {
            if (istimed && !_isTimed)
            {
                _lyricsFileHelper = new TimedLyricsFileHelper();
                _isTimed = true;
            }
            else if(_isTimed && !istimed)
            {
                _lyricsFileHelper = new LyricsFileHelper();
                _isTimed = false;
            }
        }

        public bool TimedLyricsExists(string fileName)
        {
            ChangeLyricsFileHelper(true);
            return File.Exists(_lyricsFileHelper.GetFilePath(fileName));
        }

        public string GetFilePath(string title, string artist)
        {
            return _lyricsFileHelper.GetFilePath(GetFileName(title, artist));
        }

        public async Task<MusicPlay.Database.Models.Lyrics> GetLyrics(string title, string artist, string audioFilePath, bool acceptTimedLyrics = true)
        {
            CurrentURL = "";
            string filePath;

            if (acceptTimedLyrics)
            {
                // Try with timed lyrics first
                ChangeLyricsFileHelper(true); 

                if (TimedLyricsExists(GetFileName(title, artist)))
                {
                    filePath = GetFilePath(title, artist);
                    return _lyricsFileHelper.GetLyrics(filePath);
                }
            }

            // Then try with normal lyrics
            ChangeLyricsFileHelper();
            filePath = GetFilePath(title, artist);
            if (File.Exists(filePath))
            {
                return _lyricsFileHelper.GetLyrics(filePath);
            }
            else
            {
                // then try in the file as metadata
                string lyrics = GetLyricsInMetaData(audioFilePath);
                if (string.IsNullOrWhiteSpace(lyrics))
                {
                    // finally try finding the lyrics on the web
                    try
                    {
                        return await GetLyricsOnTheWeb(title, artist);
                    }
                    catch (Exception)
                    {
                        return CreateLyricsModel("", "", "");
                    }
                }
                else
                {
                    return CreateLyricsModel(lyrics, "", "");
                }
            }
        }

        private async Task<MusicPlay.Database.Models.Lyrics> GetLyricsOnTheWeb(string title, string artist)
        {
            if (ConnectivityHelper.Instance.CheckInternetAccess())
            {
                string url = _lyricsHelper.GetUrl(title, artist);
                string lyrics = await GetLyricsWithUrl(url);

                if(lyrics.IsNotNullOrWhiteSpace())
                {
                    return CreateLyricsModel(lyrics, url, CurrentWebSite);
                }

                return CreateLyricsModel("", "", "");
            }
            else
            {
                ConnectivityHelper.PublishNoConnectionMsg();
                return CreateLyricsModel("", "", "");
            }
        }

        public string GetPotentialUrl(string title, string artistName)
        {
            return _lyricsHelper.GetUrl(title, artistName);
        }

        public async Task<string> GetLyricsWithUrl(string url)
        {
            HttpResponseMessage response = await _connectivityHelper.GetAsync(url);
            string lyrics = _lyricsHelper.ReadWebPage(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(lyrics))
            {
                CurrentURL = url;
                return lyrics;
            }
            else
            {
                // if the resource was not found don't trigger a msg notification
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                    _connectivityHelper.HandleHttpError(response.StatusCode);

                return "";
            }
        }

        private MusicPlay.Database.Models.Lyrics CreateLyricsModel(string lyrics, string url, string website)
        {
            MusicPlay.Database.Models.Lyrics lyricsModel = new()
            {
                LyricsText = lyrics,
                Url = url,
                WebsiteSource = website
            };
            return lyricsModel;
        }

        public void SaveLyrics(MusicPlay.Database.Models.Lyrics lyrics, string fileName, bool isTimed =false)
        {
            ChangeLyricsFileHelper(isTimed);
            _lyricsFileHelper.SaveLyrics(_lyricsFileHelper.GetFilePath(fileName), lyrics);
        }

        public List<TimedLyricsLine> ConvertToTimedLyricsModel(string lyrics, bool create = false)
        {
            List<string> lines = lyrics.Split("\n").ToList();
            if (create)
            {
                List<TimedLyricsLine> timedLyricsLines = new List<TimedLyricsLine>();
                int count = 0;
                for (int i = 0; i < lines.Count; i++)
                {
                    // remove blank lines and info lines (AZLyrics adds [Language])
                    if (string.IsNullOrWhiteSpace(lines[i]) || Regex.IsMatch(lines[i], @"\[(.*?)\]"))
                        continue;
                    TimedLyricsLine timedLyrics = new()
                    {
                        TimestampMs = 0,
                        Line = lines[i],
                    };
                    count++;
                    timedLyricsLines.Add(timedLyrics);
                }
                return timedLyricsLines;
            }
            return lines.ConvertTimedLyrics();
        }

        public void DeleteTimedLyrics(string title, string artist)
        {
            string fileName = GetFileName(title, artist);

            string filePath = Path.Combine(DirectoryHelper.TimedLyricsDirectory, fileName);

            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public MusicPlay.Database.Models.Lyrics GetLyrics(MusicPlay.Database.Models.Lyrics lyrics, bool isTimed = false)
        {
            ChangeLyricsFileHelper(isTimed);
            return _lyricsFileHelper.GetLyrics(lyrics, "");
        }

        public string GetFileName(string title, string artist)
        {
            return title + "_" + artist + ".txt";
        }

        public string GetLyricsInMetaData(string audioFile)
        {
            using (TagLib.File file = TagLib.File.Create(audioFile))
            {
                return file.Tag.Lyrics;
            }
        }
    }
}
