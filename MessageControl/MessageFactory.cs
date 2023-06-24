using MessageControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MessageControl
{
    public static class DefaultMessageFactory
    {
        public static MessageModel? ErrorMessageStyle { get; private set; }
        public static MessageModel? WarningMessageStyle { get; private set; }
        public static MessageModel? SuccessMessageStyle { get; private set; }
        public static MessageModel? InfoMessageStyle { get; private set; }

        public static void RegisterErrorMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon)
        {
            RegisterErrorMessageStyle(background, foreground, mouseOver, icon, false, false, true, false);
        }

        public static void RegisterErrorMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon, bool staysOpen, bool isInteractive, bool isDateTimeDisplayed, bool isUndoEnabled)
        {
            ErrorMessageStyle = new()
            {
                Background = background,
                Foreground = foreground,
                MouseOverBackground = mouseOver,
                IconBrush = foreground,
                Icon = icon,
                IsInteractive= isInteractive,
                IsUndoEnabled = isUndoEnabled,
                IsDateTimeDisplayed = isDateTimeDisplayed,
                StaysOpen =staysOpen,
            };
        }

        /// <summary>
        /// Create a message model using the default error message model if it has been registered
        /// </summary>
        /// <param name="message"></param>
        /// <returns> the message model used for publishing a message with <see cref="MessageHelper.PublishMessage(MessageModel)"/> 
        /// if the default error message has been registered, otherwise return null
        /// </returns>
        public static MessageModel? CreateErrorMessage(string message)
        {
            if(ErrorMessageStyle != null)
            {
                MessageModel errorMessage = (MessageModel)ErrorMessageStyle.Clone();
                errorMessage.Message = message; 
                return errorMessage;
            }
            else
            {
                return null;
            }
        }

        public static void RegisterWarningMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon)
        {
            RegisterWarningMessageStyle(background, foreground, mouseOver, icon, false, false, true, false);
        }

        public static void RegisterWarningMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon, bool staysOpen, bool isInteractive, bool isDateTimeDisplayed, bool isUndoEnabled)
        {
            WarningMessageStyle = new()
            {
                Background = background,
                Foreground = foreground,
                MouseOverBackground = mouseOver,
                IconBrush = foreground,
                Icon = icon,
                IsInteractive = isInteractive,
                IsUndoEnabled = isUndoEnabled,
                IsDateTimeDisplayed = isDateTimeDisplayed,
                StaysOpen = staysOpen,
            };
        }

        public static MessageModel? CreateWarningMessage(string message)
        {
            if (WarningMessageStyle != null)
            {
                MessageModel errorMessage = (MessageModel)WarningMessageStyle.Clone();
                errorMessage.Message = message;
                return errorMessage;
            }
            else
            {
                return null;
            }
        }

        public static void RegisterSuccessMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon)
        {
            RegisterSuccessMessageStyle(background, foreground, mouseOver, icon, false, false, true, false);
        }

        public static void RegisterSuccessMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon, bool staysOpen, bool isInteractive, bool isDateTimeDisplayed, bool isUndoEnabled)
        {
            SuccessMessageStyle = new()
            {
                Background = background,
                Foreground = foreground,
                MouseOverBackground = mouseOver,
                IconBrush = foreground,
                Icon = icon,
                IsInteractive = isInteractive,
                IsUndoEnabled = isUndoEnabled,
                IsDateTimeDisplayed = isDateTimeDisplayed,
                StaysOpen = staysOpen,
            };
        }

        public static MessageModel? CreateSuccessMessage(string message)
        {
            if (SuccessMessageStyle != null)
            {
                MessageModel Message = (MessageModel)SuccessMessageStyle.Clone();
                Message.Message = message;
                return Message;
            }
            else
            {
                return null;
            }
        }

        public static void RegisterInfoMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon)
        {
            RegisterInfoMessageStyle(background, foreground, mouseOver, icon, false, false, true, false);
        }

        public static void RegisterInfoMessageStyle(Brush background, Brush foreground, Brush mouseOver, Geometry icon, bool staysOpen, bool isInteractive, bool isDateTimeDisplayed, bool isUndoEnabled)
        {
            InfoMessageStyle = new()
            {
                Background = background,
                Foreground = foreground,
                MouseOverBackground = mouseOver,
                IconBrush = foreground,
                Icon = icon,
                IsInteractive = isInteractive,
                IsUndoEnabled = isUndoEnabled,
                IsDateTimeDisplayed = isDateTimeDisplayed,
                StaysOpen = staysOpen,
            };
        }

        public static MessageModel? CreateInfoMessage(string message)
        {
            if (InfoMessageStyle != null)
            {
                MessageModel Message = (MessageModel)InfoMessageStyle.Clone();
                Message.Message = message;
                return Message;
            }
            else
            {
                return null;
            }
        }
    }
}
