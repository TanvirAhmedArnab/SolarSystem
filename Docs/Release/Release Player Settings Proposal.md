# Release Player Settings Proposal

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Approved, applied, and validated; builds pending  
**Prepared:** 2026-07-26  
**Unity baseline:** `6000.5.3f1`

## Purpose

This document separates deliberate release identity and platform decisions
from Unity template defaults. Values must be applied through Unity or
project-owned Editor tooling, followed by serialized-diff review, compilation,
tests, builds, and player smoke testing.

## Pre-Application Findings

Before application, `ProjectSettings/ProjectSettings.asset` contained:

- company name `DefaultCompany`;
- product name `SolarSystem`;
- template Standalone application identifier;
- version `0.1.0`;
- desktop default `1024×768`;
- a non-resizable desktop window;
- web default `960×600`;
- WebGL Decompression Fallback disabled.

These values were valid development defaults but were replaced by the approved
public release identity on 2026-07-26.

## Approved Shared Identity

| Setting | Approved value | Rationale | Approval |
|---|---|---|---|
| Company name | `Tanvir Ahmed Arnab` | Uses the owner's public portfolio identity without inventing a studio or legal entity | Approved and applied |
| Product name | `Solar System Simulation` | Matches the GDD, repository, and public page title | Approved and applied |
| Version | `1.0.0` | First complete public assignment and portfolio release | Approved and applied |
| Application identifier | `com.tanvirahmedarnab.solarsystem` | Unique lowercase reverse-domain-style identifier aligned with the repository owner | Approved and applied |
| Color space | Linear | Preserve the existing approved physically based URP presentation | Already configured |
| Active scene | `Assets/SolarSystem/Scenes/SolarSystem.unity` | Single production entry point | Already configured |

The application identifier is a technical package identity, not a claim that
Tanvir owns the `tanvirahmedarnab.com` domain.

## Approved Windows Settings

| Setting | Approved value | Rationale | Approval |
|---|---|---|---|
| Target | Windows x86-64 | Required downloadable assignment build | Approved platform |
| Default mode | Windowed | Predictable first launch on both large and small displays | Approved and applied |
| Default resolution | `1280×720` | Supported 16:9 minimum QA case that fits common laptop displays | Approved and applied |
| Resizable window | Enabled | Allows the responsive HUD to adapt to the user's display | Approved and applied |
| Fullscreen switch | Enabled | Keeps maximized/fullscreen presentation available | Approved and applied |
| Run in background | Disabled | Avoids unnecessary simulation and audio work while unfocused | Retain |
| Player log | Enabled for release candidate | Required for first public smoke-test evidence | Retain for RC |
| Scripting backend | IL2CPP | Stronger release build, stripping, and native-code portfolio baseline | Approved and applied |
| Architecture | x86-64 only | Matches the approved hardware and avoids obsolete 32-bit output | Approved |
| Performance certification | `1920×1080` | Preserve the approved reference benchmark independently of first-launch size | Approved |

The `1280×720` default does not lower the quality target. It improves safe
first launch, while the release is still visually and performance tested at
`1920×1080`.

## Approved WebGL Settings

| Setting | Approved value | Rationale | Approval |
|---|---|---|---|
| Default canvas | `960×540` | 16:9 embedded presentation that fits an itch.io page and can expand fullscreen | Approved and applied |
| Compression | Brotli | Smaller public release downloads | Approved and applied |
| Decompression Fallback | Enabled | Explicit assignment requirement and robust when host response headers are unavailable | Required and applied |
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

## Approved macOS Settings

| Setting | Approved value | Rationale | Status |
|---|---|---|---|
| Target | macOS desktop | Third assignment artifact | Approved |
| Architecture | Universal: Intel 64-bit + Apple silicon | Broad Mac CPU coverage | Approved |
| Scripting backend | Mono for the Windows-hosted build | Avoids an unavailable local Xcode IL2CPP toolchain | Approved |
| Signing and notarization | None | No Apple Developer Program membership | Approved limitation |
| Test certification | Not available | No Mac test device | Required disclosure |

The macOS download must be labeled unsigned, unnotarized, and not tested on
macOS. It is an additional assignment artifact rather than a certified
portfolio platform.

## Build-Profile Policy

- Windows, macOS, and WebGL builds must use the same clean, pushed release
  commit.
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

Validation completed in this slice: serialized-diff review, Unity compilation,
settings/module validation, `208` Edit Mode tests, `26` Play Mode tests, and a
clean Console. Responsive player inspection and all three long builds remain
separate approved release steps.

## Owner Decision Record

Tanvir approved the complete shared, Windows, and WebGL proposal on
2026-07-26. Tanvir then added macOS Universal as a third target while accepting
that it will be unsigned, unnotarized, and untested because no Mac or Apple
Developer Program membership is available.
