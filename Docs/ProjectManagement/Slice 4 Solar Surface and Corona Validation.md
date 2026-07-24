# Slice 4 Solar Surface and Corona Validation

**Owner:** Tanvir  
**Implementation steward:** Codex, subject to owner review  
**Validation date:** 2026-07-24  
**Unity baseline:** Unity 6000.5.3f1, URP 17.5.0  
**Status:** Implementation and validation complete

## Outcome

The static Sun baseline has been replaced by a reproducibly authored,
project-owned hero treatment. It reuses the approved Solar System Scope 2K Sun
texture, preserves the existing Sun transform as the scientific and lighting
origin, and keeps visible emission separate from the `Solar Radial Light`.

The treatment consists of:

- An opaque `SolarSystem/Celestial/Solar Surface` URP shader.
- A separate transparent `SolarSystem/Celestial/Solar Corona` shell.
- Immutable authoring and runtime models for reviewed visual parameters.
- Absolute simulation-time phase evaluation through cached material property
  blocks.
- Reproducible builder and real-scene wiring.

## Technical Contract

`SolarVisualDefinition` converts once into a validated immutable
`SolarVisualModel`. `SolarVisualView` caches the Sun's signed rotation period
and two `MaterialPropertyBlock` instances. Each rendered snapshot evaluates
surface and corona phases from the authoritative absolute simulation time.
Pausing therefore freezes the visual motion, resuming continues from the
authoritative clock, and repeated evaluation at the same time yields the same
result.

The treatment does not accumulate frame delta, instantiate runtime materials,
or allocate during steady-state updates. The `Solar Radial Light` remains a
separate child of the Sun root at local origin; visual shell changes cannot
move or replace the lighting source.

Reviewed presentation parameters:

- Corona shell radius multiplier: `1.045`.
- Surface flow: `0.125` cycles per authored Sun rotation.
- Corona flow: `0.05` cycles per authored Sun rotation.
- Surface flow strength: `0.006`.
- Corona flow strength: `0.012`.
- Corona intensity: `0.22`.

These are artistic presentation parameters. They are not a scientific
simulation of solar granulation, convection, plasma, or the physical corona.

## Rendering and Performance Review

- The surface is opaque and samples the source texture twice to break up an
  obvious linear slide.
- The corona uses one transparent shell, front-face culling, premultiplied
  blending, and depth testing against the surface.
- Surface and corona renderers do not cast shadows.
- The corona does not contribute to reflection probes.
- Both materials enable GPU instancing.
- Two property blocks are allocated once during initialization and reused.
- No lens flare was added because live evidence showed the thin corona and
  restrained bloom already supplied clear solar emphasis.

The transparent shell adds localized overdraw only around the Sun's silhouette.
Formal GPU profiling remains part of the release performance pass.

## Automated Validation

Final Unity results:

- Edit Mode: `121 passed`, `0 failed`, `0 skipped`, `0 inconclusive`;
  duration `0.445 s`.
- Play Mode: `10 passed`, `0 failed`, `0 skipped`, `0 inconclusive`;
  duration `9.999 s`.
- `Solar Surface` shader compiler messages: `0`.
- `Solar Corona` shader compiler messages: `0`.
- Unity Console warnings: `0`.
- Unity Console errors: `0`.

Coverage verifies:

- ScriptableObject-to-model conversion and invalid-parameter rejection.
- Finite and deterministic absolute-time phases.
- Shell scale, renderer references, property-block application, and shadow
  policy.
- Shader names, material wiring, import settings, render queue, intensity, and
  instancing.
- Real-scene hierarchy, light-origin separation, phase progression, paused
  state, close focus, renderer persistence, and camera-state restoration.

## Live Visual Inspection

The generated scene was inspected in Play Mode in both the full-system overview
and close Sun focus.

Accepted evidence:

- The source texture retains readable large-scale detail.
- Motion does not read as a simple horizontal texture conveyor.
- The corona reads as a thin warm rim rather than a thick brown shell.
- Bloom and HDR color emphasize the Sun without washing out planetary
  surfaces, orbit guides, selected-body information, or quick controls.
- Close focus preserves surface detail and selection feedback.
- No lens flare was necessary for readability or hierarchy.

During iteration, an overly thick/dark corona was corrected by reducing shell
scale and intensity while increasing the HDR color range. An invalid HLSL
half-literal form and a Unity-disallowed `MaterialPropertyBlock` field
initializer were also corrected before the final clean validation.

## Licensing and Provenance

No new third-party media was introduced.

The active Sun texture is the unchanged Unity derivative of
`TEX-SSS-003`, supplied by Solar System Scope under CC BY 4.0. The shaders,
shell geometry/wiring, authoring data, and runtime code are project-authored.
The existing Solar System Scope release attribution remains required.

## Remaining Release Work

- Capture profiler evidence on the documented mid-range reference PC.
- Include the solar treatment in final screenshots and portfolio video.
- Re-check the Solar System Scope attribution and source terms immediately
  before release.
