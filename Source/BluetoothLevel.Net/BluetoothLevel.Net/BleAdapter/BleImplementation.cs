//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

using BluetoothLevel.Net.BleHelper;

namespace BluetoothLevel.Net.BleAdapter
{
    public class BleImplementation : BleImplementationBase
    {
        private static IBluetoothLE _instance;
        public static IBluetoothLE Instance => _instance
            ?? (_instance = new BleImplementation());

        private BluetoothLEHelper _bluetoothHelper;

        protected override IAdapter CreateNativeAdapter()
        {
            return new Adapter(_bluetoothHelper);
        }

        protected override BluetoothState GetInitialStateNative()
        {
            //The only way to get the state of bluetooth through windows is by
            //getting the radios for a device. This operation is asynchronous
            //and thus cannot be called in this method. Thus, we are just
            //returning "On" as long as the BluetoothLEHelper is initialized
            if (_bluetoothHelper == null)
            {
                return BluetoothState.Unavailable;
            }
            return BluetoothState.On;
        }

        protected override void InitializeNative()
        {
            //create local helper using the app context
            var localHelper = BluetoothLEHelper.Context;
            _bluetoothHelper = localHelper;
        }

        private BleImplementation()
        {
            
        }
    }
}
