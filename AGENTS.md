# AGENTS.md — Phone Controller

You are working on **Phone Controller**, a public KAVD Corp project: a phone used as a PC gamepad, where the unsolved problem is triggers and eyes-free feel, not “can we send touch events”. Read this file first.

## Read before coding

| Question | File |
|---|---|
| What we decided and what we rejected | `docs/TECH_STACK.md` |
| How packets, the host and the layouts fit | `docs/ARCHITECTURE.md` |
| Byte layout | `docs/PROTOCOL.md` |
| Why Flutter, why VIIPER, why UDP | `docs/adr/` |
| The original brief (layouts, haptics, risks) | `docs/reference/Phone_Controller_Feasibility.docx` |
| Which prompt to run | `docs/PROMPTS.md` |
| How to build | `CONTRIBUTING.md` |

## Stack (do not substitute)

Flutter (Dart 3) on Android and iOS. .NET 9 on Windows for the host. VIIPER as the virtual pad, ViGEmBus only as a fallback behind one interface. UDP snapshots as specified. xUnit for the protocol. MIT license.

No React Native. No MAUI. No browser controller. No custom kernel driver. No JSON on the wire.

## Non-negotiable rules

1. **A lost packet must not stick a button.** Send full snapshots. On silence (~100 ms) the host releases triggers and buttons and zeroes sticks.
2. **Interface haptics are local.** Detents and edge ticks play on the phone at the moment of touch. They do not wait for rumble from the PC.
3. **Gyro is touch-gated.** It is active only while the designated zone is held. Tilt-trigger mode cannot run at the same time.
4. **Do not depend on ViGEmBus alone.** New virtual-pad code goes through `IVirtualPad`. Default implementation is VIIPER.
5. **Two encoders, one byte layout.** If you change `docs/PROTOCOL.md`, you change `src/protocol`, the xUnit tests, and the Dart encoder in the same PR.
6. **No per-game setup.** The device is a generic Xbox 360 or DualShock 4. Steam Input does the rest.
7. **True force feedback is out of scope.** Haptics imitate a spring. Do not claim otherwise in UI copy.
8. **Anti-cheat:** if a game blocks virtual devices, document it. Do not hide or disguise the device.

## Conventions

- Protocol changes are a version bump, not a quiet extra byte.
- Host code lives in `apps/host`. Phone code lives in `apps/phone` once Flutter is created.
- Conventional Commits. One phase per PR.
- Before finishing: `dotnet test`. Once the Flutter app exists, also `flutter analyze` and `flutter test`.

## When unsure

ADRs beat the feasibility doc. The feasibility doc beats a guess about ergonomics. If a change needs a new driver or a new wire format, stop and write an ADR instead of coding it.
