using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using MusicPlayUI.MVVM.ViewModels;
using Microsoft.UI;
using WinRT.Interop;
using System.Runtime.InteropServices;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Shell;

namespace MusicPlayUI.MVVM.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WM_MOUSEHWHEEL = 0x020E;
        const int WM_NCHITTEST = 0x0084;

        public MainWindow()
        {
            InitializeComponent();
            PreviewKeyDown += OnPreviewKeyDownHandler;
            Focus();
        }

        private void OnPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space ||
                e.Key == Key.Left || e.Key == Key.Right ||
                 e.Key == Key.Up || e.Key == Key.Down)
            {
                var element = Keyboard.FocusedElement;
                if(element != null && element as TextBox == null && element as MainWindow == null)
                {
                    FocusManager.SetFocusedElement(this, this);
                }
                e.Handled = false; // not handled because we want the input bindings to call their commands (play, previous, next...)
            }
        }

        private void WindowTopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // add horizontal scrolling support with touchpad
        protected override void OnSourceInitialized(EventArgs e)
        {
            var source = PresentationSource.FromVisual(this);
            ((HwndSource)source)?.AddHook(Hook);
        }

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEHWHEEL:
                    int tilt = (short)HIWORD(wParam);
                    OnMouseTilt(tilt);
                    return (IntPtr)1;
            }

            return IntPtr.Zero;
        }

        private void OnMouseTilt(int tilt)
        {
            UIElement element = Mouse.DirectlyOver as UIElement;

            if (element == null) return;

            ScrollViewer scrollViewer = element is ScrollViewer viewer ? viewer : FindParent<ScrollViewer>(element);

            if (scrollViewer == null || scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled)
                return;

            //scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + tilt);
            scrollViewer?.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + tilt / 3);
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        /// <summary>
        /// Gets high bits values of the pointer.
        /// </summary>
        private static int HIWORD(IntPtr ptr)
        {
            unchecked
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    var val64 = ptr.ToInt64();
                    return (short)((val64 >> 16) & 0xFFFF);
                }
                var val32 = ptr.ToInt32();
                return (short)((val32 >> 16) & 0xFFFF);
            }
        }

        /// <summary>
        /// Gets low bits values of the pointer.
        /// </summary>
        private static int LOWORD(IntPtr ptr)
        {
            unchecked
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    var val64 = ptr.ToInt64();
                    return (short)(val64 & 0xFFFF);
                }

                var val32 = ptr.ToInt32();
                return (short)(val32 & 0xFFFF);
            }
        }
    }
}
