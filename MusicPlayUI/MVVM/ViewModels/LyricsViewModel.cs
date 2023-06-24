using DataBaseConnection.DataAccess;
using MessageControl;
using MusicFilesProcessor.Helpers;
using MusicFilesProcessor.Lyrics;
using MusicFilesProcessor.Models;
using MusicPlay.Language;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Input;
using Clipboard = System.Windows.Clipboard;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class LyricsViewModel : ViewModel
    {
        private readonly IQueueService _queueService;
        private readonly IAudioTimeService _audioService;
        private readonly LyricsProcessor _lyricsProcessor;

        private string SavedLyrics { get; set; }

        private string _lyrics = "";
        public string Lyrics
        {
            get { return _lyrics; }
            set
            {
                if (IsSaved && value != SavedLyrics)
                {
                    IsSaved = false;
                }
                else if (value == SavedLyrics && !string.IsNullOrWhiteSpace(SavedLyrics))
                {
                    IsSaved = true;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    CanSave = true;
                    CanCopy = true;
                    if (!LyricsFound)
                    {
                        LyricsModel.IsFromUser = true;
                    }
                }
                else
                {
                    CanSave = false;
                    CanCopy = false;
                }
                _lyrics = value;
                OnPropertyChanged(nameof(Lyrics));
            }
        }

        private bool _isTimed;
        public bool IsTimed
        {
            get { return _isTimed; }
            set
            {
                _isTimed = value;
                OnPropertyChanged(nameof(IsTimed));
            }
        }

        private LyricsModel _lyricsModel = new();
        public LyricsModel LyricsModel
        {
            get { return _lyricsModel; }
            set
            {
                _lyricsModel = value;
                OnPropertyChanged(nameof(LyricsModel));
            }
        }

        private ObservableCollection<TimedLyricsLineModel> _timedLyrics = new();
        public ObservableCollection<TimedLyricsLineModel> TimedLyrics
        {
            get { return _timedLyrics; }
            set
            {
                _timedLyrics = value;
                OnPropertyChanged(nameof(TimedLyrics));
            }
        }

        private TimedLyricsLineModel _playingTimedLyrics;
        public TimedLyricsLineModel PlayingTimedLyrics
        {
            get { return _playingTimedLyrics; }
            set
            {
                _playingTimedLyrics = value;
                OnPropertyChanged(nameof(PlayingTimedLyrics));
            }
        }

        private int _fontSize = ConfigurationService.GetPreference(SettingsEnum.LyricsFontSize);
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
                ConfigurationService.SetPreference(SettingsEnum.LyricsFontSize, value.ToString());
            }
        }

        private string _lyricsFilePath = "";
        public string LyricsFilePath
        {
            get { return _lyricsFilePath; }
            set
            {
                _lyricsFilePath = value;
                OnPropertyChanged(nameof(LyricsFilePath));
            }
        }

        private string _websiteSource;
        public string WebsiteSource
        {
            get { return _websiteSource; }
            set
            {
                _websiteSource = value;
                OnPropertyChanged(nameof(WebsiteSource));
            }
        }

        private string _url;
        public string URL
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged(nameof(URL));
            }
        }

        private bool _isURLValid = false;
        public bool IsURLValid
        {
            get { return _isURLValid; }
            set
            {
                _isURLValid = value;
                OnPropertyChanged(nameof(IsURLValid));
            }
        }


        private bool _autoSave = false;
        public bool AutoSave
        {
            get { return _autoSave; }
            set
            {
                _autoSave = value;
                OnPropertyChanged(nameof(AutoSave));
            }
        }

        private bool _isSaved = false;
        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                _isSaved = value;
                OnPropertyChanged(nameof(IsSaved));
            }
        }

        private bool _canSave = false;
        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                _canSave = value;
                OnPropertyChanged(nameof(CanSave));
            }
        }


        private bool _canCopy = false;
        public bool CanCopy
        {
            get { return _canCopy; }
            set
            {
                _canCopy = value;
                OnPropertyChanged(nameof(CanCopy));
            }
        }

        private bool _lyricsFound = false;
        public bool LyricsFound
        {
            get { return _lyricsFound; }
            set
            {
                _lyricsFound = value;
                OnPropertyChanged(nameof(LyricsFound));
            }
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
            }
        }

        private bool _isTimedEditMode = false;
        public bool IsTimedEditMode
        {
            get { return _isTimedEditMode; }
            set
            {
                _isTimedEditMode = value;
                OnPropertyChanged(nameof(IsTimedEditMode));
            }
        }

        public int CurrentPosition
        {
            get => _audioService.CurrentPositionMs;
        }

        public ICommand SaveCommand { get; }
        public ICommand EnterLeaveEditModeCommand { get; }
        public ICommand EnterLeaveTimedLyricsCommand { get; }
        public ICommand SaveTimedLyricsCommand { get; }
        public ICommand DeleteTimedLyricsCommand { get; }
        public ICommand SetLengthCommand { get; }
        public ICommand GetLyricsCommand { get; }
        public ICommand PasteLyricsCommand { get; }
        public ICommand CopyLyricsCommand { get; }
        public ICommand OpenLyricsWebPageCommand { get; }
        public LyricsViewModel(IQueueService queueService, IAudioTimeService audioService)
        {
            _queueService = queueService;
            _audioService = audioService;

            _queueService.PlayingTrackChanged += OnPlayingTrackChanged;
            _audioService.CurrentPositionChanged += OnCurrentAudioPositionChanged;

            _lyricsProcessor = new LyricsProcessor();

            SaveCommand = new RelayCommand(() => Task.Run(SaveLyrics));
            GetLyricsCommand = new RelayCommand(() => Task.Run(GetLyrics));

            EnterLeaveEditModeCommand = new RelayCommand(EnterLeaveEditMode);

            EnterLeaveTimedLyricsCommand = new RelayCommand(() =>
            {
                if (IsEditMode) // Leave edit mode
                {
                    IsEditMode = false;
                    IsTimedEditMode = false;
                }

                if (LyricsModel.IsTimed) // Timed lyrics exist and habe been found
                {
                    IsTimed = !IsTimed; // Enter or leave timed lyrics
                    if (!IsTimed && string.IsNullOrWhiteSpace(LyricsModel.Lyrics)) // Get the lyrics
                    {
                        LyricsModel = _lyricsProcessor.GetLyrics(LyricsModel, false);
                        Lyrics = LyricsModel.Lyrics;
                        SavedLyrics = Lyrics;
                    }
                }
                else // Timed lyrics have not been fetched or they doesn't exist
                {
                    if (_lyricsProcessor.TimedLyricsExists(LyricsModel.FileName)) // if Timed lyrics exists (file found)
                    {
                        LyricsModel = _lyricsProcessor.GetLyrics(LyricsModel, true);
                        TimedLyrics = new(LyricsModel.TimedLyrics);
                        TimedLyrics[0].IsPlaying = true; // set the fisrt line to the one playing
                        IsTimed = true;
                    }
                    else if (Lyrics != Resources.No_Lyrics_Found && !string.IsNullOrWhiteSpace(Lyrics)) // If the lyrics are valid
                    {
                        Task.Run(ConvertToTimedLyrics); // Create an "empty" List<TimedLyricsLineModel> (all lengths are 0)
                        IsTimed = true; // enter timed lyrics
                        IsEditMode = true; // Enter timed edit mode
                        IsTimedEditMode = true;
                    }

                }
            });

            SaveTimedLyricsCommand = new RelayCommand(() =>
            {
                foreach (TimedLyricsLineModel t in TimedLyrics)
                {
                    if (t.LengthInMilliseconds == 0 && !string.IsNullOrWhiteSpace(t.Time)) // If the time has been changed but not the Length
                    {
                        t.LengthInMilliseconds = (int)TimeSpan.Parse(t.Time.FormatStringToTime()).TotalMilliseconds;
                    }
                }
                LyricsModel.TimedLyrics = TimedLyrics.ToList();

                IsTimed = true;
                IsTimedEditMode = false; // Leave timed edit mode to save
                SaveLyrics();
            });

            DeleteTimedLyricsCommand = new RelayCommand(() =>
            {
                if(TimedLyrics.Count > 0)
                {
                    LyricsModel.TimedLyrics.Clear();
                    IsTimed = false;

                    // reset timestamp
                    foreach (TimedLyricsLineModel line in TimedLyrics)
                    {
                        line.LengthInMilliseconds = 0;
                        line.Time = "";
                    }

                    // delete file
                    _lyricsProcessor.DeleteTimedLyrics(_queueService.PlayingTrack.Title, _queueService.PlayingTrack.GetAlbumArtist().Name);
                }
                IsEditMode = false; // leave edit mode anyway
            });

            SetLengthCommand = new RelayCommand<TimedLyricsLineModel>((l) =>
            {
                if (l is not null)
                {
                    int currentLength = CurrentPosition;
                    TimedLyrics[l.index].LengthInMilliseconds = currentLength;
                    TimedLyrics[l.index].Time = TimeSpan.FromMilliseconds(currentLength).ToFormattedString();
                }
            });

            OpenLyricsWebPageCommand = new RelayCommand(() =>
            {
                try
                {
                    Process.Start(URL);
                }
                catch(Exception) 
                {
                    Process.Start(new ProcessStartInfo(URL.Replace("&", "^&")) { UseShellExecute = true });
                }
            });

            CopyLyricsCommand = new RelayCommand(() =>
            {
                Clipboard.SetText(Lyrics);
            });

            PasteLyricsCommand = new RelayCommand(() =>
            {
                Lyrics = Clipboard.GetText();
                LyricsModel.Lyrics = Lyrics;
                LyricsModel.IsFromUser = true;
                LyricsModel.IsSaved = false;
                IsSaved = false;
            });

            OnPlayingTrackChanged();
        }

        private void ConvertToTimedLyrics()
        {
            LyricsModel.TimedLyrics = new(_lyricsProcessor.ConvertToTimedLyricsModel(Lyrics, true));
            LyricsModel.IsTimed = true;
            IsTimed = true;
            TimedLyrics = new(LyricsModel.TimedLyrics);
            AddEmptyLineToLyrics();
        }

        public override void Dispose()
        {
            _audioService.CurrentPositionChanged -= OnCurrentAudioPositionChanged;
            _queueService.PlayingTrackChanged -= OnPlayingTrackChanged;
            GC.SuppressFinalize(this);
        }

        private void EnterLeaveEditMode()
        {
            if (IsTimed) // Enter or leave timed edit mode and set edit mode to the corresponding value
            {
                IsTimedEditMode = !IsTimedEditMode;
                IsEditMode = IsTimedEditMode; // Edit Mode
            }
            else // Enter or Leave normal edit mode and set timed edit mode to false
            {
                IsEditMode = !IsEditMode;
                IsTimedEditMode = false;
            }
        }

        private void SaveLyrics()
        {
            if (IsTimedEditMode) // don't save unfinished timed lyrics
            {
                MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("You need to save manually the timed lyrics."));
                return;
            }

            LyricsModel.Lyrics = Lyrics;
            _lyricsProcessor.SaveLyrics(LyricsModel, IsTimed);
            SavedLyrics = Lyrics;

            IsSaved = true;
            IsEditMode = false; // Escape edit mode
        }

        private async void GetLyrics()
        {
            if (_queueService.PlayingTrack is null) return;

            LyricsModel = await _lyricsProcessor.GetLyrics(_queueService.PlayingTrack.Title, _queueService.PlayingTrack.GetAlbumArtist().Name, _queueService.PlayingTrack.Path);

            IsTimed = LyricsModel.IsTimed;
            IsSaved = LyricsModel.IsSaved;
            OnPropertyChanged(nameof(IsSaved));
            IsURLValid = !string.IsNullOrWhiteSpace(LyricsModel.URL);

            WebsiteSource = LyricsModel.WebSiteSource;
            URL = LyricsModel.URL;

            _lyrics = LyricsModel.Lyrics;
            SavedLyrics = Lyrics;
            OnPropertyChanged(nameof(Lyrics));
            TimedLyrics = new(LyricsModel.TimedLyrics);
            AddEmptyLineToLyrics();

            LyricsFound = IsSaved || IsURLValid;
            CanSave = LyricsFound;
            CanCopy = !string.IsNullOrWhiteSpace(Lyrics) || Lyrics != Resources.No_Lyrics_Found;
        }

        private void AddEmptyLineToLyrics()
        {
            if(TimedLyrics == null || TimedLyrics.Count == 0) return;

            for (int i = 0; i < 4; i++)
            {
                int index = TimedLyrics.Last().index + i + 1;
                TimedLyrics.Add(new()
                {
                    Lyrics = "  ",
                    index = index,
                    LengthInMilliseconds = TimedLyrics.Last().LengthInMilliseconds + index * 2000,
                    IsPlaying = false,
                });
            }
        }


        private void OnCurrentAudioPositionChanged()
        {
            if (IsTimed && !IsTimedEditMode)
            {
                int currentLength = CurrentPosition;
                bool found = false;
                for (int i = 0; i < TimedLyrics.Count; i++)
                {
                    TimedLyrics[i].IsPlaying = false;
                    if (!found)
                    {
                        if (i < TimedLyrics.Count - 1 && currentLength > TimedLyrics[i].LengthInMilliseconds && 
                            currentLength < TimedLyrics[i + 1].LengthInMilliseconds)
                        {
                            PlayingTimedLyrics = TimedLyrics[i];
                            TimedLyrics[i].IsPlaying = true;
                            found = true;
                        }
                        else if (i == 0 && CurrentPosition <= TimedLyrics[0].LengthInMilliseconds)
                        {
                            PlayingTimedLyrics = TimedLyrics[i];
                            TimedLyrics[i].IsPlaying = true;
                            found = true;
                        }
                        else if (i == TimedLyrics.Count - 1 && CurrentPosition >= TimedLyrics[i].LengthInMilliseconds)
                        {
                            PlayingTimedLyrics = TimedLyrics[i];
                            TimedLyrics[i].IsPlaying = true;
                        }
                    }
                }
            }
        }

        private void OnPlayingTrackChanged()
        {
            if (!IsEditMode)
            {
                IsTimedEditMode = false;
                Task.Run(GetLyrics);
            }
        }
    }
}
