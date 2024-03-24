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
using System.Windows.Threading;

namespace LoadingControl
{
    public class LoadingControl : Canvas
    {
        public Brush MainColor
        {
            get { return (Brush)GetValue(MainColorProperty); }
            set { SetValue(MainColorProperty, value); }
        }

        public static readonly DependencyProperty MainColorProperty =
            DependencyProperty.Register("MainColor", typeof(Brush), typeof(LoadingControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 150, 150, 150))));

        public int BarCount
        {
            get { return (int)GetValue(BarCountProperty); }
            set { SetValue(BarCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarCountProperty =
            DependencyProperty.Register("BarCount", typeof(int), typeof(LoadingControl), new PropertyMetadata(4));

        private static Random rng = new Random();
        private static int barQty = 4;
        private double[] data = new double[barQty];
        private double[] minData= new double[barQty];
        private double[] maxData = new double[barQty];
        private bool[] increaseBool= new bool[barQty];
        private DispatcherTimer _dispatcherTimer = new();
        static LoadingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingControl), new FrameworkPropertyMetadata(typeof(LoadingControl)));
        }

        public override void EndInit()
        {
            base.EndInit();

            barQty = BarCount;
            data = new double[barQty];
            minData = new double[barQty];
            maxData = new double[barQty];
            increaseBool = new bool[barQty];
            
            IsVisibleChanged += LoadingControl_IsVisibleChanged;
            IsHitTestVisible = false;

            for (int i = 0; i < barQty; i++)
            {
                int min = 6;
                int max = (int)Height;

                // random starting value
                data[i] = Height / barQty * i;
                minData[i] = min;
                maxData[i] = max;
                increaseBool[i] = true;
            }

            _dispatcherTimer = new();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(20);
            _dispatcherTimer.Tick += Update;
            _dispatcherTimer.Start();
        }

        private void LoadingControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Collapsed || Visibility == Visibility.Hidden)
            {
                _dispatcherTimer.Stop();
            }
            else if (!_dispatcherTimer.IsEnabled)
            {
                _dispatcherTimer.Start();
            }
        }

        private void Update(object? sender, EventArgs e)
        {
            if (Children != null)
            {
                if(Children.Count >0) 
                {
                    Children.Clear();
                }

                double space = (ActualWidth / barQty);
                for (int i = 0; i < barQty; i++)
                {
                    Buff(i);

                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = space * 0.85;
                    rectangle.Height = data[i]; // min data values
                    rectangle.Fill = MainColor;
                    rectangle.StrokeThickness = 0;

                    rectangle.RadiusX = rectangle.Width / 4;
                    rectangle.RadiusY = rectangle.RadiusX;

                    SetLeft(rectangle, i * space);
                    SetBottom(rectangle, 0);
                    Children.Add(rectangle);
                }
            }
        }

        private void Buff(int index)
        {
            double value = data[index];
            if (value >= maxData[index])
            {
                increaseBool[index] = false;
            }
            else if (value <= minData[index])
            {
                increaseBool[index] = true;
            }

            if (increaseBool[index])
            {
                data[index] = value + 0.05 * value + 0.02 * Height;
            }
            else
            {
                data[index] = value - 0.05 * value - 0.02 * Height;
            }
        }
    }
}
