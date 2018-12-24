using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CodeBrix.Prism.Abstract;
using CodeBrix.Prism.Services;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace RocketLauncher.Services
{
    public class BleLauncherService : LoggerBase, ILauncherService
    {
        // ReSharper disable once InconsistentNaming
        private static readonly string DeviceToLookFor = "Adafruit Bluefruit LE";
        public static readonly Guid BleUartServiceId = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly Guid BleTxCharacteristicId = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly Guid BleRxCharacteristicId = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");

        private static readonly IAdapter adapter = CrossBluetoothLE.Current.Adapter;

        private static readonly SemaphoreSlim _sendMessageLocker = new SemaphoreSlim(1, 1);

        private readonly IUserDialogs _dialogService;

        private bool _isScanning;
        private IDevice _bleDevice;
        private bool? _hasLocationPermission;
        private ICharacteristic _rxCharacteristic;
        private ICharacteristic _txCharacteristic;

        private Action _launchCompleteAction;

        private void ShowToast(string toastMsg)
        {
            var toastConfig = new ToastConfig(toastMsg);
            toastConfig.SetPosition(ToastPosition.Top);
            toastConfig.SetDuration(2000);
            toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(12, 131, 193));

            Debug.WriteLine(toastMsg);

            _dialogService.Toast(toastConfig);
        }

        private string GetByteString(byte[] bytes) => (bytes?.Any() ?? false) ? BitConverter.ToString(bytes) : "empty";

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
                            await _dialogService.AlertAsync("App needs location info",
                                "This app requires access to location info to search for nearby devices.", "OK");
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
                    string value;
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

        private void CharacteristicUpdated(object sender, CharacteristicUpdatedEventArgs args)
        {
            byte[] receivedBytes = args?.Characteristic?.Value;
            if (receivedBytes != null && receivedBytes.Length > 0)
            {
                Debug.WriteLine($"Received bytes: {GetByteString(receivedBytes)}");
                string message = (Encoding.ASCII.GetString(receivedBytes)).Trim().ToUpper();
                if (message == "LC")
                {
                    _launchCompleteAction?.Invoke();
                }
            }
        }

        private async Task<bool> ConnectToDevice(IDevice bleDevice)
        {
            if (!IsConnected)
            {
                try
                {
                    await adapter.ConnectToDeviceAsync(bleDevice);

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

                    _rxCharacteristic.ValueUpdated += CharacteristicUpdated;
                    await _rxCharacteristic.StartUpdatesAsync();
                    IsConnected = true;
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
            }

            return IsConnected;
        }

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

        private async Task<bool> SendMessage(string message)
        {
            bool result = false;

            if (IsConnected && (!String.IsNullOrWhiteSpace(message)))
            {
                await _sendMessageLocker.WaitAsync();

                try
                {
                    byte[] bytesToSend = Encoding.ASCII.GetBytes(message.Trim().ToUpper());

                    if (_bleDevice != null)
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

                        result = await _txCharacteristic.WriteAsync(bytesToSend);
                    }
                }
                catch (Exception e)
                {
                    LogError(e);
                }
                finally
                {
                    _sendMessageLocker.Release();
                }
            }

            return result;
        }

        #region ILauncherService implementation

        public bool IsConnected { get; private set; }

        public async Task<bool> Connect()
        {
            bool connected = false;

            if (!_isScanning)
            {
                _isScanning = true;
                ShowToast("Scanning for BLE devices...");

                _hasLocationPermission = await CheckLocationPermission(_hasLocationPermission);

                if (_hasLocationPermission.GetValueOrDefault(false))
                {
                    _bleDevice = null;
                    var scanCompletionSource = new TaskCompletionSource<bool>();

                    adapter.DeviceDiscovered += async (o, eventArgs) =>
                    {
                        if (!String.IsNullOrWhiteSpace(eventArgs.Device.Name) && eventArgs.Device.Name.Contains(DeviceToLookFor))
                        {
                            Debug.WriteLine($"Bluetooth LE device found: {eventArgs.Device.Name}");
                            if (await RegisterBleDevice(eventArgs.Device))
                            {
                                ShowToast("Status: Device successfully connected!");
                                await adapter.StopScanningForDevicesAsync();
                                await Task.Delay(500); //Not sure what this delay is for
                                //await Task.Delay(3000);
                                scanCompletionSource?.TrySetResult(true);
                            }
                        }
                    };

                    adapter.ScanTimeoutElapsed += (o, args) =>
                    {
                        Debug.WriteLine("Scan timed out.");
                        scanCompletionSource?.TrySetResult(false);
                    };

                    adapter.ScanMode = ScanMode.Balanced;
                    adapter.ScanTimeout = 10000; //Should be 10 seconds

                    await adapter.StartScanningForDevicesAsync();
                    connected = await scanCompletionSource.Task;
                    if (_bleDevice == null)
                    {
                        ShowToast("Status: No device found.");
                        await Task.Delay(5000);
                    }
                }
            };

            _isScanning = false;
            if (!connected)
            {
                ShowToast("Done scanning.");
            }

            return connected;
        }

        public Task<bool> SendLaunchMessage(string message, Action launchCompleteAction = null)
        {
            _launchCompleteAction = launchCompleteAction;
            return SendMessage(message);
        }

        #endregion

        public BleLauncherService(ILoggerService loggerService, 
            IUserDialogs dialogService)
            : base(loggerService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }
    }
}
