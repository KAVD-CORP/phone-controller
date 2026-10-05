namespace PhoneController.Protocol;

/// <summary>Host → phone. Interface haptics never wait on this packet.</summary>
public readonly record struct RumbleFrame(byte LargeMotor, byte SmallMotor, byte LedR, byte LedG, byte LedB);
