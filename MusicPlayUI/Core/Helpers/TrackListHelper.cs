using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlayUI.Core.Helpers
{
    public static class TrackListHelper
    {
        private static readonly Random rng = new();

        public static IEnumerable<T> YieldShuffle<T>(this IEnumerable<T> source)
        {
            if (source is null || !source.Any()) yield return default;
            else
            {
                T[] elements = source.ToArray();

                for (int i = elements.Length - 1; i >= 0; i--)
                {
                    // Swap element "i" with a random earlier element it (or itself)
                    // ... except we don't really need to swap it fully, as we can
                    // return it immediately, and afterwards it's irrelevant.
                    int swapIndex = rng.Next(i + 1);
                    yield return elements[swapIndex];
                    elements[swapIndex] = elements[i];
                }
            }
        }

        public static ObservableCollection<T> Shuffle<T>(this IEnumerable<T> tracks, T firstTrack = null) where T : ObservableObject
        {
            if (tracks is null || tracks.Count() < 2) return new(tracks);

            T[] elements = tracks.ToArray();
            int firstTrackIndex = -1;
            int n = tracks.Count();
            while(n > 1)
            {
                int k = rng.Next(n);
                n--;
                if (elements[k].Equals(firstTrack))
                    firstTrackIndex = k;
                else
                    (elements[k], elements[n]) = (elements[n], elements[k]);
            }

            if(firstTrackIndex != -1)
            {
                (elements[firstTrackIndex], elements[0]) = (elements[0], elements[firstTrackIndex]);
            }
            return new ObservableCollection<T>(elements);
        }

        public static ObservableCollection<T> OrderTracks<T>(this ObservableCollection<T> queue) where T : Track
        {
            return new(queue.OrderBy(t => t.Album.Id).ThenBy(t => t.DiscNumber).ThenBy(t => t.TrackNumber));
        }

        public static List<T> OrderTracks<T>(this List<T> queue) where T : Track
        {
            return new(queue.OrderBy(t => t.Album.Id).ThenBy(t => t.DiscNumber).ThenBy(t => t.TrackNumber));
        }

        public static ObservableCollection<T> Order<T>(this ObservableCollection<T> queue) where T : OrderedTrack
        {
            return new(queue.OrderBy(t => t.TrackIndex));
        }

        public static List<T> Order<T>(this List<T> queue) where T : OrderedTrack
        {
            return new(queue.OrderBy(t => t.TrackIndex));
        }

        public static ObservableCollection<T> ToOrderedTracks<T>(this ObservableCollection<Track> tracks) where T : OrderedTrack
        {
            ObservableCollection<T> output = [];
            for (int i = 0; i < tracks.Count; i++)
            {
                output.Add((T)new OrderedTrack(tracks[i], i));
            }
            return output;
        }

        public static List<T> ToOrderedTracks<T>(this List<Track> tracks) where T : OrderedTrack
        {
            List<T> output = [];
            for (int i = 0; i < tracks.Count; i++)
            {
                if(typeof(T) == typeof(PlaylistTrack))
                {
                    output.Add((T)(OrderedTrack)new PlaylistTrack(tracks[i], i + 1));
                }
                else
                {
                    output.Add((T)new OrderedTrack(tracks[i], i + 1));
                }
            }
            return output;
        }

        public static List<Track> ToTrack<T>(this List<T> tracks) where T : OrderedTrack
        {
            return tracks.Select(t => t.Track).ToList();
        }

        public static ObservableCollection<Track> ToTrack<T>(this ObservableCollection<T> tracks) where T : OrderedTrack
        {
            return new(tracks.Select(t => t.Track));
        }

        public static int GetLengthUntilTrack<T>(this int trackIndex, ObservableCollection<T> queue) where T : Track
        {
            int length = 0;
            var asSpan = CollectionsMarshal.AsSpan(queue.ToList());
            for (int i = 0; i < asSpan.Length; i++)
            {
                if (i == trackIndex)
                    return length;
                if (asSpan[i] != null)
                {
                    length += queue[i].Length;
                }
            }
            return length;
        }

        public static bool AreEquals<T>(this List<T> list1, List<T> list2) where T : BaseModel
        {
            if(list1.Count != list2.Count) return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].Id != list2[i].Id)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
