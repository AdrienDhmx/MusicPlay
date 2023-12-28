using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicFilesProcessor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MusicPlayUI.MVVM.Models;
using MusicPlayModels;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Windows.Navigation;

namespace MusicPlayUI.Core.Helpers
{
    public static class TrackListHelper
    {
        private static readonly Random rng = new();

        public async static Task<UIQueueModel> CreateQueue(this List<TrackModel> tracks, string playingFrom, string cover, bool albumCoverOnly, bool autoCover = false,
            TrackModel playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTrack = false)
        {
            if (tracks is null || tracks.Count == 0)
                return null;

            string duration = tracks.GetTotalLength(out int length);
            tracks = await tracks.GetAlbumTrackProperties();

            ObservableCollection<TrackModel> Tracks = new();

            if (isShuffled)
                Tracks = await Task.Run(() => Shuffle<TrackModel>(tracks));
            else if (orderTrack)
                Tracks = await Task.Run(Tracks.OrderTracks);
            else
                Tracks = new(tracks);

            if (playingTrack is null)
            {
                playingTrack = Tracks.First();
            }
            else
            {
                playingTrack = Tracks.ToList().Find(t => t.Id == playingTrack.Id);
                if (isShuffled)
                {
                    int index = Tracks.IndexOf(playingTrack);
                    if (index == -1)
                        playingTrack = Tracks.First();
                    else
                    {
                        Tracks.RemoveAt(index);
                        Tracks.Insert(0, playingTrack);
                    }
                }
            }

            return new(isShuffled, isOnRepeat, length, duration, playingTrack.Id, playingFrom, cover, playingTrack, tracks, albumCoverOnly, autoCover);
        }

        /// <summary>
        /// Get the albumName and the albumCover of each track.
        /// </summary>
        /// <param name="tracks"></param>
        /// <returns></returns>
        public async static Task<List<T>> GetAlbumTrackProperties<T>(this List<T> tracks) where T : TrackModel
        {
            int albumId = 0;
            string albumTitle = "";
            string albumCover = "";
            foreach (TrackModel t in tracks)
            {
                if (t != null && !string.IsNullOrWhiteSpace(t.Path))
                {
                    if (t.AlbumId != albumId)
                    {
                        albumId = t.AlbumId;

                        AlbumModel album = await DataAccess.Connection.GetAlbum(t.AlbumId);
                        albumTitle = album.Name;
                        albumCover = album.AlbumCover;
                    }
                    t.AlbumName = albumTitle;
                    t.AlbumCover = albumCover;
                }
            }
            return tracks;
        }

        public async static Task<T> GetAlbumTrackProperties<T>(this T track) where T : TrackModel
        {
            if (track != null && track.Path.ValidPath())
            {
                AlbumModel album = await DataAccess.Connection.GetAlbum(track.AlbumId);
                track.AlbumName = album.Name;
                track.AlbumCover = album.AlbumCover;
            }
            return track;
        }

        public static IEnumerable<T> YieldShuffle<T>(IEnumerable<T> source)
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

        public static ObservableCollection<T> Shuffle<T>(this IEnumerable<T> tracks, T firstTrack = null) where T : BaseModel
        {
            if (tracks is null || tracks.Count() < 2) return new(tracks);

            T[] elements = tracks.ToArray();
            int fisrtTrackIndex = -1;
            int n = tracks.Count();
            while(n > 1)
            {
                int k = rng.Next(n);
                n--;
                if (elements[k].Equals(firstTrack))
                    fisrtTrackIndex = k;
                else
                    (elements[k], elements[n]) = (elements[n], elements[k]);
            }

            if(fisrtTrackIndex != -1)
            {
                (elements[fisrtTrackIndex], elements[0]) = (elements[0], elements[fisrtTrackIndex]);
            }
            return new ObservableCollection<T>(elements);
        }

        public static ObservableCollection<T> OrderTracks<T>(this ObservableCollection<T> queue) where T : TrackModel
        {
            return new(queue.OrderBy(t => t.AlbumId).ThenBy(t => t.DiscNumber).ThenBy(t => t.TrackNumber));
        }

        public static List<T> OrderTracks<T>(this List<T> queue) where T : TrackModel
        {
            return new(queue.OrderBy(t => t.AlbumId).ThenBy(t => t.DiscNumber).ThenBy(t => t.TrackNumber));
        }

        public static ObservableCollection<UIOrderedTrackModel> OrderTracks(this ObservableCollection<UIOrderedTrackModel> queue)
        {
            return new(queue.OrderBy(t => t.TrackIndex));
        }

        public static List<UIOrderedTrackModel> OrderTracks(this List<UIOrderedTrackModel> queue)
        {
            return new(queue.OrderBy(t => t.TrackIndex));
        }

        public static int GetLengthUntilTrack<T>(this int trackIndex, ObservableCollection<T> queue) where T : TrackModel
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
    }
}
