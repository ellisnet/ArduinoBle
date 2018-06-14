//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;

namespace BluetoothLevel.Net.BleAdapter
{
    public class Descriptor : DescriptorBase
    {
        private readonly GattDescriptor _nativeDescriptor;
        /// <summary>
        /// The locally stored value of a descriptor updated after a
        /// notification or a read
        /// </summary>
        private byte[] _value;
        public override Guid Id => _nativeDescriptor.Uuid;
        public override byte[] Value
        {
            get
            {
                if (_value == null)
                {
                    return new byte[0];
                }
                return _value;
            }
        }

        public Descriptor(GattDescriptor nativeDescriptor, ICharacteristic characteristic) : base(characteristic)
        {
            _nativeDescriptor = nativeDescriptor;
        }

        protected async override Task<byte[]> ReadNativeAsync()
        {
            var readResult = await _nativeDescriptor.ReadValueAsync();
            if (readResult.Status == GattCommunicationStatus.Success)
            {
                Trace.Message("Descriptor Read Successfully");
            }
            else
            {
                Trace.Message("Descriptor Read Failed");
            }
            _value = readResult.Value.ToArray();
            return _value;
        }

        protected async override Task WriteNativeAsync(byte[] data)
        {
            //method contains no option for writing with response, so always write
            //without response
            var writeResult = await _nativeDescriptor.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data));
            if (writeResult == GattCommunicationStatus.Success)
            {
                Trace.Message("Descriptor Write Successfully");
            }
            else
            {
                Trace.Message("Descriptor Write Failed");
            }
        }
    }
}
