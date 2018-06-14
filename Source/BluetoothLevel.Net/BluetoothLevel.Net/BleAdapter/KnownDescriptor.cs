//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

using System;

namespace BluetoothLevel.Net.BleAdapter
{
    public struct KnownDescriptor
    {
        public string Name { get; }
        public Guid Id { get; }

        public KnownDescriptor(string name, Guid id)
        {
            Name = name;
            Id = id;
        }
    }
}