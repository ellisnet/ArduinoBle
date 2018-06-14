//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

using System;

namespace BluetoothLevel.Net.BleAdapter
{
    public struct KnownService
    {
        public string Name { get; private set; }
        public Guid Id { get; private set; }

        public KnownService(string name, Guid id)
        {
            Name = name;
            Id = id;
        }
    }
}