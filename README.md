# Solar System Simulation

Solar System Simulation is a polished Unity portfolio project that turns verified planetary data and deterministic analytical orbital motion into an accessible, cinematic exploration of our Solar System.

The project is currently in foundation development. Its product, engineering, art, licensing, and repository decisions are maintained as living, reviewable documents rather than being left implicit in scenes or code.

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

## Current validation state

The approved foundation, deterministic simulation, representative Sun-Earth-Moon-Jupiter graybox, keyboard/mouse interaction, bounded simulation-time controls, and first professional HUD proof compile with a clean Console. The project currently passes 64 Edit Mode cases and three real-scene Play Mode cases. Coverage includes assembly boundaries, immutable authoring conversion, catalog validation, elliptical motion, hierarchy composition, coordinate mapping, presentation scaling, centralized view updates, project-owned input bindings, raycast selection, redirectable focus, damped free flight, bounded pause/speed commands, UI Toolkit asset contracts, live HUD feedback, and camera framing. Detailed evidence is recorded in `Docs/ProjectManagement` and will be refreshed at each material baseline change.

## License

Project-authored source code and documentation are licensed under the [MIT License](LICENSE). Third-party media remains governed by its original license and is documented separately in the licensing ledger.

Copyright (c) 2026 Tanvir.
