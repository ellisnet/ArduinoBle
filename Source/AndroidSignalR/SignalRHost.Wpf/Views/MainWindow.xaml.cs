using System;
using System.Windows;
using System.Windows.Media;
using SignalRHost.Wpf.ViewModels;

namespace SignalRHost.Wpf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DataContext is ILedColorAware colorAware)
            {
                colorAware.SetLeftLedAction = color =>
                {
                    switch (color)
                    {
                        case LedColor.None:
                            UiLeftLed.Background = new SolidColorBrush(Color.FromRgb(255,255,255));
                            break;

                        case LedColor.Green:
                            UiLeftLed.Background = new SolidColorBrush(Color.FromRgb(0, 190, 0));
                            break;

                        case LedColor.Amber:
                            UiLeftLed.Background = new SolidColorBrush(Color.FromRgb(255, 190, 0));
                            break;

                        case LedColor.Red:
                            UiLeftLed.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(color), color, null);
                    }
                };

                colorAware.SetRightLedAction = color =>
                {
                    switch (color)
                    {
                        case LedColor.None:
                            UiRightLed.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            break;

                        case LedColor.Green:
                            UiRightLed.Background = new SolidColorBrush(Color.FromRgb(0, 190, 0));
                            break;

                        case LedColor.Amber:
                            UiRightLed.Background = new SolidColorBrush(Color.FromRgb(255, 190, 0));
                            break;

                        case LedColor.Red:
                            UiRightLed.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(color), color, null);
                    }
                };
            }
        }
    }
}
