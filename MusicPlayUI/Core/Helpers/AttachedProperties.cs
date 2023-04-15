using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayUI.Core.Helpers
{
    class AttachedProperties
    {
        public static bool GetObserve(DependencyObject obj)
        {
            return (bool)obj.GetValue(ObserveProperty);
        }

        public static void SetObserve(DependencyObject obj, bool value)
        {
            obj.SetValue(ObserveProperty, value);
        }

        public static readonly DependencyProperty ObserveProperty =
            DependencyProperty.RegisterAttached("Observe", typeof(bool), typeof(AttachedProperties), new UIPropertyMetadata(false, ObserveChanged));

        static void ObserveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ele = obj as FrameworkElement;
            if ((bool)e.NewValue)
                ele.SizeChanged += new SizeChangedEventHandler(ele_SizeChanged);
            else
                ele.SizeChanged -= new SizeChangedEventHandler(ele_SizeChanged);
        }

        static void ele_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var ele = sender as FrameworkElement;
            ele.SetCurrentValue(ObservedHeightProperty, ele.ActualHeight);
        }

        public static double GetObservedHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ObservedHeightProperty);
        }

        public static void SetObservedHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ObservedHeightProperty, value);
        }

        public static readonly DependencyProperty ObservedHeightProperty =
            DependencyProperty.RegisterAttached("ObservedHeight", typeof(double), typeof(AttachedProperties), new UIPropertyMetadata(0.0D));
    }
}
