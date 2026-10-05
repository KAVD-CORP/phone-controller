# Phone Controller

Use your phone as a PC gamepad. The hard part is not sending input. The hard part is triggers: a phone has no bumpers, no analog springs and no tactile landmarks, so players look down. This project treats that as the product.

A **public** KAVD Corp repository. MIT license. Outside contributions are welcome.

## Start here

1. `AGENTS.md` — rules.
2. `docs/TECH_STACK.md` — Flutter on the phone, .NET on Windows, and the options we rejected (including “one language for both”).
3. `docs/ARCHITECTURE.md` — packet path, layouts, latency.
4. `docs/PROTOCOL.md` — the 36-byte snapshot.
5. `docs/adr/` — Flutter, VIIPER, UDP, Bluetooth spike.
6. `docs/reference/Phone_Controller_Feasibility.docx` — the brief (4 Oct 2026).
7. `docs/PROMPTS.md` — phase prompts for Cursor.

## What runs today

The wire format and its tests. The Windows host prints a round-trip and exits. The Flutter app is Phase 0 (`docs/PROMPTS.md`); the SDK is not vendored in this repo.

```bash
dotnet test
dotnet run --project apps/host
```

## Layout

| Path | What |
|---|---|
| `src/protocol` | C# codec |
| `tests/protocol` | Conformance tests the Dart port must match |
| `apps/host` | Windows host (UDP and virtual pad: Phase 1) |
| `apps/phone` | Flutter app (created in Phase 0) |

## Status

Stack decided. Phase 0 not started.
