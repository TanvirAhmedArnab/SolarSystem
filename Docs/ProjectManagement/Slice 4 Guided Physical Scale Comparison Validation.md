# Slice 4 Guided Physical Scale Comparison Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation date:** 2026-07-24  
**Unity:** 6000.5.3f1  
**URP:** 17.5.0  
**Status:** Implemented and validated; awaiting owner commit approval

## Scope

This candidate completes the approved guided physical-scale comparison without
changing the authoritative orbital data:

- `C` starts, advances, and finishes a deterministic three-stage sequence.
- Escape cancels safely from any active stage.
- The simulation pauses during comparison and restores its prior paused or
  running state afterward.
- Selection, time, navigation, focus, and zoom input are temporarily locked.
- The exact prior selection, camera pose, focus state, clip planes, time rate,
  and paused/running state are restored.
- The HUD identifies every transformation and explains why bodies become
  sub-pixel or invisible.
- One licensed CC0 UI cue confirms effective scale-stage changes.

## Scale stages

### Stage 1 — Readable overview

- Body radii remain exact Earth-relative proportions.
- Orbital distance remains logarithmically compressed.
- The whole authored system fits the portfolio overview.
- The caption explicitly distinguishes proportional radii from compressed
  orbit spacing.

### Stage 2 — Linear orbit spacing

- Radii and distances use one shared linear divisor:
  `37,658,725.03012079 km`.
- The divisor is the conservative gap between Venus perihelion and Mercury
  aphelion, derived from the authored JPL orbital elements.
- Orbit paths remain visible as teaching guides.
- Real bodies are allowed to fall below one pixel; no visible radius clamp is
  introduced.

### Stage 3 — Literal Earth-radius reference

- Earth's adopted mean radius, `6,371 km`, equals one unit.
- The average Earth-Moon distance is approximately `60.34` units.
- The average Earth-Sun distance is approximately `23,481.13` units.
- Earth becomes the temporary float-space render origin.
- The camera frames the literal Earth-Sun relationship while Neptune is
  intentionally outside that frame.

## Architecture

`GuidedScaleComparisonService` owns only the deterministic stage and simulation
pause policy. It uses `IScaleModeController` rather than referencing the Unity
scene controller directly.

`GuidedScaleComparisonInputController` coordinates application adapters:

- scale projection and orbit-path rebuild;
- guided camera poses and exact pre-guide state restoration;
- temporary selection/time/camera command locking;
- HUD presentation and comparison audio through state-change events.

`CelestialScaleProjector` remains the only physical-to-presentation conversion
boundary. The immutable physical catalog and simulation state are never
rewritten by a comparison stage.

## Automated validation

| Suite | Passed | Failed | Skipped | Inconclusive |
|---|---:|---:|---:|---:|
| Edit Mode | 112 | 0 | 0 | 0 |
| Play Mode | 8 | 0 | 0 | 0 |

Coverage verifies:

- all three projection contracts and their shared-scale equality;
- Mercury-Venus reference derivation from authored elements;
- literal Earth render-origin hierarchy composition;
- deterministic service ordering and effective-change event counts;
- completion and cancellation restoring the prior paused state;
- `C` binding and generated Input System asset contract;
- HUD element/style contracts and scale-mode text;
- comparison cue import, scene wiring, and event routing;
- real-scene stage traversal, camera framing, orbit-line policies, Earth origin,
  and restoration of selection, time, and camera state.

## Live Unity inspection

The three stages were advanced in Play Mode through the initialized application
service so each rendered state could be inspected without altering serialized
assets.

- The comparison teaching card remains fully within the 16:9 Game view.
- Stage title, numeric reference, explanation, progress, and next/exit controls
  are visually distinct and readable.
- The selected-body and quick-control cards hide during comparison; the
  top-left status card remains visible.
- The simulation visibly reports paused while the guide is active.
- Linear-stage bodies become appropriately sub-pixel while orbit guides remain
  subordinate and readable.
- Finishing the sequence restores the selected Sun, its information card, the
  prior overview framing, readable scale, and running simulation.

## Final Unity result

- Compilation: passed
- Edit Mode: `112 passed`, `0 failed`, `0 skipped`, `0 inconclusive`
- Play Mode: `8 passed`, `0 failed`, `0 skipped`, `0 inconclusive`
- Final Console: `0 errors`, `0 warnings`
- Editor state: Edit Mode

## Repository preflight

- Staged files: `56`
- Generated paths: `0`
- Staged files over 5 MB: `0`
- Tracked repository files over 5 MB: `2`; both copies of the approved Sun
  ambience are governed by Git LFS.
- Missing Unity `.meta` files: `0`
- Orphan Unity `.meta` files: `0`
- Secret-pattern matches: `0`
- Merge-conflict markers: `0`
- `git diff --cached --check`: passed
- `git lfs fsck --pointers`: passed

The unrelated Unity serialization changes in
`ProjectSettings/PackageManagerSettings.asset`,
`ProjectSettings/ProjectSettings.asset`, and
`ProjectSettings/URPProjectSettings.asset` must remain deliberately unstaged.

The candidate is staged for owner review. No commit or push has been performed
for this goal.
