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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CardDropDown
{
    public class CardDropDown : ContentControl
    {
        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public string HeaderDescription
        {
            get { return (string)GetValue(HeaderDescriptionProperty); }
            set { SetValue(HeaderDescriptionProperty, value); }
        }

        public string Applied
        {
            get { return (string)GetValue(AppliedProperty); }
            set { SetValue(AppliedProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public Brush HeaderDescriptionForeground
        {
            get { return (Brush)GetValue(HeaderDescriptionForegroundProperty); }
            set { SetValue(HeaderDescriptionForegroundProperty, value); }
        }

        public double HeaderDescriptionFontSize
        {
            get { return (double)GetValue(HeaderDescriptionFontSizeProperty); }
            set { SetValue(HeaderDescriptionFontSizeProperty, value); }
        }

        public FontWeight HeaderDescriptionFontWeight
        {
            get { return (FontWeight)GetValue(HeaderDescriptionFontWeightProperty); }
            set { SetValue(HeaderDescriptionFontWeightProperty, value); }
        }

        public Thickness HeaderBorderThickness
        {
            get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
            set { SetValue(HeaderBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty HeaderBorderThicknessProperty =
            DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(CardDropDown), new PropertyMetadata(defaultValue: new Thickness()));

        public static readonly DependencyProperty HeaderDescriptionFontWeightProperty =
            DependencyProperty.Register("HeaderDescriptionFontWeight", typeof(FontWeight), typeof(CardDropDown), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty HeaderDescriptionFontSizeProperty =
            DependencyProperty.Register("HeaderDescriptionFontSize", typeof(double), typeof(CardDropDown), new PropertyMetadata(12d));

        public static readonly DependencyProperty HeaderDescriptionForegroundProperty =
            DependencyProperty.Register("HeaderDescriptionForeground", typeof(Brush), typeof(CardDropDown), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 220, 220, 220))));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(CardDropDown), new PropertyMetadata(TextAlignment.Left));

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(CardDropDown), new PropertyMetadata(25d));

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(CardDropDown), new PropertyMetadata(25d));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CardDropDown), new PropertyMetadata(new CornerRadius(5)));

        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(double), typeof(CardDropDown), new PropertyMetadata(80d));

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(CardDropDown), new PropertyMetadata(false));

        public static readonly DependencyProperty AppliedProperty =
            DependencyProperty.Register("Applied", typeof(string), typeof(CardDropDown), new PropertyMetadata(""));

        public static readonly DependencyProperty HeaderDescriptionProperty =
            DependencyProperty.Register("HeaderDescription", typeof(string), typeof(CardDropDown), new PropertyMetadata(""));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CardDropDown), new PropertyMetadata(""));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(CardDropDown), new PropertyMetadata(new PathGeometry()));

        private static readonly Duration _openCloseDuration = new Duration(TimeSpan.FromSeconds(0.3));
        static CardDropDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardDropDown), new FrameworkPropertyMetadata(typeof(CardDropDown)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IsHitTestVisible = true;

            //PreviewMouseDown += CardDropDown_MouseDown;
            var element = GetTemplateChild("PART_HEADER");
            if (element != null)
            {
                if (element is Border header)
                {
                    header.MouseDown += CardDropDown_MouseDown;
                }
            }
        }

        private void CardDropDown_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void CardDropDown_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var expborder = GetTemplateChild("PART_EXPANDING");
            var content = GetTemplateChild("PART_ContentHost");
            if (expborder != null && content != null)
            {
                if (expborder is Border expanding && content is ContentPresenter contentPresenter)
                {
                    IsOpen = !IsOpen;

                    if (IsOpen)
                    {
                        contentPresenter.Measure(new Size(contentPresenter.MaxWidth, contentPresenter.MaxHeight + 10));
                        DoubleAnimation doubleAnimation = new(contentPresenter.DesiredSize.Height, _openCloseDuration);
                        expanding.BeginAnimation(HeightProperty, doubleAnimation);
                    }
                    else
                    {
                        DoubleAnimation doubleAnimation = new(0, _openCloseDuration);
                        expanding.BeginAnimation(HeightProperty, doubleAnimation);
                    }
                }
            }
                
        }

    }
}
