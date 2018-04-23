using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace ArduinoBle.XFApp
{
	public partial class MainPage : ContentPage
	{
	    private static readonly string DeviceToLookFor = "Adafruit Bluefruit LE";
	    public static readonly Guid BleCommServiceId = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
	    public static readonly Guid BleTxCharacteristicId = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
	    public static readonly Guid BleRxCharacteristicId = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");

        private bool _isScanning;
	    private readonly IAdapter _adapter = CrossBluetoothLE.Current.Adapter;
	    private IDevice _bleDevice;
	    private bool? _hasLocationPermission;

        public MainPage()
		{
			InitializeComponent();
		}

	    private async void UiScanButton_Clicked(object sender, EventArgs e)
	    {
	        if (!_isScanning)
	        {
	            _isScanning = true;
	            UiScanButton.IsEnabled = !_isScanning;
                App.ShowToast("Scanning for BLE devices...");

                _hasLocationPermission = await CheckLocationPermission(_hasLocationPermission);

                if (_hasLocationPermission.GetValueOrDefault(false))
                {
                    _bleDevice = null;
                    var scanCompletionSource = new TaskCompletionSource<bool>();

                    _adapter.DeviceDiscovered += async (o, eventArgs) =>
                    {
                        if (!String.IsNullOrWhiteSpace(eventArgs.Device.Name) && eventArgs.Device.Name.Contains(DeviceToLookFor))
                        {
                            Debug.WriteLine($"Bluetooth LE device found: {eventArgs.Device.Name}");
                            if (await RegisterBleDevice(eventArgs.Device))
                            {
                                App.ShowToast("Status: Device successfully connected!");
                                await _adapter.StopScanningForDevicesAsync();
                                await Task.Delay(3000);
                                scanCompletionSource.SetResult(true);
                            }
                        }
                    };

                    _adapter.ScanTimeoutElapsed += (o, args) =>
                    {
                        Debug.WriteLine("Scan timed out.");
                        scanCompletionSource.SetResult(false);
                    };

                    _adapter.ScanMode = ScanMode.Balanced;
                    _adapter.ScanTimeout = 10000; //Should be 10 seconds

                    await _adapter.StartScanningForDevicesAsync();
                    await scanCompletionSource.Task;
                    if (_bleDevice == null)
                    {
                        App.ShowToast("Status: No device found.");
                        await Task.Delay(5000);
                    }
                }
            };

            _isScanning = false;
	        UiScanButton.IsEnabled = !_isScanning;
	        App.ShowToast("Done scanning.");
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
                            await DisplayAlert("App needs location info",
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

        private async Task<bool> ConnectToDevice(IDevice bleDevice)
        {
            bool connected = false;

            try
            {
                await _adapter.ConnectToDeviceAsync(bleDevice);
                //List<IService> services = (await bleDevice.GetServicesAsync())?.ToList() ?? new List<IService>();
                //foreach (IService service in services)
                //{
                //    Debug.WriteLine(
                //        $"Service name: {service.Name} - ID: {service.Id} - Is Primary? {service.IsPrimary}");

                //    foreach (ICharacteristic chx in (await service.GetCharacteristicsAsync() ??
                //                                     new List<ICharacteristic>()))
                //    {
                //        Debug.WriteLine(
                //            $"Characteristic name: {chx.Name} - ID: {chx.Id} - Properties: {chx.Properties} - String value: {chx.StringValue} - Byte value: {GetByteString(chx.Value)}");
                //        if (chx.Properties.HasFlag(CharacteristicPropertyType.Read))
                //        {
                //            Debug.WriteLine($"Characteristic data: {GetByteString(await chx.ReadAsync())}");
                //        }

                //        foreach (IDescriptor dsc in (await chx.GetDescriptorsAsync() ?? new List<IDescriptor>()))
                //        {
                //            Debug.WriteLine(
                //                $"Descriptor name: {dsc.Name} - ID: {dsc.Id} - Byte value: {GetByteString(dsc.Value)}");
                //            Debug.WriteLine($"Descriptor data: {GetByteString(await dsc.ReadAsync())}");
                //        }
                //    }
                //}

                _bleDevice = bleDevice;
                connected = true;
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

            return connected;
        }
    }
}
