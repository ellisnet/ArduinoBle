using System;
using SkiaSharp;

namespace BluetoothLevel.XFApp.ViewModels
{
    public interface ILevelValueProvider
    {
        Action<int, SKColor> IndicatorUpdateAction { get; set; }
    }
}
