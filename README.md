# Solar System Simulation

Solar System Simulation is a polished Unity portfolio project that turns verified planetary data and deterministic analytical orbital motion into an accessible, cinematic exploration of our Solar System.

The project is currently in visual/content development. Its product, engineering, art, licensing, and repository decisions are maintained as living, reviewable documents rather than being left implicit in scenes or code.

## Portfolio goals

- Present the Sun, eight planets, and selected major moons with honest educational scaling.
- Demonstrate deterministic orbital mechanics without Rigidbody-driven orbits.
- Support free-fly and cinematic exploration with clear scientific overlays.
- Deliver a stable 60 FPS experience at 1080p on a reasonable mid-range gaming PC.
- Show professional Unity architecture, testing, documentation, licensing, and Git practices.

## Approved technical baseline

- Unity `6000.5.3f1`
- Universal Render Pipeline `17.5.0`
- Windows 10/11 x86-64
- Keyboard and mouse
- ScriptableObject-authored celestial data
- Double-precision domain calculations with float-space rendering
- Lightweight trunk-based development with short-lived feature branches
- Conventional Commits and explicit owner approval before every commit or push

## Repository map

```text
Assets/SolarSystem/        Project-authored Unity content, runtime, editor, and tests
Docs/Design/               Living product-design authority
Docs/Technical/            Living technical authority and package baseline
Docs/Art/                  Living visual and content-production authority
Docs/Legal/                Third-party provenance and licensing ledger
Docs/ProjectManagement/    Audits, validation evidence, and approval checklists
SourceAssets/              Licensed source files retained outside Unity's Assets tree
Tools/                     Reproducible project tooling
```

## Open the project

1. Install Unity Hub and Unity Editor `6000.5.3f1` with Windows build support.
2. Clone the repository with Git LFS installed: `git lfs install`, then `git clone <repository-url>`.
3. Add the cloned folder in Unity Hub and open it with the exact editor version above.
4. Allow Package Manager and asset import to finish before evaluating the Console.
5. Confirm the Console has no errors or warnings, then run the Edit Mode tests in Test Runner.

Generated folders such as `Library`, `Temp`, `Logs`, `Obj`, `UserSettings`, and IDE project files are intentionally not versioned.

## Documentation

- [Game Design Document](Docs/Design/GDD.md)
- [Technical Design Document](Docs/Technical/TDD.md)
- [Keyboard and Mouse Controls](Docs/Design/Controls.md)
- [Art Bible](Docs/Art/ArtBible.md)
- [Unity Package Baseline](Docs/Technical/Unity%20Package%20Baseline.md)
- [Third-Party Assets and Licensing](Docs/Legal/ThirdPartyAssets.md)
- [Celestial Data Sources](Docs/Science/Celestial%20Data%20Sources.md)
- [Pre-First-Project-Commit Checklist](Docs/ProjectManagement/Pre-First-Project-Commit%20Checklist.md)
- [Slice 1 Deterministic Simulation Validation](Docs/ProjectManagement/Slice%201%20Deterministic%20Simulation%20Validation.md)
- [Slice 2 Sun-Earth-Moon Validation](Docs/ProjectManagement/Slice%202%20Sun%20Earth%20Moon%20Validation.md)
- [Slice 2 Jupiter Scale Validation](Docs/ProjectManagement/Slice%202%20Jupiter%20Scale%20Validation.md)
- [Slice 3 Interaction Proof Validation](Docs/ProjectManagement/Slice%203%20Interaction%20Proof%20Validation.md)
- [Slice 3 Simulation Time and HUD Validation](Docs/ProjectManagement/Slice%203%20Simulation%20Time%20and%20HUD%20Validation.md)
- [Slice 3 Selection and Body Information Validation](Docs/ProjectManagement/Slice%203%20Selection%20and%20Body%20Information%20Validation.md)
- [Slice 4 Visual Foundation Validation](Docs/ProjectManagement/Slice%204%20Visual%20Foundation%20Validation.md)
- [Slice 4 Sun-Origin Illumination Validation](Docs/ProjectManagement/Slice%204%20Sun-Origin%20Illumination%20Validation.md)
- [Slice 4 Eight-Planet Content Validation](Docs/ProjectManagement/Slice%204%20Eight-Planet%20Content%20Validation.md)
- [Slice 4 Audio Baseline Validation](Docs/ProjectManagement/Slice%204%20Audio%20Baseline%20Validation.md)
- [Slice 4 Proportional Scale Calibration Validation](Docs/ProjectManagement/Slice%204%20Proportional%20Scale%20Calibration%20Validation.md)
- [Slice 4 Guided Physical Scale Comparison Validation](Docs/ProjectManagement/Slice%204%20Guided%20Physical%20Scale%20Comparison%20Validation.md)
- [Slice 4 Layered Earth Rendering Validation](Docs/ProjectManagement/Slice%204%20Layered%20Earth%20Rendering%20Validation.md)
- [Slice 4 Solar Surface and Corona Validation](Docs/ProjectManagement/Slice%204%20Solar%20Surface%20and%20Corona%20Validation.md)
- [Slice 4 Jupiter Hero Rendering Validation](Docs/ProjectManagement/Slice%204%20Jupiter%20Hero%20Rendering%20Validation.md)

## Current validation state

The deterministic simulation, full Sun/eight-planet/Earth-Moon baseline,
Slice 3 interaction vertical slice, and first visual-production foundation
compile successfully. The project currently passes 126 Edit Mode cases and
11 real-scene Play Mode cases. Coverage includes orbital behavior,
interaction, UI, selected-body facts, project-owned skybox and URP profile
contracts, audited planet textures and materials, generated Saturn rings,
full-system overview framing, Sun-origin radial illumination, and preserved
real-scene behavior. Every rendered body radius is now a strict multiple of
Earth's mean radius (`Earth = 1`), while orbital distances use a disclosed
readability compression with tested adjacent-orbit clearances. At `1x`, one
real second advances exactly one Earth sidereal rotation; all bodies derive
their spin rate and direction from signed source periods. The scene also
includes licensed event-driven music, spatial celestial ambience, and UI
feedback with independent runtime levels and mute. A cancellable three-stage
comparison now teaches the scale problem by moving from the readable overview
to one shared linear orbit unit and then to literal `Earth radius = 1`
spacing, while preserving the prior selection, time, audio, and camera state.
Earth now provides the representative layered-rendering proof with a custom
URP surface, ocean response, nightside-only city lights, independent clouds,
a restrained atmosphere rim, and close-focus orbit-guide suppression.
The Sun now provides a second representative hero treatment with a
project-owned deterministic surface shader, a thin separate corona, preserved
radial-light origin, and exposure-safe overview and close-focus presentation.
Jupiter now proves a reusable gas-giant pattern with an anchored source
texture, source-derived band relief, deterministic low-amplitude detail, and a
thin Sun-aware atmosphere while preserving its exact proportional radius,
analytical orbit, signed rotation, and Great Red Spot identity.
Detailed evidence is recorded in `Docs/ProjectManagement`.

## License

Project-authored source code and documentation are licensed under the [MIT License](LICENSE). Third-party media remains governed by its original license and is documented separately in the licensing ledger.

Copyright (c) 2026 Tanvir.
