using FilesProcessor.Helpers;
using MessageControl;
using MusicFilesProcessor.Enums;
using MusicFilesProcessor.Helpers;
using MusicFilesProcessor.Lyrics.Helper;
using MusicFilesProcessor.Models;
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

        public async Task<LyricsModel> GetLyrics(string title, string artist, string audioFilePath, bool acceptTimedLyrics = true)
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
                        return CreateLyricsModel("", "", "", GetFileName(title, artist), true, false);
                    }
                }
                else
                {
                    return CreateLyricsModel(lyrics, "", "", GetFileName(title, artist), true, false);
                }
            }
        }

        private async Task<LyricsModel> GetLyricsOnTheWeb(string title, string artist)
        {
            if (ConnectivityHelper.CheckInternetAccess())
            {
                string url = _lyricsHelper.GetUrl(title, artist);

                HttpResponseMessage response = await _connectivityHelper.SendRequestAsync(url);
                string lyrics = _lyricsHelper.ReadWebPage(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(lyrics))
                {
                    CurrentURL = url;
                    LyricsModel lyricsModel = CreateLyricsModel(lyrics, url, CurrentWebSite, GetFileName(title, artist));
                    return lyricsModel;
                }
                else
                {
                    // if the resource was not found don't trigger a msg notification
                    if(response.StatusCode != System.Net.HttpStatusCode.NotFound)
                        _connectivityHelper.HandleHttpError(response.StatusCode);

                    return CreateLyricsModel("", "", "", GetFileName(title, artist), true, false);
                }
            }
            else
            {
                ConnectivityHelper.PublishNoConnectionMsg();
                return CreateLyricsModel("", "", "", GetFileName(title, artist), true, false);
            }
        }


        private LyricsModel CreateLyricsModel(string lyrics, string url, string website, string fileName = "", bool isFromUser = false, bool isTimed = false)
        {
            LyricsModel lyricsModel = new LyricsModel()
            {
                Lyrics = lyrics,
                FileName = fileName,
                IsFromUser = isFromUser,
                IsTimed = isTimed,
                URL = url,
                WebSiteSource = website
            };
            return lyricsModel;
        }

        public void SaveLyrics(LyricsModel lyrics, bool isTimed =false)
        {
            ChangeLyricsFileHelper(isTimed);
            _lyricsFileHelper.SaveLyrics(_lyricsFileHelper.GetFilePath(lyrics.FileName), lyrics);
        }

        public List<TimedLyricsLineModel> ConvertToTimedLyricsModel(string lyrics, bool create = false)
        {
            List<string> lines = lyrics.Split("\n").ToList();
            if (create)
            {
                List<TimedLyricsLineModel> timedLyricsLines = new List<TimedLyricsLineModel>();
                int count = 0;
                for (int i = 0; i < lines.Count; i++)
                {
                    // remove blank lines and info lines (AZLyrics adds [Language])
                    if (string.IsNullOrWhiteSpace(lines[i]) || Regex.IsMatch(lines[i], @"\[(.*?)\]"))
                        continue;
                    TimedLyricsLineModel timedLyrics = new()
                    {
                        index = count, // Not i since we ignore empty _line
                        LengthInMilliseconds = 0,
                        Time = TimeSpan.FromMilliseconds(0).ToFormattedString(),
                        Lyrics = lines[i],
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

        public LyricsModel GetLyrics(LyricsModel lyrics, bool isTimed = false)
        {
            ChangeLyricsFileHelper(isTimed);
            return _lyricsFileHelper.GetLyrics(lyrics);
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
