using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlay.Language;

namespace MusicPlay.Database.Helpers
{
    /// <summary>
    /// A class with very common extension methods to improve readability
    /// </summary>
    public static class Utils
    {
        public static bool IsNull(this object? obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object? obj)
        {
            return obj != null;
        }

        public static bool IsNullOrEmpty(this IEnumerable<object>? list)
        {
            return list == null || !list.Any();
        }

        public static bool IsNotNullOrEmpty(this IEnumerable<object>? list)
        {
            return list != null && list.Any();
        }

        public static bool IsNullOrWhiteSpace(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrWhiteSpace(this string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static bool ToBool(this int value)
        {
            return value == 1;
        }

        public static Dictionary<string, object> AddRange(this Dictionary<string, object> dic1, Dictionary<string, object> dic2)
        {
            return dic1.Union(dic2).ToDictionary(k => k.Key, v => v.Value);
        }

        public static Dictionary<string, List<string>> AddRange(this Dictionary<string, List<string>> dic1, Dictionary<string, List<string>> dic2)
        {
            return dic1.Union(dic2).ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
