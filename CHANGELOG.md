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
- Cancellable three-stage guided scale comparison covering the readable
  overview, one shared linear orbit unit, and literal Earth-radius scale.
- Guided camera framing, explicit transformation captions, scale-mode HUD
  status, comparison audio feedback, and exact explorer-state restoration.
- Edit Mode conversion/service/input/UI/audio coverage and a real-scene
  Play Mode journey across all comparison stages.
- Project-owned URP Earth surface, cloud, and atmosphere shaders using the
  approved day, normal, specular, night, and cloud source textures.
- Immutable layered-body authoring, deterministic absolute cloud drift, and a
  shared allocation-free live-Sun shader position.
- Edit Mode layer/material/import contracts and a real-scene Play Mode journey
  for hierarchy, shell scale, day/night policy, cloud motion, focus visibility,
  and paused-state preservation.
- Project-owned URP solar surface and corona shaders that reuse the approved
  Sun texture with subtle deterministic, absolute-time motion.
- Immutable solar visual authoring, a cached allocation-free property-block
  adapter, and reproducible Sun surface/corona scene wiring.
- Edit Mode solar authoring/material/shader contracts and real-scene Play Mode
  coverage for hierarchy, phase progression, pause, focus, renderer policy,
  and separation from the Sun-origin radial light.
- Reusable immutable gas-giant authoring plus an allocation-free runtime
  property-block adapter driven by authoritative absolute simulation time.
- Project-owned URP Jupiter surface and atmosphere shaders that preserve the
  anchored approved texture, add restrained source-derived band relief, and
  keep transparent limb overdraw bounded to one shell.
- Edit Mode gas-giant authoring/material/import contracts and real-scene Play
  Mode coverage for hierarchy, exact proportional radius, deterministic phase,
  pause, Sun-origin lighting, focus, renderer policy, and state preservation.

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
- Replaced Earth's baseline Lit material with the representative layered
  treatment and disclosed that cloud/atmosphere thickness is exaggerated
  without changing the proportional surface radius.
- Replaced the Sun's static baseline material with an exposure-safe hero
  treatment and a restrained non-shadow-casting corona; live evidence did not
  justify adding a lens flare.
- Replaced Jupiter's generic Lit baseline with a reusable gas-giant hero
  treatment while retaining the existing texture pixels, Great Red Spot,
  scientific data, scale, orbit, and signed rotation.
- Suppressed cached orbit guides only during close camera focus and restored
  them in free flight so overview geometry cannot obscure hero views.
- Paused simulation and temporarily locked selection, time, and navigation
  commands during the guided scale comparison, restoring their exact prior
  state when the sequence finishes or is cancelled.

### Removed

- Unity template tutorial and readme content.
- Self-deleting package-resolution bootstrap files.
- Mobile-only quality tier and URP assets.
- Generated IDE solution file from version control.

[Unreleased]: https://github.com/TanvirAhmedArnab/SolarSystem/commits/main
