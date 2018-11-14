using System;
using System.Text;

namespace KeyboardLed.Models
{
    public class KeyboardMessage
    {
        public static KeyMessage[] KeyboardMessages =
        {
            KeyMessage.KeyDot,
            KeyMessage.KeyDash,
            KeyMessage.Key0,
            KeyMessage.Key1,
            KeyMessage.Key2,
            KeyMessage.Key3,
            KeyMessage.Key4,
            KeyMessage.Key5,
            KeyMessage.Key6,
            KeyMessage.Key7,
            KeyMessage.Key8,
            KeyMessage.Key9,
        };

        public static KeyMessage[] ButtonMessages =
        {
            KeyMessage.KeyPower,
            KeyMessage.KeyMenu,
            KeyMessage.KeyUp,
            KeyMessage.KeyLeft,
            KeyMessage.KeyEnter,
            KeyMessage.KeyRight,
            KeyMessage.KeyDown,
            KeyMessage.KeyShift,
            KeyMessage.KeyBackspace,
        };

        public KeyMessage Message { get; }

        public KeyboardMessage(KeyMessage bleMessage)
        {
            Message = bleMessage;
        }

        public static KeyboardMessage FromReceivedMessage(string message)
        {
            var received = KeyMessage.Unknown;

            message = (message ?? "").Trim().ToUpper();

            if (message.Length == 2)
            {
                switch (message[0])
                {
                    case 'B':
                        switch (message[1])
                        {
                            case 'P':
                                received = KeyMessage.KeyPower;
                                break;
                            case 'M':
                                received = KeyMessage.KeyMenu;
                                break;
                            case 'U':
                                received = KeyMessage.KeyUp;
                                break;
                            case 'L':
                                received = KeyMessage.KeyLeft;
                                break;
                            case 'E':
                                received = KeyMessage.KeyEnter;
                                break;
                            case 'R':
                                received = KeyMessage.KeyRight;
                                break;
                            case 'D':
                                received = KeyMessage.KeyDown;
                                break;
                            case 'H':
                                received = KeyMessage.KeyShift;
                                break;
                            case 'S':
                                received = KeyMessage.KeyBackspace;
                                break;
                            default:
                                break;
                        }
                        break;

                    case 'K':
                        switch (message[1])
                        {
                            case 'S':
                                received = KeyMessage.KeyDot;
                                break;
                            case 'H':
                                received = KeyMessage.KeyDash;
                                break;
                            case '0':
                                received = KeyMessage.Key0;
                                break;
                            case '1':
                                received = KeyMessage.Key1;
                                break;
                            case '2':
                                received = KeyMessage.Key2;
                                break;
                            case '3':
                                received = KeyMessage.Key3;
                                break;
                            case '4':
                                received = KeyMessage.Key4;
                                break;
                            case '5':
                                received = KeyMessage.Key5;
                                break;
                            case '6':
                                received = KeyMessage.Key6;
                                break;
                            case '7':
                                received = KeyMessage.Key7;
                                break;
                            case '8':
                                received = KeyMessage.Key8;
                                break;
                            case '9':
                                received = KeyMessage.Key9;
                                break;
                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
            return new KeyboardMessage(received);
        }

        public static byte[] GetLedMessageBytes(LedMessage message)
        {
            byte[] result = null;

            switch (message)
            {
                case LedMessage.LeftRed:
                    result = Encoding.ASCII.GetBytes("LR");
                    break;
                case LedMessage.LeftGreen:
                    result = Encoding.ASCII.GetBytes("LG");
                    break;
                case LedMessage.LeftBlue:
                    result = Encoding.ASCII.GetBytes("LB");
                    break;
                case LedMessage.LeftYellow:
                    result = Encoding.ASCII.GetBytes("LY");
                    break;
                case LedMessage.LeftOff:
                    result = Encoding.ASCII.GetBytes("LO");
                    break;

                case LedMessage.RightRed:
                    result = Encoding.ASCII.GetBytes("RR");
                    break;
                case LedMessage.RightGreen:
                    result = Encoding.ASCII.GetBytes("RG");
                    break;
                case LedMessage.RightBlue:
                    result = Encoding.ASCII.GetBytes("RB");
                    break;
                case LedMessage.RightYellow:
                    result = Encoding.ASCII.GetBytes("RY");
                    break;
                case LedMessage.RightOff:
                    result = Encoding.ASCII.GetBytes("RO");
                    break;

                default:
                    break;
            }

            return result;
        }
    }
}
