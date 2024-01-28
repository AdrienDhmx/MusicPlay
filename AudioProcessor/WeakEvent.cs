using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioHandler
{
    public class WeakEvent<T> where T : Delegate
    {
        private readonly List<WeakReference<T>> _callbacks = [];

        public void RegisterCallback(T callback)
        {
            _callbacks.Add(new WeakReference<T>(callback));
        }

        public void RemoveCallback(T callback)
        {
            _callbacks.RemoveAll(wr => !wr.TryGetTarget(out var target) || target == callback);
        }

        /// <summary>
        /// Invoke all the Delegate registered.
        /// </summary>
        public void TriggerEvent(params object[] parameters)
        {
            foreach (var weakRef in _callbacks)
            {
                if (weakRef.TryGetTarget(out T callback))
                {
                    callback.DynamicInvoke(parameters);
                }
            }
        }
    }
}
