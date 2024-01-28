using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels
{
    public static class Utils
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static Dictionary<string, object> AddRange(this Dictionary<string, object> dic1, Dictionary<string, object> dic2)
        {
            return dic1.Union(dic2).ToDictionary(k => k.Key, v => v.Value);
        }

        public static string DateTimeToString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string DateTimeToDateOnlyString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// take a timespan and return its string representation
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns> hh:mm:ss or mm:ss if hours are not needed </returns>
        public static string ToFormattedString(this TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
                return timeSpan.ToString(@"d\.hh\:mm\:ss");
            else if (timeSpan.Hours > 0)
                return timeSpan.ToString(@"hh\:mm\:ss");
            else return timeSpan.ToString(@"mm\:ss");
        }

        public static string FormatStringToTime(this string time)
        {
            if (time.Split(":").Length == 2)
            {
                return "00:" + time;
            }
            else return time;
        }
    }
}
