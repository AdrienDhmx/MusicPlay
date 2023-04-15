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

namespace PlaceHolderTextBox
{
    public class PlaceholderTextBox : TextBox
    {
        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            private set { SetValue(IsEmptyPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for IsEmpty.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey IsEmptyPropertyKey =
            DependencyProperty.RegisterReadOnly("IsEmpty", typeof(bool), typeof(PlaceholderTextBox), new PropertyMetadata(true));

        public static readonly DependencyProperty IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(string.Empty));



        public Brush PlaceholderForeground
        {
            get { return (Brush)GetValue(PlaceholderForegroundProperty); }
            set { SetValue(PlaceholderForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(PlaceholderTextBox), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250,250,250,250))));




        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PlaceholderTextBox), new PropertyMetadata(new CornerRadius(0)));


        static PlaceholderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            IsEmpty = string.IsNullOrEmpty(Text);

            base.OnTextChanged(e);
        }
    }
}
