using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MusicFilesProcessor;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels.AppBars
{
    public class AppBar : ObservableObject, IDisposable
    {
        public const double _defaultHeight = 60;
        public static Brush _defaultBackground => AppTheme.Palette.Background;
        public static Brush _defaultForeground => AppTheme.Palette.OnBackground;
        public static Brush _defaultSecondaryForeground => AppTheme.Palette.OnSurfaceVariant;
        public const double _defaultbackgroundOpacity = 1;
        public const double _defaultTitleFontSize = 20;
        public const double _defaultContentOpacity = 1;
        public const bool _defaultApplyDropShadow = false;
        private static DropShadowEffect _defaultDropShadowEffect => new() 
        { 
            BlurRadius = 14,
            Color = AppTheme.IsLightTheme ? Color.FromRgb(10, 10, 10) : Color.FromRgb(0, 0, 0),
            Direction = -5,
            ShadowDepth = 2,
            Opacity = 0.5,
        };

        private readonly ICommandsManager _commandsManager;
        public static IAppState AppState => App.State;

        private double _height = _defaultHeight;
        private Brush _background = _defaultBackground;
        private double _backgroundOpacity = _defaultbackgroundOpacity;
        private bool _applyDropShadow = _defaultApplyDropShadow;
        private DropShadowEffect _dropShadowEffect = _defaultDropShadowEffect;
        private double _contentOpacity = _defaultContentOpacity;
        private Brush _foreground = _defaultForeground;
        private Brush _secondareForeground = _defaultForeground;
        private double _titleFontSize = _defaultTitleFontSize;

        private string _title = string.Empty;
        private string _subTitle = string.Empty;

        public double Height
        {
            get => _height;
            set => SetField(ref _height, value);
        }

        public Brush Background
        {
            get => _background;
            set => SetField(ref _background, value);
        }

        public double BackgroundOpacity
        {
            get => _backgroundOpacity;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                SetField(ref _backgroundOpacity, value);
            }
        }

        public bool ApplyDropShadow
        {
            get => _applyDropShadow;
            set => SetField(ref _applyDropShadow, value);
        }

        public DropShadowEffect DropShadowEffect
        {
            get => _dropShadowEffect;
            set => SetField(ref _dropShadowEffect, value);
        }

        public double ContentOpacity
        {
            get => _contentOpacity;
            set
            {
                if (value < 0)
                    value = 0;
                else if(value > 1)
                    value = 1;
                SetField(ref _contentOpacity, value);
            }
        }

        public Brush Foreground
        {
            get => _foreground;
            set => SetField(ref _foreground, value);
        }

        public Brush SecondaryForeground
        {
            get => _secondareForeground;
            set => SetField(ref _secondareForeground, value);
        }

        public double TitleFontSize
        {
            get => _titleFontSize;
            set => SetField(ref _titleFontSize, value);
        }


        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        public string Subtitle
        {
            get => _subTitle;
            set => SetField(ref _subTitle, value);
        }


        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateForwardCommand { get; }
        public ICommand ToggleMenuDrawerCommand { get; }
        public AppBar(ICommandsManager commandsManager)
        {
            _commandsManager = commandsManager;

            NavigateBackCommand = _commandsManager.NavigateBackCommand;
            NavigateForwardCommand = _commandsManager.NavigateForwardCommand;
            ToggleMenuDrawerCommand = _commandsManager.ToggleMenuDrawerCommand;

            AppTheme.ThemeChanged += StyleNeedUpdate;
        }

        private void StyleNeedUpdate()
        {
            AppState.CurrentView.ViewModel.UpdateAppBarStyle();
        }

        public void SetStyle(Brush background, double backgroundOpacity, double contentOpacity = 1, bool applyDropShadow = false, double titleFontSize = _defaultTitleFontSize)
        {
            Background = background;
            BackgroundOpacity = backgroundOpacity;
            ContentOpacity = contentOpacity;
            ApplyDropShadow = applyDropShadow;
            TitleFontSize = titleFontSize;
        }

        public void SetDropShadow(double opacity)
        {
            DropShadowEffect.Opacity = opacity;
            OnPropertyChanged(nameof(DropShadowEffect));
            ApplyDropShadow = DropShadowEffect.Opacity != 0;
        }

        public void SetForeground(Brush foreground)
        {
            Foreground = foreground;

            SolidColorBrush solidColorBrush = (SolidColorBrush)foreground;
            if(solidColorBrush.CalculateBrightness() > 0.5)
            {
                SecondaryForeground = solidColorBrush.MultiplyBy(0.8);
            } 
            else
            {
                SecondaryForeground = solidColorBrush.MultiplyBy(1.2);
            }
        }

        public void SetData(string title, string subtitle)
        {
            Title = title;
            Subtitle = subtitle;
        }


        /// <summary>
        /// Reset the style of the App Bar (Height, Background...) by setting them to their default value
        /// </summary>
        public void ResetStyle()
        {
            Height = _defaultHeight;
            Background = _defaultBackground;
            BackgroundOpacity = _defaultbackgroundOpacity;
            ContentOpacity = _defaultContentOpacity;
            Foreground = _defaultForeground;
            SecondaryForeground = _defaultSecondaryForeground;
            TitleFontSize = _defaultTitleFontSize;
            ApplyDropShadow = _defaultApplyDropShadow;
            DropShadowEffect = _defaultDropShadowEffect;
        }

        /// <summary>
        /// Reset the data of the App Bar (Title, Subtitle...) by setting them to their default value
        /// </summary>
        public void ResetData()
        {
            Title = string.Empty;
            Subtitle = string.Empty;
        }

        /// <summary>
        /// Calls <see cref="ResetStyle"/> and <see cref="ResetData"/>
        /// </summary>
        public void Reset()
        {
            ResetStyle();
            ResetData();            
        }

        public void AnimateContentWithScroll(double currentOffset, double startOffset, double endOffset)
        {
            if(currentOffset < startOffset)
            {
                ContentOpacity = 0;
                return;
            }

            if(currentOffset > endOffset)
            {
                ContentOpacity = 1;
                return;
            }

            double newOpacity = currentOffset / endOffset;

            if(newOpacity < 0.05)
            {
                ContentOpacity = 0;
                return;
            }
            ContentOpacity = newOpacity;
        }

        public void AnimateBackgroundWithScroll(double currentOffset, double startOffset, double endOffset, double minOpacity = 0, double maxOpacity = 1)
        {
            if (currentOffset < startOffset)
            {
                BackgroundOpacity = minOpacity;
                return;
            }

            if (currentOffset > endOffset)
            {
                BackgroundOpacity = maxOpacity;
                return;
            }

            double newOpacity = currentOffset / endOffset;

            if (newOpacity < minOpacity)
            {
                BackgroundOpacity = minOpacity;
                return;
            }
            else if(newOpacity > maxOpacity)
            {
                BackgroundOpacity = maxOpacity;
                return;
            }
            BackgroundOpacity = newOpacity;
        }

        public void AnimateElevation(double currentOffset, 
            double startBackgroundOffset, double endBackgroundOffset,
            double startContentOffset, double endContentOffset,
            double startDropShadowOffset, double endDropShadowOffset,
            double minBackgroundOpacity, double maxBackgroundOpacity,
            double minContentOpacity, double maxContentOpacity,
            double minDropShadowOpacity, double maxDropShadowOpacity)
        {
            if (currentOffset <= startBackgroundOffset)
            {
                BackgroundOpacity = minBackgroundOpacity;
            }
            else
            {
                BackgroundOpacity = Math.Max(Math.Min((currentOffset - startBackgroundOffset) / endBackgroundOffset, 
                                                        maxBackgroundOpacity), 
                                            minBackgroundOpacity);
            }

            if (currentOffset <= startContentOffset)
            {
                ContentOpacity = minContentOpacity;
            }
            else
            {
                ContentOpacity = Math.Max(Math.Min((currentOffset - startContentOffset) / endContentOffset, 
                                                    maxContentOpacity), 
                                            minContentOpacity);
            }

            double newOpacity = minDropShadowOpacity;
            if (currentOffset > startDropShadowOffset)
            {
                newOpacity = Math.Max(Math.Min((currentOffset- startDropShadowOffset) / endDropShadowOffset, 
                                                maxDropShadowOpacity), 
                                        minDropShadowOpacity);
            }

            DropShadowEffect.Opacity = newOpacity;
            OnPropertyChanged(nameof(DropShadowEffect));
            ApplyDropShadow = DropShadowEffect.Opacity != 0;
        }

        public void AnimateElevation(double currentOffset, bool quickAnimation = false)
        {
            double startOffset = 4;
            double startContentOffset = 40;
            double endBackgroundOffset = quickAnimation ? 80 : 100;
            double endContentOffset = 140;
            double endDropShadowOffset = quickAnimation ? 300 : 500;

            AnimateElevation(currentOffset,
                            startOffset, endBackgroundOffset,
                            startContentOffset, endContentOffset,
                            endBackgroundOffset, endDropShadowOffset,
                            0, 1,
                            0, 1,
                            0, 1);
        }

        public void Dispose()
        {
        }
    }
}
