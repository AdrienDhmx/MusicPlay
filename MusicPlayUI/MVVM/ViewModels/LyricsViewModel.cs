using DynamicScrollViewer;
using MessageControl;
using MusicFilesProcessor.Helpers;
using MusicFilesProcessor.Lyrics;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlay.Language;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Clipboard = System.Windows.Clipboard;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class LyricsViewModel : ViewModel
    {
        private readonly IQueueService _queueService;
        private readonly IAudioTimeService _audioService;
        private readonly LyricsProcessor _lyricsProcessor;

        private IVisualizerParameterStore _visualizerParameterService;
        public IVisualizerParameterStore VisualizerParameterService
        {
            get => _visualizerParameterService;
            set { SetField(ref _visualizerParameterService, value); }
        }

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

        private Lyrics _lyricsModel = new();
        public Lyrics LyricsModel
        {
            get { return _lyricsModel; }
            set
            {
                _lyricsModel = value;
                OnPropertyChanged(nameof(LyricsModel));
            }
        }

        private ObservableCollection<TimedLyricsLine> _timedLyrics = new();
        public ObservableCollection<TimedLyricsLine> TimedLyrics
        {
            get { return _timedLyrics; }
            set
            {
                _timedLyrics = value;
                OnPropertyChanged(nameof(TimedLyrics));
            }
        }

        private TimedLyricsLine _playingTimedLyrics;
        public TimedLyricsLine PlayingTimedLyrics
        {
            get { return _playingTimedLyrics; }
            set
            {
                _playingTimedLyrics = value;
                OnPropertyChanged(nameof(PlayingTimedLyrics));
            }
        }

        private bool _settingsVisibility = false;
        public bool SettingsVisibility
        {
            get => _settingsVisibility;
            set
            {
                SetField(ref _settingsVisibility, value);
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

        private bool _autoForeground = ConfigurationService.GetPreference(SettingsEnum.AutoForeground) == 1;
        public bool IsAutoForeground
        {
            get => _autoForeground;
            set
            {
                SetField(ref _autoForeground, value);
                ConfigurationService.SetPreference(SettingsEnum.AutoForeground, value ? "1" : "0");
            }
        }
        
        public int CurrentPosition
        {
            get => _audioService.CurrentPositionMs;
        }

        private int _wasControlledScrolling = 0;

        private bool _controlledScrolling = false;
        public bool ControlledScrolling
        {
            get => _controlledScrolling;
            set => SetField(ref _controlledScrolling, value);
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
        public ICommand ToggleAutoForegroundCommand { get; }
        public ICommand ToggleSettingsVisibilityCommand { get; }
        public ICommand SyncScrollWithLyricsCommand { get; }
        public LyricsViewModel(IQueueService queueService, IAudioTimeService audioService, IVisualizerParameterStore visualizerParameterStore)
        {
            _queueService = queueService;
            _audioService = audioService;
            VisualizerParameterService = visualizerParameterStore;

            _queueService.PlayingTrackChanged += OnPlayingTrackChanged;
            _audioService.CurrentPositionChanged += OnCurrentAudioPositionChanged;

            _lyricsProcessor = LyricsProcessor.Instance;

            SaveCommand = new RelayCommand(() => Task.Run(SaveLyrics));
            GetLyricsCommand = new RelayCommand(() => Task.Run(() => GetLyrics()));

            EnterLeaveEditModeCommand = new RelayCommand(EnterLeaveEditMode);

            EnterLeaveTimedLyricsCommand = new RelayCommand(async () =>
            {
                IsTimed = !IsTimed; // Enter or leave timed lyrics
                if (IsEditMode) // Leave edit mode
                {
                    IsEditMode = false;
                    IsTimedEditMode = false;

                    // leaving timed mode as well
                    if (!IsTimed)
                    {
                        TimedLyrics = new(); // reset not saved changes
                        LyricsModel.TimedLines.Clear();

                        // we don't have the normal lyrics, try to get them
                        if(string.IsNullOrWhiteSpace(LyricsModel.LyricsText))
                        {
                            await GetLyrics();
                        }
                        
                        return;
                    }
                }

                if (LyricsModel.HasTimedLyrics) // if Timed lyrics exists (file found)
                {
                    TimedLyrics = new(LyricsModel.TimedLines);
                    AddEmptyLineToLyrics();
                    // TimedLyrics[0].IsPlaying = true; // set the first line to the one playing
                }
                else if (!string.IsNullOrWhiteSpace(Lyrics)) // If the normal lyrics were found
                {
                    ConvertToTimedLyrics(); // Create an "empty" List<TimedLyricsLine> (all lengths are 0)
                    IsEditMode = true; // Enter timed edit mode
                    IsTimedEditMode = true;
                } 
                else
                {
                    // No lyrics found, impossible to go in timed mode
                    IsTimed = false;
                }
            });

            SaveTimedLyricsCommand = new RelayCommand(() =>
            {
                LyricsModel.TimedLines = TimedLyrics;

                IsTimed = true;
                IsTimedEditMode = false; // Leave timed edit mode to save
                SaveLyrics();
            });

            DeleteTimedLyricsCommand = new RelayCommand(() =>
            {
                if(TimedLyrics.Count > 0)
                {
                    LyricsModel.TimedLines.Clear();
                    IsTimed = false;

                    // reset timestamp
                    foreach (TimedLyricsLine line in TimedLyrics)
                    {
                        line.TimestampMs = 0;
                    }

                    // delete file
                    _lyricsProcessor.DeleteTimedLyrics(_queueService.Queue.PlayingTrack.Title, _queueService.Queue.PlayingTrack.Album.PrimaryArtist.Name);
                }
                IsEditMode = false; // leave edit mode
            });

            SetLengthCommand = new RelayCommand<TimedLyricsLine>((l) =>
            {
                if (l is not null)
                {
                    //int currentLength = CurrentPosition;
                    //TimedLyrics[l.I].LengthInMilliseconds = currentLength;
                    //TimedLyrics[l.index].Time = TimeSpan.FromMilliseconds(currentLength).ToShortString();
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
                LyricsModel.LyricsText = Lyrics;
                LyricsModel.IsSaved = false;
                IsSaved = false;
            });

            ToggleAutoForegroundCommand = new RelayCommand(() =>
            {
                IsAutoForeground = !IsAutoForeground;
            });
            
            ToggleSettingsVisibilityCommand = new RelayCommand(() =>
            {
                SettingsVisibility = !SettingsVisibility;
            });

            SyncScrollWithLyricsCommand = new RelayCommand(() =>
            {
                _scrollViewer?.ScrollToItem(PlayingTimedLyrics, true);
                ControlledScrolling = false;
            });

            OnPlayingTrackChanged();
        }

        private void ConvertToTimedLyrics()
        {
            IsTimed = true;
            TimedLyrics = new(LyricsModel.TimedLines);
        }

        public override void Dispose()
        {
            _audioService.CurrentPositionChanged -= OnCurrentAudioPositionChanged;
            _queueService.PlayingTrackChanged -= OnPlayingTrackChanged;
            GC.SuppressFinalize(this);
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            _scrollViewer = e.Sender;
            if(IsTimed && !IsTimedEditMode)
            {
                if(!e.ScrollInitiatedByAnimation)
                {
                    _wasControlledScrolling++;
                    if(_wasControlledScrolling > 10)
                    {
                        ControlledScrolling = true;
                    }
                } 
                else
                {
                    _wasControlledScrolling--;
                    if(_wasControlledScrolling <= 0)
                    {
                        ControlledScrolling = false;
                        _wasControlledScrolling = 0;
                    }
                }                
            } 
            else
            {
                ControlledScrolling = true;
                _wasControlledScrolling = 11;
            }
        }

        public override void Init()
        {
            base.Init();
        }

        private void EnterLeaveEditMode()
        {
            if (IsTimed) // Enter or leave timed edit mode and set edit mode to the corresponding value
            {
                IsTimedEditMode = !IsTimedEditMode;
                IsEditMode = IsTimedEditMode;
            }
            else // Enter or Leave normal edit mode and set timed edit mode to false
            {
                IsEditMode = !IsEditMode;
                IsTimedEditMode = false;

            }

            if (IsEditMode)
            {
                RemoveEmptyLineToLyrics();
            } 
            else
            {
                AddEmptyLineToLyrics();
            }
        }

        private void SaveLyrics()
        {
            if (IsTimedEditMode) // don't save unfinished timed lyrics
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("You need to finish timing the lyrics and click on the 'Save' button."));
                }));
                return;
            }

            RemoveEmptyLineToLyrics();

            LyricsModel.LyricsText = Lyrics;
            SavedLyrics = Lyrics;

            if(IsTimed)
            {
                if(LyricsModel.Id == 0)
                {
                    MusicPlay.Database.Models.Lyrics.Insert(LyricsModel, _queueService.Queue.PlayingTrack);
                }

                LyricsModel.TimedLines = TimedLyrics;
                MusicPlay.Database.Models.Lyrics.InsertUpdateTimedLyrics(LyricsModel);
            }
            else
            {
                MusicPlay.Database.Models.Lyrics.Update(LyricsModel);
            }

            if (IsEditMode)
            {
                AddEmptyLineToLyrics();
            }

            IsSaved = true;
            IsEditMode = false; // Escape edit mode
        }

        private async Task InitPropertiesWithLyrics(Lyrics lyrics, bool needSaving = false)
        {
            if(lyrics == null)
            {
                LyricsModel = null;
                IsTimed = false;
                IsSaved = false;
                IsURLValid = false;
                WebsiteSource = "";
                URL = "";
                _lyrics = "";
                TimedLyrics = [];
                CanSave = false;
                CanCopy = false;
                OnPropertyChanged(nameof(Lyrics));
                LyricsFound = false;
                return;
            }

            LyricsModel = lyrics;

            IsTimed = lyrics.HasTimedLyrics;
            IsSaved = lyrics.IsSaved || !needSaving;
            IsURLValid = !string.IsNullOrWhiteSpace(lyrics.Url);

            WebsiteSource = lyrics.WebsiteSource;
            URL = lyrics.Url;

            _lyrics = lyrics.LyricsText;

            IsTimed = lyrics.HasTimedLyrics;
            if (_lyrics.IsNullOrWhiteSpace() && IsTimed)
            {
                Lyrics NonTimedLyrics = await _lyricsProcessor.GetLyrics(_queueService.Queue.PlayingTrack.Title, _queueService.Queue.PlayingTrack.Album.PrimaryArtist.Name, _queueService.Queue.PlayingTrack.Path,  false);
                _lyrics = NonTimedLyrics.LyricsText;

                if(_lyrics.IsNullOrWhiteSpace() && URL.IsNotNullOrWhiteSpace())
                {
                    _lyrics = await _lyricsProcessor.GetLyricsWithUrl(URL);
                }
            }

            SavedLyrics = Lyrics;
            TimedLyrics = new(lyrics.TimedLines);
            AddEmptyLineToLyrics();
            OnPropertyChanged(nameof(Lyrics));

            LyricsFound = IsSaved || IsURLValid;
            CanSave = LyricsFound;
            CanCopy = !string.IsNullOrWhiteSpace(Lyrics);

            if(!IsSaved && _lyrics.IsNotNullOrWhiteSpace() && needSaving)
            {
                MusicPlay.Database.Models.Lyrics.Insert(lyrics, _queueService.Queue.PlayingTrack);

                if(IsTimed)
                {
                    MusicPlay.Database.Models.Lyrics.InsertUpdateTimedLyrics(lyrics);
                }

                IsSaved = true;
            }
        }

        private async Task GetLyrics(bool acceptTimedLyrics = true)
        {
            if (_queueService.Queue?.PlayingTrack is null) return;

            await InitPropertiesWithLyrics(_queueService.Queue.PlayingTrack.Lyrics);

            if(!LyricsFound)
            {
                Track currentTrack = _queueService.Queue.PlayingTrack;
                Lyrics lyrics  = await _lyricsProcessor.GetLyrics(currentTrack.Title, currentTrack.Album.PrimaryArtist.Name, currentTrack.Path, acceptTimedLyrics);

                await InitPropertiesWithLyrics(lyrics, true);
            }
        }

        private void AddEmptyLineToLyrics()
        {
            if (IsTimed && (TimedLyrics == null || TimedLyrics.Count == 0)) return;
            if (!IsTimed && string.IsNullOrWhiteSpace(Lyrics)) return;

            int nbLineToAdd = 3;
            if (IsTimed)
            {
                nbLineToAdd = 4;

                TimedLyrics.Insert(0, new()
                {
                    Line = "",
                    Index = 0,
                    TimestampMs = 0,
                    IsPlaying = false,
                });
            }

            for (int i = 0; i < nbLineToAdd; i++)
            {
                if (IsTimed)
                {
                    int index = TimedLyrics.Count + 1;
                    TimedLyrics.Add(new()
                    {
                        Line = "",
                        Index = index,
                        TimestampMs = TimedLyrics.Last().TimestampMs + index * 2000,
                        IsPlaying = false,
                    });
                } 
                else
                {
                    _lyrics += "\n";
                    _lyrics = _lyrics.Insert(0, "\n");
                }
            }
            OnPropertyChanged(nameof(Lyrics));
        }

        private void RemoveEmptyLineToLyrics()
        {
            if (IsTimed && (TimedLyrics == null || TimedLyrics.Count == 0)) return;
            if (!IsTimed && string.IsNullOrWhiteSpace(Lyrics)) return;

            int nbLineToAdd = 3;
            if(IsTimed)
            {
                nbLineToAdd = 4;

                if (TimedLyrics.First().Line == "")
                {
                    TimedLyrics.RemoveAt(0);
                }
            }

            for (int i = 0; i < nbLineToAdd; i++)
            {
                if(IsTimed)
                {
                    int index = TimedLyrics.Count - 1;
                    if(TimedLyrics.Last().Line == "")
                    {
                        TimedLyrics.RemoveAt(index);
                    }
                } 
                else
                {
                    if (_lyrics.StartsWith('\n'))
                    {
                        _lyrics = _lyrics.Remove(0, 1);
                    }

                    if (_lyrics.EndsWith('\n'))
                    {
                        _lyrics = _lyrics.Remove(_lyrics.Length - 1);
                    }
                }
            }
            OnPropertyChanged(nameof(Lyrics));
        }


        private void OnCurrentAudioPositionChanged()
        {
            if (IsTimed && !IsTimedEditMode)
            {
                int currentLength = CurrentPosition;
                bool found = false;
                TimedLyricsLine previousPlayingLine = PlayingTimedLyrics;
                for (int i = 0; i < TimedLyrics.Count; i++)
                {
                    TimedLyrics[i].IsPlaying = false;
                    if (!found)
                    {
                        if (i < TimedLyrics.Count - 1 && currentLength > TimedLyrics[i].TimestampMs && 
                            currentLength < TimedLyrics[i + 1].TimestampMs)
                        {
                            PlayingTimedLyrics = TimedLyrics[i];
                            TimedLyrics[i].IsPlaying = true;
                            found = true;
                        }
                        else if (i == 0 && CurrentPosition <= TimedLyrics[0].TimestampMs)
                        {
                            PlayingTimedLyrics = TimedLyrics[i];
                            TimedLyrics[i].IsPlaying = true;
                            found = true;
                        }
                        else if (i == TimedLyrics.Count - 1 && CurrentPosition >= TimedLyrics[i].TimestampMs)
                        {
                            PlayingTimedLyrics = TimedLyrics[i];
                            TimedLyrics[i].IsPlaying = true;
                        }

                        if(found && previousPlayingLine != PlayingTimedLyrics && !ControlledScrolling)
                        {
                            _scrollViewer?.ScrollToItem(TimedLyrics[i], true);
                        }
                    }
                }
            }
        }

        private async void OnPlayingTrackChanged()
        {
            if (!IsEditMode)
            {
                IsTimedEditMode = false;
                await GetLyrics();
            }
        }
    }
}
