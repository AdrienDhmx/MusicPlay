using System;
using ManagedBass;
using SpectrumVisualizer.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Brush = System.Windows.Media.Brush;
using System.Collections.Generic;

namespace SpectrumVisualizer
{
    public class SpectrumVisualizer : Control, IDisposable
    {
        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _visuals[index];
        }


        public bool CenterFreqRange
        {
            get { return (bool)GetValue(CenterFreqRangeProperty); }
            set { SetValue(CenterFreqRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterFreqRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterFreqRangeProperty =
            DependencyProperty.Register("CenterFreqRange", typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(false, CenterFreqrangeChanged));


        public bool Fill
        {
            get { return (bool)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public bool SmoothGraph
        {
            get { return (bool)GetValue(SmoothGraphProperty); }
            set { SetValue(SmoothGraphProperty, value); }
        }

        public ObjectLengthEnum ObjectLength
        {
            get { return (ObjectLengthEnum)GetValue(ObjectLengthProperty); }
            set { SetValue(ObjectLengthProperty, value); }
        }

        public double CutPercentage
        {
            get { return (double)GetValue(CutPercentageProperty); }
            set { SetValue(CutPercentageProperty, value); }
        }

        public bool CutHighFreq
        {
            get { return (bool)GetValue(CutHighFreqProperty); }
            set { SetValue(CutHighFreqProperty, value); }
        }

        public FrameRateEnum FrameRate
        {
            get { return (FrameRateEnum)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        public DataRepresentationTypeEnum RepresentationType
        {
            get { return (DataRepresentationTypeEnum)GetValue(RepresentationTypeProperty); }
            set { SetValue(RepresentationTypeProperty, value); }
        }

        public bool Gradient
        {
            get { return (bool)GetValue(GradientProperty); }
            set { SetValue(GradientProperty, value); }
        }

        public DataQuantityEnum DataQuantity
        {
            get { return (DataQuantityEnum)GetValue(DataQuantityProperty); }
            set { SetValue(DataQuantityProperty, value); }
        }

        public int Stream
        {
            get { return (int)GetValue(StreamProperty); }
            set { SetValue(StreamProperty, value); }
        }

        public Brush ObjectColor
        {
            get { return (Brush)GetValue(ObjectColorProperty); }
            set { SetValue(ObjectColorProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(1d, Redraw));

        public static readonly DependencyProperty SmoothGraphProperty =
            DependencyProperty.Register("SmoothGraph", typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(true));

        public static readonly DependencyProperty CutPercentageProperty =
            DependencyProperty.Register("CutPercentage", typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(0.6d, CutPercentageChanged));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(true, Redraw));

        public static readonly DependencyProperty ObjectLengthProperty =
            DependencyProperty.Register("ObjectLength", typeof(ObjectLengthEnum), typeof(SpectrumVisualizer), new PropertyMetadata(ObjectLengthEnum.Normal, BarThicknessChanged));

        public static readonly DependencyProperty CutHighFreqProperty =
            DependencyProperty.Register("CutHighFreq", typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(false, CutHighfreqChanged));

        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(FrameRateEnum), typeof(SpectrumVisualizer), new PropertyMetadata(FrameRateEnum.High, FrameRateChanged));

        public static readonly DependencyProperty RepresentationTypeProperty =
            DependencyProperty.Register("RepresentationType", typeof(DataRepresentationTypeEnum), typeof(SpectrumVisualizer), new PropertyMetadata(DataRepresentationTypeEnum.LinearUpwardBar, RepresentationTypeChanged));

        public static readonly DependencyProperty GradientProperty =
            DependencyProperty.Register("Gradient", typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(false, Redraw));

        public static readonly DependencyProperty DataQuantityProperty =
            DependencyProperty.Register("DataQuantity", typeof(DataQuantityEnum), typeof(SpectrumVisualizer), new PropertyMetadata(DataQuantityEnum.FFT256, OnDataQuantityChanged));

        public static readonly DependencyProperty StreamProperty =
            DependencyProperty.Register("Stream", typeof(int), typeof(SpectrumVisualizer), new PropertyMetadata(0, OnStreamChanged));

        public static readonly DependencyProperty ObjectColorProperty =
            DependencyProperty.Register("ObjectColor", typeof(Brush), typeof(SpectrumVisualizer), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250)), Redraw));


        private static void Redraw(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _gradientCalculated = false;
        }

        private static void RepresentationTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemWidthMustChange();
        }

        private static void BarThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _barThickness = (int)e.NewValue;
            ItemWidthMustChange();
        }

        private static void CutHighfreqChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _cutHighFreq = (bool)e.NewValue;
            SetDataRendered();
            ItemWidthMustChange();
        }

        private static void CenterFreqrangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool.TryParse(e.NewValue.ToString(), out _centerFreqRange);
            _gradientCalculated = false;
        }


        private static void CutPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _cutPercentage = (double)e.NewValue;
            SetDataRendered();
            ItemWidthMustChange();
        }

        private static void FrameRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _frameRate = (FrameRateEnum)e.NewValue;
            SetDispatcherTimerInterval();
        }

        private static void ItemWidthMustChange()
        {
            _itemWidthCalculated = false;
        }

        private static void OnDataQuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            dispatcherTimer.Stop();

            _dataQuantity = (int)e.NewValue;
            SetDataRendered();

            fft = new float[_dataQuantity];
            bandBuffer = new float[_dataQuantity];
            _bufferDecrease = new float[_dataQuantity];

            ItemWidthMustChange();

            SetDispatcherTimerInterval();
            dispatcherTimer.Start();
        }

        private static void OnStreamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _stream = (int)e.NewValue;
        }

        private static VisualCollection _visuals;

        private static int _stream;
        private static int _dataQuantity = 512;
        public static float[] fft = new float[_dataQuantity];
        public static float[] bandBuffer = new float[_dataQuantity];
        private static float[] _bufferDecrease = new float[_dataQuantity];
        private static List<SolidColorBrush> _gradientvalues = new();

        private static bool _gradientCalculated = false;
        private static long _currentpos = -1;

        private static bool _cutHighFreq = false;
        private static double _cutPercentage = 0.6;
        private static int _dataRendered = 512;
        private static bool _centerFreqRange = false;

        private static readonly double diameterCoeff = 0.6;
        private static readonly double circleSizeMultiplier = 0.25;
        private static readonly double linearSizeMultiplier = 0.8;
        private static readonly double linearMirroredSizeCoeff = 0.5;

        private static int _barThickness = 1;
        private readonly double _minHeight = 2;

        private static double _itemWidth = 0;
        private static double _space = 0;
        private static bool _itemWidthCalculated = false;

        private static FrameRateEnum _frameRate;
        private static readonly int lowFrameRate = 33; // 30.30 fps
        private static readonly int highFrameRate = 17; // 58.8 fps
        private static readonly int veryHighFrameRate = 8; // 125 fps

        private static readonly float lowBuffIncrease = 1.4f;
        private static readonly float lowBuffDecrease = 0.004f;
        private static readonly float highBuffIncrease = 1.2f;
        private static readonly float highBuffDecrease = 0.004f;
        private static readonly float veryHighBuffIncrease = 1.05f;
        private static readonly float veryHighBuffDecrease = 0.004f;

        private static float BuffIncrease = highBuffIncrease;
        private static float BuffDecrease = highBuffDecrease;

        private static readonly DispatcherTimer dispatcherTimer = new();
        static SpectrumVisualizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumVisualizer), new FrameworkPropertyMetadata(typeof(SpectrumVisualizer)));
        }

        public SpectrumVisualizer()
        {
            _visuals = new(this);
        }

        public override void EndInit()
        {
            base.EndInit();
            
            IsManipulationEnabled= false;
            IsHitTestVisible = false;
           
            _stream = Stream;
            _dataQuantity = (int)DataQuantity;
            fft = new float[_dataQuantity];
            bandBuffer = new float[_dataQuantity];
            _bufferDecrease = new float[_dataQuantity];

            _centerFreqRange = CenterFreqRange;

            _cutHighFreq = CutHighFreq;
            _cutPercentage = CutPercentage;
            SetDataRendered();

            GetData();

            _barThickness = (int)ObjectLength;

            dispatcherTimer.Tick += Update;
            _frameRate = FrameRate;
            SetDispatcherTimerInterval();
            _itemWidthCalculated = false;

            if(Visibility == Visibility.Collapsed || Visibility == Visibility.Hidden)
            {
                dispatcherTimer.Stop();
            }

            this.IsVisibleChanged += SpectrumVisualizer_IsVisibleChanged;
        }

        private void SpectrumVisualizer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue as Visibility? == Visibility.Collapsed || e.NewValue as Visibility? == Visibility.Hidden)
            {
                dispatcherTimer.Stop();
            }
            else if(!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
            }
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            // when the base class calculate the size the visual drawing are being casted to UIElement
            // they need to be cleared
            _visuals.Clear();
            System.Windows.Size size = base.MeasureOverride(constraint);
            ItemWidthMustChange();
            return size;
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeBounds)
        {
            // when the base class calculate the size the visual drawing are being casted to UIElement
            // they need to be cleared
            dispatcherTimer.Stop();
            _visuals.Clear();
            System.Windows.Size size = base.ArrangeOverride(arrangeBounds);
            ItemWidthMustChange();
            dispatcherTimer.Start();
            return size;
        }

        private static void SetDataRendered()
        {
            _gradientCalculated = false;
            if (!_cutHighFreq)
            {
                _dataRendered = _dataQuantity;
            }
            else
            {
                _dataRendered = (int)(_dataQuantity * _cutPercentage);
            }
            if (_dataRendered % 2 != 0)
            {
                _dataRendered++;
            }
        }
        private static void SetDispatcherTimerInterval()
        {
            dispatcherTimer.Stop();
            if (_frameRate == FrameRateEnum.Low)
            {
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, lowFrameRate);
                BuffDecrease = lowBuffDecrease;
                BuffIncrease = lowBuffIncrease;
            }
            else if (_frameRate == FrameRateEnum.High)
            {
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, highFrameRate);
                BuffDecrease = highBuffDecrease;
                BuffIncrease = highBuffIncrease;
            }
            else if (_frameRate == FrameRateEnum.VeryHigh)
            {
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, veryHighFrameRate);
                BuffDecrease = veryHighBuffDecrease;
                BuffIncrease = veryHighBuffIncrease;
            }
            else
            {
                if (_dataRendered >= 1024)
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, lowFrameRate);
                    BuffDecrease = lowBuffDecrease;
                    BuffIncrease = lowBuffIncrease;
                }
                else
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, highFrameRate);
                    BuffDecrease = highBuffDecrease;
                    BuffIncrease = highBuffIncrease;
                }
            }
            dispatcherTimer.Start();
        }

        private void SetItemWidth()
        {
            if (RepresentationType == DataRepresentationTypeEnum.CircledBar || RepresentationType == DataRepresentationTypeEnum.CircledPoints)
            {

                _itemWidth = GetCircledBarWidth(GetCircleRadius());
            }
            else
            {
                _itemWidth = GetBarWidth(ActualWidth, out _space);
            }

            _itemWidthCalculated = _itemWidth!=0;
        }

        private void Update(object? sender, EventArgs e)
        {
            if (_currentpos != Bass.ChannelGetPosition(_stream) && GetData())
            {
                _visuals.Clear();
                if (!_itemWidthCalculated)
                {
                    dispatcherTimer.Stop();
                    SetItemWidth();
                    dispatcherTimer.Start();
                }
                switch (RepresentationType)
                {
                    case DataRepresentationTypeEnum.LinearUpwardBar:
                        _visuals.Add(DrawLinearBar());
                        break;
                    case DataRepresentationTypeEnum.LinearDownwardBar:
                        _visuals.Add(DrawLinearBar());
                        break;
                    case DataRepresentationTypeEnum.CircledBar:
                        _visuals.Add(DrawCircledBar());
                        break;
                    case DataRepresentationTypeEnum.LinearMirroredBar:
                        _visuals.Add(DrawLinearMirroredRectangle());
                        break;
                    case DataRepresentationTypeEnum.LinearUpwardPoints:
                        _visuals.Add(DrawLinearPoints());
                        break;
                    case DataRepresentationTypeEnum.LinearDownwardPoints:
                        _visuals.Add(DrawLinearPoints());
                        break;
                    case DataRepresentationTypeEnum.CircledPoints:
                        _visuals.Add(DrawCircledPoints());
                        break;
                    case DataRepresentationTypeEnum.LinearMirroredPoints:
                        _visuals.Add(DrawMirroredPoint());
                        break;
                }
            }
            _currentpos = Bass.ChannelGetPosition(_stream);
        }

        public DrawingVisual DrawLinearBar()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext context = drawingVisual.RenderOpen();

            double baseY = _minHeight;
            if (RepresentationType == DataRepresentationTypeEnum.LinearUpwardBar)
            {
                baseY = ActualHeight - _minHeight;
            }

            double amp = linearSizeMultiplier * ActualHeight;
            for (int i = 0; i < _dataRendered; i++)
            {
                BuffData(i);

                double height = _minHeight + bandBuffer[i] * amp;
                double PosY = baseY;
                if (RepresentationType == DataRepresentationTypeEnum.LinearUpwardBar)
                {
                    PosY = baseY - height;
                }

                Rect rect = new(i * (_itemWidth + _space), PosY, _itemWidth, height);
                DrawShape(context, rect, i);
            }

            context.Close();
            return drawingVisual;
        }

        private DrawingVisual DrawCircledBar()
        {
            (double CircleCenterX, double CircleCenterY, double diameter) = GetCircleProperties();
            double radius = diameter / 2;

            // coeff to determine the width and height cornerRadius (0 = 0 and 1 = 360)
            double coeff1;
            double PointX;
            double PointY;
            int quarter = _dataRendered / 4; // divide the data in 4 to represent the 4 quarter of a circle
            double multiplier = circleSizeMultiplier * ActualHeight; // multiplie the buffed data from the fft to make it visible

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext context = drawingVisual.RenderOpen(); // allow the rendering of new objects

            double angle;
            RotateTransform rotateTransform;
            for (int i = 0; i < _dataRendered; i++)
            {
                BuffData(i);
                coeff1 = (double)i / (double)_dataRendered;
                double coeff2 = 1 - coeff1;

                angle = ((360 * Math.PI / 180) * coeff1) - Math.PI / 2; // the angle is in radians

                double Height = _minHeight + bandBuffer[i] * multiplier; // the height of the rectangle to draw
                double heightcoeff = Height; // used to find calculate the y position of the top left corner of the rectangle
                if (i > quarter && i < 3 * quarter)
                {
                    heightcoeff = 0; // the top left corner is already on the circle (y under the center of the circle)
                }

                if (_centerFreqRange)
                {
                    PointY = CircleCenterY + radius * Math.Sin(angle + Math.PI);
                    PointX = CircleCenterX + radius * Math.Cos(angle + Math.PI);
                }
                else
                {
                    PointY = CircleCenterY + radius * Math.Sin(angle);
                    PointX = CircleCenterX + radius * Math.Cos(angle);
                }

                Rect rect = new(PointX, PointY - heightcoeff, _itemWidth, Height);
                angle = 360 * coeff2; // recalculate the angle in degree to rotate the rectangle

                // 360 - angle because the rotation must go counterclockwise
                // the center of the rotation is the point on the circle to anchor the rect on it
                if (_centerFreqRange)
                {
                    if (i > quarter && i < 3 * quarter)
                    {
                        rotateTransform = new RotateTransform(360 - angle, rect.Left + _itemWidth / 2, rect.Top);
                    }
                    else
                    {
                        rotateTransform = new RotateTransform(360 - (angle - 180), rect.Left + _itemWidth / 2, rect.Bottom);
                    }
                }
                else
                {
                    if (i > quarter && i < 3 * quarter)
                    {
                        // -180 because the rectcangles are already looking down (top on the circle)
                        rotateTransform = new RotateTransform(360 - (angle - 180), rect.Left + _itemWidth / 2, rect.Top);
                    }
                    else
                    {
                        rotateTransform = new RotateTransform(360 - angle, rect.Left + _itemWidth / 2, rect.Bottom);
                    }
                }

                context.PushTransform(rotateTransform);
                DrawShape(context, rect, i);
                context.Pop(); // remove the rotation for the next rectangle
            }

            context.Close();
            return drawingVisual;
        }

        public DrawingVisual DrawLinearMirroredRectangle()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext context = drawingVisual.RenderOpen();
            double baseY = ActualHeight / 2; // center the rectangle in the available _space

            double amp = linearMirroredSizeCoeff * ActualHeight;
            for (int i = 0; i < _dataRendered; i++)
            {
                BuffData(i);

                double height = _minHeight + bandBuffer[i] * amp;
                double PosY = baseY;

                Rect rect = new(i * (_itemWidth + _space), PosY - height / 2, _itemWidth, height);

                DrawShape(context, rect, i);
            }

            context.Close();
            return drawingVisual;
        }

        private DrawingVisual DrawLinearPoints()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext context = drawingVisual.RenderOpen();
            double baseY = _minHeight;
            if (RepresentationType == DataRepresentationTypeEnum.LinearUpwardPoints)
            {
                baseY = ActualHeight - MinHeight;
            }
            double amp = linearSizeMultiplier * ActualHeight;
            for (int i = 0; i < _dataRendered; i++)
            {
                BuffData(i);
                double height = _minHeight + bandBuffer[i] * amp;
                double PosY = baseY + height;
                if (RepresentationType == DataRepresentationTypeEnum.LinearUpwardPoints)
                {
                    PosY = baseY - height;
                }
                Rect rect = new(i * (_itemWidth + _space), PosY, _itemWidth, _itemWidth);
                DrawShape(context, rect, i, _itemWidth / 2);
            }

            context.Close();
            return drawingVisual;
        }

        private DrawingVisual DrawCircledPoints()
        {
            (double CircleCenterX, double CircleCenterY, double diameter) = GetCircleProperties();
            double radius = diameter / 2;

            double cornerRadius = _itemWidth / 2;

            // coeff to determine the width and height angle (0 = 0 and 1 = 360)
            double coeff1;

            // point coordinates
            double PointX;
            double PointY;
            double multiplier = circleSizeMultiplier * ActualHeight; // multiplie the buffed data from the fft to make it visible

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext context = drawingVisual.RenderOpen(); // allow the rendering of new objects
            double angle;
            for (int i = 0; i < _dataRendered; i++)
            {
                BuffData(i);

                coeff1 = (double)i / (double)_dataRendered;
                if (_centerFreqRange) // center at the top
                {
                    angle = ((360 * Math.PI / 180) * coeff1) + Math.PI / 2; // the angle is in radians
                }
                else // start at the top but not centered
                {
                    angle = ((360 * Math.PI / 180) * coeff1) - Math.PI / 2; // the angle is in radians
                }

                double offset = bandBuffer[i] * multiplier; // the height of the rectangle to draw

                PointY = CircleCenterY + (radius + offset) * Math.Sin(angle);
                PointX = CircleCenterX + (radius + offset) * Math.Cos(angle);
                Rect rect = new(PointX, PointY, _itemWidth, _itemWidth);

                DrawShape(context, rect, i, cornerRadius);
            }

            context.Close();
            return drawingVisual;
        }

        private DrawingVisual DrawMirroredPoint()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext context = drawingVisual.RenderOpen();

            double cornerRadius = _itemWidth / 2;
            double baseY = ActualHeight / 2;
            double amp = linearMirroredSizeCoeff * ActualHeight;

            for (int i = 0; i < _dataRendered; i++)
            {
                BuffData(i);
                double height = bandBuffer[i] * amp;
                double PosY = baseY + height;

                // draw rectangle with cornered radius instead of circle because it's more efficient
                Rect rect = new(i * (_itemWidth + _space), PosY, _itemWidth, _itemWidth);
                DrawShape(context, rect, i, cornerRadius);

                PosY = baseY - height;

                rect = new(i * (_itemWidth + _space), PosY, _itemWidth, _itemWidth);
                DrawShape(context, rect, i, cornerRadius);
            }

            context.Close();
            return drawingVisual;
        }

        private void DrawShape(DrawingContext context, Rect rect, int index)
        {
            double radius = 0;
            if (_dataRendered < 32)
            {
                radius = rect.Width / 8;
            }
            else if (_dataRendered < 512)
            {
                radius = rect.Width / 4;
            }

            if (Gradient)
            {
                if (!_gradientCalculated)
                {
                    GetGradientColor();
                }
                SolidColorBrush brush = _gradientvalues[index];

                Pen pen = new Pen(brush, StrokeThickness);
                if (Fill)
                {
                    context.DrawRoundedRectangle(brush, pen, rect, radius, radius);
                }
                else
                {
                    context.DrawRoundedRectangle(null, pen, rect, radius, radius);
                }
            }
            else
            {
                SolidColorBrush brush = (SolidColorBrush)ObjectColor.Clone();
                Pen pen = new Pen(brush, StrokeThickness);
                if (Fill)
                {
                    context.DrawRoundedRectangle(brush, pen, rect, radius, radius);
                }
                else
                {
                    context.DrawRoundedRectangle(null, pen, rect, radius, radius);
                }
            }

        }

        private void DrawShape(DrawingContext context, Rect rect, int index, double radius)
        {
            if (Gradient)
            {
                if (!_gradientCalculated)
                {
                    GetGradientColor();
                }
                SolidColorBrush brush = _gradientvalues[index];

                Pen pen = new Pen(brush, StrokeThickness);
                if (Fill)
                {
                    context.DrawRoundedRectangle(brush, pen, rect, radius, radius);
                }
                else
                {
                    context.DrawRoundedRectangle(null, pen, rect, radius, radius);
                }
            }
            else
            {
                SolidColorBrush brush = (SolidColorBrush)ObjectColor.Clone();
                Pen pen = new Pen(brush, StrokeThickness);
                if (Fill)
                {
                    context.DrawRoundedRectangle(brush, pen, rect, radius, radius);
                }
                else
                {
                    context.DrawRoundedRectangle(null, pen, rect, radius, radius);
                }
            }

        }

        /// <summary>
        /// Retrieve the data from the stream by calling <see cref="Bass.ChannelGetData"/> 
        /// and normalise it if needed with <see cref="NormaliseData"/>
        /// </summary>
        /// <returns></returns>
        private bool GetData()
        {
            int result;
            switch (DataQuantity)
            {
                case DataQuantityEnum.FFT256:
                    result = Bass.ChannelGetData(_stream, fft, -2147483648);
                    break;
                case DataQuantityEnum.FFT512:
                    result = Bass.ChannelGetData(_stream, fft, -2147483647);
                    break;
                case DataQuantityEnum.FFT1024:
                    result = Bass.ChannelGetData(_stream, fft, -2147483646);
                    break;
                case DataQuantityEnum.FFT2048:
                    result = Bass.ChannelGetData(_stream, fft, -2147483645);
                    break;
                case DataQuantityEnum.FFT4096:
                    result = Bass.ChannelGetData(_stream, fft, -2147483644);
                    break;
                case DataQuantityEnum.FFT8192:
                    result = Bass.ChannelGetData(_stream, fft, -2147483644);
                    break;
                default:
                    float[] data = new float[128];
                    result = Bass.ChannelGetData(_stream, data, -2147483648);
                    NormaliseData(data);
                    break;
            }
            if (_centerFreqRange) CenterFrequencyRange();
            return result != -1;
        }

        /// <summary>
        /// Center the low frequencies and remove the 2 lowest freq to make the visualizer look better
        /// </summary>
        private static void CenterFrequencyRange()
        {
            List<float> data = new();
            int i = 0;
            while (i < _dataRendered)
            {
                if (i % 2 == 0)
                {
                    data.Add(fft[i]);
                }
                else
                {
                    data.Insert(0, fft[i]);
                }
                i++;
            }

            for (int j = 0; j < data.Count; j++)
            {
                fft[j] = data[j];
            }
            data.Clear();
        }

        /// <summary>
        /// Normalise the data when the data quantity from <see cref="Bass.ChannelGetData"/> 
        /// is higher than the one aked. Typicaly when the data asked is < 64 (fft128).
        /// The data is divided exponentially-like the higher the index (which correpond to freq => index 0 = 20Hz ... index n = 20 000 Hz)
        /// because the higher freq have lower values compared to low freq grouping more of them allow to keep the most interesting data (low freq)
        /// closer to the real values.
        /// To keep the data's information the values grouped together are averaged.
        /// </summary>
        /// <param name="data"></param>
        private static void NormaliseData(float[] data)
        {
            int count = 0;
            for (int i = 0; i < _dataQuantity; i++)
            {
                float average = 0f;
                int sampleCount = 1;

                if (i >= 1)
                {
                    sampleCount = _dataQuantity - (_dataQuantity - count / _dataQuantity);
                }

                if (sampleCount == 0) sampleCount = 1;

                for (int j = 0; j < sampleCount; j++)
                {
                    if (count < data.Length)
                    {
                        average += data[count];
                        count++;
                    }
                }

                average /= sampleCount;
                fft[i] = average;
            }
        }

        /// <summary>
        /// decrease the values smoothly if they are lower than their precedent value with the help of a buffer
        /// </summary>
        /// <param name="index">the index of the value to buff</param>
        private static void BuffData(int index)
        {
            if (fft[index] > bandBuffer[index])
            {
                bandBuffer[index] = fft[index];
                _bufferDecrease[index] = BuffDecrease;
            }
            else if (fft[index] < bandBuffer[index])
            {
                bandBuffer[index] -= _bufferDecrease[index];
                _bufferDecrease[index] *= BuffIncrease;
            }

            if (bandBuffer[index] < 0.0005)
            {
                bandBuffer[index] = 0;
            }
        }


        private void GetGradientColor(double opacity = 1)
        {
            dispatcherTimer.Stop();

            _gradientvalues = new();
            SolidColorBrush clone = (SolidColorBrush)ObjectColor.CloneCurrentValue();

            (double rRatio, double gRatio, double bRatio) = CalculateRatio(clone.Color.R, clone.Color.G, clone.Color.B);

            // recenter the RGB values btw 20 and 200 while keeping the ratio
            byte R = FormatRGB(clone.Color.R, rRatio, 20, 200);
            byte G = FormatRGB(clone.Color.G, gRatio, 20, 200);
            byte B = FormatRGB(clone.Color.B, bRatio, 20, 200);
            double colorBrighness = (R + G + B) / 765d; // get the mean value for the "brightness" (simplified)

            int minRGBValue = (int)(colorBrighness * 60d); // the max min value is 60
            int maxRGBValue = 200 + (int)(colorBrighness * 55d); // the min max value is 200 
            // knowing that the brighness will always be btw 0.08 and 0.78
            // the min value is 5
            // and the max value is 243

            if (colorBrighness < 0.35) colorBrighness += 0.5 - colorBrighness;
            else if (colorBrighness > 0.7) colorBrighness += 0.7 - colorBrighness;

            double comparer = (double)_dataRendered / 2d; // divide data to get the nomber of color in gradient
            double modifiedComparer = (double)_dataRendered / 200d; // same as comparer but for high or low data quantity
            if (_dataRendered < 64)
            {
                /*modifiedComparer = colorBrighness * (_dataRendered * 2);*/ // for low data quantity
                modifiedComparer = (double)_dataRendered * 2d;
            }

            for (int i = 0; i < _dataRendered; i++)
            {
                double modifiedi = colorBrighness * i; // change the brightness based on i value (low to high)
                double modifier = modifiedi - comparer; // increase or decrease the color RGB values

                if (_dataRendered >= 300)
                {
                    double currentzone = modifiedi / modifiedComparer; // give back a value btw 0 and 200
                    modifier = currentzone - 100; // -100 is modifiedcomparer / 2
                }
                else if (_dataRendered < 64)
                {
                    modifier = modifiedi*2 - modifiedComparer;
                }

                int red = R + (int)modifier;
                int green = G + (int)modifier;
                int blue = B + (int)modifier;

                red = FormatRGB(red, rRatio, minRGBValue, maxRGBValue);
                green = FormatRGB(green, gRatio, minRGBValue, maxRGBValue);
                blue = FormatRGB(blue, bRatio, minRGBValue, maxRGBValue);

                SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromArgb((byte)(255 * opacity), (byte)red, (byte)green, (byte)blue));
                if (_centerFreqRange)
                {
                    if(i % 2 == 0)
                    {
                        _gradientvalues.Add(solidColorBrush);
                    }
                    else
                    {
                        _gradientvalues.Insert(0, solidColorBrush);
                    }
                }
                else
                {
                    _gradientvalues.Add(solidColorBrush);
                }
            }
            _gradientCalculated = true;
            dispatcherTimer.Start();
        }

        private static (double, double, double) CalculateRatio(double red, double green, double blue)
        {
            double rRatio = 1;
            double gRatio = 1;
            double bRatio = 1;

            if(red == green && red == blue) return (rRatio, gRatio, bRatio);

            if (red > green)
            {
                if (red > blue) // red higher
                {
                    rRatio = 1;
                    gRatio = green / red;
                    bRatio = blue / red;
                }
                else
                {
                    if (green > blue) // green higher
                    {
                        gRatio = 1;
                        rRatio = red / green;
                        bRatio = blue / green;
                    }
                    else // blue higher
                    {
                        bRatio = 1;
                        rRatio = red / blue;
                        gRatio = green / blue;
                    }
                }
            }
            else
            {
                if(green > blue) // green higher
                {
                    gRatio = 1;
                    rRatio = red / green;
                    bRatio = blue / green;
                }
                else // blue higher
                {
                    bRatio = 1;
                    rRatio = red / blue;
                    gRatio = green / blue;
                }
            }

            return (rRatio, gRatio, bRatio);
        }

        private static byte FormatRGB(byte value, double ratio, int minValue, int maxValue)
        {
            int max = (int)(ratio * maxValue);
            int min = (int)(ratio * minValue);
            byte output = value> max ? (byte)max : value;
            output = output < min ? (byte)min : output;
            return output;
        }

        private static int FormatRGB(int value, double ratio, int minValue, int maxValue)
        {
            int max = (int)(ratio * maxValue);
            int min = (int)(ratio * minValue);
            int output = value > max ? max : value;
            output = output < min ? min : output;
            return output;
        }

        /// <summary>
        /// calculate the circle's center coordinate X, Y and its diameter
        /// </summary>
        /// <returns></returns>
        private (double, double, double) GetCircleProperties()
        {
            double CircleCenterX = ActualWidth * 0.5;
            double CircleCenterY = ActualHeight * 0.5;

            double diameter = ActualHeight * diameterCoeff;

            if (CircleCenterX < CircleCenterY) // need the smallest one
            {
                diameter = ActualWidth * diameterCoeff;
            }

            return (CircleCenterX, CircleCenterY, diameter);
        }
        private double GetCircleRadius()
        {
            double diameter = ActualHeight * diameterCoeff;

            if (ActualWidth < ActualHeight) // need the smallest one
            {
                diameter = ActualWidth * diameterCoeff;
            }
            return diameter / 2;
        }

        private static double GetCircledBarWidth(double radius)
        {
            double circ = 2 * Math.PI * radius;
            return GetBarWidth(circ, out double _);
        }
        private static double GetBarWidth(double length, out double space)
        {
            double itemWidth;
            double spacePerItem = length / _dataRendered;
            if(spacePerItem <= 1) // not even enough _space to display all data
            {
                itemWidth = spacePerItem;
                space = 0;
            }
            else
            {
                itemWidth = spacePerItem * (_barThickness / 100d);
                if(itemWidth < 1)
                {
                    itemWidth = 1;
                }

                space = spacePerItem - itemWidth;
            }
            return itemWidth;
        }

        public void Dispose()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= Update;
            _visuals.Clear();
            fft = new float[1];
            bandBuffer = new float[1];
            _bufferDecrease = new float[1];
            _gradientvalues = new();
        }
    }
}
