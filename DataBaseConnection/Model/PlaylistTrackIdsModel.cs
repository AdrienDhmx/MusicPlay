using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.Model
{
    public class PlaylistTrackIdsModel
    {
        public int PlaylistId { get; set; }
        public int TrackId { get; set; }
        public int TrackIndex { get; set; }
    }

    public class QueueTrackIdsModel
    {
        public int QueueId { get; set; }
        public int TrackId { get; set; }
        public int TrackIndex { get; set; }
    }
}
