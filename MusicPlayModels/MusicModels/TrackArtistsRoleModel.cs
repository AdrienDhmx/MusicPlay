using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class TrackArtistsRoleModel : BaseModel
    {
        private ArtistModel _artist;
        public ArtistModel Artist
        {
            get => _artist;
            set => SetField(ref _artist, value);
        }

        private List<ArtistRoleModel> _artistRoles = new();
        public List<ArtistRoleModel> ArtistRoles
        {
            get => _artistRoles;
            set => SetField(ref _artistRoles, value);
        }

        public TrackArtistsRoleModel(ArtistModel artist, List<ArtistRoleModel> roles)
        {
            _artist = artist;
            ArtistRoles = roles;
        }
    }

    public static class TrackArtistsRoleModelExt
    {
        public static List<TrackArtistsRoleModel> Order(this List<TrackArtistsRoleModel> artists)
        {
            if (artists is null) return new();

            Dictionary<TrackArtistsRoleModel, int> keyValues = new();
            List<TrackArtistsRoleModel> sortedList = new();

            for (int i = 0; i < artists.Count; i++)
            {
                int score = artists[i].GetScore();

                if (score > 1)
                {
                    // count the number of artist with a higher score to get its sorted index
                    int sortedIndex = 0;
                    foreach (KeyValuePair<TrackArtistsRoleModel, int> kvp in keyValues)
                    {
                        if (kvp.Value > score)
                        {
                            sortedIndex++;
                        }
                    }
                    sortedList.Insert(sortedIndex, artists[i]);
                    keyValues.Add(artists[i], score);
                }
                else
                {
                    // add at the end since 1 is the min score
                    sortedList.Add(artists[i]);
                    keyValues.Add(artists[i], score);
                }
            }

            return sortedList;
        }

        private static int GetScore(this TrackArtistsRoleModel artist)
        {
            int score = 0;

            foreach (ArtistRoleModel role in artist.ArtistRoles)
            {
                string roleToLower = role.Role.ToLower();
                if (roleToLower.Contains("primary"))
                {
                    score += 50;
                }
                else if (roleToLower.Contains("featured"))
                {
                    score += 25;
                }
                else if (roleToLower.Contains("performer"))
                {
                    score += 10;
                }
                else if (roleToLower.Contains("composer"))
                {
                    score += 5;
                }
                else
                {
                    score++;
                }
            }

            return score;
        }
    }
}
