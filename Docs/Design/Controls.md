# Solar System Keyboard and Mouse Controls

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Implemented Slice 3 interaction proof  
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
selection; it does not move the camera until the user presses F.

## Deferred Controls

The following approved product capabilities are not part of this bounded proof
and remain pending for later Slice 3 candidates:

- simulation pause and speed controls;
- guided scale-comparison controls;
- HUD, help, and selection feedback;
- reduced-motion or instant-transition setting;
- gamepad bindings, which are outside the first public-release baseline.
