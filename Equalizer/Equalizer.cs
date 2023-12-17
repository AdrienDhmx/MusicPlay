using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioHandler;
using AudioHandler.Models;
using Equalizer.Models;

namespace Equalizer
{
    public class Equalizer : Canvas, IDisposable
    {
        public EQManager EQManager
        {
            get { return (EQManager)GetValue(EQManagerProperty); }
            set { SetValue(EQManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EQManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EQManagerProperty =
            DependencyProperty.Register("EQManager", typeof(EQManager), typeof(Equalizer), new PropertyMetadata(null, OnEQManagerChange));


        public UIEQBandModel SelectedEQBand
        {
            get { return (UIEQBandModel)GetValue(SelectedEQBandProperty); }
            set { SetValue(SelectedEQBandProperty, value); }
        }

        public static readonly DependencyProperty SelectedEQBandProperty =
            DependencyProperty.Register("SelectedEQBand", typeof(UIEQBandModel), typeof(Equalizer), new PropertyMetadata(null));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Equalizer), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250 ,250))));

        public Brush GraphColor
        {
            get { return (Brush)GetValue(GraphColorProperty); }
            set { SetValue(GraphColorProperty, value); }
        }

        public static readonly DependencyProperty GraphColorProperty =
            DependencyProperty.Register("GraphColor", typeof(Brush), typeof(Equalizer), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250))));

        public Brush PreviewGraphColor
        {
            get { return (Brush)GetValue(PreviewGraphColorProperty); }
            set { SetValue(PreviewGraphColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewGraphColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewGraphColorProperty =
            DependencyProperty.Register("PreviewGraphColor", typeof(Brush), typeof(Equalizer), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250)), OnForegroundChanged));



        private static event Action PresetChanged;
        private static EQPresetModel preset = new();
        private static List<EQEffectModel> effects => preset.Effects;

        private const int fontSize = 16;
        private const double PointDiameter = 16;
        private const double BandGraphOpacity = 0.6;
        private const double CumulatedGraphOpacity = 1;
        private const double PreviewGraphOpacity = 0.3;

        private const int pointToPlot = 300;

        private const double _graphLeftMargin = 40;
        private const double _graphRightMargin = 30;
        private const double _graphTopMargin = 50;
        private const double _graphBottomMargin = 25;

        private const double _graphHorizontalMargin = _graphLeftMargin + _graphRightMargin;
        private const double _graphVerticalMargin = _graphTopMargin + _graphBottomMargin;
        private const double _yAxesWidth = 40;
        private const double _xAxesHeight = 25;

        private const int YTickQty = 24;
        private const int XTickQty = 11;
        private const int YStartValue = -12;
        private const double MinBandwidth = 0.2;
        private const double MaxBandwidth = 2.2;

        private const string YLegend = "db";
        private readonly string[] XLegend = {"", "32", "64", "125", "250", "500", "1k", "2k", "4k", "8k", "16k", ""};
        private const int XLegendTopMargin = 20;
        private const int YLegendRightMargin = 10;

        private const decimal _maxExponentN = 14.96578428466M; // 2^14.287 = 20000
        private const decimal _minExponentN = 4; // 2^4 = 16

        private double GraphXStart => _graphLeftMargin + _yAxesWidth;
        private double GraphYStart => _graphBottomMargin + _xAxesHeight;
        private double GraphXEnd => GraphXStart + GraphWidth;
        private double GraphYEnd => GraphYStart + GraphHeight;
        private double GraphWidth => (int)Width - _graphHorizontalMargin - _yAxesWidth;
        private double GraphHeight => Height - _graphVerticalMargin - _xAxesHeight;
        private double GraphMiddle => GraphHeight / 2;

        private double YTickStep => GraphHeight / YTickQty;
        private double XTickStep => GraphWidth / XTickQty;

        private const string CumulatedGraphUpdatePreview = "CumulatedUpdatePreview";
        private const string BandUpdatePreview = "BandUpdatePreview";

        private bool RightMouseDown = false;
        private static int MovingEffectIndex = -1;
        private static Ellipse MovingPoint;
        private static Point MovingPointPos;
        private TranslateTransform? originTT;
        private Point clickPosition;
        static Equalizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Equalizer), new FrameworkPropertyMetadata(typeof(Equalizer)));
        }

        protected override void OnStyleChanged(Style oldStyle, Style newStyle)
        {
            base.OnStyleChanged(oldStyle, newStyle);
            Draw();
        }

        public override void EndInit()
        {
            base.EndInit();

            Background = DrawHelper.Tranparent;

            PresetChanged += Draw;
        }

        private static void OnEQManagerChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EQManager eQManager = (EQManager)e.NewValue;
            if(eQManager != null)
            {
                EQManager oldEQManager = (EQManager)e.OldValue;
                if(oldEQManager != null)
                    oldEQManager.PresetChanged -= EQManager_PresetChanged;

                preset = eQManager.Preset;
                eQManager.PresetChanged += EQManager_PresetChanged;
            } 
            PresetChanged?.Invoke();
        }

        private static void EQManager_PresetChanged()
        {
            PresetChanged?.Invoke(); // draw
        }

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PresetChanged?.Invoke(); // draw
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if(EQManager != null)
            {
                Draw();
            }
        }

        private void OnLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.Source is Ellipse ellipse && int.TryParse(ellipse.Name.AsSpan(1), out int effectIndex))
            {
                ellipse.ReleaseMouseCapture();

                if (Math.Abs(MovingPointPos.Y - clickPosition.Y) > PointDiameter / 4 || Math.Abs(MovingPointPos.X - clickPosition.X) > PointDiameter / 4)
                {
                    // we don't want to modify the preset object directly
                    EQEffectModel effect = (EQEffectModel)effects[effectIndex].Clone();

                    effect.Gain = (Height - MovingPointPos.Y - GraphYStart) / YTickStep + YStartValue; // db can be negative (30 total => -15db ... +15db)
                    effect.CenterFrequency = GraphCoorToHz(ToGraphCoor(MovingPointPos).X);

                    EQManager.UpdatePreset(effect);
                }

                MovingEffectIndex = -1;
                Draw();
            }   
        }

        private void OnLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            RightMouseDown = false;
            if (e.Source is Ellipse ellipse && int.TryParse(ellipse.Name.AsSpan(1), out int effectIndex))
            {
                ellipse.CaptureMouse();

                MovingEffectIndex = effectIndex;
                MovingPoint = ellipse;
                originTT = ellipse.RenderTransform as TranslateTransform ?? new TranslateTransform();
                clickPosition = e.GetPosition(this);
                MovingPointPos = clickPosition;

                SelectedEQBand = effects[effectIndex].ToUIEQBand((SolidColorBrush)GraphColor);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if(MovingEffectIndex != -1)
            {
                MovingPointPos = e.GetPosition(this);
                // change bandwidth
                if (RightMouseDown)
                {
                    double xChange = Math.Abs(MovingPointPos.X - clickPosition.X);
                    double newBandwidth = Math.Clamp(xChange / 100, MinBandwidth, MaxBandwidth);

                    // update the graph
                    EQEffectModel effect = (EQEffectModel)effects[MovingEffectIndex].Clone();
                    effect.BandWidth = newBandwidth;
                    SelectedEQBand.BandWidth = newBandwidth;
                    effects[MovingEffectIndex] = effect;
                }
                else // change center freq and gain
                {
                    var transform = MovingPoint.RenderTransform as TranslateTransform ?? new TranslateTransform();

                    double topSpace = clickPosition.Y - _graphTopMargin;
                    double bottomSpace = GraphHeight - topSpace;
                    double leftSpace = clickPosition.X - GraphXStart;
                    double RightSpace = GraphWidth - leftSpace;
                    double transformYValue = originTT!.Y + (MovingPointPos.Y - clickPosition.Y);
                    double transformXValue = originTT!.X + (MovingPointPos.X - clickPosition.X);

                    // limit the Graphic point position within the graph space
                    transform.Y = Math.Clamp(transformYValue, -topSpace, bottomSpace);
                    transform.X = Math.Clamp(transformXValue, -leftSpace, RightSpace);

                    // limit the saved point position used to calculate the gain within the graph space
                    MovingPointPos.Y = Math.Clamp(MovingPointPos.Y, _graphTopMargin, Height - GraphYStart);
                    MovingPointPos.X = Math.Clamp(MovingPointPos.X, GraphXStart, GraphXEnd);

                    MovingPoint.RenderTransform = new TranslateTransform(transform.X, transform.Y);

                    // update the graph
                    EQEffectModel effect = (EQEffectModel)effects[MovingEffectIndex].Clone();
                    effect.Gain = (Height - MovingPointPos.Y - GraphYStart) / YTickStep + YStartValue; // db can be negative
                    effect.CenterFrequency = GraphCoorToHz(ToGraphCoor(MovingPointPos).X);
                    SelectedEQBand.Gain = effect.Gain;
                    SelectedEQBand.CenterFrequency = effect.CenterFrequency;

                    effects[MovingEffectIndex] = effect;
                }
                DrawPreset(true);
            }
        }

        private void OnRightMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Ellipse ellipse && int.TryParse(ellipse.Name.AsSpan(1), out int effectIndex))
            {
                ellipse.ReleaseMouseCapture();

                double xChange = Math.Abs(MovingPointPos.X - clickPosition.X);

                if (xChange > PointDiameter / 4)
                {
                    double newBandwidth = Math.Clamp(xChange / 100, MinBandwidth, MaxBandwidth);

                    // update the graph
                    EQEffectModel effect = (EQEffectModel)effects[MovingEffectIndex].Clone();
                    effect.BandWidth = newBandwidth;
                    effects[MovingEffectIndex] = effect;

                    EQManager.UpdatePreset(effect);
                }
                MovingEffectIndex = -1;
                Draw();
            }
            RightMouseDown = false;
        }

        private void OnRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Ellipse ellipse && int.TryParse(ellipse.Name.AsSpan(1), out int effectIndex))
            {
                ellipse.CaptureMouse();

                RightMouseDown = true;

                MovingEffectIndex = effectIndex;
                MovingPoint = ellipse;
                originTT = ellipse.RenderTransform as TranslateTransform ?? new TranslateTransform();
                clickPosition = e.GetPosition(this);
                MovingPointPos = clickPosition;

                SelectedEQBand = effects[effectIndex].ToUIEQBand((SolidColorBrush)GraphColor);
            }
        }

        private void Draw()
        {
            if (EQManager == null)
            {
                Dispose();
                return;
            } 
            else
            {
                preset = EQManager.Preset;
            }

            if (Children != null && Children?.Count > 0)
            {
                foreach (var child in Children)
                {
                    if(child is Ellipse ellipse)
                    {
                        ellipse.MouseLeftButtonDown -= OnLeftMouseDown;
                        ellipse.MouseLeftButtonUp -= OnLeftMouseUp;
                        ellipse.MouseRightButtonDown -= OnRightMouseDown;
                        ellipse.MouseRightButtonUp -= OnRightMouseUp;
                    }
                }

                Children.Clear(); // remove all existing children
            }

            DrawAxes();
            DrawPreset();
        }

        private void DrawAxes()
        {
            DrawYAxes();
            DrawXAxes();
        }

        private void DrawYAxes()
        {
            // draw the axe
            this.DrawRectangle(GraphXStart, GraphYStart, DrawHelper.LineMinThickness, GraphHeight, Foreground, 0.8);

            // draw the legend
            for (int i = 0; i <= YTickQty; i+=3)
            {
                string legend = (YStartValue + i).ToString() + YLegend; 

                double yPos = GraphYStart + i * YTickStep;
                double xPos = Width - GraphXStart + YLegendRightMargin; // from the right to align the text to the right

                if (i != YTickQty)
                {
                    this.DrawRectangle(GraphXStart, yPos, GraphWidth, DrawHelper.LineMinThickness, Foreground, 0.2);
                }
                // center vertically the text
                this.DrawText(DrawHelper.CreateTextBlock(legend, fontSize, Foreground), new(xPos, yPos - fontSize / 2), alignLeft: false, center: false);
            }
        }

        private void DrawXAxes()
        {
            // draw the axe
            this.DrawRectangle(GraphXStart, GraphYStart, GraphWidth, DrawHelper.LineMinThickness, Foreground, 1);

            // draw the legend
            for (int i = 0; i <= XTickQty; i++)
            {
                decimal distance = i * (decimal)XTickStep;
                double xPos = GraphXStart + (double)distance;
                double yPos = GraphYStart - XLegendTopMargin;

                string legend = XLegend[i];

                if(i != XTickQty || true)
                {
                    this.DrawRectangle(xPos, GraphYStart, DrawHelper.LineMinThickness, GraphHeight, Foreground, 0.2);
                }
                this.DrawText(DrawHelper.CreateTextBlock(legend, fontSize, Foreground), new(xPos, yPos));
            }
        }

        private void DrawPreset(bool preview = false)
        {
            List<EQEffectModel> _effects = new List<EQEffectModel>();
            if(preview)
            {
                _effects = effects;

                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i] is Shape shape && (shape.Name == CumulatedGraphUpdatePreview || shape.Name == BandUpdatePreview))
                    {
                        Children.RemoveAt(i);
                    }
                }
            }
            else
            {
                _effects = EQManager.Preset.Effects;
            }

            if (_effects.Count == 0) return;

            Point CenterPoint; // the point for the centerFrequency
            List<Point> Freqpoints = new List<Point>(); // points to draw and are draggable

            List<List<Point>> AllBandsPlotedPoints = new List<List<Point>>();

            int pointIndex = 1;
            foreach (EQEffectModel effect in _effects)
            {
                double yCenter = GainToGraphCoor(effect.Gain);
                double xCenter = HzToGraphCoor(effect.CenterFrequency);
                CenterPoint = new Point(xCenter, yCenter);

                Freqpoints.Add(CenterPoint);
                pointIndex++;

                List<Point> bandPoints = PlotBandCurve(effect);
                AllBandsPlotedPoints.Add(bandPoints);
                if (preview)
                {
                    if(effect.Band == MovingEffectIndex) // only draw preview for the band currently being modified
                        DrawBezierCurve(bandPoints, ColorHelper.AdjustHue((SolidColorBrush)PreviewGraphColor, (effect.Band + 1) * 25), PreviewGraphOpacity, 2.5, BandUpdatePreview);
                } 
                else
                {
                    if(effect.Band == SelectedEQBand?.Band)
                    {
                        DrawBezierCurve(bandPoints, ColorHelper.AdjustHue((SolidColorBrush)GraphColor, (effect.Band + 1) * 25), CumulatedGraphOpacity, 3.5);
                    }
                    else
                    {
                        DrawBezierCurve(bandPoints, ColorHelper.AdjustHue((SolidColorBrush)GraphColor, (effect.Band + 1) * 25), BandGraphOpacity, 2.5);
                    }
                }
            }

            DrawCumulatedBands(AllBandsPlotedPoints, preview ? PreviewGraphColor : GraphColor, preview);

            if (!preview)
            {
                for(int i = 0; i < Freqpoints.Count; i++)
                {
                    this.DrawPoint(ToCanvasCoor(Freqpoints[i]), PointDiameter, ColorHelper.AdjustHue((SolidColorBrush)GraphColor, (effects[i].Band + 1) * 25), 
                        onLeftMouseDown: OnLeftMouseDown, onLeftMouseUp: OnLeftMouseUp, 
                        onRightMouseDown: OnRightMouseDown, onRightMouseUp: OnRightMouseUp, 
                        name: $"p{effects[i].Band}");
                }
            }
        }

        private void DrawBezierCurve(List<Point> points, Brush color, double opacity, double thickness, string name = "")
        {
            Path path = new Path();
            PathGeometry geometry = new();
            PathFigureCollection figureCollection = new();
            PathFigure figure = new();
            PolyBezierSegment polyBezierSegment = new PolyBezierSegment();
            PointCollection pc = new();

            foreach (Point point in points)
            {
               pc.Add(ToCanvasCoor(new(point.X, point.Y)));
            }

            polyBezierSegment.Points = pc;
            figure.StartPoint = polyBezierSegment.Points[0];
            figure.Segments.Add(polyBezierSegment);

            figureCollection.Add(figure);
            geometry.Figures = figureCollection;
            path.Data = geometry;

            path.Stroke = color;
            path.StrokeThickness = thickness;
            path.Opacity = opacity;
            path.IsHitTestVisible = false;

            if (!string.IsNullOrEmpty(name))
            {
                path.Name = name;
            }

            Children.Add(path);
        }

        private Point ToCanvasCoor(Point point)
        {
            return new(GraphXStart + point.X, GraphYStart + point.Y);
        }

        private Point ToGraphCoor(Point point)
        {
            return new(point.X - GraphXStart, point.Y - GraphYStart);
        }

        private decimal MapRange(decimal value, decimal fromMin, decimal fromMax, decimal toMin, decimal toMax)
        {
            decimal fromRange = fromMax - fromMin;
            decimal toRange = toMax - toMin;
            return (value - fromMin) * toRange / fromRange + toMin;
        }

        private double GraphCoorToHz(double x)
        {
            decimal n = MapRange((decimal)x, 0, (decimal)GraphWidth, _minExponentN, _maxExponentN);
            return Math.Pow(2, (double)n);
        }

        private double HzToGraphCoor(double hz)
        {
            double n = Math.Log2(hz);
            return (double)MapRange((decimal)n, _minExponentN, _maxExponentN, 0, (decimal)GraphWidth);
        }

        private double GainToGraphCoor(double gain)
        {
            return (gain - YStartValue) * YTickStep;
        }

        /// <summary>
        /// Calculate the gain of a band at a specific frequency
        /// </summary>
        /// <param name="A">The gain at the center frequency of the band</param>
        /// <param name="f">The frequency at which the gain needs to be calculated</param>
        /// <param name="fc">The center frequency of the band</param>
        /// <param name="Q">The Q factor of the band</param>
        /// <returns></returns>
        private double CalculateGain(double A, double f, double fc, double Q)
        {
            return -A / (1 + Math.Pow((f / fc) - (fc / f), 2) * Math.Pow(Q, 2));
        }

        private void DrawCumulatedBands(List<List<Point>> points, Brush color, bool preview)
        {
            List<Point> cumulatedPoints = points[0];

            for(int i = 1; i < points.Count; i++)
            {
                for(int y = 0; y < points[i].Count; y++)
                {
                    Point currentPoint;
                    currentPoint = cumulatedPoints[y];
                    currentPoint.Y += (points[i][y].Y - GraphMiddle);
                    cumulatedPoints[y] = currentPoint;
                }
            }

            DrawBezierCurve(cumulatedPoints, color, preview ? PreviewGraphOpacity : CumulatedGraphOpacity, 5, preview ? CumulatedGraphUpdatePreview : "");
        }

        private List<Point> PlotBandCurve(EQEffectModel eqBand)
        {
            double spaceBetweenPoints = GraphWidth / pointToPlot;
            bool centerFreqAdded = false;
            List<Point> points = new();

            for (int i = 0; i < pointToPlot; i++)
            {
                double hz = GraphCoorToHz(i * spaceBetweenPoints);

                if(!centerFreqAdded && hz > eqBand.CenterFrequency)
                {
                    // add the exact point of the center freq to make sure the graph pass through it
                    points.Add(CalculateBandCoor(eqBand, eqBand.CenterFrequency));
                    centerFreqAdded = true;
                }

                Point point = CalculateBandCoor(eqBand, hz);
                points.Add(point);
            }


            return points;
        }

        /// <summary>
        /// Calculate the coordinates in the graph of a band at the specified frequency
        /// </summary>
        /// <param name="eqBand">The band to calculate the coordinates from, used to get the y</param>
        /// <param name="f">The frequency at wich to calculate the coordinates, used to get the x</param>
        /// <returns></returns>
        private Point CalculateBandCoor(EQEffectModel eqBand, double f)
        {
            double gain = CalculateGain(eqBand.Gain, f, eqBand.CenterFrequency, eqBand.Q);
            double x = HzToGraphCoor(f);
            double y = GainToGraphCoor(gain);
            return new(x, y);
        }

        public void Dispose()
        {
            PresetChanged -= Draw;
            if (Children != null && Children?.Count > 0)
            {
                foreach (var child in Children)
                {
                    if (child is Ellipse ellipse)
                    {
                        ellipse.MouseLeftButtonDown -= OnLeftMouseDown;
                        ellipse.MouseLeftButtonUp -= OnLeftMouseUp;
                    }
                }

                Children.Clear(); // remove all existing children
            }
        }
    }
}
