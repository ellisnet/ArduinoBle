//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

namespace BluetoothLevel.Net.BleAdapter
{
    /// <summary>
    /// Determines the connection state of the device.
    /// </summary>
    public enum DeviceState
    {
        /// <summary>
        /// Device is disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Device is connecting.
        /// </summary>
        Connecting,

        /// <summary>
        /// Device is connected.
        /// </summary>
        Connected,

        /// <summary>
        /// OnAndroid: Device is connected to the system. In order to use this device please call connect it by using the Adapter. 
        /// </summary>
        Limited
    }
}