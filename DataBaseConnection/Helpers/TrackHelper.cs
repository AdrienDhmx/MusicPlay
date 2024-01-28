using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Helpers
{
    public static class TrackHelper
    {
        public static IEnumerable<QueueTrack> ToQueueTrack(this IEnumerable<OrderedTrack> tracks)
        {
            IEnumerable<QueueTrack> queueTracks = [];
            foreach (var track in tracks)
            {
                queueTracks = queueTracks.Append(new(track.Track, track.TrackIndex));
            }
            return queueTracks;
        }

        public static ObservableCollection<OrderedTrack> ToOrderedTrack(this ObservableCollection<Track> tracks) 
        {
            ObservableCollection<OrderedTrack> orderedTracks = [];
            for (int i = 0; i < tracks.Count; ++i)
            {
                orderedTracks.Add(new(tracks[i], i + 1));
            }
            return orderedTracks;
        }

        public static int GetTotalLength<T>(this ObservableCollection<T> tracks) where T : Track
        {
            int length = 0;
            var asSpan = CollectionsMarshal.AsSpan(tracks.ToList());
            for (int i = 0; i < asSpan.Length; i++)
            {
                if (asSpan[i] != null)
                {
                    length += asSpan[i].Length;
                }
            }
            return length;
        }

        public static int GetTracksTotalLength<T>(this IEnumerable<T> tracks) where T : OrderedTrack
        {
            int length = 0;
            var asSpan = CollectionsMarshal.AsSpan(tracks.ToList());
            for (int i = 0; i < asSpan.Length; i++)
            {
                if (asSpan[i] != null)
                {
                    length += asSpan[i].Track.Length;
                }
            }
            return length;
        }
    }
}
