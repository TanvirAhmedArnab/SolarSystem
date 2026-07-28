# Release Settings and Build Automation Validation

**Date:** 2026-07-26  
**Owner:** Tanvir  
**Status:** Implemented and validated; synchronized three-platform rebuild
pending clean commit and push

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
  domain reloads, waits for compilation/import work, observes the final build
  target, and then requests the approved standalone target asynchronously.
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

The macOS Universal builder was regenerated from clean, pushed hardening
commit `bf807333`. The build succeeded with zero errors and produced a valid
`.app` bundle whose launcher contains both Intel x86-64 and Apple silicon arm64
slices. The artifact remains unsigned, unnotarized, and untested because no
Mac test device or Apple Developer membership is available.

Unity counted one non-blocking `Hidden/Core/DebugOccluder` shader warning from
the installed render-pipeline package. The previous deferred-target-switch
diagnostic did not recur. The editor returned automatically to
`StandaloneWindows64` with IL2CPP, architecture `0`, no pending restoration,
and a zero-warning/error Console.

See:

`Docs/Release/macOS Universal Build Validation.md`

## Target-restoration hardening validation

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

The fix was committed and pushed as `bf807333`, then exercised by the clean
macOS rebuild described above. The final Windows, WebGL, and macOS publication
artifacts must still be regenerated from one final release commit after the
remaining release work is complete.

## Ordered all-platform hardening validation

The first synchronized attempt from clean pushed commit `c130637` began while
the Editor's active target was WebGL. Its Windows player completed successfully,
but the ordered command then stopped before macOS because the individual
desktop builder tried to restore WebGL through a standalone-only coordinator.
That diagnostic Windows artifact is not a release artifact.

Review also proved that invoking the three public build commands sequentially
would create two additional risks: Unity's expected platform-import
serialization churn could fail the second command's clean-tree precondition,
and an intermediate asynchronous restoration request could race the following
platform build.

The corrected architecture:

- validates settings, clean source, and pushed upstream identity once before
  any platform mutation;
- performs Windows, macOS, and WebGL builds inside one owned sequence;
- restores standalone scripting backend and architecture once in a final
  block;
- records WebGL as the final target that must be observed before restoration;
- preserves a previous standalone target, or uses
  `StandaloneWindows64` when the command started from WebGL; and
- retains the domain-reload-safe asynchronous switch after the final build.

The navigator's obsolete `PreventDefault()` call was also removed while
`StopImmediatePropagation()` continues to consume an activated Enter event.
The existing real-scene test confirms that Enter still selects, focuses, and
closes the navigator. The focused fast-moon reticle regression now allows one
UI pixel of layout rounding while still sampling all 20 maximum-speed frames.

Candidate validation:

- Unity compilation: passed with no compiler warning.
- Edit Mode: `226 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Play Mode: `27 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Unity Console: `0 errors`, `0 warnings`.
- Focused restoration tests cover standalone preservation, WebGL-to-Windows
  normalization, final-WebGL observation, busy-editor waiting, safe switching,
  and already-restored completion.

The corrected command still requires a clean commit and push before the
synchronized all-platform build can be repeated.
