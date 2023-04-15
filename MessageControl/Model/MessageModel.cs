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

        public Geometry? Icon { get; set; }

        public Brush? IconBrush { get; set; }

        public Brush? Foreground { get; set; }

        public Brush? Background { get; set; }

        public Brush? MouseOverBackground { get; set; }

        public bool IsInteractive { get; set; } = false;

        public bool IsInteractiveWithCancel { get; set; } = false;

        public bool IsUndoEnabled { get; set; } = false;

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
