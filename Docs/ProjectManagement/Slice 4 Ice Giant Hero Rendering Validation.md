# Slice 4 Ice Giant Hero Rendering Validation

**Date:** 2026-07-24  
**Owner:** Tanvir  
**Candidate:** `feat(visuals): add Uranus and Neptune hero rendering`  
**Status:** Implemented and validated; awaiting owner approval before commit

## Outcome

Uranus and Neptune now use a reusable, scientifically named ice-giant
presentation path with distinct anchored source identities, one restrained
Sun-aware atmosphere shell each, and bounded deterministic detail. Their
scientific radii, analytical orbits, axial tilts, and signed rotations remain
unchanged.

## Reviewed Contract

- Uranus remains `25,362 km` in the scientific model and
  `25,362 / 6,371.0084` Earth radii in the rendered proportional scale.
- Neptune remains `24,622 km` and
  `24,622 / 6,371.0084` Earth radii.
- Uranus retains its `97.77 degree` axial tilt and negative `0.71833-day`
  sidereal period; Neptune retains its `28 degree` educational tilt and
  positive `0.67125-day` period.
- `TEX-SSS-014` and `TEX-SSS-015` remain anchored to their respective
  surfaces. Neither texture is replaced, merged, or shared across bodies.
- Uranus uses a `1.009` atmosphere shell, `0.07` source-derived band normal,
  `0.012` moving-detail contribution, `0.0002` detail cycles per signed
  rotation, `0.12` rim intensity, and `0.035` nightside readability.
- Neptune uses a `1.01` shell, `0.16` source-derived band normal, `0.035`
  moving-detail contribution, `0.0009` detail cycles, `0.17` rim intensity,
  and `0.04` nightside readability.
- The readability term uses anchored source color only on the live-Sun
  nightside. It adds no second light and preserves the terminator.
- Surface and atmosphere renderers cast no shadows and use no light or
  reflection probes. Each body adds only one transparent shell.
- All giant-planet materials retain GPU instancing support.

The shell sizes, relief, moving samples, tints, and readability floors are
presentation values. They are not measured wind speeds, fluid velocities,
scale heights, atmosphere boundaries, chemistry, volumetric scattering,
exact photometry, or date-specific weather.

## Architecture

`IceGiantVisualDefinition` stores reviewed body ID, shell scale, and signed
presentation-detail rate. Startup converts it to immutable
`IceGiantVisualModel` state. `IceGiantVisualView` evaluates absolute
simulation time against the authoritative signed rotation period and writes
one phase through a cached `MaterialPropertyBlock`; it does not accumulate
frame delta, instantiate materials, or allocate in steady-state updates.

Uranus and Neptune share the project-owned `Giant Planet Surface` and
`Giant Planet Atmosphere` shaders with Jupiter and Saturn while retaining
separate definitions and materials. The deterministic editor builder creates
both definitions, atmosphere materials, shell GameObjects, view wiring, and
renderer policy reproducibly.

## Automated Validation

### Edit Mode

- Result: `137 passed, 0 failed, 0 skipped, 0 inconclusive`.
- Final duration: `4.981 seconds`.
- Coverage includes immutable model validation, stable-ID matching,
  deterministic prograde/retrograde phase evaluation, renderer policy,
  anchored source assignment, distinct material constants, shell scales,
  importer settings, and GPU instancing.

### Play Mode

- Result: `15 passed, 0 failed, 0 skipped, 0 inconclusive`.
- Final duration: `17.247 seconds`.
- The real `SolarSystem` scene validates both bodies' exact proportional
  radii, distinct hierarchy/material identity, source rotation signs, axial
  tilts, shell scales, phase direction, pause behavior, Sun-origin lighting,
  close focus, renderer policy, and property-block state.

## Live Unity Review

Both bodies were inspected in the running Game view at close focus.

- Uranus reads as muted cyan with very low-contrast source detail and a thin
  pale limb; its nightside remains dark but identifiable.
- Neptune reads as a distinct controlled deep blue with a stronger but still
  restrained band response; its nightside does not become electric blue.
- The live-Sun terminator remains visible on both bodies.
- The selection reticle, responsive HUD, information card, and quick controls
  remain readable at the validated 16:9 Game-view size.
- Direct review motivated the small nightside readability floor; the first
  build was scientifically directional but too dark to preserve portfolio
  source identity at the deterministic focus angles.

## Shader and Compilation Validation

- `SolarSystem/Celestial/Giant Planet Surface`: supported, `0` compiler
  messages.
- `SolarSystem/Celestial/Giant Planet Atmosphere`: supported, `0` compiler
  messages.
- Project script compilation: pass.
- Final product test suites: `0` failures.

## Licensing and Provenance

- Uranus source and derivative SHA-256:
  `D15239D46F82D3EA13D2B260B5B29B2A382F42F2916DAE0694D0387B1204A09D`.
- Neptune source and derivative SHA-256:
  `CB42EA82709741D28B0AF44D8B283CBC6DBD0C521A7F0E1E1E010ADE00977DF6`.
- Both source/derivative pairs match; texture pixels are unchanged.
- Both textures remain Solar System Scope CC BY 4.0 media with release
  attribution required.
- All architecture, shaders, material configuration, tests, and scene wiring
  added by this slice are project-authored and introduce no new third-party
  license.

## Repository Preflight

- Candidate scope: `40` staged paths.
- `git diff --cached --check`: pass.
- Generated Unity, IDE, build, and temporary path matches: `0`.
- Strong-signature secret matches across the staged tree: `0`.
- Missing Unity asset/`.meta` partners: `0`.
- Duplicate Unity GUIDs: `0`.
- Staged files at or above `1 MiB`: `0`.
- Staged binary diff entries: `0`.
- `git lfs fsck --pointers`: pass.
- Scene semantic review: exactly two new `Atmosphere Layer` GameObjects,
  two Transforms, two MeshFilters, two MeshRenderers, and two
  `IceGiantVisualView` components; total serialized-object delta is `+10`.
  The ten existing `CelestialBodyView` records gain the optional
  `iceGiantVisualView` field, with non-ice-giant references remaining null.
- Existing Jupiter and Saturn materials gain only their explicit shared-shader
  nightside-readability values; their definitions, geometry, source textures,
  shell scales, orbits, tilts, and signed rotations are unchanged.
- The unrelated local changes in `PackageManagerSettings.asset`,
  `ProjectSettings.asset`, and `URPProjectSettings.asset` remain unstaged.

## Remaining Limits

- No physical atmospheric-fluid, wind, cloud, storm, chemistry, scattering,
  oblateness, or date-exact appearance is simulated.
- Uranus rings and planetary moons are outside this slice.
- Atmosphere-shell thickness and nightside visibility are presentation
  exaggerations and do not change simulation radius or selected-body facts.
- Final whole-project profiling and release capture remain later Slice 4/5
  work.
