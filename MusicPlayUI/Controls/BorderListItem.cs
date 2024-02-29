using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DynamicScrollViewer;

namespace MusicPlayUI.Controls
{
    public class BorderListItem : Border, IIsInViewport
    {
        public bool IsInViewport
        {
            get { return (bool)GetValue(IsInViewportProperty); }
            set { SetValue(IsInViewportProperty, value); }
        }

        public static readonly DependencyProperty IsInViewportProperty =
            DependencyProperty.Register("IsInViewport", typeof(bool), typeof(BorderListItem), new PropertyMetadata(false, OnIsInViewPortChange));

        public BorderListItem()
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

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is IIsInViewport IsInViewportChild)
                {
                    IsInViewportChild.IsInViewport = IsInViewport;
                    continue;
                }

                PropagateIsInViewport(child, isInViewport);
            }

            return;
        }

        private static void OnIsInViewPortChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BorderListItem border && e.NewValue is bool newvalue)
            {
                border.IsInViewPortChange(newvalue);
            }
        }
    }
}
