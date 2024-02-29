using System.ComponentModel.DataAnnotations.Schema;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;

namespace MusicPlay.Database.Models.DataBaseModels
{
    [NotMapped]
    public class OrderedTrack : ObservableObject
    {
        public int TrackId { get; set; }

        private int _trackIndex;
        public int TrackIndex 
        {
            get => _trackIndex;
            set => SetField(ref _trackIndex, value);
        }

        private Track _track;
        public Track Track
        {
            get => _track;
            set => SetField(ref _track, value);
        }

        public OrderedTrack(Track track, int trackIndex) 
        { 
            TrackId = track.Id;
            Track = track;
            TrackIndex = trackIndex;
        }
        public OrderedTrack() { }
    }

    [Table("PlaylistTrack")]
    public class PlaylistTrack : OrderedTrack
    {
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        public PlaylistTrack(int playlistId, int trackId, int trackIndex)
        {
            PlaylistId = playlistId;
            TrackId = trackId;
            TrackIndex = trackIndex;
        }

        public PlaylistTrack(Playlist playlist, Track track, int trackIndex)
        {
            PlaylistId = playlist.Id;
            TrackId = track.Id;
            Track = track;
            Playlist = playlist;
            TrackIndex = trackIndex;
        }

        public PlaylistTrack(Track track, int trackIndex)
        {
            TrackId = track.Id;
            Track = track;
            TrackIndex = trackIndex;
        }

        public PlaylistTrack() { }
    }

    [Table("QueueTrack")]
    public class QueueTrack : OrderedTrack
    {
        public int QueueId { get; set; }
        public Queue Queue { get; set; }

        public QueueTrack(int queueId, int trackId, int trackIndex)
        {
            QueueId = queueId;
            TrackId = trackId;
            TrackIndex = trackIndex;
        }

        public QueueTrack(Queue queue, int trackId, int trackIndex)
        {
            QueueId = queue.Id;
            TrackId = trackId;
            TrackIndex = trackIndex;
        }

        public QueueTrack(Track track, int trackIndex) : base(track, trackIndex) { }

        public QueueTrack() { }

        public override bool Equals(object obj)
        {
            if (obj is QueueTrack queueTrack)
            {
                if(queueTrack.TrackId != 0 && TrackId != 0)
                {
                    return queueTrack.TrackId == TrackId;
                } 
                else if(queueTrack.IsNotNull() && Track.IsNotNull())
                {
                    return queueTrack.Track.Id == Track.Id;
                }
                return base.Equals(obj); // can't compare by Id
            }
            return false;
        }
    }

    public class ArtistGroupMember
    {
        public int GroupId {  get; set; }
        public int MemberId { get; set; }

        public Artist Group { get; set; }
        public Artist Member { get; set; }

        public ArtistGroupMember(int groupArtistId, int memberArtistId)
        {
            GroupId = groupArtistId;
            MemberId = memberArtistId;
        }

        public ArtistGroupMember() { }
    }

    [NotMapped]
    public class TagRelationModel
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }

        public TagRelationModel(int tagId)
        {
            TagId = tagId;
        }

        public TagRelationModel(Tag tag)
        {
            TagId = tag.Id;
            Tag = tag;
        }

        public TagRelationModel()
        {
            
        }
    }

    [Table("TrackTag")]
    public class TrackTag : TagRelationModel
    {
        public int TrackId { get; set; }
        public Track Track { get; set; }

        public TrackTag(Tag tag, Track track) : base(tag)
        {
            TrackId = track.Id;
            Track = track;
        }

        public TrackTag()
        {
            
        }
    }

    [Table("AlbumTag")]
    public class AlbumTag : TagRelationModel
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public AlbumTag(Tag tag, Album album) : base(tag)
        {
            AlbumId = album.Id;
            Album = album;
        }

        public AlbumTag(int tagId, int albumId)
        {
            TagId = tagId;
            AlbumId = albumId;
        }

        public AlbumTag()
        {

        }
    }

    [Table("ArtistTag")]
    public class ArtistTag : TagRelationModel
    {
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }

        public ArtistTag(Tag tag, Artist artist) : base(tag)
        {
            ArtistId = artist.Id;
            Artist = artist;
        }

        public ArtistTag()
        {
            
        }
    }

    [Table("PlaylistTag")]
    public class PlaylistTag : TagRelationModel
    {
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        public PlaylistTag(Tag tag, Playlist playlist) : base(tag)
        {
            PlaylistId = playlist.Id;
            Playlist = playlist;
        }

        public PlaylistTag()
        {
            
        }
    }
}
