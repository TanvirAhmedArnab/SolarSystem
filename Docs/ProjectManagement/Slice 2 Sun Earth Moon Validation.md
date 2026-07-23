# Slice 2 Sun-Earth-Moon Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation date:** 2026-07-22  
**Last revalidated:** 2026-07-23  
**Unity version:** 6000.5.3f1  
**Render pipeline:** URP 17.5.0  
**Result:** Passed for the approved Sun-Earth-Moon proof

## Validated Scope

- Serialized `CelestialBodyDefinition` assets for the Sun, Earth, and Moon.
- Serialized catalog and provisional presentation-scale assets.
- Read-only conversion into the immutable Slice 1 Core model.
- One-time right-handed Core to Y-up Unity coordinate mapping.
- Monotonic hierarchy-relative distance compression and radius exaggeration.
- One centralized simulation controller with caller-owned state buffers.
- Explicitly registered body views with no runtime scene searches.
- Cached Earth and Moon orbit paths.
- Textured Sun, Earth, and Moon graybox materials.
- Reproducible editor builder and intentional `SolarSystem` build scene.
- Focused editor-builder boundaries for orchestration, asset authoring, build data, and scene construction.

## Scene Hierarchy

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
    OrbitPaths
      Earth Orbit
      Moon Orbit
  _Environment
    Main Camera
    Sun Key Light
    Global Volume
  _Diagnostics
```

The template `SampleScene` remains on disk but is no longer enabled in build settings. No existing scene was deleted.

## Unity Validation Results

| Check | Result |
|---|---|
| Runtime and editor assembly compilation | Pass |
| Unity Console errors | Pass: 0 |
| Unity Console warnings | Pass: 0 |
| Rebuild semantic contract | Pass: hierarchy, references, presentation values, environment, and build-scene registration preserved |
| Authored output stability | Pass: 10 scientific, material, and build-setting file hashes unchanged |
| Editor builder source boundaries | Pass: four focused source files, each below 300 lines |
| Complete Edit Mode suite | Pass: 40 |
| Edit Mode failures, skipped, or inconclusive | Pass: 0 |
| Real-scene Play Mode suite | Pass: 1 |
| Play Mode failures, skipped, or inconclusive | Pass: 0 |
| Serialized catalog bodies | Pass: 3 |
| Explicitly registered body views | Pass: 3 |
| Cached orbit paths | Pass: 2 |
| Enabled build scene | Pass: `SolarSystem` |

The Play Mode case loaded the actual enabled build scene, verified successful composition, observed Earth and Moon moving from the shared clock, paused both through the central controller, and confirmed both orbit paths.

The public editor command was rerun after separating its responsibilities. Scientific assets, materials, and build settings retained identical SHA-256 hashes. Unity assigned new local file IDs when it created the fresh scene, so the scene was compared semantically: the exact hierarchy, three initialized bodies, two cached paths, clock configuration, projected transforms, materials, camera, lighting, volume, and sole enabled build scene all matched the captured contract.

Two initial Edit Mode failures were traced to test defects: a `params` call represented a null array instead of one null catalog entry, and an exact `Vector3` comparison ignored expected float addition rounding. Both tests were corrected without weakening runtime validation; the complete suite then passed.

## Visual Inspection

Unity multi-angle Scene view inspection confirmed:

- The Sun and Earth-Moon system are separated at readable graybox distances.
- Earth and Moon remain visually distinct.
- Earth and Moon orbit paths are present and centered on their logical parents.
- The system remains within the configured camera and clipping range.

The presentation scale is intentionally non-physical and visibly tuned for this proof.

## Settings Classification

- `ProjectSettings/EditorBuildSettings.asset`: intentional; enables only `Assets/SolarSystem/Scenes/SolarSystem.unity`.
- `ProjectSettings/URPProjectSettings.asset`: automatic bookkeeping change removed from the candidate.
- `ProjectSettings/SceneTemplateSettings.json`: automatic template state removed from the candidate.

## Remaining Risks and Next Gate

- `TDD-OPEN-004` remains open: the current compression and exaggeration values are provisional.
- Jupiter remains outstanding from the broader TDD Slice 2 plan.
- The camera is composition-only; free flight, focus, transitions, and input belong to Slice 3.
- Materials are an initial textured proof, not the final atmosphere, clouds, emission, exposure, or lighting treatment.
- Orbit paths are cached at the active presentation scale; runtime scale-mode transitions are not implemented.
- UI, labels, selection, audio playback, and guided scale comparison remain deferred.

No commit or push was performed as part of this validation.
