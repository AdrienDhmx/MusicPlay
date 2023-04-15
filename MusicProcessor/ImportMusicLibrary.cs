using MusicPlayModels.MusicModels;
using DataBaseConnection.DataAccess;
using TagLib;
using File = TagLib.File;
using MusicFilesProcessor.Helpers;
using System.IO;
using System.Globalization;
using System.Net.Http;
using MessageControl;
using System.Windows;
using FilesProcessor;
using System.Net;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Text;
using System.Net.WebSockets;

namespace MusicFilesProcessor
{
    public class ImportMusicLibrary
    {
        private readonly string _folder;

        public static readonly List<string> FilesExtensions = new() 
        { 
            ".aac",
            ".aiff", 
            ".alac", 
            ".flac", 
            ".mp3", 
            ".m4a", 
            ".ogg", 
            ".wma", 
            ".wav", 
        };
        private readonly string UNKNOWN = "UNKNOWN";
        private readonly string VARIOUSARTIST = "Various Artists";
        private List<string> files = new();
        private List<string> importedFiles = new();
        private List<ArtistModel> importedArtists = new();

        private List<TrackModel> AllTracksModel { get; set; } = new();
        private List<AlbumModel> AllAlbumsModel { get; set; } = new();
        private List<GenreModel> AllGenres { get; set; } = new();
        private Dictionary<int, Dictionary<string, List<bool>>> FileArtists = new();
        private Dictionary<string, string> CoverDictionary = new();

        public int FileNumber { get; private set; }
        public int TotalProgress { get; private set; }

        private string _currentStep;
        public string CurrentStep
        {
            get { return _currentStep; }
            private set { _currentStep = value; }
        }

        private string _currentFile;
        public string CurrentFile
        {
            get { return _currentFile; }
            private set 
            {
                _currentFile = value;
            }
        }

        private int _progress;
        public int Progress
        {
            get => _progress;
            private set
            {
                _progress = value;
                ProgressPercentage = (int)((double)((double)value / (double)TotalProgress) * 100);
            }
        }

        private int _progressPercentage;
        public int ProgressPercentage
        {
            get => _progressPercentage;
            private set
            {
                _progressPercentage = value;
                OnProgressChanged();
            }
        }

        public event Action ProgressChanged;
        private void OnProgressChanged()
        {
            ProgressChanged?.Invoke();
        }

        public async static Task<int> CheckTrackPaths()
        {
            List<TrackModel> tracks = await DataAccess.Connection.GetAllTracks();
            int deletedTrack = 0;

            foreach (TrackModel track in tracks)
            {
                if (!System.IO.File.Exists(track.Path))
                {
                    await DataAccess.Connection.DeleteTrack(track.Id);
                    CoverProcessor.DeleteCover(track.Artwork); // try to delete artwork if there is one

                    var albumTracks = await DataAccess.Connection.GetTracksFromAlbum(track.AlbumId);
                    if (albumTracks.Count == 0)
                    {
                        await DataAccess.Connection.DeleteAlbum(track.AlbumId);
                    }

                    foreach (ArtistDataRelation relation in track.Artists)
                    {
                        var albums = await DataAccess.Connection.GetAlbumsFromArtist(relation.ArtistId);
                        var artistTracks = await DataAccess.Connection.GetTracksFromArtist(relation.ArtistId);

                        ArtistModel artist = await DataAccess.Connection.GetArtist(relation.ArtistId);
                    
                        if (albums.Count == 0 && artistTracks.Count == 0) // delete artist
                        {
                            await DataAccess.Connection.DeleteArtist(relation.ArtistId);

                            CoverProcessor.DeleteCover(artist.Cover);
                        }
                        else if (albums.Count == 0) // artist is not an albumArtist
                        {
                            if (artist.IsAlbumArtist)
                            {
                                artist.IsAlbumArtist = false;
                                await DataAccess.Connection.UpdateArtist(artist); // try to delete the artist cover
                            }
                        }
                        else if(artistTracks.Count == 0) // artist is not a Performer anymore
                        {
                            artist.IsPerformer = false;
                            await DataAccess.Connection.UpdateArtist(artist);
                        }
                    }

                    deletedTrack++;
                }
            }
            return deletedTrack;
        }

        public ImportMusicLibrary(string folder)
        {
            _folder = folder;
            GetImportedFiles();
            FileNumber = GetFiles();
            TotalProgress = FileNumber * 2; // scan all files twice and then update all artists1
        }

        /// <summary>
        /// Scan the folder passed in the ctor and insert/update the data
        /// </summary>
        public void Import()
        {
            Progress = 0;
            CurrentStep = "Looking for artists...";
            ImportArtists();

            CurrentStep = "Importing albums and tracks...";
            ImportAlbumsAndTracks();

            CurrentFile = "";
            CurrentStep = "Import Done! Now updating artists data...";
            UpdateArtistData();

            Progress = TotalProgress;
            CurrentStep = "Done!";
        }

        private async void GetImportedFiles() 
        {
            List<TrackModel> tracks = await DataAccess.Connection.GetAllTracks();
            foreach(var track in tracks)
            {
                importedFiles.Add(track.Path);
            }
            importedArtists = await DataAccess.Connection.GetAllArtists();
            foreach (ArtistModel artist in importedArtists)
            {
                artist.Genres = await DataAccess.Connection.GetArtistGenre(artist.Id);
            }
        } 

        private int GetFiles()
        {
            files = Directory.EnumerateFiles(_folder, "*.*", SearchOption.AllDirectories).ToList().Where(s => FilesExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();
            files = files.Except(importedFiles).Distinct().ToList();
            return files.Count;
        }

        /// <summary>
        /// scan all files once to find all artists1 and add them to the database
        /// </summary>
        /// <returns></returns>
        private async void ImportArtists()
        {
            // get already imported genres
            AllGenres = await DataAccess.Connection.GetAllGenres();

            int i = 0;
            foreach (string file in files)
            {
                File fileInfo = File.Create(file);

                // artistName => { IsAlbumArtist, IsComposer, IsPerformer, IsFeatured, IsLyricist }
                Dictionary<string, List<bool>> artists = GetArtists(fileInfo, file);
                FileArtists.Add(i, artists); // avoid getting the artists afterward
                i++;

                if(artists.Count > 0)
                    CurrentFile = artists.Keys.ToList()[0];
                Progress++;

                string[] genres = fileInfo.Tag.Genres;
                List<GenreModel> artistGenre = new();

                // insert genres if not in database
                if (genres != null && genres.Length > 0)
                {
                    foreach (string g in genres)
                    {
                        if (!string.IsNullOrWhiteSpace(g))
                        {
                            string curGenre = g.ToLower();
                            if (!AllGenres.Any(g => g.Name.ToLower() == curGenre)) // not in the database
                            {
                                GenreModel genreModel = new() { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(curGenre) };
                                genreModel.Id = await DataAccess.Connection.InsertGenre(genreModel);
                                artistGenre.Add(genreModel);
                                AllGenres.Add(genreModel);
                            }
                            else if(!artistGenre.Any(g => g.Name.ToLower() == curGenre.ToLower()))
                            {
                                artistGenre.Add(AllGenres.Find(g => g.Name.ToLower() == curGenre));
                            }
                        }
                    }
                }

                foreach (var kvp in artists)
                {
                    string artist = kvp.Key;

                    // ignore accent and case
                    ArtistModel importedArtist = importedArtists.Find(a => a.Name.ToLower() == artist.ToLower());

                    // artist not in imported yet
                    if (importedArtist is null || importedArtist.Name == "")
                    {
                        ArtistModel artistModel = new(artist);

                        if (kvp.Value[0]) // album artist
                        {
                            artistModel.IsAlbumArtist = true;
                        }

                        if (kvp.Value[1]) // composer
                        {
                            artistModel.IsComposer = true;
                        }

                        if (kvp.Value[2]) // performer
                        {
                            artistModel.IsPerformer = true;
                        }

                        if (kvp.Value[3]) // featured 
                        {
                            artistModel.IsFeatured = true;
                        }

                        if (kvp.Value[4]) // lyricist
                        {
                            artistModel.IsLyricist = true;
                        }

                        artistModel.Genres = artistGenre ?? new();
                        artistModel.Id = await DataAccess.Connection.InsertArtist(artistModel);

                        importedArtists.Add(artistModel);
                    }
                    else // artist already in the list, update it
                    {
                        int index = importedArtists.IndexOf(importedArtist);
                        bool updated = await CheckArtistBool(importedArtist, kvp.Value);

                        foreach (GenreModel genre in artistGenre)
                        {
                            if (genre is null)
                                continue;

                            if (!importedArtist.Genres.Any(g => g.Id == genre.Id))
                            {
                                await DataAccess.Connection.InsertArtistGenre(importedArtist.Id, genre.Id);
                                importedArtist.Genres.Add(genre);
                                updated = true;
                            }
                        }

                        if (updated)
                        {
                            importedArtists.RemoveAt(index);
                            importedArtists.Add(importedArtist);
                        }
                    }
                }
            }
        }

        private async void ImportAlbumsAndTracks()
        {
            // get already imported genres
            AllGenres = await DataAccess.Connection.GetAllGenres();

            List<TrackModel> AlbumTracks = new(); // store tracks of each albums
            AlbumModel album = new(); // album
            List<ArtistDataRelation> ArtistDataRelations = new(); // store artists1 of each albums

            string previousAlbum = ""; // previous album name, used to compare with current one to know if the album has changed
            List<string> previousArtist = new(); // 
            string previousFile = ""; // 

            List<string> albumGenres = new(); // genres of the album
            int albumLength = 0; // nb of ms for the current album
            int artistCount = 0; // nb of artist
            int i = 0;
            foreach (string file in files)
            {
                CurrentFile = file;
                Progress++;

                File tag = File.Create(file); // file metadata

                string title = tag.Tag.Title;
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = Path.GetFileNameWithoutExtension(file); // use the file name as the title of the track
                }
                string albumTitle = tag.Tag.Album;
                if (string.IsNullOrWhiteSpace(albumTitle))
                {
                    albumTitle = DirectoryHelper.GetFolderName(file); // use the folder name containing the file as the album name
                }

                // artistName => { IsAlbumArtist, IsComposer, IsPerformer, IsFeatured, IsLyricist }
                Dictionary<string, List<bool>> artists = FileArtists[i];
                string albumArtist = TagHelper.GetArtist(artists, 0);
                i++;

                string[] genres = tag.Tag.Genres;
                string copyright = tag.Tag.Copyright;
                string duration = TagHelper.ToFormattedString(tag.Properties.Duration);
                int length = (int)tag.Properties.Duration.TotalMilliseconds;
                int discNumber = (int)tag.Tag.Disc;
                int trackNumber = (int)tag.Tag.Track;
                int year = (int)tag.Tag.Year;

                // the album has changed
                if (albumTitle != album.Name && file.GetDirectory() != previousFile.GetDirectory())
                {
                    InsertAlbum(album, albumGenres, AlbumTracks, ArtistDataRelations);

                    // Create a new album 
                    album = new()
                    {
                        Name = albumTitle,
                        Year = year,
                        Copyright = copyright,
                        AlbumCover = GetAlbumCover(tag, file, albumTitle)
                    };                   

                    // Reinitialise the data used for the previous album
                    AlbumTracks = new();
                    previousArtist = new();
                    ArtistDataRelations = new();
                    albumGenres = new();
                    artistCount = 0;
                    albumLength = 0;
                    previousAlbum = albumTitle;
                }
                else
                {
                    // Check if the album features multiple artist (>=2)
                    if (!previousArtist.Contains(albumArtist))
                    {
                        previousArtist.Add(albumArtist);
                        artistCount++;
                        if (artistCount == 2) // at least 3 artists needed to be categorized as Various Artists
                        {
                            album.VariousArtists = true;
                        }
                    }
                }

                List<ArtistDataRelation> relationList = new List<ArtistDataRelation>(); // artists track and their relation to the current track
                foreach (var kvp in artists)
                {
                    ArtistModel ArtistModel = importedArtists.Find(a => a.Name.ToLower() == kvp.Key.ToLower());

                    if(ArtistModel != null)
                    {
                        ArtistDataRelation artistTrack = new();
                        ArtistDataRelation artistAlbum = new();
                        artistTrack.ArtistId = ArtistModel.Id;
                        artistTrack.Name = ArtistModel.Name;
                        artistAlbum.ArtistId = ArtistModel.Id;
                        artistAlbum.Name = ArtistModel.Name;

                        if (kvp.Key == albumArtist) // album artist
                        {
                            artistAlbum.IsAlbumArtist = true;
                            artistTrack.IsAlbumArtist = true;
                        }

                        if (kvp.Value[1]) // composer
                        {
                            artistAlbum.IsComposer = true;
                            artistTrack.IsComposer = true;
                        }

                        if (kvp.Value[2]) // performer
                        {
                            artistAlbum.IsPerformer = true;
                            artistTrack.IsPerformer = true;
                        }

                        if (kvp.Value[3] && !artistAlbum.IsAlbumArtist) // featured
                        {
                            artistAlbum.IsFeatured = true;
                            artistTrack.IsFeatured = true;
                        }

                        if (kvp.Value[4]) // lyricist
                        {
                            artistAlbum.IsLyricist = true;
                            artistTrack.IsLyricist = true;
                        }

                        if(!relationList.Select(a => a.ArtistId).Contains(artistTrack.ArtistId))
                            relationList.Add(artistTrack);

                        ArtistDataRelation ArtistDataRelation = ArtistDataRelations.Find(a => a.ArtistId == artistAlbum.ArtistId);
                        if (ArtistDataRelation is null || ArtistDataRelation.ArtistId == -1)
                        {
                            if(!ArtistDataRelations.Any(a => a.IsAlbumArtist && a.ArtistId == artistAlbum.ArtistId))
                                ArtistDataRelations.Add(artistAlbum);
                        }
                        else
                        {
                            ArtistDataRelations.Remove(ArtistDataRelation);
                            ArtistDataRelations.Add(ArtistDataRelation.Update(artistAlbum));
                        }
                    }
                }

                string artwork = ImageHelper.GetTrackCoverFromDirectory(DirectoryHelper.GetDirectory(file), title, albumTitle);

                TrackModel Track = new()
                {
                    Path = file,
                    Title = title,
                    Duration = duration,
                    Artists = relationList,
                    Length = length,
                    DiscNumber = discNumber == 0 ? 1 : discNumber,
                    Tracknumber = trackNumber,
                    PlayCount = 0,
                    Rating = 0,
                    Artwork = artwork,
                    LastPlayed = DateTime.MinValue,
                    IsFavorite = false,
                };

                AlbumTracks.Add(Track);
                albumLength += length;
                previousFile = file;

                foreach (string genre in genres)
                {
                    if(!albumGenres.Any(g => g.ToLower() == genre.ToLower()))
                    {
                        albumGenres.Add(genre);
                    }
                }
            }

            // Get the last album (last file done but its album hasn't been inserted in database
            InsertAlbum(album, albumGenres, AlbumTracks, ArtistDataRelations);
        }       

        /// <summary>
        /// Update the artists1 data that may have changed with the added tracks and albums
        /// </summary>
        /// <returns></returns>
        private async void UpdateArtistData()
        {
            TotalProgress = importedArtists.Count; // start a new progress
            Progress = 0;
            foreach (ArtistModel artist in importedArtists)
            {
                Progress++;

                if (string.IsNullOrWhiteSpace(artist.Cover))
                {
                    string cover = "";
                    string artistFolder = DirectoryHelper.TryGetArtistDirectory(artist.Name, _folder);
                    if (!string.IsNullOrWhiteSpace(artistFolder))
                    {
                        cover = ImageHelper.GetArtistCoverFromDirectory(artistFolder, artist.Name);
                    }
                    artist.Cover = cover;
                }

                List<TrackModel> AllTracks = await GetArtistTracksNotInAlbums(artist.Id);
                List<AlbumModel> AllAlbums = await DataAccess.Connection.GetAlbumsFromArtist(artist.Id);

                List<AlbumModel> albums = AllAlbumsModel.Where(a => a.ContainsArtist(artist.Id)).ToList();

                string coverPath = ImageHelper.CreateCoverPath(ImageHelper.CreateCoverFilename());
                if (albums?.Count > 0)
                {
                    bool coverFound = true;

                    albums = albums.OrderByDescending(a => a.Year).ToList(); // recent albums first
                    if (string.IsNullOrWhiteSpace(artist.Cover) & albums.Count >= 4)
                    {
                        coverFound = ImageProcessor.Merge4ImagesInOne(albums[0].AlbumCover, albums[1].AlbumCover, albums[2].AlbumCover, albums[3].AlbumCover, coverPath);
                        if (coverFound)
                        {
                            artist.Cover = coverPath;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(artist.Cover))
                    {
                        string validAlbumCover = "";
                        for (int i = 0; i < albums.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(albums[i].AlbumCover))
                            {
                                validAlbumCover = albums[i].AlbumCover;
                                break;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(validAlbumCover))
                        {
                            artist.Cover = validAlbumCover;
                        }
                        else
                        {
                            artist.Cover = "";
                        }
                    }
                }
                else if (AllTracks.Count > 0)
                {
                    AlbumModel album = await DataAccess.Connection.GetAlbum(AllTracks[0].AlbumId);
                    artist.Cover = album.AlbumCover;
                }

                if (!artist.IsPerformer && !artist.IsAlbumArtist && !artist.IsComposer && !artist.IsFeatured && !artist.IsLyricist)
                {
                    // artist has no data, there may have been an error
                    //Debug.WriteLine($"Artist has no data and is removed from database: {artist.Name}");
                    await DataAccess.Connection.DeleteArtist(artist.Id);
                }
                else
                {
                    await DataAccess.Connection.UpdateArtistCover(artist);
                    await DataAccess.Connection.UpdateArtist(artist);
                }
            }
        }

        private async void InsertAlbum(AlbumModel album, List<string> genres, List<TrackModel> tracks, List<ArtistDataRelation> ArtistDataRelations)
        {
            if (!string.IsNullOrWhiteSpace(album.Name)) // valid album name
            {
                album.Artists = ArtistDataRelations;

                tracks.GetTotalLength(out int length);
                if(tracks.Count < 4 && length < 11 * 60 * 1000) // less than 11 minutes
                {
                    album.IsSingle = true;
                }
                else if(tracks.Count > 4 && tracks.Count < 8 && length < 30 * 60 * 1000) // less than 30 minutes
                {
                    album.IsEP = true;
                }

                // insert the model in the database and get its id
                album.Id = await DataAccess.Connection.InsertAlbum(album);

                // insert all genres 
                // if new genre found it's inserted in the database
                if (genres != null && genres.Count > 0)
                {
                    foreach (string g in genres)
                    {
                        if (!string.IsNullOrWhiteSpace(g))
                        {
                            string curGenre = g.ToLower();
                            if (!AllGenres.Any(ag => ag.Name.ToLower() == curGenre)) // not in the database
                            {
                                GenreModel genreModel = new() { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(curGenre) };
                                genreModel.Id = await DataAccess.Connection.InsertGenre(genreModel);
                                AllGenres.Add(genreModel);

                                // insert the album genres
                                await DataAccess.Connection.InsertAlbumGenre(album.Id, genreModel.Id);

                            }
                            else
                            {
                                // Find and insert the album genres
                                await DataAccess.Connection.InsertAlbumGenre(album.Id, AllGenres.Find(ag => ag.Name.ToLower() == curGenre).Id);
                            }
                        }
                    }
                }

                // Update all the tracks in the album with the albumId and insert them in the database and add to the list
                foreach (TrackModel t in tracks)
                {
                    t.AlbumId = album.Id;
                    t.Id = await DataAccess.Connection.InsertTrack(t);
                    AllTracksModel.Add(t);
                }

                // Add the model with the id in the list
                AllAlbumsModel.Add(album);
            }
        }

        private static async Task<bool> CheckArtistBool(ArtistModel artist, List<bool> booleans)
        {
            bool updated = false;
            if (booleans[0] && !artist.IsAlbumArtist) // album artist
            {
                artist.IsAlbumArtist = true;
                updated = true;
            }

            if (booleans[1] && !artist.IsComposer) // composer
            {
                artist.IsComposer = true;
                updated = true;
            }

            if (booleans[2] && !artist.IsPerformer) // performer
            {
                artist.IsPerformer = true;
                updated = true;
            }

            if (booleans[3] && !artist.IsFeatured) // featured 
            {
                artist.IsFeatured = true;
                updated = true;
            }

            if (booleans[4] && !artist.IsLyricist) // lyricist
            {
                artist.IsLyricist = true;
                updated = true;
            }

            if (updated)
            {
                await DataAccess.Connection.UpdateArtist(artist);
            }
            return updated;
        }

        private Dictionary<string, List<bool>> GetArtists(File audioTags, string filePath)
        {
            Dictionary<string, List<bool>> AlbumArtist = CreateArtistDic(audioTags.Tag.AlbumArtists.ToList(), new List<bool>() { true, false, false, false, false});
            Dictionary<string, List<bool>> Composer = CreateArtistDic(audioTags.Tag.Composers.ToList(), new List<bool>() { false, true, false, false, false });
            Dictionary<string, List<bool>> Performer = CreateArtistDic(audioTags.Tag.Performers.ToList(), new List<bool>() { false, false, true, false, false });
            // featured
            // lyricist

            Dictionary<string, List<bool>> description = TagHelper.ReadDescription(audioTags.Tag.Description);

            List<Dictionary<string, List<bool>>> ArtistsDic = new()
            {
                AlbumArtist,
                Composer,
                Performer,
                description,
            };

            // get all artists formatted with their correct bool values
            return FormatArtistsDictionary(audioTags, ArtistsDic);
        }

        private static Dictionary<string, List<bool>> CreateArtistDic(List<string> artists, List<bool> values)
        {
            Dictionary<string, List<bool>> output = new Dictionary<string, List<bool>>();

            foreach (string artist in artists)
            {
                if (!string.IsNullOrWhiteSpace(artist) && !output.ContainsKey(artist))
                {
                    output.Add(artist, values);
                }
            }

            return output;
        }

        private Dictionary<string, List<bool>> FormatArtistsDictionary(File audioTags, List<Dictionary<string, List<bool>>> artists)
        {
            Dictionary<string, List<bool>> output = new();
            for (int i = 0; i < artists.Count; i++)
            {
                foreach (KeyValuePair<string, List<bool>> kvp in artists[i])
                {
                    string artistName = RemoveDiacritics(kvp.Key.Trim());
                    if (string.IsNullOrWhiteSpace(kvp.Key))
                    {
                        artistName = audioTags.Tag.Publisher;
                        if (string.IsNullOrWhiteSpace(artistName))
                        {
                            artistName = UNKNOWN;
                        }
                        artistName.Trim();
                    }

                    if (artistName.Contains('&'))
                    {
                        List<string> splittedArtists = artistName.Split("&").ToList();

                        artistName = splittedArtists[0].Trim();

                        string secondArtist = splittedArtists[1].Trim();
                        secondArtist = TagHelper.FormatTag(secondArtist);

                        if (!secondArtist.Contains("..."))
                        {
                            if (artistName == VARIOUSARTIST && !kvp.Value[0])
                                continue;

                            InsertArtistInDic(ref output, secondArtist, kvp.Value);
                        }
                    }

                    artistName = TagHelper.FormatTag(artistName);

                    if (!artistName.Contains("..."))
                    {
                        if (artistName == VARIOUSARTIST && !kvp.Value[0])
                            continue;

                        InsertArtistInDic(ref output, artistName, kvp.Value);
                    }
                }
            }
            return output;
        }

        private static void InsertArtistInDic(ref Dictionary<string, List<bool>> dic, string key, List<bool> values)
        {
            if (!dic.Any(a => a.Key.ToLower() == key.ToLower()))
            {
                // not in the dictionnay insert it
                dic.Add(key, values);
            }
            else // update
            {
                string realKey = key;
                if (!dic.ContainsKey(key))
                {
                    realKey = dic.Where(a => a.Key.ToLower() == key.ToLower()).ToList()[0].Key;
                }

                List<bool> Ovalues = dic[realKey];
                for (int y = 0; y < 5; y++)
                {
                    Ovalues[y] = Ovalues[y] || values[y]; // keep true value only
                }

                dic[key] = Ovalues;
            }
        }

        private static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        private string GetAlbumCover(File audioTags, string file, string album)
        {
            (string OriginalCover, string AlbumCover) = ImageHelper.GetAlbumCover(album, file);
            if (!string.IsNullOrWhiteSpace(AlbumCover) && System.IO.File.Exists(AlbumCover))
            {
                if(CoverDictionary.ContainsKey(OriginalCover))
                {
                    return CoverDictionary[OriginalCover];
                }

                CoverDictionary.Add(OriginalCover, AlbumCover);
                return AlbumCover;
            }
            else if (audioTags.Tag.Pictures.Length > 0)
            {
                return SavePicture(audioTags.Tag.Pictures[0], album, DirectoryHelper.GetDirectory(file));
            }
            else
            {
                return "";
            }
        }

        private string SavePicture(IPicture picture, string album, string folder)
        {
            if (picture != null)
            {
                string Cover = ImageHelper.CreateCoverPath(ImageHelper.CreateCoverFilename());
                string ImgPath = Path.Combine(folder, album.Split(" ")[0] + "_Cover" + ImageHelper.imgFormat);

                if (CoverDictionary.ContainsKey(ImgPath))
                {
                    return CoverDictionary[ImgPath];
                }

                ByteVector picdata = picture.Data;
                byte[] data = picdata.Data;

                try
                {
                    System.IO.File.WriteAllBytes(ImgPath, data);

                    string newPath = ImageHelper.CreateCoverPath(ImageHelper.CreateCoverFilename());
                    ImageHelper.SaveFileToNewPath(ImgPath, newPath);

                    CoverDictionary.Add(ImgPath, newPath);
                    return newPath;
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Error while saving a cover: {ex.Message}"));
                    }));
                }
            }
            return "";
        }

        public static async Task<List<TrackModel>> GetArtistTracksNotInAlbums(int artistId)
        {
            var Albums = await DataAccess.Connection.GetAlbumsFromArtist(artistId);
            var Tracks = await DataAccess.Connection.GetTracksFromArtist(artistId);
            //Tracks.ForEach(t => t.ArtistId = artistId);
            Tracks = Tracks.Where(t => !Albums.Any(a => a.Id == t.AlbumId)).ToList();
            List<TrackModel> albumsId = Tracks.DistinctBy(t => t.AlbumId).ToList();
            foreach (TrackModel track in albumsId)
            {
                AlbumModel album = await DataAccess.Connection.GetAlbum(track.AlbumId);
                foreach (TrackModel t in Tracks)
                {
                    if (t.AlbumId == album.Id)
                    {
                        t.AlbumCover = album.AlbumCover;
                        t.AlbumName = album.Name;
                    }
                }
            }
            return Tracks;
        }
    }
}
