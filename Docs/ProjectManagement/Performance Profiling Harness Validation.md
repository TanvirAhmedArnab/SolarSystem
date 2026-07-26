# Performance Profiling Harness Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Technical steward:** Codex, subject to owner review  
**Validation date:** 2026-07-25  
**Status:** Harness candidate validated; release performance not yet certified

## Purpose

This record verifies that the project can reproduce performance captures
across the approved release experience. It deliberately separates Editor
diagnostics from standalone acceptance evidence.

## Approved Contract

The reference target is Windows 10/11 at 1920x1080 using the PC quality tier:

- Intel Core i5-12400F or AMD Ryzen 5 5600 class CPU;
- NVIDIA GeForce RTX 3060 or AMD Radeon RX 6600 class GPU;
- 16 GB RAM and SSD storage.

The gates are:

| Metric | Budget |
|---|---:|
| Total frame time P95 | 16.67 ms |
| Total frame time P99 | 25.00 ms |
| Main-thread frame time P95 | 13.33 ms |
| GPU frame time P95 | 13.33 ms |
| Steady-state managed allocation P95 | 0 bytes/frame |
| Steady process memory | 1.5 GiB |
| Peak process memory | 2 GiB |
| Dedicated application GPU memory | 2 GiB |
| Cold launch to interactive | 10 seconds |

## Implemented Harness

The runtime harness remains inactive during normal play. It is activated by
`-solarSystemPerformance` for player automation or by:

`Tools > Solar System > Validation > Run Performance Diagnostic`

The Editor command writes an ignored JSON result to:

`Temp/Performance/solar-system-editor-diagnostic.json`

The capture uses the production scene and services. It records raw samples,
nearest-rank percentile summaries, environment identity, capture settings,
metric provenance, counter availability, scenario observations, and known
limitations. Missing counters are marked unavailable, never reported as zero.

## Scenario Matrix

| ID | Production state |
|---|---|
| PERF-01 | Readable overview |
| PERF-02 | Earth close focus |
| PERF-03 | Credits & Sources |
| PERF-04 | Scale comparison: readable overview |
| PERF-05 | Scale comparison: normalized orbits |
| PERF-06 | Scale comparison: literal Earth reference |
| PERF-07 | Cinematic chapter 1 |
| PERF-08 | Cinematic chapter 2 |
| PERF-09 | Cinematic chapter 3 |
| PERF-10 | Cinematic chapter 4 |
| PERF-11 | Cinematic chapter 5 |

Each scenario warms for at least 60 frames and 1 second, then samples for at
least 240 frames and 3 seconds. Sampling buffers are preallocated and capped
at 8,192 samples per metric.

## 1920x1080 Editor Diagnostic

The diagnostic ran at exact 1920x1080, PC quality, Direct3D 12 on:

- Intel Core i9-13900HX;
- NVIDIA GeForce RTX 4090 Laptop GPU;
- 64 GB system RAM;
- Windows 11.

| ID | Samples | Frame P95 | Frame P99 | Main P95 | GPU P95 | GC P95 |
|---|---:|---:|---:|---:|---:|---:|
| PERF-01 | 588 | 7.425 ms | 43.111 ms | 6.791 ms | 1.410 ms | 8,459 B |
| PERF-02 | 721 | 6.535 ms | 7.489 ms | 5.617 ms | 0.954 ms | 8,459 B |
| PERF-03 | 718 | 7.099 ms | 8.161 ms | 5.728 ms | 0.830 ms | 8,459 B |
| PERF-04 | 719 | 7.187 ms | 8.059 ms | 5.967 ms | 0.834 ms | 8,459 B |
| PERF-05 | 715 | 6.925 ms | 8.360 ms | 5.618 ms | 0.803 ms | 8,459 B |
| PERF-06 | 721 | 6.936 ms | 7.769 ms | 5.697 ms | 0.790 ms | 8,459 B |
| PERF-07 | 716 | 6.984 ms | 8.117 ms | 5.731 ms | 0.811 ms | 8,459 B |
| PERF-08 | 720 | 6.945 ms | 8.107 ms | 5.832 ms | 0.793 ms | 8,459 B |
| PERF-09 | 700 | 7.329 ms | 8.508 ms | 6.107 ms | 0.786 ms | 8,459 B |
| PERF-10 | 717 | 7.200 ms | 8.103 ms | 6.170 ms | 0.773 ms | 8,459 B |
| PERF-11 | 717 | 6.920 ms | 8.136 ms | 5.638 ms | 0.813 ms | 8,459 B |

The harness correctly classified every Editor scenario as `DiagnosticOnly`.
Ten of eleven frame-time P99 values were below the numeric release threshold.
The overview contained an Editor-side 43.111 ms P99 spike. Main-thread and GPU
P95 values remained below their numeric gates, but this is a faster development
machine and is not reference-hardware evidence.

The Editor reported approximately 8.4 KiB of managed allocation at P95 in
every scenario. Editor and harness overhead contaminate this value, so it
neither passes nor fails the player allocation gate. The standard `batches`
counter was unavailable; the standard draw-call, SRP Batcher draw-call,
SetPass, and triangle counters were available and retained in the JSON.

## Automated Validation

- Unity compilation: passed with zero errors and zero warnings.
- Edit Mode: 197 passed, 0 failed, 0 skipped, 0 inconclusive.
- Play Mode: 25 passed, 0 failed, 0 skipped, 0 inconclusive.
- Ordinary Play Mode remained free of performance-harness activation because
  no activation argument or Editor request was present.

## Remaining Certification Work

This slice does not claim release performance certification. That requires:

1. owner approval for a clean Windows standalone build;
2. the candidate commit SHA embedded in the capture;
3. a 1920x1080 player run on the approved mid-range hardware class;
4. external dedicated-application VRAM measurement;
5. external cold-process timing to the interactive state;
6. player-side investigation of any steady-state managed allocation;
7. a final pass/fail record linked to the release candidate.

## Candidate Result

The harness architecture, production-state routing, evidence schema, and
Editor diagnostic workflow are suitable for the next commit. Release
performance remains explicitly unverified until the standalone certification
work is approved and completed.
