# Solar System Simulation

Solar System Simulation is a polished Unity portfolio project that turns verified planetary data and deterministic analytical orbital motion into an accessible, cinematic exploration of our Solar System.

The project is currently in visual/content development. Its product, engineering, art, licensing, and repository decisions are maintained as living, reviewable documents rather than being left implicit in scenes or code.

## Portfolio goals

- Present the Sun, eight planets, and selected major moons with honest educational scaling.
- Demonstrate deterministic orbital mechanics without Rigidbody-driven orbits.
- Support free-fly and cinematic exploration with clear scientific overlays.
- Add restrained pooled comet fly-throughs without colliders or physics-driven
  orbital claims.
- Provide first-launch orientation plus a unified Help, Settings, and Credits
  experience with persistent audio, motion, orbit-guide, and label preferences.
- Deliver a stable 60 FPS experience at 1080p on a reasonable mid-range gaming PC.
- Show professional Unity architecture, testing, documentation, licensing, and Git practices.

## Approved technical baseline

- Unity `6000.5.3f1`
- Universal Render Pipeline `17.5.0`
- Windows 10/11 x86-64
- Keyboard and mouse
- ScriptableObject-authored celestial data
- Double-precision domain calculations with float-space rendering
- Lightweight trunk-based development with short-lived feature branches
- Conventional Commits and explicit owner approval before every commit or push

## Repository map

```text
Assets/SolarSystem/        Project-authored Unity content, runtime, editor, and tests
Docs/Design/               Living product-design authority
Docs/Technical/            Living technical authority and package baseline
Docs/Art/                  Living visual and content-production authority
Docs/Legal/                Third-party provenance and licensing ledger
Docs/ProjectManagement/    Audits, validation evidence, and approval checklists
SourceAssets/              Licensed source files retained outside Unity's Assets tree
Tools/                     Reproducible project tooling
```

## Open the project

1. Install Unity Hub and Unity Editor `6000.5.3f1` with Windows build support.
2. Clone the repository with Git LFS installed: `git lfs install`, then
   `git clone https://github.com/TanvirAhmedArnab/SolarSystem.git`.
3. Add the cloned folder in Unity Hub and open it with the exact editor version above.
4. Allow Package Manager and asset import to finish before evaluating the Console.
5. Confirm the Console has no errors or warnings, then run the Edit Mode tests in Test Runner.

Generated folders such as `Library`, `Temp`, `Logs`, `Obj`, `UserSettings`, and IDE project files are intentionally not versioned.

## Documentation

- [Game Design Document](Docs/Design/GDD.md)
- [Technical Design Document](Docs/Technical/TDD.md)
- [Keyboard and Mouse Controls](Docs/Design/Controls.md)
- [Art Bible](Docs/Art/ArtBible.md)
- [Unity Package Baseline](Docs/Technical/Unity%20Package%20Baseline.md)
- [Third-Party Assets and Licensing](Docs/Legal/ThirdPartyAssets.md)
- [Celestial Data Sources](Docs/Science/Celestial%20Data%20Sources.md)
- [Pre-First-Project-Commit Checklist](Docs/ProjectManagement/Pre-First-Project-Commit%20Checklist.md)
- [Slice 1 Deterministic Simulation Validation](Docs/ProjectManagement/Slice%201%20Deterministic%20Simulation%20Validation.md)
- [Slice 2 Sun-Earth-Moon Validation](Docs/ProjectManagement/Slice%202%20Sun%20Earth%20Moon%20Validation.md)
- [Slice 2 Jupiter Scale Validation](Docs/ProjectManagement/Slice%202%20Jupiter%20Scale%20Validation.md)
- [Slice 3 Interaction Proof Validation](Docs/ProjectManagement/Slice%203%20Interaction%20Proof%20Validation.md)
- [Slice 3 Simulation Time and HUD Validation](Docs/ProjectManagement/Slice%203%20Simulation%20Time%20and%20HUD%20Validation.md)
- [Slice 3 Selection and Body Information Validation](Docs/ProjectManagement/Slice%203%20Selection%20and%20Body%20Information%20Validation.md)
- [Slice 4 Visual Foundation Validation](Docs/ProjectManagement/Slice%204%20Visual%20Foundation%20Validation.md)
- [Slice 4 Sun-Origin Illumination Validation](Docs/ProjectManagement/Slice%204%20Sun-Origin%20Illumination%20Validation.md)
- [Slice 4 Eight-Planet Content Validation](Docs/ProjectManagement/Slice%204%20Eight-Planet%20Content%20Validation.md)
- [Slice 4 Audio Baseline Validation](Docs/ProjectManagement/Slice%204%20Audio%20Baseline%20Validation.md)
- [Slice 4 Proportional Scale Calibration Validation](Docs/ProjectManagement/Slice%204%20Proportional%20Scale%20Calibration%20Validation.md)
- [Slice 4 Guided Physical Scale Comparison Validation](Docs/ProjectManagement/Slice%204%20Guided%20Physical%20Scale%20Comparison%20Validation.md)
- [Slice 4 Layered Earth Rendering Validation](Docs/ProjectManagement/Slice%204%20Layered%20Earth%20Rendering%20Validation.md)
- [Slice 4 Solar Surface and Corona Validation](Docs/ProjectManagement/Slice%204%20Solar%20Surface%20and%20Corona%20Validation.md)
- [Slice 4 Jupiter Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Jupiter%20Hero%20Rendering%20Validation.md)
- [Slice 4 Saturn Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Saturn%20Hero%20Rendering%20Validation.md)
- [Slice 4 Venus Atmosphere Rendering Validation](Docs/ProjectManagement/Slice%204%20Venus%20Atmosphere%20Rendering%20Validation.md)
- [Slice 4 Mars Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Mars%20Hero%20Rendering%20Validation.md)
- [Slice 4 Ice Giant Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Ice%20Giant%20Hero%20Rendering%20Validation.md)
- [Slice 4 Airless Rocky Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Airless%20Rocky%20Hero%20Rendering%20Validation.md)
- [Slice 4 Major Moon Content Validation](Docs/ProjectManagement/Slice%204%20Major%20Moon%20Content%20Validation.md)
- [Slice 4 Titan Haze Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Titan%20Haze%20Hero%20Rendering%20Validation.md)
- [Slice 4 Io and Europa Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Io%20and%20Europa%20Hero%20Rendering%20Validation.md)
- [Slice 4 Ganymede and Callisto Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Ganymede%20and%20Callisto%20Hero%20Rendering%20Validation.md)
- [Slice 4 Triton Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Triton%20Hero%20Rendering%20Validation.md)
- [Slice 4 Celestial Navigator and World Labels Validation](Docs/ProjectManagement/Slice%204%20Celestial%20Navigator%20and%20World%20Labels%20Validation.md)
- [Slice 4 Cinematic Tour Validation](Docs/ProjectManagement/Slice%204%20Cinematic%20Tour%20Validation.md)
- [Slice 4 Cinematic Tour Polish Validation](Docs/ProjectManagement/Slice%204%20Cinematic%20Tour%20Polish%20Validation.md)
- [Repository Publication Cleanup Validation](Docs/ProjectManagement/Repository%20Publication%20Cleanup%20Validation.md)
- [Explorer Menu and Settings Validation](Docs/ProjectManagement/Explorer%20Menu%20and%20Settings%20Validation.md)
- [Performance Profiling Harness Validation](Docs/ProjectManagement/Performance%20Profiling%20Harness%20Validation.md)
- [Assignment and Publication Readiness Audit](Docs/ProjectManagement/Assignment%20and%20Publication%20Readiness%20Audit.md)
- [Assignment Comet Spawner Validation](Docs/ProjectManagement/Assignment%20Comet%20Spawner%20Validation.md)
- [Readable Initial Orbit Composition Validation](Docs/ProjectManagement/Readable%20Initial%20Orbit%20Composition%20Validation.md)
- [Proposed itch.io Page Copy](Docs/Release/itch.io%20Page%20Draft.md)
- [Release Evidence Checklist](Docs/Release/Release%20Evidence%20Checklist.md)
- [Release Player Settings Proposal](Docs/Release/Release%20Player%20Settings%20Proposal.md)

## Current validation state

The deterministic simulation, full Sun/eight-planet/seven-moon baseline,
Slice 3 interaction vertical slice, and first visual-production foundation
compile successfully. The project currently passes 204 Edit Mode cases and
26 real-scene Play Mode cases. Coverage includes orbital behavior,
interaction, UI, selected-body facts, project-owned skybox and URP profile
contracts, audited planet textures and materials, generated Saturn rings,
full-system overview framing, Sun-origin radial illumination, and preserved
real-scene behavior. Every rendered body radius is now a strict multiple of
Earth's mean radius (`Earth = 1`), while orbital distances use a disclosed
readability compression with tested adjacent-orbit clearances. At `1x`, one
real second advances exactly one Earth sidereal rotation; all bodies derive
their spin rate and direction from signed source periods. The scene also
opens at one shared J2000-plus-4,904-day deterministic presentation epoch,
which distributes all eight planet directions without changing their source
orbital elements or claiming current-date positions. It also
includes licensed event-driven music, spatial celestial ambience, and UI
feedback with independent runtime levels and mute. A deterministic rotating
spawner reuses six collider-free comet instances, producing restrained
project-authored trails and automatically returning expired or safely
off-camera comets to the pool. These fly-throughs are illustrative rather than
date-specific scientific comet trajectories. A cancellable three-stage
comparison now teaches the scale problem by moving from the readable overview
to one shared linear orbit unit and then to literal `Earth radius = 1`
spacing, while preserving the prior selection, time, audio, and camera state.
A deterministic five-chapter cinematic tour now reuses the same simulation,
camera, selection, navigation, time, audio, and UI services to visit the Sun,
Earth-Moon system, Jupiter system, Saturn's rings, and the outer system. It
supports `T`, Escape, and visible mouse controls, keeps scientific motion
running, prevents conflict with scale comparison, and restores the exact prior
explorer state relative to any live focused target. Per-chapter composition and
easing are authored as data; `M` or the visible Motion control switches between
full eased travel and a persisted instant reduced-motion mode. Tour-only orbit
guide suppression and renderer spotlighting preserve visual hierarchy while
restoring every prior visibility state on completion or cancellation.
A unified Explorer Menu now provides first-launch orientation, reopenable Help,
persistent Settings, and Credits & Sources. `H` opens Help, `O` toggles orbit
guides, and Escape follows one contextual route instead of competing handlers.
Master/music/interface/celestial volumes, mute, reduced motion, orbit guides,
and body labels persist locally in a versioned record and remain synchronized
with their existing keyboard controls.
A dormant profiling harness now traverses 11 production states and records raw
frame, CPU, GPU, allocation, memory, and rendering-counter samples to versioned
JSON. A 1920x1080 Editor diagnostic has verified the harness and scenario
routing; release certification still requires a standalone Windows player on
the approved mid-range hardware class plus external VRAM and cold-launch
measurement.
Earth now provides the representative layered-rendering proof with a custom
URP surface, ocean response, nightside-only city lights, independent clouds,
a restrained atmosphere rim, and close-focus orbit-guide suppression.
The Sun now provides a second representative hero treatment with a
project-owned deterministic surface shader, a thin separate corona, preserved
radial-light origin, and exposure-safe overview and close-focus presentation.
Jupiter now proves a reusable gas-giant pattern with an anchored source
texture, source-derived band relief, deterministic low-amplitude detail, and a
thin Sun-aware atmosphere while preserving its exact proportional radius,
analytical orbit, signed rotation, and Great Red Spot identity.
Saturn now extends that reusable pattern with a distinct restrained band
profile, thin atmosphere, and a dedicated two-sided Sun-aware ring shader that
keeps the approved radial alpha texture anchored to the generated annulus.
Venus now uses the reusable layered-body path to place its approved opaque
cloud map above the proportional surface, add a restrained Sun-aware limb, and
evaluate retrograde cloud motion from absolute simulation time without exposing
an unjustifiably detailed solid surface.
Mars now uses the same layered composition in explicit atmosphere-only mode,
with an anchored source-derived rocky surface, restrained rust/ochre balance,
and one narrow Sun-aware limb. No dummy cloud renderer or non-scientific
surface motion is introduced, and the exact proportional radius, analytical
orbit, axial tilt, and prograde rotation remain authoritative.

The approved major-moon roster is now complete: Earth's Moon, Io, Europa,
Ganymede, Callisto, Titan, and Triton. The six new bodies use JPL physical
parameters and J2000 mean parent-relative elements, exact Earth-relative mean
radii, synchronous signed rotation, distinct audited USGS browse mosaics,
selectable views, educational fact cards, and cached orbit paths. Triton's
157.3-degree orbital inclination and signed spin preserve its retrograde
behavior. These are deterministic educational mean orbits, not date-exact
ephemerides; source-image coverage and color limitations remain disclosed.
Titan now adds a haze-dominant hero treatment through the reusable
atmosphere-only layered-body path: its audited USGS mosaic remains anchored but
subdued beneath one bounded transparent amber shell with live-Sun day/night
response and deterministic low-amplitude presentation motion. Its approved
Saturn hierarchy, exact Earth-relative radius, analytical orbit, synchronous
rotation, selection, focus, and educational facts remain unchanged.
Io and Europa now extend the reusable airless-rocky path with separate
immutable presentation contracts and clean project-owned materials. Io uses a
restrained warm sulfur/ochre reconstruction with a dry response; Europa uses
pale neutral ice, gentler source-derived relief, and modestly higher
smoothness. Their audited USGS browse mosaics remain byte-identical and
anchored, while live-Sun terminators, exact Earth-relative radii,
Jupiter-parented analytical orbits, and positive synchronous rotations remain
authoritative. Neither moon receives invented emission, active plumes,
terrain displacement, a visible atmosphere, or an exposed subsurface ocean.
Browse resolution, coverage, color, and reconstruction limits are documented.
Ganymede and Callisto now complete the Galilean-moon hero surface set through
the same reusable airless-rocky path. Ganymede uses a restrained warm-neutral
mixed ice-and-rock response that preserves bright grooved and older dark
terrain; Callisto uses a darker, drier, more strongly relief-shaped response
for its ancient cratered identity. Their audited grayscale USGS browse
mosaics remain byte-identical and anchored. Live-Sun day/night response,
exact Earth-relative radii, Jupiter-parented analytical orbits, positive
synchronous rotation, selection, focus, and educational facts remain
authoritative. Bounded nightside source-color floors add no emission.
Neither body receives a visible atmosphere, aurora, magnetosphere, exposed
ocean, terrain displacement, fictional source fill, or date-specific state.
Triton now completes the approved major-moon hero set through the same
airless-rocky architecture. Its byte-identical USGS/Voyager 2
synthesized-color browse remains anchored, while near-black unobserved
coverage receives a clearly documented uniform neutral fill with no invented
terrain detail. The body preserves its exact Earth-relative radius, Neptune
parentage, `157.3`-degree retrograde mean orbit, negative synchronous spin,
selection, focus, and educational facts. The existing Sun point light now
uses a `1000`-unit culling envelope so Triton stays inside live radial
illumination; inverse-square attenuation and the day/night terminator remain
intact. No visible atmosphere, cloud, active geyser, plume animation,
emission, terrain displacement, or date-specific state is added.
Uranus and Neptune now use a reusable ice-giant authoring/model/view path with
distinct anchored source materials, restrained source-derived band response,
signed deterministic presentation detail, one thin Sun-aware atmosphere shell
each, and controlled nightside readability. Uranus remains pale cyan and
retrograde; Neptune remains a deeper prograde blue. Their exact proportional
radii, analytical orbits, axial tilts, and source rotation periods remain
unchanged, and the visual motion is not presented as a wind or fluid model.
Mercury and Earth's Moon now use a reusable airless-rocky
authoring/model/view path with distinct anchored sources, body-specific
restrained source-derived relief, dry PBR response, and small live-Sun
nightside readability floors. Neither body receives an atmosphere, cloud
layer, emissive night treatment, or animated terrain. Mercury remains
Sun-parented and the Moon remains Earth-parented; their exact proportional
radii, authored orbits, axial tilts, and signed rotations remain unchanged.
Every authored body is now reachable through an `N` celestial navigator in
deterministic parent-first order, with moons indented and identified by parent.
Activation reuses the existing selection and camera-focus services and does
not alter simulation pause state. `L` toggles cached projected labels that
prioritize the selected body and planets, suppress overlaps, avoid responsive
HUD safe areas, reduce to one target during focus, and hide during the guided
physical-scale comparison. The feature changes no body radius, orbit,
collider, or scientific source data and adds no third-party font or icon
dependency.
Detailed evidence is recorded in `Docs/ProjectManagement`.

## License

Project-authored source code and documentation are licensed under the [MIT License](LICENSE). Third-party media remains governed by its original license and is documented separately in the licensing ledger.

Copyright (c) 2026 Tanvir.
