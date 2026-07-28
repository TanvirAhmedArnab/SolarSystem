# Assignment and Publication Readiness Audit

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Audit date:** 2026-07-26  
**Repository commit inspected:** `99ac9a6`  
**Status:** Windows acceptance and provisional WebGL pipeline verified; final
synchronized builds, packaging, and publication evidence remain

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
- A WebGL release player has been built successfully from clean pushed commit
  `99ac9a6`, structurally validated, and loaded over plain local HTTP to prove
  Decompression Fallback. The simulation rendered, accepted the `H` keyboard
  command, and produced zero browser-console errors. Full hosted acceptance
  remains pending on itch.io.
- A Windows x86-64 IL2CPP release player has been rebuilt successfully from
  clean pushed commit `f6f4d61`, structurally validated, and owner-tested for
  focus, proportional zoom, and stable fast-body reticle behavior. Earlier
  available-hardware performance evidence remains valid for unchanged
  simulation/content systems. Tanvir has completed all eleven owner-visible
  Windows acceptance groups against the current release-source candidate; the
  final rebuilt player still requires a concise smoke recheck.
- A macOS Universal application has been regenerated from clean pushed
  target-restoration commit `bf807333`. Its bundle structure and Intel x86-64
  plus Apple silicon arm64 launcher slices are verified; it remains unsigned,
  unnotarized, and untested on macOS.
- Both source commits used for the verified desktop builds were clean and
  synchronized with `origin/main`. Final publication artifacts must later be
  regenerated from one final release commit.

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
| At least two builds | Verified Windows x86-64 and macOS Universal release builds exist from clean pushed source commits | Complete | Build WebGL, then regenerate all final artifacts from one release commit |
| WebGL decompression fallback | `webGLDecompressionFallback: 1`; protected by Edit Mode regression | Complete | Verify `.unityweb` output and hosted loading after the WebGL build |
| Test at least one desktop build | Windows launches cleanly, automated diagnostics pass, and Tanvir completed the full owner-visible input, audio, settings, motion, responsive-layout, performance, and exit walkthrough | Complete for current source candidate | Repeat a concise launch/focus/exit smoke check after the final synchronized rebuild |
| Test WebGL on itch.io | Provisional WebGL build passes local fallback-loading and keyboard-focus smoke checks; no itch.io page exists | Incomplete | Regenerate from the final commit, upload, and smoke-test the embedded player |
| ZIP every build folder | No current release archives; packager now strips Unity do-not-ship output across all platforms | Incomplete | Produce deterministic Windows, macOS, and WebGL ZIP archives |
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

- concise launch/focus/exit smoke confirmation in the final synchronized
  Windows rebuild;
- WebGL keyboard focus, audio-start, persistence, and browser-resize checks.

## Release Settings Gaps

The approved identity, version, application identifiers, window policy,
desktop/WebGL resolutions, Windows IL2CPP backend, Brotli compression, and
WebGL Decompression Fallback are now serialized and protected by Edit Mode
tests. Windows build and automated process/log/performance evidence now exist.
The macOS Universal bundle and both required CPU architectures are also
verified. Owner-visible input, audio, settings, motion, accessibility,
responsive-layout, perceived-performance, and exit acceptance now passes for
the current source candidate; final rebuilt-player smoke confirmation remains.

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
5. Build WebGL, then regenerate Windows, WebGL, and the unsigned Universal
   macOS bundle from one final release commit.
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
