using MessageControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MessageControl
{
    public class MessageHelper
    {
        private static Panel _container;

        public static bool GetIsMessagesContainer(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMessagesContainerProperty);
        }

        public static void SetIsMessagesContainer(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMessagesContainerProperty, value);
        }

        public static readonly DependencyProperty IsMessagesContainerProperty =
            DependencyProperty.RegisterAttached("IsMessagesContainer", typeof(bool), typeof(MessageHelper), new PropertyMetadata(false, OnContainerChanged));

        private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is null) return;

            if(d is Panel container && e.NewValue is bool isContainer && isContainer)
            {
                _container = container;
            }
        }

        public static void PublishMessage(MessageModel message)
        {
            if(_container is not null && message is not null)
            {
                MessageControl messageControl = new MessageControl()
                {
                    Message = message.Message,
                    UndoBtnText = message.UndoMessage,
                    CancelBtnText = message.CancelMessage,
                    ConfirmBtnText = message.ConfirmMessage,
                    Icon = message.Icon,
                    IconStretch = Stretch.Uniform,
                    FillColor = message.IconBrush,
                    StrokeColor = new SolidColorBrush(Color.FromArgb(0,0,0,0)),
                    Foreground = message.Foreground,
                    Background = message.Background,
                    MouseOverBackground = message.MouseOverBackground,
                    IsTimeDisplayed = message.IsDateTimeDisplayed,
                    IsInteractive = message.IsInteractive,
                    IsInteractiveWithCancel = message.IsInteractiveWithCancel,
                    IsUndoneChangeThemeEnabled = message.IsUndoneChangeThemeEnabled,
                    IsUndoBtnVisible = message.IsUndoEnabled,
                    OnCloseCallBack= message.OnCloseCallBack,
                    InteractionCallBack = message.ConfirmCallBack,
                    UndoCallBack= message.UndoCallBack,
                    UndoneMessage = message.UndoneMessage,
                    UndoneFailedMessage = message.UndoneFailedMessage,
                    FontSize = 16,
                    CornerRadius = new(8)
                };

                _container.Children.Insert(0,messageControl);
            }
        }
    }
}
