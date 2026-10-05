# Cursor Agent Prompts — Phone Controller

Paste one phase into a fresh Agent chat. Every prompt starts by reading `AGENTS.md`. This repo is public: do not add KAVD-only secrets, and do not vendor driver binaries.

Roles: **phone** = Flutter layouts (Nissi, Bubu secondary), **host** = Windows virtual pad (Allan), **QA** = latency and feel (Bubu).

## Phase 0 — Phone skeleton

```
Read AGENTS.md, docs/TECH_STACK.md, docs/PROTOCOL.md and docs/adr/0001.

Flutter is not in the repo yet. From apps/phone run `flutter create . --project-name phone_controller --org com.kavd.phonecontroller --platforms android,ios` (stable channel).

Then:
1. A landscape-only scaffold with two placeholder layouts: Claw and Slide. No networking yet.
2. A Dart port of src/protocol/Wire.cs that round-trips the exact bytes from tests/protocol (add a Dart test with the same vectors: A+RB+DpadUp, RT=200, sticks and gyro as in WireTests).
3. Do not add Firebase, a backend, accounts, or a web build.

Run flutter analyze && flutter test. Open a PR titled "Phase 0: Flutter skeleton and Dart wire codec".
```

## Phase 1 — Playable path

```
Read docs/ARCHITECTURE.md, docs/PROTOCOL.md, docs/adr/0002 and docs/adr/0003.

Host (apps/host):
1. UDP listener on port 4510. Decode snapshots. Log sequence gaps and snapshot age.
2. IVirtualPad with ViiperPad (default) and ViGEmPad (fallback). Xbox 360 when gyro is invalid, DualShock 4 when gyro is valid.
3. If no packet arrives for 100 ms, release all buttons and triggers and zero the sticks.
4. Pairing: phone can type the host IP. Broadcast discovery can wait.

Phone:
5. Claw layout (digital corners) and Slide layout (analog edge strips, L1/R1 still corners). Sticks are floating virtual joysticks.
6. Emit Wire snapshots at 250 Hz from the latest touch state, not a queue of events.
7. Local haptic tick on press. No network in that path.

Exit: joy.cpl shows the pad moving from the phone, in one test game. Record p50/p95 snapshot age on Wi-Fi in the PR. Run dotnet test and flutter test.
```

## Phase 2 — Haptics

```
Read feasibility brief section 6 and docs/ARCHITECTURE.md section 5.

1. Host forwards game rumble as a KR frame to UDP 4511.
2. Phone maps large motor to a low primitive and small motor to a sharp tick, with a waveform fallback when the primitive is missing.
3. Slide strips: detents at 25/50/75/100 percent, played locally. Edge tick when a finger leaves a claw zone.
4. QA note in the PR: which phone, which effects were felt without looking.

Do not block iOS on Android-only primitives.
```

## Phase 3 — Motion

```
Read feasibility brief section 7.

1. Touch-gated gyro. GyroValid is set only while the zone is held. Host switches to DS4 output while valid.
2. Flick turn: a quick yaw adds a one-shot stick pulse, implemented on the phone in the snapshot, not as a special packet.
3. Racing layout: tilt steering, slide pedals, L1/R1 shifters.
4. Tilt triggers as an opt-in mode that is forced off while gyro aim is on.
5. Android volume rocker as two digital buttons while the app is foregrounded. If iOS cannot do this, hide the option. Do not fake it.
```

## Phase 4 — Adaptive layout

```
Read feasibility brief section 10 (breakthrough fixes).

1. On touch-down, a stick or face cluster re-centres under the thumb within a bounded radius.
2. Persist resting positions on device only. No account, no server.
3. A debug overlay (off by default) shows the learned centres so QA can compare missed presses against the fixed layout.
Do not auto-switch so aggressively that the pad feels haunted. Cap the shift and show a one-line "layout adjusted" toast the first time.
```

## Phase 5 — Driverless spike

```
Read docs/adr/0004. Android only.

Prototype Bluetooth HID gamepad output. Measure p50/p95 against the UDP path on the same phone and the same game. Write the numbers into ADR-0004. Do not delete the UDP path in this PR. Recommend keep / kill in the PR body.
```

## Standing reviews

- **Feel review:** "Play Claw and Slide for ten minutes in <game>. List every time you looked at the phone, and why."
- **Protocol review:** "Diff the Dart encoder against docs/PROTOCOL.md and tests/protocol. List any byte that can disagree."
- **Driver review:** "Confirm this PR does not vendor a .sys, does not require ViGEm for the default path, and releases inputs after silence."
