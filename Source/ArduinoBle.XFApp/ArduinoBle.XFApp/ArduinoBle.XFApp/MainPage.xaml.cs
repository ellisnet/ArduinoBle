/* Copyright 2018 Ellisnet - Jeremy Ellis (jeremy@ellisnet.com)
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
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
	    public static readonly Guid BleUartServiceId = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
	    public static readonly Guid BleTxCharacteristicId = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
	    public static readonly Guid BleRxCharacteristicId = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");

	    private static readonly int NotificationMilliseconds = 300;

        private bool _isScanning;
	    private readonly IAdapter _adapter = CrossBluetoothLE.Current.Adapter;
	    private IDevice _bleDevice;
	    private bool? _hasLocationPermission;
	    private ICharacteristic _rxCharacteristic;

        public MainPage()
		{
			InitializeComponent();
		}

	    private async void UiScanButton_Clicked(object sender, EventArgs e)
	    {
	        bool connected = false;

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
                    connected = await scanCompletionSource.Task;
                    if (_bleDevice == null)
                    {
                        App.ShowToast("Status: No device found.");
                        await Task.Delay(5000);
                    }
                }
            };

            _isScanning = false;
	        if (connected)
	        {
	            UiScanButton.IsVisible = false;
            }
	        else
	        {
	            UiScanButton.IsEnabled = !_isScanning;
	            App.ShowToast("Done scanning.");
            }        	        
        }

        private string GetByteString(byte[] bytes) => (bytes?.Any() ?? false) ? BitConverter.ToString(bytes) : "empty";

	    private void ShowReceived(string message)
	    {
	        if (!String.IsNullOrWhiteSpace(message))
	        {
	            var toastConfig = new ToastConfig(message);
	            toastConfig.SetPosition(ToastPosition.Bottom);
	            toastConfig.SetDuration(NotificationMilliseconds);
	            toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(12, 131, 193));

	            Debug.WriteLine(message);

	            UserDialogs.Instance.Toast(toastConfig);
            }
	    }

	    private string TranslateReceived(string message)
	    {
	        string result = "";

	        message = (message ?? "").Trim().ToUpper();

	        if (message.Length == 2)
	        {
	            switch (message[0])
	            {
                    case 'B':
                        switch (message[1])
                        {
                            case 'G':
                                result = "GREEN button pressed.";
                                break;
                            case 'B':
                                result = "BLUE button pressed.";
                                break;
                            case 'R':
                                result = "RED button pressed.";
                                break;
                            case 'Y':
                                result = "YELLOW button pressed.";
                                break;
                            default:
                                break;
                        }
                        break;
	                case 'K':
	                    switch (message[1])
	                    {
	                        case 'S':
	                            result = "* (star) key pressed.";
	                            break;
                            case 'H':
                                result = "# (hash) key pressed.";
                                break;
	                        case '0':
	                            result = "0 key pressed.";
	                            break;
                            case '1':
	                            result = "1 key pressed.";
	                            break;
	                        case '2':
	                            result = "2 key pressed.";
	                            break;
	                        case '3':
	                            result = "3 key pressed.";
	                            break;
	                        case '4':
	                            result = "4 key pressed.";
	                            break;
	                        case '5':
	                            result = "5 key pressed.";
	                            break;
	                        case '6':
	                            result = "6 key pressed.";
	                            break;
	                        case '7':
	                            result = "7 key pressed.";
	                            break;
	                        case '8':
	                            result = "8 key pressed.";
	                            break;
	                        case '9':
	                            result = "9 key pressed.";
	                            break;
                            default:
	                            break;
	                    }
	                    break;
	                case 'J':
	                    switch (message[1])
	                    {
	                        case 'C':
	                            result = "Joystick CENTERED.";
	                            break;
	                        case 'N':
	                            result = "Joystick NORTH.";
	                            break;
	                        case 'O':
	                            result = "Joystick NORTHEAST.";
	                            break;
	                        case 'E':
	                            result = "Joystick EAST.";
	                            break;
	                        case 'F':
	                            result = "Joystick SOUTHEAST.";
	                            break;
	                        case 'S':
	                            result = "Joystick SOUTH.";
	                            break;
	                        case 'T':
	                            result = "Joystick SOUTHWEST.";
	                            break;
	                        case 'W':
	                            result = "Joystick WEST.";
	                            break;
	                        case 'X':
	                            result = "Joystick NORTHWEST.";
	                            break;
	                        default:
	                            break;
	                    }
	                    break;
                    default:
                        break;
	            }
	        }

	        return result;
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
                    byte[] receivedBytes = charUpdatedEventArgs?.Characteristic?.Value;
                    if (receivedBytes != null && receivedBytes.Length > 0)
                    {
                        Debug.WriteLine($"Received bytes: {GetByteString(receivedBytes)}");
                        ShowReceived(TranslateReceived(Encoding.ASCII.GetString(receivedBytes)));
                    }
                };

                await _rxCharacteristic.StartUpdatesAsync();

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
