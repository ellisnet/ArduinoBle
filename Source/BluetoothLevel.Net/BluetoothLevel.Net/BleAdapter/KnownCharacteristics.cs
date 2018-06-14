﻿//Code from here:
// https://github.com/xabre/xamarin-bluetooth-le/tree/uwp_creators_update/Source

using System;
using System.Collections.Generic;
using System.Linq;

namespace BluetoothLevel.Net.BleAdapter
{
    public static class KnownCharacteristics
    {
        static KnownCharacteristics()
        {
            //ToDo do we need a lock here?
            LookupTable = Characteristics.ToDictionary(c => c.Id, c => c);
        }

        public static KnownCharacteristic Lookup(Guid id)
        {
            return LookupTable.ContainsKey(id) ? LookupTable[id] : new KnownCharacteristic("Unknown characteristic", Guid.Empty);
        }

        private static readonly Dictionary<Guid, KnownCharacteristic> LookupTable;

        /// <summary>
        /// https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicsHome.aspx
        /// </summary>
        private static readonly List<KnownCharacteristic> Characteristics = new List<KnownCharacteristic>()
        {
            new KnownCharacteristic("Alert Category ID", Guid.ParseExact("00002a43-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Alert Category ID Bit Mask", Guid.ParseExact("00002a42-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Alert Level", Guid.ParseExact("00002a06-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Alert Notification Control Point", Guid.ParseExact("00002a44-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Alert Status", Guid.ParseExact("00002a3f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Appearance", Guid.ParseExact("00002a01-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Battery Level", Guid.ParseExact("00002a19-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Blood Pressure Feature", Guid.ParseExact("00002a49-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Blood Pressure Measurement", Guid.ParseExact("00002a35-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Body Sensor Location", Guid.ParseExact("00002a38-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Boot Keyboard Input Report", Guid.ParseExact("00002a22-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Boot Keyboard Output Report", Guid.ParseExact("00002a32-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Boot Mouse Input Report", Guid.ParseExact("00002a33-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("CSC Feature", Guid.ParseExact("00002a5c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("CSC Measurement", Guid.ParseExact("00002a5b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Current Time", Guid.ParseExact("00002a2b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Cycling Power Control Point", Guid.ParseExact("00002a66-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Cycling Power Feature", Guid.ParseExact("00002a65-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Cycling Power Measurement", Guid.ParseExact("00002a63-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Cycling Power Vector", Guid.ParseExact("00002a64-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Date Time", Guid.ParseExact("00002a08-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Day Date Time", Guid.ParseExact("00002a0a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Day of Week", Guid.ParseExact("00002a09-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Device Name", Guid.ParseExact("00002a00-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("DST Offset", Guid.ParseExact("00002a0d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Exact Time 256", Guid.ParseExact("00002a0c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Firmware Revision String", Guid.ParseExact("00002a26-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Glucose Feature", Guid.ParseExact("00002a51-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Glucose Measurement", Guid.ParseExact("00002a18-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Glucose Measure Context", Guid.ParseExact("00002a34-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Hardware Revision String", Guid.ParseExact("00002a27-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Heart Rate Control Point", Guid.ParseExact("00002a39-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Heart Rate Measurement", Guid.ParseExact("00002a37-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("HID Control Point", Guid.ParseExact("00002a4c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("HID Information", Guid.ParseExact("00002a4a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("IEEE 11073-20601 Regulatory Certification Data List", Guid.ParseExact("00002a2a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Intermediate Cuff Pressure", Guid.ParseExact("00002a36-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Intermediate Temperature", Guid.ParseExact("00002a1e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("LN Control Point", Guid.ParseExact("00002a6b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("LN Feature", Guid.ParseExact("00002a6a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Local Time Information", Guid.ParseExact("00002a0f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Location And Speed", Guid.ParseExact("00002a67-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Manufacturer Name String", Guid.ParseExact("00002a29-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Measurement Interval", Guid.ParseExact("00002a21-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Model Number String", Guid.ParseExact("00002a24-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Navigation", Guid.ParseExact("00002a68-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("New Alert", Guid.ParseExact("00002a46-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Peripheral Preferred Connection Parameters", Guid.ParseExact("00002a04-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Peripheral Privacy Flag", Guid.ParseExact("00002a02-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("PnP ID", Guid.ParseExact("00002a50-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Position Quality", Guid.ParseExact("00002a69-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Protocol Mode", Guid.ParseExact("00002a4e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Reconnection Address", Guid.ParseExact("00002a03-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Record Access Control Point (Test Version)", Guid.ParseExact("00002a52-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Reference Time Information", Guid.ParseExact("00002a14-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Report", Guid.ParseExact("00002a4d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Report Map", Guid.ParseExact("00002a4b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Ringer Control Point", Guid.ParseExact("00002a40-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Ringer Setting", Guid.ParseExact("00002a41-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("RSC Feature", Guid.ParseExact("00002a54-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("RSC Measurement", Guid.ParseExact("00002a53-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("SC Control Point", Guid.ParseExact("00002a55-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Scan Interval Window", Guid.ParseExact("00002a4f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Scan Refresh", Guid.ParseExact("00002a31-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Sensor Location", Guid.ParseExact("00002a5d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Serial Number String", Guid.ParseExact("00002a25-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Service Changed", Guid.ParseExact("00002a05-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Software Revision String", Guid.ParseExact("00002a28-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Supported New Alert Category", Guid.ParseExact("00002a47-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Supported Unread Alert Category", Guid.ParseExact("00002a48-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("System ID", Guid.ParseExact("00002a23-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Temperature Measurement", Guid.ParseExact("00002a1c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Temperature Type", Guid.ParseExact("00002a1d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Time Accuracy", Guid.ParseExact("00002a12-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Time Source", Guid.ParseExact("00002a13-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Time Update Control Point", Guid.ParseExact("00002a16-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Time Update State", Guid.ParseExact("00002a17-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Time with DST", Guid.ParseExact("00002a11-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Time Zone", Guid.ParseExact("00002a0e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Tx Power Level", Guid.ParseExact("00002a07-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Unread Alert Status", Guid.ParseExact("00002a45-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Aerobic Heart Rate Lower Limit", Guid.ParseExact("00002a7e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Aerobic Heart Rate Uppoer Limit", Guid.ParseExact("00002a84-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Age", Guid.ParseExact("00002a80-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Aggregate Input", Guid.ParseExact("00002a5a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Anaerobic Heart Rate Lower Limit", Guid.ParseExact("00002a81-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Anaerobic Heart Rate Upper Limit", Guid.ParseExact("00002a82-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Anaerobic Threshold", Guid.ParseExact("00002a83-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Analog Input", Guid.ParseExact("00002a58-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Fat Burn Heart Rate Upper Limit", Guid.ParseExact("00002a89-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Apparent Wind Direction", Guid.ParseExact("00002a73-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Apparent Wind Speed", Guid.ParseExact("00002a72-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Body Composition Feature", Guid.ParseExact("00002a9b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Body Composition Measurement", Guid.ParseExact("00002a9c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Database Change Increment", Guid.ParseExact("00002a99-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Date of Birth", Guid.ParseExact("00002a85-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Date of Threshold Assessment", Guid.ParseExact("00002a86-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Descriptor Value Changed", Guid.ParseExact("00002a7d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Dew Point", Guid.ParseExact("00002a7b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Digital Input", Guid.ParseExact("00002a56-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Digital Output", Guid.ParseExact("00002a57-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Elevation", Guid.ParseExact("00002a6c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Email Address", Guid.ParseExact("00002a87-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Exact Time 100", Guid.ParseExact("00002a0b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Fat Burn Heart Rate Lower Limit", Guid.ParseExact("00002a88-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("First Name", Guid.ParseExact("00002a8a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Five Zone Heart Rate Limits", Guid.ParseExact("00002a8b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Gender", Guid.ParseExact("00002a8c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Gust Factor", Guid.ParseExact("00002a74-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Heart Rate Max", Guid.ParseExact("00002a8d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Heat Index", Guid.ParseExact("00002a7a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Height", Guid.ParseExact("00002a8e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Hip Circumference", Guid.ParseExact("00002a8f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Humidity", Guid.ParseExact("00002a6f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Irradiance", Guid.ParseExact("00002a77-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Last Name", Guid.ParseExact("00002a90-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Maximum Recommended Heart Rate", Guid.ParseExact("00002a91-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Network Availability", Guid.ParseExact("00002a3e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Pollen Concentration", Guid.ParseExact("00002a75-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Pressure", Guid.ParseExact("00002a6d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Rainfall", Guid.ParseExact("00002a78-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Resting Heart Rate", Guid.ParseExact("00002a92-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Scientific Temperature in Celsius", Guid.ParseExact("00002a3c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Secondary Time Zone", Guid.ParseExact("00002a10-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Sport Type for Aerobic and Anaerobic Thresholds", Guid.ParseExact("00002a93-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("String", Guid.ParseExact("00002a3d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Temperature", Guid.ParseExact("00002a6e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Temperature in Celsius", Guid.ParseExact("00002a1f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Temperature in Fahrenheit", Guid.ParseExact("00002a20-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Three Zone Heart Rate Limits", Guid.ParseExact("00002a94-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Time Broadcast", Guid.ParseExact("00002a15-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Trend", Guid.ParseExact("00002a7c-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("True Wind Direction", Guid.ParseExact("00002a71-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("True Wind Speed", Guid.ParseExact("00002a70-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Two Zone Heart Rate Limit", Guid.ParseExact("00002a95-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("User Control Point", Guid.ParseExact("00002a9f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("User Index", Guid.ParseExact("00002a9a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("UV Index", Guid.ParseExact("00002a76-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("VO2 Max", Guid.ParseExact("00002a96-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Waist Circumference", Guid.ParseExact("00002a97-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Weight", Guid.ParseExact("00002a98-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Weight Measurement", Guid.ParseExact("00002a9d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Weight Scale Feature", Guid.ParseExact("00002a9e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Wind Chill", Guid.ParseExact("00002a79-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Battery Level State", Guid.ParseExact("00002a1b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Battery Power State", Guid.ParseExact("00002a1a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Latitude", Guid.ParseExact("00002a2d-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Longitude", Guid.ParseExact("00002a2e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Position 2D", Guid.ParseExact("00002a2f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Position 3D", Guid.ParseExact("00002a30-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Pulse Oximetry Continuous Measurement", Guid.ParseExact("00002a5f-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Pulse Oximetry Control Point", Guid.ParseExact("00002a62-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Pulse Oximetry Features", Guid.ParseExact("00002a61-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Pulse Oximetry Pulsatile Event", Guid.ParseExact("00002a60-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Pulse Oximetry Spot-Check Measurement", Guid.ParseExact("00002a5e-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Removable", Guid.ParseExact("00002a3a-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("Service Required", Guid.ParseExact("00002a3b-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("TI SensorTag Keys Data", Guid.ParseExact("0000ffe1-0000-1000-8000-00805f9b34fb", "d")),
            new KnownCharacteristic("TI SensorTag Infrared Temperature Data", Guid.ParseExact("f000aa01-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Infrared Temperature On/Off", Guid.ParseExact("f000aa02-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Infrared Temperature Sample Rate", Guid.ParseExact("f000aa03-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Accelerometer Data", Guid.ParseExact("f000aa11-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Accelerometer On/Off", Guid.ParseExact("f000aa12-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Accelerometer Sample Rate", Guid.ParseExact("f000aa13-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Humidity Data", Guid.ParseExact("f000aa21-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Humidity On/Off", Guid.ParseExact("f000aa22-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Humidity Sample Rate", Guid.ParseExact("f000aa23-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Magnometer Data", Guid.ParseExact("f000aa31-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Magnometer On/Off", Guid.ParseExact("f000aa32-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Magnometer Sample Rate", Guid.ParseExact("f000aa33-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Barometer Data", Guid.ParseExact("f000aa41-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Barometer On/Off", Guid.ParseExact("f000aa42-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Barometer Calibration", Guid.ParseExact("f000aa43-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Barometer Sample Rate", Guid.ParseExact("f000aa44-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Gyroscope Data", Guid.ParseExact("f000aa51-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Gyroscope On/Off", Guid.ParseExact("f000aa52-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Gyroscope Sample Rate", Guid.ParseExact("f000aa53-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Test Data", Guid.ParseExact("f000aa61-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Test Configuration", Guid.ParseExact("f000aa62-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Connection Parameters", Guid.ParseExact("f000ccc1-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Connection Request Parameters", Guid.ParseExact("f000ccc2-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag Connection Request Disconnect", Guid.ParseExact("f000ccc3-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag OAD Image Identify", Guid.ParseExact("f000ffc1-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("TI SensorTag OAD Image Block", Guid.ParseExact("f000ffc2-0451-4000-b000-000000000000", "d")),
            new KnownCharacteristic("RedBearLabs Biscuit Read", Guid.ParseExact("713d0001-503e-4c75-ba94-3148f18d941e", "d")),
            new KnownCharacteristic("RedBearLabs Biscuit TX_DATA_CHAR_UUID Notify", Guid.ParseExact("713d0002-503e-4c75-ba94-3148f18d941e", "d")),
            new KnownCharacteristic("RedBearLabs Biscuit RX_DATA_CHAR_UUID WriteWithoutResponse", Guid.ParseExact("713d0003-503e-4c75-ba94-3148f18d941e", "d")),
            new KnownCharacteristic("RedBearLabs Biscuit BAUDRATE_CHAR_UUID WriteWithoutResponse", Guid.ParseExact("713d0004-503e-4c75-ba94-3148f18d941e", "d")),
            new KnownCharacteristic("RedBearLabs Biscuit DEV_NAME_CHAR_UUID Read", Guid.ParseExact("713d0005-503e-4c75-ba94-3148f18d941e", "d")),
            new KnownCharacteristic("RedBearLabs Biscuit VERSION_CHAR_UUID unknown", Guid.ParseExact("713d0006-503e-4c75-ba94-3148f18d941e", "d")),
            new KnownCharacteristic("RedBearLabs Biscuit TX_POWER_CHAR_UUID unknown", Guid.ParseExact("713d0007-503e-4c75-ba94-3148f18d941e", "d")),
        };
    }
}