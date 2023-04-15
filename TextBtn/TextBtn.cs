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

namespace TextBtn
{
    public class TextButton : Control
    {
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TextButton), new PropertyMetadata(new CornerRadius(0)));

        public Brush MouseOverBtnColor
        {
            get { return (Brush)GetValue(MouseOverBtnColorProperty); }
            set { SetValue(MouseOverBtnColorProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBtnColorProperty =
            DependencyProperty.Register("MouseOverBtnColor", typeof(Brush), typeof(TextButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(100, 40, 40, 40))));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TextButton), new PropertyMetadata(null));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TextButton), new PropertyMetadata(null));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextButton), new PropertyMetadata(string.Empty));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextButton), new PropertyMetadata(TextAlignment.Left));

        static TextButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextButton), new FrameworkPropertyMetadata(typeof(TextButton)));
        }

        public TextButton()
        {
            MouseLeftButtonUp += MouseLeftButtonUpCommand;
        }

        private void MouseLeftButtonUpCommand(object sender, MouseButtonEventArgs e)
        {
            if (Command is null)
                return;

            Command.Execute(CommandParameter);
        }
    }
}
