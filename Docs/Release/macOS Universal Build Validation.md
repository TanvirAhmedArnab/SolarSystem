# macOS Universal Build Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation support:** Codex, subject to owner review  
**Validation date:** 2026-07-26  
**Source commit:** `bf80733370a847f05f41d6a0d8fb879fb6940eb1`  
**Status:** Build and bundle-structure validation complete; macOS runtime,
signing, and notarization remain unavailable

## Purpose

This record preserves the macOS Universal release build regenerated on the
Windows development machine after hardening editor-target restoration.
Generated players, raw logs, and future archives remain outside Git. This
document does not claim that the application has run on macOS.

## Release Build

The committed release builder produced an unsigned macOS application bundle
from clean, pushed target-restoration commit `bf807333`. The final Windows,
WebGL, and macOS publication artifacts will later be regenerated from one
final release commit.

| Field | Result |
|---|---|
| Unity | `6000.5.3f1` |
| Version | `1.0.0` |
| Target | `StandaloneOSX` |
| Result | Succeeded |
| Build duration | `15.649` seconds |
| Build warnings | `1` |
| Build errors | `0` |
| Reported output bytes | `129,771,983` |
| Application | `Solar System Simulation.app` |
| Application files | `177` |
| Bundle identifier | `com.tanvirahmedarnab.solarsystem` |
| Minimum macOS version | `12.0` |
| Launcher SHA-256 | `6FFBED3E4AE2911CB1AEF7D5D53680260348D771C7A22D264DB4B28703930E68` |

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

## Build Warning and Editor Restoration

The player build succeeded with zero errors. Unity counted one non-blocking
shader warning from its installed render-pipeline package:

`Hidden/Core/DebugOccluder: implicit truncation of vector type`

The warning originates from
`Packages/com.unity.render-pipelines.core/Runtime/RenderPipelineResources/GPUDriven/DebugOccluder.shader`
and does not come from project-authored shader code.

The previous deferred-target-switch diagnostic did not recur. The two-phase,
domain-reload-safe coordinator observed macOS activation before requesting
restoration and then completed the asynchronous return to
`StandaloneWindows64`. Post-build inspection verified:

- active target: `StandaloneWindows64`;
- selected standalone target: `StandaloneWindows64`;
- scripting backend: `IL2CPP`;
- standalone architecture: `0`;
- pending restoration: `false`; and
- Unity Console: `0 errors`, `0 warnings`.

Compilation, all `213` Edit Mode tests, all `26` Play Mode tests, and the
focused restoration-policy tests had already passed before the clean pushed
build.

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
