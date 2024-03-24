using System.Runtime.InteropServices;
using System.Data;
using System.Text;
using System.Globalization;
using MusicPlay.Database.Models;
using MusicPlay.Database.Helpers;

namespace MusicFilesProcessor.Helpers
{
    public static class TagHelper
    {
        public static List<Track> GetAllArtistTracks(this Artist artist)
        {
            List<Track> tracks = Track.GetAllFromArtist(artist.Id);
            List<Album> albums = Album.GetAllFromArtist(artist.Id);
            foreach (Album album in albums)
            {
                tracks.AddRange(album.Tracks);
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
            foreach (string artistToAdd in artistsToAdd)
            {
                if (string.IsNullOrWhiteSpace(artistToAdd))
                    continue;

                string artist = artistToAdd?.Trim().RemoveDiacritics();
                string[] splitArtists = artist.Split('&', StringSplitOptions.TrimEntries);

                for (int i = 0; i < splitArtists.Length; i++)
                {
                    artist = splitArtists[i];
                    if(artist == string.Empty) 
                        continue;

                    if (artists.TryGetValue(artist, out List<string> roles))
                    {
                        // Ensure unique roles for each artist
                        if (!roles.Contains(role, StringComparer.OrdinalIgnoreCase))
                        {
                            roles.Add(role);
                        }
                    }
                    else
                    {
                        artists[artist] = [role];
                    }
                }
            }
        }

        public static void ReadDescription(string description, Dictionary<string, List<string>> artists)
        {
            if (string.IsNullOrWhiteSpace(description)) return;

            string[] categories = description.Split(':');

            foreach (string category in categories)
            {
                string[] keyValues = category.Split("\n");

                if(keyValues.Length < 2) continue;

                ReadDescriptionLines(keyValues, artists);
            }
        }

        public static void ReadDescriptionLines(string[] lines, Dictionary<string, List<string>> artists, bool skipFirstLine = true)
        {
            List<string> rolesToIgnore =
            [
                "main",
                "primary",
                "album",
                "http",
                "label",
                "visit",
                "purchased",
                "copyright",
                "other",
                "unknown",
                "...",
            ];

            for (int i = 1; i < lines.Length; i++)
            {
                // format => artist name, role 1, role 2...
                string[] values = lines[i].Split(",");
                List<string> foundArtists = [];

                // there may be multiple artists separated by a '&'
                string[] splitArtists = values[0].Split("&");
                foreach (string artist in splitArtists)
                {
                    if (!artist.IsNullOrWhiteSpace() && 
                        !artist.Contains("...") && 
                        !artist.Contains("label", StringComparison.CurrentCultureIgnoreCase) &&
                        !foundArtists.Any(fa => fa.Equals(artist, StringComparison.OrdinalIgnoreCase)))
                    {
                        foundArtists.Add(artist);
                    }
                }

                // find the roles
                for (int y = 1; y < values.Length; y++)
                {
                    string role = values[y].Trim();
                    string roleToLower = role.ToLower();
                    if (roleToLower.IsNullOrWhiteSpace() || rolesToIgnore.Any(r => roleToLower.Contains(r)))
                        continue;

                    if (roleToLower.Contains("performer"))
                    {
                        role = "Performer";
                    }

                    if (roleToLower.Contains("featured"))
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

