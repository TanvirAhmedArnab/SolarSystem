# Release Player Settings Proposal

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Proposed; no Player Settings have been changed  
**Prepared:** 2026-07-26  
**Unity baseline:** `6000.5.3f1`

## Purpose

This document separates deliberate release identity and platform decisions
from Unity template defaults. Values must be applied through Unity or
project-owned Editor tooling, followed by serialized-diff review, compilation,
tests, builds, and player smoke testing.

## Current-State Findings

The current `ProjectSettings/ProjectSettings.asset` still contains:

- company name `DefaultCompany`;
- product name `SolarSystem`;
- template Standalone application identifier;
- version `0.1.0`;
- desktop default `1024×768`;
- a non-resizable desktop window;
- web default `960×600`;
- WebGL Decompression Fallback disabled.

These values are valid development defaults but are not an approved public
release identity.

## Proposed Shared Identity

| Setting | Proposed value | Rationale | Approval |
|---|---|---|---|
| Company name | `Tanvir` | Matches the approved creator credit without inventing a studio or legal entity | Pending |
| Product name | `Solar System Simulation` | Matches the GDD, repository, and public page title | Pending |
| Version | `1.0.0` | First complete public assignment and portfolio release | Pending |
| Application identifier | `com.tanvirahmedarnab.solarsystem` | Unique lowercase reverse-domain-style identifier aligned with the repository owner | Pending |
| Color space | Linear | Preserve the existing approved physically based URP presentation | Already configured |
| Active scene | `Assets/SolarSystem/Scenes/SolarSystem.unity` | Single production entry point | Already configured |

The application identifier is a technical package identity, not a claim that
Tanvir owns the `tanvirahmedarnab.com` domain.

## Proposed Windows Settings

| Setting | Proposed value | Rationale | Approval |
|---|---|---|---|
| Target | Windows x86-64 | Required downloadable assignment build | Approved platform |
| Default mode | Windowed | Predictable first launch on both large and small displays | Pending |
| Default resolution | `1280×720` | Supported 16:9 minimum QA case that fits common laptop displays | Pending |
| Resizable window | Enabled | Allows the responsive HUD to adapt to the user's display | Pending |
| Fullscreen switch | Enabled | Keeps maximized/fullscreen presentation available | Pending |
| Run in background | Disabled | Avoids unnecessary simulation and audio work while unfocused | Retain |
| Player log | Enabled for release candidate | Required for first public smoke-test evidence | Retain for RC |
| Scripting backend | IL2CPP | Stronger release build, stripping, and native-code portfolio baseline | Pending |
| Architecture | x86-64 only | Matches the approved hardware and avoids obsolete 32-bit output | Approved |
| Performance certification | `1920×1080` | Preserve the approved reference benchmark independently of first-launch size | Approved |

The `1280×720` default does not lower the quality target. It improves safe
first launch, while the release is still visually and performance tested at
`1920×1080`.

## Proposed WebGL Settings

| Setting | Proposed value | Rationale | Approval |
|---|---|---|---|
| Default canvas | `960×540` | 16:9 embedded presentation that fits an itch.io page and can expand fullscreen | Pending |
| Compression | Brotli | Unity 6 recommends Brotli for its smaller release downloads | Pending |
| Decompression Fallback | Enabled | Explicit assignment requirement and robust when host response headers are unavailable | Required |
| Data caching | Enabled | Preserve existing browser asset caching | Retain |
| Debug symbols | Disabled in public build | Avoid shipping release debug payload unless diagnosing a hosted failure | Retain |
| Threads | Disabled | Preserve broad browser compatibility and avoid cross-origin isolation requirements | Retain |
| Memory | Existing dynamic-growth policy | Change only if a measured WebGL build or hosted smoke test proves a problem | Retain provisionally |
| Web template | Unity default for first RC | Minimize host-specific template risk; customize only after functional validation | Retain provisionally |

Unity documents that Decompression Fallback embeds a JavaScript decompressor
and adds the `.unityweb` extension. It is appropriate when server response
headers cannot be configured, but it increases loader size and is less
efficient than native browser decompression. The assignment explicitly
requires it, so compatibility takes precedence for this release.

Official references:

- https://docs.unity3d.com/Manual/webgl-deploying.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/web-optimization-player.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/class-PlayerSettingsWebGL.html

## Build-Profile Policy

- Windows and WebGL builds must use the same clean, pushed release commit.
- Release builds must be non-development builds.
- Output must be outside the repository or under ignored `Build`/`Builds`
  directories.
- Build reports must record settings, result, warnings, errors, duration,
  output size, Unity version, release version, and commit.
- A settings validator must fail the release build when identity, version,
  enabled scenes, architecture, or WebGL Decompression Fallback drifts.
- Applying a build profile must not silently mutate unrelated project
  settings.

## Required Validation After Applying

1. Review the complete `ProjectSettings` and build-profile diff.
2. Let Unity resolve and recompile.
3. Confirm zero Console errors and review every warning.
4. Run the complete Edit Mode and Play Mode suites.
5. Validate 1280×720, 1920×1080, and small-window responsive behavior.
6. Build and smoke-test Windows.
7. Build WebGL and verify the `.unityweb` fallback output.
8. Upload WebGL privately, then test loading, focus, audio, resizing, and the
   browser console before public publication.

## Owner Decision Gate

Before changing Player Settings, Tanvir must approve or revise:

- the public version;
- company name;
- application identifier;
- Windows first-launch mode and resolution;
- IL2CPP for the Windows release;
- WebGL canvas size;
- Brotli plus Decompression Fallback.
