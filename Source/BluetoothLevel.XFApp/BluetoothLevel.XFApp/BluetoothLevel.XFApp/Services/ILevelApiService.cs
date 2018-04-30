using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLevel.XFApp.Services
{
    public interface ILevelApiService
    {
        Task<bool> ScanForLevel();

        Task<bool> RequestCalibration();

        Task<bool> RequestMeasurement();
    }
}
