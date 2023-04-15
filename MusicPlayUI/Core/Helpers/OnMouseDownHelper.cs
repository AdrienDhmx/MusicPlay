using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MusicPlayUI.Core.Helpers
{
    public interface IOnMouseDownListener
    {
        void OnClick(MouseButtonEventArgs e);
    }

    public static class OnMouseDownHelper
    {
        public static IOnMouseDownListener GetOnClickListener(DependencyObject obj)
        {
            return (IOnMouseDownListener)obj.GetValue(OnClickListenerProperty);
        }

        public static void SetOnClickListener(DependencyObject obj, bool value)
        {
            obj.SetValue(OnClickListenerProperty, value);
        }

        public static readonly DependencyProperty OnClickListenerProperty =
                DependencyProperty.RegisterAttached("OnClickListener", typeof(IOnMouseDownListener), typeof(OnMouseDownHelper), new PropertyMetadata(OnClickListenerChanged));

        private static void OnClickListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is null) return;

            object target = d.GetValue(OnClickListenerProperty);

            if (target is IOnMouseDownListener && d is FrameworkElement element)
            {
                element.PreviewMouseLeftButtonDown += Element_MouseLeftButtonDown;
            }
        }

        private static void Element_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject d = sender as DependencyObject;
            if (d == null) return;

            object target = d.GetValue(OnClickListenerProperty);

            if (target is IOnMouseDownListener listener)
            {
                listener.OnClick(e);
            }
        }
    }
}
