using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlay.Language;
using System.Runtime.InteropServices;
using System.Data;
using System.Text;
using System.Windows.Markup.Localizer;
using System.Net;
using System.Drawing;
using System.Globalization;

namespace MusicFilesProcessor.Helpers
{
    public static class TagHelper
    {
        //    "arranger",
        //    "associatedperformer",
        //    "bass",
        //    "composer",
        //    "composerlyricist",
        //    "copyright",
        //    "drums",
        //    "engineer",
        //    "featuredartist",
        //    "http",
        //    "lyricist",
        //    "mainartist",
        //    "mixer",
        //    "producer",
        //    "programming",
        //    "publisher",
        //    "studiopersonnel",
        //    "vocals",

        //    "interprete",
        //    "label",
        //    "purchased",
        //    "visit",


        //public static string DateTimeToString(this DateTime dateTime)
        //{
        //    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        //}

        //public static string DateTimeToDateOnlyString(this DateTime dateTime)
        //{
        //    return dateTime.ToString("yyyy-MM-dd");
        //}

        //public static string FormatStringToTime(this string time)
        //{
        //    if (time.Split(":").Length == 2)
        //    {
        //        return "00:" + time;
        //    }
        //    else return time;
        //}

        /// <summary>
        /// take a timespan and return its string representation
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns> hh:mm:ss or mm:ss if hours are not needed </returns>
        public static string ToFormattedString(this TimeSpan timeSpan, bool written = false)
        {
            if (written)
            {
                string output = "";
                if(timeSpan.Days > 0)
                {
                    if(timeSpan.Days > 1)
                    {
                        output += $"{timeSpan.Days} {Resources.Days} ";
                    }
                    else
                    {
                        output += $"{timeSpan.Days} {Resources.Day} ";
                    }
                }
                if(timeSpan.Hours > 0)
                {
                    if(timeSpan.Hours > 1)
                    {
                        output += $"{timeSpan.Hours} {Resources.Hours} ";
                    }
                    else
                    {
                        output += $"{timeSpan.Hours} {Resources.Hour} ";
                    }
                }
                if(timeSpan.Minutes > 1)
                    output += $"{timeSpan.Minutes} {Resources.Minutes} ";
                else
                    output += $"{timeSpan.Minutes} {Resources.Minute} ";
                if(timeSpan.Seconds > 1)
                    output += $"{timeSpan.Seconds} {Resources.Seconds} ";
                else
                    output += $"{timeSpan.Seconds} {Resources.Second} ";
                return output;
            }
            else
            {
                if(timeSpan.Days > 0)
                    return timeSpan.ToString(@"d\.hh\:mm\:ss");
                else if(timeSpan.Hours > 0)
                    return timeSpan.ToString(@"hh\:mm\:ss");
                else return timeSpan.ToString(@"mm\:ss");
            }
        }

        /// <summary>
        /// Take a string representation of a time (hh:mm:ss or mm:ss) and get the number of seconds corresponding to it
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ToSeconds(this string time)
        {
            if(string.IsNullOrEmpty(time))
                return 0;

            int length = 0;
            List<string> values = time.Split(":").ToList();
            for (int i = 1; i == values.Count; i++)
            {
                int value = 0;
                bool result = int.TryParse(values[values.Count - i], out value);
                if (!result)
                    return 0;

                if(i == 1) // Seconds
                {
                    length += value;
                }
                else // Minutes, hours
                {
                    length += value * i * 60;
                }
            }
            return length;
        }

        public static string GetTotalLength<T>(this IEnumerable<T> models, out int length) where T : TrackModel
        {
            length = 0;
            var asspan = CollectionsMarshal.AsSpan<T>(models.ToList());
            for (var i = 0; i < asspan.Length; i ++)
            {
                if (asspan[i] != null)
                    length += asspan[i].Length;
            }

            return TimeSpan.FromMilliseconds(length).ToFormattedString();
        }

        public static async Task<List<TrackModel>> GetAllArtistTracks(this ArtistModel artist)
        {
            List<TrackModel> tracks = new List<TrackModel>();
            tracks.AddRange(await DataAccess.Connection.GetTracksFromArtist(artist.Id));
            List<AlbumModel> albums = await DataAccess.Connection.GetAlbumsFromArtist(artist.Id);
            foreach (AlbumModel album in albums)
            {
                tracks.AddRange(await DataAccess.Connection.GetTracksFromAlbum(album.Id));
            }

            return tracks.DistinctBy(t => t.Id).ToList();
        }

        public static string FormatTag(this string tag)
        {
            if (tag != null)
            {
                tag = tag.Replace("\n", ",");
                tag = tag.Replace("\r", ",");
                return tag.Split(',')[0];
            }
            return null;
        }

        public static Dictionary<string, List<bool>> ReadDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) return new();

            Dictionary<string, List<bool>> output = new();

            string[] categories = description.Split(':');

            foreach (string category in categories)
            {
                string[] keyValues = category.Split("\n");

                if(keyValues.Length > 1)
                {
                    bool mainArtist = false;
                    bool composer = false;
                    bool performer = false;
                    bool featured = false;
                    bool lyricist = false;

                    for (int i = 1; i < keyValues.Length; i++)
                    {
                        string[] values = keyValues[i].ToLower().Split(",");
                        for (int y = 0; y < values.Length; y++)
                        {
                            if (values[y].Contains("associatedperformer") || values[y].Contains("featuredartist"))
                            {
                                performer = true;

                                if (values[y].Contains("featuredartist"))
                                {
                                    featured = true;
                                }
                            }

                            if (values[y].Contains("composerlyricist") || values[y].Contains("lyricist"))
                            {
                                lyricist = true;
                            }
                        }

                        if (values[0].Contains('&'))
                        {
                            string[] artists = values[0].Split("&");

                            foreach (string artist in artists)
                            {
                                if (!string.IsNullOrEmpty(artist) && !artist.Contains("...") && !artist.Contains("label"))
                                {
                                    output.TryAdd(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(artist.Trim()), new() {mainArtist, composer, performer, featured, lyricist });
                                }
                            }
                        }
                        else if (!values[0].Contains("...") && !values[0].Contains("label"))
                        {
                            output.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(values[0].Trim()), new() {mainArtist, composer, performer, featured, lyricist });
                        }
                    }

                }
            }

            return output;
        }

        /// <summary>
        /// Get the key of the 1st value that has its boolean to true at the specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="boolIndex"></param>
        /// <returns> </returns>
        public static string GetArtist(this Dictionary<string, List<bool>> data, int boolIndex)
        {
            foreach (KeyValuePair<string, List<bool>> kvp in data)
            {
                if (kvp.Value[boolIndex])
                {
                    return kvp.Key;
                }
            }
            return null;
        }
    }
}

