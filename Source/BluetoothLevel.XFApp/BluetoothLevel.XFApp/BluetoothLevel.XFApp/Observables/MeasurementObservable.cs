using System;
using BluetoothLevel.XFApp.Models;

namespace BluetoothLevel.XFApp.Observables
{
    public class MeasurementObservable : IObservable<LevelMeasurement>, IDisposable
    {
        private IObserver<LevelMeasurement> _observer;
        private Action _disposeAction;

        public IDisposable Subscribe(IObserver<LevelMeasurement> observer)
        {
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            return this;
        }

        public void NotifyMeasurement(LevelMeasurement measurement)
        {
            if (measurement != null)
            {
                _observer?.OnNext(measurement);
            }            
        }

        public void NotifyError(Exception exception)
        {
            if (exception != null)
            {
                _observer?.OnError(exception);
            }
        }

        public void NotifyCompletion()
        {
            _observer?.OnCompleted();
        }

        public MeasurementObservable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _observer = null;
            _disposeAction?.Invoke();
            _disposeAction = null;
        }
    }
}
