using PhoneController.Protocol;
using Xunit;

namespace PhoneController.Protocol.Tests;

public class WireTests
{
    [Fact]
    public void Snapshot_roundtrips_including_analog_triggers_and_gyro_flag()
    {
        var original = new GamepadSnapshot(
            Sequence: 42,
            TimestampUs: 1_700_000_000_000,
            Buttons: Buttons.A | Buttons.RightBumper | Buttons.DpadUp,
            LeftTrigger: 0,
            RightTrigger: 200,
            LeftStickX: -1000,
            LeftStickY: 32767,
            RightStickX: 0,
            RightStickY: -32768,
            GyroX: 12,
            GyroY: -4,
            GyroZ: 90,
            BatteryPercent: 80,
            GyroValid: true);

        var bytes = Wire.Encode(original);
        Assert.Equal(Wire.SnapshotLength, bytes.Length);
        Assert.True(Wire.TryDecode(bytes, out var decoded));
        Assert.Equal(original, decoded);
    }

    [Fact]
    public void A_later_snapshot_replaces_state_without_the_dropped_one()
    {
        var first = Sample(sequence: 1, rightTrigger: 10);
        var third = Sample(sequence: 3, rightTrigger: 255);
        Assert.True(Wire.TryDecode(Wire.Encode(third), out var decoded));
        Assert.Equal(3u, decoded.Sequence);
        Assert.Equal(255, decoded.RightTrigger);
        Assert.NotEqual(first.Sequence, decoded.Sequence);
    }

    [Fact]
    public void Bad_magic_or_short_buffer_is_rejected()
    {
        var bytes = Wire.Encode(Sample(1, 0));
        bytes[0] = (byte)'X';
        Assert.False(Wire.TryDecode(bytes, out _));
        Assert.False(Wire.TryDecode(bytes.AsSpan(0, 10), out _));
    }

    [Fact]
    public void Rumble_roundtrips()
    {
        var frame = new RumbleFrame(10, 200, 1, 2, 3);
        Assert.True(Wire.TryDecodeRumble(Wire.Encode(frame), out var decoded));
        Assert.Equal(frame, decoded);
    }

    private static GamepadSnapshot Sample(uint sequence, byte rightTrigger) => new(
        sequence, 0, Buttons.None, 0, rightTrigger, 0, 0, 0, 0, 0, 0, 0, 100, false);
}
