using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MessageControl.Model
{
    public class MessageModel : ICloneable
    {
        public string Message { get; set; } = string.Empty;

        public string CancelMessage { get; set; } = string.Empty;

        public string ConfirmMessage { get; set; } = string.Empty;

        public string UndoMessage { get; set; } = string.Empty;

        public string UndoneMessage { get; set; } = string.Empty;

        public string UndoneFailedMessage { get; set; } = string.Empty;

        public Geometry? Icon { get; set; }

        public Brush? IconBrush { get; set; }

        public Brush? Foreground { get; set; }

        public Brush? Background { get; set; }

        public Brush? MouseOverBackground { get; set; }    

        public bool IsInteractive { get; set; } = false;

        public bool IsInteractiveWithCancel { get; set; } = false;

        public bool IsUndoEnabled { get; set; } = false;

        /// <summary>
        /// When the undo button is pressed the theme of the message will change
        /// If successful the Success theme registered in <see cref="DefaultMessageFactory"/> will be used,
        /// if it failed then the error theme registered will be used
        /// If no theme is not registered the theme will not change
        /// </summary>
        public bool IsUndoneChangeThemeEnabled = true;

        public bool StaysOpen { get; set; } = false;

        public bool IsDateTimeDisplayed { get; set; } = true;

        public Action? OnCloseCallBack { get; set; } = null;

        public Action<bool>? ConfirmCallBack { get; set; } = null;

        public Func<bool>? UndoCallBack { get; set; } = null;

        public object Clone()
        {
            MessageModel clone = (MessageModel)MemberwiseClone();

            return clone;
        }
    }
}
