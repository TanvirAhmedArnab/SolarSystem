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
- [Art Bible](Docs/Art/ArtBible.md)
- [Unity Package Baseline](Docs/Technical/Unity%20Package%20Baseline.md)
- [Third-Party Assets and Licensing](Docs/Legal/ThirdPartyAssets.md)
- [Pre-First-Project-Commit Checklist](Docs/ProjectManagement/Pre-First-Project-Commit%20Checklist.md)
- [Slice 1 Deterministic Simulation Validation](Docs/ProjectManagement/Slice%201%20Deterministic%20Simulation%20Validation.md)

## Current validation state

The approved foundation and deterministic-simulation slice have completed Unity import and compilation with a clean Console. The project Edit Mode suite currently passes 31/31 cases covering assembly boundaries, catalog validation, simulation time, elliptical orbital motion, hierarchy composition, analytical speed, rotation, and repeatability. Detailed evidence is recorded in `Docs/ProjectManagement` and will be refreshed at each material baseline change.

## License

Project-authored source code and documentation are licensed under the [MIT License](LICENSE). Third-party media remains governed by its original license and is documented separately in the licensing ledger.

Copyright (c) 2026 Tanvir.
