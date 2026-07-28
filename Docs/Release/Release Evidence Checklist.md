# Release Evidence Checklist

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Active; no release candidate certified  
**Created:** 2026-07-26  
**Release authority:** Tanvir  
**Build and verification support:** Codex with owner-reviewed evidence

## Purpose

This checklist is the authoritative release ledger for the assignment and
portfolio publication. A checkbox may be marked only when the referenced
artifact or observation exists and identifies the exact source commit.

Build folders, ZIP archives, performance captures, and browser-session data
must remain outside Git. Versioned summaries and sanitized evidence may be
recorded under `Docs/Release`.

## Release Identity

| Field | Required value | Current state |
|---|---|---|
| Release version | `1.0.0` | Approved and serialized |
| Release commit | Full Git SHA, clean and pushed | Latest pushed source: `c130637f27db3601185017a8c64086bed74e530e`; ordered-builder hardening candidate and final synchronized release commit pending |
| Unity version | `6000.5.3f1` | Verified project baseline |
| Render pipeline | URP `17.5.0` | Verified project baseline |
| Required platforms | Windows x86-64, WebGL, and macOS Universal | Approved |
| macOS limitation | Unsigned, unnotarized, and untested | Approved and must be disclosed |
| itch.io page URL | Public, player-facing URL | Pending |
| Creator credit | `Created by Tanvir` | Approved copy; publication pending |

## Gate 0 — Assignment Interpretation

- [x] Owner approves the single adaptive multi-mode camera as the intentional
      replacement for the lesson's two fixed cameras.
- [x] Owner approves a simple rotating pooled comet spawner with collider-free
      comets, TrailRenderer VFX, and automatic off-camera despawn.
- [x] Owner confirms no body-specific selection audio is required for Mercury,
      Venus, or Mars.
- [x] The GDD decision log records the approved interpretation.
- [x] The implementation and assignment description match the decision.

## Gate 1 — Source Release Candidate

- [x] All approved content is implemented.
- [x] `companyName`, product name, application identifiers, version, desktop
      resolution, window policy, and WebGL settings are intentionally authored.
- [x] WebGL Decompression Fallback is enabled.
- [x] The production scene is the only enabled release scene.
- [x] Unity finishes compilation with zero Console errors.
- [x] Console warnings are zero.
- [x] Complete Edit Mode suite passes.
- [x] Complete Play Mode suite passes.
- [x] Keyboard-only menu and simulation walkthrough passes.
- [x] Focus order and visible focus indicators are inspected.
- [x] UI passes at 1280×720, 1920×1080, and the approved 960×540
      small-window case.
- [x] Audio, mute, four channel levels, and persistence pass.
- [x] Full Motion and Reduced Motion both pass.
- [ ] Credits and scientific disclosures match the versioned ledgers.
- [ ] Repository preflight, secret scan, generated-file scan, `.meta` scan, LFS
      scan, and staged diff review pass.
- [ ] Working tree is clean and the candidate commit is pushed.

### Gate 1 Evidence

| Evidence | Exact source | Result |
|---|---|---|
| Commit SHA | `[PENDING]` | Pending |
| Unity Console report | `Docs/ProjectManagement/Release Settings and Build Automation Validation.md` | 0 errors, 0 warnings |
| Edit Mode report and count | Unity Test Runner validation on `2026-07-27` | 226 passed |
| Play Mode report and count | Unity Test Runner validation on `2026-07-27` | 27 passed |
| Responsive UI report | `Docs/Release/Windows Release Build and Performance Validation.md` | Pass at exact 960×540, 1280×720, and 1920×1080 client areas |
| Accessibility walkthrough | `Docs/Release/Windows Owner Acceptance Walkthrough.md` | All 11 current-source acceptance groups passed by Tanvir on `2026-07-27`; final rebuilt-player smoke recheck pending |
| Repository preflight | `[PENDING]` | Pending |

## Gate 2 — Windows x86-64

- [x] Windows IL2CPP build support is installed.
- [x] Build is produced from clean, pushed candidate commit
      `f6f4d61d9f8a652851735b58edd5aed546f6c970`.
- [x] Build report records target, Unity version, version, result, warnings,
      errors, duration, size, and output.
- [x] Executable and companion data folder are present.
- [x] Player launches without a crash or release-blocking log entry.
- [ ] Main scene, input, selection, focus, zoom, time, navigation, labels,
      scale comparison, tour, settings, credits, lighting, and audio pass.
- [x] 1280×720 windowed presentation passes.
- [x] 1920×1080 presentation passes.
- [x] Approved 960×540 small-window presentation and UI safe areas pass;
      owner resize-drag feel remains in the manual walkthrough.
- [x] Owner-visible screenshot confirms that the status panel, Help button,
      and complete quick-controls panel remain readable and inside the current
      maximized player window; subsequent explicit launches prove the three
      exact resolution cases above.
- [ ] Cold launch is measured on the approved reference hardware class.
- [ ] Standalone performance capture meets the approved frame-time, allocation,
      process-memory, and rendering budgets.
- [x] Available-hardware cold launch is measured externally at `1.472` seconds
      to a responsive player window.
- [x] Available frame, CPU, GPU, process-memory, and rendering counters are
      captured across all 11 production scenarios; the managed-allocation
      counter remains unavailable, so formal performance certification is
      incomplete.
- [x] External dedicated application GPU memory is measured at
      `349,573,120` bytes peak across ten samples.

### Windows Artifact Record

| Field | Value |
|---|---|
| Source commit | `f6f4d61d9f8a652851735b58edd5aed546f6c970` |
| Build output | `Builds/Release/SolarSystem-1.0.0-Windows-x86_64/` |
| Executable | `Solar System Simulation.exe` |
| Player log inspected | `%USERPROFILE%/AppData/LocalLow/Tanvir Ahmed Arnab/Solar System Simulation/Player.log` |
| Smoke-test date and tester | `2026-07-27`; Codex structural validation plus Tanvir focus/zoom/reticle acceptance, remaining owner walkthrough checks pending |
| Performance evidence | `Builds/Release/Evidence/windows-performance-81ca928-full.json`; versioned summary in `Docs/Release/Windows Release Build and Performance Validation.md` |
| Uncompressed size | `1,164,372,950` bytes generated; final shippable archive size pending synchronized release packaging |
| ZIP path | `<release-root>/Archives/SolarSystem-[VERSION]-Windows-x86_64.zip` |
| ZIP SHA-256 | `[PENDING]` |
| itch.io classification | Executable / Windows |

## Gate 3 — WebGL

- [x] WebGL build support is installed for Unity `6000.5.3f1` on the release
      workstation.
- [x] Decompression Fallback is enabled in serialized Player Settings.
- [ ] Build is produced from the same release commit as Windows.
- [x] Build report records target, result, warnings, errors, duration, and size.
- [x] Expected `Build`, `TemplateData`, and entry files exist.
- [ ] Complete WebGL build folder is archived without an extra parent nesting
      level that prevents itch.io playback.
- [ ] Archive is uploaded as `This file will be played in the browser`.
- [ ] Embedded player loads successfully on itch.io.
- [x] First local interaction grants keyboard focus; hosted confirmation remains
      pending.
- [ ] Browser audio starts after an allowed user gesture.
- [ ] Main input and learning flows pass.
- [ ] Browser resize and fullscreen presentation pass.
- [x] Local browser console has no release-blocking error; hosted confirmation
      remains pending.
- [ ] Persistence behavior is verified within browser-storage limitations.

### WebGL Artifact Record

| Field | Value |
|---|---|
| Source commit | `99ac9a678dd57af3fe827455e6642e0c2e37f4d5` (provisional; synchronized rebuild pending) |
| Build output | `Builds/Release/SolarSystem-1.0.0-WebGL/` |
| Local smoke-test browser | Codex in-app Chromium over plain local HTTP |
| Local smoke-test date and tester | `2026-07-27`; Codex |
| Browser-console evidence | `0` errors; three documented non-blocking Unity/platform warnings |
| Build evidence | `Docs/Release/WebGL Build Validation.md` |
| Uncompressed size | `27,188,882` bytes |
| ZIP path | `<release-root>/Archives/SolarSystem-[VERSION]-WebGL.zip` |
| ZIP SHA-256 | `[PENDING]` |
| itch.io classification | This file will be played in the browser |

## Gate 3B — macOS Universal

- [x] macOS Standalone build support is installed.
- [x] Universal Intel 64-bit plus Apple silicon build is produced from clean,
      pushed target-restoration commit
      `bf80733370a847f05f41d6a0d8fb879fb6940eb1`.
- [ ] Final macOS artifact is regenerated from the same final release commit
      as Windows and WebGL.
- [x] Build report records target, result, warnings, errors, duration, size,
      Unity version, release version, source commit, and output.
- [x] Bundle structure and Universal Mach-O architecture records are verified.
- [ ] The complete `.app` bundle is archived without losing executable
      permissions.
- [ ] itch.io upload is classified for macOS.
- [ ] Download description clearly states that the artifact is unsigned,
      unnotarized, and not tested on macOS.
- [ ] No macOS functionality or Gatekeeper certification claim is made.

### macOS Artifact Record

| Field | Value |
|---|---|
| Source commit | `bf80733370a847f05f41d6a0d8fb879fb6940eb1` |
| Build output | `Builds/Release/SolarSystem-1.0.0-macOS-Universal/` |
| Application | `Solar System Simulation.app` |
| Build evidence | `Docs/Release/macOS Universal Build Validation.md` |
| Test device | Unavailable |
| Signing/notarization | Unavailable |
| ZIP path | `<release-root>/Archives/SolarSystem-[VERSION]-macOS-Universal.zip` |
| ZIP SHA-256 | `[PENDING]` |
| itch.io classification | Executable / macOS |

## Gate 4 — Documentation, Licensing, and Credits

- [ ] Living GDD reflects assignment and release scope.
- [ ] TDD reflects build architecture and platform constraints.
- [ ] Art Bible reflects release presentation and media requirements.
- [ ] README describes the released rather than in-development state.
- [ ] CHANGELOG contains the complete release entry.
- [ ] Release version and tag policy are consistent.
- [ ] Third-party asset ledger is rechecked against live source pages.
- [ ] Required CC BY 4.0 attribution and modification disclosure are present.
- [ ] USGS and NASA source credits are present without implying endorsement.
- [ ] CC0 audio provenance is retained.
- [ ] Inter and the SIL OFL 1.1 record are retained.
- [ ] Scientific sources and educational-accuracy limitations are linked.
- [ ] Project-authored MIT License is present.
- [ ] Known limitations are visible on the itch.io page.

## Gate 5 — Assignment and Portfolio Media

### Required Assignment Evidence

- [ ] Expanded scene hierarchy screenshot shows all parent GameObjects opened
      and names remain readable.
- [ ] Expanded Project assets screenshot shows authored folder organization and
      names remain readable.
- [ ] Gameplay screenshot demonstrates the running simulation.
- [ ] Modification description is included on itch.io.
- [ ] `Created by Tanvir` is included on itch.io.

### Portfolio Evidence

- [ ] Clean full-system overview.
- [ ] Focused Earth with correct Sun-facing illumination.
- [ ] Jupiter or Saturn hero view.
- [ ] Guided scale-comparison view.
- [ ] Responsive UI view at a smaller supported window.
- [ ] Short animated clip.
- [ ] Recorded portfolio video.
- [ ] Optional itch.io cover image.
- [ ] Personal portfolio page or case study.

### Media Record

| ID | Artifact | Dimensions/format | Source commit | Status |
|---|---|---|---|---|
| MEDIA-001 | Expanded scene hierarchy | PNG | `[PENDING]` | Pending |
| MEDIA-002 | Expanded Project assets | PNG | `[PENDING]` | Pending |
| MEDIA-003 | Gameplay overview | PNG | `[PENDING]` | Pending |
| MEDIA-004 | Earth hero | PNG | `[PENDING]` | Pending |
| MEDIA-005 | Giant-planet hero | PNG | `[PENDING]` | Pending |
| MEDIA-006 | Scale comparison | PNG | `[PENDING]` | Pending |
| MEDIA-007 | Cover image | PNG/JPG | `[PENDING]` | Optional |
| MEDIA-008 | Animated clip | GIF/MP4 | `[PENDING]` | Pending |
| MEDIA-009 | Portfolio video | MP4/hosted URL | `[PENDING]` | Pending |

## Gate 6 — Archive Integrity

- [x] Repository-owned validator/packager passes isolated commit, version,
      structure, exclusion, unsafe-entry, fail-before-write, archive-root,
      determinism, and macOS permission fixtures.
- [ ] Windows ZIP contains the executable and complete companion files.
- [ ] WebGL ZIP contains the playable files at the archive root expected by
      itch.io.
- [ ] macOS ZIP contains the `.app` at its root and preserves Unix executable
      metadata for the launcher and native libraries.
- [ ] All three archives open successfully after creation.
- [ ] SHA-256 hashes are recorded above and in the release manifest.
- [ ] Archive names and embedded version match the release.
- [ ] Archives contain no source files, credentials, logs, personal paths, or
      unrelated build outputs.
- [ ] Build folders and archives remain ignored or outside the repository.

## Gate 7 — itch.io Configuration

Account and authentication:

- [ ] Tanvir creates the itch.io account.
- [ ] Tanvir completes email verification and signs in personally.
- [ ] No password, two-factor code, recovery code, cookie, or session token is
      recorded by Codex or committed.

Page configuration:

- [ ] Title and short description match the approved offline draft.
- [ ] Full modification description, controls, accessibility notes, scaling
      disclosure, limitations, credits, and links are populated.
- [ ] Windows archive is classified as executable for Windows.
- [ ] WebGL archive is classified for browser play.
- [ ] Release Status is `Released`.
- [ ] Pricing is `No Payment`.
- [ ] Screenshots layout is `Sidebar`.
- [ ] Cover image is applied if approved.
- [ ] Visibility is `Public`.
- [ ] `Unlisted in search & browse` matches the final owner decision.
- [ ] Owner reviews exact uploads and settings before public publication.

## Gate 8 — Player-Facing Verification

- [ ] Save and open `View page`.
- [ ] Screenshots render in the Sidebar layout.
- [ ] Embedded WebGL player works.
- [ ] Windows download is visible and correctly labeled.
- [ ] Pricing displays as intended.
- [ ] Credits and links render correctly.
- [ ] Page opens from a fresh signed-out browser session.
- [ ] Page and WebGL build are checked on another computer if available.
- [ ] Final public URL is recorded below.
- [ ] Exact URL is submitted to the peer-review assignment.

### Final Publication Record

| Field | Value |
|---|---|
| Public URL | `[PENDING]` |
| Publication date | `[PENDING]` |
| Owner approval | `[PENDING]` |
| Fresh-session verification | `[PENDING]` |
| Second-computer verification | `[PENDING OR NOT AVAILABLE]` |
| Assignment submission date | `[PENDING]` |

## Definition of Release Completion

This release is complete only when every required checkbox above is supported
by direct evidence, the public page and both platform artifacts work from the
same pushed release commit, Tanvir has approved publication, and the exact
verified itch.io URL has been submitted.
