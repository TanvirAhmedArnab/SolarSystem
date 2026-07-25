# Solar System Simulation Art Bible

**Owner:** Tanvir  
**Status:** Living authority with validated visual, audio, guided-scale, Sun, eight-planet, seven-moon, Titan haze, Galilean-moon, and Triton hero foundations  
**Version:** 0.23.0  
**Last updated:** 2026-07-25  
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

URP is the production pipeline. The Sun is the dominant motivated source and
planetary terminators must read clearly. A low environment contribution may
preserve silhouettes. Sun-facing hemispheres must be illuminated and opposing
hemispheres must read as night at every orbital position.

Post-processing baseline:

- Restrained bloom, primarily for the Sun.
- ACES tonemapping.
- Subtle color correction only.
- A subtle edge vignette; motion blur remains off by default.
- Film grain and chromatic aberration excluded unless evidence later justifies them.
- No exposure pumping during focus transitions.

**[IMPLEMENTED BASELINE]** The project-owned `VP_SolarSystem` profile replaces
the runtime scene's Unity template profile. It contains only ACES, Bloom,
Color Adjustments, and Vignette. The candidate baseline values are `1.10` bloom
threshold, `0.32` bloom intensity, `-0.10 EV` post exposure, `+6` contrast,
`-2` saturation, and `0.12` vignette intensity. These values preserve solar
surface detail and UI legibility at the 1080p mid-range-PC baseline.

**[IMPLEMENTED]** A warm `5600 K` point light is parented to the authored Sun
at local origin. Its `165,000 cd` intensity and `620`-unit range are presentation
values calibrated for the compressed-distance scene; they are not literal
astronomical photometry. The point source supplies a radial light vector to
Earth, Moon, Jupiter, and future bodies, so the day hemisphere always faces the
live Sun position. Low flat ambient fill and `0.18` sky reflection preserve
some silhouette information without erasing the terminator.

Realtime point-light shadows are disabled in this baseline. Cubemap shadows
would add material GPU cost and the exaggerated body radii plus compressed
distances would produce misleading eclipses. A custom body-to-Sun shader
remains an evidence-gated fallback only if later profiling or hero shots expose
attenuation or precision limitations.

## 4. Texture Sources and Policy

### Solar System Scope

Use its 2K CC BY 4.0 set for the Sun, eight planets, Earth layers, Saturn rings, starfield, and Moon.

Disclose that some unmapped regions contain fictional fill, colors are enhanced, and the maps are artistic composites suitable for educational visualization rather than scientific analysis.

### USGS Astrogeology

Use browse derivatives for Io, Europa, Ganymede, Callisto, Titan, and Triton.

These 512-pixel browse files are approved runtime baseline sources, not
hero-resolution masters. Hero shots may require later upgrades. Products can
contain incomplete coverage, enhanced or synthesized color, interpolation,
pole fill, and illumination seams. Titan's visible identity should emphasize
haze over sharp surface detail. No browse mosaic is presented as raw,
date-exact, globally complete scientific imagery.

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

**[IMPLEMENTED REPRESENTATIVE SLICE]** The Sun uses a project-owned opaque URP
surface shader with the approved 2K Solar System Scope texture, restrained HDR
color, and two phase-driven samples that create slow cellular motion without a
recognizable linear texture slide. Its motion is evaluated from absolute
simulation time and the Sun's authored signed rotation period, so pausing,
resuming, and repeated evaluation remain deterministic.

A separate front-face-culled transparent shell at `1.045` surface radius adds a
thin warm corona. It does not cast shadows or contribute to reflection probes,
and its presentation remains independent from the Sun-parented radial point
light. The existing fixed-exposure profile retains solar surface detail in both
overview and close focus without washing out planetary materials or the UI.
Live review found that a lens flare was not required, so none is included in
this slice.

### Mercury
Neutral gray-brown, high roughness, restrained normals, no metallic response.

**[IMPLEMENTED REPRESENTATIVE SLICE]** Mercury keeps the audited Solar System
Scope 2K source anchored and byte-identical. Four neighboring luminance samples
provide restrained crater relief at strength `0.24` and sample distance
`1.25`; the center sample remains the color authority. The dry non-metallic
surface uses `0.018` specular, `0.07` smoothness, and a `0.018` source-color
readability floor restricted to the Sun-opposed hemisphere. It has no
atmosphere shell, clouds, weather, emission, animated terrain, or claim of
scientific elevation accuracy.

### Venus
Separate surface and atmosphere/cloud spheres. Warm cream and sulfur yellow, not emissive orange. Cloud motion is slow and independent.

**[IMPLEMENTED REPRESENTATIVE SLICE]** Venus uses the reusable layered-body
architecture with the audited 2K surface map retained beneath an opaque cloud
deck. The approved 2K atmosphere map remains anchored to that deck, supplies
the recognizable warm cream and sulfur-yellow identity, and prevents a falsely
detailed solid surface from showing through the planet's continuous cloud
cover. Three nearby source samples add restrained relief without animated UVs
or procedural replacement of the source identity.

The cloud shell is `1.0115` times the physical surface radius and carries
deterministic retrograde motion derived from absolute simulation time and
Venus's signed rotation. Its reviewed presentation rate uses an approximately
`4.5`-day upper-cloud reference; this is not a claim of exact cloud altitude,
fluid dynamics, atmospheric chemistry, or date-specific circulation. A
separate transparent, non-shadow-casting atmosphere shell at `1.02` radius
adds a restrained Sun-aware limb. Only that outer rim uses transparency, so
overdraw remains bounded while Venus's exact proportional surface radius,
analytical orbit, axial tilt, and signed solid-body rotation remain unchanged.

### Earth
Separate surface, cloud, and atmosphere layers. Day map drives albedo, specular differentiates oceans, normal detail stays subtle, night lights appear only on the unlit hemisphere, and clouds rotate independently.

**[IMPLEMENTED REPRESENTATIVE SLICE]** Earth uses a project-owned URP surface
shader with the audited day, linear normal, linear ocean-specular, and
night-emission maps. The day surface remains physically lit by the live
Sun-origin point source; warm city lights fade in only on the Sun-opposed
hemisphere. Ocean response is driven by the source specular mask and tuned
below mirror-like intensity.

Clouds use a separate transparent shell at `1.004` Earth-surface radius and
rotate deterministically at `1.025` times Earth's signed spin. A separate
non-shadow-casting atmosphere shell at `1.018` radius supplies a restrained
Sun-aware blue rim. These shell thicknesses are intentionally exaggerated for
readability and are disclosed in the selected-body panel; they do not change
Earth's proportional physical surface radius. Orbit paths are suppressed only
while the camera is in close focus so they cannot obscure the layered material,
then restored in free flight.

### Moon
**[IMPLEMENTED REPRESENTATIVE SLICE]** The Moon uses the audited Solar System
Scope 2K map as its distinct anchored source. Its airless-regolith treatment
uses restrained source-derived relief `0.34`, sample distance `1.5`, specular
`0.015`, smoothness `0.055`, and nightside readability `0.022`. Live review
retains recognizable maria, crater structure, low saturation, a clear
Sun-driven terminator, and no atmospheric halo. NASA CGI Moon Kit remains only
an evidence-gated upgrade if an approved release shot demonstrates a material
2K limitation.

### Mars
Restrained rust, ochre, and basalt. Preserve polar contrast without clipping.

**[IMPLEMENTED REPRESENTATIVE SLICE]** Mars uses the audited 2K surface map as
an anchored source of recognizable terrain and polar identity. A cool
multiplicative correction offsets the source's strong red cast without
altering its pixels; the final planet reads as dark rust and ochre rather than
emissive red. Four neighboring source samples derive restrained relief around
the anchored center sample. The surface remains dry and broad with `0.025`
specular and `0.10` smoothness.

A single warm atmosphere shell at `1.008` surface radius supplies a narrow
Sun-aware limb with `0.16` intensity and minimal nightside visibility. Mars
has no authored cloud shell. The atmosphere casts and receives no shadows,
uses no light or reflection probes, and remains subordinate to the surface.
The shell thickness is presentation exaggeration; dust storms, clouds,
volumetric scattering, atmospheric chemistry, and date-exact color are not
simulated.

### Jupiter
Cloud bands and Great Red Spot define identity. Use soft broad response; optional band motion must preserve features.

**[IMPLEMENTED REPRESENTATIVE SLICE]** Jupiter uses a project-owned URP
surface shader that keeps the approved 2K Solar System Scope map anchored as
its primary color source. The Great Red Spot and large cloud structures
therefore rotate only with the authored planet rather than sliding across its
surface. Source-derived north/south luminance differences add restrained
latitudinal relief, while one low-amplitude periodic detail sample contributes
only `0.08` of the final color. The detail phase comes from absolute simulation
time and Jupiter's signed rotation period; it freezes when the simulation is
paused and is not a claim of scientific atmospheric-fluid simulation.

A separate warm atmosphere shell at `1.01` surface radius adds a thin
Sun-aware limb. It is transparent, non-shadow-casting, excluded from light and
reflection probes, and tuned to `0.20` rim intensity. Live overview and close
focus preserve the Great Red Spot, readable bands, a legible nightside,
selection feedback, and UI exposure without turning Jupiter into a glowing
sphere.

### Io
USGS browse mosaic; sulfur yellow, orange, white, and dark volcanic markings. Animated eruptions are out of minimum scope.

**[IMPLEMENTED REPRESENTATIVE SLICE]** The byte-identical grayscale USGS
browse mosaic stays anchored as the feature source. A restrained global
sulfur/ochre tint establishes Io's identity, while moderate source-luminance
normal perturbation, low smoothness, and low non-metallic specular response
keep it visually dry. The tint is a presentation reconstruction, not a
localized compositional map or natural-color claim.

Do not add emissive lava, animated eruptions, plumes, terrain displacement,
clouds, or a visible atmosphere without a separately audited, date-specific
source and an approved scope change. Source-image luminance includes
illumination and processing effects and must not be called elevation.

### Europa
USGS browse mosaic; pale ice and tan lineae. Avoid exaggerated blue tint and displacement.

**[IMPLEMENTED REPRESENTATIVE SLICE]** The byte-identical grayscale USGS
browse mosaic remains the feature source. A pale neutral/cool tint preserves
the fracture network, with gentler source-derived relief than Io and a
modestly smoother, higher-specular ice response. The treatment remains
restrained: Europa must read pale rather than neon blue or mirror-like.

The surface product combines Galileo and Voyager coverage with varying source
resolution and gap fill. Dark or incomplete source regions are preserved
rather than fictionally repainted. Do not expose the subsurface ocean, add
active plumes, displace terrain, or render a visible atmosphere. The ocean is
an educational fact beneath the ice, not a visible surface layer.

### Ganymede
USGS browse mosaic; bright grooved terrain and older dark regions distinguished through albedo.

**[IMPLEMENTED REPRESENTATIVE SLICE]** The byte-identical grayscale USGS
browse mosaic remains anchored. A restrained warm-neutral presentation tint,
moderate source-derived non-displacing relief, and a modest mixed
ice-and-rock sheen preserve the contrast between bright grooved terrain and
older dark cratered regions. The tint is not natural-color or compositional
data, and browse luminance is not elevation.

Do not add a visible oxygen atmosphere, aurora, magnetosphere, exposed
subsurface ocean, clouds, terrain displacement, or date-specific activity.
Ganymede's magnetic field and possible deep ocean belong in the educational
facts, not as unsupported visible layers.

### Callisto
USGS browse mosaic; dark neutral base and bright impact structures, distinct from the Moon.

**[IMPLEMENTED REPRESENTATIVE SLICE]** The byte-identical grayscale USGS
browse mosaic remains anchored. A darker neutral presentation tint, stronger
source-derived non-displacing relief, and low dry-surface sheen distinguish
Callisto's ancient, heavily cratered terrain and bright impact structures
from Ganymede, Europa, and Earth's Moon.

Preserve Callisto's inherently dark identity. The bounded nightside floor is
higher than the lighter airless bodies only because this browse derivative is
very dark; it remains non-emissive and visibly below the Sun-facing response.
Do not add a visible exosphere, exposed subsurface ocean, clouds, geological
activity, terrain displacement, fictional gap fill, or date-specific state.

### Saturn
Soft cream/gold bands. Ring alpha drives a dedicated flat mesh with tested two-sided rendering, transparency, and shadows.

**[IMPLEMENTED REPRESENTATIVE SLICE]** Saturn reuses the immutable gas-giant
model/view architecture with a distinct, lower-amplitude presentation profile.
The audited 2K surface map remains the anchored primary color source; shallow
source-derived relief, `0.035` moving-detail contribution, and absolute-time
phase preserve broad bands without claiming scientific fluid simulation. A
thin atmosphere shell at `1.008` surface radius uses a restrained `0.14` rim
intensity.

The approved ring-alpha strip remains anchored radially across the generated
128-segment annulus. A project-owned transparent shader renders both faces,
uses one texture sample, premultiplied blending, and the live Sun direction.
It adds a restrained grazing-angle visibility term but does not model particle
scale, transmission through ring depth, ring self-shadowing, or exact
photometry. Ring and atmosphere renderers cast no shadows and use no light or
reflection probes. Saturn's physical radius, orbit, signed rotation, axial
tilt, and generated ring geometry remain unchanged.

### Titan
USGS near-global browse below a dense amber atmosphere. Surface detail remains
subdued.

**[IMPLEMENTED REPRESENTATIVE SLICE]** Titan uses a dedicated project-owned
surface shader and one transparent haze shell. The anchored USGS Cassini ISS
near-global mosaic contributes only faint softened structure: source contrast
is limited to `0.12`, with no moving surface sample, terrain relief, specular
highlight, or inferred topography. A warm amber shell at `1.028` surface
radius supplies a `0.64` full-disk haze term, broad restrained limb, live-Sun
day/night response, and a low-amplitude `0.018` presentation variation.

The shell is intentionally atmosphere-dominant in close focus. It is not a
literal atmosphere-scale visualization, volumetric scattering solution,
measured methane-opacity model, cloud layer, or weather simulation. The slow
absolute-time phase is presentation motion only and must not be described as
wind speed or observed weather. The source mosaic is processed through methane
windows and haze correction, covers approximately 45 degrees north to 65
degrees south, and is not natural visible-light, globally complete, or
date-specific imagery.

### Uranus
Pale cyan-blue, low-contrast bands, never neon. Rings remain conditional.

The validated hero treatment keeps the audited 2K source map anchored, uses
`0.28` smoothness, `0.07` source-derived band relief, and only `0.012`
moving-detail contribution. A pale-cyan atmosphere shell at `1.009` surface
radius uses `0.12` limb intensity. A `0.035` source-color floor applies only
on the unlit hemisphere so close focus preserves identity without flattening
the Sun-facing terminator. Absolute-time detail follows Uranus's signed
retrograde rotation. Rings remain conditional and are not invented by this
slice.

### Neptune
Controlled deep blue with subtle bands; nightside must not become electric blue.

The validated hero treatment keeps the distinct audited 2K source map
anchored, uses `0.30` smoothness, `0.16` source-derived band relief, and a
bounded `0.035` moving-detail contribution. A deeper-blue atmosphere shell at
`1.01` surface radius uses `0.17` limb intensity. A `0.04` anchored-color
floor applies only on the unlit hemisphere; live review confirms that it
preserves a dark blue nightside rather than producing electric-blue emission.
Absolute-time detail follows Neptune's prograde signed rotation.

For both ice giants, the shell thickness, relief, moving sample, tint, and
nightside floor are presentation controls. They do not claim measured wind
speed, fluid simulation, atmospheric chemistry, physical scale height,
volumetric scattering, exact photometry, or date-specific appearance.

### Triton

Use the retained USGS/Voyager 2 synthesized-color browse mosaic as the
observed surface anchor. Preserve its muted pink polar deposits, greenish
equatorial band, dark plume deposits, seams, and incomplete coverage; do not
market the result as natural color or a globally observed modern map.

The validated hero material uses a pale rose-neutral tint, relief `0.21`,
sample distance `1.25`, specular `0.03`, smoothness `0.18`, and nightside
readability `0.06`. Near-black unobserved source coverage receives a uniform
neutral mauve-gray fill at strength `0.85` using a `0.015` luminance
threshold. This is a disclosed presentation reconstruction, not inferred
terrain or spacecraft imagery. Relief is disabled on the reconstructed region
so the shader does not manufacture surface detail.

Triton remains one opaque sphere with no atmosphere shell. Its measured
nitrogen atmosphere is too thin to justify a visible layer at this scale.
Do not add clouds, active geysers, animated plumes, emission, exposed
subsurface ice/ocean, terrain displacement, fluid simulation, or
date-specific activity. The Sun-facing hemisphere must remain clearly
brighter than the opposing hemisphere; the widened radial-light culling
envelope exists only to keep the outer-system body inside the live light.

**[IMPLEMENTED BASELINE]** Io, Europa, Ganymede, Callisto, Titan, and Triton
now use distinct audited browse mosaics and restrained opaque URP materials.
Rendered radii stay scientifically proportional; invisible selection
colliders and cached orbit guides provide usability without enlarging the
surfaces. The baseline intentionally adds no invented eruptions, clouds,
terrain displacement, emissive geology, or date-specific weather. Titan's
validated hero treatment supersedes its interim surface-first material and
subordinates sharp source detail to its documented amber haze without
inventing a cloud deck.

## 6. Background, UI, and Motion

Use the Solar System Scope Stars + Milky Way 2K map initially. Keep it dim and independent of focus-body rotation. Orbit lines remain thin and subordinate.

**[IMPLEMENTED SCALE RULE]** Earth is the visual-radius reference at one unit,
and every other rendered celestial body uses its exact mean-radius ratio to
Earth. Do not enlarge individual planets or moons to make them easier to click.
Use invisible selection hit areas, focus transitions, labels, and navigator
entries for accessibility. Orbital distance remains logarithmically compressed
and must be disclosed separately from the proportional body-size rule.

At system overview, the Sun should read as dominant, the giant planets should
remain visibly larger than the terrestrial planets, and small rocky bodies may
be genuinely tiny. Orbit lines can be widened enough to survive the expanded
overview envelope, but remain lower contrast than bodies and selection
feedback. Focused compositions reveal surface materials without changing the
body's scale ratio.

**[IMPLEMENTED BASELINE]** `M_SpaceSkybox` presents the approved 2K
equirectangular map with a restrained cool tint and `0.62` exposure. The camera
uses skybox clearing, HDR, post-processing, NaN suppression, and dithering.

Use an open-source sans-serif with tabular numerals after license review. Icons use a single consistent line family or project-authored SVG. UI transitions use short fades and translations; reduced-motion mode uses brief fades or cuts.

**[IMPLEMENTED PROOF]** The first explorer HUD uses a compact observatory card
with deep navy transparency, a thin cyan instrumentation edge, neutral
high-contrast text, green running state, amber paused state, and a separate
low-priority control-hint surface. It preserves the system view at 16:9 and
uses UI Toolkit scaling from a 1920x1080 reference. Unity's default runtime
sans-serif remains temporary until the open font decision and license review
are complete.

**[IMPLEMENTED GUIDED COMPARISON]** A wide bottom-center teaching card replaces
the normal quick-control strip and selected-body card while scale comparison is
active. It presents stage progress, a plain-language title, the current numeric
reference, one concise explanation, and separate `C / NEXT` and `ESC / EXIT`
keycaps. The normal top-left status card remains visible so paused state, time
rate, and active scale transformation stay auditable.

The comparison deliberately changes visual density:

- Readable overview retains the normal body hierarchy and thin orbit lines.
- Linear orbit spacing makes real bodies sub-pixel while orbit guides remain
  visible at restrained contrast.
- Literal Earth-radius scale frames the Earth-Sun relationship and allows Earth
  to become effectively invisible beside the Sun.

Do not add minimum visible body sizes, glow markers, or decorative substitutes
in the two linear stages; the apparent disappearance is the educational point.

## 7. Audio Direction

### Music

Approved source: **Outer Space Loop by wipics**, CC0 1.0.

Use the seamless loop as quiet, non-diegetic 2D ambience with independent music volume and mute controls. CC0 permits commercial use, modification, and redistribution without attribution; retain an optional creator credit as professional provenance.

**[IMPLEMENTED BASELINE]** `A_Music_OuterSpaceLoop.mp3` imports as a
stereo, streaming Vorbis clip with background loading and plays from a
non-spatial scene source.

### Celestial-body ambience

- **Sun:** `A_Sun_BurningLoop.wav`, sourced from PagDev's CC0 Fireplace Sound Loop. Configure as a looping 2D AudioSource (`Spatial Blend = 0`) so it supplies a stylized solar-burning layer independent of camera distance.
- **Earth:** `A_Earth_ForestAmbienceLoop.mp3`, sourced from TinyWorlds' CC0 Forest Ambience. Configure as a looping 3D AudioSource (`Spatial Blend = 1`) attached to Earth, with a measured logarithmic rolloff range that supports focus shots without leaking across the system view.

**[IMPLEMENTED BASELINE]** Both ambience clips import as mono, streaming
Vorbis audio with background loading. The Sun source is 2D and parented to the
Sun. The Earth source is fully 3D and parented to Earth, using logarithmic
rolloff from `1.5` to `12` presentation units with Doppler and reverb-zone
effects disabled.

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

**[IMPLEMENTED BASELINE]** Selection uses `A_UI_Select.ogg`, focus uses
`A_UI_FocusConfirmation.ogg`, and pause/speed changes use
`A_UI_TimeTick.ogg`. These short mono cues preload and decompress on load.
Open, close, toggle, and invalid-action mappings remain reserved for future
features.

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
3. **[OPEN] Moon upgrade:** compare the current validated 2K focus shot against
   the NASA CGI Moon Kit only if release capture reveals a visible limitation.
4. **[OPEN] Remaining shader fidelity:** profile the validated Earth, Sun,
   eight-planet, and Earth-Moon patterns against release screenshots and
   measured screen-space need; add no unique shader without evidence.
5. **[OPEN] Audio mix:** approve music, Sun, Earth, and UI loudness after in-scene audition.

## Revision History

| Version | Date | Summary | Approval |
|---|---|---|---|
| 0.23.0 | 2026-07-25 | Added Triton's distinct airless hero treatment with anchored synthesized-color USGS/Voyager imagery, a disclosed uniform fill for unobserved black coverage, source-derived non-displacing detail only on observed pixels, corrected outer-system light culling, and explicit atmosphere/activity limits | Triton hero slice implemented and visually validated |
| 0.22.0 | 2026-07-25 | Added distinct Ganymede and Callisto airless hero treatments with anchored byte-identical USGS mosaics, restrained reconstructed color, source-derived non-displacing relief, mixed-versus-ancient surface separation, and explicit magnetic-field, ocean, atmosphere, coverage, and elevation limits | Ganymede and Callisto hero slice implemented and visually validated |
| 0.21.0 | 2026-07-25 | Added distinct Io and Europa airless hero treatments with anchored byte-identical USGS mosaics, restrained reconstructed color, source-derived non-displacing relief, dry-versus-icy PBR separation, and explicit activity/ocean/coverage limits | Io and Europa hero slice implemented and visually validated |
| 0.20.0 | 2026-07-25 | Added Titan's haze-dominant project-owned surface/shell treatment, anchored and subdued USGS source use, live-Sun day/night response, deterministic low-amplitude presentation motion, bounded overdraw, and explicit image/atmosphere limitations | Titan hero slice implemented and visually validated |
| 0.19.0 | 2026-07-25 | Added the approved major-moon baseline with distinct audited USGS browse mosaics, exact proportional radii, and explicit coverage/color limitations | Major-moon visual baseline implemented and validated |
| 0.18.0 | 2026-07-24 | Added distinct anchored Mercury/Moon airless-rocky treatments, body-specific restrained source-derived relief and dry PBR values, live-Sun nightside readability, no-atmosphere guarantees, and explicit elevation-model limits | Airless rocky hero slice implemented and visually validated |
| 0.17.0 | 2026-07-24 | Added distinct anchored Uranus/Neptune treatments, restrained source-derived detail, signed deterministic presentation motion, thin Sun-aware limbs, controlled nightside readability, and explicit non-fluid-simulation limits | Ice-giant hero slice implemented and visually validated |
| 0.16.0 | 2026-07-24 | Added Mars's anchored rocky surface, restrained source-derived relief, atmosphere-only layered composition, thin Sun-aware limb, and explicit color/atmosphere limitations | Mars hero slice implemented and visually validated |
| 0.15.0 | 2026-07-24 | Added Venus's anchored opaque cloud deck, deterministic retrograde cloud presentation, restrained Sun-aware limb, bounded transparency, and explicit scientific limitations | Venus atmosphere slice implemented and visually validated |
| 0.14.0 | 2026-07-24 | Added Saturn's anchored gas-giant surface, restrained limb, and two-sided Sun-aware radial ring treatment with explicit scientific limitations | Saturn hero slice implemented and visually validated |
| 0.13.0 | 2026-07-24 | Added the validated anchored-texture Jupiter surface, restrained source-derived band relief, deterministic low-amplitude detail, Sun-aware limb, and scientific-limitation disclosure | Jupiter hero slice implemented and visually validated |
| 0.12.0 | 2026-07-24 | Added the validated deterministic solar surface, separate restrained corona, exposure-safe overview/focus treatment, and evidence-based decision to omit lens flare | Solar hero slice implemented and visually validated |
| 0.11.0 | 2026-07-24 | Added the validated layered-Earth surface, nightside emission, independent cloud shell, restrained atmosphere rim, focus readability, and explicit scale disclosure | Representative visual slice implemented and visually validated |
| 0.10.0 | 2026-07-24 | Added the validated three-stage scale-comparison card, visual-density rules, transformation disclosures, and linear-stage honesty requirements | Approved behavior implemented and visually validated |
| 0.9.0 | 2026-07-24 | Established exact Earth-relative rendered radii, selection-only accessibility accommodation, and readable compressed-overview composition rules | Approved and implementation validated |
| 0.8.0 | 2026-07-24 | Recorded the implemented licensed music, 2D Sun ambience, 3D Earth ambience, UI cues, and deterministic import contracts | Technical baseline validated; owner listening approval pending |
| 0.7.0 | 2026-07-23 | Recorded the eight-planet material baseline, deterministic Saturn annulus, and deferred atmosphere/cloud/ring fidelity layers | Pending owner review |
| 0.6.0 | 2026-07-23 | Replaced the fixed directional approximation with validated Sun-origin radial illumination and explicit day/night readability | Pending owner review |
| 0.5.0 | 2026-07-23 | Recorded the validated project-owned skybox, URP volume, lighting, and representative material foundation | Pending owner review |
| 0.4.0 | 2026-07-23 | Recorded the visually validated UI Toolkit explorer HUD proof and retained the licensed-font gate | Pending owner review |
| 0.3.0 | 2026-07-22 | Replaced restricted music with CC0 audio and defined Sun 2D and Earth 3D ambience direction | Pending owner review |
| 0.2.0 | 2026-07-22 | Reconciled the Unity content root and recorded the imported texture and UI-audio working set | Pending owner review |
| 0.1.0 | 2026-07-22 | Initial visual, material, texture, UI, and audio direction | Pending owner review |
