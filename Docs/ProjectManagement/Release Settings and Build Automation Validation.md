# Release Settings and Build Automation Validation

**Date:** 2026-07-26  
**Owner:** Tanvir  
**Status:** Implemented and validated; Windows and macOS builds verified
separately

## Scope

This slice replaces Unity template defaults with the approved public identity
and adds deterministic release commands for Windows x86-64, macOS Universal,
and WebGL. It does not execute the long platform builds.

## Approved identity

- Company: `Tanvir Ahmed Arnab`
- Product: `Solar System Simulation`
- Version: `1.0.0`
- Application identifier: `com.tanvirahmedarnab.solarsystem`
- Enabled scene: `Assets/SolarSystem/Scenes/SolarSystem.unity`

## Platform contracts

### Windows

- x86-64 non-development player
- IL2CPP
- 1280x720 windowed first launch
- Resizable window and fullscreen switching
- Player log retained for release-candidate QA

### WebGL

- 960x540 initial canvas
- Brotli compression
- Decompression Fallback enabled
- Data caching enabled

### macOS

- Universal Intel 64-bit plus Apple silicon
- Mono for the Windows-hosted build
- Unsigned and unnotarized
- Not tested on macOS

The last three limitations are required public disclosures. Windows remains
the performance and desktop-quality authority.

## Architecture

- `ReleaseBuildContract` owns platform-neutral identity and output constants.
- `ReleaseSettingsManager` applies settings and reports all drift before a
  build.
- `SolarSystemReleaseBuilder` exposes individual and ordered build commands,
  restores temporary desktop backend/architecture changes, and writes one
  JSON report beside every ignored artifact.
- `StandaloneTargetRestorationCoordinator` persists restoration through
  domain reloads, waits for compilation/import work, observes the build target,
  and then requests the prior standalone target asynchronously.
- Edit Mode tests protect identity, window, WebGL, scene, and Universal macOS
  contracts.

## Initial evidence

- Windows build support detected.
- WebGL build support detected.
- macOS Standalone build support detected after Unity reloaded the newly
  installed playback engine.
- Settings validator: `0` issues.
- Edit Mode: `208 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Play Mode: `26 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Unity Console: `0 errors`, `0 warnings`.
- Release artifacts: intentionally not built in this slice.

## Subsequent Windows evidence

The committed Windows x86-64 IL2CPP builder was subsequently executed from
clean pushed commit `81ca928`. The build succeeded with zero reported build
warnings and errors, launched without a release-blocking player-log entry, and
completed the automated available-hardware performance pass. The managed
allocation counter and human-visible acceptance remain incomplete.

See:

`Docs/Release/Windows Release Build and Performance Validation.md`

## Subsequent macOS evidence

The macOS Universal builder was subsequently executed from the same clean,
pushed commit `81ca928`. The build succeeded with zero errors and produced a
valid `.app` bundle whose launcher contains both Intel x86-64 and Apple silicon
arm64 slices. The artifact remains unsigned, unnotarized, and untested because
no Mac test device or Apple Developer membership is available.

Unity recorded one non-blocking warning while the builder restored the prior
Windows target before a deferred platform switch had settled. The editor was
then restored and verified explicitly. The target-restoration timing remains
an automation-hardening item in that first artifact.

See:

`Docs/Release/macOS Universal Build Validation.md`

## Target-restoration hardening candidate

The release builder no longer writes `selectedStandaloneTarget` directly.
Restoration now uses a two-phase, `SessionState`-backed coordinator that first
waits until Unity has visibly activated the platform used by the build, then
waits for compilation/import work to settle before requesting the previous
standalone target asynchronously. Pending state survives the domain reload
caused by the target switch.

Final candidate validation:

- Unity compilation: passed.
- Edit Mode: `213 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Play Mode: `26 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Unity Console: `0 errors`, `0 warnings`.
- Focused restoration-policy tests: early completion blocked, built-target
  observation required, busy-editor wait required, asynchronous switch
  requested only when safe, and already-restored completion verified.

The fix is not yet committed or pushed. The release guard therefore correctly
blocks a macOS rebuild until owner approval, commit, and push are complete.
