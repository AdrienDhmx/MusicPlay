using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DynamicScrollViewer;

namespace MusicPlayUI.Controls
{
    public class IsInViewportBorder : Border, IIsInViewport
    {
        private readonly List<IIsInViewport> _cachedIIsInViewportElements = [];

        public bool IsInViewport
        {
            get { return (bool)GetValue(IsInViewportProperty); }
            set { SetValue(IsInViewportProperty, value); }
        }

        public static readonly DependencyProperty IsInViewportProperty =
            DependencyProperty.Register("IsInViewport", typeof(bool), typeof(IsInViewportBorder), new PropertyMetadata(false, OnIsInViewPortChange));

        public IsInViewportBorder()
        {
        }

        private void IsInViewPortChange(bool isInViewport)
        {
            if (isInViewport)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
            PropagateIsInViewport(Child, isInViewport);
        }

        private void PropagateIsInViewport(DependencyObject parent, bool isInViewport)
        {
            if (parent == null)
                return;

            if(_cachedIIsInViewportElements.Count > 0)
            {
                bool canReturn = true;
                foreach(IIsInViewport element in _cachedIIsInViewportElements)
                {
                    if(element is null)
                    {
                        canReturn = false; // need to iterate trough children again to get back the reference
                        _cachedIIsInViewportElements.Clear();
                        break;
                    }
                    element.IsInViewport = isInViewport;
                }

                if(canReturn)
                {
                    return;
                }
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is IIsInViewport IsInViewportChild)
                {
                    IsInViewportChild.IsInViewport = IsInViewport;
                    _cachedIIsInViewportElements.Add(IsInViewportChild);
                    continue;
                }

                PropagateIsInViewport(child, isInViewport);
            }

            return;
        }

        private static void OnIsInViewPortChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is IsInViewportBorder border && e.NewValue is bool newvalue)
            {
                border.IsInViewPortChange(newvalue);
            }
        }
    }
}
