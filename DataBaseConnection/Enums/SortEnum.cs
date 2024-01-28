namespace MusicPlay.Database.Enums
{
    /// <summary>
    /// Describe a way of sorting a collection,
    /// the direction (A-Z or Z-A) of the sorting is not specified here
    /// </summary>
    public enum SortEnum
    {
        AZ = 0, // default
        Year = 1, // release date
        MostPlayed = 2,
        LastPlayed = 3,
        AddedDate = 4, // date added to collection
        UpdatedDate = 5,  // date of the last update of the data in database
    }
}
