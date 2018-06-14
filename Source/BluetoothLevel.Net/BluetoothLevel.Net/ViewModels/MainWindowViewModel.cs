using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BluetoothLevel.Net.BleAdapter;
using BluetoothLevel.Net.BleHelper;
using BluetoothLevel.Net.Models;

//Check this website for info about adding appropriate references:
// https://docs.microsoft.com/en-us/windows/uwp/porting/desktop-to-uwp-enhance#first-set-up-your-project

namespace BluetoothLevel.Net.ViewModels
{
    public class MainWindowViewModel : SimpleViewModel
    {
        // ReSharper disable InconsistentNaming
        private static readonly string DeviceToLookFor = "Adafruit Bluefruit LE";
        private static readonly Guid BleUartServiceId = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        private static readonly Guid BleTxCharacteristicId = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        private static readonly Guid BleRxCharacteristicId = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");
        // ReSharper restore InconsistentNaming

        private readonly IAdapter _adapter;
        private TaskCompletionSource<bool> _deviceScanCompletion;
        private IDevice _bleDevice;
        private ICharacteristic _rxCharacteristic;
        private ICharacteristic _txCharacteristic;
        private int _previousValue;

        #region Bindable properties

        private bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (value != _isScanning)
                {
                    _isScanning = value;
                    NotifyPropertyChanged(nameof(IsScanning));
                    ScanForDeviceCommand.RaiseCanExecuteChanged();
                }                
            }
        }

        #endregion

        #region Commands and their implementations

        #region ScanForDeviceCommand

        private SimpleCommand _scanForDeviceCommand;
        public SimpleCommand ScanForDeviceCommand => _scanForDeviceCommand
            ?? (_scanForDeviceCommand = new SimpleCommand(() => !IsScanning, DoScanForDevice));

        public async void DoScanForDevice()
        {
            try
            {
                IsScanning = true;

                _bleDevice = null;
                _deviceScanCompletion = new TaskCompletionSource<bool>();

                _adapter.DeviceDiscovered += async (o, eventArgs) =>
                {
                    if (!String.IsNullOrWhiteSpace(eventArgs.Device.Name) && eventArgs.Device.Name.Contains(DeviceToLookFor))
                    {
                        Debug.WriteLine($"Bluetooth LE device found: {eventArgs.Device.Name}");
                        if (await RegisterBleDevice(eventArgs.Device))
                        {
                            await ShowInfo("Status: Device successfully connected!");
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
                await _deviceScanCompletion.Task;
                if (_bleDevice == null)
                {
                    await ShowError("Status: No device found.");
                }
            }
            catch (Exception e)
            {
                string details = e.ToString();
                Debugger.Break();
                await ShowError(e, "An error occurred while scanning for BLE devices.");
            }
            finally
            {
                IsScanning = false;
            }
        }

        #endregion

        #endregion

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

        private string GetByteString(byte[] bytes) => (bytes?.Any() ?? false) ? BitConverter.ToString(bytes) : "empty";

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
                    NotifyAboutIncomingBleMessage(charUpdatedEventArgs?.Characteristic?.Value);
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
                _previousValue = 0;
            }

            return result;
        }

        private void NotifyAboutIncomingBleMessage(byte[] incoming)
        {
            if (incoming != null && incoming.Length > 0)
            {
                Debug.WriteLine($"Received bytes: {GetByteString(incoming)}");
                string message = Encoding.ASCII.GetString(incoming).Trim().ToUpper();
                Debug.WriteLine($"Translated to: {message}");

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
                            CurrentValue = value,
                            PreviousValue = _previousValue
                        };

                        _previousValue = value;

                        //TODO: Need to notify something?
                        //_measurementNotifier?.NotifyMeasurement(lvlMeasurement);
                    }
                }
            }
        }

        public MainWindowViewModel()
        {
            _adapter = BluetoothLEHelper.GetBluetoothLE().Adapter;
        }
    }
}
