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

        private static Random rng = new Random();
        private static readonly int barQty = 4;
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
            
            IsVisibleChanged += LoadingControl_IsVisibleChanged;
            IsHitTestVisible = false;

            for (int i = 0; i < barQty; i++)
            {
                int min = rng.Next(4, (int)(Height / 3));
                int max = rng.Next((int)(Height / 2) + min/2, (int)Height);

                data[i] = min;
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
                    rectangle.Width = space * 0.95;
                    rectangle.Height = data[i]; // min data values
                    rectangle.Fill = MainColor;
                    rectangle.Stroke = MainColor;
                    rectangle.StrokeThickness = 1;

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
                data[index] = value + 0.1 * value;
            }
            else
            {
                data[index] = value - 0.1 * value;
            }
        }
    }
}
