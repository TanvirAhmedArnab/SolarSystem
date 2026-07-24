# Slice 4 Layered Earth Rendering Validation

**Owner:** Tanvir  
**Technical steward:** Codex, subject to owner review  
**Date:** 2026-07-24  
**Unity baseline:** Unity 6000.5.3f1, URP 17.5.0  
**Status:** Commit candidate validated

## Outcome

Earth now supplies the representative layered celestial-rendering slice. Its
surface remains physically lit, ocean response comes from the licensed
specular source, warm city lights appear only on the Sun-opposed hemisphere,
and independent cloud and atmosphere shells remain readable in close focus.
The implementation changes presentation only; scientific body radius, orbit,
rotation source data, selection, simulation, audio, and guided comparison
remain authoritative and unchanged.

## Implemented architecture

- `CelestialLayerVisualDefinition` stores the reviewed Earth layer authoring.
- `CelestialLayerVisualModel` validates and exposes immutable runtime values.
- `CelestialLayeredBodyView` applies shell scale and derives cloud drift from
  the evaluated body rotation, avoiding accumulated frame-delta error.
- `SolarShaderGlobals` publishes the live Sun world position once per frame.
- `EarthSurface.shader` combines physically lit day albedo, linear normal and
  ocean-specular inputs, and nightside-only emission.
- `EarthCloudLayer.shader` renders a source-driven transparent cloud shell.
- `AtmosphereRim.shader` renders a restrained Sun-aware transparent rim.
- `CelestialOrbitPathVisibilityController` hides overview guides only while
  the camera is transitioning to or holding close focus, then restores them.
- The editor builder recreates definitions, materials, shell hierarchy,
  shader wiring, focus policy, and the scene deterministically.

## Presentation contract

| Property | Verified value or policy |
|---|---|
| Earth surface radius | Existing exact Earth-relative value; unchanged |
| Cloud shell radius | `1.004` times Earth surface radius |
| Atmosphere shell radius | `1.018` times Earth surface radius |
| Cloud total rotation | `1.025` times Earth's signed rotation |
| Surface normal strength | `0.28` |
| City emission | Smoothly restricted to the Sun-opposed hemisphere |
| Shell shadows | Disabled |
| GPU instancing | Enabled on all three Earth materials |
| Focus orbit guides | Hidden during close focus; restored in free flight |

The cloud and atmosphere thicknesses are presentation exaggerations. The
selected-body panel discloses this explicitly and does not imply a change to
Earth's scientific or proportional surface radius.

## Asset and licensing review

The slice activates the existing Solar System Scope CC BY 4.0 Earth day,
night, cloud, normal, and specular derivatives. No source pixels were modified,
no new third-party asset entered the repository, and the existing attribution
obligation remains recorded in `Docs/Legal/ThirdPartyAssets.md`. All three
shaders, layer data, and runtime/editor code are project-authored.

## Automated validation

| Gate | Result |
|---|---|
| Unity C# compilation | Pass |
| Earth surface shader compiler messages | `0` |
| Earth cloud shader compiler messages | `0` |
| Atmosphere rim shader compiler messages | `0` |
| Edit Mode | `116 passed`, `0 failed`, `0 skipped`, `0 inconclusive` |
| Play Mode | `9 passed`, `0 failed`, `0 skipped`, `0 inconclusive` |
| Final Console errors | `0` |
| Final Console warnings | `0` |

Edit Mode coverage verifies the immutable layer contract, invalid authoring
rejection, shell scale, deterministic cloud drift, day/night reference
function, shader/material wiring, import color spaces, and UI disclosure.
Play Mode coverage loads the enabled scene and verifies live hierarchy,
materials, shell settings, Sun global, cloud motion and pause behavior, and
focus-time orbit suppression/restoration.

## Visual inspection

The live 16:9 Game view was inspected in Play Mode at system overview and
focused Earth framing. The overview retained its existing composition and UI.
Close focus showed:

- a clear day/night terminator;
- readable continents and oceans;
- source-masked ocean glint;
- warm city lights on the night side;
- independent cloud coverage;
- a restrained blue atmospheric rim;
- no detached shells or visible orbit-guide occlusion; and
- unchanged selected-body and control panels.

The first close inspection exposed world-space orbit lines as oversized
foreground bands and an overly strong ocean highlight. The final candidate
adds focus-only path suppression and reduces the ocean response; both complete
test suites and the live inspection were repeated afterward.

## Performance-oriented review

- No material instances are created at runtime.
- Shader property IDs and references are cached.
- Cloud rotation is computed from deterministic simulation state.
- The Sun global update is allocation-free in steady state.
- Transparent shells do not cast or receive shadows and do not use light or
  reflection probes.
- GPU instancing remains enabled.
- No new texture resolution or third-party binary was introduced.

## Remaining owner gates

- Final release profiling on the recorded mid-range reference PC.
- Screenshot/video capture and final color approval.
- Body-specific shader expansion only after measured need; this slice does not
  authorize copying Earth's treatment to every planet.
