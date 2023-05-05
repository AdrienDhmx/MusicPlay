using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.MVVM.Models
{
    public class MessageCancelClosedModel<T>
    {
        public bool IsCanceled { get; set; }

        public T Data { get; set; }

        private Func<bool> CancelCallback { get; set; }

        public MessageCancelClosedModel(T data, Func<bool> cancelCallback)
        {
            Data = data;
            CancelCallback = cancelCallback;
        }

        public bool Cancel()
        {
            return IsCanceled = CancelCallback();
        }
    }
}
