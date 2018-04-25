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
	    private static readonly Color DefaultButtonColor = Color.Silver;
	    private static readonly int ButtonSelectMilliseconds = 300;

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
	            UiScanButtonContainer.IsVisible = false;
	            UiButtonsContainer.IsVisible = true;
                ResetButtonColors(DefaultButtonColor, true);
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

	    private void ResetSelectColors(Color color)
	    {
	        UiButtonUp.BackgroundColor = color;
	        UiButtonLeft.BackgroundColor = color;
	        UiButtonRight.BackgroundColor = color;
	        UiButtonDown.BackgroundColor = color;
        }

	    private void ResetButtonColors(Color color, bool withSelect)
	    {
	        UiButtonGreen.BackgroundColor = color;
	        UiButtonBlue.BackgroundColor = color;
	        UiButtonRed.BackgroundColor = color;
	        UiButtonYellow.BackgroundColor = color;

	        UiButtonCenter.BackgroundColor = color;
	        if (withSelect)
	        {
	            ResetSelectColors(color);
	        }

	        UiKey0.BackgroundColor = color;
	        UiKey1.BackgroundColor = color;
	        UiKey2.BackgroundColor = color;
	        UiKey3.BackgroundColor = color;
	        UiKey4.BackgroundColor = color;
	        UiKey5.BackgroundColor = color;
	        UiKey6.BackgroundColor = color;
	        UiKey7.BackgroundColor = color;
	        UiKey8.BackgroundColor = color;
	        UiKey9.BackgroundColor = color;
	        UiKeyStar.BackgroundColor = color;
	        UiKeyHash.BackgroundColor = color;
        }

        private string TranslateReceived(string message)
	    {
	        string result = "";

	        message = (message ?? "").Trim().ToUpper();

	        if (message.Length == 2)
	        {
                //ResetButtonColors(DefaultButtonColor, false);

	            switch (message[0])
	            {
                    case 'B':
                        switch (message[1])
                        {
                            case 'G':
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UiButtonGreen.BackgroundColor = Color.Green;
                                });                               
                                result = "GREEN button pressed.";
                                break;
                            case 'B':
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UiButtonBlue.BackgroundColor = Color.Blue;
                                });
                                result = "BLUE button pressed.";
                                break;
                            case 'R':
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UiButtonRed.BackgroundColor = Color.Red;
                                });
                                result = "RED button pressed.";
                                break;
                            case 'Y':
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UiButtonYellow.BackgroundColor = Color.Yellow;
                                });
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
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKeyStar.BackgroundColor = Color.Blue;
                                });
                                result = "* (star) key pressed.";
	                            break;
                            case 'H':
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UiKeyHash.BackgroundColor = Color.Blue;
                                });
                                result = "# (hash) key pressed.";
                                break;
	                        case '0':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey0.BackgroundColor = Color.Blue;
	                            });
                                result = "0 key pressed.";
	                            break;
                            case '1':
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UiKey1.BackgroundColor = Color.Blue;
                                });
                                result = "1 key pressed.";
	                            break;
	                        case '2':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey2.BackgroundColor = Color.Blue;
	                            });
                                result = "2 key pressed.";
	                            break;
	                        case '3':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey3.BackgroundColor = Color.Blue;
	                            });
                                result = "3 key pressed.";
	                            break;
	                        case '4':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey4.BackgroundColor = Color.Blue;
	                            });
                                result = "4 key pressed.";
	                            break;
	                        case '5':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey5.BackgroundColor = Color.Blue;
	                            });
                                result = "5 key pressed.";
	                            break;
	                        case '6':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey6.BackgroundColor = Color.Blue;
	                            });
                                result = "6 key pressed.";
	                            break;
	                        case '7':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey7.BackgroundColor = Color.Blue;
	                            });
                                result = "7 key pressed.";
	                            break;
	                        case '8':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey8.BackgroundColor = Color.Blue;
	                            });
                                result = "8 key pressed.";
	                            break;
	                        case '9':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiKey9.BackgroundColor = Color.Blue;
	                            });
                                result = "9 key pressed.";
	                            break;
                            default:
	                            break;
	                    }
	                    break;
	                case 'J':
	                    switch (message[1])
	                    {
	                        case 'B':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                UiButtonCenter.BackgroundColor = Color.Green;
	                            });
                                result = "Joystick BUTTON pressed.";
	                            break;
                            case 'C':
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    ResetSelectColors(DefaultButtonColor);
                                });
                                result = "Joystick CENTERED.";
	                            break;
	                        case 'N':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                ResetSelectColors(DefaultButtonColor);
	                                UiButtonUp.BackgroundColor = Color.Blue;
                                });
                                result = "Joystick NORTH.";
	                            break;
	                        case 'O':
	                            result = "Joystick NORTHEAST.";
	                            break;
	                        case 'E':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                ResetSelectColors(DefaultButtonColor);
	                                UiButtonRight.BackgroundColor = Color.Blue;
	                            });
                                result = "Joystick EAST.";
	                            break;
	                        case 'F':
	                            result = "Joystick SOUTHEAST.";
	                            break;
	                        case 'S':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                ResetSelectColors(DefaultButtonColor);
	                                UiButtonDown.BackgroundColor = Color.Blue;
	                            });
                                result = "Joystick SOUTH.";
	                            break;
	                        case 'T':
	                            result = "Joystick SOUTHWEST.";
	                            break;
	                        case 'W':
	                            Device.BeginInvokeOnMainThread(() =>
	                            {
	                                ResetSelectColors(DefaultButtonColor);
	                                UiButtonLeft.BackgroundColor = Color.Blue;
	                            });
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

	            Task.Run(async () =>
	            {
	                await Task.Delay(ButtonSelectMilliseconds);
	                Device.BeginInvokeOnMainThread(() =>
	                {
	                    ResetButtonColors(DefaultButtonColor, false);
                    });                    
	            });
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
                        string translated = TranslateReceived(Encoding.ASCII.GetString(receivedBytes));
                        Debug.WriteLine(translated);
                        //ShowReceived(translated);
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
