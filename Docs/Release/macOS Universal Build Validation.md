# macOS Universal Build Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation support:** Codex, subject to owner review  
**Validation date:** 2026-07-26  
**Source commit:** `81ca928932d9d695ad019888c5abeabd0fb18baa`  
**Status:** Build and bundle-structure validation complete; macOS runtime,
signing, and notarization remain unavailable

## Purpose

This record preserves the first macOS Universal release build produced on the
Windows development machine. Generated players, raw logs, and future archives
remain outside Git. This document does not claim that the application has run
on macOS.

## Release Build

The committed release builder produced an unsigned macOS application bundle
from the same clean, pushed source commit as the Windows release build.

| Field | Result |
|---|---|
| Unity | `6000.5.3f1` |
| Version | `1.0.0` |
| Target | `StandaloneOSX` |
| Result | Succeeded |
| Build duration | `190.746` seconds |
| Build warnings | `1` |
| Build errors | `0` |
| Reported output bytes | `129,771,983` |
| Application | `Solar System Simulation.app` |
| Application files | `177` |
| Bundle identifier | `com.tanvirahmedarnab.solarsystem` |
| Minimum macOS version | `12.0` |
| Launcher SHA-256 | `E722AE1E3FFACAE6DC16F2A66A311D2F15753CB1516492FAB1179CA9A45A6EBF` |

Local ignored evidence:

- `Builds/Release/SolarSystem-1.0.0-macOS-Universal/release-build-report.json`
- `Builds/Release/SolarSystem-1.0.0-macOS-Universal/Solar System Simulation.app/`

## Universal Architecture Evidence

The application launcher has the Mach-O Universal/Fat magic value
`CAFEBABE` and contains two architecture records:

| CPU type | Architecture |
|---|---|
| `0x01000007` | Intel x86-64 |
| `0x0100000C` | Apple silicon arm64 |

The bundle also contains the expected `Contents/Info.plist`,
`Contents/MacOS`, `Contents/Resources`, and `Contents/Frameworks` entries.
Its `Info.plist` identifies the product, version, application category,
supported macOS platform, and minimum OS version.

## Build Warning

The player build itself succeeded with zero errors. Unity counted one warning
after producing the bundle because the release builder attempted to restore
the selected Windows standalone target while Unity's deferred macOS target
switch was still completing:

`A previous deferred switch build target has not been completed`

The editor target was subsequently restored explicitly and verified as
`StandaloneWindows64` with IL2CPP and the approved architecture. This warning
did not invalidate the generated `.app`, but the target-restoration timing
required hardening before the final release-candidate rebuild.

The hardening candidate now replaces direct target assignment with a
domain-reload-safe, two-phase asynchronous coordinator. Compilation, all `213`
Edit Mode tests, all `26` Play Mode tests, and a fresh zero-warning/error
Console check pass. Because release builds require clean pushed source, the
macOS artifact must be rebuilt only after Tanvir approves this candidate and
the fix is committed and pushed.

## Required Limitations

This artifact is:

- unsigned;
- unnotarized; and
- untested on macOS because no Mac test device is available.

These limitations must appear in the itch.io download description. The
project must not claim Gatekeeper certification or macOS runtime compatibility
until a Mac owner performs an actual launch and walkthrough.

## Packaging Requirements

Before upload:

1. exclude `Solar System Simulation_BurstDebugInformation_DoNotShip`;
2. create the archive with a method that preserves the `.app` bundle and Unix
   executable permissions;
3. inspect the ZIP contents and record its SHA-256;
4. classify the upload as an executable for macOS; and
5. retain the unsigned, unnotarized, and untested disclosure.

If the final source release commit changes, this artifact and its evidence
must be regenerated from that final pushed commit.
