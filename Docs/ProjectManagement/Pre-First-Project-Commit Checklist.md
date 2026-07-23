# SolarSystem Pre-First-Project-Commit Checklist

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Reviewed by:** Codex with Tanvir approval gates  
**Date:** 2026-07-22  
**Status:** Repository-baseline candidate prepared; awaiting owner commit decision

This is the project instance of the central Efficient Unity checklist. `[x]` means verified, `[ ]` means pending, and `[!]` means an approved exception.

## A. Authority and scope

- [x] Purpose, owner, repository, Unity version, URP, Windows target, and hardware category are recorded.
- [x] The living GDD is approved as product-design authority.
- [x] Living TDD authority and initial architecture are written; architecture-gating proposals await owner approval.
- [x] Milestone 0 and first implementation direction are identified.
- [x] URP, analytical orbits, and trunk-based development are approved.

## B. Git foundation

- [x] Branch, dirty state, remote, and initial history inspected.
- [x] Unity-aware `.gitignore` exists and excludes generated IDE, Unity, build, and local AI-assistant state.
- [x] Unity-aware `.gitattributes` exists and covers Unity YAML and binary formats.
- [x] Git LFS 3.6.1 is installed and binary attributes resolve to LFS.
- [x] Unity organization/cloud identifiers are classified as intentional project identity and were reviewed in the staged diff.
- [x] Generated Unity folders and IDE caches are ignored.
- [x] Repository text attributes explicitly normalize `.gitignore`, JSON, Markdown, CSV, and PowerShell files to LF.

## C. Unity integrity

- [x] No missing `.meta` partners were found under `Assets` on 2026-07-22.
- [x] The final staged tree has zero missing `.meta` partners and zero duplicate Unity GUIDs.
- [x] Separate project-authored Runtime, Editor, Edit Mode, and Play Mode test locations are established.
- [x] Icon, package, and ProjectSettings changes classified in `Repository Baseline Classification.md`.
- [x] Build Settings contains only `Assets/Scenes/SampleScene.unity`.
- [x] Package manifest/lock and URP settings received a read-only reference audit; see `Unity Package and URP Settings Audit.md`.
- [x] Owner-approved package cleanup applied and validated; Unity AI Assistant retained for MCP, scope-unneeded packages removed, and retained packages checked against `latestCompatible`.
- [x] Owner-approved mobile quality-tier and URP asset cleanup applied and validated; PC is the sole tier and active Standalone pipeline.
- [x] Clean Unity import/compilation verified on 2026-07-22 after explicit owner approval; see `Slice 0 Unity Validation.md`.
- [x] `SolarSystem.slnx` removed from tracking and covered by the generated IDE-file ignore rules.

## D. Structure and naming

- [x] Proposed `Assets/SolarSystem` authored root and folder structure are defined in the living TDD.
- [x] `Tanvir.SolarSystem` namespace and Level 2 assembly boundaries are owner-approved.
- [x] Branch convention: `<type>/<short-kebab-case-description>`.
- [x] Commit convention: Conventional Commits.
- [x] Initial folders, assembly definitions, namespaces, and test file pass naming review.

## E. Documentation and licensing

- [x] Create the portfolio/setup README entry point.
- [x] Living GDD, TDD, and Art Bible exist as versioned authority documents.
- [x] Third-party provenance and licenses are recorded in `Docs/Legal/ThirdPartyAssets.md` and the manifest.
- [x] Imported audio candidates use licenses that permit commercial use and redistribution; provenance is recorded for the separate asset-import review.
- [x] Originals remain under `SourceAssets`; reviewed Unity-ready copies remain under `Assets/SolarSystem/Content` with provenance recorded.
- [x] `SourceAssets`, `Assets/SolarSystem/Content`, and asset retrieval tooling are excluded from this baseline and reserved for a separate asset-import candidate.

## F. First commit review

- [x] Isolate exact baseline files from unrelated or unexplained changes.
- [x] Re-run large-file/LFS and `.meta` checks after staging candidates are known.
- [x] Make `git diff --check` pass or record an approved exception.
- [x] Review staged textual, serialized, binary, and LFS diff.
- [x] Applicable first-import, compilation, Console, and Edit Mode validation recorded in `Slice 0 Unity Validation.md`.
- [x] Strong-signature secret scan reports zero matches; the only generic-label match is the GDD policy sentence forbidding committed secrets.
- [x] No generated paths, asset-import paths, non-pointer blobs at or above 1 MiB, or invalid LFS pointers exist in the staged candidate.
- [x] Present proposed message and exact candidate scope to Tanvir.
- [ ] Obtain Tanvir's explicit approval before committing.
- [ ] Obtain Tanvir's explicit approval before pushing.

## Current blockers

| ID | Blocker | Required resolution | Status |
|---|---|---|---|
| BLK-001 | Template `URP.png` contained JPEG data and bypassed the PNG LFS rule | Remove it with the complete unused TutorialInfo template content | Resolved |
| BLK-002 | Self-deleting `HubForceResolve` files appear deleted | Classified as intentional one-time template cleanup | Resolved |
| BLK-003 | Package Manager, Shader Graph, and project-association settings were mixed | Classification recorded; session state restored and intentional settings retained | Resolved |
| BLK-004 | Untracked Unity AI Assistant settings lacked a track-or-ignore decision | Classified as local preferences and ignored | Resolved |
| BLK-005 | TDD, namespace, and initial assembly boundary awaited owner approval | Approved for Slice 0 implementation | Resolved |
| BLK-006 | First import modified an already tracked generated `SolarSystem.slnx` and introduced a whitespace failure | Removed from tracking and added to generated IDE-file ignore coverage | Resolved |

## Approval record

| Role | Name | Decision | Date |
|---|---|---|---|
| Project owner | Tanvir | Pending first commit review | 2026-07-22 |
