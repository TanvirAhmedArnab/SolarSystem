# Package Baseline Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation date:** 2026-07-22  
**Unity version:** 6000.5.3f1  
**Result:** Passed

## Validated change

- Kept Unity AI Assistant `2.16.0-pre.1` for the MCP bridge.
- Removed Unity AI Inference/Sentis, AI Navigation, Unity Version Control/Plastic, Rider, Multiplayer Center, and Visual Scripting.
- Kept Visual Studio as the sole IDE integration.
- Updated Visual Studio Editor to `2.0.27` and Input System to `1.20.0`.
- Removed the orphaned `SENTIS_ANALYTICS_ENABLED` and `APP_UI_EDITOR_ONLY` Standalone scripting symbols and the stale `com.unity.dt.app-ui` build-settings object.
- Allowed Unity Package Manager to regenerate the dependency lock.

## Evidence

| Check | Result |
|---|---|
| Unity package resolution finished | Pass |
| Editor idle after resolution | Pass: `isCompiling=False`, `isUpdating=False` |
| Removed packages absent from registered graph | Pass |
| Unity AI Assistant remains registered | Pass: `2.16.0-pre.1` |
| Visual Studio Editor latest compatible | Pass: `2.0.27` |
| Input System latest compatible | Pass: `1.20.0` |
| Sentis scripting symbol absent | Pass |
| App UI scripting symbol and build-settings object absent | Pass |
| Console errors | Pass: 0 |
| Console warnings | Pass: 0 |
| Edit Mode smoke test | Pass: 1/1 in 0.015 seconds |

The project-local Editor log contains transient Unity Licensing Client 404 messages followed by successful entitlement resolution. They were not Unity Console warnings/errors and did not affect package resolution, compilation, or tests.

No Windows build was run as part of this package-only validation. The target-platform smoke build remains a later approval-gated validation step.
