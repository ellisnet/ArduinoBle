using System;

namespace BluetoothLevel.Net.Models
{
    public enum MeasurementType
    {
        Intermediate = 0,
        Final = 1,
    }

    public class LevelMeasurement
    {
        public MeasurementType MeasurementType { get; set; } = MeasurementType.Intermediate;
        public int CurrentValue { get; set; }
        public int PreviousValue { get; set; }
        public int ValueDelta => Math.Abs(CurrentValue - PreviousValue);
    }
}
