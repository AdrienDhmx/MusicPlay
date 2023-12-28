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
using MusicPlayModels.Enums;

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

        private readonly string PrimaryArtistString = "Primary Artist";


        private List<string> files = new();
        private readonly List<string> importedFiles = new();

        private List<ArtistModel> AllArtistsModel { get; set; } = new();
        private List<AlbumModel> AllAlbumsModel { get; set; } = new();
        private List<TrackModel> AllTracksModel { get; set; } = new();
        private List<TagModel> AllTagsModel { get; set; } = new();

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
            List<TrackModel> tracks = await DataAccess.Connection.GetAllTracks();
            int deletedTrack = 0;

            foreach (TrackModel track in tracks)
            {
                if (!System.IO.File.Exists(track.Path))
                {
                    await DataAccess.Connection.DeleteTrack(track.Id);
                    CoverProcessor.DeleteCover(track.Artwork); // try to delete artwork if there is one

                    var albumTracks = await DataAccess.Connection.GetTracksFromAlbum(track.Album.Id);
                    if (albumTracks.Count == 0)
                    {
                        await DataAccess.Connection.DeleteAlbum(track.Album.Id);
                    }

                    foreach (TrackArtistsRoleModel relation in track.Artists)
                    {
                        var albums = await DataAccess.Connection.GetAlbumsFromArtist(relation.Artist.Id);
                        var artistTracks = await DataAccess.Connection.GetTracksFromArtist(relation.Artist.Id);

                        ArtistModel artist = await DataAccess.Connection.GetArtist(relation.Artist.Id);
                    
                        if (albums.Count == 0 && artistTracks.Count == 0) // delete artist
                        {
                            await DataAccess.Connection.DeleteArtist(relation.Artist.Id);

                            CoverProcessor.DeleteCover(artist.Cover);
                        }
                        else if (albums.Count == 0) // artist is not an albumArtist
                        {
                            //if (artist.IsAlbumArtist)
                            //{
                            //    artist.IsAlbumArtist = false;
                            //    await DataAccess.Connection.UpdateArtist(artist); // try to delete the artist cover
                            //}
                        }
                        else if(artistTracks.Count == 0) // artist is not a Performer anymore
                        {
                            //artist.IsPerformer = false;
                            //await DataAccess.Connection.UpdateArtist(artist);
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
            TotalProgress = FileNumber * 2; // scan all files twice and then update all artists
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

        private async void GetImportedFiles() 
        {
            AllTracksModel = await DataAccess.Connection.GetAllTracks();
            foreach(TrackModel track in AllTracksModel)
            {
                importedFiles.Add(track.Path);
            }
            AllArtistsModel = await DataAccess.Connection.GetAllArtists();
        } 

        private int GetFiles()
        {
            files = Directory.EnumerateFiles(_folder, "*.*", SearchOption.AllDirectories).ToList().Where(s => FilesExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();
            files = files.Except(importedFiles).Distinct().ToList();
            return files.Count;
        }

        private async void ScanFiles()
        {
            // get all the existing data (the artists and tracks were already fetch in GetImportedFiles())
            AllAlbumsModel = await DataAccess.Connection.GetAllAlbums();
            AllTagsModel = await DataAccess.Connection.GetAllTags();

            bool unknownArtistNotInDb = false;
            bool variousArtistNotInDb = false;
            // find if exist or create an Unknown Artists model in case a primary artist does not exist for an album
            ArtistModel UnknownArtistModel = AllArtistsModel.Find(a => a.Name == UNKNOWN);
            if(UnknownArtistModel == null)
            {
                unknownArtistNotInDb = true;
                UnknownArtistModel = new ArtistModel()
                {
                    Name = UNKNOWN,
                    Roles = new() { new(PrimaryArtistString) }
                };
            }

            // find if exist or create a Various Artists model in case an album has multiple primary artists
            ArtistModel VariousArtistModel = AllArtistsModel.Find(a => a.Name == VARIOUSARTIST);
            if (VariousArtistModel == null)
            {
                variousArtistNotInDb = true;
                VariousArtistModel ??= new ArtistModel()
                {
                    Name = VARIOUSARTIST,
                    Roles = new() { new(PrimaryArtistString) }
                };
            }

            // variables to store the currently scanned album data
            // it supposed that the each album has its own folder that contains the tracks within it
            AlbumModel CurrentAlbum = new();
            int CurrentAlbumIndexInAllAlbums = -1;
            List<TagModel> CurrentAlbumNewTags = new();
            List<TrackModel> CurrentAlbumNewTracks = new();

            int primaryArtistInAlbumCount = 0;
            string previousFile = "";
            for (int i = 0; i < files.Count; i++)
            {
                CurrentFile = files[i];
                Progress++;

                // get the metadata
                File fileMetadata = File.Create(CurrentFile);

                // STEP 1: Handle Artists metadata
                //  - Find all the artists credited and their role
                //  - create or update the artists

                // get the artists and their role
                Dictionary<string, List<string>> artistsCreditedDic = GetArtists(fileMetadata);

                // keep track of all the artist credited in this track with their specific role
                List<TrackArtistsRoleModel> artistsCreditedInTrack = new();
                ArtistModel primaryArtist = null;

                // loop through all found artists to create or update them
                foreach (KeyValuePair<string, List<string>> kvp in artistsCreditedDic)
                {
                    ArtistModel foundArtist = AllArtistsModel.Find(a => a.Name.ToLower() == kvp.Key.ToLower());
                    bool isPrimaryArtist = false;
                    bool needUpdate = false;
                    List<ArtistRoleModel> artistRoles = new List<ArtistRoleModel>();

                    // new artist => create it and insert it in the db
                    if (foundArtist == null)
                    {
                        foundArtist = new ArtistModel();
                        foundArtist.Name = kvp.Key;
                        foundArtist.Id = DataAccess.Connection.InsertArtist(foundArtist);
                        AllArtistsModel.Add(foundArtist);
                    }

                    // add the new roles if any
                    foreach (string role in kvp.Value)
                    {
                        artistRoles.Add(new ArtistRoleModel(role));
                        if (role == PrimaryArtistString)
                        {
                            isPrimaryArtist = true;
                        }

                        if (!foundArtist.Roles.Any(a => a.Role.ToLower() == role.ToLower()))
                        {
                            needUpdate = true;
                            foundArtist.Roles.Add(new(role));
                        }
                    }
                    artistsCreditedInTrack.Add(new(foundArtist, artistRoles));

                    // new roles added
                    if (needUpdate)
                    {
                        // TODO changing the update artist to update the roles instead
                        await DataAccess.Connection.UpdateArtist(foundArtist);
                    }

                    // Keep track of the primary artists to later compare it with
                    // the primary artist of the CurrentAlbum
                    if(isPrimaryArtist)
                    {
                        primaryArtist = foundArtist;
                    }
                }
                // primary artist not found
                primaryArtist ??= UnknownArtistModel;

                // STEP 2: Handle Album metadata
                //  - Find if the album has changed
                //  -> changed
                //      - Create or update the CurrentAlbum in the db
                //      - Insert all the tracks from this album in the db
                //      - Create the new Album model
                //          -> get the main info (title, release date, copyright)
                //          -> try to get the cover embedded or in the folder
                //  -> NOT changed
                //      - update the CurrentAlbum primary artists if needed
                //  - update the CurrentAlbum data if new information is found

                string albumName = fileMetadata.Tag.Album;
                if (string.IsNullOrWhiteSpace(albumName))
                {   // use the folder name containing the file as the album name
                    albumName = DirectoryHelper.GetFolderName(CurrentFile); 
                }

                int year = (int)fileMetadata.Tag.Year;
                string copyright = fileMetadata.Tag.Copyright;

                // The Album has changed
                if(CurrentAlbum.Name != albumName && CurrentFile.GetDirectory() != previousFile.GetDirectory())
                {
                    // make sure the UNKNOWN and VARIOUS ARTISTS artists models are inserted in the db if used
                    if(CurrentAlbum.PrimaryArtist.Name == UNKNOWN && unknownArtistNotInDb)
                    {
                        CurrentAlbum.PrimaryArtist.Id = DataAccess.Connection.InsertArtist(CurrentAlbum.PrimaryArtist);
                        UnknownArtistModel.Id = CurrentAlbum.PrimaryArtist.Id;
                        unknownArtistNotInDb = false;
                    }
                    else if (CurrentAlbum.PrimaryArtist.Name == VARIOUSARTIST && variousArtistNotInDb)
                    {
                        CurrentAlbum.PrimaryArtist.Id = DataAccess.Connection.InsertArtist(CurrentAlbum.PrimaryArtist);
                        VariousArtistModel.Id = CurrentAlbum.PrimaryArtist.Id;
                        variousArtistNotInDb = false;
                    }

                    // insert or update the album
                    InsertAlbum(CurrentAlbum, CurrentAlbumNewTracks, CurrentAlbumNewTags);
                    AllAlbumsModel.Add(CurrentAlbum);

                    // reset some variables for the new album
                    CurrentAlbumNewTags = new();
                    CurrentAlbumNewTracks = new();
                    primaryArtistInAlbumCount = 0;


                    // try to find if the new currently scanned album is already in the db
                    CurrentAlbumIndexInAllAlbums = AllAlbumsModel.FindIndex(a => 
                            a.Name.ToLower() == albumName && 
                            a.PrimaryArtist.Name.ToLower() == primaryArtist.Name.ToLower()
                        );
                    if(CurrentAlbumIndexInAllAlbums == -1)
                    {
                        // not found, Create a new album 
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

                }
                // the album didn't have a known primary artist but now has
                else if(CurrentAlbum.PrimaryArtist.Name == PrimaryArtistString && primaryArtist.Name != PrimaryArtistString)
                {
                    CurrentAlbum.PrimaryArtist = primaryArtist;
                }
                // the album has a different primary artists from the current one => may be a "various artist" album
                else if (primaryArtist.Name != PrimaryArtistString && CurrentAlbum.PrimaryArtist.Name.ToLower() != primaryArtist.Name.ToLower())
                {
                    primaryArtistInAlbumCount++;
                    if (primaryArtistInAlbumCount == 3) // at least 3 artists needed to be categorized as Various Artists
                    {
                        CurrentAlbum.VariousArtists = true;
                        CurrentAlbum.PrimaryArtist = VariousArtistModel;
                    }
                }

                // update any new data for the album
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
                //  - Create the new track model
                //      -> Try to find an artwork specific for this track in the folder
                //      -> Try to get the lyrics embedded in the track
                //  - Add the track to the lists and update the CurrentAlbum Length

                TrackModel currentTrack = CreateTrackModel(fileMetadata);
                currentTrack.Artists = artistsCreditedInTrack;
                currentTrack.Artwork = ImageHelper.GetTrackCoverFromDirectory(DirectoryHelper.GetDirectory(CurrentFile), currentTrack.Title, CurrentAlbum.Name);
                string lyrics = fileMetadata.Tag.Lyrics;
                if(!string.IsNullOrWhiteSpace(lyrics))
                {
                    LyricsModel lyricsModel = new()
                    {
                        Lyrics = lyrics,
                        IsFromUser = true,
                        IsSaved = true,
                        IsTimed = false,
                    };
                    currentTrack.Lyrics = lyricsModel;
                }

                CurrentAlbumNewTracks.Add(currentTrack);
                CurrentAlbum.Tracks.Add(currentTrack);
                CurrentAlbum.Length += currentTrack.Length;

                // STEP 4 : Handle Genres / Tags
                //  - create new tags if they are not in the db
                //  - add the new tags to the album tags

                string[] tags = fileMetadata.Tag.Genres;
                foreach (string tag in tags)
                {
                    string formattedTag = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tag.Trim().FormatTag());

                    if (!CurrentAlbum.Tags.Any(t => t.Name.ToLower() == formattedTag.ToLower()))
                    {
                        TagModel foundTag = AllTagsModel.Find(t => t.Name.ToLower() == formattedTag.ToLower());
                        if(foundTag == null)
                        {
                            foundTag = new()
                            {
                                Name = formattedTag,
                            };
                            foundTag.Id = DataAccess.Connection.InsertTag(foundTag);
                        }

                        CurrentAlbum.Tags.Add(foundTag);
                        CurrentAlbumNewTags.Add(foundTag);
                    }
                }

                _fileImportedCount++;
                previousFile = CurrentFile;
            }

            // for the last album (it ends the loop without being inserter/updated)
            InsertAlbum(CurrentAlbum, CurrentAlbumNewTracks, CurrentAlbumNewTags);
        }

        private TrackModel CreateTrackModel(File fileMetadata)
        {
            string title = fileMetadata.Tag.Title;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = Path.GetFileNameWithoutExtension(CurrentFile); // use the file name as the title of the track
            }

            string duration = TagHelper.ToFormattedString(fileMetadata.Properties.Duration);
            int length = (int)fileMetadata.Properties.Duration.TotalMilliseconds;
            int discNumber = (int)fileMetadata.Tag.Disc;
            int trackNumber = (int)fileMetadata.Tag.Track;

            return new()
            {
                Path = CurrentFile,
                Title = title,
                Duration = duration,
                Length = length,
                DiscNumber = discNumber == 0 ? 1 : discNumber,
                TrackNumber = trackNumber,
                PlayCount = 0,
                Rating = 0,
                LastPlayed = DateTime.MinValue,
                IsFavorite = false,
            };
        }

        /// <summary>
        /// Insert or update the album, insert the new tracks and new tags relation
        /// </summary>
        /// <param name="album"></param>
        /// <param name="newTracks"></param>
        /// <param name="newTags"></param>
        private void InsertAlbum(AlbumModel album, List<TrackModel> newTracks, List<TagModel> newTags)
        {
            if (string.IsNullOrWhiteSpace(album.Name))
                return; // invalid album name

            if (album.Tracks.Count < 4 && album.Length < 11 * 60 * 1000) // less than 11 minutes
            {
                album.Type = AlbumType.Single; // max 3 tracks for a single because there may be multiple versions => instrumental, remix...
            }
            else if (album.Tracks.Count > 4 && album.Tracks.Count < 8 && album.Length < 30 * 60 * 1000) // less than 30 minutes
            {
                album.Type = AlbumType.EP;
            }
            else
            {
                album.Type = AlbumType.Main;
            }

            // new Album
            if(album.Id == -1)
            {
                // insert the model in the database and get its id
                album.Id = DataAccess.Connection.InsertAlbum(album);
            }
            else
            {
                // the fact that this method is called means that there are new tracks and consequently the album needs to be updated
                DataAccess.Connection.UpdateAlbum(album);
            }

            // insert new tracks to db
            foreach (TrackModel track in newTracks)
            {
                track.Album = album;
                track.Id = DataAccess.Connection.InsertTrack(track);
                AllTracksModel.Add(track);
            }

            foreach (TagModel tag in newTags)
            {
                DataAccess.Connection.InsertAlbumTag(album.Id, tag.Id);
            }
        }

        private static Dictionary<string, List<string>> GetArtists(File audioTags)
        {
            // artists Name => artists roles (primary Artist, Performer, producer, mixer, Arranger...)
            Dictionary<string, List<string>> artists = new Dictionary<string, List<string>>();

            TagHelper.AddArtistsToDic(audioTags.Tag.AlbumArtists.ToList(), artists, "Primary Artist");
            TagHelper.AddArtistsToDic(audioTags.Tag.Composers.ToList(), artists, "Composer");
            TagHelper.AddArtistsToDic(audioTags.Tag.Performers.ToList(), artists, "Performer");
            
            TagHelper.AddArtistsToDic(new List<string>() { audioTags.Tag.Conductor }, artists, "Conductor");
            TagHelper.AddArtistsToDic(new List<string>() { audioTags.Tag.RemixedBy }, artists, "Remixer");

            TagHelper.ReadDescription(audioTags.Properties.Description, artists);
            TagHelper.ReadDescription(audioTags.Tag.Description, artists);
            TagHelper.ReadDescription(audioTags.Tag.Comment, artists);

            return artists;
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
    }
}
