# Release Settings and Build Automation Validation

**Date:** 2026-07-26  
**Owner:** Tanvir  
**Status:** Implemented and validated; release builds pending

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
