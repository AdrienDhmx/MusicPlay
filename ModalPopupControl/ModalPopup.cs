using System.Text;
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

namespace ModalPopupControl
{
    public class ModalPopup : Popup
    {
        private bool _rightClickPassedThrough = false;

        public bool AllowRightClickToPassThrough
        {
            get { return (bool)GetValue(AllowRightClickToPassThroughProperty); }
            set { SetValue(AllowRightClickToPassThroughProperty, value); }
        }

        public static readonly DependencyProperty AllowRightClickToPassThroughProperty =
            DependencyProperty.Register("AllowRightClickToPassThrough", typeof(bool), typeof(ModalPopup), new PropertyMetadata(true));

        static ModalPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalPopup), new FrameworkPropertyMetadata(typeof(ModalPopup)));
        }

        public static T? FindParent<T>(DependencyObject? child) where T : DependencyObject
        {
            DependencyObject? parentObject = GetParentObject(child);

            // end of the tree
            if (parentObject == null) return null;

            T? parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return FindParent<T>(parentObject);
            }
        }

        public static DependencyObject? GetParentObject(DependencyObject? child)
        {
            if (child == null) return null;

            ContentElement? contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement? fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement? frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if(IsOpen)
            {
                e.Handled = true;
                return;
            }
            base.OnPreviewMouseUp(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            UIElement? element = Mouse.DirectlyOver as UIElement;
            ModalPopup? ParentOfElement = GetParentObject(element) as ModalPopup;
            // the click was outside the popup content (the direct parent of element is this)
            if (element is not null && ParentOfElement is not null)
            {

                this.IsOpen = false;
                if (AllowRightClickToPassThrough && e.RightButton == MouseButtonState.Pressed)
                {
                    _rightClickPassedThrough = true;
                }
                else
                {
                    e.Handled = true; // AND DO NOT PROPAGATE THE EVENT LIKE THE DEFAULT POPUP
                }
            }
            else
            {
                base.OnPreviewMouseDown(e);
            }
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if(_rightClickPassedThrough)
            {
                _rightClickPassedThrough = false;
            }
            else
            {
                base.OnPreviewMouseRightButtonDown(e);
            }
        }
    }
}