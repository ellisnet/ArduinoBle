using System;
using System.Collections.Generic;

namespace KeyboardMenu.Models
{
    public class KeyboardMessageNotifier : IObservable<KeyboardMessage>
    {
        private readonly object _subscriptionLocker = new object();

        private readonly List<KeyboardMessageSubscription> _subscriptions = new List<KeyboardMessageSubscription>();

        public void NotifyCompleted()
        {
            lock (_subscriptionLocker)
            {
                foreach (KeyboardMessageSubscription subscription in _subscriptions)
                {
                    subscription.Subscriber.OnCompleted();
                }
            }
        }

        public void NotifyError(Exception error)
        {
            if (error != null)
            {
                lock (_subscriptionLocker)
                {
                    foreach (KeyboardMessageSubscription subscription in _subscriptions)
                    {
                        subscription.Subscriber.OnError(error);
                    }
                }
            }
        }

        public void NotifyNext(KeyboardMessage value)
        {
            if (value != null)
            {
                lock (_subscriptionLocker)
                {
                    foreach (KeyboardMessageSubscription subscription in _subscriptions)
                    {
                        subscription.Subscriber.OnNext(value);
                    }
                }
            }
        }

        private void RemoveSubscription(KeyboardMessageSubscription subscription)
        {
            if (subscription == null) { throw new ArgumentNullException(nameof(subscription));}
            lock (_subscriptionLocker)
            {
                _subscriptions.Remove(subscription);
            }
        }

        public IDisposable Subscribe(IObserver<KeyboardMessage> observer)
        {
            if (observer == null) { throw new ArgumentNullException(nameof(observer));}
            lock (_subscriptionLocker)
            {
                var newSubscription = new KeyboardMessageSubscription(observer, RemoveSubscription);
                _subscriptions.Add(newSubscription);
                return newSubscription;
            }
        }
    }
}
