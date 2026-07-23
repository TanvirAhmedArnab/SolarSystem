# Mobile URP Cleanup Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation date:** 2026-07-22  
**Unity version:** 6000.5.3f1  
**Result:** Passed

## Validated change

- Removed the `Mobile` quality tier.
- Remapped `PC` to quality index 0 and made it the current and Standalone default tier.
- Cleared obsolete target-platform exclusions from the sole remaining quality tier.
- Removed `Mobile_RPAsset.asset`, `Mobile_Renderer.asset`, and both Unity `.meta` partners.
- Preserved `PC_RPAsset.asset` and `PC_Renderer.asset`.

## Evidence

| Check | Result |
|---|---|
| Unity refresh completed | Pass |
| Script compilation/domain reload completed | Pass |
| Editor idle | Pass: `isCompiling=False`, `isUpdating=False` |
| Quality tier count | Pass: 1 |
| Sole quality tier | Pass: `PC` |
| Current quality index | Pass: 0 |
| Standalone default index | Pass: 0 |
| Active render pipeline | Pass: `PC_RPAsset` |
| Mobile RP asset absent as file and Unity object | Pass |
| Mobile renderer absent as file and Unity object | Pass |
| Console errors | Pass: 0 |
| Console warnings | Pass: 0 |
| Edit Mode smoke test | Pass: 1/1 in 0.013 seconds |

The first attempted MCP deletion was rejected because the Unity API could require an interactive confirmation; it made no changes. A later diagnostic loaded a duplicate in-memory QualitySettings manager and produced one validation-generated Console error. The subsequent domain reload cleared that transient diagnostic state, and the final Console inspection was clean.

No Windows build was run in this cleanup-only validation. The target-platform smoke build remains approval-gated.
