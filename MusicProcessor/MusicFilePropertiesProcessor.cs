using System.IO;

namespace MusicFilesProcessor
{
    public class MusicFilePropertiesProcessor
    {
        private string _file;
        private TagLib.File tag;

        public string FileName
        {
            get => Path.GetFileName(_file);
        }

        public int AudioBitrate
        {
            get => tag.Properties.AudioBitrate;
        }

        public int AudioChannels
        {
            get => tag.Properties.AudioChannels;
        }

        public int AudioSampleRate
        {
            get => tag.Properties.AudioSampleRate;
        }

        public int BitsPerSample
        {
            get => tag.Properties.BitsPerSample;
        }

        public string FileExt
        {
            get => tag.Properties.Codecs.First().Description;
        }

        public string AlbumArtist
        {
            get => tag.Tag.AlbumArtists.First();
            set
            {
                string[] albumArtist = tag.Tag.AlbumArtists;
                if(albumArtist.Length > 0)
                {
                    albumArtist[0] = value;
                }
                else
                {
                    albumArtist = new string[1] { value };
                }

                tag.Tag.AlbumArtists = albumArtist;
            }
        }

        public string Performer
        {
            get => tag.Tag.Performers.First();
            set
            {
                string[] performers = tag.Tag.Performers;
                if (performers.Length > 0)
                {
                    performers[0] = value;
                }
                else
                {
                    performers = new string[1] { value };
                }

                tag.Tag.Performers = performers;
            }
        }

        public int Year
        {
            get => (int)tag.Tag.Year;
            set 
            {
                tag.Tag.Year = (uint)value;
            }
        }

        public int TrackNumber
        {
            get => (int)tag.Tag.Track;
            set
            {
                tag.Tag.Track = (uint)value;
            }
        }

        public int DiscNumber
        {
            get => (int)tag.Tag.Disc;
            set
            {
                tag.Tag.Disc = (uint)value;
            }
        }

        public string[] Genres
        {
            get => tag.Tag.Genres;
            set
            {
                tag.Tag.Genres = value;
            }
        }


        public MusicFilePropertiesProcessor(string file)
        {
            if (File.Exists(file))
            {
                _file = file;
                tag = TagLib.File.Create(_file);
                
            }
        }

        public void ChangeFile(string file)
        {
            if (File.Exists(file))
            {
                _file = file;
                tag = TagLib.File.Create(_file);
            }
        }


        public void Save()
        {
            tag.Save();
        }
    }
}
