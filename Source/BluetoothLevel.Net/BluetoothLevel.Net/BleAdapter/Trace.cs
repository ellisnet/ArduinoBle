//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

using System;

namespace BluetoothLevel.Net.BleAdapter
{
    public static class Trace
    {
        public static Action<string, object[]> TraceImplementation { get; set; }

        public static void Message(string format, params object[] args)
        {
            try
            {
                TraceImplementation?.Invoke(format, args);
            }
            catch { /* ignore */ }
        }
    }
}