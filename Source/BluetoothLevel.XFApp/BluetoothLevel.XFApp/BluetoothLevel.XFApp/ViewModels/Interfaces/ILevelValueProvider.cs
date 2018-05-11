using System;
using SkiaSharp;

namespace BluetoothLevel.XFApp.ViewModels.Interfaces
{
    public interface ILevelValueProvider
    {
        Action<int, SKColor> IndicatorUpdateAction { get; set; }
    }
}
