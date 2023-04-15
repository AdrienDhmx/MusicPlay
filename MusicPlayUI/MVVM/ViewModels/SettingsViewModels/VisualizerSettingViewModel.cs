using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using SpectrumVisualizer.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class VisualizerSettingViewModel : BaseSettingViewModel
    {
        private readonly IWindowService _windowService;

        private IVisualizerParameterStore _visualizerParameter;
        public IVisualizerParameterStore VisualizerParameter
        {
            get { return _visualizerParameter;}
            set { SetField(ref _visualizerParameter, value); }

        }

        private ObservableCollection<SettingValueModel<int>> _dataQuantities = new(SettingsModelFactory.GetVisualizerDataQuantities());
        public ObservableCollection<SettingValueModel<int>> DataQuantities
        {
            get { return _dataQuantities; }
            set
            {
                _dataQuantities = value;
                OnPropertyChanged(nameof(DataQuantities));
            }
        }

        private ObservableCollection<SettingValueModel<int>> _dataRepresentations = new(SettingsModelFactory.GetVisualizerRepresentations());
        public ObservableCollection<SettingValueModel<int>> DataRepresentations
        {
            get { return _dataRepresentations; }
            set
            {
                _dataRepresentations = value;
                OnPropertyChanged(nameof(DataRepresentations));
            }
        }


        private ObservableCollection<SettingValueModel<int>> _objectLengths = new(SettingsModelFactory.GetVisualizerObjectLengths());
        public ObservableCollection<SettingValueModel<int>> ObjectLengths
        {
            get { return _objectLengths; }
            set
            {
                _objectLengths = value;
                OnPropertyChanged(nameof(ObjectLengths));
            }
        }

        private ObservableCollection<SettingValueModel<int>> _refreshRates = new(SettingsModelFactory.GetVisualizerRefreshRates());
        public ObservableCollection<SettingValueModel<int>> RefreshRates
        {
            get { return _refreshRates; }
            set
            {
                _refreshRates = value;
                OnPropertyChanged(nameof(RefreshRates));
            }
        }

        private string _applieDataQt;
        public string AppliedDataQt
        {
            get { return _applieDataQt; }
            set
            {
                _applieDataQt = AppliedSetting(value);
                OnPropertyChanged(nameof(AppliedDataQt));
            }
        }

        private string _appliedRepresentation;
        public string AppliedRepresentation
        {
            get { return _appliedRepresentation; }
            set
            {
                _appliedRepresentation = AppliedSetting(value); ;
                OnPropertyChanged(nameof(AppliedRepresentation));
            }
        }

        private string _appliedObjectLength;
        public string AppliedObjectLength
        {
            get { return _appliedObjectLength; }
            set
            {
                _appliedObjectLength = AppliedSetting(value);
                OnPropertyChanged(nameof(AppliedObjectLength));
            }
        }

        private string _appliedRefreshRate;
        public string AppliedRefreshRate
        {
            get { return _appliedRefreshRate; }
            set
            {
                _appliedRefreshRate = AppliedSetting(value);
                OnPropertyChanged(nameof(AppliedRefreshRate));
            }
        }

        public ICommand UpdateDataQuantityCommand { get; }
        public ICommand UpdateDataRepresentationCommand { get; }
        public ICommand UpdateObjectsLengthCommand { get; }
        public ICommand UpdateRefreshRateCommand { get; }
        public ICommand UpdateColorCommand { get; }
        public ICommand UpdateCutPercentage { get; }
        public ICommand UpdateStrokeThickness { get; }
        public ICommand CloseWindowCommand { get; }
        public ICommand OpenInWindowCommand { get; }
        public VisualizerSettingViewModel(IVisualizerParameterStore visualizerParameterStore, IWindowService windowService)
        {
            VisualizerParameter = visualizerParameterStore;
            _windowService = windowService;

            Init();

            UpdateDataQuantityCommand = new RelayCommand<SettingValueModel<int>>((settingValue) =>
            {
                VisualizerParameter.DataQuantity = (DataQuantityEnum)settingValue.Value;
                UpdateCollection(DataQuantities, settingValue);
                AppliedDataQt = settingValue.Name;
            });

            UpdateDataRepresentationCommand = new RelayCommand<SettingValueModel<int>>((settingValue) =>
            {
                VisualizerParameter.Representation = (DataRepresentationTypeEnum)settingValue.Value;
                UpdateCollection(DataRepresentations, settingValue);
                AppliedRepresentation = settingValue.Name;
            });

            UpdateObjectsLengthCommand = new RelayCommand<SettingValueModel<int>>((settingValue) =>
            {
                VisualizerParameter.ObjectLength = (ObjectLengthEnum)settingValue.Value;
                UpdateCollection(ObjectLengths, settingValue);
                AppliedObjectLength = settingValue.Name;
            });

            UpdateRefreshRateCommand = new RelayCommand<SettingValueModel<int>>((settingValue) =>
            {
                VisualizerParameter.RefreshRate = (FrameRateEnum)settingValue.Value;
                UpdateCollection(RefreshRates, settingValue);
                AppliedRefreshRate = settingValue.Name;
            });

            CloseWindowCommand = new RelayCommand(() => _windowService.CloseWindow(ViewNameEnum.Visualizer));

            OpenInWindowCommand = new RelayCommand(() =>
            {
                _windowService.OpenWindow(ViewNameEnum.Visualizer);
            });
        }

        private void Init()
        {
            UpdateCollection(DataQuantities, ConfigurationService.GetPreference(SettingsEnum.VDataQt));
            UpdateCollection(DataRepresentations, ConfigurationService.GetPreference(SettingsEnum.VRepresentation));
            UpdateCollection(ObjectLengths, ConfigurationService.GetPreference(SettingsEnum.VObjectLength));
            UpdateCollection(RefreshRates, ConfigurationService.GetPreference(SettingsEnum.VRefreshRate));

            AppliedDataQt = DataQuantities.ToList().Find(d => d.IsSelected)?.Name;
            AppliedRepresentation = DataRepresentations.ToList().Find(d => d.IsSelected)?.Name;
            AppliedObjectLength = ObjectLengths.ToList().Find(d => d.IsSelected)?.Name;
            AppliedRefreshRate = RefreshRates.ToList().Find(d => d.IsSelected)?.Name;
        }

        private void UpdateCollection(ObservableCollection<SettingValueModel<int>> collection, SettingValueModel<int> selected)
        {
            foreach (SettingValueModel<int> item in collection)
            {
                if (item.IsSelected)
                {
                    item.IsSelected = false;
                }
                if (item.Name == selected.Name)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void UpdateCollection(ObservableCollection<SettingValueModel<int>> collection, int value)
        {
            foreach (SettingValueModel<int> item in collection)
            {
                if (item.IsSelected)
                {
                    item.IsSelected = false;
                }
                if (item.Value == value)
                {
                    item.IsSelected = true;
                }
            }
        }
    }
}
