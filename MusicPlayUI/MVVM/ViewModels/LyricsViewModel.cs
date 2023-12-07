﻿using DataBaseConnection.DataAccess;
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
        public LyricsViewModel(IQueueService queueService, IAudioTimeService audioService, IVisualizerParameterStore visualizerParameterStore)
        {
            _queueService = queueService;
            _audioService = audioService;
            VisualizerParameterService = visualizerParameterStore;

            _queueService.PlayingTrackChanged += OnPlayingTrackChanged;
            _audioService.CurrentPositionChanged += OnCurrentAudioPositionChanged;

            _lyricsProcessor = new LyricsProcessor();

            SaveCommand = new RelayCommand(() => Task.Run(SaveLyrics));
            GetLyricsCommand = new RelayCommand(() => Task.Run(() => GetLyrics()));

            EnterLeaveEditModeCommand = new RelayCommand(EnterLeaveEditMode);

            EnterLeaveTimedLyricsCommand = new RelayCommand(() =>
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
                        LyricsModel.TimedLyrics.Clear();

                        // we don't have the normal lyrics, try to get them
                        if(string.IsNullOrWhiteSpace(LyricsModel.Lyrics))
                        {
                            GetLyrics();
                        }
                        
                        return;
                    }
                }

                if (_lyricsProcessor.TimedLyricsExists(LyricsModel.FileName)) // if Timed lyrics exists (file found)
                {
                    LyricsModel = _lyricsProcessor.GetLyrics(LyricsModel, true);
                    TimedLyrics = new(LyricsModel.TimedLyrics);
                    AddEmptyLineToLyrics();
                    TimedLyrics[0].IsPlaying = true; // set the fisrt line to the one playing
                }
                else if (!string.IsNullOrWhiteSpace(Lyrics)) // If the normal lyrics were found
                {
                    ConvertToTimedLyrics(); // Create an "empty" List<TimedLyricsLineModel> (all lengths are 0)
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
                IsEditMode = false; // leave edit mode
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

            ToggleAutoForegroundCommand = new RelayCommand(() =>
            {
                IsAutoForeground = !IsAutoForeground;
            });
            
            ToggleSettingsVisibilityCommand = new RelayCommand(() =>
            {
                SettingsVisibility = !SettingsVisibility;
            });

            OnPlayingTrackChanged();
        }

        private void ConvertToTimedLyrics()
        {
            LyricsModel.TimedLyrics = new(_lyricsProcessor.ConvertToTimedLyricsModel(Lyrics, true));
            LyricsModel.IsTimed = true;
            IsTimed = true;
            TimedLyrics = new(LyricsModel.TimedLyrics);
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

            LyricsModel.Lyrics = Lyrics;
            _lyricsProcessor.SaveLyrics(LyricsModel, IsTimed);
            SavedLyrics = Lyrics;

            if (IsEditMode)
            {
                AddEmptyLineToLyrics();
            }

            IsSaved = true;
            IsEditMode = false; // Escape edit mode
        }

        private async void GetLyrics(bool acceptTimedLyrics = true)
        {
            if (_queueService.PlayingTrack is null) return;

            LyricsModel = await _lyricsProcessor.GetLyrics(_queueService.PlayingTrack.Title, _queueService.PlayingTrack.GetAlbumArtist().Name, _queueService.PlayingTrack.Path, acceptTimedLyrics);

            IsTimed = LyricsModel.IsTimed;
            IsSaved = LyricsModel.IsSaved;
            OnPropertyChanged(nameof(IsSaved));
            IsURLValid = !string.IsNullOrWhiteSpace(LyricsModel.URL);

            WebsiteSource = LyricsModel.WebSiteSource;
            URL = LyricsModel.URL;

            _lyrics = LyricsModel.Lyrics;
            SavedLyrics = Lyrics;
            TimedLyrics = new(LyricsModel.TimedLyrics);
            AddEmptyLineToLyrics();
            OnPropertyChanged(nameof(Lyrics));

            LyricsFound = IsSaved || IsURLValid;
            CanSave = LyricsFound;
            CanCopy = !string.IsNullOrWhiteSpace(Lyrics);
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
                    Lyrics = "",
                    index = 0,
                    LengthInMilliseconds = 0,
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
                        Lyrics = "",
                        index = index,
                        LengthInMilliseconds = TimedLyrics.Last().LengthInMilliseconds + index * 2000,
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

                if (TimedLyrics.First().Lyrics == "")
                {
                    TimedLyrics.RemoveAt(0);
                }
            }

            for (int i = 0; i < nbLineToAdd; i++)
            {
                if(IsTimed)
                {
                    int index = TimedLyrics.Count - 1;
                    if(TimedLyrics.Last().Lyrics == "")
                    {
                        TimedLyrics.RemoveAt(index);
                    }
                } 
                else
                {
                    if (_lyrics.StartsWith("\n"))
                    {
                        _lyrics = _lyrics.Remove(0, 1);
                    }

                    if (_lyrics.EndsWith("\n"))
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
                Task.Run(() => GetLyrics());
            }
        }
    }
}
