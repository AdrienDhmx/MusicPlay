using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace HistoryChart
{
    public class HistoryChart : Canvas, IDisposable
    {
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public Brush PrimaryColor
        {
            get { return (Brush)GetValue(PrimaryColorProperty); }
            set { SetValue(PrimaryColorProperty, value); }
        }

        public Brush SecondaryColor
        {
            get { return (Brush)GetValue(SecondaryColorProperty); }
            set { SetValue(SecondaryColorProperty, value); }
        }

        public Brush MouseOverBtnColor
        {
            get { return (Brush)GetValue(MouseOverBtnColorProperty); }
            set { SetValue(MouseOverBtnColorProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBtnColorProperty =
            DependencyProperty.Register("MouseOverBtnColor", typeof(Brush), typeof(HistoryChart), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(50, 50, 50, 50))));

        public static readonly DependencyProperty SecondaryColorProperty =
            DependencyProperty.Register("SecondaryColor", typeof(Brush), typeof(HistoryChart), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public static readonly DependencyProperty PrimaryColorProperty =
            DependencyProperty.Register("PrimaryColor", typeof(Brush), typeof(HistoryChart), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(HistoryChart), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250)), OnForegroundChanged));

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StyleChanged?.Invoke();
        }

        public int NumberOfWeek { get; private set; }
        public DateTime StartDate { get; private set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public List<PlayHistory> HistoryModels { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private static event Action StyleChanged;

        private int _maxPlayCount;
        private double _xTickSpace = 0;
        private double _yTickSpace = 0;
        private int _yStep = 1;
        private double curveXCoeff = 0.2;
        private double curveYCoeff = 0.05;
        private readonly double _graphWidthMargin = 80;
        private readonly double _graphHeightMargin = 60;
        private readonly double _topGraphHeightMargin = 60;
        private double _graphWidth = 0;
        private double _graphHeight = 0;
        private readonly double _lineMinSize = 2;
        private readonly double _backgroundLineOpacity = 0.2;
        private bool _isInit = false;
        private readonly SolidColorBrush tranparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        private readonly string TodayIcon = "M12 12c-1.1 0-2-.9-2-2s.9-2 2-2 2 .9 2 2-.9 2-2 2zm6-1.8C18 6.57 15.35 4 12 4s-6 2.57-6 6.2c0 2.34 1.95 5.44 6 9.14 4.05-3.7 6-6.8 6-9.14zM12 2c4.2 0 8 3.22 8 8.2 0 3.32-2.67 7.25-8 11.8-5.33-4.55-8-8.48-8-11.8C4 5.22 7.8 2 12 2z";
        private readonly string PreviousIcon = "M7 6c.55 0 1 .45 1 1v10c0 .55-.45 1-1 1s-1-.45-1-1V7c0-.55.45-1 1-1zm3.66 6.82l5.77 4.07c.66.47 1.58-.01 1.58-.82V7.93c0-.81-.91-1.28-1.58-.82l-5.77 4.07c-.57.4-.57 1.24 0 1.64z";
        private readonly string NextIcon = "M7.58 16.89l5.77-4.07c.56-.4.56-1.24 0-1.63L7.58 7.11C6.91 6.65 6 7.12 6 7.93v8.14c0 .81.91 1.28 1.58.82zM16 7v10c0 .55.45 1 1 1s1-.45 1-1V7c0-.55-.45-1-1-1s-1 .45-1 1z"; 
        static HistoryChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HistoryChart), new FrameworkPropertyMetadata(typeof(HistoryChart)));
        }

        public override void EndInit()
        {
            base.EndInit();

            StyleChanged += Redraw;

            NumberOfWeek = 2;
            StartDate = DateTime.Today.AddDays(-NumberOfWeek * 7);
            InitGraph(6);
        }

        public void Redraw()
        {
            if(_isInit)
                InitGraph(0);
        }

        private void InitGraph(int childrenCount)
        {
            if (Children != null && Children.Count > childrenCount)
                Children.RemoveRange(childrenCount, Children.Count - childrenCount);

            HistoryModels = PlayHistory.GetHistoryBetween(StartDate, StartDate.AddDays(7 * NumberOfWeek));

            if (Children != null && Children.Count < 6) // if btn not drawn
            {
                DrawButtons();
            }

            if (HistoryModels != null && HistoryModels.Count > 0) // if data
                DrawChart();
            else
                DrawText(MusicPlay.Language.Resources.NoDataFound, 20);
            _isInit = true;

        }

        private void DrawButtons()
        {
            double y = ActualHeight - _topGraphHeightMargin + 20;
            double x = ActualWidth/2 - 120;
            DrawBtn(x, y, PreviousIcon, 0);
            x += 70;
            DrawBtn(x, y, TodayIcon, 1);
            x += 70;
            DrawBtn(x, y, NextIcon, 2);
        }

        private void DrawBtn(double x, double y, string pathData, int command)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var path = new Path
            {
                Data = converter.ConvertFrom(pathData) as Geometry
            };

            path.Fill = PrimaryColor;
            path.Width = 20;
            path.Height = 25;
            path.Stretch = Stretch.Fill;
            path.IsHitTestVisible = false;

            Rectangle rectangle = new();
            rectangle.IsHitTestVisible = true;
            rectangle.Width = 60;
            rectangle.Height = 40;
            rectangle.Fill = tranparent;
            rectangle.RadiusX = rectangle.Height / 4;
            rectangle.RadiusY = rectangle.Height / 4;

            if(command == 0)
                rectangle.MouseDown += PreviousWeek_Command;
            else if(command == 1)
                rectangle.MouseDown += GoToToday_Command;
            else if(command == 2)
                rectangle.MouseDown += NextWeek_Command;

            rectangle.MouseEnter += Btn_MouseEnter;
            rectangle.MouseLeave += Btn_MouseLeave;

            SetLeft(rectangle, x);
            SetBottom(rectangle, y);

            SetLeft(path, x + 20);
            SetBottom(path, y + 8);

            Children.Add(path);
            Children.Add(rectangle);
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            Rectangle? rectangle = sender as Rectangle;
            if(rectangle != null)
            {
                rectangle.Fill = tranparent;
            }
        }

        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle? rectangle = sender as Rectangle;
            if (rectangle != null)
            {
                rectangle.Fill = MouseOverBtnColor;
            }
        }

        private void NextWeek_Command(object sender, MouseButtonEventArgs e)
        {
            StartDate = StartDate.AddDays(7);
            InitGraph(6);
        }
        private void PreviousWeek_Command(object sender, MouseButtonEventArgs e)
        {
            StartDate = StartDate.AddDays(-7);
            InitGraph(6);
        }
        private void GoToToday_Command(object sender, MouseButtonEventArgs e)
        {
            StartDate = DateTime.Today.AddDays(-NumberOfWeek * 7);
            InitGraph(6);
        }

        private void DrawChart()
        {
            _maxPlayCount = HistoryModels.OrderByDescending(h => h.PlayCount).First().PlayCount;
            _graphHeight = ActualHeight - _graphHeightMargin - _topGraphHeightMargin;
            _graphWidth = ActualWidth - 2 * _graphWidthMargin;
            if(_graphHeight > 0 && _graphWidth > 0)
            {
                DrawAxes();
                DrawGraph();
            }
        }
        private void DrawGraph()
        {
            Path path = new Path();
            PathGeometry geometry = new();
            PathFigureCollection figureCollection = new();
            PathFigure figure = new();
            PointCollection pc = new();
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            PolyBezierSegment polyBezierSegment = new PolyBezierSegment();

            int index = 0;
            int lastDayDif = -1;
            Point graphPoint;
            foreach (PlayHistory h in HistoryModels)
            {
                if (h.PlayCount == 0)
                    continue;
                int daydif = h.Date.Subtract(StartDate).Days;
                if (lastDayDif != -1 && lastDayDif + 1 < daydif) // day(s) with no playcount
                {
                    for (int i = 1; i < daydif - lastDayDif; i++) // draw every missing day with listenTime = 0
                    {
                        graphPoint = DrawPoints(lastDayDif + i, new PlayHistory() { Id = 99999999+i });

                        if (index == 0)
                        {
                            figure.StartPoint = graphPoint;
                        }
                        pc.Add(graphPoint);
                    }
                }
                graphPoint = DrawPoints(daydif, h);
                if (index == 0)
                {
                    figure.StartPoint = graphPoint;
                }
                pc.Add(graphPoint);

                lastDayDif = daydif;
                index++;
            }
            polyBezierSegment.Points = ControlPoints(pc);
            figure.Segments.Add(polyBezierSegment);

            figureCollection.Add(figure);
            geometry.Figures = figureCollection;
            path.Data = geometry;

            path.Stroke = PrimaryColor;
            path.StrokeThickness = 4;
            //path.Fill = PrimaryColor;
            path.Opacity = 0.8;
            path.IsHitTestVisible = false;

            Children.Add(path);
        }

        private Point DrawPoints(int daydif, PlayHistory h)
        {
            double x = _graphWidthMargin + _xTickSpace * daydif;
            double y = _graphHeightMargin + _yTickSpace / _yStep * TimeSpan.FromMilliseconds(h.PlayTime).TotalMinutes;

            double graphY = ActualHeight - y;
            Point graphPoint = new Point(x, graphY);


            Ellipse ellipse = new();
            ellipse.Width = 14;
            ellipse.Height = ellipse.Width;
            ellipse.Name = "ellipse_" + h.Id;

            ellipse.Fill = PrimaryColor;
            ellipse.Stroke = tranparent;
            ellipse.StrokeThickness = 2;
            ellipse.ToolTip = h.PlayCount.ToString() + " - " + TimeSpan.FromMilliseconds(h.PlayTime).ToShortString();

            SetLeft(ellipse, x - ellipse.Width / 2);
            SetBottom(ellipse, y - ellipse.Width / 2);

            Children.Add(ellipse);

            return graphPoint;
        }

        private void DrawAxes()
        {
            DrawTimeYAxes();
            DrawXAxes();
        }

        private void DrawTimeYAxes()
        {
            int totalms = HistoryModels.Max(h => h.PlayTime);
            double totalMin = TimeSpan.FromMilliseconds(totalms).TotalMinutes;
            int numberOfTicks = (int)totalMin;
            _yStep = 1;
            if(totalMin >= 20)
            {
                _yStep = (int)(totalMin / 10);
                numberOfTicks /= _yStep;
            }

            numberOfTicks++; // so that the max value is not at the very top of the chart and has one tick above it
            _yTickSpace = (_graphHeight - 10) / numberOfTicks; // -10 to make the ticks lines cross even at the extremes of the graph (top and right)

            DrawLine(_graphWidthMargin, _graphHeightMargin, _lineMinSize, _graphHeight); // draw line y axes
            DrawText(MusicPlay.Language.Resources.Time, 16, _graphWidthMargin + 5, _graphHeightMargin + _graphHeight + 5, 0); // draw y axes legend

            double leftTick = _graphWidthMargin - 4;

            for (int i = 0; i <= numberOfTicks; i++)
            {
                double y = _graphHeightMargin + _yTickSpace * i;

                string text = TimeSpan.FromMinutes(i * _yStep).ToShortString();
                text = RemoveSec(text);

                DrawLine(leftTick, y, _graphWidth, _lineMinSize , _backgroundLineOpacity); // draw tick
                DrawText(text, 12, (leftTick - 10) - text.Length * 6, y - 5); // draw tick legend (time value)
            }
        }

        private static string RemoveSec(string time)
        {
            string output = "";
            for (int i = 0; i < time.Length - 3; i++)
            {
                output += time[i];
            }
            return output;
        }

        private void DrawPlayCountYAxes()
        {
            int numberOfTicks = _maxPlayCount;
            _yStep = 1;
            if(numberOfTicks >= 20)
            {
                _yStep = numberOfTicks /10;
                numberOfTicks /= _yStep;
            }
            numberOfTicks ++; // so that the max value is not at the very top of the chart and has one tick above it
            _yTickSpace = (_graphHeight - 10) / numberOfTicks; // -10 to make the ticks lines cross even at the extremeties (top and right)

            DrawLine(_graphWidthMargin, _graphHeightMargin, _lineMinSize, _graphHeight); // draw line y axes
            DrawText(MusicPlay.Language.Resources.PlayCount, 16, _graphWidthMargin + 5, _graphHeightMargin + _graphHeight + 5, 0); // draw y axes legend

            double leftTick = _graphWidthMargin - 4;
            for (int i = 0; i <= numberOfTicks; i++)
            {
                double y = _graphHeightMargin + _yTickSpace * i;

                double amp = 0;
                if (i != 0 && i % 4 == 0) amp += 2;

                DrawLine(leftTick, y, _graphWidth, _lineMinSize + amp, _backgroundLineOpacity); // draw tick
                DrawText((i * _yStep).ToString(), 12+(int)amp*2, leftTick - 25 - amp, y - 5 -amp); // draw tick legend (playcount value)
            }
        }
        private void DrawXAxes()
        {
            double amp;
            double rotation;
            double rotationWidthChange;
            _xTickSpace = (_graphWidth - 10) / ((NumberOfWeek * 7));

            DrawLine(_graphWidthMargin, _graphHeightMargin, _graphWidth, _lineMinSize); // draw line x axes
            DrawText(MusicPlay.Language.Resources.Date, 16, _graphWidthMargin + _graphWidth + 5, _graphHeightMargin + 5, 0); // draw x axes legend

            double LowY = _graphHeightMargin - 4;
            for (int i = 0; i <= NumberOfWeek * 7; i++)
            {
                amp = 0;
                rotation = 0d;
                rotationWidthChange = 0d;

                if (i != 0 && i % 7 == 0) amp = 2;

                double x = _graphWidthMargin + _xTickSpace * i;

                DateTime date = StartDate.AddDays(i);

                string legende = date.ToString("MM-dd");
                if(date.Date == DateTime.Today)
                {
                    legende = MusicPlay.Language.Resources.Today;
                }
                else if(date.Date == DateTime.Today.AddDays(1))
                {
                    legende = MusicPlay.Language.Resources.Tomorow;
                }
                else if (date.Date == DateTime.Today.AddDays(-1))
                {
                    legende = MusicPlay.Language.Resources.Yesterday;
                }

                if (_xTickSpace <= (legende.Length + amp * 2) * 5)
                {
                    rotation = 30d;
                    rotationWidthChange = legende.Length - amp * 2 > 8 ? legende.Length + rotation/10: 0;
                }

                DrawLine(x, _graphHeightMargin - 4, _lineMinSize + amp, _graphHeight, _backgroundLineOpacity); // draw tick
                DrawText(legende, 12 + (int)amp*2, x - legende.Length * 3 + rotationWidthChange, LowY - 25 - legende.Length * 0.8 * rotation/10, rotation); // draw tick legend (playcount value)
            }

        }

        private void DrawText(string text, int fontSize)
        {
            TextBlock textBlock = new();
            textBlock.Text = text;
            textBlock.Foreground = Foreground;
            textBlock.FontSize = fontSize;
           
            SetLeft(textBlock, ActualWidth/2 - text.Length *8);
            SetBottom(textBlock, ActualHeight / 2);

            Children.Add(textBlock);
        }
        private void DrawText(string text, int fontSize, double x, double y, double rotation = 0, string name = "")
        {
            TextBlock textBlock = new();
            textBlock.Text = text;
            textBlock.Foreground = Foreground;
            textBlock.FontSize = fontSize;

            if (!string.IsNullOrWhiteSpace(name))
            {
                textBlock.Name = name;
            }

            if(rotation != 0)
                textBlock.LayoutTransform = new RotateTransform(rotation, x, y);

            SetLeft(textBlock, x);

            SetBottom(textBlock, y);

            Children.Add(textBlock);
        }
        private void DrawLine(double X1, double Y1, double width, double height, double opacity = 1)
        {
            double smallSide = width >= height ? height : width;

            Rectangle rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.RadiusX = 1;
            rectangle.RadiusY = 1;

            rectangle.Stroke = Foreground;
            rectangle.StrokeThickness = 1;
            rectangle.Fill = Foreground;
            rectangle.Opacity = opacity;

            rectangle.IsHitTestVisible = false;

            SetLeft(rectangle, X1 - smallSide/2);
            SetBottom(rectangle, Y1 - smallSide/2);

            Children.Add(rectangle);
        }

        private PointCollection ControlPoints(PointCollection points)
        {
            if (points.Count == 0)
                return points;

            PointCollection pc = new();
            for(int i = 0; i < points.Count; i++)
            {
                if (i == 0 || i == points.Count - 1)
                {
                    pc.Add(points[i]);
                }
                else
                {
                    double dx = points[i - 1].X - points[i + 1].X;
                    double dy = points[i - 1].Y - points[i + 1].Y;

                    double x2 = points[i].X + dx * curveXCoeff;
                    double y2 = points[i].Y + dy * curveYCoeff;
                    pc.Add(new Point(x2, y2));

                    pc.Add(points[i]);

                    double x1 = points[i].X - dx * curveXCoeff;
                    double y1 = points[i].Y -dy * curveYCoeff/2;
                    pc.Add(new Point(x1, y1));
                }
            }
            pc.Add(points.Last());
            return pc;
        }

        public void Dispose()
        {
            Children.Clear();
            StyleChanged -= Redraw;
        }
    }
}
