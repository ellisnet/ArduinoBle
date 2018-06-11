using System;

namespace KeyboardMenu.Models
{
    public class KeyboardMessageSubscription : IDisposable
    {
        private Action<KeyboardMessageSubscription> _disposeAction;

        public bool IsDisposed { get; private set; }
        public IObserver<KeyboardMessage> Subscriber { get; private set; }

        public KeyboardMessageSubscription(IObserver<KeyboardMessage> subscriber, Action<KeyboardMessageSubscription> disposeAction)
        {
            Subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            _disposeAction = disposeAction ?? throw new ArgumentNullException(nameof(disposeAction));
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                _disposeAction.Invoke(this);
            }

            IsDisposed = true;
            Subscriber = null;
            _disposeAction = null;
        }
    }
}
