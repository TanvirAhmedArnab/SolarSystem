# Repository Baseline Classification

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Prepared by:** Codex  
**Date:** 2026-07-22  
**Status:** Prepared for owner review; not committed

## Purpose

This record separates intentional Unity project state from disposable machine or template churn before the first project-specific implementation commit.

## Classification

| Path | Classification | Prepared treatment | Commit boundary |
|---|---|---|---|
| `Assets/Editor.meta` | Intentional one-time template cleanup | Retain deletion | Repository baseline |
| `Assets/Editor/HubForceResolve.cs` | Self-deleting package-resolution bootstrap | Retain deletion | Repository baseline |
| `Assets/Editor/HubForceResolve.cs.meta` | Metadata for self-deleting bootstrap | Retain deletion | Repository baseline |
| `Assets/Readme.asset` and `Assets/TutorialInfo/**` | Unity template tutorial content, including a malformed icon that is not required by the project | Retain deletion of the complete template tutorial set | Repository baseline |
| `Assets/Settings/Mobile_RPAsset.asset` and `Assets/Settings/Mobile_Renderer.asset` | Mobile-only URP assets outside the approved Windows scope | Retain owner-approved deletion after quality-tier remapping and Unity validation | Unity project baseline |
| `ProjectSettings/PackageManagerSettings.asset` | Package Manager UI/session serialization with opaque entity IDs | Restore committed version | Excluded |
| `ProjectSettings/ProjectSettings.asset` | Project identity association: project name, organization, and cloud project ID | Retain after owner-authorized classification | Unity project baseline |
| `ProjectSettings/ShaderGraphSettings.asset` | Current Shader Graph schema adds an explicit default override flag | Retain as Unity serialization normalization | Unity project baseline |
| `ProjectSettings/Packages/com.unity.ai.assistant/Settings.json` | Local checkpoint and UI preferences | Ignore; do not track | Excluded |

## Git LFS note

The malformed template icon is deleted with the unused tutorial rather than normalized. The repository's binary patterns remain covered by Git LFS. The separate third-party asset-import candidate must verify every staged binary pointer and its local LFS object before commit approval.

## Proposed baseline commit separation

1. Repository and Unity baseline: repository policies, root documentation, design and technical authorities, assembly/test foundation, approved package cleanup, intentional template cleanup, PC-only URP settings, project identity, and Unity serialization normalization.
2. Third-party asset import: approved Unity-ready media, source originals, provenance ledger, manifest, and retrieval tooling after an independent LFS and licensing review.

Neither commit may include local Unity AI preferences, generated folders, or unexplained Package Manager session state.
