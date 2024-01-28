using TagLib;
using File = TagLib.File;
using MusicFilesProcessor.Helpers;
using System.IO;
using System.Globalization;
using System.Net.Http;
using MessageControl;
using System.Windows;
using FilesProcessor;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Enums;
using MusicFilesProcessor.Lyrics;
using MusicPlay.Database.DatabaseAccess;
using System.Drawing;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.DirectoryServices.ActiveDirectory;

namespace MusicFilesProcessor
{
    public class ImportMusicLibrary
    {
        private readonly Folder _folder;
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

        // the 3 most common artist roles
        private readonly string PrimaryArtistString = "Primary Artist";
        private readonly string ComposerRole = "Composer";
        private readonly string PerformerRole = "Performer";


        private List<string> files = new();
        private readonly List<string> importedFiles = new();

        private List<Artist> AllArtistsModel { get; set; } = new();
        private List<Album> AllAlbumsModel { get; set; } = new();
        private List<Track> AllTracksModel { get; set; } = new();
        private List<MusicPlay.Database.Models.Tag> AllTagsModel { get; set; } = new();

        private Dictionary<int, Dictionary<string, List<bool>>> FileArtists = new();
        private readonly Dictionary<string, string> CoverDictionary = new();

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

        private int _fileImportedCount = 0;
        public int FileImportedCount
        {
            get => _fileImportedCount;
            private set => _fileImportedCount = value;
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
            List<Track> tracks = Track.GetAll();
            int deletedTrack = 0;

            foreach (Track track in tracks)
            {
                if (!System.IO.File.Exists(track.Path))
                {
                    await Track.Delete(track);
                    deletedTrack++;
                }
            }
            return deletedTrack;
        }

        public ImportMusicLibrary(Folder folder)
        {
            _folder = folder;
            GetImportedFiles();
            FileNumber = GetFiles();
            TotalProgress = FileNumber;
        }

        /// <summary>
        /// Scan the folder passed in the ctor and insert/update the data
        /// </summary>
        public void Import()
        {
            Progress = 0;
            ScanFiles();
            Progress = TotalProgress;
            CurrentStep = "Done!";
        }

        private void GetImportedFiles() 
        {
            AllTracksModel = Track.GetAll();
            foreach(Track track in AllTracksModel)
            {
                importedFiles.Add(track.Path);
            }
            AllArtistsModel = Artist.GetAll();
        } 

        private int GetFiles()
        {
            files = Directory.EnumerateFiles(_folder.Path, "*.*", SearchOption.AllDirectories).ToList().Where(s => FilesExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();
            files = files.Except(importedFiles).Distinct().ToList();
            return files.Count;
        }

        private async void ScanFiles()
        {
            // get all the existing data (the artists and tracks were already fetch in GetImportedFiles())
            AllAlbumsModel = Album.GetAll();
            AllTagsModel = MusicPlay.Database.Models.Tag.GetAll();
            List<Role> AllRoles = Role.GetAll();
            Role primaryArtistRole = AllRoles.Where(r => r.Name == PrimaryArtistString).FirstOrDefault();
            Role composerRole = AllRoles.Where(r => r.Name == ComposerRole).FirstOrDefault();
            Role performerRole = AllRoles.Where(r => r.Name == PerformerRole).FirstOrDefault();
            
            if(primaryArtistRole.IsNull()) // first role inserted to db => Id = 1
            {
                primaryArtistRole = new Role(PrimaryArtistString);
                Role.Insert(primaryArtistRole);
                AllRoles.Add(primaryArtistRole);
            }
            if(composerRole.IsNull()) // second role inserted to db => Id = 2
            {
                composerRole = new Role(ComposerRole);
                Role.Insert(composerRole);
                AllRoles.Add(composerRole);
            }
            if(performerRole.IsNull()) // third role inserted to db => Id = 3
            {
                performerRole = new Role(PerformerRole);
                Role.Insert(performerRole);
                AllRoles.Add(performerRole);
            }

            bool unknownArtistNotInDb = false;
            bool variousArtistNotInDb = false;
            // find if exist or create an Unknown TrackArtistRole model in case a primary artist does not exist for an albumName
            Artist UnknownArtistModel = AllArtistsModel.Find(a => a.Name == UNKNOWN);
            if(UnknownArtistModel == null)
            {
                unknownArtistNotInDb = true;
                UnknownArtistModel = new Artist()
                {
                    Name = UNKNOWN,
                    ArtistRoles = new() { new() { RoleId = primaryArtistRole.Id } }
                };
            }

            // find if exist or create a Various TrackArtistRole model in case an albumName has multiple primary artists
            Artist VariousArtistModel = AllArtistsModel.Find(a => a.Name == VARIOUSARTIST);
            if (VariousArtistModel == null)
            {
                variousArtistNotInDb = true;
                VariousArtistModel ??= new Artist()
                {
                    Name = VARIOUSARTIST,
                    ArtistRoles = new() { new() { RoleId = primaryArtistRole.Id } }
                };
            }

            // variables to store the currently scanned albumName data
            // it supposed that the each albumName has its own folder that contains the tracks within it
            Album CurrentAlbum = new();
            CurrentAlbum.PrimaryArtist = new();
            int CurrentAlbumIndexInAllAlbums = -1;
            List<MusicPlay.Database.Models.Tag> CurrentAlbumNewTags = new();
            Dictionary<Track, List<ArtistRole>> CurrentAlbumNewTracks = new();

            int primaryArtistInAlbumCount = 0;
            string previousFile = "";
            for (int i = 0; i < files.Count; i++)
            {
                CurrentFile = files[i];
                Progress++;

                // get the metadata
                File fileMetadata = File.Create(CurrentFile);

                // STEP 1: Handle TrackArtistRole metadata
                //  - Find all the artists credited and their role
                //  - create or update the artists

                // get the artists and their role
                Dictionary<string, List<string>> artistsCreditedDic = GetArtists(fileMetadata);

                // keep kvp of all the artist credited in this kvp with their specific role
                List<ArtistRole> artistsCreditedInTrack = new();
                Artist primaryArtist = null;

                // loop through all found artists to create or update them
                foreach (KeyValuePair<string, List<string>> kvp in artistsCreditedDic)
                {
                    Artist foundArtist = AllArtistsModel.Find(a => a.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));
                    bool isPrimaryArtist = false;

                    // new artist => create it's model
                    if (foundArtist == null)
                    {
                        string artistCover = ImageHelper.GetArtistCoverFromDirectory(DirectoryHelper.TryGetArtistDirectory(kvp.Key, CurrentFile), kvp.Key);
                        foundArtist = new Artist()
                        {
                            Name = kvp.Key,
                        };
                        await Artist.Insert(foundArtist);

                        if(artistCover.IsNotNullOrWhiteSpace())
                        {
                            await foundArtist.UpdateCoverWithFile(artistCover);
                        }

                        AllArtistsModel.Add(foundArtist);
                    }

                    // add the new roles if any
                    foreach (string role in kvp.Value)
                    {
                        // find the role
                        Role roleModel = AllRoles.Find(r => r.Name.Equals(role, StringComparison.CurrentCultureIgnoreCase));
                        
                        if(roleModel.IsNull())
                        {
                            roleModel = new(role);
                            Role.Insert(roleModel);
                            AllRoles.Add(roleModel);
                        }
                        if (role == PrimaryArtistString)
                        {
                            isPrimaryArtist = true;
                        }

                        ArtistRole newArtistRoleModel = foundArtist.ArtistRoles.FirstOrDefault(a => a.Role != null && a.Role.Name.Equals(roleModel.Name, StringComparison.CurrentCultureIgnoreCase));                        
                        if (newArtistRoleModel == null || newArtistRoleModel.Id == 0) // the artist doesn't have this role
                        {
                            newArtistRoleModel = new(roleModel, foundArtist);
                            await Artist.AddArtistRole(foundArtist, newArtistRoleModel);
                        }
                        artistsCreditedInTrack.Add(newArtistRoleModel);
                    }

                    // Keep kvp of the primary artists to later compare it with
                    // the primary artist of the CurrentAlbum
                    if(isPrimaryArtist)
                    {
                        primaryArtist = foundArtist;
                    }
                }

                // primary artist not found
                if(primaryArtist.IsNull())
                {
                    primaryArtist = UnknownArtistModel;
                    if(unknownArtistNotInDb)
                    {
                        await Artist.Insert(primaryArtist);
                        unknownArtistNotInDb = false;
                        UnknownArtistModel.ArtistRoles[0].Role = primaryArtistRole;
                    }
                }

                // STEP 2: Handle Album metadata
                //  - Find if the albumName has changed
                //  -> changed
                //      - Create or update the CurrentAlbum in the db
                //      - Insert all the tracks from this albumName in the db
                //      - Create the new Album model
                //          -> get the main info (title, release date, copyright)
                //          -> try to get the cover embedded or in the folder
                //  -> NOT changed
                //      - update the CurrentAlbum primary artists if needed
                //  - update the CurrentAlbum data if new information is found

                string albumName = fileMetadata.Tag.Album;
                if (string.IsNullOrWhiteSpace(albumName))
                {   // use the folder name containing the file as the albumName name
                    albumName = DirectoryHelper.GetFolderName(CurrentFile); 
                }

                int year = (int)fileMetadata.Tag.Year;
                string copyright = fileMetadata.Tag.Copyright;

                // The Album has changed
                if (!CurrentAlbum.Name.Equals(albumName, StringComparison.CurrentCultureIgnoreCase) && CurrentFile.GetDirectory() != previousFile.GetDirectory())
                {
                    if(i != 0) // not the first albumName
                    {
                        // insert or update the albumName
                        await InsertAlbum(CurrentAlbum, CurrentAlbumNewTracks, CurrentAlbumNewTags);
                        AllAlbumsModel.Add(CurrentAlbum);
                    }

                    // reset some variables for the new albumName
                    CurrentAlbumNewTags = new();
                    CurrentAlbumNewTracks = new();
                    primaryArtistInAlbumCount = 0;


                    // try to find if the new currently scanned albumName is already in the db
                    CurrentAlbumIndexInAllAlbums = AllAlbumsModel.FindIndex(a => 
                            a.Name.Equals(albumName, StringComparison.CurrentCultureIgnoreCase) && 
                            a.PrimaryArtist.Name.Equals(primaryArtist.Name, StringComparison.CurrentCultureIgnoreCase));
                    if(CurrentAlbumIndexInAllAlbums == -1)
                    {
                        // not found, Create a new albumName 
                        CurrentAlbum = CurrentAlbum = new()
                        {
                            Name = albumName,
                            ReleaseDate = year,
                            Copyright = copyright,
                            AlbumCover = GetAlbumCover(fileMetadata, CurrentFile, albumName),
                            PrimaryArtist = primaryArtist,
                        };
                    }
                    else
                    {
                        // found, take its data
                        CurrentAlbum = AllAlbumsModel[CurrentAlbumIndexInAllAlbums];
                    }
                    CurrentStep = CurrentAlbum.Name;
                }
                // the albumName didn't have a known primary artist but now has
                else if(CurrentAlbum.PrimaryArtist.Name == UNKNOWN && primaryArtist.Name != UNKNOWN)
                {
                    CurrentAlbum.PrimaryArtist = primaryArtist;
                }
                // the albumName has a different primary artists from the current one => may be a "various artist" albumName
                else if (primaryArtist.Name != UNKNOWN && !CurrentAlbum.PrimaryArtist.Name.Equals(primaryArtist.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    primaryArtistInAlbumCount++;
                    if (primaryArtistInAlbumCount == 3) // at least 3 artists needed to be categorized as Various TrackArtistRole
                    {
                        CurrentAlbum.IsVariousArtists = true;
                        if (variousArtistNotInDb)
                        {
                            await Artist.Insert(VariousArtistModel);
                            variousArtistNotInDb = false;
                            VariousArtistModel.ArtistRoles[0].Role = primaryArtistRole;
                        }
                        CurrentAlbum.PrimaryArtist = VariousArtistModel;
                    }
                }

                // update any new data for the albumName
                if (CurrentAlbum.ReleaseDate == 0)
                {
                    CurrentAlbum.ReleaseDate = year;
                }
                if (string.IsNullOrWhiteSpace(CurrentAlbum.Copyright))
                {
                    CurrentAlbum.Copyright = copyright;
                }
                if (string.IsNullOrWhiteSpace(CurrentAlbum.AlbumCover))
                {
                    CurrentAlbum.AlbumCover = GetAlbumCover(fileMetadata, CurrentFile, albumName);
                }


                // STEP 3: Handle Track related metadata
                //  - Create the new kvp model
                //      -> Try to find an artwork specific for this kvp in the folder
                //      -> Try to get the lyrics embedded in the kvp
                //  - Add the kvp to the lists and update the CurrentAlbum Length

                Track currentTrack = CreateTrackModel(fileMetadata);
                currentTrack.Artwork = ImageHelper.GetTrackCoverFromDirectory(DirectoryHelper.GetDirectory(CurrentFile), currentTrack.Title, CurrentAlbum.Name);
                string lyrics = fileMetadata.Tag.Lyrics;
                if(lyrics.IsNotNullOrWhiteSpace())
                {
                    string url = LyricsProcessor.Instance.GetFileName(currentTrack.Title, CurrentAlbum.PrimaryArtist.Name);

                    MusicPlay.Database.Models.Lyrics lyricsModel = await MusicPlay.Database.Models.Lyrics.TryToGetByUrl(url);

                    if(lyricsModel.IsNull())
                    {
                        lyricsModel ??= new()
                        {
                            LyricsText = lyrics,
                            IsSaved = true,
                            Url = url,
                        };
                        await MusicPlay.Database.Models.Lyrics.Insert(lyricsModel);
                    }
                    currentTrack.Lyrics = lyricsModel;
                    currentTrack.LyricsId = lyricsModel.Id;
                }

                CurrentAlbumNewTracks.Add(currentTrack, artistsCreditedInTrack);
                CurrentAlbum.Length += currentTrack.Length;

                // STEP 4 : Handle Genres / TrackTag
                //  - create new tags if they are not in the db
                //  - add the new tags to the albumName tags

                string[] tags = fileMetadata.Tag.Genres;
                foreach (string tag in tags)
                {
                    string formattedTag = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tag.Trim().FormatTag());

                    if (formattedTag.IsNullOrWhiteSpace())
                        continue;

                    if (!CurrentAlbum.AlbumTags.Any(at => at.Tag.Name.Equals(formattedTag, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        MusicPlay.Database.Models.Tag foundTag = AllTagsModel.Find(t => t.Name.Equals(formattedTag, StringComparison.CurrentCultureIgnoreCase));
                        if(foundTag == null)
                        {
                            foundTag = new(formattedTag);
                            await MusicPlay.Database.Models.Tag.Insert(foundTag);
                            AllTagsModel.Add(foundTag);
                        }

                        CurrentAlbum.AlbumTags.Add(new(foundTag, CurrentAlbum));
                        CurrentAlbumNewTags.Add(foundTag);
                    }
                }

                _fileImportedCount++;
                previousFile = CurrentFile;
            }

            // for the last albumName (it ends the loop without being inserter/updated)
            await InsertAlbum(CurrentAlbum, CurrentAlbumNewTracks, CurrentAlbumNewTags);
        }

        private Track CreateTrackModel(File fileMetadata)
        {
            string title = fileMetadata.Tag.Title;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = Path.GetFileNameWithoutExtension(CurrentFile); // use the file name as the title of the kvp
            }

            int length = (int)fileMetadata.Properties.Duration.TotalMilliseconds;
            int discNumber = (int)fileMetadata.Tag.Disc;
            int trackNumber = (int)fileMetadata.Tag.Track;

            return new Track()
            {
                Path = CurrentFile,
                Title = title,
                Length = length,
                DiscNumber = discNumber == 0 ? 1 : discNumber,
                TrackNumber = trackNumber,
                PlayCount = 0,
                Rating = 0,
                IsFavorite = false,
                FolderId = _folder.Id,
            };
        }

        private static bool IsSingle(Album album, List<Track> tracks)
        {
            if (tracks.Count > 3)
                return false;

            if (album.Length > 11 * 60 * 1000) // more than 11 minutes
                return false;

            return true;
        }

        private static bool IsEP(Album album, List<Track> tracks)
        {
            if (tracks.Count > 8)
                return false;

            if (album.Length < 30 * 60 * 1000) // less than 8 tracks and less than 30 minutes
                return true;

            string firstTrackTitle = tracks[0].Title;
            int similarTitleCount = 0;
            foreach (Track track in tracks)
            {
                if (track.Title.StartsWith(firstTrackTitle, StringComparison.OrdinalIgnoreCase))
                {
                    similarTitleCount++;
                }
            }

            if (album.Length - similarTitleCount * tracks[0].Length < 30 * 60 * 1000)
                return true;

            return false;
        }

        /// <summary>
        /// Insert or update the albumName, insert the new tracks and new tags relation
        /// </summary>
        /// <param name="album"></param>
        /// <param name="newTracks"></param>
        /// <param name="newTags"></param>
        private async Task InsertAlbum(Album album, Dictionary<Track, List<ArtistRole>> newTracks, List<MusicPlay.Database.Models.Tag> newTags)
        {
            if (string.IsNullOrWhiteSpace(album.Name))
                return; // invalid albumName name

            AlbumTypeEnum type = album.Type;
            List<Track> allTracks = [.. album.Tracks];
            allTracks.AddRange(newTracks.Keys);
            allTracks = [..allTracks.OrderBy(t => t.TrackNumber)];
            if (IsSingle(album, allTracks))
            {
                type = AlbumTypeEnum.Single; // allow 3 tracks for a single because there may be multiple versions => instrumental, remix...
            }
            else if (IsEP(album, allTracks)) // less than 30 minutes
            {
                type = AlbumTypeEnum.EP;
            }
            else
            {
                type = AlbumTypeEnum.Main;
            }

            // new Album
            if(album.Id == 0)
            {
                album.Type = type;
                var tags = album.AlbumTags;
                Artist primaryArtist = album.PrimaryArtist;
                album.PrimaryArtistId = primaryArtist.Id;
                album.AlbumTags = null;
                album.PrimaryArtist = null;
                await Album.Insert(album);
                await album.UpdateCoverWithFile(album.AlbumCover);
                album.PrimaryArtist = primaryArtist;
                album.AlbumTags = tags;

                if (album.PrimaryArtist.Cover.IsNullOrWhiteSpace())
                {
                    await Artist.UpdateCover(album.PrimaryArtist, album.AlbumCover);
                }
            }
            else if(album.Type != type) // update
            {
                await Album.Update(a => a.Type = type, album);
            }

            foreach (MusicPlay.Database.Models.Tag tag in newTags)
            {
                await MusicPlay.Database.Models.Tag.Add(tag, album);
            }

            foreach (var kvp in newTracks)
            {
                kvp.Key.AlbumId = album.Id;
                kvp.Key.FolderId = _folder.Id;
                MusicPlay.Database.Models.Lyrics lyrics = kvp.Key.Lyrics;
                kvp.Key.Lyrics = null;
                await Track.Insert(kvp.Key);

                if(kvp.Key.Artwork.IsNotNullOrWhiteSpace())
                {
                    await kvp.Key.UpdateCoverWithFile(kvp.Key.Artwork);
                }

                foreach (ArtistRole artistRole in kvp.Value)
                {
                    Artist artist = artistRole.Artist;
                    artistRole.Artist = null;
                    Role role = artistRole.Role;
                    artistRole.Role = null;
                    TrackArtistsRole trackArtistsRole = new (kvp.Key.Id, artistRole.Id);
                    using DatabaseContext context = new();
                    context.TrackArtistRoles.Local.Add(trackArtistsRole);
                    await context.SaveChangesAsync();
                    artistRole.Artist = artist;
                    artistRole.Role = role;
                    kvp.Key.TrackArtistRole.Add(trackArtistsRole);
                }
            }
        }

        private Dictionary<string, List<string>> GetArtists(File audioTags)
        {
            // artists Name => artists roles (primary Artist, Performer, producer, mixer, Arranger...)
            Dictionary<string, List<string>> artists = [];
            GetArtistsFromTagList([.. audioTags.Tag.AlbumArtists], artists, PrimaryArtistString);
            GetArtistsFromTagList([.. audioTags.Tag.Composers], artists, ComposerRole);
            GetArtistsFromTagList([.. audioTags.Tag.Performers], artists, PerformerRole);
            
            TagHelper.AddArtistsToDic([audioTags.Tag.Conductor], artists, "Conductor");
            TagHelper.AddArtistsToDic([audioTags.Tag.RemixedBy], artists, "Remixer");

            TagHelper.ReadDescription(audioTags.Properties.Description, artists);
            TagHelper.ReadDescription(audioTags.Tag.Comment, artists);
            TagHelper.ReadDescription(audioTags.Tag.Description, artists);
            return artists;
        }

        private void GetArtistsFromTagList(List<string> list, Dictionary<string, List<string>> artists, string defaultRole)
        {
            if (list.IsNullOrEmpty()) 
                return;

            List<string> commonRoles = ["artist", "composer", "performer", "producer", "conductor", "remixer"];
            // I have a "bug" with files that give multiple artist separated by a comma
            if (list.Any(p => p.Contains(','))) 
            {
                foreach (string data in list)
                {
                    // if the data contains some roles then it must be parsed the same way Descriptions are
                    if (commonRoles.Any(role => data.Contains(role, StringComparison.OrdinalIgnoreCase)))
                    {
                        TagHelper.ReadDescriptionLines(data.Split('\n', StringSplitOptions.RemoveEmptyEntries), artists, false);
                    } 
                    else
                    {
                        TagHelper.AddArtistsToDic([.. data.Split(',')], artists, defaultRole);
                    }
                }
            }
            else
            {
                TagHelper.AddArtistsToDic([.. list], artists, defaultRole);
            }

            return;
        }

        private static string GetAlbumCover(File audioTags, string file, string album)
        {
            SavePicture(audioTags.Tag.Pictures, album, DirectoryHelper.GetDirectory(file));
            // look for covers in the folder of the albumName
            return ImageHelper.GetAlbumCover(album, file);
        }

        /// <summary>
        /// Create a new image file in the <paramref name="folder"/> for every picture in <paramref name="pictures"/>.
        /// </summary>
        /// <param name="pictures"></param>
        /// <param name="albumName"></param>
        /// <param name="folder"></param>
        /// <returns>The new path of the first image in the list</returns>
        private static void SavePicture(IPicture[] pictures, string albumName, string folder)
        {
            if (pictures.IsNullOrEmpty())
                return;

            for (int i = 0; i < pictures.Length; i++)
            {
                string ImgPath = Path.Combine(folder, $"{ImageHelper.FormatFileNameForCoverSearch(albumName)}_{i + 1}_Cover.png");
                if (System.IO.File.Exists(ImgPath))
                    continue;

                try
                {
                    using (var ms = new MemoryStream(pictures[i].Data.Data))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        image.Save(ImgPath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Error while saving a cover: {ex.Message}"));
                    }));
                }
            }
        }
    }
}
