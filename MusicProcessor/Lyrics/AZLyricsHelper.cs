using FilesProcessor.Helpers;
using MessageControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MusicFilesProcessor.Lyrics
{
    public class AZLyricsHelper : ILyricsHelper
    {
        public string BaseURL => @"https://www.azlyrics.com/lyrics/";


        public string ReadWebPage(string page)
        {
            page = StripUntilLyrics(page);
            page = StripAfterLyrics(page);
            page = StripHtmlTags(page);
            page = page.Trim();
            page += "\n";
            return page;
        }

        private string FormatTextUrl(string url)
        {
            url = url.ToLower();
            url = url.Replace(" ", string.Empty);
            url = url.Replace("'", string.Empty);
            url = url.Replace("#", string.Empty);
            url = url.Replace("&", string.Empty);
            url = Regex.Replace(url, @"[^\P{L}a-zA-Z]+", string.Empty);
            url = Regex.Replace(url, @"\([^()]*\)", string.Empty);
            url = url.Trim();
            return url;
        }

        public string GetUrl(string title, string artist)
        {
            //https://www.azlyrics.com/lyrics/imaginedragons/believer.html
            title = FormatTextUrl(title);
            artist = FormatTextUrl(artist);
            return BaseURL + artist + "/" + title + ".html";
        }

        private string StripAfterLyrics(string source)
        {
            string pattern = "<!-- MxM banner -->";
            Match match = Regex.Match(source, pattern);
            int index = match.Index;
            return source.Substring(0, index);
        }

        private string StripHtmlTags(string source)
        {
            return Regex.Replace(source, "<.*?>|&.*?;", string.Empty);
        }

        private string StripUntilLyrics(string source)
        {
            string pattern = "<!-- Usage of azlyrics.com content by any third-party lyrics provider is prohibited by our licensing agreement. Sorry about that. -->";
            Match match = Regex.Match(source, pattern);
            int index = match.Index + pattern.Length;
            return source.Substring(index);
        }
    }
}
