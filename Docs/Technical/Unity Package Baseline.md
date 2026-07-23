# SolarSystem Unity Package Baseline

**Owner:** Tanvir  
**Status:** Approved project baseline  
**Version:** 1.0.0  
**Last verified:** 2026-07-22 with Unity 6000.5.3f1

## Purpose

This document records the direct package set intentionally retained for SolarSystem. `Packages/manifest.json` and `Packages/packages-lock.json` remain the executable authorities for exact versions and the resolved dependency graph.

## Retained versioned packages

| Package | Version | Decision |
|---|---:|---|
| Unity AI Assistant | `2.16.0-pre.1` | Required for the Unity MCP collaboration bridge. Approved pre-release exception. |
| Visual Studio Editor | `2.0.27` | Sole IDE integration; Tanvir uses Visual Studio Community. |
| Input System | `1.20.0` | Required for keyboard-and-mouse interaction. |
| Universal Render Pipeline | `17.5.0` | Approved production render pipeline. |
| Unity Test Framework | `1.7.0` | Required for Edit Mode and Play Mode validation. |
| Timeline | `1.8.12` | Retained conditionally for cinematic camera sequencing; reassess after the camera proof. |
| Unity UI | `2.5.0` | Retained conditionally until the UI Toolkit proof confirms whether uGUI is unnecessary. |

All versions above reported `latestCompatible` for Unity 6000.5.3f1 on 2026-07-22.

## Excluded by scope

- Unity AI Inference/Sentis: add only when an approved runtime model-inference feature exists.
- AI Navigation: add only when NavMesh-based navigation is approved.
- Unity Version Control/Plastic: Git and GitHub own version control for this project.
- Rider integration: use exactly one IDE integration unless a contributor requirement is documented.
- Multiplayer Center: add only for an approved multiplayer feature.
- Visual Scripting: the approved implementation uses C# and explicit assembly boundaries.

## Version policy

At project intake, query Unity Package Manager for the latest compatible stable version for the locked Unity editor. Do not interpret “latest” as an unverified preview, an editor upgrade, or a release that Unity marks incompatible. Record exact direct versions in the manifest and commit the regenerated lock file for deterministic resolution.

Pre-release packages require an explicit reason and validation. Unity AI Assistant is the current exception because its MCP bridge is part of the approved workflow. During active production, review updates at milestone boundaries, before release candidates, or when a relevant security/compatibility issue appears; do not silently upgrade packages on every editor launch.

## Change gate

For any add, removal, or upgrade:

1. Confirm the package supports approved project scope.
2. Inspect direct and transitive dependencies, licensing, preview status, and editor compatibility.
3. Change the smallest coherent package batch and let Unity regenerate the lock file.
4. Inspect scripting symbols and serialized settings for package residue.
5. Wait for import and compilation; require a clean Console.
6. Run relevant Edit Mode and Play Mode tests and the target-platform smoke build when appropriate.
7. Review manifest, lock, ProjectSettings, and generated-file diffs before requesting commit approval.

Packages may be reinstalled later when an approved feature creates a real dependency.
