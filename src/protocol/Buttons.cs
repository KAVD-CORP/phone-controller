namespace PhoneController.Protocol;

/// <summary>Xbox-style face layout. Bit positions are part of the wire format (docs/PROTOCOL.md).</summary>
[Flags]
public enum Buttons : ushort
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    X = 1 << 2,
    Y = 1 << 3,
    LeftBumper = 1 << 4,
    RightBumper = 1 << 5,
    LeftStick = 1 << 6,
    RightStick = 1 << 7,
    Start = 1 << 8,
    Back = 1 << 9,
    DpadUp = 1 << 10,
    DpadDown = 1 << 11,
    DpadLeft = 1 << 12,
    DpadRight = 1 << 13,
}
