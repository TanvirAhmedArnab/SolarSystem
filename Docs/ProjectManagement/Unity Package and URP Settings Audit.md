# Unity Package and URP Settings Audit

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Audit date:** 2026-07-22  
**Unity version:** 6000.5.3f1  
**Status:** Package baseline and Windows-only URP baseline applied and validated

## Executive finding

The template declared 47 direct dependencies: 13 versioned packages and 34 built-in modules. The approved cleanup removed six scope-unneeded versioned packages, leaving 41 direct dependencies: seven versioned packages and 34 built-in modules.

Unity AI Assistant is retained because this project deliberately uses its MCP bridge to connect Unity, Codex, and Unity AI. Unity AI Inference/Sentis is a separate runtime neural-network inference package and is not required by the bridge. The final package graph resolved cleanly, the stale `SENTIS_ANALYTICS_ENABLED` Standalone symbol was removed, and the Edit Mode smoke test passed.

## Registry package classification

| Package | Classification | Rationale |
|---|---|---|
| `com.unity.render-pipelines.universal` | Keep | Approved production render pipeline; current render settings depend on it. |
| `com.unity.inputsystem` | Keep | Keyboard-and-mouse input is required for the first release. |
| `com.unity.test-framework` | Keep | Required by the approved Edit Mode and Play Mode test assemblies. |
| `com.unity.ide.visualstudio` | Keep | Tanvir uses Visual Studio Community. Updated to the latest compatible version, `2.0.27`. |
| `com.unity.ide.rider` | Removed | Rider is not used; keeping two IDE integrations adds editor tooling without project value. |
| `com.unity.timeline` | Defer | May support cinematic camera sequences; decide with the camera implementation. |
| `com.unity.ugui` | Defer | Remove only after the UI Toolkit proof confirms no uGUI/TMP dependency. |
| `com.unity.ai.assistant` | Keep, approved pre-release exception | Version `2.16.0-pre.1` provides the Unity MCP bridge used by the approved collaboration workflow. It does not depend on Sentis. |
| `com.unity.ai.inference` | Removed | No runtime inference feature or model is approved. The stale Standalone Sentis symbol was also removed. |
| `com.unity.ai.navigation` | Removed | Analytical orbits and camera navigation do not use NavMesh. The sample scene contains no NavMesh data. |
| `com.unity.collab-proxy` | Removed | Repository workflow uses Git/GitHub, not Unity Version Control/Plastic SCM. |
| `com.unity.multiplayer.center` | Removed | The experience is single-player and offline. |
| `com.unity.visualscripting` | Removed | The approved implementation language is clean C# with assembly boundaries. |

## Built-in module classification

Retain initially: audio, animation, image conversion, JSON serialization, particles, 3D physics, UI, UI Elements, and Umbra. These are likely to support materials, selection/raycasting, visual polish, audio, and interface work.

High-confidence scope candidates after package cleanup: Adaptive Performance, legacy AI, Android JNI, cloth, 2D physics/core, terrain/terrain physics, tilemap, vehicles, XR, and wind.

Conditional modules: accessibility, asset bundles, Director, IMGUI, screen capture, analytics, UnityWebRequest modules, vector graphics, and video. Their fate depends on UI, Timeline, capture, telemetry, and asset-loading decisions. Aggressive built-in-module pruning is not required before Slice 1 and should never replace a Windows build validation.

## URP settings reference audit

Every file shown in the supplied screenshot was traced by `.meta` GUID rather than inferred from its name.

| Asset | Current references | Windows release decision |
|---|---|---|
| `DefaultVolumeProfile.asset` | Referenced by `UniversalRenderPipelineGlobalSettings.asset` | Keep. |
| `Mobile_Renderer.asset` | Its only project reference was `Mobile_RPAsset.asset` | Removed after the parent RP asset and quality-tier reference were removed. |
| `Mobile_RPAsset.asset` | Its only settings reference was the `Mobile` quality tier | Removed with the Mobile tier for the approved Windows-only release. |
| `PC_Renderer.asset` | Referenced by `PC_RPAsset.asset` | Keep. |
| `PC_RPAsset.asset` | Referenced by the `PC` quality tier, which is the `Standalone` default | Keep. |
| `SampleSceneProfile.asset` | Referenced by both RP assets and `SampleScene.unity` | Keep until a project-authored volume profile replaces it. |
| `UniversalRenderPipelineGlobalSettings.asset` | Registered in `ProjectSettings/GraphicsSettings.asset` | Keep. |

`QualitySettings.asset` now contains one quality tier, `PC`, at index 0. The current quality and Standalone default both resolve to index 0, and the active pipeline is `PC_RPAsset`.

## Completed mobile-settings removal

The owner-approved sequence was executed on 2026-07-22:

1. Removed the Mobile quality tier and remapped the PC tier to index 0.
2. Remapped current quality and all stored platform defaults to the sole valid index; cleared obsolete tier exclusions.
3. Removed `Mobile_RPAsset.asset` and its `.meta` file.
4. Removed `Mobile_Renderer.asset` and its `.meta` file after its final reference was gone.
5. Refreshed Unity, requested script compilation/domain reload, confirmed the active PC tier and pipeline, inspected the Console, and reran the Edit Mode smoke test.

Validation passed with zero Console warnings/errors and one of one Edit Mode tests passing in 0.013 seconds. A Windows smoke build remains a separately approved release validation step.

## Applied package baseline

The owner-approved package operation performed on 2026-07-22:

- retained `com.unity.ai.assistant` for the MCP bridge;
- removed AI Inference/Sentis, AI Navigation, Unity Version Control/Plastic, Rider, Multiplayer Center, and Visual Scripting;
- retained Visual Studio as the sole IDE integration;
- updated Visual Studio Editor from `2.0.26` to `2.0.27` and Input System from `1.19.0` to `1.20.0`, their latest versions compatible with Unity `6000.5.3f1` at audit time;
- allowed Unity to regenerate `Packages/packages-lock.json`;
- removed the orphaned `SENTIS_ANALYTICS_ENABLED` and `APP_UI_EDITOR_ONLY` Standalone scripting symbols and the stale `com.unity.dt.app-ui` build-settings object;
- verified zero Console warnings/errors and a passing Edit Mode smoke test.

Timeline, uGUI, built-in-module, and mobile URP decisions remain separate so later changes stay attributable and reviewable.
