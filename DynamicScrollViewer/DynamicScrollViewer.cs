using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DynamicScrollViewer.Helper;

namespace DynamicScrollViewer
{

    public class DynamicScrollViewer : ScrollViewer
    {
        private double _totalVerticalOffset;
        private double _totalHorizontalOffset;
        private bool _isRunning;
        private double _lastCallOnCommandMs = DateTime.Now.TimeOfDay.TotalMilliseconds;

        public bool IsInit { get; private set; } = false;

        public bool CanMouseWheel
        {
            get => (bool)GetValue(CanMouseWheelProperty);
            set => SetValue(CanMouseWheelProperty, value);
        }

        public bool IsInertiaEnabled
        {
            get => (bool)GetValue(IsInertiaEnabledProperty);
            set => SetValue(IsInertiaEnabledProperty, value);
        }

        public bool IsPenetrating
        {
            get => (bool)GetValue(IsPenetratingProperty);
            set => SetValue(IsPenetratingProperty, value);
        }

        internal double CurrentVerticalOffset
        {
            // ReSharper disable once UnusedMember.Local
            get => (double)GetValue(CurrentVerticalOffsetProperty);
            set => SetValue(CurrentVerticalOffsetProperty, value);
        }

        internal double CurrentHorizontalOffset
        {
            get => (double)GetValue(CurrentHorizontalOffsetProperty);
            set => SetValue(CurrentHorizontalOffsetProperty, value);
        }

        public ICommand OnScrollCommand
        {
            get { return (ICommand)GetValue(OnScrollCommandProperty); }
            set { SetValue(OnScrollCommandProperty, value); }
        }

        public static readonly DependencyProperty OnScrollCommandProperty =
            DependencyProperty.Register("OnScrollCommand", typeof(ICommand), typeof(DynamicScrollViewer), new PropertyMetadata(null));


        public static readonly DependencyProperty CanMouseWheelProperty = DependencyProperty.Register(
            nameof(CanMouseWheel), typeof(bool), typeof(ScrollViewer), new PropertyMetadata(true));


        public static readonly DependencyProperty IsInertiaEnabledProperty = DependencyProperty.RegisterAttached(
             "IsInertiaEnabled", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));
        public static void SetIsInertiaEnabled(DependencyObject element, bool value) => element.SetValue(IsInertiaEnabledProperty, value);
        public static bool GetIsInertiaEnabled(DependencyObject element) => (bool)element.GetValue(IsInertiaEnabledProperty);


        public static readonly DependencyProperty IsPenetratingProperty = DependencyProperty.RegisterAttached(
             "IsPenetrating", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));
        public static void SetIsPenetrating(DependencyObject element, bool value) => element.SetValue(IsPenetratingProperty, value);
        public static bool GetIsPenetrating(DependencyObject element) => (bool)element.GetValue(IsPenetratingProperty);


        internal static readonly DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register(
            nameof(CurrentVerticalOffset), typeof(double), typeof(ScrollViewer), new PropertyMetadata(.0, OnCurrentVerticalOffsetChanged));

        internal static readonly DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register(
            nameof(CurrentHorizontalOffset), typeof(double), typeof(ScrollViewer), new PropertyMetadata(.0, OnCurrentHorizontalOffsetChanged));

        public double StartingVerticalOffset
        {
            get { return (double)GetValue(StartingVerticalOffsetProperty); }
            set { SetValue(StartingVerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty StartingVerticalOffsetProperty =
            DependencyProperty.Register("StartingVerticalOffset", typeof(double), typeof(DynamicScrollViewer), new PropertyMetadata(0d, OnStartingVerticalOffsetChanged));

        private static void OnStartingVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicScrollViewer ctl && !ctl.IsInit && e.NewValue is double newValue)
            {
                ctl.IsInit = true;
                if(newValue > 0)
                    ctl.ScrollToVerticalOffsetWithAnimation(newValue, 500);
            }
        }

        private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicScrollViewer ctl && e.NewValue is double newValue && e.OldValue is double oldvalue)
            {
                ctl.ScrollToVerticalOffset(newValue);
            }
        }

        private static void OnCurrentHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicScrollViewer ctl && e.NewValue is double newValue && e.OldValue is double oldvalue)
            {
                ctl.ScrollToHorizontalOffset(newValue);
            }
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);

            // avoid calling the command every milliseconds => skip 2/3
            double newCallTick = DateTime.Now.TimeOfDay.TotalMilliseconds;
            // a bit more than 60FPS and if the vertical offset reaches 0 still call the command since it's an important value (animation...)
            if (newCallTick - _lastCallOnCommandMs < 20 && (e.VerticalOffset != 0 && e.VerticalChange != 0)) 
            {
                return;
            }
            _lastCallOnCommandMs = newCallTick;

            bool isVertical = e.VerticalChange != 0;
            bool isForward = e.VerticalChange > 0;
            double delta = e.VerticalChange;
            if(!isVertical)
            {
                isForward = e.HorizontalChange > 0;
                delta = e.HorizontalChange;
            }

            if(delta != 0)
                this.OnScrollCommand?.Execute(new OnScrollEvent(delta, e.VerticalOffset, e.HorizontalOffset, isVertical, isForward, this));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            IsInit = true;
            if (!CanMouseWheel)
            {
                e.Handled = true;
                bool vertical = ScrollViewerAttach.GetOrientation(this) == Orientation.Vertical;

                double verticalOffset = vertical ? Math.Min(Math.Max(0, _totalVerticalOffset - e.Delta), ScrollableHeight) : 0;
                double horizontalOffset = vertical ? 0 : Math.Min(Math.Max(0, _totalHorizontalOffset - e.Delta), ScrollableWidth); ;

                this.OnScrollCommand?.Execute(new OnScrollEvent(e.Delta, verticalOffset, horizontalOffset, vertical, e.Delta > 0, this));
                return;
            }

            if (!IsInertiaEnabled)
            {
                if (ScrollViewerAttach.GetOrientation(this) == Orientation.Vertical)
                {
                    this.ScrollToVerticalOffset(this.VerticalOffset - e.Delta / 3);
                }
                else
                {
                    _totalHorizontalOffset = HorizontalOffset;
                    CurrentHorizontalOffset = HorizontalOffset;
                    _totalHorizontalOffset = Math.Min(Math.Max(0, _totalHorizontalOffset - e.Delta), ScrollableWidth);
                    CurrentHorizontalOffset = _totalHorizontalOffset;
                }
                return;
            }
            e.Handled = true;

            if (ScrollViewerAttach.GetOrientation(this) == Orientation.Vertical)
            {
                if (!_isRunning)
                {
                    _totalVerticalOffset = VerticalOffset;
                    CurrentVerticalOffset = VerticalOffset;
                }
                _totalVerticalOffset = Math.Min(Math.Max(0, _totalVerticalOffset - e.Delta), ScrollableHeight);

                if(_totalVerticalOffset == 0)
                {
                    this.OnScrollCommand?.Execute(new OnScrollEvent(-e.Delta, _totalVerticalOffset, 0, true, e.Delta < 0, this));
                }
                else
                {
                    ScrollToVerticalOffsetWithAnimation(_totalVerticalOffset);
                }
            }
            else
            {
                if (!_isRunning)
                {
                    _totalHorizontalOffset = HorizontalOffset;
                    CurrentHorizontalOffset = HorizontalOffset;
                }
                _totalHorizontalOffset = Math.Min(Math.Max(0, _totalHorizontalOffset - e.Delta), ScrollableWidth);
                ScrollToHorizontalOffsetWithAnimation(_totalHorizontalOffset);
            }
        }

        internal void ScrollToTopInternal(double milliseconds = 500)
        {
            if (!_isRunning)
            {
                _totalVerticalOffset = VerticalOffset;
                CurrentVerticalOffset = VerticalOffset;
            }
            ScrollToVerticalOffsetWithAnimation(0, milliseconds);
        }

        public void ScrollToVerticalOffsetWithAnimation(double offset, double milliseconds = 200)
        {
            if (double.IsNaN(offset) || double.IsInfinity(offset))
                return;

            var animation = CreateAnimation(offset, milliseconds);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                CurrentVerticalOffset = offset;
                _isRunning = false;
            };
            _isRunning = true;

            BeginAnimation(CurrentVerticalOffsetProperty, animation, HandoffBehavior.Compose);
        }

        public void ScrollToHorizontalOffsetWithAnimation(double offset, double milliseconds = 500)
        {
            if (double.IsNaN(offset) || double.IsInfinity(offset))
                return;

            var animation = CreateAnimation(offset, milliseconds);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                CurrentHorizontalOffset = offset;
                _isRunning = false;
            };
            _isRunning = true;

            BeginAnimation(CurrentHorizontalOffsetProperty, animation, HandoffBehavior.Compose);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) =>
            IsPenetrating ? null : base.HitTestCore(hitTestParameters);

        public static DoubleAnimation CreateAnimation(double toValue, double milliseconds = 200)
        {
            return new(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            };
        }
    }
}
