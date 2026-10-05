# Architecture

## 1. Path

```
[Flutter phone]
  touch + gyro  ->  latest state  ->  250 Hz UDP snapshot (36 bytes)
  local haptics (detents, edge ticks) are played here, immediately

        |  Wi-Fi 5 GHz, or USB tether
        v

[.NET host, Windows]
  decode snapshot -> dead zones / curves -> IVirtualPad
        |                          ^
        v                          | rumble + LED
   VIIPER (DS4 if gyro, else Xbox 360)
        |
        v
   game / Steam Input

  host also sends a 10-byte rumble frame back to the phone
```

## 2. Repository

```
src/protocol/          C# wire format. The spec is docs/PROTOCOL.md.
tests/protocol/        Round-trip tests. Dart must match these bytes.
apps/host/             Windows process. UDP + virtual pad land in Phase 1.
apps/phone/            Flutter app. Created in Phase 0 (SDK not vendored here).
docs/                  Stack, protocol, ADRs, prompts.
docs/reference/        Feasibility brief (4 Oct 2026).
```

## 3. Virtual pad

`IVirtualPad` in the host, two implementations:

| Impl | When |
|---|---|
| `ViiperPad` | Default. Userspace, Xbox 360 and DualShock 4. Requires usbip-win2 once per machine. |
| `ViGEmPad` | Fallback if VIIPER will not start. Same interface. Do not add a third backend without an ADR. |

DS4 is selected when `GyroValid` is set on any snapshot in the session (or the user forces it). Otherwise Xbox 360.

## 4. Layouts (phone)

All landscape, two hands. Shipped as modes, not separate apps.

| Mode | Phase | Role |
|---|---|---|
| Claw corners | 1 | Default. Digital L1/L2/R1/R2 in the top corners. |
| Slide triggers | 1 | Analog L2/R2 as edge strips. The analog bet. |
| Volume + gyro | 3 | Android volume rocker as one hand’s real clicks. Gyro only while a zone is held. |
| Racing tilt | 3 | Phone is the wheel. Pedals under the thumb. |
| Tilt triggers | 3 | Opt-in only. Mutually exclusive L2/R2. Off while gyro aim is on. |
| Adaptive re-centre | 4 | Zones jump under the thumb on touch-down. The breakthrough, not the MVP. |

## 5. Latency budget

Target for Phase 1, measured on one mid-range Android phone and a Windows laptop on 5 GHz, same room:

- Snapshot age at the host (now − phone timestamp): p50 under 8 ms, p95 under 20 ms on Wi-Fi.
- USB tether should beat Wi-Fi. Record both.
- A gap in `Sequence` is logged, not smoothed.

Local haptic latency is not in this budget. If a detent waits for the PC, it is a bug.

## 6. Failure behaviour

- No packet for 100 ms: host holds the last sticks at zero **and** releases buttons and triggers. Do not hold a trigger because a packet was lost.
- Bad magic or version: drop the datagram.
- Phone backgrounded: send a zero snapshot, then stop. Volume-key capture ends.
