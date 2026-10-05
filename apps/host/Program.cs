using PhoneController.Protocol;

// Phase 1 binds UDP and a virtual pad (docs/adr/0002). This entry point only
// proves the wire format the phone and the host must share.
var snap = new GamepadSnapshot(
    Sequence: 1,
    TimestampUs: 0,
    Buttons: Buttons.A,
    LeftTrigger: 0,
    RightTrigger: 128,
    LeftStickX: 0,
    LeftStickY: 0,
    RightStickX: 0,
    RightStickY: 0,
    GyroX: 0,
    GyroY: 0,
    GyroZ: 0,
    BatteryPercent: 100,
    GyroValid: false);

var bytes = Wire.Encode(snap);
if (!Wire.TryDecode(bytes, out var back) || back != snap)
{
    Console.Error.WriteLine("snapshot round-trip failed");
    return 1;
}

Console.WriteLine($"phone-controller host ok · snapshot {bytes.Length} bytes · VIIPER not wired yet");
return 0;
