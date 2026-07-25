# Solar System Keyboard and Mouse Controls

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Status:** Implemented exploration, navigation, labels, guided presentation, Help, settings, credits, and onboarding controls  
**Last updated:** 2026-07-25  
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
| Contextual back/menu | Escape | Close the menu, cancel the active guided/focus state, or open Help from free flight |
| Open or close navigator | N | Show or hide the complete parent-first celestial catalog |
| Toggle body labels | L | Show or hide projected body-name labels |
| Toggle orbit guides | O | Show or hide orbit paths without changing simulation state |
| Toggle reduced motion | M | Switch guided camera transitions between eased movement and instant cuts |
| Open or close Help | H | Open the unified Explorer Menu directly on Help |

Free-flight velocity accelerates and decelerates rather than changing
instantaneously. Input and camera updates use unscaled time, so pausing the
celestial simulation does not disable exploration.

## Celestial Navigator and Labels

Press `N` to open a scrollable list of every authored body. The list is ordered
parent-first: the Sun, each planet, and then each planet's authored moons.
Moon rows are indented and identify their parent. The current selection has a
leading marker and stronger border in addition to color.

| Intent | Binding | Behavior |
|---|---|---|
| Open or close navigator | N | Toggle the navigator without changing simulation time |
| Move between entries | Tab / Shift+Tab | Move keyboard focus forward or backward |
| Select and focus | Enter or left-click on an entry | Select the body, start the existing focus transition, and close the navigator |
| Toggle projected labels | L | Preserve or suppress body-name labels without changing body size |

Projected labels are read-only so they never block clicking the body beneath
them. In overview, the selected body is considered first, then stars/planets,
then moons; labels that would overlap another accepted label or a HUD panel are
suppressed. Focus mode shows only the focused target. Guided scale comparison
temporarily hides labels and closes/locks the navigator to protect its teaching
composition; the user's label preference remains intact afterward.

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

## Explorer Menu, Help, Settings, and Credits

`H` opens or closes the unified Explorer Menu on its Help page. The same modal
surface contains Settings and Credits & Sources. While it is open, world
selection, camera movement, zoom, time commands, the navigator, and guided
presentation inputs are gated so one key press cannot affect the simulation
behind the menu.

Escape follows one predictable priority:

1. close the Explorer Menu;
2. cancel an active cinematic tour;
3. cancel an active guided scale comparison;
4. cancel focus or a focus transition;
5. open Help when already in free flight.

The first launch opens Help with a short orientation. Closing it records
completion locally; Help remains available at any time through `H`, Escape
from free flight, or the visible menu launcher.

The Settings page exposes:

- master, music, interface, and celestial-ambience volume;
- mute without discarding the four saved channel levels;
- Full Motion or Reduced Motion;
- orbit-guide visibility;
- projected-body-label visibility;
- restore release defaults.

Numeric percentages accompany the audio sliders. Preferences are saved in a
versioned local record and applied through the same services used by keyboard
shortcuts, so UI and keyboard state cannot drift apart. Presentation settings
do not change scientific data or authoritative simulation state.

Credits & Sources presents concise release-facing attribution and points to the
complete versioned licensing and scientific-source ledgers in `Docs`.

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

## Cinematic Tour

| Intent | Binding | Behavior |
|---|---|---|
| Start tour | T | Enter the first authored chapter from the current explorer state |
| Advance chapter | T or **Next** | Skip to the next deterministic chapter |
| Finish | T or **Finish** on chapter 5 | Restore the exact pre-tour explorer state |
| Exit early | Escape or **Exit** | Cancel and restore the exact pre-tour explorer state |
| Toggle transition motion | M or **Motion** | Switch between eased camera travel and instant chapter/restore cuts; the preference persists locally |

The five chapters frame the Sun, Earth-Moon system, Jupiter and the four
Galilean moons, Saturn and its rings, and an outer-system finale with Neptune
and Triton. Chapter timing uses unscaled time. Planetary motion
continues from the authoritative simulation at the user's existing rate while
the camera tracks each live target group.

**Full Motion** uses each chapter's authored deterministic duration and easing.
**Reduced Motion** completes chapter entry, advance, and explorer restoration
instantly. The current mode is shown directly on the keyboard- and
mouse-accessible Motion control, and the selected mode is retained between
sessions.

The tour and guided scale comparison are mutually exclusive because they share
one camera and guided-UI ownership coordinator. During a tour, selection,
focus, free-flight, zoom, time controls, labels, and the navigator are
temporarily locked. Orbit guides are suppressed and only the current chapter's
featured body renderers are shown, without pausing simulation or audio.
Completion and cancellation restore the captured camera pose, focus
target/mode, clip planes, navigator visibility, label preference, orbit-guide
state, and every renderer's exact prior enabled state. Selection, time rate,
pause state, and audio settings are never changed by the tour.

## Deferred Controls

The following approved product capabilities remain pending for later release
slices:

- gamepad bindings, which are outside the first public-release baseline.
