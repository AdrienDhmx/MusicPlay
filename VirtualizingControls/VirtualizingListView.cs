using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using VirtualizingControls.Enums;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using DynamicScrollViewer;
using System.Windows.Media.Media3D;

namespace VirtualizingControls
{
    public class VirtualizingListView : ListView
    {
        public bool KeepSelectedItemInView
        {
            get { return (bool)GetValue(KeepSelectedItemInViewProperty); }
            set { SetValue(KeepSelectedItemInViewProperty, value); }
        }

        public static readonly DependencyProperty KeepSelectedItemInViewProperty =
            DependencyProperty.Register("KeepSelectedItemInView", typeof(bool), typeof(VirtualizingListView), new PropertyMetadata(false));

        /// <summary>
        /// Constructor initializing the <see cref="ItemsControl.ItemsPanel"/>.
        /// </summary>
        public VirtualizingListView()
        {
            VirtualizingPanel.SetCacheLengthUnit(this, VirtualizationCacheLengthUnit.Item);
            VirtualizingPanel.SetCacheLength(this, new VirtualizationCacheLength(6));
            VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);

            Loaded += VirtualizingListView_Loaded;
        }

        private void VirtualizingListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (KeepSelectedItemInView)
            {
                OnSelectionChanged(null);
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            var contentHost = Template.FindName("PART_ContentHost", this);

            if (contentHost is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta/3);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs? e)
        {
            if(e is not null)
                base.OnSelectionChanged(e);

            if (KeepSelectedItemInView && Items.Count > 0)
            {
                ScrollViewer? scrollViewer = Template.FindName("PART_ContentHost", this) as ScrollViewer;
                if(scrollViewer is not null)
                {
                    double itemHeight = scrollViewer.ExtentHeight / Items.Count;
                    double nbItemInViewPort = scrollViewer.ViewportHeight / itemHeight;
                    if(!double.IsNaN(nbItemInViewPort) && nbItemInViewPort != double.PositiveInfinity && nbItemInViewPort != double.NegativeInfinity)
                    {
                        double OffsetBeforeSelectedItem = (nbItemInViewPort * itemHeight) / 4;
                        double selectedItemOffset = SelectedIndex * itemHeight;
                        if (scrollViewer is DynamicScrollViewer.DynamicScrollViewer dynamicScrollViewer)
                        {
                            dynamicScrollViewer.ScrollToVerticalOffsetWithAnimation(selectedItemOffset - OffsetBeforeSelectedItem);
                        }
                        else
                        {
                            scrollViewer.ScrollToVerticalOffset(selectedItemOffset - OffsetBeforeSelectedItem);
                        }
                    }
                }
            }
        }
    }
}
