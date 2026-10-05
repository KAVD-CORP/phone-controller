# ADR-0002 — .NET host, VIIPER first, ViGEm as fallback

Status: Accepted (5 Oct 2026)

## Context

Games see an Xbox 360 or DualShock 4. The usual way to create one in software is ViGEmBus. Nefarius archived it in 2023 (trademark retirement). It still works. A new public project that depends on it alone will break on a future Windows release with no upstream fix.

[VIIPER](https://github.com/Alia5/VIIPER) creates the same class of device from userspace over USB/IP (`usbip-win2` on Windows) and documents C and C# embeddings, including Xbox 360 and DualShock 4. DS4 is how gyro reaches Steam Input.

## Decision

The host is a .NET 9 process on Windows x64.

- Default backend: VIIPER.
- Fallback backend: ViGEmBus, same `IVirtualPad` interface, used only when VIIPER fails to start.
- Gyro enabled → DualShock 4 output. Gyro off → Xbox 360 output.
- We do not write or ship a kernel driver.

## Consequences

- First-run setup asks the user to install usbip-win2 (VIIPER’s documented Windows driver). We link to upstream. We do not rehost the driver binary.
- Phase 1 must show both backends creating a pad that `joy.cpl` can see, on one machine, before layouts are considered done.
- Anti-cheat may reject virtual devices. That is documented, not worked around.
