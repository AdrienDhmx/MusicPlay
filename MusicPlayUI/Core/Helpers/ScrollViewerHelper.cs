using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Windows.Security.Authentication.Identity.Provider;

namespace MusicPlayUI.Core.Helpers
{
    public class ScrollViewerHelper
    {
        private static readonly double scrollSpeed = 3d;

        public static bool GetIsStickyEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsStickyEnabledProperty);
        }

        public static void SetIsStickyEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsStickyEnabledProperty, value);
        }

        public static ScrollViewer GetScrollViewer(DependencyObject obj)
        {
            return (ScrollViewer)obj.GetValue(ScrollViewerProperty);
        }

        public static void SetScrollViewer(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollViewerProperty, value);
        }

        public static FrameworkElement GetStickyElement(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(StickyElementProperty);
        }

        public static void SetStickyElement(DependencyObject obj, bool value)
        {
            obj.SetValue(StickyElementProperty, value);
        }

        public static object GetScrollTarget(DependencyObject obj)
        {
            return obj.GetValue(ScrollTargetProperty);
        }

        public static void SetScrollTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollTargetProperty, value);
        }

        public static readonly DependencyProperty IsStickyEnabledProperty =
            DependencyProperty.RegisterAttached("IsStickyEnabled", typeof(bool), typeof(ScrollViewerHelper));

        public static readonly DependencyProperty StickyElementProperty =
            DependencyProperty.RegisterAttached("StickyElement", typeof(FrameworkElement), typeof(ScrollViewerHelper));

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.RegisterAttached("ScrollViewer", typeof(ScrollViewer), typeof(ScrollViewerHelper), new PropertyMetadata(OnScrollViewerChanged));

        public static readonly DependencyProperty ScrollTargetProperty =
                DependencyProperty.RegisterAttached("ScrollTarget", typeof(object), typeof(ScrollViewerHelper));

        private static void OnScrollViewerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not null && d is Panel control)
            {
                control.PreviewMouseWheel += Control_PreviewMouseWheel;
            }
        }

        private static void Control_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            DependencyObject d = sender as DependencyObject;
            if (d == null) return;

            if (d is not null && d is Panel)
            {
                Object scrollSource = d.GetValue(ScrollViewerProperty);
                if (scrollSource is ScrollViewer scrollViewer)
                {
                    bool scroll = true;
                    double scrollValue = e.Delta / scrollSpeed;

                    Object isStickyProp = d.GetValue(IsStickyEnabledProperty);
                    Object stickyPart = d.GetValue(StickyElementProperty);
                    if (isStickyProp is bool isSticky && isSticky && stickyPart is FrameworkElement stickyElement)
                    {
                        if(scrollViewer.VerticalOffset == 0)
                        {
                            // scroll down
                            if (e.Delta < 0 && stickyElement.ActualHeight > stickyElement.MinHeight)
                            {
                                double height = stickyElement.ActualHeight + scrollValue /2;
                                if (height > 0)
                                {
                                    stickyElement.Height = height;
                                }
                                scroll = false;
                            }
                            //scroll up
                            else if (e.Delta > 0 && stickyElement.ActualHeight < stickyElement.MaxHeight)
                            {
                                stickyElement.Height = stickyElement.ActualHeight + scrollValue/2;
                                scroll = false;
                            }
                        }
                    }

                    if(scroll)
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - scrollValue);
                    else
                    {
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset);
                    }
                }
            }

        }
    }
}
