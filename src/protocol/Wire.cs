using System.Buffers.Binary;

namespace PhoneController.Protocol;

public static class Wire
{
    public const int SnapshotLength = 36;
    public const int RumbleLength = 10;
    public const byte Version = 1;

    public static byte[] Encode(GamepadSnapshot s)
    {
        var buf = new byte[SnapshotLength];
        Encode(s, buf);
        return buf;
    }

    public static void Encode(GamepadSnapshot s, Span<byte> dest)
    {
        if (dest.Length < SnapshotLength) throw new ArgumentException($"need {SnapshotLength} bytes", nameof(dest));
        dest[0] = (byte)'K';
        dest[1] = (byte)'P';
        dest[2] = Version;
        dest[3] = s.GyroValid ? (byte)1 : (byte)0;
        BinaryPrimitives.WriteUInt32LittleEndian(dest[4..], s.Sequence);
        BinaryPrimitives.WriteUInt64LittleEndian(dest[8..], s.TimestampUs);
        BinaryPrimitives.WriteUInt16LittleEndian(dest[16..], (ushort)s.Buttons);
        dest[18] = s.LeftTrigger;
        dest[19] = s.RightTrigger;
        BinaryPrimitives.WriteInt16LittleEndian(dest[20..], s.LeftStickX);
        BinaryPrimitives.WriteInt16LittleEndian(dest[22..], s.LeftStickY);
        BinaryPrimitives.WriteInt16LittleEndian(dest[24..], s.RightStickX);
        BinaryPrimitives.WriteInt16LittleEndian(dest[26..], s.RightStickY);
        BinaryPrimitives.WriteInt16LittleEndian(dest[28..], s.GyroX);
        BinaryPrimitives.WriteInt16LittleEndian(dest[30..], s.GyroY);
        BinaryPrimitives.WriteInt16LittleEndian(dest[32..], s.GyroZ);
        dest[34] = s.BatteryPercent;
        dest[35] = 0;
    }

    public static bool TryDecode(ReadOnlySpan<byte> src, out GamepadSnapshot snapshot)
    {
        snapshot = default;
        if (src.Length < SnapshotLength) return false;
        if (src[0] != (byte)'K' || src[1] != (byte)'P' || src[2] != Version) return false;
        snapshot = new GamepadSnapshot(
            Sequence: BinaryPrimitives.ReadUInt32LittleEndian(src[4..]),
            TimestampUs: BinaryPrimitives.ReadUInt64LittleEndian(src[8..]),
            Buttons: (Buttons)BinaryPrimitives.ReadUInt16LittleEndian(src[16..]),
            LeftTrigger: src[18],
            RightTrigger: src[19],
            LeftStickX: BinaryPrimitives.ReadInt16LittleEndian(src[20..]),
            LeftStickY: BinaryPrimitives.ReadInt16LittleEndian(src[22..]),
            RightStickX: BinaryPrimitives.ReadInt16LittleEndian(src[24..]),
            RightStickY: BinaryPrimitives.ReadInt16LittleEndian(src[26..]),
            GyroX: BinaryPrimitives.ReadInt16LittleEndian(src[28..]),
            GyroY: BinaryPrimitives.ReadInt16LittleEndian(src[30..]),
            GyroZ: BinaryPrimitives.ReadInt16LittleEndian(src[32..]),
            BatteryPercent: src[34],
            GyroValid: src[3] == 1);
        return true;
    }

    public static byte[] Encode(RumbleFrame r)
    {
        return
        [
            (byte)'K', (byte)'R', Version, 0,
            r.LargeMotor, r.SmallMotor, r.LedR, r.LedG, r.LedB, 0,
        ];
    }

    public static bool TryDecodeRumble(ReadOnlySpan<byte> src, out RumbleFrame frame)
    {
        frame = default;
        if (src.Length < RumbleLength) return false;
        if (src[0] != (byte)'K' || src[1] != (byte)'R' || src[2] != Version) return false;
        frame = new RumbleFrame(src[4], src[5], src[6], src[7], src[8]);
        return true;
    }
}
