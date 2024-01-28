using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels
{
    public static class DataBaseColumns
    {
        // all ids for foreign keys needs to be written because the variable names won't work
        // (i.e.: nameof(Album.Id) != "AlbumId")
        public const string Id = "Id";
        public const string ArtistId = "ArtistId";
        public const string AlbumId = "AlbumId";
        public const string TrackId = "TrackId";
        public const string TagId = "TagId";
        public const string QueueId = "QueueId";
        public const string PlaylistId = "PlaylistId";
        public const string RoleId = "RoleId";
        public const string FolderId = "FolderId";
        public const string LyricsId = "LyricsId";
        public const string CountryId = "CountryId";
        public const string LabelId = "LabelId";
        public const string HistoryId = "HistoryId";
        public const string ArtistRoleId = "ArtistRoleId";
        public const string PrimaryArtistId = "PrimaryArtistId";
        public const string ArtistGroupId = "ArtistGroupId";
        public const string ArtistMemberId = "ArtistMemberId";
        public const string PlayingFromId = "PlayingFromId";
        public const string EQPresetId = "EQPresetId";
    }
}
