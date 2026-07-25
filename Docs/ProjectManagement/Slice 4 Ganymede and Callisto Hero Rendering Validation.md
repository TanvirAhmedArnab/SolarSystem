# Slice 4 Ganymede and Callisto Hero Rendering Validation

**Owner:** Tanvir  
**Technical steward:** Codex  
**Date:** 2026-07-25  
**Status:** Implemented and validated  
**Unity:** 6000.5.3f1  
**URP:** 17.5.0

## Objective

Replace the generic URP/Lit Ganymede and Callisto materials with distinct,
scientifically honest hero treatments while preserving their approved
physical, orbital, scale, hierarchy, selection, focus, and educational
behavior.

## Architecture decision

The existing reusable airless-rocky path is extended rather than duplicated:

`AirlessRockyVisualDefinition -> AirlessRockyVisualModel -> AirlessRockyVisualView -> Rocky Surface shader`

Both bodies are opaque solid-surface moons that need the same stable-ID
validation, live-Sun response, source-derived non-displacing normal
perturbation, bounded nightside readability, and allocation-free renderer
setup already proven by Mercury, the Moon, Io, and Europa. No new runtime
subsystem, material instance, child shell, transparent pass, steady-state
update, terrain system, or simulation state was introduced.

## Implemented visual contracts

### Ganymede

- Anchored source: `T_Ganymede_Surface_Browse.jpg`
- Global presentation tint: restrained warm neutral
- Relief strength: `0.26`
- Source-texel sample distance: `1.25`
- Non-metallic specular: `0.028`
- Smoothness: `0.12`
- Nightside readability: `0.07`

### Callisto

- Anchored source: `T_Callisto_Surface_Browse.jpg`
- Global presentation tint: dark neutral
- Relief strength: `0.30`
- Source-texel sample distance: `1.4`
- Non-metallic specular: `0.014`
- Smoothness: `0.05`
- Nightside readability: `0.08`

These are bounded presentation controls, not measured terrain height,
roughness, ice content, composition, albedo, or photometry. The nightside
values add source color only and remain non-emissive.

## Scientific preservation

| Contract | Ganymede | Callisto |
|---|---:|---:|
| Parent | Jupiter | Jupiter |
| Mean radius | 2,631.20 km | 2,410.30 km |
| Semimajor axis | 1,070,400 km | 1,882,700 km |
| Signed synchronous period | +7.155588 days | +16.690440 days |
| Display radius | exact Earth-relative ratio | exact Earth-relative ratio |

The JPL definitions, parent-relative Kepler evaluation, orbital paths,
rotation direction, axial-tilt convention, selection collider policy,
educational facts, and scale modes were not changed.

## Source integrity and limitations

| Body | Runtime dimensions | SHA-256 |
|---|---:|---|
| Ganymede | 512 x 256 | `465673D0D789658CE63275C8CCC9EBBDF6B1AEC0A148CAA41052FEBC314A1616` |
| Callisto | 512 x 249 | `FA60F8305E1B000E4FBC4446CECDD5DF919A778841D9D7354E606889BBAC856F` |

Each runtime JPEG is byte-identical to its retained source download.
Ganymede's product combines Galileo SSI and Voyager inputs from coarse gap
fill through higher-resolution observations. Callisto's product likewise
combines Galileo SSI and Voyager inputs and preserves incomplete dark
coverage. Neither browse image is a complete natural-color, elevation,
composition, activity, or date-specific product.

Global tints are disclosed presentation reconstructions. Shader luminance
perturbs normals only and must not be described as elevation.

## Explicit exclusions

- No visible atmosphere, exosphere, clouds, or emission.
- No Ganymede aurora or magnetosphere.
- No exposed subsurface ocean.
- No active geology or date-specific state.
- No terrain displacement or fluid simulation.
- No fictional repainting of missing source coverage.

## Performance contract

The opaque shader performs one center texture sample and four neighboring
luminance samples. Both views use cached `MaterialPropertyBlock` instances,
disable shadows and probe participation, and perform no steady-state material
allocation or per-frame presentation update.

## Automated validation

- Unity compilation: passed.
- Console after valid runs: zero warnings and zero errors.
- Edit Mode: `157 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Play Mode: `19 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Asset coverage verifies distinct materials, anchored source identity,
  project-owned shader selection, authored constants, texture imports, and
  visual definitions.
- Real-scene coverage verifies hierarchy, exact radii, synchronous periods,
  orbital distances, live-Sun direction, renderer policy, selection, focus,
  and unchanged pause state.

## Visual and semantic review

- Ganymede retains readable mixed bright-grooved and older dark terrain.
- Callisto retains a darker ancient cratered identity and bright impact marks.
- Their PBR and relief profiles are visibly distinct without exaggerated color.
- Lit and unlit hemispheres remain distinct; nightside fill is non-emissive.
- No extra shell, atmosphere, cloud, emission, or terrain object is present.
- Both remain Jupiter-parented, selectable, focusable, and educational.

## Remaining risks

- The low-resolution grayscale browse images limit extreme close-up release
  captures and cannot substantiate localized chemical color.
- Coverage gaps, illumination seams, and processing artifacts remain visible.
- Parent-body occlusion can temporarily reduce readability at some orbital and
  camera alignments; the simulation remains free to move and refocus.
- Any future higher-resolution or color upgrade requires a new provenance,
  license, source-integrity, art-direction, performance, and regression audit.
