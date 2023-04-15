using PlaceHolderTextBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Stepper
{
    public class Stepper : TextBox
    {
        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            private set { SetValue(IsEmptyPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsEmptyPropertyKey =
            DependencyProperty.RegisterReadOnly("IsEmpty", typeof(bool), typeof(Stepper), new PropertyMetadata(true));

        public static readonly DependencyProperty IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(Stepper), new PropertyMetadata(string.Empty));

        public Brush PlaceholderForeground
        {
            get { return (Brush)GetValue(PlaceholderForegroundProperty); }
            set { SetValue(PlaceholderForegroundProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(Stepper), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Stepper), new PropertyMetadata(new CornerRadius(0)));

        public double StepValue
        {
            get { return (double)GetValue(StepValueProperty); }
            set { SetValue(StepValueProperty, value); }
        }

        public static readonly DependencyProperty StepValueProperty =
            DependencyProperty.Register("StepValue", typeof(double), typeof(Stepper), new PropertyMetadata(1d));

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(Stepper), new PropertyMetadata(string.Empty));

        public Brush BtnBackgroundColor
        {
            get { return (Brush)GetValue(BtnBackgroundColorProperty); }
            set { SetValue(BtnBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BtnBackgroundColorProperty =
            DependencyProperty.Register("BtnBackgroundColor", typeof(Brush), typeof(Stepper), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));


        public Brush MouseOverBtnColor
        {
            get { return (Brush)GetValue(MouseOverBtnColorProperty); }
            set { SetValue(MouseOverBtnColorProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBtnColorProperty =
            DependencyProperty.Register("MouseOverBtnColor", typeof(Brush), typeof(Stepper), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(Stepper), new PropertyMetadata(0d));

        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(Stepper), new PropertyMetadata(100d));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Stepper), new PropertyMetadata(0d));

        private DispatcherTimer _increaseBtnTimer = new();
        private DispatcherTimer _decreaseBtnTimer = new();
        private int _timerElapsed = 250;
        private string PreviousText { get; set; } = "";
        static Stepper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Stepper), new FrameworkPropertyMetadata(typeof(Stepper)));
        }

        public Stepper()
        {
            AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(Init));
        }

        private void Init(object sender, RoutedEventArgs e)
        {
            _increaseBtnTimer.Interval = TimeSpan.FromMilliseconds(_timerElapsed);
            _decreaseBtnTimer.Interval = TimeSpan.FromMilliseconds(_timerElapsed);

            _increaseBtnTimer.Tick += IncreaseBtnTimer_Tick;
            _decreaseBtnTimer.Tick += DecreaseBtnTimer_Tick;

            if (Value < Min)
            {
                Value = Min;
            }
            else if (Value > Max)
            {
                Value = Max;
            }

            Text = Value.ToString();

            object increaseElement = this.Template.FindName("IncreaseBtn", this);
            object decreaseElement = this.Template.FindName("DecreaseBtn", this);            

            if (increaseElement is Border increaseBorder && decreaseElement is Border decreaseBorder)
            {
                increaseBorder.MouseLeftButtonDown += IncreaseMouseDown;
                decreaseBorder.MouseLeftButtonDown += DecreaseMouseDown;

                increaseBorder.MouseLeftButtonUp += IncreaseMouseUp;
                decreaseBorder.MouseLeftButtonUp += DecreaseMouseUp;

                increaseBorder.MouseLeave += IncreaseBorder_MouseLeave;
                decreaseBorder.MouseLeave += DecreaseBorder_MouseLeave;
            }
        }

        private void DecreaseBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            _decreaseBtnTimer.Stop();
            _decreaseBtnTimer.Interval = TimeSpan.FromMilliseconds(_timerElapsed);
        }

        private void IncreaseBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            _increaseBtnTimer.Stop();
            _increaseBtnTimer.Interval = TimeSpan.FromMilliseconds(_timerElapsed);
        }

        private void DecreaseBtnTimer_Tick(object? sender, EventArgs e)
        {
            Decrease();

            if(_decreaseBtnTimer.Interval.TotalMilliseconds > 10) // increase speed
                _decreaseBtnTimer.Interval = _decreaseBtnTimer.Interval.Subtract(TimeSpan.FromMilliseconds(10));
        }

        private void IncreaseBtnTimer_Tick(object? sender, EventArgs e)
        {
            Increase();

            if (_increaseBtnTimer.Interval.TotalMilliseconds > 10) // increase speed
                _increaseBtnTimer.Interval = _increaseBtnTimer.Interval.Subtract(TimeSpan.FromMilliseconds(10));
        }

        private void IncreaseMouseDown(object sender, MouseButtonEventArgs e)
        {
            _increaseBtnTimer.Start();
            Increase(); // still increase once
        }

        private void IncreaseMouseUp(object sender, MouseButtonEventArgs e)
        {
            _increaseBtnTimer.Stop();
            _increaseBtnTimer.Interval = TimeSpan.FromMilliseconds(_timerElapsed);
        }

        private void DecreaseMouseDown(object sender, MouseButtonEventArgs e)
        {
            _decreaseBtnTimer.Start();
            Decrease(); // sitll decrease once
        }

        private void DecreaseMouseUp(object sender, MouseButtonEventArgs e)
        {
            _decreaseBtnTimer.Stop();
            _decreaseBtnTimer.Interval = TimeSpan.FromMilliseconds(_timerElapsed);
        }

        private void Increase()
        {
            // decimal to avoid some weird rounding errors like 2.4d + 0.2d = 2.55555555559d
            if (decimal.TryParse(Text, out decimal value) && value < (decimal)Max)
            {
                value += (decimal)StepValue;
                Value = (double)value;
                Text = value.ToString();
            }
        }

        private void Decrease()
        {
            if (decimal.TryParse(Text, out decimal value) && value > (decimal)Min)
            {
                value -= (decimal)StepValue;
                Value = (double)value;
                Text = value.ToString();
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            IsEmpty = string.IsNullOrEmpty(Text);

            if (!IsEmpty)
            {
                if (!double.TryParse(Text, out double value))
                {
                    Text = PreviousText;
                }
                // not valid but let user rewrite the number freely
                else if (value < Min || value > Max)
                {
                    PreviousText = Text;
                }
                else
                {
                    PreviousText = Text;
                    Value = value;
                }
            }
        }
    }
}
