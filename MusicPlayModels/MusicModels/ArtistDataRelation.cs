namespace MusicPlayModels.MusicModels
{
    public class ArtistDataRelation
    {
        public int ArtistId { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsPerformer { get; set; }

        public bool IsComposer { get; set; }

        public bool IsLyricist { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsAlbumArtist { get; set; }

        public ArtistDataRelation(int artistId, string name, bool isPerformer, bool isComposer, bool isLyricist, bool isFeatured)
        {
            ArtistId = artistId;
            Name = name;
            IsPerformer = isPerformer;
            IsComposer = isComposer;
            IsLyricist = isLyricist;
            IsFeatured = isFeatured;
        }

        public ArtistDataRelation()
        {
            
        }

        public void SetAlbumArtist(int AlbumArtistId)
        {
            if(ArtistId == AlbumArtistId)
            {
                IsAlbumArtist = true;
            }
            else
            {
                IsAlbumArtist = false;
            }
        }

        public ArtistDataRelation Update(ArtistDataRelation artist)
        {
            IsAlbumArtist = IsAlbumArtist || artist.IsAlbumArtist;
            IsComposer = IsComposer || artist.IsComposer;
            IsPerformer = IsPerformer || artist.IsPerformer;
            IsFeatured = IsFeatured || artist.IsFeatured;

            return this;
        }
    }

    public static class ArtistsDataRelationHelper
    {
        public static List<ArtistDataRelation> Order(this List<ArtistDataRelation> list, bool acceptAlbumArtist = true, bool accept0 = false)
        {
            if (list is null) return new();

            Dictionary<ArtistDataRelation, int> keyValues = new();
            List<ArtistDataRelation> sortedList = new();

            for (int i = 0; i < list.Count; i++)
            {
                int score = list[i].GetScore(acceptAlbumArtist);

                if(score > 0)
                {
                    // count the number of artist with a higher score to get its sorted index
                    int valueSupCount = 0; 
                    foreach (KeyValuePair<ArtistDataRelation, int> kvp in keyValues)
                    {
                        if(kvp.Value > score)
                        {
                            valueSupCount++;
                        }
                    }
                    sortedList.Insert(valueSupCount, list[i]);
                    keyValues.Add(list[i], score);
                }
                else if(score == 0 && accept0)
                {
                    // add at the end since 0 is the min score
                    sortedList.Add(list[i]);
                    keyValues.Add(list[i], score);
                }
            }

            return sortedList;
        }

        public static List<ArtistDataRelation> Select(this List<ArtistDataRelation> artists, bool albumArtist, bool performer, bool composer, bool lyricist, bool featured)
        {
            List<ArtistDataRelation> selectedList = new();

            foreach (ArtistDataRelation artist in artists)
            {
                if(artist.IsAlbumArtist && albumArtist)
                {
                    selectedList.Add(artist);
                }
                else if(artist.IsPerformer && performer)
                {
                    selectedList.Add(artist);
                }
                else if(artist.IsComposer && composer)
                {
                    selectedList.Add(artist);
                }
                else if(artist.IsLyricist && lyricist)
                {
                    selectedList.Add(artist);
                }
                else if(artist.IsFeatured && featured)
                {
                    selectedList.Add(artist);
                }
            }
            return selectedList;
        }

        public static List<ArtistDataRelation> Exclude(this List<ArtistDataRelation> artists, bool albumArtist, bool performer, bool composer, bool lyricist, bool featured)
        {
            List<ArtistDataRelation> selectedList = new();

            foreach (ArtistDataRelation artist in artists)
            {
                if (artist.IsAlbumArtist && !albumArtist)
                {
                    selectedList.Add(artist);
                }
                else if (artist.IsPerformer && !performer)
                {
                    selectedList.Add(artist);
                }
                else if (artist.IsComposer && !composer)
                {
                    selectedList.Add(artist);
                }
                else if (artist.IsLyricist && !lyricist)
                {
                    selectedList.Add(artist);
                }
                else if (artist.IsFeatured && !featured)
                {
                    selectedList.Add(artist);
                }
            }
            return selectedList;
        }

        private static int GetScore(this ArtistDataRelation artist, bool acceptAlbumArtist = true)
        {
            int performerValue = 11;
            int composerValue = 6;
            int featuredValue = 4;
            int lyricistValue = 1;
            int albumArtist = 40;

            if (!acceptAlbumArtist)
                albumArtist = -40;

            int score = 0;

            if (artist.IsPerformer)
                score += performerValue;
            if(artist.IsComposer)
                score += composerValue;
            if(artist.IsFeatured)
                score += featuredValue;
            if (artist.IsLyricist)
                score += lyricistValue;
            if(artist.IsAlbumArtist)
                score += albumArtist;

            return score;
        }
    }

}
