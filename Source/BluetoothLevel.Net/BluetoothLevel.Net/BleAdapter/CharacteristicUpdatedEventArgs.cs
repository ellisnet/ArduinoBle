//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

namespace BluetoothLevel.Net.BleAdapter
{
    public class CharacteristicUpdatedEventArgs : System.EventArgs
    {
        public ICharacteristic Characteristic { get; set; }

        public CharacteristicUpdatedEventArgs(ICharacteristic characteristic)
        {
            Characteristic = characteristic;
        }
    }
}