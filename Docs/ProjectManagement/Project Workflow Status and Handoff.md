# Project Workflow Status and Handoff

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Maintainer:** Unity Game Project Manager / Codex, subject to owner review  
**Status:** Released — maintenance mode  
**Release:** `1.0.0`  
**Released:** 2026-07-28  
**Repository:** `C:\Users\taarn\Desktop\Unity\SolarSystem`  
**Public page:** https://tanvirahmedarnab.itch.io/solar-system-simulation

## Purpose

This is the project-local entry point for resuming Solar System Simulation
after its first public release. It points to the authoritative project
documents, records the certified release boundary, and prevents a future
thread from restarting completed work.

## Released State

- Unity: `6000.5.3f1`
- Render pipeline: Universal Render Pipeline `17.5.0`
- Release version: `1.0.0`
- Release-source commit:
  `6338a5fba17c79321347d634ede8dde4394aa836`
- Platforms: Windows x86-64, hosted WebGL, and unsigned Universal macOS
- Public state: itch.io `Released`, `Public`, and `No payments`
- Assignment URL submission: completed and confirmed by Tanvir
- External browser/device verification: passed and confirmed by Tanvir
- Runtime status: owner-accepted Windows player and hosted WebGL player
- macOS limitation: unsigned, unnotarized, and not runtime-tested on macOS

The documentation-only closure commit follows the release-source commit and
does not change the shipped runtime or artifact hashes.

## Authoritative Documents

| Area | Authority | Path |
|---|---|---|
| Product and design | Living GDD | `Docs/Design/GDD.md` |
| Technical architecture | Living TDD | `Docs/Technical/TDD.md` |
| Visual and content direction | Art Bible | `Docs/Art/ArtBible.md` |
| Controls | Controls reference | `Docs/Design/Controls.md` |
| Third-party assets | Licensing ledger | `Docs/Legal/ThirdPartyAssets.md` |
| Scientific data | Source ledger | `Docs/Science/Celestial Data Sources.md` |
| Repository baseline | Pre-first-project-commit checklist | `Docs/ProjectManagement/Pre-First-Project-Commit Checklist.md` |
| Release certification | Release evidence checklist | `Docs/Release/Release Evidence Checklist.md` |
| itch.io publication | Publication record | `Docs/Release/itch.io Page Draft.md` |
| Release packaging | Artifact tooling | `Tools/Release/README.md` |

Update the owning document instead of duplicating its rules elsewhere.

## Release Artifact Record

The ignored local release directory contains the final artifact manifest,
checksums, archives, and platform build reports. All three archives identify
the same release-source commit.

| Platform | SHA-256 |
|---|---|
| Windows x86-64 | `998B755C3F33F410983976BAE7E2E6C5AE97DFC98B75F2469755C51C3C0804C2` |
| WebGL | `558C638D3040E84099F57D6D2F76B3BD2C6C8AFC729D1199C37567BF9A381238` |
| macOS Universal | `495590C68DB1C370BCCFD7C2C1C7F6BBA5C53F63748C2838FDF4464ABD268481` |

Build folders, archives, raw reports, browser state, and account data remain
outside version control.

## Maintenance Rules

1. Treat v1.0.0 as the completed baseline; do not silently expand its scope.
2. Start future features, content, signing, video, case-study, or platform work
   as a separately approved milestone.
3. Inspect Git status and the latest GDD/TDD revisions before changing files.
4. Review scene, prefab, asset, package, render, and ProjectSettings diffs with
   additional care.
5. Re-run affected Unity tests and player acceptance checks after runtime
   changes.
6. Rebuild every published platform from one clean, pushed commit when a
   release changes.
7. Recheck licenses and scientific-source disclosures before redistributing
   changed media.
8. Obtain owner approval before every commit and push.

## Reusable Workflow Handoff

The validated reusable workflow is maintained in:

`G:\My Drive\08 AI Agent Projects\01 Active Projects\Personal Agents\Unity Game Project Manager`

Solar System Simulation established reusable standards for:

- Unity-aware repository foundations and pre-commit gates;
- living GDD, TDD, Art Bible, licensing, and scientific-source authorities;
- ScriptableObject-driven architecture and deterministic analytical motion;
- slice-based implementation, validation, and owner acceptance;
- same-commit multi-platform builds and deterministic packaging;
- macOS permission-preserving ZIP creation from Windows;
- hosted WebGL and responsive-UI acceptance;
- fit-with-padding media normalization with preserved originals;
- secure itch.io account boundaries, publication approval, and player-facing
  verification; and
- release closure that promotes general lessons without copying project-only
  data or account secrets.

## Resume Decision

No production work is pending for the approved v1.0.0 scope. A future thread
should begin by asking whether the owner wants maintenance, a v1.0.1 fix, a
v1.1 feature milestone, portfolio-video/case-study work, or no further change.
