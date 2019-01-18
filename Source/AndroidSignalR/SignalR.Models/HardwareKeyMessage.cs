using System;
using System.Linq;

namespace SignalR.Models
{
    public enum HardwareKeyStroke
    {
        Unknown = 0,

        KeyDot,
        KeyDash,
        Key0,
        Key1,
        Key2,
        Key3,
        Key4,
        Key5,
        Key6,
        Key7,
        Key8,
        Key9,

        KeyPower,
        KeyMenu,
        KeyUp,
        KeyLeft,
        KeyEnter,
        KeyRight,
        KeyDown,
        KeyShift,
        KeyBackspace,
    }

    public class HardwareKeyMessage
    {
        private static HardwareKeyStroke[] EntryStrokes { get; } =
        {
            HardwareKeyStroke.KeyDot,
            HardwareKeyStroke.KeyDash,
            HardwareKeyStroke.Key0,
            HardwareKeyStroke.Key1,
            HardwareKeyStroke.Key2,
            HardwareKeyStroke.Key3,
            HardwareKeyStroke.Key4,
            HardwareKeyStroke.Key5,
            HardwareKeyStroke.Key6,
            HardwareKeyStroke.Key7,
            HardwareKeyStroke.Key8,
            HardwareKeyStroke.Key9,
        };

        public HardwareKeyStroke Stroke { get; }

        public bool IsEntryStroke => EntryStrokes.Contains(Stroke);

        public string GetEntryCharacter()
        {
            string result = String.Empty;

            switch (Stroke)
            {
                case HardwareKeyStroke.KeyDot:
                    result = ".";
                    break;
                case HardwareKeyStroke.KeyDash:
                    result = "-";
                    break;
                case HardwareKeyStroke.Key0:
                    result = "0";
                    break;
                case HardwareKeyStroke.Key1:
                    result = "1";
                    break;
                case HardwareKeyStroke.Key2:
                    result = "2";
                    break;
                case HardwareKeyStroke.Key3:
                    result = "3";
                    break;
                case HardwareKeyStroke.Key4:
                    result = "4";
                    break;
                case HardwareKeyStroke.Key5:
                    result = "5";
                    break;
                case HardwareKeyStroke.Key6:
                    result = "6";
                    break;
                case HardwareKeyStroke.Key7:
                    result = "7";
                    break;
                case HardwareKeyStroke.Key8:
                    result = "8";
                    break;
                case HardwareKeyStroke.Key9:
                    result = "9";
                    break;
                default:
                    break;
            }

            return result;
        }

        public HardwareKeyMessage(HardwareKeyStroke stroke)
        {
            Stroke = stroke;
        }
    }
}
