# Windows Release Build and Performance Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation support:** Codex, subject to owner review  
**Validation date:** 2026-07-26  
**Source commit:** `81ca928932d9d695ad019888c5abeabd0fb18baa`  
**Status:** Build and automated available-hardware evidence complete; final
human-visible acceptance and reference-hardware certification remain pending

## Purpose

This record preserves the first Windows x86-64 release build and Stage 3
performance evidence. Generated players, raw JSON captures, logs, and future
archives remain outside Git. This document does not convert incomplete or
unavailable measurements into passes.

## Release Build

The committed release builder produced a non-development Windows x86-64
IL2CPP player from a clean, pushed source commit.

| Field | Result |
|---|---|
| Unity | `6000.5.3f1` |
| Version | `1.0.0` |
| Target | `StandaloneWindows64` |
| Result | Succeeded |
| Build duration | `220.839` seconds |
| Build warnings | `0` |
| Build errors | `0` |
| Reported output bytes | `1,164,091,177` |
| Executable | `Solar System Simulation.exe` |
| Executable SHA-256 | `1B8448700944F099D1E11D6B28E2BCD151A59430BF4858C60596E66515FED6A7` |

Local ignored evidence:

- `Builds/Release/SolarSystem-1.0.0-Windows-x86_64/release-build-report.json`
- `Builds/Release/SolarSystem-1.0.0-Windows-x86_64/`

The generated folder contains
`Solar System Simulation_BackUpThisFolder_ButDontShipItWithYourGame` and
`Solar System Simulation_BurstDebugInformation_DoNotShip`. They account for
approximately `1,046,346,036` bytes and must not enter the public archive.
The estimated shippable files total `117,927,789` bytes before ZIP
compression. Packaging remains a separate release gate.

## Startup Smoke Test

The player:

- created a responsive titled window;
- loaded the production player data;
- initialized Direct3D 12 on the NVIDIA GPU;
- initialized the Input System and PhysX;
- remained responsive throughout the smoke-test interval; and
- produced no exception, crash, or release-blocking `Player.log` entry.

The log contains a non-blocking Direct3D diagnostics-interface message:
`d3d12: failed to query info queue interface (0x80004002)`. Rendering
continued normally. No player failure was associated with this message.

Detailed mouse, keyboard, audio, focus, and responsive-layout acceptance was
not completed because Windows app-control permission for the newly generated
executable timed out. Those checks remain owner-visible release gates.

Tanvir subsequently confirmed that the current visible player window passes
the panel-safe-area check: the status panel, Help button, and complete
quick-controls panel are readable and remain within the window. The supplied
screenshot appears maximized, so it does not independently prove an exact
1280x720 viewport. The exact-resolution checkbox therefore remains open.

## Performance Capture Method

The production player ran at `1920x1080`, PC quality, Direct3D 12 with:

`-solarSystemPerformance -solarSystemPerformanceSampleFrames 2400`

Every one of the 11 production scenarios captured `2,400` frames and more than
the required three seconds. The first default-buffer diagnostic was retained
locally as incomplete evidence because this GPU exceeded 500 FPS and filled
the default 1,024-sample buffer early. The corrected capture used the existing
supported command-line override rather than weakening the duration contract.

Corrected ignored evidence:

`Builds/Release/Evidence/windows-performance-81ca928-full.json`

### Available Test Hardware

- Windows 11
- Intel Core i9-13900HX
- NVIDIA GeForce RTX 4090 Laptop GPU
- 64 GB system memory
- Direct3D 12

This machine is materially faster than the approved mid-range reference
contract. Results demonstrate current-build behavior on available hardware;
they do not prove equivalent results on an RTX 3060/RX 6600 class system.

## Scenario Results

| Scenario | Frame P95 | Frame P99 | Main P95 | GPU P95 | System used P95 |
|---|---:|---:|---:|---:|---:|
| PERF-01 | 3.094 ms | 6.170 ms | 1.423 ms | 0.342 ms | 757.3 MiB |
| PERF-02 | 2.938 ms | 4.510 ms | 1.305 ms | 0.339 ms | 769.6 MiB |
| PERF-03 | 3.023 ms | 5.838 ms | 1.328 ms | 0.379 ms | 781.3 MiB |
| PERF-04 | 3.101 ms | 6.498 ms | 1.152 ms | 0.316 ms | 797.3 MiB |
| PERF-05 | 3.099 ms | 6.638 ms | 1.307 ms | 0.300 ms | 802.4 MiB |
| PERF-06 | 3.092 ms | 6.600 ms | 1.352 ms | 0.287 ms | 803.6 MiB |
| PERF-07 | 3.146 ms | 5.997 ms | 1.335 ms | 0.318 ms | 804.6 MiB |
| PERF-08 | 3.047 ms | 6.098 ms | 1.306 ms | 0.260 ms | 807.5 MiB |
| PERF-09 | 3.036 ms | 5.994 ms | 1.384 ms | 0.254 ms | 807.7 MiB |
| PERF-10 | 3.009 ms | 6.015 ms | 1.376 ms | 0.265 ms | 808.0 MiB |
| PERF-11 | 3.013 ms | 6.455 ms | 1.330 ms | 0.257 ms | 808.0 MiB |

The worst available measurements remained below these numeric budgets:

| Gate | Budget | Worst observed | Available-hardware result |
|---|---:|---:|---|
| Total frame P95 | 16.67 ms | 3.146 ms | Pass |
| Total frame P99 | 25.00 ms | 6.638 ms | Pass |
| Main thread P95 | 13.33 ms | 1.423 ms | Pass |
| GPU P95 | 13.33 ms | 0.379 ms | Pass |
| Steady process memory | 1.5 GiB | 808.0 MiB harness counter | Pass |
| Peak process memory | 2 GiB | 1,209,012,224 bytes externally sampled | Pass |
| Dedicated application GPU memory | 2 GiB | 349,573,120 bytes externally sampled | Pass |
| Cold launch | 10 seconds | 1.472 seconds to responsive window | Pass |
| Managed allocation P95 | 0 bytes/frame | Counter unavailable | Incomplete |

The JSON document correctly reports `Incomplete` because
`GC Allocated In Frame` is unavailable in this non-development release
player. The non-gated `Batches Count` counter is also unavailable; Standard
Draw Calls, SRP Batcher Draw Calls, SetPass, and triangle counters are
available. No allocation pass is claimed.

## Remaining Windows Acceptance

Before Gate 2 can close:

1. complete the owner-visible keyboard and mouse walkthrough;
2. verify selection, focus, zoom, simulation time, navigation, scale
   comparison, cinematic tour, settings, credits, lighting, and audio;
3. inspect 1280x720, 1920x1080, the approved small window, resize transitions,
   and UI safe areas;
4. verify mute, channel levels, persistence, Full Motion, and Reduced Motion;
5. investigate managed allocation with a suitable development-player capture;
6. repeat representative performance checks on the approved mid-range hardware
   class when that hardware is available; and
7. create a sanitized ZIP that excludes Unity's two do-not-ship directories.

If the final source release commit changes, all release artifacts and embedded
evidence must be regenerated from that final pushed commit.
