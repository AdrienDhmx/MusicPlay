using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AudioHandler;
using AudioHandler.Models;
using DataBaseConnection.DataAccess;
using Equalizer.Models;
using MusicPlayModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
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
            }
        }

        private ObservableCollection<EQPresetModel> _presets;
        public ObservableCollection<EQPresetModel> Presets
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
        public EQPresetModel AppliedPreset
        {
            get => EQManager.Preset;
            set
            {
                CanSave = false;
                CanUpdate = value.Id != -1;
                // clone to not modify the saved preset in the collection
                EQManager.ApplyPreset((EQPresetModel)value.Clone());
                OriginalPreset = value;
                OnPropertyChanged(nameof(AppliedPresetName));
            }
        }

        /// <summary>
        /// Represent the original pre existing preset that has been modified by the user
        /// </summary>
        private EQPresetModel _originalPreset;
        public EQPresetModel OriginalPreset
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
        public DSPSettingsViewModels(INavigationService navigationService, IWindowService windowService, IAudioPlayback audioPlayback, IModalService modalService) : base(navigationService, windowService)
        {
            AudioPlayback = audioPlayback;
            _modalService = modalService;
            EQManager.EQBandChanged += OnEQBandUpdated;

            ResetPresetCommand = new RelayCommand(ResetPreset);

            ToggleEqualizerCommand = new RelayCommand(() => EqualizerEnabled = !EqualizerEnabled);

            TogglePresetPopupCommand = new RelayCommand(() => IsPresetPopupOpen = !IsPresetPopupOpen);

            ApplyPresetCommand = new RelayCommand<EQPresetModel>((preset) =>
            {
                AppliedPreset = preset;
                IsPresetPopupOpen = false;
            });

            CreatePresetCommand = new RelayCommand(() =>
            {
                Action<string> createPreset = async (string newName) =>
                {
                    EQPresetModel preset = new();
                    preset.Effects = new(EQManager.Preset.Effects);
                    preset.Name = newName;
                    preset.Id = await DataAccess.Connection.InsertEQPreset(preset);
                    AppliedPreset = preset;

                    EQManager.ApplyPreset(preset);
                };
                CreateEditNameModel model = new CreateEditNameModel("", "Preset", false, createPreset, null);
                _modalService.OpenModal(ViewNameEnum.CreateTag, (canceled) => { }, model);
            });

            SavePresetCommand = new RelayCommand(async () =>
            {
                foreach (EQEffectModel eqBand in AppliedPreset.Effects)
                {
                    await DataAccess.Connection.UpdateEQBand(eqBand);
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
                    await DataAccess.Connection.UpdateEQPreset(OriginalPreset);
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

            RemoveBandCommand = new RelayCommand<EQEffectModel>((EQEffectModel band) =>
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

        private async void ConfirmDelete(bool canceled)
        {
            if (!canceled)
            {
                await DataAccess.Connection.DeleteEQPreset(OriginalPreset);

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
        }

        private void OnEQBandUpdated()
        {
            CanSave = CanUpdate;
            OnPropertyChanged(nameof(CanAddBand));
        }

        private async void GetPresets()
        {
            // user preset at the top of the list
            List<EQPresetModel> presets = await DataAccess.Connection.GetAllEQPresets();
            presets.AddRange(EQModelsFactory.GetPreMadeEQPresets());

            Presets = new(presets);
        }

        public override void Update(BaseModel parameter = null)
        {
            _equalizerEnabled = EQManager.Enabled;
            AppliedPreset = EQManager.Preset;

            GetPresets();
        }
    }
}
