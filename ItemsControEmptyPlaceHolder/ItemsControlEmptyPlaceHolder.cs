using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ItemsControEmptyPlaceHolder
{
    public class ItemsControlEmptyPlaceHolder : ItemsControl
    {
        public Brush PlaceholderForeground
        {
            get { return (Brush)GetValue(PlaceholderForegroundProperty); }
            set { SetValue(PlaceholderForegroundProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(ItemsControlEmptyPlaceHolder), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(ItemsControlEmptyPlaceHolder), new PropertyMetadata(string.Empty));

        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            private set { SetValue(IsEmptyPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsEmptyPropertyKey =
            DependencyProperty.RegisterReadOnly("IsEmpty", typeof(bool), typeof(ItemsControlEmptyPlaceHolder), new PropertyMetadata(true));

        public static readonly DependencyProperty IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;

        static ItemsControlEmptyPlaceHolder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemsControlEmptyPlaceHolder), new FrameworkPropertyMetadata(typeof(ItemsControlEmptyPlaceHolder)));
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        { 
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue is null)
            {
                IsEmpty = true;
            }
            else
            {
                IEnumerator enumerator = newValue.GetEnumerator();
                enumerator.Reset();
                IsEmpty = !enumerator.MoveNext();
            }
        }
    }
}
