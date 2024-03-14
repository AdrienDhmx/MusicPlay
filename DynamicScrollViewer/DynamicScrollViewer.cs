using System;
using System.Threading.Tasks;
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
        private double _speed = 3d;
        private bool _scrollingHeader = false;
        public bool IsInit { get; private set; } = false;
        private bool _needToScrollToItem = false;

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

        public object ItemToKeepInView
        {
            get { return (object)GetValue(ItemToKeepInViewProperty); }
            set { SetValue(ItemToKeepInViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemToKeepInView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemToKeepInViewProperty =
            DependencyProperty.Register("ItemToKeepInView", typeof(object), typeof(DynamicScrollViewer), new PropertyMetadata(null, OnItemToKeepInViewChange));

        public ICommand OnScrollCommand
        {
            get { return (ICommand)GetValue(OnScrollCommandProperty); }
            set { SetValue(OnScrollCommandProperty, value); }
        }

        public bool EnableLazyLoading
        {
            get { return (bool)GetValue(EnableLazyLoadingProperty); }
            set { SetValue(EnableLazyLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableLazyLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableLazyLoadingProperty =
            DependencyProperty.Register("EnableLazyLoading", typeof(bool), typeof(DynamicScrollViewer), new PropertyMetadata(false));

        public static readonly DependencyProperty IsInViewportProperty =
            DependencyProperty.RegisterAttached("IsInViewport", typeof(bool), typeof(DynamicScrollViewer));

        public static bool GetIsInViewport(UIElement element)
        {
            return (bool)element.GetValue(IsInViewportProperty);
        }

        public static void SetIsInViewport(UIElement element, bool value)
        {
            element.SetValue(IsInViewportProperty, value);
        }

        public FrameworkElement AnimatedHeader
        {
            get { return (FrameworkElement)GetValue(AnimatedHeaderProperty); }
            set { SetValue(AnimatedHeaderProperty, value); }
        }

        public bool AnimateHeaderOpacity
        {
            get { return (bool)GetValue(AnimateHeaderOpacityProperty); }
            set { SetValue(AnimateHeaderOpacityProperty, value); }
        }

        public static readonly DependencyProperty AnimateHeaderOpacityProperty =
            DependencyProperty.Register("AnimateHeaderOpacity", typeof(bool), typeof(DynamicScrollViewer), new PropertyMetadata(false));

        public static readonly DependencyProperty AnimatedHeaderProperty =
            DependencyProperty.Register("AnimatedHeader", typeof(FrameworkElement), typeof(DynamicScrollViewer), new PropertyMetadata(null));

        public static readonly DependencyProperty OnScrollCommandProperty =
            DependencyProperty.Register("OnScrollCommand", typeof(ICommand), typeof(DynamicScrollViewer), new PropertyMetadata(null, OnScrollCommandPropertyChanged));

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

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            Panel? panel = Content as Panel;
            if (panel == null)
                return;

            if(_needToScrollToItem)
            {
                ScrollToItem(ItemToKeepInView);
            }

            if (EnableLazyLoading)
            {
                UpdateIsInViewPort();
            }
        }

        private static void OnScrollCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicScrollViewer ctl && !ctl.IsInit && e.NewValue is ICommand)
            {
                // trigger the OnScrollCommand once before any scroll to poetentially let the receiver get a reference to this DynamicScrollviewer instance
                ctl.OnScrollCommand?.Execute(new OnScrollEvent(0, ctl.VerticalOffset, ctl.HorizontalOffset, ScrollViewerAttach.GetOrientation(ctl) == Orientation.Vertical, false, true, ctl));
            }
        }

        private static void OnStartingVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicScrollViewer ctl && !ctl.IsInit && e.NewValue is double newValue)
            {
                ctl.IsInit = true;
                if(newValue > 0)
                {
                    if (ctl.AnimatedHeader != null && ctl.VerticalOffset == 0)
                    {
                        // because the header is not part of the scroll even an offset of 1
                        // means that the header height is equal to its min height
                        ctl.AnimatedHeader.Height = ctl.AnimatedHeader.MinHeight;
                        if (ctl.AnimateHeaderOpacity)
                        {
                            ctl.AnimatedHeader.Opacity = 0;
                        }
                    }
                    ctl.ScrollToVerticalOffsetWithAnimation(newValue, 500);
                }
            }
        }

        private static void OnItemToKeepInViewChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicScrollViewer ctl)
                ctl.ScrollToItem(e.NewValue);
        }

        private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DynamicScrollViewer ctl && e.NewValue is double newValue && e.OldValue is double oldValue)
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
            if (IsInit && AnimatedHeader != null && !_scrollingHeader && e.VerticalOffset - e.VerticalChange == 0)
            {
                AnimateHeader(0, 800);
            }

            base.OnScrollChanged(e);


            if (!EnableLazyLoading && OnScrollCommand == null)
                return;

            // avoid calling the command every milliseconds => skip 2/3
            double newCallTick = DateTime.Now.TimeOfDay.TotalMilliseconds;
            // a bit more than 60FPS and if the vertical offset reaches 0 still call the command since it's an important value (animation...)
            if (newCallTick - _lastCallOnCommandMs < 20 && VerticalOffset != 0 && e.VerticalOffset != 0 && !_isRunning) 
            {
                return;
            }
            _lastCallOnCommandMs = newCallTick;

            if(EnableLazyLoading)
            {
                UpdateIsInViewPort();
            }

            if (OnScrollCommand == null)
                return;

            bool isVertical = e.VerticalChange != 0;
            bool isForward = e.VerticalChange > 0;
            double delta = e.VerticalChange;
            if(!isVertical)
            {
                isForward = e.HorizontalChange > 0;
                delta = e.HorizontalChange;
            }

            if(delta != 0)
                this.OnScrollCommand?.Execute(new OnScrollEvent(delta, e.VerticalOffset, e.HorizontalOffset, isVertical, isForward, _isRunning, this));
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

                this.OnScrollCommand?.Execute(new OnScrollEvent(e.Delta, verticalOffset, horizontalOffset, vertical, e.Delta > 0, _isRunning, this));
                return;
            }

            double ajustedDelta = e.Delta / _speed;
            if (AnimatedHeader != null && VerticalOffset == 0)
            {
                _scrollingHeader = ScrollHeader(ajustedDelta);

                if (_scrollingHeader)
                    return;
            }
            else
            {
                _scrollingHeader = false;
            }

            if (!IsInertiaEnabled)
            {
                if (ScrollViewerAttach.GetOrientation(this) == Orientation.Vertical)
                {
                    this.ScrollToVerticalOffset(this.VerticalOffset - ajustedDelta);
                }
                else
                {
                    _totalHorizontalOffset = HorizontalOffset;
                    CurrentHorizontalOffset = HorizontalOffset;
                    _totalHorizontalOffset = Math.Min(Math.Max(0, _totalHorizontalOffset - ajustedDelta), ScrollableWidth);
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
                    this.OnScrollCommand?.Execute(new OnScrollEvent(-e.Delta, _totalVerticalOffset, 0, true, e.Delta < 0, _isRunning, this));
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
            ScrollToVerticalOffsetWithAnimation(0, milliseconds);
        }

        public void ScrollToVerticalOffsetWithAnimation(double offset, double milliseconds = 200)
        {
            if (double.IsNaN(offset) || double.IsInfinity(offset))
                return;

            if (!_isRunning)
            {
                _totalVerticalOffset = VerticalOffset;
                CurrentVerticalOffset = VerticalOffset;
            }
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

        public void ScrollToItem(object item, bool lookForItemsControl = false)
        {
            Panel? panel = Content as Panel;

            if(lookForItemsControl)
            {
                ItemsControl? itemsControl = FindChild<ItemsControl>(this);
                if(itemsControl != null)
                {
                    StackPanel? stackPanel = FindChild<StackPanel>(itemsControl);
                    if(stackPanel != null)
                    {
                        panel = stackPanel;
                    }
                }
            }

            if (panel != null)
            {
                double totalHeightUntilItem = 0;
                double meanItemHeight = 0;
                int itemIndex = 0;
                bool itemIsFrameworkElement = item is FrameworkElement;

                foreach (FrameworkElement child in panel.Children)
                {
                    if (child.ActualHeight == 0 || double.IsNaN(child.ActualHeight))
                    {
                        break; // items not rendered yet, need to wait before scrolling to the item
                    }

                    if((itemIsFrameworkElement && item == child) || item == child.DataContext)
                    {
                        double spaceBeforeItem = meanItemHeight * 2; // show 2 items before this one
                        if(spaceBeforeItem > ViewportHeight)
                        {
                            spaceBeforeItem = ViewportHeight * 0.25;
                        }

                        _needToScrollToItem = false;
                        ScrollToVerticalOffsetWithAnimation(itemIndex * meanItemHeight - spaceBeforeItem, 400);
                        return;
                    }
                    totalHeightUntilItem += child.ActualHeight;
                    itemIndex++;
                    meanItemHeight = totalHeightUntilItem / itemIndex;
                }
                _needToScrollToItem = true;
            }
        }

        public void UpdateIsInViewPort()
        {
            Panel? panel = Content as Panel;
            if (panel != null)
            {
                Rect viewport = new Rect(new Point(0, 0), RenderSize);

                double totalHeight = 0;
                bool PassedViewport = false;
                bool isWrapPanel = panel is WrapPanel;
                int childcount = 1;
                foreach (FrameworkElement child in panel.Children)
                {
                    if (!child.IsVisible)
                    {
                        SetIsInViewport(child, false);
                        continue;
                    }

                    bool InViewport;
                    if (isWrapPanel)
                    { // require a more complex logic, the simplest way is to check if child rect intersect with the viewport rect
                        Rect childRect = child.TransformToAncestor(this).TransformBounds(new Rect(0, 0, child.RenderSize.Width, child.RenderSize.Height));
                        InViewport = childRect.IntersectsWith(viewport);
                    }
                    else
                    { // only need to check if the total height until this child is within the viewport (based on offset)
                        totalHeight += child.ActualHeight;
                        childcount++;
                        PassedViewport = totalHeight > VerticalOffset + viewport.Height + child.ActualHeight; // add 1 child in viewport
                        InViewport = totalHeight >= VerticalOffset - child.ActualHeight && !PassedViewport;
                    }

                    if (child is IIsInViewport IsInViewportChild)
                    {
                        if(IsInViewportChild.IsInViewport != InViewport)
                            IsInViewportChild.IsInViewport = InViewport;
                    } 
                    else
                    {
                        IsInViewportChild = FindChildByInterface<IIsInViewport>(child);
                        if (IsInViewportChild != null)
                        {
                            if (PassedViewport && !IsInViewportChild.IsInViewport)
                                return; // no more child to update (IsInViewport is false by default)

                            if (IsInViewportChild.IsInViewport != InViewport)
                                IsInViewportChild.IsInViewport = InViewport;
                        }
                        else
                        {
                            return; // no child with IsInViewportChild interface
                        }
                    }
                    SetIsInViewport(child, InViewport);
                }
            }
        }

        private bool AnimateHeader(double to, double duration = 300)
        {
            if (double.IsNaN(to) || double.IsInfinity(to) || AnimatedHeader == null)
                return false;

            var animation = CreateAnimation(to, duration);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;

            var storyboard = new Storyboard();
            Storyboard.SetTarget(animation, AnimatedHeader);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));

            storyboard.Children.Add(animation);

            storyboard.Completed += (s, e1) =>
            {
                AnimatedHeader.Height = to;
            };

            animation.CurrentTimeInvalidated += (s, e) =>
            {
                AnimatedHeader.Height = to;
                if (AnimateHeaderOpacity)
                {
                    double progress = AnimatedHeader.Height / AnimatedHeader.MaxHeight;
                    AnimatedHeader.Opacity = progress;
                }
                this.OnScrollCommand?.Execute(new OnScrollEvent(0, VerticalOffset, HorizontalOffset, true, to < AnimatedHeader.Height, _isRunning, this));
            };

            storyboard.Begin();

            return true;
        }


        private bool ScrollHeader(double ajustedDelta)
        {
            bool scrollForward = ajustedDelta < 0;
            bool animated = false;
            if (scrollForward && AnimatedHeader.Height > AnimatedHeader.MinHeight)
            {
                animated = true;
                if (AnimatedHeader.Height + ajustedDelta > AnimatedHeader.MinHeight)
                    AnimatedHeader.Height += ajustedDelta;
                else
                    AnimatedHeader.Height = AnimatedHeader.MinHeight;
            }
            else if (!scrollForward && AnimatedHeader.Height < AnimatedHeader.MaxHeight)
            {
                animated = true;
                if (AnimatedHeader.Height + ajustedDelta < AnimatedHeader.MaxHeight)
                    AnimatedHeader.Height += ajustedDelta;
                else
                    AnimatedHeader.Height = AnimatedHeader.MaxHeight;
            }

            if (animated)
            {
                if (AnimateHeaderOpacity)
                {
                    double progress = AnimatedHeader.Height / AnimatedHeader.MaxHeight;
                    AnimatedHeader.Opacity = progress;
                }
                this.OnScrollCommand?.Execute(new OnScrollEvent(0, VerticalOffset, HorizontalOffset, true, scrollForward, _isRunning, this));
            }
            return animated;
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

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    return (T)child;
                }

                T foundChild = FindChild<T>(child);
                if (foundChild != null)
                    return foundChild;
            }

            return null;
        }

        public static T FindChildByInterface<T>(DependencyObject parent) where T : class
        {
            if (parent == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                T foundChild = child as T;
                if (foundChild != null)
                {
                    return foundChild;
                }

                foundChild = FindChildByInterface<T>(child);
                if (foundChild != null)
                    return foundChild;
            }

            return null;
        }
    }
}
