# Solar System Keyboard and Mouse Controls

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Implemented exploration and guided scale-comparison controls  
**Last updated:** 2026-07-24  
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

The calibrated scene defines `1x` as one Earth sidereal rotation per real
second: `86,164.2` simulated seconds for each real second. The supported
bounded presets are `1x`, `10x`, `100x`, `1,000x`, and `10,000x`, and the
scene starts at `1x`. Every body derives its spin rate and direction from its
signed sidereal period, so relative speeds and the retrograde rotations of
Venus and Uranus remain intact. Speed changes do not resume a paused
simulation.

The bottom-left quick-control strip presents each key and action as a separate
visual group rather than a sentence. Distinct keycap colors support rapid
scanning, while the adjacent action text preserves meaning without relying on
color alone. The `SPACE` action changes from `PAUSE` to `RESUME` when the
simulation is paused.

## Guided Scale Comparison

| Intent | Binding | Behavior |
|---|---|---|
| Start or advance | C | Enter the comparison or advance to its next deterministic stage |
| Finish | C on stage 3 | Return to the exact prior explorer camera, selection, and simulation state |
| Exit early | Escape | Cancel from any stage and restore the exact prior explorer state |

The three stages are:

1. **Readable overview:** exact Earth-relative body radii with logarithmically
   compressed orbital distance.
2. **Linear orbit spacing:** body radii and distances share one scale, where
   one unit equals the conservative Mercury-Venus envelope gap of
   `37.659 million km`.
3. **Literal Earth-radius reference:** Earth radius equals one unit and the
   average Earth-Sun distance is approximately `23,481` units.

The simulation pauses for comparison. Selection, focus, free-flight, zoom, and
time commands are temporarily locked so the educational framing cannot be
accidentally broken. The guide is cancellable and does not discard the user's
previous selection, camera pose, focus mode, time rate, or paused/running
state.

## Deferred Controls

The following approved product capabilities remain pending for later release
slices:

- complete Help, settings, and contextual onboarding;
- reduced-motion or instant-transition setting;
- gamepad bindings, which are outside the first public-release baseline.
