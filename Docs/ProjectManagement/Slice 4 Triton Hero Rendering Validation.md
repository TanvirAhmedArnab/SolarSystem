# Slice 4 Triton Hero Rendering Validation

**Owner:** Tanvir  
**Technical steward:** Codex, subject to owner review  
**Date:** 2026-07-25  
**Status:** Implemented and validated  
**Scope:** Triton hero rendering, source honesty, outer-system illumination, and regression evidence

## Objective

Complete the approved major-moon hero set by replacing Triton's generic
URP/Lit material with a distinct, source-grounded presentation while
preserving its scientific state and disclosing the severe limits of the
Voyager-era imagery.

## Implemented result

- Reused the existing immutable `AirlessRockyVisualDefinition -> Model ->
  View` path; no Triton-only runtime subsystem was introduced.
- Migrated `M_Triton` to the project-owned
  `SolarSystem/Celestial/Rocky Surface` shader.
- Added deterministic Triton visual-definition authoring and scene wiring.
- Kept one opaque renderer, no atmosphere shell, no material instance, one
  cached property block, and no steady-state update or allocation.
- Corrected the selected-body summary to state that Voyager 2 observed
  nitrogen geyser activity in 1989 rather than implying current activity.
- Expanded the existing Sun point-light culling range from `620` to `1000`
  units. Intensity remains `165000` candela, shadows remain disabled, and
  inverse-square attenuation remains active.

## Reviewed rendering contract

| Control | Value | Meaning |
|---|---:|---|
| Relief strength | `0.21` | Bounded source-luminance normal response |
| Relief sample distance | `1.25` texels | Neighbor sampling distance |
| Specular | `0.03` | Restrained non-metallic frost response |
| Smoothness | `0.18` | Presentation smoothness, not measured roughness |
| Nightside readability | `0.06` | Bounded anchored-color floor |
| Coverage threshold | `0.015` | Near-black unobserved-source detection |
| Coverage fill strength | `0.85` | Uniform neutral presentation fill |

The shared shader still performs five texture samples: one anchored color
sample and four neighboring luminance samples. Existing Mercury, Moon, Mars,
Io, Europa, Ganymede, and Callisto materials keep the coverage-fill option
disabled.

## Source and licensing evidence

- Source ID: `TEX-USGS-006`.
- Product: USGS/Voyager 2 Triton global color orthomosaic,
  `600 m/pixel`, orthographic projection, `PIA00317`.
- Product page:
  https://astrogeology.usgs.gov/search/map/triton_voyager_2_global_color_orthomosaic_600m
- Retained source:
  `SourceAssets/ThirdParty/Textures/USGS/triton_global_color_mosaic_browse.jpg`.
- Runtime source:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Triton/T_Triton_Surface_Browse.jpg`.
- Runtime browse dimensions: `512 x 256`.
- Cataloged source-product dimensions: `4500 x 3500`.
- SHA-256 for both retained and runtime JPEGs:
  `A71DF5E3DE28BA755200E0E9DB2633E0529CD2CDF344299715660F3DA37D1FCE`.
- USGS usage status: public domain.
- Retained release credit: `NASA/JPL/USGS`.

The mosaic's display color is synthesized from orange, violet, and ultraviolet
filter observations. It is not natural color. Voyager 2 remains the only
spacecraft to have flown by Triton and photographed roughly two-thirds of the
body during the 1989 encounter.

## Reconstruction disclosure

The source browse contains substantial black/unobserved coverage. The shader
uses the anchored center sample's luminance only to identify near-black
coverage and blend a uniform neutral mauve-gray fill. The observed source
image remains byte-identical and is not repainted.

The fill is not:

- spacecraft imagery;
- observed or inferred terrain;
- albedo, composition, or elevation;
- a global reconstruction;
- a current or date-specific state.

Source-luminance normal perturbation is suppressed wherever the fill applies,
so no terrain detail is manufactured in the reconstructed region.

## Scientific-state regression

The real scene preserves:

- parent: Neptune;
- mean radius: `1,352.60 km`;
- mean semimajor axis: `354,800 km`;
- eccentricity: `0.000`;
- orbital inclination: `157.3 degrees`;
- orbital period: positive `5.876994` days;
- synchronous rotation: negative `5.876994` days;
- exact Earth-relative rendered radius;
- deterministic parent-relative analytical motion;
- selection, HUD information, focus, pause state, and cached orbit path.

No visible atmosphere shell, cloud, active geyser, animated plume, emission,
terrain displacement, exposed subsurface layer, fluid simulation, or
date-specific activity is authored.

## Validation evidence

- Unity compilation: passed.
- Edit Mode: `157 / 157` passed.
- Play Mode: `20 / 20` passed.
- Unity Console after validation: `0` project warnings and `0` errors.
- Fixed-camera visual review: Triton remains inside the live Sun-light
  envelope, presents a brighter Sun-facing side and darker opposing side, and
  retains a muted pink/cream/gray identity without bloom or emission.
- Scene semantics: one Triton renderer, Rocky Surface shader, no atmosphere
  shell, Neptune parent, negative spin, retrograde orbit, source texture,
  selection/focus path, and educational summary verified.
- Repository preflight: staged-tree, secret, generated-file, missing-meta,
  diff, large-file, and LFS checks passed.

## Known limitations

- The retained runtime browse is intentionally low resolution.
- Coverage is incomplete and Voyager-era.
- Color is synthesized rather than natural.
- The coverage fill is presentation-only and intentionally uniform.
- The atmosphere is omitted because rendering its physical thinness at this
  scale would be visually misleading.
- Geyser activity is historical evidence, not a simulated present state.
- Fixed mean elements do not represent date-exact ephemerides or libration.
