# Solar System Keyboard and Mouse Controls

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Implemented Slice 3 interaction vertical slice  
**Last updated:** 2026-07-23  
**Input asset:** `Assets/SolarSystem/Settings/Input/IA_SolarSystem.asset`

This document is the readable control contract for the project-owned `Explorer`
Input System action map. The asset is generated reproducibly by the editor
graybox builder and must not be edited independently of this contract and its
automated tests.

## Free Flight

| Intent | Binding | Behavior |
|---|---|---|
| Move | W, A, S, D or arrow keys | Move relative to the camera |
| Elevate | Q / E | Move down / up in world space |
| Look | Hold right mouse button and move mouse | Rotate the camera |
| Temporary boost | Left Shift | Increase movement speed while held |
| Select body | Left mouse button | Raycast from the pointer and select a celestial body |
| Focus selection | F | Smoothly transition to the selected body |
| Return to free flight | Escape | Cancel a focus transition or leave focused mode without snapping |

Free-flight velocity accelerates and decelerates rather than changing
instantaneously. Input and camera updates use unscaled time, so pausing the
celestial simulation does not disable exploration.

## Focused View

| Intent | Binding | Behavior |
|---|---|---|
| Orbit | Hold right mouse button and move mouse | Orbit around the focused body |
| Zoom | Mouse wheel | Adjust focus distance within body-relative safety limits |
| Redirect | Select another body, then press F | Start a new transition from the current camera pose |
| Return to free flight | Escape | Keep the current camera pose and resume free flight |

Selection and focus are intentionally separate. Clicking a body changes
selection; it does not move the camera until the user presses F. Successful
selection is confirmed by the target name, a four-corner reticle around the
on-screen body, and an educational information panel. If the selected body
moves off-screen, the reticle hides while the selection remains active.

## Simulation Time

| Intent | Binding | Behavior |
|---|---|---|
| Pause or resume | Space | Toggle authoritative simulation-time advancement |
| Slower | Left bracket `[` | Move to the next slower supported preset |
| Faster | Right bracket `]` | Move to the next faster supported preset |

The current proof defines `1x` as one simulated day per real second. The
supported bounded presets are `1x`, `10x`, `100x`, `1,000x`, and `10,000x`;
the scene starts at `10x`. The values and default remain provisional product
tuning decisions, but the command boundary, units, bounds, and display contract
are implemented and tested. Speed changes do not resume a paused simulation.

The bottom-left quick-control strip presents each key and action as a separate
visual group rather than a sentence. Distinct keycap colors support rapid
scanning, while the adjacent action text preserves meaning without relying on
color alone. The `SPACE` action changes from `PAUSE` to `RESUME` when the
simulation is paused.

## Deferred Controls

The following approved product capabilities remain pending for later release
slices:

- guided scale-comparison controls;
- complete Help, settings, and contextual onboarding;
- reduced-motion or instant-transition setting;
- gamepad bindings, which are outside the first public-release baseline.
