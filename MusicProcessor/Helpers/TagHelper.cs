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

        public static string RemoveDiacritics(this string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static void AddArtistsToDic(List<string> artistsToAdd, Dictionary<string, List<string>> artists, string role)
        {
            for (int i = 0; i < artistsToAdd.Count; i++) 
            {
                string artist = artistsToAdd.ElementAt(i).Trim();
                if (string.IsNullOrEmpty(artist))
                    continue;

                artist = artist.RemoveDiacritics();

                if(artist.Contains('&'))
                {
                    string[] splitArtists = artist.Split('&');
                    artistsToAdd.AddRange(splitArtists);
                    AddArtistsToDic(artistsToAdd.GetRange(i + 1, artistsToAdd.Count), artists, role);
                    return;
                }

                if (artists.ContainsKey(artist))
                {
                    if (!artists[artist].Contains(role))
                    {
                        artists[artist].Add(role);
                    }
                }
                else
                {
                    artists.Add(artist, new() { role });
                }
            }
            return;
        }

        public static void ReadDescription(string description, Dictionary<string, List<string>> artists)
        {
            if (string.IsNullOrWhiteSpace(description)) return;

            string[] categories = description.Split(':');

            List<string> rolesToIgnore = new()
            {
                "main",
                "primary",
                "album",
                "http",
                "label",
                "visit",
                "purchased",
                "copyright",
            };

            foreach (string category in categories)
            {
                string[] keyValues = category.Split("\n");

                if(keyValues.Length < 2) continue;

                // start at 1 because most of the time the description has a first _line that doesn't contain any information
                for (int i = 1; i < keyValues.Length; i++)
                {
                    // format => artist name, role 1, role 2...
                    string[] values = keyValues[i].Split(",");
                    List<string> foundArtists = new();

                    // there may be multiple artists separated by a '&'
                    string[] splitArtists = values[0].Split("&");
                    foreach (string artist in splitArtists)
                    {
                        if (!string.IsNullOrEmpty(artist) && !artist.Contains("...") && !artist.Contains("label"))
                        {
                            foundArtists.Add(artist);
                        }
                    }

                    // find the roles
                    for (int y = 0; y < values.Length; y++)
                    {
                        string role = values[y];

                        string roleToLower = role.ToLower();
                        if (rolesToIgnore.Any(r => rolesToIgnore.Contains(r)) || roleToLower == "engineer")
                            continue;

                        if (roleToLower.Contains("performer"))
                        {
                            role = "Performer";
                        }

                        if(roleToLower.Contains("featured"))
                        {
                            role = "Featured";
                        }
                        else if (roleToLower.Contains("lyricist"))
                        {
                            role = "Lyricist";
                        }

                        AddArtistsToDic(foundArtists, artists, role);
                    }
                }
            }
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

