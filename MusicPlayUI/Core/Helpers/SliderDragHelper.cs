using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MusicPlayUI.Core.Helpers
{
    public interface ISliderDragTarget
    {
        void OnDragStart(double value);
        void OnDragEnd(double value);
    }

    public static class SliderDragHelper
    {
        public static bool GetIsDragEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragEnabledProperty);
        }

        public static void SetIsDragEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragEnabledProperty, value);
        }

        public static bool GetDragTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(DragTargetProperty);
        }

        public static void SetDragTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(DragTargetProperty, value);
        }

        public static readonly DependencyProperty IsDragEnabledProperty =
                DependencyProperty.RegisterAttached("IsDragEnabled", typeof(bool), typeof(SliderDragHelper), new PropertyMetadata(OnIsDragEnabledChanged));

        public static readonly DependencyProperty DragTargetProperty =
                DependencyProperty.RegisterAttached("DragTarget", typeof(object), typeof(SliderDragHelper), null);

        private static void OnIsDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            var slider = d as Slider;
            if (slider != null)
            {
                Thumb thumb = GetThumb(slider);
                if(thumb is not null)
                {
                    thumb.DragStarted += OnDragStart;
                    thumb.DragCompleted += OnDragCompleted;
                }
            }
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            DependencyObject d = sender as DependencyObject;
            if (d == null) return;

            // get slider
            Slider slider = d.GetVisualAncestor<Slider>();
            if (slider is null) return;

            // get target binded to the slider
            Object target = slider.GetValue(DragTargetProperty);
            ISliderDragTarget dragTarget = target as ISliderDragTarget;
            if (dragTarget != null)
            {
                dragTarget.OnDragEnd(slider.Value);
            }
            else
            {
                throw new Exception("Drag Target object must be of type ISliderDragTarget");
            }
        }

        private static void OnDragStart(object sender, DragStartedEventArgs e)
        {
            DependencyObject d = sender as DependencyObject;
            if (d == null) return;

            // get slider
            Slider slider = d.GetVisualAncestor<Slider>();
            if (slider is null) return;

            // get target binded to the slider
            Object target = slider.GetValue(DragTargetProperty);
            ISliderDragTarget dragTarget = target as ISliderDragTarget;
            if (dragTarget != null)
            {
                dragTarget.OnDragStart(slider.Value);
            }
            else
            {
                throw new Exception("Drag Target object must be of type ISliderDragTarget");
            }
        }

        private static Thumb GetThumb(Slider slider)
        {
            if(slider.Template is not null)
            {
                var track = slider.Template.FindName("PART_Track", slider) as Track;
                return track == null ? null : track.Thumb;
            }
            else if(slider is not null)
            {
                slider.Loaded += Slider_Loaded;
            }
            return null;
        }

        private static void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            GetThumb(sender as Slider);
            Thumb thumb = GetThumb(sender as Slider);
            if (thumb is not null)
            {
                thumb.DragStarted += OnDragStart;
                thumb.DragCompleted += OnDragCompleted;
            }
        }
    }
}
