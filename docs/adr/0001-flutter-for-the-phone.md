# ADR-0001 — Flutter for the phone, not one shared language

Status: Accepted (5 Oct 2026)

## Context

The brief needs Android and iOS, five custom layouts, haptic primitives, and a 250 Hz sampler. The team knows React. A single language across phone and Windows host is attractive and wrong: the host’s job is a virtual USB device, which Flutter and React Native do not speak.

## Decision

The phone app is Flutter (Dart 3, stable channel). Android and iOS share the layouts. Platform channels are allowed only for haptic primitives and the Android volume rocker. React Native, MAUI, and two fully native apps are rejected for the reasons in `docs/TECH_STACK.md` §3.

## Consequences

- A second encoder (Dart) must match `tests/protocol`.
- If Flutter cannot hold 250 Hz on a target phone, we add a native sampler behind a channel. We do not rewrite the UI.
- iOS ships the touch layouts. Volume keys are Android-only until a spike says otherwise.
