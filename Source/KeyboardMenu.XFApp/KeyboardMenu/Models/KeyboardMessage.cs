using System;
using System.Collections.Generic;
using System.Text;

namespace KeyboardMenu.Models
{
    public class KeyboardMessage
    {
        public static BleMessage[] ButtonMessages =
        {
            BleMessage.ButtonGreen,
            BleMessage.ButtonBlue,
            BleMessage.ButtonRed,
            BleMessage.ButtonYellow,
        };

        public static BleMessage[] KeyboardMessages =
        {
            BleMessage.KeyStar,
            BleMessage.KeyHash,
            BleMessage.Key0,
            BleMessage.Key1,
            BleMessage.Key2,
            BleMessage.Key3,
            BleMessage.Key4,
            BleMessage.Key5,
            BleMessage.Key6,
            BleMessage.Key7,
            BleMessage.Key8,
            BleMessage.Key9,
        };

        public static BleMessage[] JoystickMessages =
        {
            BleMessage.JoystickButton,
            BleMessage.JoystickCentered,
            BleMessage.JoystickNorth,
            BleMessage.JoystickNorthEast,
            BleMessage.JoystickEast,
            BleMessage.JoystickSouthEast,
            BleMessage.JoystickSouth,
            BleMessage.JoystickSouthWest,
            BleMessage.JoystickWest,
            BleMessage.JoystickNorthWest,
        };

        public BleMessage Message { get; }

        public KeyboardMessage(BleMessage bleMessage)
        {
            Message = bleMessage;
        }

        public static KeyboardMessage FromReceivedMessage(string message)
        {
            var received = BleMessage.Unknown;

            message = (message ?? "").Trim().ToUpper();

            if (message.Length == 2)
            {
                switch (message[0])
                {
                    case 'B':
                        switch (message[1])
                        {
                            case 'G':
                                received = BleMessage.ButtonGreen;
                                break;
                            case 'B':
                                received = BleMessage.ButtonBlue;
                                break;
                            case 'R':
                                received = BleMessage.ButtonRed;
                                break;
                            case 'Y':
                                received = BleMessage.ButtonYellow;
                                break;
                            default:
                                break;
                        }

                        break;
                    case 'K':
                        switch (message[1])
                        {
                            case 'S':
                                received = BleMessage.KeyStar;
                                break;
                            case 'H':
                                received = BleMessage.KeyHash;
                                break;
                            case '0':
                                received = BleMessage.Key0;
                                break;
                            case '1':
                                received = BleMessage.Key1;
                                break;
                            case '2':
                                received = BleMessage.Key2;
                                break;
                            case '3':
                                received = BleMessage.Key3;
                                break;
                            case '4':
                                received = BleMessage.Key4;
                                break;
                            case '5':
                                received = BleMessage.Key5;
                                break;
                            case '6':
                                received = BleMessage.Key6;
                                break;
                            case '7':
                                received = BleMessage.Key7;
                                break;
                            case '8':
                                received = BleMessage.Key8;
                                break;
                            case '9':
                                received = BleMessage.Key9;
                                break;
                            default:
                                break;
                        }

                        break;
                    case 'J':
                        switch (message[1])
                        {
                            case 'B':
                                received = BleMessage.JoystickButton;
                                break;
                            case 'C':
                                received = BleMessage.JoystickCentered;
                                break;
                            case 'N':
                                received = BleMessage.JoystickNorth;
                                break;
                            case 'O':
                                received = BleMessage.JoystickNorthEast;
                                break;
                            case 'E':
                                received = BleMessage.JoystickEast;
                                break;
                            case 'F':
                                received = BleMessage.JoystickSouthEast;
                                break;
                            case 'S':
                                received = BleMessage.JoystickSouth;
                                break;
                            case 'T':
                                received = BleMessage.JoystickSouthWest;
                                break;
                            case 'W':
                                received = BleMessage.JoystickWest;
                                break;
                            case 'X':
                                received = BleMessage.JoystickNorthWest;
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
    }
}
