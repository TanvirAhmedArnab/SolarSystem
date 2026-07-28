# WebGL Build Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation support:** Codex, subject to owner review  
**Validation date:** 2026-07-27  
**Source commit:** `99ac9a678dd57af3fe827455e6642e0c2e37f4d5`  
**Status:** Provisional build, structure, fallback-loading, and keyboard-input
validation complete; final synchronized rebuild and itch.io-hosted acceptance
remain pending

## Purpose

This record preserves the first successful WebGL release build and its local
browser smoke test. Generated players, raw build reports, browser-session data,
and future ZIP archives remain outside Git. This build proves the platform
pipeline before Windows, WebGL, and macOS are regenerated from one final pushed
release commit.

## Release Build

The committed release builder produced a non-development WebGL player from
clean, pushed commit `99ac9a6`. Unity's Decompression Fallback was enabled, and
the generated payloads use the expected `.unityweb` naming.

| Field | Result |
|---|---|
| Unity | `6000.5.3f1` |
| Version | `1.0.0` |
| Target | `WebGL` |
| Result | Succeeded |
| Build duration | `568.251` seconds |
| Build warnings | `2` |
| Build errors | `0` |
| Reported output bytes | `27,098,373` |
| Validated file count | `19` |
| Validated uncompressed bytes | `27,188,882` |

Local ignored evidence:

- `Builds/Release/SolarSystem-1.0.0-WebGL/release-build-report.json`
- `Builds/Release/SolarSystem-1.0.0-WebGL/`

The structural validator confirmed:

- `index.html` exists at the build root;
- `Build` contains loader, data, framework, and WebAssembly payloads;
- `TemplateData` contains the Unity Web template resources;
- the report identifies the exact pushed source commit and release version;
- the build completed with zero errors; and
- WebGL Decompression Fallback payloads are present.

## Local Browser Smoke Test

The complete build folder was served unchanged over a plain local HTTP server
without compressed-content response headers. This deliberately exercised
Unity's Decompression Fallback rather than depending on server-side
`Content-Encoding` configuration.

The Codex in-app Chromium browser:

- loaded and rendered the full simulation;
- displayed the responsive status, Help, labels, and quick-controls UI;
- accepted a canvas click as the first interaction;
- accepted the `H` keyboard command;
- opened and closed the Help & Orientation panel; and
- reported zero browser-console errors.

This is local pipeline evidence, not itch.io-hosted acceptance. Browser audio,
storage persistence, resize/fullscreen behavior, and the complete interaction
walkthrough remain required after upload.

## Non-Blocking Diagnostics

Unity reported two build-time toolchain warnings:

- a WebGPU compatibility warning for Firefox versions before `149`; and
- a deprecated internal JavaScript `$stackTrace` library API warning.

The local browser emitted three non-blocking runtime warnings:

- Unity's legacy manual persistent-data synchronization API is deprecated;
- the current WebGL graphics path does not support URP's Edge Adaptive Spatial
  Upsampling pass; and
- the current platform does not support GPU Resident Drawer batch buffers.

The player rendered and accepted keyboard input despite these diagnostics.
They originate in Unity platform/render-pipeline capabilities rather than
project-authored gameplay code. None produced a browser-console error during
the smoke test.

## Public Packaging Correction

Unity also generated
`Solar System Simulation_BurstDebugInformation_DoNotShip`. The raw build may
retain that local diagnostic folder, but it must not enter an itch.io upload.
The repository packager now excludes and rejects Unity do-not-ship directories
for every platform, with regression coverage for WebGL.

## Remaining WebGL Gates

Before publication:

1. commit and push the cross-platform packaging correction;
2. regenerate WebGL from the final synchronized release commit;
3. create and validate the root-correct ZIP archive;
4. upload it to itch.io as `This file will be played in the browser`;
5. repeat focus, input, audio-start, persistence, resize, fullscreen, and
   browser-console checks on the hosted page; and
6. record the final ZIP SHA-256 and public URL.
