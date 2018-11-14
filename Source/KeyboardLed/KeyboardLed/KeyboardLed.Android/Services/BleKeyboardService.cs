using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CodeBrix.Prism.Observables;
using KeyboardLed.Models;
using KeyboardLed.Services;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace KeyboardLed.Droid.Services
{
    public class BleKeyboardService : IBleKeyboardService
    {
        // ReSharper disable once InconsistentNaming
        private static readonly string DeviceToLookFor = "Adafruit Bluefruit LE";
        public static readonly Guid BleUartServiceId = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly Guid BleTxCharacteristicId = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly Guid BleRxCharacteristicId = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");

        private static readonly IAdapter adapter = CrossBluetoothLE.Current.Adapter;

        private IUserDialogs _userDialogService;
        private SimpleObserverNotifier<KeyboardMessage> _notifier = new SimpleObserverNotifier<KeyboardMessage>();

        private bool _isScanning;        
        private IDevice _bleDevice;
        private bool? _hasLocationPermission;
        private ICharacteristic _rxCharacteristic;
        private ICharacteristic _txCharacteristic;
        private bool _connected;

        private void ShowToast(string toastMsg)
        {
            var toastConfig = new ToastConfig(toastMsg);
            toastConfig.SetPosition(ToastPosition.Top);
            toastConfig.SetDuration(2000);
            toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(12, 131, 193));

            Debug.WriteLine(toastMsg);

            _userDialogService.Toast(toastConfig);
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
                            await _userDialogService.AlertAsync("App needs location info",
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

        private void CharacteristicUpdated(object sender, CharacteristicUpdatedEventArgs args)
        {
            byte[] receivedBytes = args?.Characteristic?.Value;
            if (receivedBytes != null && receivedBytes.Length > 0)
            {
                Debug.WriteLine($"Received bytes: {GetByteString(receivedBytes)}");
                var message = KeyboardMessage.FromReceivedMessage(Encoding.ASCII.GetString(receivedBytes));
                Debug.WriteLine(message.Message.ToString());
                if (message.Message != KeyMessage.Unknown)
                {
                    _notifier.NotifyNext(message);
                }
            }
        }

        private async Task<bool> ConnectToDevice(IDevice bleDevice)
        {
            if (!_connected)
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
                    _connected = true;
                }
                catch (DeviceConnectionException connectEx)
                {
                    Debug.WriteLine($"Device connection problem: {connectEx.ToString()}");
                }
                catch (InvalidOperationException operationEx)
                {
                    Debug.WriteLine($"Device operation problem: {operationEx.ToString()}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Device data reading problem: {ex.ToString()}");
                }
            }

            return _connected;
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

        public async Task<bool> ScanForKeyboard()
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
                                await Task.Delay(3000);
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

        public IObservable<KeyboardMessage> GetMessageNotifier() => _notifier;

        public async Task<bool> SendLedMessage(LedMessage message)
        {
            bool result = false;
            byte[] bytesToSend = KeyboardMessage.GetLedMessageBytes(message);

            if (_bleDevice != null && bytesToSend != null)
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

            return result;
        }

        public BleKeyboardService(IUserDialogs userDialogService)
        {
            _userDialogService = userDialogService ?? throw new ArgumentNullException(nameof(userDialogService));
        }

        public async void Dispose()
        {
            if (_connected && _rxCharacteristic != null)
            {
                await _rxCharacteristic.StopUpdatesAsync();
                _rxCharacteristic.ValueUpdated -= CharacteristicUpdated;
            }
            _connected = false;
            _rxCharacteristic = null;
            _txCharacteristic = null;

            _bleDevice?.Dispose();
            _bleDevice = null;
            _notifier.NotifyCompleted();
            _notifier = null;
            _userDialogService = null;
        }
    }
}