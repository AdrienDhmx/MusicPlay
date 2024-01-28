using MusicFilesProcessor.Helpers;
using MusicPlay.Database.Helpers;
using System.IO;

namespace MusicFilesProcessor.Lyrics.Helper
{
    public class LyricsFileHelper : ILyricsFileHelper
    {
        public string GetFilePath(string fileName)
        {
            return Path.Combine(DirectoryHelper.LyricsDirectory, fileName);
        }

        public MusicPlay.Database.Models.Lyrics GetLyrics(string filePath)
        {
            MusicPlay.Database.Models.Lyrics lyricsModel = new()
            {
                IsSaved = true
            };
            string l = File.ReadAllText(filePath);
            lyricsModel.LyricsText = l.Replace("\r", string.Empty);
            (string website, string url, bool isFromUser) = lyricsModel.LyricsText.GetHeader();
            lyricsModel.WebSiteSource = website;
            lyricsModel.Url = url;
            lyricsModel.LyricsText = lyricsModel.LyricsText.RemoveHeader();
            return lyricsModel;
        }

        public MusicPlay.Database.Models.Lyrics GetLyrics(MusicPlay.Database.Models.Lyrics lyrics, string fileName)
        {
            try
            {
                lyrics.LyricsText = File.ReadAllText(GetFilePath(fileName)).RemoveHeader();
            }
            catch (Exception)
            {
                return null;
            }
            return lyrics;
        }

        public void SaveLyrics(string filePath, MusicPlay.Database.Models.Lyrics lyrics)
        {
            File.WriteAllLines(filePath, lyrics.LyricsText.WriteHeader(lyrics.WebSiteSource, lyrics.Url));
        }
    }
}
