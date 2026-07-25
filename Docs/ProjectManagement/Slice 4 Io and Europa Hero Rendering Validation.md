# Slice 4 Io and Europa Hero Rendering Validation

**Owner:** Tanvir  
**Technical steward:** Codex  
**Date:** 2026-07-25  
**Status:** Implemented and validated commit candidate  
**Unity:** 6000.5.3f1  
**URP:** 17.5.0

## Objective

Replace the generic URP/Lit Io and Europa materials with distinct,
scientifically honest hero treatments while preserving the approved physical,
orbital, scale, hierarchy, selection, focus, and educational behavior.

## Architecture decision

The existing reusable airless-rocky path is extended rather than duplicated:

`AirlessRockyVisualDefinition -> AirlessRockyVisualModel -> AirlessRockyVisualView -> Rocky Surface shader`

This is justified because Io's and Europa's visible baselines are opaque solid
surfaces that require the same stable-ID validation, live-Sun response,
source-derived normal perturbation, bounded nightside readability, and
allocation-free renderer setup already proven by Mercury and the Moon.

No new runtime subsystem, material instance, child shell, steady-state update,
transparent pass, terrain system, or simulation state was introduced.

## Implemented visual contracts

### Io

- Anchored source: `T_Io_Surface_Browse.jpg`
- Global presentation tint: restrained sulfur/ochre
- Relief strength: `0.22`
- Source-texel sample distance: `1.25`
- Non-metallic specular: `0.016`
- Smoothness: `0.045`
- Nightside readability: `0.018`

### Europa

- Anchored source: `T_Europa_Surface_Browse.jpg`
- Global presentation tint: pale neutral/cool ice
- Relief strength: `0.18`
- Source-texel sample distance: `1.1`
- Non-metallic specular: `0.035`
- Smoothness: `0.16`
- Nightside readability: `0.025`

These values are presentation controls, not measured terrain height,
roughness, ice purity, composition, or photometry.

## Scientific preservation

| Contract | Io | Europa |
|---|---:|---:|
| Parent | Jupiter | Jupiter |
| Mean radius | 1,821.49 km | 1,560.80 km |
| Semimajor axis | 421,800 km | 671,100 km |
| Signed synchronous period | +1.762732 days | +3.525463 days |
| Display radius | exact Earth-relative ratio | exact Earth-relative ratio |

The JPL definitions, parent-relative Kepler evaluation, orbital paths,
rotation direction, axial-tilt convention, selection collider policy,
educational facts, and scale modes were not changed.

## Source integrity and limitations

| Body | Runtime dimensions | SHA-256 |
|---|---:|---|
| Io | 512 x 256 | `DE69759452F5479B6F56FF5C72A90ED402AB8D7F11219524C26E5B60610B9597` |
| Europa | 512 x 256 | `3369BA56CBFA447347B5AFC003B80B69FBF87DD90A724D59F09ABAD8691A9819` |

Each runtime JPEG is byte-identical to its retained source download. Io's
browse is a grayscale Voyager/Galileo mosaic with uneven source coverage and
control quality. Europa's browse combines Voyager/Galileo inputs ranging from
coarse gap fill to higher-resolution observations and preserves dark or
incomplete regions. Neither is a complete natural-color, elevation,
composition, activity, or date-specific product.

The global tints are disclosed presentation reconstructions. Shader luminance
perturbs normals only and must not be described as elevation.

## Explicit exclusions

- No emissive Io lava or date-specific eruption.
- No active plume for either moon.
- No visible atmosphere or cloud shell.
- No terrain displacement or fluid simulation.
- No exposed Europa ocean.
- No fictional repainting of missing source coverage.

## Performance contract

The opaque shader performs one center texture sample and four neighboring
luminance samples. Both views use cached `MaterialPropertyBlock` instances,
disable shadows and probe participation, and perform no steady-state material
allocation or per-frame presentation update.

## Automated validation

- Unity compilation: passed.
- Console after the valid runs: zero warnings and zero errors.
- Edit Mode: `157 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Play Mode: `18 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Asset coverage verifies distinct materials, anchored source identity,
  project-owned shader selection, authored constants, texture imports, and
  visual definitions.
- Real-scene coverage verifies hierarchy, exact radii, synchronous periods,
  orbital distances, live-Sun direction, renderer policy, selection, focus,
  and unchanged pause state.

An initial Edit Mode launch was rejected by Unity's Test Runner because the
editor had been left in Play Mode. That invalid tooling-state run was cleared,
the editor was returned to Edit Mode, and only the clean rerun counts above
are acceptance evidence.

## Visual and semantic review

- Io remains recognizable as the warmer, drier volcanic moon.
- Europa remains pale and distinctly smoother without exaggerated blue.
- Both retain readable Sun-facing and night hemispheres.
- No extra atmosphere, cloud, emission, or terrain object is present.
- The scene contains initialized airless adapters for both stable IDs.
- Selection and focus retain the existing educational interaction flow.

## Remaining risks

- The `512 x 256` browse images will limit extreme close-up release captures.
- Io's grayscale browse cannot substantiate localized chemical color.
- Europa's varying input resolution and incomplete regions remain visible.
- Any future higher-resolution or color upgrade requires a fresh provenance,
  license, source-integrity, art-direction, performance, and regression audit.
