using System;
using System.Collections.Generic;
using System.Printing;
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

        public double PlaceholderHeight
        {
            get { return (double)GetValue(PlaceholderHeightProperty); }
            set { SetValue(PlaceholderHeightProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderHeightProperty =
            DependencyProperty.Register("PlaceholderHeight", typeof(double), typeof(IsInViewportBorder), new PropertyMetadata(double.NaN));


        public double PlaceholderWidth
        {
            get { return (double)GetValue(PlaceholderWidthProperty); }
            set { SetValue(PlaceholderWidthProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderWidthProperty =
            DependencyProperty.Register("PlaceholderWidth", typeof(double), typeof(IsInViewportBorder), new PropertyMetadata(double.NaN));


        public Brush BackgroundPlaceholder
        {
            get { return (Brush)GetValue(BackgroundPlaceholderProperty); }
            set { SetValue(BackgroundPlaceholderProperty, value); }
        }

        public static readonly DependencyProperty BackgroundPlaceholderProperty =
            DependencyProperty.Register("BackgroundPlaceholder", typeof(Brush), typeof(IsInViewportBorder), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255)), OnBackgroundPlaceholderChanged));

        private If _if;
        public IsInViewportBorder()
        {
            SetIf();
        }

        private void SetIf()
        {
            if (Child is not null)
            {
                _if = GetIf();
                if(_if != null)
                {
                    Child = _if;
                }
            }
        }

        private If GetIf()
        {
            Border placeholder = GetPlaceholderBorder();
            if( placeholder != null )
            {
                return new If()
                {
                    Condition = IsInViewport,
                    True = Child,
                    False = placeholder,
                };
            }
            return null;
        }

        private Border GetPlaceholderBorder()
        {
            double width = double.IsNaN(PlaceholderWidth) 
                            ? double.IsNaN(Width) 
                                ? double.IsNaN(ActualWidth) 
                                    ? 0 
                                    : ActualWidth 
                                : Width
                            : PlaceholderWidth;

            double height = double.IsNaN(PlaceholderHeight) 
                            ? double.IsNaN(Height)
                                ? double.IsNaN(ActualHeight)
                                    ? 0
                                    : ActualHeight
                                : Height
                            : PlaceholderHeight;

            return new Border()
            {
                Width = width,
                Height = height,
                Background = BackgroundPlaceholder,
                BorderBrush = BorderBrush,
                BorderThickness = BorderThickness,
                CornerRadius = CornerRadius,
            };
        }

        private void IsInViewPortChange(bool isInViewport)
        {
            if(_if == null)
            {
                SetIf();
            }

            if(_if != null)
            {
                _if.Condition = isInViewport;
                if(isInViewport)
                {
                    PropagateIsInViewport(_if.True, isInViewport);
                }
            }

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


        private static void OnBackgroundPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IsInViewportBorder border && e.NewValue is Brush newvalue)
            {
                if(border._if != null)
                {
                    (border._if.False as Border).Background = newvalue;
                }
            }
        }
    }
}
