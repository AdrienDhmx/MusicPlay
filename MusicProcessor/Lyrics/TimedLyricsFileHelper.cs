using MusicFilesProcessor.Helpers;
using MusicFilesProcessor.Lyrics.Helper;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
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

        public MusicPlay.Database.Models.Lyrics GetLyrics(string filePath)
        {
            MusicPlay.Database.Models.Lyrics lyricsModel = new MusicPlay.Database.Models.Lyrics();
            lyricsModel.IsSaved = false;

            List<string> lines = File.ReadAllLines(filePath).ToList();
            (string website, string url) = lines.GetHeader();
            lyricsModel.WebsiteSource = website;
            lyricsModel.Url = url;

            // remove the header
            lines = lines.RemoveHeader();

            // Create the list of timed lyrics model
            lyricsModel.TimedLines = new(lines.ConvertTimedLyrics());
            return lyricsModel;
        }

        public MusicPlay.Database.Models.Lyrics GetLyrics(MusicPlay.Database.Models.Lyrics lyrics, string fileName)
        {
            lyrics.IsSaved = true;

            // Get and read the file
            string filePath = GetFilePath(fileName);
            List<string> lines = File.ReadAllLines(filePath).ToList();

            // remove the header and the footer data
            lines = lines.RemoveHeader();

            // Create the list of timed lyrics model
            lyrics.TimedLines = new(lines.ConvertTimedLyrics());
            return lyrics;
        }

        public void SaveLyrics(string filePath, MusicPlay.Database.Models.Lyrics lyrics)
        {
            List<string> lines = lyrics.TimedLines.Select(l => l.Line).ToList();
            DirectoryHelper.CheckDirectory(DirectoryHelper.TimedLyricsDirectory);
            File.WriteAllLines(filePath, lines.WriteHeader(lyrics.WebsiteSource, lyrics.Url));
        }
    }
}
