//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

namespace BluetoothLevel.Net.BleAdapter
{
    /// <summary>
    /// Specifies how a value should be written.
    /// </summary>
    public enum CharacteristicWriteType
    {
        /// <summary>
        /// Value should be written with response if supported, else without response.
        /// </summary>
        Default,

        /// <summary>
        /// Value should be written with response.
        /// </summary>
        WithResponse,

        /// <summary>
        /// Value should be written without response.
        /// </summary>
        WithoutResponse
    }
}
