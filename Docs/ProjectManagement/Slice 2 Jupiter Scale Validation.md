# Slice 2 Jupiter Scale Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation date:** 2026-07-23  
**Unity version:** 6000.5.3f1  
**Render pipeline:** URP 17.5.0  
**Result:** Passed

## Validated Scope

- Verified serialized Jupiter definition using JPL physical and J2000 orbital data.
- Solar System Scope 2K Jupiter texture connected through a project material.
- Deterministic Jupiter orbit and signed sidereal rotation through the existing Core evaluator.
- Jupiter body view and cached orbit path registered through the centralized composition root.
- Earth-to-Jupiter-to-Sun radius ordering under the provisional presentation scale.
- Earth-orbit and Jupiter-orbit distance ordering under logarithmic compression.
- Existing composition camera coverage for the complete representative graybox.
- Reproducible four-body editor build and sole enabled `SolarSystem` build scene.

## Scientific Record

The Jupiter asset resolves to source record
`JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_JUPITER_FACTS`.

Key authored values:

- Mean radius: `69,911 km`
- Mass: `1.898125 × 10^27 kg`
- Sidereal rotation period: `35,729.856 seconds`
- Axial tilt: `3 degrees`
- Semi-major axis: `778,340,816.6927109 km`
- Eccentricity: `0.04838624`
- Inclination: `1.30439695 degrees`
- Longitude of ascending node: `100.47390909 degrees`
- Argument of periapsis: `-85.74542926 degrees`
- Mean anomaly at J2000: `19.66796068 degrees`
- Sidereal orbital period: `374,355,659.124 seconds`

Primary-source links and conversions are maintained in
`Docs/Science/Celestial Data Sources.md`.

## Presentation-Scale Evidence

The existing provisional scale was retained unchanged:

- Earth display radius: `0.8` Unity units
- Jupiter display radius: `2.085572` Unity units
- Sun display radius: `4.8` Unity units
- Jupiter periapsis display distance: approximately `43.05` Unity units
- Jupiter apoapsis display distance: approximately `43.68` Unity units

At the J2000 authoring state, Jupiter appeared at approximately
`(34.67, -0.88, 25.54)` with viewport center `(0.73, 0.55)`. Earth, Moon,
Jupiter, and the Sun were all inside the existing camera viewport, so no camera
change was introduced solely to satisfy the proof.

This evidence validates range handling, not final UX approval. The exact curve
remains a tunable proposal under `TDD-OPEN-004`.

## Scene Contract

```text
SolarSystem
  _Application
    SolarSystemCompositionRoot
  _Simulation
    CelestialBodies
      Sun
        Visual
      Earth
        Visual
      Moon
        Visual
      Jupiter
        Visual
    OrbitPaths
      Earth Orbit
      Moon Orbit
      Jupiter Orbit
  _Environment
    Main Camera
    Sun Key Light
    Global Volume
  _Diagnostics
```

The initialized composition contains four catalog entries, four explicitly
registered views, and three cached orbit paths.

## Unity Validation Results

| Check | Result |
|---|---|
| Runtime, editor, and test assembly compilation | Pass |
| Unity Console errors | Pass: 0 |
| Unity Console warnings | Pass: 0 |
| Complete Edit Mode suite | Pass: 43 |
| Edit Mode failures, skipped, or inconclusive | Pass: 0 |
| Real-scene Play Mode suite | Pass: 1 |
| Play Mode failures, skipped, or inconclusive | Pass: 0 |
| Serialized catalog bodies | Pass: 4 |
| Explicitly registered body views | Pass: 4 |
| Cached orbit paths | Pass: 3 |
| Jupiter orbit samples | Pass: 257 closed-path positions |
| Required bodies inside camera viewport | Pass: 4 |
| Enabled build scene | Pass: `SolarSystem` |

The Play Mode case loaded the actual enabled scene, verified the composition,
observed Earth, Moon, and Jupiter moving from the shared clock, paused all three
through the central controller, checked all three paths, and confirmed that all
four bodies remained inside the camera viewport.

The first new Edit Mode run exposed a bit-exact assertion against a Unity-
serialized derived double. The value differed by approximately
`0.00000012 km`. The scientific data was correct; the test was corrected to use
explicit unit-appropriate tolerances, and the complete 43-case suite then
passed.

## Visual Inspection

Multi-angle Scene view inspection confirmed:

- Jupiter is immediately distinguishable from the terrestrial bodies.
- Jupiter remains smaller than the deliberately capped Sun and larger than Earth.
- The Earth-Moon system remains readable after the gas giant is introduced.
- Earth and Jupiter orbit tiers are visibly distinct.
- The complete representative system remains within the existing camera range.

## Licensing and Provenance

The active Jupiter texture is `TEX-SSS-011` from Solar System Scope under
CC BY 4.0. Its source original and Unity derivative share SHA-256
`B0F04D005350252636B0E3396FC592548CBD9E9126B269D32D5C6ABD4B0E4F2B`.
The required attribution remains recorded in
`Docs/Legal/ThirdPartyAssets.md`.

## Remaining Risks and Next Gate

- `TDD-OPEN-004` remains open for final guided-comparison scale tuning.
- Jupiter is a spherical graybox using mean radius; oblateness is not modeled.
- Atmosphere, animated cloud bands, the Great Red Spot treatment, rings, and
  Galilean moons remain visual/content work.
- The camera is composition-only; user-controlled free flight, focus, and
  transitions belong to Slice 3.
- Formal performance and memory profiling remains gated on the documented
  reference PC.

No commit or push was performed as part of this validation.
