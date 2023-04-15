using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VirtualizingControls
{
    public class VirtualizingItemsControl : ItemsControl
    {
        /// <summary>
        /// Property for <see cref="CacheLengthUnit"/>.
        /// </summary>
        public static readonly DependencyProperty CacheLengthUnitProperty =
            DependencyProperty.Register(nameof(CacheLengthUnit), typeof(VirtualizationCacheLengthUnit), typeof(VirtualizingItemsControl),
                new FrameworkPropertyMetadata(VirtualizationCacheLengthUnit.Page));

        /// <summary>
        /// Gets or sets the cache length unit.
        /// </summary>
        public VirtualizationCacheLengthUnit CacheLengthUnit
        {
            get => VirtualizingPanel.GetCacheLengthUnit(this);
            set
            {
                SetValue(CacheLengthUnitProperty, value);
                VirtualizingPanel.SetCacheLengthUnit(this, value);
            }
        }

        public double CacheLength
        {
            get { return (double)GetValue(CacheLengthProperty); }
            set { SetValue(CacheLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CacheLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CacheLengthProperty =
            DependencyProperty.Register("CacheLength", typeof(double), typeof(VirtualizingItemsControl), new PropertyMetadata(1d));

        /// <summary>
        /// Constructor that initialize the <see cref="VirtualizingPanel"/>.
        /// </summary>
        public VirtualizingItemsControl()
        {
            VirtualizingPanel.SetCacheLengthUnit(this, CacheLengthUnit);
            VirtualizingPanel.SetCacheLength(this, new VirtualizationCacheLength(CacheLength));
            VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);
        }

        static VirtualizingItemsControl() 
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualizingItemsControl), new FrameworkPropertyMetadata(typeof(VirtualizingItemsControl)));
        }
    }
}
