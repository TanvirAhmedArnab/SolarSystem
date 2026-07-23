# Solar System Simulation Art Bible

**Owner:** Tanvir  
**Status:** Living draft with approved source-asset direction  
**Version:** 0.4.0  
**Last updated:** 2026-07-23  
**Baseline:** Unity 6000.5.3f1, URP 17.5.0  
**Related:** `Docs/Design/GDD.md`, `Docs/Legal/ThirdPartyAssets.md`

> This document owns visual and audio production language. The GDD owns experience and scope; the TDD will own shader architecture and implementation.

## 1. Creative North Star

Present the Solar System as a restrained contemporary observatory experience: source-derived surfaces, physically motivated illumination, clear silhouettes, limited bloom, and honest disclosure wherever imagery is enhanced, incomplete, or reconstructed.

### Art pillars

- **Source-grounded identity:** recognizable global features originate in documented astronomical imagery or mosaics.
- **Cinematic restraint:** atmosphere, exposure, and motion support rather than obscure.
- **Scale-aware clarity:** materials work from system overview through approved focus shots.
- **Honest reconstruction:** fictional fill, synthesized color, and incomplete coverage are disclosed.
- **Cohesive instrumentation:** UI feels calm and precise, not militaristic or arcade-like.

## 2. Visual Language

### Palette

- Space black: `#03050A`
- Observatory navy: `#0B1424`
- Primary text: `#EDF4FF`
- Secondary text: `#9FB2CC`
- Selection cyan: `#56C8FF`
- Orbit blue: `#3C82D4`
- Solar amber: `#FFB24A`
- Error coral: `#FF6B6B`

Color is never the sole state indicator. Use clean panels, small corner radii, fine rules, limited decoration, and local scrims behind labels. Avoid fake telemetry, scanlines, hexagon clutter, and glitch effects.

## 3. Rendering Direction

URP is the production pipeline. The Sun is the dominant motivated source and planetary terminators must read clearly. A low environment contribution may preserve silhouettes. A shader-driven solar-light vector may replace a literal point light if extreme scale makes it more stable.

Post-processing baseline:

- Restrained bloom, primarily for the Sun.
- ACES as the starting tonemapping candidate.
- Subtle color correction only.
- Vignette and motion blur off by default.
- Film grain and chromatic aberration excluded unless evidence later justifies them.
- No exposure pumping during focus transitions.

## 4. Texture Sources and Policy

### Solar System Scope

Use its 2K CC BY 4.0 set for the Sun, eight planets, Earth layers, Saturn rings, starfield, and Moon.

Disclose that some unmapped regions contain fictional fill, colors are enhanced, and the maps are artistic composites suitable for educational visualization rather than scientific analysis.

### USGS Astrogeology

Use browse derivatives for Io, Europa, Ganymede, Callisto, Titan, and Triton.

These 512-1024 pixel files are evaluation sources. Hero shots may require later upgrades. Products can contain incomplete coverage, enhanced or synthesized color, interpolation, pole fill, and illumination seams. Titan's visible identity should emphasize haze over sharp surface detail.

### Resolution and derivative rules

- 2K is the default initial source resolution.
- Upgrade to 4K only when an approved 1080p hero shot shows meaningful benefit.
- 8K+ requires memory evidence, Git LFS review, and explicit approval.
- Never upscale merely to advertise a larger size.
- Preserve originals under `SourceAssets/ThirdParty`.
- Put Unity-ready derivatives under `Assets/SolarSystem/Content` only after orientation, licensing, and import review.
- Never modify originals in place.
- Record crop, rotation, seam repair, color conversion, channel packing, and derived maps.

## 5. Material Direction

### Sun
Emissive/unlit custom material using the 2K solar map for large-scale patterning. Optional subtle layered motion must avoid obvious texture sliding. Use a separate restrained corona; lens flare is optional.

### Mercury
Neutral gray-brown, high roughness, restrained normals, no metallic response.

### Venus
Separate surface and atmosphere/cloud spheres. Warm cream and sulfur yellow, not emissive orange. Cloud motion is slow and independent.

### Earth
Separate surface, cloud, and atmosphere layers. Day map drives albedo, specular differentiates oceans, normal detail stays subtle, night lights appear only on the unlit hemisphere, and clouds rotate independently.

### Moon
Use Solar System Scope 2K initially. NASA CGI Moon Kit is the upgrade candidate if hero shots justify it. Keep saturation and normal intensity low.

### Mars
Restrained rust, ochre, and basalt. Preserve polar contrast without clipping.

### Jupiter
Cloud bands and Great Red Spot define identity. Use soft broad response; optional band motion must preserve features.

### Io
USGS browse mosaic; sulfur yellow, orange, white, and dark volcanic markings. Animated eruptions are out of minimum scope.

### Europa
USGS browse mosaic; pale ice and tan lineae. Avoid exaggerated blue tint and displacement.

### Ganymede
USGS browse mosaic; bright grooved terrain and older dark regions distinguished through albedo.

### Callisto
USGS browse mosaic; dark neutral base and bright impact structures, distinct from the Moon.

### Saturn
Soft cream/gold bands. Ring alpha drives a dedicated flat mesh with tested two-sided rendering, transparency, and shadows.

### Titan
USGS near-global browse below a dense amber atmosphere. Surface detail remains subdued.

### Uranus
Pale cyan-blue, low-contrast bands, never neon. Rings remain conditional.

### Neptune
Controlled deep blue with subtle bands; nightside must not become electric blue.

### Triton
USGS color browse mosaic with pale pink, cream, and gray. Disclose incomplete and gap-filled regions.

## 6. Background, UI, and Motion

Use the Solar System Scope Stars + Milky Way 2K map initially. Keep it dim and independent of focus-body rotation. Orbit lines remain thin and subordinate.

Use an open-source sans-serif with tabular numerals after license review. Icons use a single consistent line family or project-authored SVG. UI transitions use short fades and translations; reduced-motion mode uses brief fades or cuts.

**[IMPLEMENTED PROOF]** The first explorer HUD uses a compact observatory card
with deep navy transparency, a thin cyan instrumentation edge, neutral
high-contrast text, green running state, amber paused state, and a separate
low-priority control-hint surface. It preserves the system view at 16:9 and
uses UI Toolkit scaling from a 1920x1080 reference. Unity's default runtime
sans-serif remains temporary until the open font decision and license review
are complete.

## 7. Audio Direction

### Music

Approved source: **Outer Space Loop by wipics**, CC0 1.0.

Use the seamless loop as quiet, non-diegetic 2D ambience with independent music volume and mute controls. CC0 permits commercial use, modification, and redistribution without attribution; retain an optional creator credit as professional provenance.

### Celestial-body ambience

- **Sun:** `A_Sun_BurningLoop.wav`, sourced from PagDev's CC0 Fireplace Sound Loop. Configure as a looping 2D AudioSource (`Spatial Blend = 0`) so it supplies a stylized solar-burning layer independent of camera distance.
- **Earth:** `A_Earth_ForestAmbienceLoop.mp3`, sourced from TinyWorlds' CC0 Forest Ambience. Configure as a looping 3D AudioSource (`Spatial Blend = 1`) attached to Earth, with a measured logarithmic rolloff range that supports focus shots without leaking across the system view.

These sounds are interpretive experience design, not a claim that sound propagates through space. Each must have an independent mixer level, a restrained fade during focus transitions, and no audible discontinuity when looping.

### Interface sounds

Approved source: **Kenney Interface Sounds**, CC0 1.0.

Initial audition mapping:

- Select: `select_001.ogg`
- Focus confirmation: `confirmation_001.ogg`
- Open: `open_001.ogg`
- Close: `close_001.ogg`
- Toggle/scale: `switch_002.ogg`
- Time tick: `tick_002.ogg`
- Invalid action: `error_004.ogg`

Only the selected subset enters the runtime build. Music stays below informational feedback, and repeated sounds must remain fatigue-free.

## 8. Folder and Naming Rules

Source: `SourceAssets/ThirdParty/<MediaType>/<Provider>/<OriginalFileName>`

Unity-ready:

- `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/<Body>`
- `Assets/SolarSystem/Content/Art/Textures/Environment`
- `Assets/SolarSystem/Content/Art/Materials/<Body>`
- `Assets/SolarSystem/Content/Audio/Music`
- `Assets/SolarSystem/Content/Audio/Ambience/CelestialBodies/<Body>`
- `Assets/SolarSystem/Content/Audio/SFX/UI`

Names:

- `T_<Body>_<Purpose>_<Resolution>`
- `M_<Body>_<Variant>`
- `SG_<Purpose>`
- `A_Music_<Title>` or `A_UI_<Action>`

Provider filenames remain unchanged under `SourceAssets`.

### Imported working set

The Unity working set contains byte-identical, renamed copies of 17 Solar System Scope textures, six USGS browse mosaics, seven selected Kenney UI sounds, one CC0 music loop, and two CC0 celestial-body ambience loops. Originals remain under `SourceAssets/ThirdParty`; the exact provenance and import mapping are owned by `Docs/Legal/ThirdPartyAssets.md`.

## 9. Release Quality Gate

An asset is ready only when:

- Source, creator, license, URL, date, attribution, and SHA-256 are recorded.
- Orientation and seams are validated on the target mesh.
- Color space, alpha, wrap, filtering, compression, and mipmaps are reviewed.
- Intended shots show no visible poles, seams, clipping, or transparency defects.
- Modifications and inherited licenses are recorded.
- Memory and frame-time costs meet the measured budget.
- Required credits appear in all release surfaces.

## 10. Open Art Decisions

1. **[OPEN] Font family:** decide after UI wireframes and license review.
2. **[OPEN] Icon family:** project-authored or compatible open-license set.
3. **[OPEN] Moon upgrades:** identify browse maps needing higher resolution after hero shots.
4. **[OPEN] Shader fidelity:** select atmosphere and solar techniques after profiling.
5. **[OPEN] Audio mix:** approve music, Sun, Earth, and UI loudness after in-scene audition.

## Revision History

| Version | Date | Summary | Approval |
|---|---|---|---|
| 0.4.0 | 2026-07-23 | Recorded the visually validated UI Toolkit explorer HUD proof and retained the licensed-font gate | Pending owner review |
| 0.3.0 | 2026-07-22 | Replaced restricted music with CC0 audio and defined Sun 2D and Earth 3D ambience direction | Pending owner review |
| 0.2.0 | 2026-07-22 | Reconciled the Unity content root and recorded the imported texture and UI-audio working set | Pending owner review |
| 0.1.0 | 2026-07-22 | Initial visual, material, texture, UI, and audio direction | Pending owner review |
