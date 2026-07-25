# Slice 4 Mars Hero Rendering Validation

**Date:** 2026-07-24  
**Owner:** Tanvir  
**Candidate:** `feat(visuals): add Mars hero rendering`  
**Status:** Implemented and validated; awaiting owner approval before commit

## Outcome

Mars now presents as a source-grounded rocky world with restrained rust,
ochre, basalt, and polar contrast plus one thin atmospheric limb. The
implementation extends the proven layered-body architecture to an explicit
atmosphere-only mode, so the real scene contains no invented Mars cloud shell.

## Reviewed Contract

- Physical surface radius remains the exact Earth-relative ratio derived from
  `3,389.5 km`.
- The deterministic analytical orbit, `25.2 degree` axial tilt, and positive
  `1.02595676-day` sidereal rotation remain unchanged.
- `TEX-SSS-010` remains anchored to the proportional surface. It does not
  slide, rotate independently, or receive a time-driven shader phase.
- The rocky shader uses the center source sample plus four neighboring
  luminance samples to derive restrained tangent-space relief.
- Dry-surface response is bounded to `0.025` specular and `0.10` smoothness.
- A single atmosphere shell at `1.008` surface radius uses the shared
  Sun-aware rim shader with `5.2` falloff power, `0.16` intensity, and `0.025`
  nightside visibility.
- Mars has no cloud GameObject, material, renderer, cloud-rate state, or
  cloud-update work.
- Surface and atmosphere renderers cast no shadows and use no light or
  reflection probes. Only the atmosphere contributes transparent overdraw.
- Both materials retain GPU instancing support.

The shell thickness, enhanced source color, and relief response are
presentation choices. They are not claims about atmospheric pressure,
composition, scale height, weather, dust storms, scattering, or date-exact
appearance.

## Architecture

`CelestialLayerVisualDefinition` now stores an explicit `hasCloudLayer` flag.
Startup converts the asset to immutable `CelestialLayerVisualModel` state.
`CelestialLayeredBodyView` conditionally validates and updates cloud
dependencies only when that flag is true. Earth and Venus retain their
existing cloud behavior; Mars shares the same composition boundary with the
flag false and null cloud references.

The project-owned `SolarSystem/Celestial/Rocky Surface` shader preserves the
approved texture as the primary color source and derives relief without an
additional normal-map asset. It has no runtime adapter, material property
block, time phase, or steady-state allocation. The deterministic editor
builder creates the Mars definition, materials, atmosphere hierarchy, and
renderer policy reproducibly.

## Automated Validation

### Edit Mode

- Result: `132 passed, 0 failed, 0 skipped, 0 inconclusive`.
- Final duration: `4.315 seconds`.
- Coverage includes immutable atmosphere-only validation, conditional view
  dependencies, exact shell scale, anchored surface assignment, shader
  parameters, importer settings, material instancing, and reproducible asset
  contracts.

### Play Mode

- Result: `14 passed, 0 failed, 0 skipped, 0 inconclusive`.
- Final duration: `17.371 seconds`.
- The real `SolarSystem` scene validates Mars's exact proportional radius,
  atmosphere-only hierarchy, surface and limb shaders, shadow/probe policy,
  live Sun direction, close-focus visibility, and preservation of the user's
  simulation-time state.

One intermediate rerun was interrupted because a forced script refresh
overlapped Play Mode teardown. Unity was returned to clean Edit Mode, the
pending bridge state was cleared, and the suite was rerun without an
overlapping refresh. The final complete result above supersedes that
interrupted runner state.

## Live Unity Review

The rebuilt scene was inspected in system overview and close focus.

- The final color balance reads as restrained dark rust and ochre rather than
  emissive red.
- Polar brightness and dark basalt regions remain recognizable.
- Relief adds close-focus definition without obvious seams or UV motion.
- The illuminated hemisphere follows the live Sun direction.
- The atmosphere remains a narrow warm limb and does not read as a cloud deck
  or a large uniform halo.
- Selection reticle, responsive HUD, information panel, camera focus, and
  quick controls remain readable around the focused body.

## Shader and Compilation Validation

- `SolarSystem/Celestial/Rocky Surface`: supported, `0` compiler messages.
- `SolarSystem/Celestial/Atmosphere Rim`: supported, `0` compiler messages.
- Project script compilation: pass.
- Final product test suites: `0` failures.
- Unity AI MCP process-validation diagnostics remain external tooling warnings
  and are not emitted by SolarSystem runtime or shader code.

## Licensing and Provenance

- `TEX-SSS-010` source and Unity derivative SHA-256:
  `2D187F3E77A98EAA8CEA5F4CC722F633C122EF170B9E94ACE6B5FB6CBC3F8E01`.
- The source and derivative hashes match; texture pixels are unchanged.
- The texture remains licensed by Solar System Scope under CC BY 4.0 with
  release attribution required.
- The authored tint, rocky shader, layer architecture, atmosphere material,
  tests, and scene wiring are project-authored and add no new third-party
  license.

## Repository Preflight

- Candidate scope: `29` staged paths.
- `git diff --cached --check`: pass before final evidence update.
- Generated Unity, IDE, build, and temporary path matches: `0`.
- Strong-signature secret matches across the staged tree: `0`.
- Missing Unity asset/`.meta` partners: `0`.
- Duplicate Unity GUIDs: `0`.
- Staged files at or above `1 MiB`: `0`.
- Staged binary diff entries: `0`.
- `git lfs fsck --pointers`: pass.
- Scene semantic review: exactly one new GameObject named `Atmosphere Layer`
  plus its Transform, MeshFilter, MeshRenderer, and
  `CelestialLayeredBodyView`; total serialized-object delta is `+5`.
- Existing Earth and Venus layer assets gain only the explicit
  `hasCloudLayer: true` field; their layer behavior and values are unchanged.
- The unrelated local changes in `PackageManagerSettings.asset`,
  `ProjectSettings.asset`, and `URPProjectSettings.asset` remain unstaged.

## Remaining Limits

- The treatment is not a scientific topography, elevation, or atmospheric
  simulation.
- Atmosphere thickness is presentation exaggeration and does not change the
  physical radius used by simulation or the selected-body information panel.
- Dynamic dust, clouds, weather, volumetric scattering, and surface
  self-shadowing are intentionally outside this slice.
- Final whole-project profiling and release capture remain later Slice 4/5
  work.
