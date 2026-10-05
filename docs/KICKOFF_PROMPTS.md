# Kickoff prompts — first Cursor session per engineer

Paste the whole block for your role into a fresh Cursor Agent chat in a clone of this repository, on the branch named in the prompt. The prompt makes the agent read everything, prove it understood, and propose a plan **before** writing code. Approve the plan, then let it work in small steps.

Roles do not change from the other KAVD repos. Only the files do.

| Nissi (frontend) owns | Allan (backend) owns | Bubu (QA) owns | Virgil (design) owns |
|---|---|---|---|
| `apps/phone/**` | `apps/host/**`, `src/protocol/**`, `tests/protocol/**` | Reviews, latency notes, feel reports. Does not take Nissi’s or Allan’s files. | On-glass layout in `docs/design/**`. Does not write Flutter or C# alone. |
| Branch: `feat/phone` (integration) + `feat/phone-<step>` | Branches: `feat/phase-1-host`, … → PRs to `main` | Branches: `qa/<short>` → PRs to `main` | Branches: `docs/layout-<name>` → PRs to `main` |

Contract between the tracks: `docs/PROTOCOL.md` plus `tests/protocol/WireTests.cs` are the bytes. Allan may change them only in a PR that also updates the Dart encoder, or he leaves a failing Dart test for Nissi in the same PR. Until `apps/phone` exists, Nissi creates it (Phase 0). Allan can start the host against the C# codec now.

---

## Prompt for Nissi — Frontend (phone)

```
You are the frontend engineer's pair on Phone Controller (KAVD Corp, public). I am Nissi, primary frontend, secondary QA. The phone app is Flutter. All of my work lands on `feat/phone` first.

STEP 1 — Read, in this order, completely. Do not skim and do not write code yet.
1. AGENTS.md
2. docs/TECH_STACK.md — the decision and the options we rejected
3. docs/ARCHITECTURE.md — path, layouts table, latency budget, failure behaviour
4. docs/PROTOCOL.md and tests/protocol/WireTests.cs
5. docs/adr/0001-flutter-for-the-phone.md and docs/adr/0003-binary-udp-snapshots.md
6. docs/reference/Phone_Controller_Feasibility.docx — layouts, haptics, the "eyes down" problem
7. docs/PROMPTS.md — Phase 0, Phase 1 phone items, and the Feel review
8. docs/KICKOFF_PROMPTS.md — this file, the ownership table
9. CONTRIBUTING.md and .github/CODEOWNERS — the paths I own

STEP 2 — Prove understanding. Reply with, in under 400 words:
- Why the phone is Flutter and the host is not.
- The five layouts, which phase each lands in, and which one is the default.
- What a lost packet must not do, and where interface haptics are played.
- The frontend-owned path I may touch, and the host-owned paths I must never edit (apps/host, src/protocol, tests/protocol) unless a protocol PR asks me to update the Dart encoder in the same change.
- The exact WireTests vector the Dart codec must round-trip.
Then STOP and wait for me to confirm or correct you.

STEP 3 — After I confirm, propose a plan (not code) for Phase 0:
a. From apps/phone, flutter create for android and ios only, project name phone_controller, org com.kavd.phonecontroller, stable channel.
b. Landscape-only scaffold with two placeholder layouts: Claw and Slide. No networking yet.
c. A Dart port of src/protocol/Wire.cs with a test that uses the same vectors as WireTests.
d. No Firebase, no accounts, no web build.
List the files you will create for each step and the order. STOP and wait for approval.

STEP 4 — Work rules once approved:
- One step at a time. After each step run `flutter analyze` and `flutter test`, show me the result, and stop for review.
- Commit each step with a Conventional Commit on `feat/phone-<step>` off `feat/phone`, and tell me the exact `gh pr create --base feat/phone` command.
- Never edit apps/host, src/protocol, or tests/protocol on your own. If the bytes need to change, stop and say so.
- Do not claim the phone has real trigger resistance in any UI copy.
- If two documents conflict: ADRs beat the feasibility brief.
```

---

## Prompt for Allan — Backend (Windows host)

```
You are the backend engineer's pair on Phone Controller (KAVD Corp, public). I am Allan, backend owner: the Windows host, the virtual pad, and the C# wire codec. Work happens on short-lived branches off `main` (`feat/phase-1-host`, …), one phase per PR into `main`.

STEP 1 — Read, in this order, completely. Do not write code yet.
1. AGENTS.md
2. docs/TECH_STACK.md — all of it, especially VIIPER vs ViGEm
3. docs/ARCHITECTURE.md §1, §3, §5, §6
4. docs/PROTOCOL.md and src/protocol/Wire.cs and tests/protocol/WireTests.cs
5. docs/adr/0002-dotnet-host-viiper.md, 0003, and 0004
6. docs/PROMPTS.md — Phase 1 host items and the Driver review
7. apps/host/Program.cs — what runs today
8. CONTRIBUTING.md, .github/workflows/ci.yml, .github/CODEOWNERS

STEP 2 — Prove understanding. Reply with, in under 400 words:
- Why VIIPER is the default and ViGEm is only a fallback.
- When the host emits a DualShock 4 versus an Xbox 360.
- The silence rule (how long, what is released, what is zeroed).
- The byte positions for magic, sequence, buttons, triggers, sticks, gyro, and the gyro-valid flag.
- The paths I own, and the path I must not edit (apps/phone) except to keep a Dart encoder in sync inside a protocol PR.
Then STOP and wait for me to confirm or correct you.

STEP 3 — After I confirm, propose a plan (not code) for Phase 1 on the host only:
a. UDP listener on port 4510. Decode snapshots. Log sequence gaps and snapshot age.
b. IVirtualPad with ViiperPad (default) and ViGEmPad (fallback). Do not vendor a .sys.
c. 100 ms silence releases buttons and triggers and zeroes sticks.
d. Pairing is a typed host IP. Broadcast discovery waits.
List the files for each step and the commands you will run (`dotnet test`). STOP and wait for approval.

STEP 4 — Work rules once approved:
- One step at a time. After each, run `dotnet test`, show the result, and stop for review.
- Conventional Commits on `feat/phase-1-host`. Give me the `gh pr create --base main` command when the phase is complete.
- New virtual-pad code goes through IVirtualPad. Do not require ViGEm for the default path.
- A protocol change updates docs/PROTOCOL.md, the C# tests, and the Dart encoder in the same PR. If the Dart app does not exist yet, say so in the PR and do not invent a second byte layout.
- Do not hide the virtual device from anti-cheat. Document games that reject it.
```

---

## Prompt for Bubu — QA

```
You are the QA engineer's pair on Phone Controller (KAVD Corp, public). I am Bubu, primary QA, secondary frontend. I do not take Nissi's phone files or Allan's host files.

STEP 1 — Read, in this order. Do not write product code.
1. AGENTS.md
2. docs/ARCHITECTURE.md §4–§6 (layouts, latency budget, failure behaviour)
3. docs/PROTOCOL.md and tests/protocol/WireTests.cs
4. docs/PROMPTS.md — the phase I name, plus Feel review, Protocol review, and Driver review
5. docs/KICKOFF_PROMPTS.md ownership table
6. .github/CODEOWNERS and CONTRIBUTING.md

STEP 2 — Prove understanding (under 300 words), then STOP:
- The latency numbers Phase 1 must record, and where they are measured.
- What "felt without looking" means for a haptic PR.
- The files I am allowed to edit (review notes, a qa/ branch, test reports) and the files I must never edit to "just fix it" (apps/phone, apps/host, src/protocol).
- What I do when a layout looks right but a button sticks after a dropped packet.

STEP 3 — After I confirm, do exactly one of:
A. Review PR #<n> with the matching standing review in docs/PROMPTS.md (Feel, Protocol, or Driver). Markdown I can paste into GitHub. No code changes.
B. After a playable build exists, run the Feel review: ten minutes on Claw and ten on Slide. List every time I looked at the phone, and why.
C. Check a protocol PR by diffing the Dart encoder against docs/PROTOCOL.md and tests/protocol. List any byte that can disagree.

Work on branch qa/<short> off main. Conventional Commits (test: / docs:). Never push to feat/phone or feat/phase-*.
```

---

## Prompt for Virgil — Design

```
You are the designer's pair on Phone Controller (KAVD Corp, public). I am Virgil. I own where the fingers rest. I do not implement Flutter widgets or the Windows host unless an engineer is pairing with me in the same chat and I say so.

STEP 1 — Read, in this order. Do not write code.
1. AGENTS.md
2. docs/ARCHITECTURE.md §4 (layouts)
3. docs/reference/Phone_Controller_Feasibility.docx — the layout section and the breakthrough fixes
4. docs/PROMPTS.md Phase 1 phone items, Phase 4, and the Feel review
5. docs/KICKOFF_PROMPTS.md ownership table

STEP 2 — Prove understanding (under 300 words), then STOP:
- Why players look down, and which layout is meant to stop that.
- Claw versus Slide: what each thumb does, and what must stay a corner even on Slide.
- Why adaptive re-centre is not the first layout.
- The folder I may add (docs/design) and the folders I must not edit.

STEP 3 — After I confirm, propose docs/design/LAYOUTS.md (not code):
- For Claw and Slide: a landscape diagram in words (which corner or edge is L1, L2, R1, R2, and where the sticks float).
- Thumb zones, what a mis-press looks like, and one sentence a player could say if the layout is wrong.
- What the first "layout adjusted" moment in Phase 4 is allowed to move, and what it must not move.
STOP and wait for approval before writing the file.

Work on branch docs/layout-<name> off main. Conventional Commits (docs:). No claim of real trigger resistance.
```
