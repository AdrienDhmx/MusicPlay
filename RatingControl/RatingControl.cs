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

namespace RatingControl
{
    public class RatingControl : Canvas
    {
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Brush MouseOverColor
        {
            get { return (Brush)GetValue(MouseOverColorProperty); }
            set { SetValue(MouseOverColorProperty, value); }
        }

        public Geometry Shape
        {
            get { return (Geometry)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        public double RatingValue
        {
            get { return (double)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(double), typeof(RatingControl), new PropertyMetadata(0d, OnRatingChanged));


        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(Geometry), typeof(RatingControl), new PropertyMetadata(new PathGeometry()));


        public static readonly DependencyProperty MouseOverColorProperty =
            DependencyProperty.Register("MouseOverColor", typeof(Brush), typeof(RatingControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250)), OnMouseOverColorChanged));


        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(RatingControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250)), OnFillChanged));


        private static void OnMouseOverColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is Brush color)
            {
                _mouseOver = (SolidColorBrush)color;
                // redraw
                OnColorChanged();
            }
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null && e.NewValue is Brush color)
            {
                _fill = (SolidColorBrush)color;
                // redraw
                OnColorChanged();
            }
        }

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(double.TryParse(e.NewValue.ToString(), out double newRating))
            {
                Rating = newRating;
            }
        }

        private static double _rating;
        private static double Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnRatingChanged();
            }
        }

        private static Action ColorChanged;
        private static void OnColorChanged()
        {
            ColorChanged?.Invoke();
        }

        private static Action ratingChanged;
        private static void OnRatingChanged()
        {
            ratingChanged?.Invoke();
        }

        private static readonly int maxValue = 5;
        private static double shapeSize = 10;
        private static double space = 2;
        private static SolidColorBrush _fill { get; set; } = new();
        private static SolidColorBrush _mouseOver { get; set; } = new();
        private static readonly SolidColorBrush tranparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        static RatingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl), new FrameworkPropertyMetadata(typeof(RatingControl)));
        }

        public override void EndInit()
        {
            base.EndInit();

            Background = tranparent;
            MouseLeave += Shape_MouseLeave;

            Width = 120;
            Height = 35;

            shapeSize = Width / maxValue * 0.9;
            space = Width / maxValue * 0.1;

            ratingChanged += UpdateRating;
            ColorChanged += Redraw;
            _fill = (SolidColorBrush)Fill;
            _mouseOver = (SolidColorBrush)MouseOverColor;
            Rating = RatingValue;

            Draw();
        }

        private void Redraw()
        {
            Draw();
        }

        private void UpdateRating()
        {
            if (Children == null || Children.Count < 5)
            {
                Draw();
            }
            else
            {
                for (int i = 1; i <= maxValue; i++)
                {
                    if(i -1< Children.Count)
                    {
                        Path? c = Children[i - 1] as Path;
                        if(c != null)
                        {
                            if (i <= RatingValue)
                                c.Fill = _fill;
                            else
                                c.Fill = tranparent;
                        }
                    }
                }
            }
        }

        private void Draw()
        {
            if(Children != null && Children.Count > 0)
            {
                Children.Clear();
            }

            for (int i = 0; i < maxValue; i++)
            {
                double x = (shapeSize + space) * i;
                double y = (Height - shapeSize) / 2;

                if(i+1 <= Rating)
                {
                    DrawShape(i + 1, x, y, _fill);
                }
                else
                {
                    DrawShape(i + 1, x, y, tranparent);
                }
            }
        }

        private void DrawShape(int index, double x, double y, SolidColorBrush color)
        {
            var path = new Path
            {
                Data = Shape
            };

            path.Fill = color;
            path.Stroke = Fill;
            path.StrokeThickness = 1;

            path.Width = shapeSize;
            path.Height = shapeSize;
            path.Stretch = Stretch.Fill;
            path.IsHitTestVisible = true;
            path.Name = "shape_" + index.ToString();

            path.MouseEnter += Shape_MouseEnter;
            path.MouseLeave += Shape_MouseLeave;

            path.MouseDown += Shape_MouseDown;

            SetLeft(path, x);
            SetBottom(path, y);
            Children.Add(path);
        }


        private void Shape_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Path? shape = sender as Path;
            if(shape != null && int.TryParse(shape.Name.Replace("shape_", string.Empty), out int rating))
            {
                RatingValue = rating;
            }
        }

        private void Shape_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsMouseOver)
            {
                Path? shape = sender as Path;
                if (shape != null && int.TryParse(shape.Name.Replace("shape_", string.Empty), out int rating))
                {
                    for (int i = 1; i <= maxValue; i++)
                    {
                        if (i - 1 < Children.Count)
                        {
                            Path? c = Children[i - 1] as Path;
                            if (c != null)
                            {
                                if (i <= RatingValue)
                                {
                                    c.Fill = _fill;
                                }
                                else if (i < rating) // don't include this shape
                                    c.Fill = _mouseOver;
                                else
                                    c.Fill = tranparent;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i <= maxValue; i++)
                {
                    if (i - 1 < Children.Count)
                    {
                        Path? c = Children[i - 1] as Path;
                        if (c != null)
                        {
                            if (i <= RatingValue)
                            {
                                c.Fill = _fill;
                            }
                            else
                                c.Fill = tranparent;
                        }
                    }
                }
            }
        }

        private void Shape_MouseEnter(object sender, MouseEventArgs e)
        {
            Path? shape = sender as Path;
            if (shape != null && int.TryParse(shape.Name.Replace("shape_", string.Empty), out int rating))
            {
                for (int i = 1; i <= maxValue; i++)
                {
                    if(i -1 < Children.Count)
                    {
                        Path? c = Children[i - 1] as Path;
                        if(c != null)
                        {
                            if(i <= RatingValue)
                            {
                                c.Fill = _fill;
                            }
                            else if (i <= rating)
                                c.Fill = _mouseOver;
                            else
                                c.Fill = tranparent;
                        }
                    }
                }
            }
        }
    }
}
