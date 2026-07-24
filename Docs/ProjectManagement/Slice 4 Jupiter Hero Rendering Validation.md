# Slice 4 Jupiter Hero Rendering Validation

**Owner:** Tanvir  
**Implementation steward:** Codex, subject to owner review  
**Validation date:** 2026-07-24  
**Unity baseline:** Unity 6000.5.3f1, URP 17.5.0  
**Status:** Implementation and validation complete

## Outcome

Jupiter's generic URP/Lit baseline has been replaced by a reproducibly
authored, project-owned hero treatment. It reuses the approved Solar System
Scope 2K Jupiter texture without changing its pixels, keeps the source map
anchored so the Great Red Spot and major bands remain stable features, and adds
restrained depth and motion without altering scientific state.

The treatment consists of:

- An opaque `SolarSystem/Celestial/Gas Giant Surface` URP shader.
- One transparent `SolarSystem/Celestial/Gas Giant Atmosphere` shell.
- Reusable immutable gas-giant authoring and runtime models.
- Absolute simulation-time phase evaluation through one cached material
  property block.
- Reproducible builder and real-scene wiring.

## Technical Contract

`GasGiantVisualDefinition` converts once into a validated immutable
`GasGiantVisualModel`. `GasGiantVisualView` caches Jupiter's signed rotation
period and one `MaterialPropertyBlock`. Each rendered snapshot evaluates a
wrapping detail phase from authoritative absolute simulation time.

Pausing freezes the detail, resuming continues from the authoritative clock,
retrograde bodies would preserve their signed direction, and repeated
evaluation at the same time produces the same phase. The adapter does not
accumulate frame delta, instantiate runtime materials, or allocate in
steady-state updates.

Reviewed presentation parameters:

- Atmosphere shell radius multiplier: `1.01`.
- Band-detail cycles per signed rotation: `0.0015`.
- Maximum longitudinal detail displacement: `0.0035` UV.
- Moving-detail color contribution: `0.08`.
- Source-derived band-normal strength: `0.42`.
- Atmosphere rim intensity: `0.20`.

These values affect presentation only. Jupiter retains its exact
Earth-relative mean-radius ratio, source-derived signed rotation, analytical
orbit, axial tilt, mass, and educational facts.

## Source-Identity and Scientific-Honesty Review

The surface shader always uses the unshifted source texture as its primary
color. A second, low-amplitude periodic sample contributes only local detail,
so the Great Red Spot and broad cloud structures do not become a scrolling
texture conveyor. Two vertical samples derive shallow latitudinal normal
variation from source luminance.

This is not a fluid-dynamics, wind-speed, storm-evolution, or real-time
atmospheric model. It is a deterministic educational visualization treatment,
and that limitation is recorded in the GDD, TDD, and Art Bible.

## Rendering and Performance Review

- The opaque surface performs four source-texture samples per fragment:
  anchored color, one moving detail sample, and north/south relief samples.
- The surface uses URP PBR lighting and remains inside the existing
  Sun-origin radial-light contract.
- One transparent shell supplies the warm Sun-aware limb.
- Surface and atmosphere renderers do not cast shadows.
- The atmosphere does not receive shadows or use light/reflection probes.
- Both materials enable GPU instancing.
- One material property block is allocated during initialization and reused.
- No additional texture, mesh asset, runtime material instance, or
  steady-state managed allocation was introduced.

Transparent overdraw is bounded to one shell around Jupiter. Formal GPU timing
and VRAM evidence remain part of the documented release profiling pass.

## Automated Validation

Validated Unity results:

- Edit Mode: `126 passed`, `0 failed`, `0 skipped`, `0 inconclusive`;
  duration `4.623 s`.
- Play Mode: `11 passed`, `0 failed`, `0 skipped`, `0 inconclusive`;
  duration `14.555 s`.
- `Gas Giant Surface` shader compiler messages: `0`.
- `Gas Giant Atmosphere` shader compiler messages: `0`.
- Unity Console warnings after final compilation: `0`.
- Unity Console errors after final compilation: `0`.

Coverage verifies:

- ScriptableObject-to-model conversion and invalid-parameter rejection.
- Finite, signed, deterministic absolute-time phase evaluation.
- Shell scale, renderer references, property-block application, probe policy,
  and shadow policy.
- Shader names, material wiring, anchored source texture, import settings,
  render queue, reviewed parameters, and instancing.
- Real-scene hierarchy, exact Jupiter/Earth radius ratio, Sun-origin lighting,
  phase progression, pause, close focus, overview restoration, renderer
  persistence, and preserved simulation/camera state.

## Live Visual Inspection

The generated scene was inspected in Play Mode at the full-system overview and
in close Jupiter focus.

Accepted evidence:

- Jupiter remains subordinate to the Sun in the system overview.
- The approved source map and large cloud structures remain recognizable.
- The Great Red Spot remains part of the anchored source identity.
- Band relief reads in close focus without looking carved or displaced.
- The Sun-facing hemisphere and opposing nightside remain distinct and
  readable.
- The warm atmosphere reads as a thin limb rather than a uniform glow.
- The selected-body panel, reticle, and quick controls remain legible.
- Focus hides orbit guides as designed, and leaving focus restores the
  existing free-flight behavior without changing selection or simulation.

No parameter retuning was required after the accepted live pass.

## Licensing and Provenance

No new third-party media was introduced.

The active Jupiter map is the unchanged Unity derivative of `TEX-SSS-011`,
supplied by Solar System Scope under CC BY 4.0. The shaders, authoring data,
runtime model/view, and shell wiring are project-authored. The existing Solar
System Scope release attribution remains required.

## Remaining Release Work

- Capture GPU, CPU, memory, and VRAM evidence on the documented reference PC.
- Include the Jupiter hero treatment in final screenshots and portfolio video.
- Re-check Solar System Scope attribution and source terms immediately before
  release.
