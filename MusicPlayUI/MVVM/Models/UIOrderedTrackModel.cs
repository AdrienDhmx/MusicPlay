using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicFilesProcessor.Helpers;
using MusicPlayModels.MusicModels;

namespace MusicPlayUI.MVVM.Models
{
    public class UIOrderedTrackModel : OrderedTrackModel
    {
        private string _cover;
        public string Cover 
        {
            get => _cover;
            set
            {
                _cover = value;
                SetField(ref _cover, value);
            }
        }

        public UIOrderedTrackModel(TrackModel trackModel, bool albumCover, bool autoCover = false) : base(trackModel)
        {
            Cover = SetCover(trackModel, albumCover, autoCover);
        }

        public UIOrderedTrackModel(TrackModel trackModel, int index, bool albumCover, bool autoCover = false) : base(trackModel, index)
        {
            Cover = SetCover(trackModel, albumCover, autoCover);
        }

        public UIOrderedTrackModel(OrderedTrackModel trackModel, bool albumCover, bool autoCover = false) : base(trackModel, trackModel.TrackIndex)
        {
            Cover = SetCover(trackModel, albumCover, autoCover);
        }

        public UIOrderedTrackModel() : base()
        {

        }

        private string SetCover(TrackModel trackModel, bool albumCover, bool autoCover = false)
        {
            if (autoCover)
            {
                if (!string.IsNullOrWhiteSpace(trackModel.Artwork))
                {
                    Cover = trackModel.Artwork;
                }
                else
                {
                    Cover = trackModel.AlbumCover;
                }
            }
            else if (albumCover)
            {
                Cover = trackModel.AlbumCover;
            }
            else
            {
                Cover = trackModel.Artwork;
            }

            return ImageHelper.GetModifiedCoverPath(Cover, false);
        }

        public override bool Equals(object obj)
        {
            if (obj is UIOrderedTrackModel track && Id == track.Id)
            {
                return true;
            }
            return false;
        }
    }

    public static class QueueTrackModelExt
    {
        public static List<TrackModel> ToTrackModel(this IEnumerable<UIOrderedTrackModel> queueTracks)
        {
            List<TrackModel> output = new List<TrackModel>();

            foreach (UIOrderedTrackModel qt in queueTracks)
            {
                output.Add(qt);
            }

            return output;
        }

        public static List<UIOrderedTrackModel> ToUIOrderedTrackModel(this IEnumerable<OrderedTrackModel> tracks, bool albumCover, bool autoCover)
        {
            List<UIOrderedTrackModel> output = new();

            foreach (OrderedTrackModel qt in tracks)
            {
                if(qt.TrackIndex - 1 >= output.Count)
                {
                    if(output.Count > 1 && output.Last().TrackIndex > qt.TrackIndex) // need to insert before the higher indexes
                    {
                        // find the index to insert the track to
                        int index = 0;
                        while (index<output.Count && output[index].TrackIndex < qt.TrackIndex)
                        {
                            index++;
                        }

                        if (index >= output.Count)
                        {
                            output.Add(new(qt, albumCover, autoCover));
                        }
                        else
                        {
                            output.Insert(index, new(qt, albumCover, autoCover));
                        }
                    }
                    else
                    {
                        output.Add(new(qt, albumCover, autoCover));
                    }
                }
                else
                {
                    output.Insert(qt.TrackIndex - 1, new(qt, albumCover, autoCover));
                }
            }

            return output;
        }

        public static List<UIOrderedTrackModel> ToUIOrderedTrackModel(this IEnumerable<TrackModel> tracks, bool albumCover, bool autoCover)
        {
            List<UIOrderedTrackModel> output = new();

            int index = 1;
            foreach (TrackModel track in tracks)
            {
                output.Add(new(track, index, albumCover, autoCover));
                index++;
            }

            return output;
        }

        public static List<UIOrderedTrackModel> ToUIOrderedTrackModelWithTrackNumber<T>(this List<T> tracks, bool albumCover, bool autoCover) where T : TrackModel
        {
            List<UIOrderedTrackModel> output = new();

            foreach (TrackModel track in tracks)
            {
                output.Add(new(track, track.Tracknumber, albumCover, autoCover));
            }

            return output;
        }
    }

}
