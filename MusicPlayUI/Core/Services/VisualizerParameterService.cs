using System;
using SpectrumVisualizer.Enums;
using System.Windows.Media;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.Core.Services
{
    public class VisualizerParameterService : BaseSettingViewModel, IVisualizerParameterStore
    {
        private DataRepresentationTypeEnum _representation = (DataRepresentationTypeEnum)ConfigurationService.GetPreference(SettingsEnum.VRepresentation);
        public DataRepresentationTypeEnum Representation
        {
            get { return _representation; }
            set
            {
                SetField(ref _representation, value);
                OnRepresentationChanged();
                SetPreference(SettingsEnum.VRepresentation, ((int)value).ToString());
            }
        }

        private Brush _objectColor = ConfigurationService.GetVColor();
        public Brush ObjectColor
        {
            get { return _objectColor; }
            set
            {
                SetField(ref _objectColor, value);
                if (!AutoColor) // if color chosen by user save it
                {
                    ConfigurationService.SetPreference(SettingsEnum.VColor, ToHex(value));
                }
            }
        }

        private bool _autoColor = ConfigurationService.GetPreference(SettingsEnum.VAutoColor) == 1;
        public bool AutoColor
        {
            get { return _autoColor; }
            set
            {
                _autoColor = value;
                OnAutoColorChanged();
                SetPreference(SettingsEnum.VAutoColor, BoolToString(value));
            }
        }

        private bool _fill = ConfigurationService.GetPreference(SettingsEnum.VFill) == 1;
        public bool Fill
        {
            get { return _fill; }
            set
            {
                SetField(ref _fill, value);
                SetPreference(SettingsEnum.VFill, BoolToString(value));
            }
        }

        private double _strokeThickness = ConfigurationService.GetPreference(SettingsEnum.VStrokeThickness);
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                SetField(ref _strokeThickness, value);
                SetPreference(SettingsEnum.VStrokeThickness, StrokeThickness.ToString());
            }
        }

        private ObjectLengthEnum _objectLength = (ObjectLengthEnum)ConfigurationService.GetPreference(SettingsEnum.VObjectLength);
        public ObjectLengthEnum ObjectLength
        {
            get { return _objectLength; }
            set
            {
                SetField(ref _objectLength, value);
                SetPreference(SettingsEnum.VObjectLength, ((int)value).ToString());
            }
        }

        private bool _cutHigherFreq = ConfigurationService.GetPreference(SettingsEnum.VCutHighFreq) == 1;
        public bool CutHigherFreq
        {
            get { return _cutHigherFreq; }
            set
            {
                SetField(ref _cutHigherFreq, value);
                SetPreference(SettingsEnum.VCutHighFreq, BoolToString(value));
            }
        }

        private double _cutPercentage = ConfigurationService.GetPreference(SettingsEnum.VCutPercentage);
        public double CutPercentage
        {
            get { return _cutPercentage; }
            set
            {
                SetField(ref _cutPercentage, value);
                SetPreference(SettingsEnum.VCutPercentage, value.ToString());
            }
        }

        private bool _smoothGraph = ConfigurationService.GetPreference(SettingsEnum.VSmoothGraph) == 1;
        public bool SmoothGraph
        {
            get { return _smoothGraph; }
            set
            {
                SetField(ref _smoothGraph, value);
            }
        }

        private FrameRateEnum _refreshRate = (FrameRateEnum)ConfigurationService.GetPreference(SettingsEnum.VRefreshRate);
        public FrameRateEnum RefreshRate
        {
            get { return _refreshRate; }
            set
            {
                SetField(ref _refreshRate, value);
                SetPreference(SettingsEnum.VRefreshRate, ((int)value).ToString());
            }
        }

        private bool _gradient = ConfigurationService.GetPreference(SettingsEnum.VGradient) == 1;
        public bool Gradient
        {
            get { return _gradient; }
            set
            {
                SetField(ref _gradient, value);
                SetPreference(SettingsEnum.VGradient, BoolToString(value));
            }
        }

        private DataQuantityEnum _dataQuantity = (DataQuantityEnum)ConfigurationService.GetPreference(SettingsEnum.VDataQt);
        public DataQuantityEnum DataQuantity
        {
            get { return _dataQuantity; }
            set
            {
                SetField(ref _dataQuantity, value);
                SetPreference(SettingsEnum.VDataQt, ((int)value).ToString());
            }
        }

        private bool _centerFreq = ConfigurationService.GetPreference(SettingsEnum.VCenterFreq) == 1;
        public bool CenterFreq
        {
            get { return _centerFreq; }
            set
            {
                SetField(ref _centerFreq, value);
                SetPreference(SettingsEnum.VCenterFreq, BoolToString(CenterFreq));
            }
        }

        public event Action AutoColorChanged;
        private void OnAutoColorChanged()
        {
            AutoColorChanged?.Invoke();
        }

        public event Action RepresentationChanged;
        private void OnRepresentationChanged()
        {
            RepresentationChanged?.Invoke();
        }

        private static string ToHex(Brush color)
        {
            SolidColorBrush solidColorBrush = (SolidColorBrush)color;
            return ToHex(solidColorBrush.Color);
        }

        private static string ToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
