using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlay.Language;

namespace MusicPlay.Database.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly DateTime _janFirst1970 = new(1970, 1, 1);
        public static long GetTime(this DateTime dateTime)
        {
            return (long)((dateTime.ToUniversalTime() - _janFirst1970).TotalMilliseconds + 0.5);
        }

        public static DateTime ToDateTime(this long milliseconds)
        {
            return _janFirst1970.AddMilliseconds(milliseconds);
        }

        public static int GetDay(this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime).DayNumber;
        }

        public static DateOnly ToDateOnly(this int dayNumber)
        {
            return DateOnly.FromDayNumber(dayNumber);
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
        /// take a timespan and return its string representation (d.hh:mm:ss)
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns> hh:mm:ss or mm:ss if hours are not needed </returns>
        public static string ToShortString(this TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
                return timeSpan.ToString(@"d\.hh\:mm\:ss");
            else if (timeSpan.Hours > 0)
                return timeSpan.ToString(@"hh\:mm\:ss");
            else return timeSpan.ToString(@"mm\:ss");
        }

        /// <summary>
        /// take a timespan and return its string representation (x days y hours z minutes k seconds)
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>  </returns>
        public static string ToFullString(this TimeSpan timeSpan)
        {
            string output = "";
            if (timeSpan.Days > 0)
            {
                if (timeSpan.Days > 1)
                {
                    output += $"{timeSpan.Days} {Resources.Days} ";
                }
                else
                {
                    output += $"{timeSpan.Days} {Resources.Day} ";
                }
            }
            if (timeSpan.Hours > 0)
            {
                if (timeSpan.Hours > 1)
                {
                    output += $"{timeSpan.Hours} {Resources.Hours} ";
                }
                else
                {
                    output += $"{timeSpan.Hours} {Resources.Hour} ";
                }
            }
            if (timeSpan.Minutes > 1)
                output += $"{timeSpan.Minutes} {Resources.Minutes} ";
            else
                output += $"{timeSpan.Minutes} {Resources.Minute} ";
            if (timeSpan.Seconds > 1)
                output += $"{timeSpan.Seconds} {Resources.Seconds} ";
            else
                output += $"{timeSpan.Seconds} {Resources.Second} ";
            return output;
        }


    }
}
