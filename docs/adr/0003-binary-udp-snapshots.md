# ADR-0003 — Binary UDP snapshots, not events and not JSON

Status: Accepted (5 Oct 2026)

## Context

The brief requires compact state snapshots so a lost packet costs nothing, at 250 Hz or more. Existing apps often send touch events. A lost “button up” sticks the trigger. JSON or protobuf at that rate allocates for no benefit: the payload is a handful of integers.

## Decision

Versioned little-endian datagrams as in `docs/PROTOCOL.md`. 36 bytes phone → host, 10 bytes host → phone. UDP. Sequence number plus timestamp. The host drops unknown versions.

## Consequences

- Dart and C# each encode. Conformance is a byte-for-byte test, not a shared schema compiler.
- Interface haptics never wait for the return packet.
- After 100 ms with no packet the host releases buttons and triggers and recentres sticks.
