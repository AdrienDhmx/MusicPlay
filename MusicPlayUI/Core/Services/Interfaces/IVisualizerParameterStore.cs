using SpectrumVisualizer.Enums;
using System;
using System.Windows;
using System.Windows.Media;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IVisualizerParameterStore
    {
        bool CutHigherFreq { get; set; }
        double CutPercentage { get; set; }
        DataQuantityEnum DataQuantity { get; set; }
        bool Fill { get; set; }
        bool Gradient { get; set; }
        Brush ObjectColor { get; set; }
        Brush EmphasizedObjectColor { get; }
        TextAlignment TextAlignment { get; set; }
        ObjectLengthEnum ObjectLength { get; set; }
        FrameRateEnum RefreshRate { get; set; }
        DataRepresentationTypeEnum Representation { get; set; }
        bool SmoothGraph { get; set; }
        double StrokeThickness { get; set; }
        bool AutoColor { get; set; }
        bool CenterFreq { get; set; }

        event Action AutoColorChanged;
        event Action RepresentationChanged;
    }
}