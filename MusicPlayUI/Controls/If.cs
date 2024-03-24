using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicPlayUI.Controls
{
    /// <summary>
    /// This component is taken from https://github.com/SingletonSean/maui-tutorials/blob/master/ConditionalRender/If.cs
    /// <br>
    ///   The original code being for MAUI I changed ContentView to ContentControl and View to UIElement...
    /// </br>
    /// </summary>
    public class If : ContentControl
    {
        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register(nameof(Condition), typeof(bool), typeof(If), new PropertyMetadata(false, OnContentDependentPropertyChanged));

        public bool Condition
        {
            get => (bool)GetValue(ConditionProperty);
            set => SetValue(ConditionProperty, value);
        }

        public static readonly DependencyProperty TrueProperty =
            DependencyProperty.Register(nameof(True), typeof(UIElement), typeof(If), new PropertyMetadata(null, OnContentDependentPropertyChanged));

        public UIElement True
        {
            get => (UIElement)GetValue(TrueProperty);
            set => SetValue(TrueProperty, value);
        }

        public static readonly DependencyProperty FalseProperty =
            DependencyProperty.Register(nameof(False), typeof(UIElement), typeof(If), new PropertyMetadata(null, OnContentDependentPropertyChanged));

        public UIElement False
        {
            get => (UIElement)GetValue(FalseProperty);
            set => SetValue(FalseProperty, value);
        }

        private static void OnContentDependentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            If currentIf = (If)d;
            currentIf.UpdateContent();
        }

        private void UpdateContent()
        {
            if (Condition)
            {
                Content = True;
            }
            else
            {
                Content = False;
            }
        }
    }
}
