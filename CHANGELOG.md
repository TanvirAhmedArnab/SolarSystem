# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html) for public releases.

## [Unreleased]

### Added

- Living GDD, TDD, Art Bible, package baseline, and project validation records.
- Project-authored runtime, editor, Edit Mode test, and Play Mode test assembly boundaries.
- Unity-aware repository attributes, Git LFS rules, and generated-file ignores.
- Initial Edit Mode assembly-foundation smoke test.
- Initial licensed texture, ambient-audio, music, and UI-sound working set with source provenance.
- Immutable celestial value types, validated parent-first catalog, and authoritative simulation clock.
- Deterministic analytical Kepler orbit, hierarchy, speed, and signed-rotation evaluation with Edit Mode coverage.
- Serialized Sun, Earth, and Moon definitions with audited scientific source records.
- Verified Jupiter definition, textured material, deterministic orbit, and cached orbit path.
- Unity coordinate and presentation-scale adapters with allocation-free catalog evaluation.
- Centralized composition, simulation controller, body views, and cached orbit-path views.
- Reproducible `SolarSystem` graybox scene with textured bodies, camera, lighting, and volume.
- Edit Mode authoring/projection coverage and real-scene Play Mode motion/pause validation.
- Project-owned `Explorer` Input System map for reviewed keyboard and mouse controls.
- Stable-ID selection service and celestial-body raycast selection colliders.
- Damped free-flight camera plus smooth, redirectable focus/orbit behavior using unscaled time.
- Interaction composition root with Edit Mode contract tests and a real-scene Play Mode journey.
- Bounded simulation pause/speed application service with a documented one-Earth-sidereal-rotation-per-real-second baseline.
- UI Toolkit explorer HUD showing run state, rate, selection, and contextual keyboard hints.
- Edit Mode time/UI asset coverage and a real-scene HUD feedback and resumed-motion journey.
- Authored educational summaries for the representative Sun, Earth, Moon, and Jupiter definitions.
- Selected-body information card with verified physical/orbital values, explicit units, scale disclosure, and source-record provenance.
- Screen-space four-corner selection reticle that follows the selected body's projected presentation radius.
- Edit Mode fact-formatting and UI-contract coverage plus real-scene selected-body panel validation.
- Color-coded quick-control keycaps with independently readable action labels and contextual pause/resume feedback.
- Project-owned panoramic space skybox using the approved 2K Milky Way texture.
- Focused `VP_SolarSystem` URP profile with ACES, restrained bloom, fixed color/exposure shaping, and subtle vignette.
- In-place visual-foundation builder that preserves scene identities during rendering iteration.
- Edit Mode visual-asset contracts and a real-scene Play Mode rendering-foundation test.
- Real-scene regression coverage for Sun parenting, radial-light units,
  co-location, range, and shadow policy.
- Serialized scientific definitions, materials, deterministic orbits, body
  views, and orbit paths for Mercury, Venus, Mars, Saturn, Uranus, and Neptune.
- Generated 128-segment Saturn annulus with the audited ring alpha texture and
  two-sided transparent presentation.
- Full-system overview framing and regression coverage for all ten authored
  bodies, nine orbit paths, the radial-light envelope, and Saturn's ring wiring.
- Event-driven audio direction with independently adjustable master, music, UI,
  and celestial levels plus a non-destructive master mute.
- Licensed CC0 music, 2D Sun ambience, 3D Earth ambience, and UI feedback for
  selection, focus, and simulation-time changes.
- Deterministic audio import policies and Edit/Play Mode regression coverage for
  routing, spatialization, event mapping, and scene wiring.
- Shared Earth radius and sidereal-rotation reference units for presentation
  scale and simulation-time calibration.
- Exact Earth-relative body radii, a reusable literal physical-length
  conversion, and a compressed-overview clearance contract.
- Complete deterministic synodic-cycle sampling for every adjacent planet pair,
  plus Sun-Mercury, Earth-Moon, signed-spin, and scene-integration coverage.

### Changed

- Reduced the direct Unity package set to the approved project baseline.
- Configured the PC quality tier and PC URP asset as the sole release baseline.
- Recorded intentional Unity project identity and current serialization state.
- Enabled the intentional `SolarSystem` scene as the sole build scene.
- Separated Slice 2 editor orchestration, asset authoring, build data, and scene construction without changing the visible or serialized domain contract.
- Renamed the evolving graybox catalog to `Catalog_SolarSystem` and validated the provisional scale across Earth, Jupiter, and the Sun.
- Expanded the reproducible graybox builder to author and wire the Slice 3 interaction graph.
- Advanced the project input contract to include Space and bracket-key time commands.
- Set the provisional scene default to the documented `10x` rate preset.
- Replaced the runtime scene's Unity template volume dependency with
  project-owned skybox, post-processing, ambient-light, reflection, and solar-key settings.
- Tuned the representative Sun, Earth, Moon, Jupiter, and orbit materials;
  enabled instancing and added subtle linear normal detail to Earth.
- Replaced the fixed directional presentation light with a Sun-parented
  realtime point source so every lit body derives its day hemisphere from the
  live Sun position.
- Made volume-profile authoring idempotent by reusing valid component
  subassets instead of replacing their stable local file IDs on every run.
- Replaced fixed camera-test sleeps with bounded state-based waits to prevent
  false failures under variable editor load.
- Replaced per-body scene wiring with one ordered editor content collection so
  catalog, material, view, and orbit authoring scale together.
- Reframed the initial camera from the representative four-body view to the
  complete eight-planet presentation envelope.
- Replaced radius exponentiation and clamping with strict proportional sizing
  from an `Earth = 1` reference; retained compressed orbital spacing with a
  disclosed minimum readable surface gap.
- Changed the `1x` time reference to Earth's `86,164.2`-second sidereal
  rotation, so one real second produces one Earth rotation and every other
  body's signed spin remains proportional.
- Enlarged only invisible selection colliders for sub-pixel bodies, preserving
  proportional rendered geometry while keeping the overview usable.

### Removed

- Unity template tutorial and readme content.
- Self-deleting package-resolution bootstrap files.
- Mobile-only quality tier and URP assets.
- Generated IDE solution file from version control.

[Unreleased]: https://github.com/TanvirAhmedArnab/SolarSystem/commits/main
