using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomCardControl
{
    public class CustomCardControl : ButtonBase
    {
        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(CustomCardControl), new PropertyMetadata(new PathGeometry()));

        public Brush FillColor
        {
            get { return (Brush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register("FillColor", typeof(Brush), typeof(CustomCardControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 250, 250, 250))));

        public Brush StrokeColor
        {
            get { return (Brush)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register("StrokeColor", typeof(Brush), typeof(CustomCardControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 250, 250, 250))));




        public double StrokeWidth
        {
            get { return (double)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeWidthProperty =
            DependencyProperty.Register("StrokeWidth", typeof(double), typeof(CustomCardControl), new PropertyMetadata(1d));



        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(CustomCardControl), new PropertyMetadata(20d));

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(CustomCardControl), new PropertyMetadata(20d));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CustomCardControl), new PropertyMetadata(new CornerRadius(0)));


        public Stretch IconStretch
        {
            get { return (Stretch)GetValue(IconStretchProperty); }
            set { SetValue(IconStretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconStretchProperty =
            DependencyProperty.Register("IconStretch", typeof(Stretch), typeof(CustomCardControl), new PropertyMetadata(Stretch.None));



        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(CustomCardControl), new PropertyMetadata(new Thickness(4)));



        public string CardHeader
        {
            get { return (string)GetValue(CardHeaderProperty); }
            set { SetValue(CardHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardHeaderProperty =
            DependencyProperty.Register("CardHeader", typeof(string), typeof(CustomCardControl), new PropertyMetadata(""));

        public string CardDescription
        {
            get { return (string)GetValue(CardDescriptionProperty); }
            set { SetValue(CardDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardDescriptionProperty =
            DependencyProperty.Register("CardDescription", typeof(string), typeof(CustomCardControl), new PropertyMetadata(""));

        public Brush CardDescriptionForeground
        {
            get { return (Brush)GetValue(CardDescriptionForegroundProperty); }
            set { SetValue(CardDescriptionForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardDescriptionForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardDescriptionForegroundProperty =
            DependencyProperty.Register("CardDescriptionForeground", typeof(Brush), typeof(CustomCardControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));


        public double CardDescriptionFontSize
        {
            get { return (double)GetValue(CardDescriptionFontSizeProperty); }
            set { SetValue(CardDescriptionFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardDescriptionFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardDescriptionFontSizeProperty =
            DependencyProperty.Register("CardDescriptionFontSize", typeof(double), typeof(CustomCardControl), new PropertyMetadata(12d));



        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(CustomCardControl), new PropertyMetadata(TextAlignment.Left));


        public Visibility CardDescriptionVisibility
        {
            get { return (Visibility)GetValue(CardDescriptionVisibilityProperty); }
            set { SetValue(CardDescriptionVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardDescriptionVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardDescriptionVisibilityProperty =
            DependencyProperty.Register("CardDescriptionVisibility", typeof(Visibility), typeof(CustomCardControl), new PropertyMetadata(Visibility.Visible));

        public double BackgroundOpacity
        {
            get { return (double)GetValue(BackgroundOpacityProperty); }
            set { SetValue(BackgroundOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundOpacityProperty =
            DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(CustomCardControl), new PropertyMetadata(1d));



        public Brush MouseOverBackgroundColor
        {
            get { return (Brush)GetValue(MouseOverBackgroundColorProperty); }
            set { SetValue(MouseOverBackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseOverBackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBackgroundColorProperty =
            DependencyProperty.Register("MouseOverBackgroundColor", typeof(Brush), typeof(CustomCardControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))));



        static CustomCardControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomCardControl), new FrameworkPropertyMetadata(typeof(CustomCardControl)));

            
        }

    }
}
