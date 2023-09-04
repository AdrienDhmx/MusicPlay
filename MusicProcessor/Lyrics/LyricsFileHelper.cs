using MusicFilesProcessor.Helpers;
using MusicFilesProcessor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFilesProcessor.Lyrics.Helper
{
    public class LyricsFileHelper : ILyricsFileHelper
    {
        public string GetFilePath(string fileName)
        {
            return Path.Combine(DirectoryHelper.LyricsDirectory, fileName);
        }

        public LyricsModel GetLyrics(string filePath)
        {
            LyricsModel lyricsModel = new LyricsModel();
            lyricsModel.IsTimed = false;
            lyricsModel.IsSaved = true;
            lyricsModel.FileName = Path.GetFileName(filePath);
            string l = File.ReadAllText(filePath);
            lyricsModel.Lyrics = l.Replace("\r", string.Empty);
            (string website, string url, bool isFromUser) = lyricsModel.Lyrics.GetHeader();
            lyricsModel.IsFromUser = isFromUser;
            lyricsModel.WebSiteSource = website;
            lyricsModel.URL = url;
            lyricsModel.Lyrics = lyricsModel.Lyrics.RemoveHeader();
            return lyricsModel;
        }

        public LyricsModel GetLyrics(LyricsModel lyrics)
        {
            try
            {
                lyrics.Lyrics = File.ReadAllText(GetFilePath(lyrics.FileName)).RemoveHeader();
            }
            catch (Exception)
            {
                return null;
            }
            return lyrics;
        }

        public void SaveLyrics(string filePath, LyricsModel lyrics)
        {
            File.WriteAllLines(filePath, lyrics.Lyrics.WriteHeader(lyrics.WebSiteSource, lyrics.URL, lyrics.IsFromUser));
        }
    }
}
