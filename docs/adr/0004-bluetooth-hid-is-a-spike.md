# ADR-0004 — Bluetooth HID is a spike, not the transport

Status: Accepted (5 Oct 2026). Revisit after Phase 5.

## Context

Android 9+ can act as a Bluetooth HID device, which would mean no PC driver. The brief marks this unverified. iOS does not offer the same role to ordinary apps. Latency for fast action games is the open question, not the API’s existence.

## Decision

Phases 1–4 use UDP (and USB tether as a jitter escape). Phase 5 builds an Android-only HID prototype and writes the measurements in this file: p50/p95 latency versus UDP, which games accepted the device, which phones failed to register. We drop UDP only if HID is at least as good on those numbers.

## Consequences

Do not design the packet format around HID reports. The snapshot format stays. A HID backend, if it wins, is another `IVirtualPad`-shaped output on the phone, not a new protocol.
