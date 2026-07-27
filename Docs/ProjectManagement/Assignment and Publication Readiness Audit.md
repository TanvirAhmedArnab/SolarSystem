# Assignment and Publication Readiness Audit

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Audit date:** 2026-07-26  
**Repository commit inspected:** `81ca928`  
**Status:** Windows and macOS builds verified; full Windows acceptance, WebGL
build, packaging, and publication evidence remain

## Purpose

This audit maps the peer-review assignment to the actual Unity project and the
stronger portfolio-release requirements. A requirement is complete only when
direct project, build, media, or publication evidence proves it.

## Current Project Evidence

- Unity `6000.5.3f1`, URP `17.5.0`.
- One enabled production scene:
  `Assets/SolarSystem/Scenes/SolarSystem.unity`.
- One organized scene root with application, simulation, environment, audio,
  interface, and diagnostics branches.
- The Sun, all eight planets, and seven approved moons are present.
- Every rendered body has a project-owned view adapter and source-grounded
  material path.
- Motion is driven by deterministic analytical orbit and signed-rotation
  systems rather than Rigidbody gravity.
- The scene contains one adaptive production camera, four audio sources, and
  one Sun-parented point light.
- A project-authored pooled comet spawner is present under the simulation
  hierarchy. Its six collider-free instances use bounded TrailRenderer VFX and
  return to the pool when expired or safely outside the camera.
- Installed platform modules include Windows Standalone, WebGL, and macOS
  Standalone.
- A Windows x86-64 IL2CPP release player has been built successfully from
  clean pushed commit `81ca928`, launched, log-smoke-tested, and measured with
  the project-owned 11-scenario performance harness.
- A macOS Universal application has been built from the same commit. Its
  bundle structure and Intel x86-64 plus Apple silicon arm64 launcher slices
  are verified; it remains unsigned, unnotarized, and untested on macOS.
- The source commit used for both builds was clean and synchronized with
  `origin/main`; this audit update is the only intended post-build source
  change.

## Assignment Compliance Matrix

| Requirement | Evidence | Status | Remaining action |
|---|---|---|---|
| Build the lesson's Solar System basis | Production scene and complete simulation are present | Exceeded | Describe the independent production architecture in submission notes |
| Add at least three planets | Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, and Neptune are present | Complete | None |
| Materials on added planets | Distinct Mercury, Venus, and Mars material/visual-layer assets are wired | Complete | Include representative screenshots |
| Behaviors on added planets | Data-driven orbit, signed rotation, selection, focus, labels, and facts are wired | Complete | None |
| Audio | Music, interface feedback, 2D Sun ambience, and 3D Earth ambience are implemented | Complete | Owner confirmed that Mercury, Venus, and Mars need no body-specific selection audio |
| Representative planet positions | Verified mean orbital data drives elliptical positions; readable compression is disclosed | Exceeded | Explain presentation scaling in the description |
| Adjust two cameras | One adaptive camera supports free flight, focus, guided comparison, and five cinematic chapters | Complete | Explain the intentional stronger single-camera architecture in the submission |
| Adjust comet spawner if necessary | A rotating Sun-relative spawner produces deterministic randomized pooled comets with project-authored nucleus/trail materials, no colliders, and automatic despawn | Complete | Disclose that comet paths and visual scale are illustrative rather than ephemeris-driven |
| Additional creative modifications | Eight planets, seven moons, scale comparison, navigator, labels, facts, settings, audio, and cinematic tour | Exceeded | Select highlights for the itch.io description |
| At least two builds | Windows x86-64 and macOS Universal release builds exist from the same pushed source commit | Complete | Build WebGL to deliver the approved third platform |
| WebGL decompression fallback | `webGLDecompressionFallback: 1`; protected by Edit Mode regression | Complete | Verify `.unityweb` output and hosted loading after the WebGL build |
| Test at least one desktop build | Windows player launches, remains responsive, initializes production systems, and has no release-blocking log entry | Partial | Complete the owner-visible input, audio, settings, motion, and responsive-layout walkthrough |
| Test WebGL on itch.io | No current WebGL build/page | Incomplete | Upload and smoke-test the embedded player |
| ZIP every build folder | No current release archives | Incomplete | Produce deterministic Windows, macOS, and WebGL ZIP archives |
| itch.io page | No authenticated publication evidence | Incomplete | Owner creates/signs into account; Codex configures page after approval |
| Expanded scene-hierarchy screenshot | Current hierarchy is suitable but no release evidence image exists | Incomplete | Expand every parent and capture readable evidence |
| Expanded project-assets screenshot | Folder organization exists but no release evidence image exists | Incomplete | Expand relevant authored folders and capture readable evidence |
| Builds classified correctly | No uploads exist | Incomplete | Mark Windows executable and WebGL browser-playable uploads |
| Modification description and credit | Runtime credits exist; itch.io copy does not | Incomplete | Publish modification summary and `Created by Tanvir` |
| Optional cover image | No release cover selected | Optional | Derive a clean hero cover from approved gameplay media |
| Released / No Payment / Sidebar / Public | No page exists | Incomplete | Configure and verify each field on itch.io |
| Submit and verify final URL | No page exists | Incomplete | Verify player-facing page, then submit exact URL |

## Accessibility Evidence

Implemented evidence:

- persisted reduced-motion mode;
- independent master, music, interface, and celestial audio levels plus mute;
- keyboard and mouse access to Help, navigation, focus, labels, orbit guides,
  time, scale comparison, cinematic tour, and menu routing;
- non-color-only selection reticle and textual state labels;
- responsive safe-area policies for panels and projected labels;
- motion blur disabled by default;
- no narrated content, so narration captions are not applicable.

Release evidence still required:

- complete keyboard-only walkthrough in a standalone build;
- focus-order and visible-focus inspection for every menu control;
- contrast and minimum-text-size inspection at supported resolutions;
- 1280x720, 1920x1080, and a small-window responsive pass;
- WebGL keyboard focus, audio-start, persistence, and browser-resize checks.

## Release Settings Gaps

The approved identity, version, application identifiers, window policy,
desktop/WebGL resolutions, Windows IL2CPP backend, Brotli compression, and
WebGL Decompression Fallback are now serialized and protected by Edit Mode
tests. Windows build and automated process/log/performance evidence now exist.
The macOS Universal bundle and both required CPU architectures are also
verified. Owner-visible input, audio, settings, motion, accessibility, and
responsive-layout acceptance remains pending.

## Platform Decision

The evidence supports a three-platform submission:

- all three build-support modules are installed;
- Windows can be tested locally and remains the performance authority;
- WebGL can be tested after itch.io upload;
- macOS Universal can be built from Windows but remains unsigned,
  unnotarized, and untested because no Mac or Apple Developer membership is
  available.

## Required Publication Sequence

1. Correct release Player Settings and add reproducible build automation.
2. Compile, test, and build Windows.
3. Run player performance certification and Windows smoke QA.
4. Build WebGL with decompression fallback and browser-safe settings.
5. Approve, commit, and push the validated macOS target-restoration hardening,
   then regenerate the unsigned Universal bundle.
6. Complete release licensing, documentation, and known limitations.
7. Capture hierarchy, assets, gameplay, and cover media.
8. Package and hash all three build archives.
9. Configure uploads and page metadata after owner authentication.
10. Verify the public player page and final peer-review URL.

## Resolved Assignment Interpretation

On 2026-07-26, Tanvir approved retaining the single adaptive multi-mode camera,
adding the scoped pooled comet presentation, and omitting body-specific
selection audio for Mercury, Venus, and Mars. The implementation and living
authorities now match that decision.
