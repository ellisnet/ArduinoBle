namespace KeyboardLed.Models
{
    public enum KeyMessage
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

    public enum LedMessage
    {
        None,

        LeftRed,
        LeftGreen,
        LeftBlue,
        LeftYellow,
        LeftOff,

        RightRed,
        RightGreen,
        RightBlue,
        RightYellow,
        RightOff,
    }
}
