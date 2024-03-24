using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class OrderedTrackModel : TrackModel
    {
        private int _index;

        /// <summary>
        /// The index of the track inside a list. This is not the track number (<see cref="TrackModel.TrackNumber"/>)
        /// </summary>
        public int TrackIndex
        {
            get => _index;
            set
            {
                SetField(ref _index, value);
            }
        }

        public OrderedTrackModel(TrackModel trackModel) : base(trackModel)
        {

        }

        public OrderedTrackModel(TrackModel trackModel, int index) : base(trackModel)
        {
            TrackIndex = index;
        }

        public OrderedTrackModel() : base()
        {

        }
    }

    public static class QueueTrackModelExt
    {
        public static List<TrackModel> ToTrackModel(this IEnumerable<OrderedTrackModel> queueTracks)
        {
            List<TrackModel> output = new List<TrackModel>();

            foreach (OrderedTrackModel qt in queueTracks)
            {
                output.Add(qt);
            }

            return output;
        }

        public static List<OrderedTrackModel> ToOrderedTrackModel<T>(this List<T> tracks) where T : TrackModel
        {
            List<OrderedTrackModel> output = new();

            int index = 1;
            foreach (TrackModel track in tracks)
            {
                output.Add(new(track, index));
                index++;
            }

            return output;
        }
    }
}
