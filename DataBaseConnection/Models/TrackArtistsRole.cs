using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlay.Database.Models
{
    [Table("TrackArtistRole")]
    public class TrackArtistsRole
    {
        public int TrackId { get; set; } = -1;
        public int ArtistRoleId { get; set; } = -1;

        public Track Track { get; set; }
        public ArtistRole ArtistRole { get; set; }

        public TrackArtistsRole(Track track, ArtistRole artistRole) 
        {
            Track = track;
            ArtistRole = artistRole;
            TrackId = track.Id;
            ArtistRoleId = artistRole.Id;
        }

        public TrackArtistsRole(int trackId, ArtistRole artistRole)
        {
            ArtistRole = artistRole;
            ArtistRoleId = artistRole.Id;
            TrackId = trackId;
        }

        public TrackArtistsRole(int trackId, int artistRoleId)
        {
            TrackId = trackId; 
            ArtistRoleId = artistRoleId;
        }

        public TrackArtistsRole() { }

        public static async Task Insert(TrackArtistsRole trackArtistsRole, int trackId)
        {
            //foreach (ArtistRole trackArtistRole in trackArtistsRole.ArtistRoles)
            //{
            //    // make sure all the trackArtistRole roles exist in the database
            //    if(trackArtistRole.Id < 0)
            //    {
            //        await ArtistRole.Insert(trackArtistRole);
            //    }

            //    // insert the track trackArtistRole role
            //    await DataAccess.Connection.InsertOneRelation(new TrackArtistRoleRelation(trackArtistRole.Id, trackId));
            //}
        }

        /// <summary>
        /// Delete all the Track Artist Role and the trackArtistRole role if no other track trackArtistRole role reference it
        /// </summary>
        /// <param name="trackArtistsRole"></param>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public static async Task Delete(TrackArtistsRole trackArtistsRole, int trackId)
        {
            //foreach (ArtistRole trackArtistRole in trackArtistsRole.ArtistRoles)
            //{
            //    await DataAccess.Connection.DeleteOneRelation(new TrackArtistRoleRelation(trackArtistRole.Id, trackId));

            //    Where sameArtistRoleIdCondition = new Where(DataBaseColumns.ArtistRoleId, trackArtistRole.Id);
            //    List<TrackArtistRoleRelation> sameArtistRoleId = await DataAccess.Connection.GetAllRelationWhere<TrackArtistRoleRelation>(sameArtistRoleIdCondition);

            //    if(sameArtistRoleId.Count == 0)
            //    {
            //        await ArtistRole.Delete(trackArtistRole);
            //    }
            //}
        }
    }

    public static class TrackArtistsRoleModelExt
    {
        public static ObservableCollection<Artist> Order(this ObservableCollection<TrackArtistsRole> artists)
        {
            if (artists is null) return new();

            Dictionary<Artist, int> keyValues = new();

            for (int i = 0; i < artists.Count; i++)
            {
                int score = artists[i].GetScore();

                Artist artist = artists[i].ArtistRole.Artist;
                if (!keyValues.TryAdd(artist, score))
                {
                    keyValues[artist] += score;
                }
            }

            return new(keyValues.OrderByDescending(kvp => kvp.Value).Select(i => i.Key));
        }

        private static int GetScore(this TrackArtistsRole trackArtistRole)
        {
            string roleToLower = trackArtistRole.ArtistRole.Role.Name.ToLower();
            if (roleToLower.Contains("primary"))
            {
                return 100;
            }
            else if (roleToLower.Contains("featured"))
            {
                return 25;
            }
            else if (roleToLower.Contains("performer"))
            {
                return 10;
            }
            else if (roleToLower.Contains("composer"))
            {
                return 5;
            }
            else
            {
                return 1;
            }
        }
    }
}
