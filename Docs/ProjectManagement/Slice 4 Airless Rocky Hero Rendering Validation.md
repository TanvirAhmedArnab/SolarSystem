# Slice 4 Airless Rocky Hero Rendering Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation date:** 2026-07-24  
**Status:** Commit candidate validated; owner commit approval pending  
**Scope:** Mercury and Earth's Moon reusable airless-rocky hero rendering  

## 1. Outcome

Mercury and Earth's Moon now use a reusable airless-rocky
definition/model/view architecture and the project-owned `Rocky Surface`
shader. Each body preserves its distinct approved anchored source texture,
exact Earth-relative mean radius, authoritative parent-relative analytical
orbit, axial tilt, and signed sidereal rotation.

The slice introduces body-specific restrained source-luminance relief, dry
non-metallic PBR response, and a small live-Sun readability floor confined to
the unlit hemisphere. It does not create an atmosphere shell, cloud layer,
weather system, emissive night surface, animated terrain, or scientific
elevation model.

## 2. Authored Contracts

| Property | Mercury | Moon |
|---|---:|---:|
| Relief strength | `0.24` | `0.34` |
| Source-texel sample distance | `1.25` | `1.5` |
| Specular | `0.018` | `0.015` |
| Smoothness | `0.07` | `0.055` |
| Nightside readability | `0.018` | `0.022` |
| Atmosphere/cloud shell | None | None |

These values are presentation parameters. They do not change the celestial
catalog or enter the deterministic simulation.

## 3. Scientific Invariants

- Mercury remains parented to the Sun.
- The Moon remains parented to Earth.
- The Moon retains its authored `384,400 km` semimajor axis.
- Mercury retains a `2 degree` educational axial tilt and positive
  `58.6462-day` sidereal rotation.
- The Moon retains a `6.68 degree` visual tilt and positive `27.322-day`
  sidereal rotation.
- Rendered radius remains `meanRadiusKm / 6,371.0084 km`:
  approximately `0.38288` Earth radii for Mercury and `0.27270` for the Moon.
- The existing all-body Play Mode contract continues to compare every visual
  transform with the deterministic Kepler and signed-rotation evaluation.

## 4. Architecture and Reproducibility

- `AirlessRockyVisualDefinition` owns serialized, read-only body presentation.
- `AirlessRockyVisualModel` validates immutable runtime values and stable IDs.
- `AirlessRockyVisualView` applies the contract with one cached
  `MaterialPropertyBlock`; it does not instantiate materials or update every
  frame.
- `CelestialBodyView` initializes the optional adapter without changing
  simulation ownership.
- The deterministic content and scene builders create both definitions,
  configure both materials, add exactly two adapters, and serialize their
  references.
- The active scene contains exactly two airless-rocky adapters:
  `mercury,moon`.
- Both physical `Visual` meshes have zero child render layers.
- The scene was saved clean and the editor was not in Play Mode after review.

## 5. Shader and Material Validation

- Shader: `SolarSystem/Celestial/Rocky Surface`.
- Unity reports the shader present, supported, and carrying zero compiler
  messages.
- The anchored center sample remains the color authority.
- Four neighboring source-luminance samples estimate restrained tangent-space
  relief.
- The nightside term uses the shared live Sun position and is bounded to the
  reviewed per-body floor; it does not create a second light or flatten the
  terminator.
- Mercury and the Moon own separate material and definition assets.
- Mars retains its own atmosphere-only composition and now explicitly authors
  the shared shader's bounded nightside property.

## 6. Asset Provenance

Both source files and their Unity derivatives are byte-identical.

| Asset | License | SHA-256 |
|---|---|---|
| Mercury 2K surface | Solar System Scope, CC BY 4.0 | `5A5C80607F643496BAC9A631E71957DEF35ED788895F18B678AC849C2B38E48A` |
| Moon 2K surface | Solar System Scope, CC BY 4.0 | `2764BA6535EA0481A062846EE033CC7A909DAE05B31A8FD13F3E98F3A7FD92BD` |

No new third-party media or license was introduced. Release attribution
remains required under the existing licensing ledger.

## 7. Automated Validation

Final complete Unity Test Framework results:

- Edit Mode: `142 passed`, `0 failed`, `0 skipped`, `0 inconclusive`;
  duration `4.507 s`.
- Play Mode: `16 passed`, `0 failed`, `0 skipped`, `0 inconclusive`;
  duration `18.744 s`.
- Final Console: `0 errors`, `0 warnings`.

Coverage includes model validation, mismatched stable-ID rejection, cached
property-block application, renderer policy, distinct texture/material
contracts, import policy, exact proportional radii, parent identities, Moon
orbit data, tilt and rotation signs, no-shell composition, Sun-origin
lighting, and the existing complete scene journeys.

The first complete Edit Mode run correctly exposed one stale baseline
expectation for Mercury's old generic `0.08` smoothness. That test was updated
to read the named airless-rocky contract (`0.07`), after which the complete
suite passed.

## 8. Live Unity Review

The production selection and focus path was exercised in a maximized Game
view for both bodies.

- Mercury reads as a neutral gray-brown cratered body with restrained surface
  relief, no atmospheric halo, and legible focus-scale identity.
- The Moon retains recognizable maria and crater structure, low saturation, a
  clear Sun-driven terminator, and no atmospheric halo.
- Both selected-body panels display the expected parent and scientific values.
- The multi-angle Scene View tool was not used as beauty-shot evidence because
  edit-mode shader globals are inactive and orbit-line width dominates its
  extreme close framing; it was useful only to confirm absence of child shells.

## 9. Documentation Updated

- Living GDD `0.17.0`
- Living TDD `0.22.0`
- Art Bible `0.18.0`
- Third-party licensing ledger `0.11.0`
- Celestial data presentation limitations
- README validation state
- Changelog

## 10. Repository Preflight

The final staged candidate contains `35` paths.

- Generated-folder and IDE-artifact matches: `0`
- Staged `ProjectSettings` paths: `0`
- Strong secret-pattern matches: `0`
- Missing Unity `.meta` partners: `0`
- Duplicate Unity GUIDs: `0`
- Staged files at or above `1 MiB`: `0`
- Binary diffs: `0`
- `git diff --cached --check`: passed
- `git lfs fsck --pointers`: passed

The three pre-existing, unrelated Unity settings changes remain unstaged:

- `ProjectSettings/PackageManagerSettings.asset`
- `ProjectSettings/ProjectSettings.asset`
- `ProjectSettings/URPProjectSettings.asset`

## 11. Remaining Gate

The candidate is staged and ready for Tanvir's explicit commit approval. No
commit or push is authorized by this validation record.
