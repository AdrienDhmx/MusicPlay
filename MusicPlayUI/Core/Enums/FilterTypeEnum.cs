using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Enums
{
    /// <summary>
    /// Describe a type of filter, each enum have multiple values to select from
    /// which filter a collection
    /// </summary>
    public enum FilterEnum
    {
        ArtistType = 0, // AlbumArtist, Performer, Composer...
        Artist = 1, // Only Specific Artist (Similar to searching for an Artist)
        Genre = 3, // Album CurrentTagView
        Year = 4, // release Date
        PlayCount = 5, // can filter by specifying a min an max playcount (0 - n)
        Rating = 6, // can filter by specifying a min and max ration (0-5)
        AlbumType = 7, // Singles, EP, Albums
    }
}
