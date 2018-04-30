using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BluetoothLevel.XFApp.Models;
using BluetoothLevel.XFApp.Observables;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace BluetoothLevel.XFApp.Services
{
    public class BleLevelApiService : ILevelApiService
    {
        private readonly IUserDialogs _userDialogs;

        // ReSharper disable InconsistentNaming
        private static readonly string DeviceToLookFor = "Adafruit Bluefruit LE";
        private static readonly Guid BleUartServiceId = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        private static readonly Guid BleTxCharacteristicId = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        private static readonly Guid BleRxCharacteristicId = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");
        // ReSharper restore InconsistentNaming

        private TaskCompletionSource<bool> _deviceScanCompletion;
        private bool _isScanning;
        private readonly IAdapter _adapter = CrossBluetoothLE.Current.Adapter;
        private IDevice _bleDevice;
        private bool? _hasLocationPermission;
        private ICharacteristic _rxCharacteristic;
        private ICharacteristic _txCharacteristic;
        private MeasurementObservable _measurementNotifier;

        private Action DisposeNotifierAction => () => _measurementNotifier = null;

        #region Private instance methods

        private void ShowToast(string toastMsg)
        {
            var toastConfig = new ToastConfig(toastMsg);
            toastConfig.SetPosition(ToastPosition.Top);
            toastConfig.SetDuration(2000);
            toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(12, 131, 193));

            Debug.WriteLine(toastMsg);
            _userDialogs.Toast(toastConfig);
        }

        private string GetByteString(byte[] bytes) => (bytes?.Any() ?? false) ? BitConverter.ToString(bytes) : "empty";

        private async Task<ICharacteristic> FindBleCharacteristic(Guid serviceId, Guid characteristicId, IDevice device)
        {
            ICharacteristic result = null;
            if (device == null) { throw new InvalidOperationException("Unable to find a characteristic when there is no BLE device."); }

            try
            {
                IService service = (await device.GetServicesAsync())?.FirstOrDefault(f => f.Id == serviceId);
                if (service != null)
                {
                    result = (await service.GetCharacteristicsAsync())?.FirstOrDefault(f => f.Id == characteristicId);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error while retrieving BLE characteristic '{characteristicId}' on service '{serviceId}': {e}");
                Debugger.Break();
                throw;
            }

            return result;
        }

        private async Task<bool?> CheckLocationPermission(bool? hasLocationPermission)
        {
            if (hasLocationPermission.GetValueOrDefault(false))
            {
                return hasLocationPermission;
            }

            //Only need to request permission on Android API 25+ (the plugin looks at the API level to see if permission must be requested)
            if (hasLocationPermission == null)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                    hasLocationPermission = (status == PermissionStatus.Granted);
                    if (!hasLocationPermission.GetValueOrDefault(false))
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                        {
                            await _userDialogs.AlertAsync("This app requires access to location info to search for nearby devices.", 
                                "App needs location info", "OK");
                        }

                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                        hasLocationPermission = results.ContainsKey(Permission.Location) &&
                                                 results[Permission.Location] == PermissionStatus.Granted;
                    }
                }
                else
                {
                    hasLocationPermission = true;
                }
            }

            return hasLocationPermission;
        }

        private async Task<bool> RegisterBleDevice(IDevice device)
        {
            bool registered = false;

            bool isConnectable = true;

            Debug.WriteLine($"BLE device name: {device.Name}");
            Debug.WriteLine($"BLE device ID: {device.Id}");
            Debug.WriteLine($"BLE device state: {device.State}");
            Debug.WriteLine($"BLE device RSSI: {device.Rssi}");
            if (device.AdvertisementRecords?.Any() ?? false)
            {
                int index = 0;
                foreach (AdvertisementRecord record in device.AdvertisementRecords)
                {
                    // ReSharper disable once RedundantAssignment
                    string value = "";
                    switch (record.Type)
                    {
                        case AdvertisementRecordType.ShortLocalName:
                        case AdvertisementRecordType.CompleteLocalName:
                            value = Encoding.Default.GetString(record.Data);
                            break;
                        case AdvertisementRecordType.TxPowerLevel:
                        case AdvertisementRecordType.Deviceclass:
                        case AdvertisementRecordType.SimplePairingHash:
                        case AdvertisementRecordType.SimplePairingRandomizer:
                        case AdvertisementRecordType.DeviceId:
                        case AdvertisementRecordType.SecurityManager:
                        case AdvertisementRecordType.SlaveConnectionInterval:
                        case AdvertisementRecordType.SsUuids16Bit:
                        case AdvertisementRecordType.SsUuids128Bit:
                        case AdvertisementRecordType.ServiceData:
                        case AdvertisementRecordType.PublicTargetAddress:
                        case AdvertisementRecordType.RandomTargetAddress:
                        case AdvertisementRecordType.Appearance:
                        case AdvertisementRecordType.DeviceAddress:
                        case AdvertisementRecordType.LeRole:
                        case AdvertisementRecordType.PairingHash:
                        case AdvertisementRecordType.PairingRandomizer:
                        case AdvertisementRecordType.SsUuids32Bit:
                        case AdvertisementRecordType.ServiceDataUuid32Bit:
                        case AdvertisementRecordType.ServiceData128Bit:
                        case AdvertisementRecordType.SecureConnectionsConfirmationValue:
                        case AdvertisementRecordType.SecureConnectionsRandomValue:
                        case AdvertisementRecordType.Information3DData:
                        case AdvertisementRecordType.ManufacturerSpecificData:
                        case AdvertisementRecordType.Flags:
                        case AdvertisementRecordType.UuidsIncomple16Bit:
                        case AdvertisementRecordType.UuidsComplete16Bit:
                        case AdvertisementRecordType.UuidsIncomplete32Bit:
                        case AdvertisementRecordType.UuidCom32Bit:
                        case AdvertisementRecordType.UuidsIncomplete128Bit:
                        case AdvertisementRecordType.UuidsComplete128Bit:
                            value = "Bytes: " + GetByteString(record.Data);
                            break;
                        case AdvertisementRecordType.IsConnectable:
                            isConnectable = record.Data[0] == 1;
                            value = isConnectable.ToString();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Advertisement Record Type is unknown: {(int)record.Type}");
                    }
                    Debug.WriteLine($"BLE device AdvertismentRecord {index}: {record.Type}" + (value == "" ? "" : $" - {value}"));
                    index++;
                }
            }

            if (isConnectable)
            {
                registered = await ConnectToDevice(device);
            }

            return registered;
        }

        private void NotifyAboutIncomingBleMessage(byte[] incoming)
        {
            if (incoming != null && incoming.Length > 0)
            {
                //Debug.WriteLine($"Received bytes: {GetByteString(incoming)}");
                string message = Encoding.ASCII.GetString(incoming).Trim().ToUpper();
                //Debug.WriteLine($"Translated to: {message}");
                
                foreach (string measurement in message.Split(':'))
                {
                    string[] measurementParts = measurement.Split(',');
                    if (measurementParts.Length == 2
                        && (measurementParts[0].Trim() == "INT" || measurementParts[0].Trim() == "FINAL")
                        && Int32.TryParse(measurementParts[1], out int value))
                    {
                        var lvlMeasurement = new LevelMeasurement
                        {
                            MeasurementType = (measurementParts[0].Trim() == "FINAL")
                                ? MeasurementType.Final
                                : MeasurementType.Intermediate,
                            Value = value
                        };

                        _measurementNotifier?.NotifyMeasurement(lvlMeasurement);
                    }
                }
            }
        }

        private async Task<bool> ConnectToDevice(IDevice bleDevice)
        {
            bool connected = false;

            try
            {
                await _adapter.ConnectToDeviceAsync(bleDevice);

                _bleDevice = bleDevice;
                _rxCharacteristic = await FindBleCharacteristic(BleUartServiceId, BleRxCharacteristicId, _bleDevice);
                if (_rxCharacteristic == null)
                {
                    throw new InvalidOperationException("Unable to retrieve the characteristic needed to read command responses from the BLE device.");
                }
                else if (!_rxCharacteristic.CanUpdate)
                {
                    throw new InvalidOperationException("The characteristic needed for notifications from the BLE device does not support this operation.");
                }

                _rxCharacteristic.ValueUpdated += (sender, charUpdatedEventArgs) =>
                {
                    if (_measurementNotifier != null)
                    {
                        NotifyAboutIncomingBleMessage(charUpdatedEventArgs?.Characteristic?.Value);
                    }
                };

                await _rxCharacteristic.StartUpdatesAsync();

                connected = true;
            }
            catch (DeviceConnectionException connectEx)
            {
                Debug.WriteLine($"Device connection problem: {connectEx}");
            }
            catch (InvalidOperationException operationEx)
            {
                Debug.WriteLine($"Device operation problem: {operationEx}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Device data reading problem: {ex}");
            }

            return connected;
        }

        private async Task<bool> SendBleMessage(string message)
        {
            bool result = false;

            if (_bleDevice != null && (!String.IsNullOrWhiteSpace(message)))
            {
                _txCharacteristic = _txCharacteristic
                    ?? await FindBleCharacteristic(BleUartServiceId, BleTxCharacteristicId, _bleDevice);
                if (_txCharacteristic == null)
                {
                    throw new InvalidOperationException("Unable to retrieve the characteristic needed to send commands to the BLE device.");
                }
                else if (!_txCharacteristic.CanWrite)
                {
                    throw new InvalidOperationException("The characteristic needed to send commands to the BLE device does not support this operation.");
                }

                byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
                result = await _txCharacteristic.WriteAsync(bytesToSend);
            }

            return result;
        }

        #endregion

        public async Task<bool> ScanForLevel()
        {
            bool result = false;

            if (!_isScanning)
            {
                _isScanning = true;

                ShowToast("Scanning for BLE devices...");

                _hasLocationPermission = await CheckLocationPermission(_hasLocationPermission);

                if (_hasLocationPermission.GetValueOrDefault(false))
                {
                    _bleDevice = null;
                    _deviceScanCompletion = new TaskCompletionSource<bool>();

                    _adapter.DeviceDiscovered += async (o, eventArgs) =>
                    {
                        if (!String.IsNullOrWhiteSpace(eventArgs.Device.Name) && eventArgs.Device.Name.Contains(DeviceToLookFor))
                        {
                            Debug.WriteLine($"Bluetooth LE device found: {eventArgs.Device.Name}");
                            if (await RegisterBleDevice(eventArgs.Device))
                            {
                                ShowToast("Status: Device successfully connected!");
                                await _adapter.StopScanningForDevicesAsync();
                                await Task.Delay(3000);
                                if (_deviceScanCompletion?.Task != null && !_deviceScanCompletion.Task.IsCompleted)
                                {
                                    _deviceScanCompletion.SetResult(true);
                                }                                
                            }
                        }
                    };

                    _adapter.ScanTimeoutElapsed += (o, args) =>
                    {
                        Debug.WriteLine("Scan timed out.");
                        if (_deviceScanCompletion?.Task != null && !_deviceScanCompletion.Task.IsCompleted)
                        {
                            _deviceScanCompletion.SetResult(false);
                        }
                    };

                    _adapter.ScanMode = ScanMode.Balanced;
                    _adapter.ScanTimeout = 10000; //Should be 10 seconds

                    await _adapter.StartScanningForDevicesAsync();
                    result = await _deviceScanCompletion.Task;
                    if (_bleDevice == null)
                    {
                        ShowToast("Status: No device found.");
                        await Task.Delay(5000);
                    }
                }

                if (!result)
                {
                    ShowToast("Done scanning.");
                }

                _isScanning = false;
            }

            return result;
        }

        public IObservable<LevelMeasurement> GetMeasurementNotifier() 
            => (_measurementNotifier = new MeasurementObservable(DisposeNotifierAction));

        public Task<bool> RequestCalibration() => SendBleMessage("CA");

        public Task<bool> RequestMeasurement() => SendBleMessage("ST");

        public BleLevelApiService(IUserDialogs userDialogs)
        {
            _userDialogs = userDialogs ?? throw new ArgumentNullException(nameof(userDialogs));
        }
    }
}
