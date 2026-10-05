namespace PhoneController.Protocol;

/// <summary>
/// One full controller state. The phone sends these on a fixed cadence.
/// A missing sequence number is a dropped packet, not a stuck button: the next
/// snapshot replaces state entirely.
/// </summary>
public readonly record struct GamepadSnapshot(
    uint Sequence,
    ulong TimestampUs,
    Buttons Buttons,
    byte LeftTrigger,
    byte RightTrigger,
    short LeftStickX,
    short LeftStickY,
    short RightStickX,
    short RightStickY,
    short GyroX,
    short GyroY,
    short GyroZ,
    byte BatteryPercent,
    bool GyroValid);
