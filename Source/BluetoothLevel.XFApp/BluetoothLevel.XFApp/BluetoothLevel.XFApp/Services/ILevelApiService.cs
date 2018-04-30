using System;
using System.Threading.Tasks;
using BluetoothLevel.XFApp.Models;

namespace BluetoothLevel.XFApp.Services
{
    public interface ILevelApiService
    {
        Task<bool> ScanForLevel();

        IObservable<LevelMeasurement> GetMeasurementNotifier();

        Task<bool> RequestCalibration();

        Task<bool> RequestMeasurement();
    }
}
