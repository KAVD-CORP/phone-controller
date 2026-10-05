# Phone Controller — Technology stack

Owner: Bubu (CTO). Status: **Adopted** (5 Oct 2026). Changes go through an ADR in `docs/adr/`.

Source of the product vision: `docs/reference/Phone_Controller_Feasibility.docx` (4 Oct 2026). This file is the decision. The feasibility doc is the brief.

This repository is **public**. Contributors will not all be KAVD. The stack has to be something a stranger can build on a weekend, and it has to survive the actual constraints: 250 Hz touch, haptics that differ by phone, and a Windows virtual gamepad whose old driver is retired.

## 1. Decision in one paragraph

**Flutter (Dart) on the phone. .NET 9 (C#) on the Windows host. A 36-byte UDP snapshot between them, specified in `docs/PROTOCOL.md` and implemented twice, checked by tests.** Not one language for both sides. The phone problem is custom UI, haptics and motion. The PC problem is a virtual USB gamepad. Forcing one runtime makes one of those worse, and the wire format is small enough that two encoders plus a golden test are cheaper than a shared runtime.

## 2. Summary

| Layer | Choice | Why |
|---|---|---|
| Phone | **Flutter stable, Dart 3** | One UI for Android and iOS. The product is five custom layouts, not a form app. Flutter draws that directly. The open-source reference in the brief (Virtual Gamepad) is already Flutter + C#. |
| Phone sensors / haptics | Flutter sensors plugin + **platform channels** for Android `VibrationEffect` primitives and iOS Core Haptics | The stock vibration plugin is too coarse for detents. Primitives are device-dependent; every effect needs a simple-waveform fallback (brief §6). |
| Volume keys | Android platform channel only, Phase 3 | The brief says interception is reliable on Android in the foreground and limited on iOS. Do not block the iOS layout on it. |
| Host | **.NET 9 console, later a tray app** | Windows-only is correct (XInput / DualShock / Steam Input). C# is the language with a maintained example against the virtual-pad library. |
| Virtual pad | **VIIPER first** ([Alia5/VIIPER](https://github.com/Alia5/VIIPER)), **ViGEmBus behind an interface as fallback** | ViGEmBus is archived (Nefarius EOL, 2023). It still works and games know it, so we may speak it, but a new product must not depend on it alone (brief §3.2, §9). VIIPER emulates Xbox 360 and DualShock 4 from userspace over a TCP API and a signed `usbip-win2` driver. We do not write a kernel driver. |
| Gyro path | **DualShock 4 output when gyro is on; Xbox 360 when it is off** | Motion data rides DS4 emulation. Xbox output covers more games that ignore gyro. |
| Transport | **UDP state snapshots, 5 GHz Wi-Fi default; USB tethering as the low-jitter option** | A lost packet must cost nothing. Events would stick a button down. JSON on the wire is too much allocation at 250 Hz. |
| Protocol source of truth | `docs/PROTOCOL.md` + `src/protocol` (C#) tests | The Dart encoder (Phase 0) must round-trip the same bytes. No protobuf. |
| Bluetooth HID | **Spike only** (ADR-0004) | Would remove the PC driver on Android 9+. Unverified latency. iOS almost certainly cannot be a HID gamepad. Not the architecture. |
| Tests | **xUnit** now; Flutter widget + integration tests once `apps/phone` exists; a latency log in the host | The first bug will be “it feels late”, so the host must print sequence gaps and age of the last snapshot. |
| CI | GitHub Actions, `dotnet test` | Public repo. Flutter CI is added in the same PR that adds the app. |
| License | **MIT** | Drivers we call (VIIPER, usbip-win2, ViGEm) keep their own licenses. We do not vendor them. |

## 3. Options we weighed

| Option | What you gain | Why it is not the start |
|---|---|---|
| **One language: Flutter on the phone and Flutter on Windows** | One repo language. | The Windows side still has to call VIIPER or ViGEm through FFI. You pay for a desktop UI toolkit you do not need (the host should be a tray icon). |
| **One language: .NET MAUI phone + C# host** | Shared C# types, no second encoder. | MAUI is a weak place to invent five full-screen gamepad layouts and Android haptic primitives. Fewer open-source contributors than Flutter. iOS volume keys are still a platform channel. |
| **React Native / Expo** | KAVD already ships React. | The JS bridge is the wrong place for a 250 Hz sampler and haptic timelines. This is not a CRUD app. |
| **Kotlin + Swift, two native apps** | Best volume-key and Core Haptics APIs. | Two UIs for a four-person team and for outside contributors. Native code comes later, as platform channels, only where Flutter cannot see the actuator or the volume rocker. |
| **Kotlin Multiplatform + Compose** | Shared logic, native-feeling UI. | Compose Multiplatform is less boring for contributors than Flutter, and the Windows host is still C#. Three runtimes. |
| **Rust host + Flutter phone** | A small, fast host. | VIIPER’s documented embeddings are C and C#. A Rust client can wait until the C# host is boring. |
| **Go** | — | No phone UI story, no virtual-gamepad story. |
| **Browser page (WebRTC), like VGamepadWeb** | No install on the phone. | No real haptic primitives, no volume keys, no 250 Hz gyro you control. The brief’s differentiator is haptics. A browser throws that away. |
| **ViGEmBus only** | Most shipped phone-pads use it. | Archived. A public project that starts there will be stuck when the driver finally fails on a new Windows build. |
| **Our own kernel driver** | Full control. | Signing, HVCI, and review. Explicitly out. VIIPER already paid that cost. |

## 4. What “shared” actually means

The thing that must be shared is the **packet**, not the language.

```
phone (Dart)  --UDP snapshot-->  host (C#)  --VIIPER-->  game
phone (Dart)  <--UDP rumble--    host (C#)  <--rumble--  game
```

Interface clicks (detents, edge ticks) are played **on the phone immediately**, with no round trip (brief §4). Only game rumble crosses the network.

## 5. Sampling, stated honestly

Flutter’s widget layer is not a 250 Hz sensor. The rule is:

- Pointer events update the latest touch state as they arrive.
- A timer emits the **latest** snapshot at 250 Hz. It does not queue events.
- Gyro is sampled on that same tick and marked invalid when the touch-gate is up.

If a real device cannot hold that rate, the fix is a small Android/iOS sampler behind a platform channel, not a rewrite of the UI. That decision is deferred until Phase 1 measures it.

## 6. What we are not building in v1

- A game running on the phone screen (the game is on the PC; occlusion is a known constraint).
- True trigger springs (not possible; brief §8).
- Per-game profiles. Steam Input and a generic Xbox/DS4 device are the compatibility strategy.
- Linux/macOS hosts. Windows is the only OS where this virtual pad matters for the brief.
- Anti-cheat bypass. If a game rejects virtual devices, we document it. We do not hide the device.

## 7. Team fit

Nissi can own Flutter layouts. Allan can own the host and VIIPER. Bubu owns latency measurements and the phase gate. Virgil owns the on-glass layout, because the product fails or succeeds on where the fingers rest. Outside contributors can change a layout without touching the driver.
