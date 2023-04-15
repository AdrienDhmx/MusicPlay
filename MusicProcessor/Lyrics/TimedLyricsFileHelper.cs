using MusicFilesProcessor.Helpers;
using MusicFilesProcessor.Lyrics.Helper;
using MusicFilesProcessor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicFilesProcessor.Lyrics
{
    public class TimedLyricsFileHelper : ILyricsFileHelper
    {
        public string GetFilePath(string fileName)
        {
            return Path.Combine(DirectoryHelper.TimedLyricsDirectory, fileName);
        }

        public LyricsModel GetLyrics(string filePath)
        {
            LyricsModel lyricsModel = new LyricsModel();
            lyricsModel.IsTimed = true;
            lyricsModel.IsSaved = true;

            List<string> lines = File.ReadAllLines(filePath).ToList();
            (string website, string url, bool isFromUser) = lines.GetHeader();
            lyricsModel.IsFromUser = isFromUser;
            lyricsModel.WebSiteSource = website;
            lyricsModel.URL = url;
            lyricsModel.FileName = Path.GetFileName(filePath);

            // remove the header
            lines = lines.RemoveHeader();

            // Create the list of timed lyrics model
            lyricsModel.TimedLyrics = lines.ConvertTimedLyrics();
            return lyricsModel;
        }

        public LyricsModel GetLyrics(LyricsModel lyrics)
        {
            lyrics.IsTimed = true;
            lyrics.IsSaved = true;

            // Get and read the file
            string filePath = GetFilePath(lyrics.FileName);
            List<string> lines = File.ReadAllLines(filePath).ToList();

            // remove the header and the footer data
            lines = lines.RemoveHeader();

            // Create the list of timed lyrics model
            lyrics.TimedLyrics = lines.ConvertTimedLyrics();
            return lyrics;
        }

        public void SaveLyrics(string filePath, LyricsModel lyrics)
        {
            List<string> lines = lyrics.TimedLyrics.ConvertToString();
            DirectoryHelper.CheckDirectory(DirectoryHelper.TimedLyricsDirectory);
            File.WriteAllLines(filePath, lines.WriteHeader(lyrics.WebSiteSource, lyrics.URL, lyrics.IsFromUser));
        }
    }
}
