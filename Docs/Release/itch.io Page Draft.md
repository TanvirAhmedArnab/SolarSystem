# itch.io Page Draft

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Proposed copy; builds, media, URLs, and public publication pending  
**Prepared:** 2026-07-26  
**Authoritative sources:** Living GDD, Controls, Third-Party Assets ledger, and
Celestial Data Sources ledger

> This document is the offline source for the itch.io page. Replace every
> `[PENDING]` field with verified release evidence before entering the browser.
> The owner must approve the final page, uploads, pricing, visibility, and
> release status before publication.

## Page Metadata

- **Title:** Solar System Simulation
- **Project type:** HTML
- **Release status:** Released
- **Pricing:** No Payment
- **Visibility:** Public
- **Optional discovery setting:** Unlisted in search and browse only if Tanvir
  approves it before publication
- **Genre:** Educational
- **Tags:** Unity, Simulation, Educational, Space, Solar System, 3D
- **Created by:** Tanvir
- **Version:** `[PENDING RELEASE VERSION]`
- **Repository:** https://github.com/TanvirAhmedArnab/SolarSystem
- **Portfolio case study:** `[PENDING URL]`
- **Portfolio video:** `[PENDING URL]`

## Short Description

Explore a cinematic, data-driven model of the Solar System with deterministic
orbits, proportional planetary sizes, guided scale comparison, and accessible
educational controls.

## Full Description

**Solar System Simulation** is a polished educational Unity experience created
by Tanvir. Explore the Sun, all eight planets, and seven major moons through a
responsive free-flight camera, body-focused inspection, projected labels,
scientific information panels, and a five-chapter cinematic tour.

The simulation uses verified planetary parameters and deterministic analytical
orbital motion. It is designed as an educational visualization rather than a
date-exact ephemeris: the bodies follow convincing elliptical mean orbits, but
their positions do not represent the real Solar System on a particular date.

### What I Added and Modified

- Expanded the lesson-scale foundation into the Sun, all eight planets, and
  seven major moons.
- Created distinct physically based and project-authored visual treatments,
  including layered Earth rendering, atmospheric bodies, Saturn's rings, a
  live solar day/night terminator, and a restrained emissive Sun.
- Replaced Rigidbody orbital motion with deterministic analytical Kepler
  evaluation and source-driven signed rotation.
- Calibrated every visible body to its exact Earth-relative mean-radius ratio,
  with Earth defined as one radius unit.
- Added a guided three-stage comparison that demonstrates why realistic body
  sizes and interplanetary distances cannot be shown clearly at the same scale.
- Added free flight, selection, body focus, safe zoom, a complete celestial
  navigator, projected labels, and educational fact panels.
- Added a five-chapter cinematic tour of the Sun, Earth-Moon system, Jupiter
  system, Saturn, and the outer Solar System.
- Added a rotating pooled comet spawner with collider-free comet nuclei,
  restrained project-authored trails, and automatic off-camera cleanup.
- Added ambient music, interface feedback, Sun ambience, Earth spatial
  ambience, independent audio levels, mute, and persisted settings.
- Added first-launch Help, reduced-motion presentation, orbit-guide and label
  preferences, responsive UI, credits, and scientific-source disclosures.
- Added automated Edit Mode and Play Mode coverage plus a reproducible
  performance-profiling harness.

## How Scale Works

The default overview preserves exact relative body radii but logarithmically
compresses the enormous distances between orbits so the Solar System remains
readable on one screen. This is disclosed in the interface.

Press `C` to enter the guided physical-scale comparison:

1. readable overview;
2. one shared linear unit for body sizes and orbital spacing;
3. literal Earth-radius reference, where Earth radius equals one unit and the
   average Earth-Sun distance is approximately 23,481 units.

## Controls

### Exploration

- `W A S D` or arrow keys — move
- `Q / E` — move down or up
- hold right mouse button and move — look or orbit a focused body
- `Left Shift` — temporary movement boost
- left-click a body — select it
- `F` — focus the selected body
- mouse wheel — zoom while focused
- `Escape` — contextual back, cancel, or Help

### Simulation and Learning

- `Space` — pause or resume
- `[` / `]` — decrease or increase simulation speed
- `N` — celestial navigator
- `L` — projected body labels
- `O` — orbit guides
- `C` — guided physical-scale comparison
- `T` — cinematic tour
- `M` — Full Motion or Reduced Motion
- `H` — Help, Settings, and Credits

## Accessibility and Settings

- keyboard and mouse navigation;
- visible selection reticle plus textual target feedback;
- Full Motion and persisted Reduced Motion modes;
- independent master, music, interface, and celestial-audio levels;
- mute without discarding saved channel levels;
- toggleable labels and orbit guides;
- responsive interface safe areas;
- first-launch orientation and reopenable Help.

The release currently targets keyboard and mouse. Gamepad support is not part
of this version.

## Builds

### Play in Browser

`[PENDING VERIFIED WEBGL BUILD AND HOSTED SMOKE TEST]`

Click the embedded player once to give it keyboard focus and allow browser
audio playback.

### Windows Download

`[PENDING VERIFIED WINDOWS X86-64 BUILD]`

Extract the complete ZIP archive before launching the executable.

### macOS

`[PENDING UNSIGNED UNIVERSAL MACOS BUILD]`

This additional build targets Intel 64-bit and Apple silicon. It is unsigned,
unnotarized, and not tested on macOS because no Mac test device or Apple
Developer Program membership is available. macOS may show Gatekeeper warnings;
no compatibility certification is claimed.

## Known Limitations

- This is an educational mean-orbit visualization, not a date-specific
  astronomical ephemeris.
- Default orbital distances are compressed for readability; use the guided
  comparison for honest shared-scale context.
- The project includes seven selected major moons rather than every known moon.
- Asteroids, dwarf planets, spacecraft, collisions, N-body gravity, and
  scientifically propagated comet catalogs are outside this release. The
  included comet fly-throughs are illustrative presentation events rather than
  named or date-specific comet trajectories.
- Keyboard and mouse are the supported input devices.
- Some source planetary imagery contains enhanced color, incomplete coverage,
  interpolation, seams, synthesized color, or documented fill.
- WebGL performance and browser compatibility remain unverified until the
  hosted release candidate is tested.
- The macOS Universal artifact is unsigned, unnotarized, and untested.

## Credits

**Created by Tanvir**

Project-authored code and documentation are available under the MIT License.
Third-party media retains its original license.

- Planet and space textures: Solar System Scope, licensed under Creative
  Commons Attribution 4.0 International. Textures may include enhanced color
  and fictional fill for unmapped regions.
- Selected planetary mosaics: U.S. Geological Survey Astrogeology Science
  Center and source NASA missions. Individual source and citation records are
  maintained in the repository.
- “Outer Space Loop” by wipics, “Fireplace Sound Loop” by PagDev, and “Forest
  Ambience” by TinyWorlds, released under CC0 1.0 through OpenGameArt.
- Interface sounds by Kenney, released under CC0 1.0.
- Inter Regular and SemiBold by Rasmus Andersson, bundled under the SIL Open
  Font License 1.1.

Complete asset URLs, licenses, hashes, modifications, scientific sources, and
limitations:

- https://github.com/TanvirAhmedArnab/SolarSystem/blob/main/Docs/Legal/ThirdPartyAssets.md
- https://github.com/TanvirAhmedArnab/SolarSystem/blob/main/Docs/Science/Celestial%20Data%20Sources.md

No endorsement by NASA, USGS, or any asset provider is implied.

## Upload Manifest

| Upload | itch.io classification | Local evidence | Status |
|---|---|---|---|
| `SolarSystem-[VERSION]-WebGL.zip` | This file will be played in the browser | Build report, hosted smoke test, SHA-256 | Pending |
| `SolarSystem-[VERSION]-Windows-x86_64.zip` | Executable / Windows | Build report, local smoke test, SHA-256 | Pending |
| `SolarSystem-[VERSION]-macOS-Universal.zip` | Executable / macOS | Build report, SHA-256, unsigned/untested disclosure | Pending |

## Media Manifest

| Media | Purpose | Status |
|---|---|---|
| Cover image | Optional page hero | Pending |
| Gameplay overview | Show the complete system and responsive HUD | Pending |
| Focused Earth | Show layered Earth and Sun-facing illumination | Pending |
| Saturn or Jupiter hero | Show visual-production quality | Pending |
| Guided scale comparison | Show educational differentiation | Pending |
| Expanded scene hierarchy | Assignment evidence | Pending |
| Expanded Project assets | Assignment evidence | Pending |
| Portfolio video or animated clip | Portfolio deliverable | Pending |

## Pre-Publication Browser Checklist

- [ ] Replace every `[PENDING]` field.
- [ ] Upload and classify both archives correctly.
- [ ] Verify WebGL loads, accepts keyboard focus, plays audio after interaction,
      and resizes correctly.
- [ ] Add screenshots and optional cover.
- [ ] Set screenshot layout to `Sidebar`.
- [ ] Confirm modification description and `Created by Tanvir`.
- [ ] Confirm credits, licenses, and links.
- [ ] Set release status to `Released`.
- [ ] Set pricing to `No Payment`.
- [ ] Obtain owner approval of Public versus Public + unlisted discovery.
- [ ] Save, view the player-facing page, and perform a fresh-session check.
- [ ] Obtain owner confirmation before final public publication.
- [ ] Record and independently test the final URL.
