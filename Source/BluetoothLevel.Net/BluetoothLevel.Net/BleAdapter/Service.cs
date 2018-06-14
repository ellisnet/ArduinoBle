//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BluetoothLevel.Net.BleHelper;

namespace BluetoothLevel.Net.BleAdapter
{
    class Service : ServiceBase
    {
        private readonly GattDeviceService _nativeService;
        private readonly ObservableBluetoothLEDevice _nativeDevice;
        public override Guid Id => _nativeService.Uuid;
        //method to get parent devices to check if primary is obselete
        //return true as a placeholder
        public override bool IsPrimary => true;

        public Service(GattDeviceService service, IDevice device) : base(device)
        {
            _nativeDevice = (ObservableBluetoothLEDevice)device.NativeDevice;
            _nativeService = service;
        }

        protected override async Task<IList<ICharacteristic>> GetCharacteristicsNativeAsync()
        {
            var nativeChars = (await _nativeService.GetCharacteristicsAsync()).Characteristics;
            var charList = new List<ICharacteristic>();
            var nativeCharArray = nativeChars.ToArray();  //TODO: Not getting any results here
            foreach (var nativeChar in nativeCharArray)
            {
                var characteristic = new Characteristic(nativeChar, this);
                Debug.WriteLine($"Characteristic found: {characteristic.Id}");
                charList.Add(characteristic);
            }
            return charList;
        }
    }
}
