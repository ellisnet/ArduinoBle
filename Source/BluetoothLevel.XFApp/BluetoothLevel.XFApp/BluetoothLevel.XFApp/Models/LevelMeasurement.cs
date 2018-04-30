namespace BluetoothLevel.XFApp.Models
{
    public enum MeasurementType
    {
        Intermediate = 0,
        Final = 1,
    }

    public class LevelMeasurement
    {
        public MeasurementType MeasurementType { get; set; } = MeasurementType.Intermediate;
        public int Value { get; set; }
    }
}
