﻿using MusicPlay.Database.Models;
using System.Text.RegularExpressions;

namespace MusicFilesProcessor.Lyrics.Helper
{
    public static class LyricsFileHeaderHelper
    {
        private const string header = "#WebSiteSource#";
        private const string headerEnd = "#Lyrics#";
        private const string separator = "||";

        /// <summary>
        /// Gets the footer data
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns> the website, the URL, if the lyrics are from the user </returns>
        public static (string, string) GetHeader(this string lyrics)
        {
            List<string> lines = [.. lyrics.Trim().Split("\n")];
            return GetHeader(lines);
        }

        /// <summary>
        /// Gets the footer data
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns> the website, the URL, if the lyrics are from the user </returns>
        public static (string, string) GetHeader(this List<string> lyrics)
        {
            string website = "";
            string url = "";
            if (lyrics[0] == header) // Footer start
            {
                website = lyrics[2].Trim();
                url = lyrics[3].Trim();
            }
            return (website, url);
        }

        /// <summary>
        /// Write the footer data to the lyrics
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns> The list of lines representing the lyrics </returns>
        public static List<string> WriteHeader(this string lyrics, string website, string url, bool isFromUser = false)
        {
            List<string> lines = lyrics.Split("\n").ToList();
            return lines.WriteHeader(website, url, isFromUser);
        }

        /// <summary>
        /// Write the footer data to the lyrics
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns> The list of lines representing the lyrics </returns>
        public static List<string> WriteHeader(this List<string> lyrics, string website, string url, bool isFromUser = false)
        {
            lyrics.Insert(0,header);
            lyrics.Insert(1,isFromUser ? "1" : "0");
            lyrics.Insert(2,website);
            lyrics.Insert(3,url);
            lyrics.Insert(4, headerEnd);
            return lyrics;
        }

        /// <summary>
        /// Remove the Footer data
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns> the lyrics without the Header and Footer data </returns>
        public static string RemoveHeader(this string lyrics)
        {
            Match match = Regex.Match(lyrics, headerEnd);
            int index = match.Index;
            return lyrics.Substring(index + headerEnd.Length).Trim();
        }

        /// <summary>
        /// Remove the Header and the Footer data
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns> the lyrics without the Header and Footer data </returns>
        public static List<string> RemoveHeader(this List<string> lyrics)
        {
            int index = lyrics.IndexOf(headerEnd);
            lyrics.RemoveRange(0, index + 1);
            return lyrics;
        }

        /// <summary>
        /// For every string in lines create a TimedLyricsLine.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns> A list of TimedLyricsLine. </returns>
        public static List<TimedLyricsLine> ConvertTimedLyrics(this List<string> lines)
        {
            List<TimedLyricsLine> timedLyricsLines = new List<TimedLyricsLine>();
            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(separator); // Separate the Length in milliseconds and the lyrics
                if (parts.Length == 2 && parts is not null)
                {
                    int length = int.Parse(parts[0]);
                    TimedLyricsLine timedLyrics = new()
                    {
                        TimestampMs = length,
                        Line = parts[1]
                    };
                    timedLyricsLines.Add(timedLyrics);
                }
            }

            return timedLyricsLines;
        }

        /// <summary>
        /// For every timedLyricsLineModel in the list create its string representation:
        /// _line = LengthInMilliseconds + separator + Lyrics.
        /// </summary>
        /// <param name="timedLyricsLines"></param>
        /// <returns> A list of string representing each _line of the lyrics with its time in milliseconds. </returns>
        public static List<string> ConvertToString(this List<TimedLyricsLine> timedLyricsLines)
        {
            List<string> output = new List<string>();
            foreach (TimedLyricsLine t in timedLyricsLines)
            {
                string line = t.TimestampMs + separator + t.Line;
                output.Add(line);
            }
            return output;
        }
    }
}
