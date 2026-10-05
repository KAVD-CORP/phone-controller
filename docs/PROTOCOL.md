# Wire protocol v1

Little-endian. One datagram is one snapshot or one rumble frame. There is no length prefix and no JSON.

The C# implementation is `src/protocol/Wire.cs`. Tests in `tests/protocol` are the conformance suite. A Dart encoder that disagrees with those tests is wrong.

## Phone → host (36 bytes)

| Offset | Size | Field |
|---|---|---|
| 0 | 2 | Magic `K` `P` (`0x4B 0x50`) |
| 2 | 1 | Version `1` |
| 3 | 1 | Flags. Bit 0 = gyro valid. Other bits must be 0. |
| 4 | 4 | Sequence, `uint32`. Wraps. Gaps mean loss. |
| 8 | 8 | Timestamp, microseconds, `uint64`, phone monotonic clock. |
| 16 | 2 | Buttons, `uint16`, bits below. |
| 18 | 1 | Left trigger, 0–255. |
| 19 | 1 | Right trigger, 0–255. |
| 20 | 2 | Left stick X, `int16`, −32768…32767. |
| 22 | 2 | Left stick Y, `int16`. Up is positive. |
| 24 | 2 | Right stick X, `int16`. |
| 26 | 2 | Right stick Y, `int16`. |
| 28 | 2 | Gyro X, `int16`, device units. Ignored unless gyro valid. |
| 30 | 2 | Gyro Y, `int16`. |
| 32 | 2 | Gyro Z, `int16`. |
| 34 | 1 | Battery percent, 0–100. |
| 35 | 1 | Reserved, `0`. |

### Button bits

| Bit | Button |
|---|---|
| 0 | A |
| 1 | B |
| 2 | X |
| 3 | Y |
| 4 | Left bumper (L1) |
| 5 | Right bumper (R1) |
| 6 | Left stick click |
| 7 | Right stick click |
| 8 | Start |
| 9 | Back |
| 10 | D-pad up |
| 11 | D-pad down |
| 12 | D-pad left |
| 13 | D-pad right |

L2 and R2 are the trigger bytes, not bits.

## Host → phone (10 bytes)

| Offset | Size | Field |
|---|---|---|
| 0 | 2 | Magic `K` `R` |
| 2 | 1 | Version `1` |
| 3 | 1 | Flags, `0` |
| 4 | 1 | Large motor, 0–255 |
| 5 | 1 | Small motor, 0–255 |
| 6 | 1 | LED red |
| 7 | 1 | LED green |
| 8 | 1 | LED blue |
| 9 | 1 | Reserved, `0` |

## Transport

- UDP. Default port **4510** (phone → host) and **4511** (host → phone).
- The phone learns the host address from a short pairing exchange in Phase 1 (broadcast `KP` discovery). Until that exists, type the host IP.
- Do not use TCP for the snapshot stream. Head-of-line blocking is worse than a dropped frame.

## Versioning

A new field bumps Version and gets a new decoder. Do not append bytes to version 1. Old hosts must ignore unknown versions, not crash.
