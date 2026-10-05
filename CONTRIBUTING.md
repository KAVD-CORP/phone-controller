# Contributing

This repository is public. Pull requests are the way in, including from people who are not on the KAVD team.

## KAVD team

| Person | Primary | GitHub |
|---|---|---|
| Bubu (Daniel Mawuena) | CTO, QA, latency | `danbubu` |
| Nissi | Phone UI | `kkopong` |
| Allan | Windows host, virtual pad | `allan-osei` |
| Virgil | On-glass layout | `Nana-asante-nocturnal` |

## Local setup

- .NET 9 SDK
- Flutter stable (only once you work on `apps/phone`)
- Windows, if you are working on the host’s virtual pad
- Optional later: [VIIPER](https://github.com/Alia5/VIIPER) / usbip-win2, installed from upstream. Do not commit driver binaries.

```bash
dotnet test
dotnet run --project apps/host
```

## Branching

`main` stays buildable. Branch `feat/…`, `fix/…`, `docs/…`. One phase from `docs/PROMPTS.md` per PR. Your first session is the block with your name in `docs/KICKOFF_PROMPTS.md`. Squash-merge with a Conventional Commit title.

## Rules that keep the project open

- MIT. Do not add a CLA.
- Do not commit proprietary drivers, `.sys` files, or paid SDKs.
- Protocol changes update `docs/PROTOCOL.md` and the tests together.
- UI copy must not claim the phone has real trigger resistance.

## Definition of done

`dotnet test` green. A protocol change has a failing test that the change makes pass. A layout change names which concept in the brief it implements, and what it refuses to do (analog, gyro, iOS).
