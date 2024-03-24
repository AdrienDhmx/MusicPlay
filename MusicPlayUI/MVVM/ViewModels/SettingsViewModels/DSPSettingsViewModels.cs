using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AudioHandler;
using AudioHandler.Models;

using Equalizer.Models;

using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;

using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class DSPSettingsViewModels : SettingsViewModel
    {
        private readonly IModalService _modalService;
        public IAudioPlayback AudioPlayback { get; }
        public EQManager EQManager => AudioPlayback.EQManager;

        private bool _equalizerEnabled;
        public bool EqualizerEnabled
        {
            get { return _equalizerEnabled; }
            set
            {
                SetField(ref _equalizerEnabled, value);
                EQManager.Enabled = value;
                ConfigurationService.SetPreference(SettingsEnum.EqualizerEnabled, value ? "1" : "0");
            }
        }

        private ObservableCollection<EQPreset> _presets;
        public ObservableCollection<EQPreset> Presets
        {
            get => _presets;
            set
            {
                SetField(ref _presets, value);
            }
        }

        private UIEQBandModel _selectedPresetBand;
        public UIEQBandModel SelectedPresetBand
        {
            get => _selectedPresetBand;
            set
            {
                SetField(ref _selectedPresetBand, value);
                OnPropertyChanged(nameof(BandSelected));
            }
        }

        public bool BandSelected => SelectedPresetBand != null;

        /// <summary>
        /// The currently applied preset to the audio
        /// </summary>
        public EQPreset AppliedPreset
        {
            get => EQManager.Preset;
            set
            {
                CanSave = false;
                CanUpdate = value.Id >= 0; // preset is user made
                // clone to not modify the saved preset in the collection
                EQManager.ApplyPreset((EQPreset)value.Clone());
                OriginalPreset = value;
                OnPropertyChanged(nameof(AppliedPresetName));
                ConfigurationService.SetPreference(SettingsEnum.EqualizerPreset, value.Id.ToString());
            }
        }

        /// <summary>
        /// Represent the original pre existing preset that has been modified by the user
        /// </summary>
        private EQPreset _originalPreset;
        public EQPreset OriginalPreset
        {
            get => _originalPreset;
            set
            {
                SetField(ref _originalPreset, value);
            }
        }

        public string AppliedPresetName => string.IsNullOrEmpty(AppliedPreset.Name) ? "Manual" : AppliedPreset.Name;

        private bool _canSave;
        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                SetField(ref _canSave, value);
            }
        }

        private bool _canUpdate;
        public bool CanUpdate
        {
            get { return _canUpdate; }
            set
            {
                SetField(ref _canUpdate, value);
            }
        }

        private bool _isPresetPopupOpen;
        public bool IsPresetPopupOpen
        {
            get { return _isPresetPopupOpen; }
            set
            {
                SetField(ref _isPresetPopupOpen, value);
            }
        }

        public bool CanAddBand => AppliedPreset.Effects.Count < MaxBandCount;

        public const int MaxBandCount = 10;

        public ICommand ResetPresetCommand { get; }
        public ICommand TogglePresetPopupCommand { get; }
        public ICommand ToggleEqualizerCommand { get; }
        public ICommand ApplyPresetCommand { get; }
        public ICommand CreatePresetCommand { get; }
        public ICommand SavePresetCommand { get; }
        public ICommand EditPresetNameCommand { get; }
        public ICommand DeletePresetCommand { get; }
        public ICommand AddBandCommand { get; }
        public ICommand RemoveBandCommand { get; }
        public DSPSettingsViewModels(IWindowService windowService, IAudioPlayback audioPlayback, IModalService modalService) : base(windowService)
        {
            AudioPlayback = audioPlayback;
            _modalService = modalService;
            EQManager.EQBandChanged += OnEQBandUpdated;

            ResetPresetCommand = new RelayCommand(ResetPreset);

            ToggleEqualizerCommand = new RelayCommand(() => EqualizerEnabled = !EqualizerEnabled);

            TogglePresetPopupCommand = new RelayCommand(() => IsPresetPopupOpen = !IsPresetPopupOpen);

            ApplyPresetCommand = new RelayCommand<EQPreset>((preset) =>
            {
                AppliedPreset = preset;
                IsPresetPopupOpen = false;
            });

            CreatePresetCommand = new RelayCommand(() =>
            {
                async void createPreset(string newName)
                {
                    EQPreset preset = new();
                    preset.Effects = new(EQManager.Preset.Effects);
                    preset.Name = newName;
                    await EQPreset.Insert(preset);
                    AppliedPreset = preset;
                }
                CreateEditNameModel model = new CreateEditNameModel("", "Preset", false, createPreset, null);
                _modalService.OpenModal(ViewNameEnum.CreateTag, (canceled) => { }, model);
            });

            SavePresetCommand = new RelayCommand(async () =>
            {
                foreach (EQBand eqBand in AppliedPreset.Effects)
                {
                    //DataAccess.Connection.Update(eqBand);
                }

                for (int i = 0; i < AppliedPreset.Effects.Count; i++)
                {
                    EQBand appliedBand = AppliedPreset.Effects[i];

                    // band added, insert it
                    if(appliedBand.Id < 0)
                    {
                        //await DataAccess.Connection.InsertEQBand(appliedBand, AppliedPreset.Id);
                        continue;
                    }
                    else if (i < OriginalPreset.Effects.Count)
                    {
                        EQBand originalBand = OriginalPreset.Effects[i];

                        // a band has been removed, delete it
                        if(appliedBand.Id !=  originalBand.Id && !AppliedPreset.Effects.Any(e => e.Id == originalBand.Id))
                        {
                            //await DataAccess.Connection.DeleteOne(originalBand);
                            continue;
                        }
                    }

                    // update the band
                    //DataAccess.Connection.Update(appliedBand);
                }

                GetPresets();
                OriginalPreset = AppliedPreset;
                CanSave = false;
            });

            EditPresetNameCommand = new RelayCommand(() =>
            {
                Action<string> updatePreset = async (string newName) =>
                {
                    OriginalPreset.Name = newName;
                    //DataAccess.Connection.Update(OriginalPreset);
                    AppliedPreset.Name = newName;
                    OnPropertyChanged(nameof(AppliedPresetName));
                    GetPresets();
                };
                CreateEditNameModel model = new CreateEditNameModel(OriginalPreset.Name, "Preset", true, null, onEdit: updatePreset);
                _modalService.OpenModal(ViewNameEnum.CreateTag, (canceled) => { }, model);

            });

            DeletePresetCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(ViewNameEnum.ConfirmAction, ConfirmDelete, ConfirmActionModelFactory.CreateConfirmDeleteModel(OriginalPreset.Name, ModelTypeEnum.EQPreset));
            });

            AddBandCommand = new RelayCommand(() =>
            {
                EQManager.AddEffect(new());
                OnPropertyChanged(nameof(AppliedPreset));
                OnEQBandUpdated();
            });

            RemoveBandCommand = new RelayCommand<EQBand>((EQBand band) =>
            {
                EQManager.RemoveEffect(band);
                OnPropertyChanged(nameof(AppliedPreset));
                OnEQBandUpdated();

                if(band.Id == SelectedPresetBand.Id)
                {
                    SelectedPresetBand = null;
                }
            });

            // init
            Update();
        }

        public override void Dispose()
        {
            EQManager.EQBandChanged -= OnEQBandUpdated;
            base.Dispose();
        }

        private void ConfirmDelete(bool canceled)
        {
            if (!canceled)
            {
                //DataAccess.Connection.DeleteOne(OriginalPreset);

                ResetPreset();
                GetPresets();
            }
        }

        private void ResetPreset()
        {
            EQManager.ResetPreset();
            CanSave = false;
            CanUpdate = false;
            OriginalPreset = null;
            OnPropertyChanged(nameof(AppliedPresetName));
            ConfigurationService.SetPreference(SettingsEnum.EqualizerPreset, "-1");
        }

        private void OnEQBandUpdated()
        {
            CanSave = CanUpdate;
            OnPropertyChanged(nameof(CanAddBand));
        }

        private async void GetPresets()
        {
            // user preset at the top of the list
            List<EQPreset> presets = await EQPreset.GetAll();
            presets.AddRange(EQModelsFactory.GetPreMadeEQPresets());

            Presets = new(presets);
        }

        public override void Update(BaseModel parameter = null)
        {
            _equalizerEnabled = EQManager.Enabled;
            GetPresets();
        }
    }
}
