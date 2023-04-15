using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace IconButton
{
    public class IconButton : Control
    {
        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Brush FillColor
        {
            get { return (Brush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public Brush StrokeColor
        {
            get { return (Brush)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Stretch IconStretch
        {
            get { return (Stretch)GetValue(IconStretchProperty); }
            set { SetValue(IconStretchProperty, value); }
        }

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(IconButton), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(IconButton), new PropertyMetadata(null));

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 90, 90, 90))));

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(IconButton), new PropertyMetadata(20d));

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(IconButton), new PropertyMetadata(20d));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(IconButton), new PropertyMetadata(new PathGeometry()));

        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register("FillColor", typeof(Brush), typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250,250,250,250))));

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register("StrokeColor", typeof(Brush), typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(IconButton), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty IconStretchProperty =
            DependencyProperty.Register("IconStretch", typeof(Stretch), typeof(IconButton), new PropertyMetadata(Stretch.None));

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(IconButton), new PropertyMetadata(new Thickness(4)));

        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        public override void EndInit()
        {
            base.EndInit();

            if (IconWidth < 0) IconWidth = 0;
            if (IconHeight <0) IconHeight = 0;
        }

        public IconButton()
        {
            PreviewMouseLeftButtonDown += MouseLeftButtonDownCommand;
            PreviewMouseLeftButtonUp += IconButton_PreviewMouseLeftButtonUp;
        }

        private void IconButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MouseLeftButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            if(Command is not null)
            {
                e.Handled= true;
                Command.Execute(CommandParameter);
            }
        }

    }
}
