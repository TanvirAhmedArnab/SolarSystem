# Windows Owner Acceptance Walkthrough

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Assisted by:** Codex  
**Candidate build source:** `81ca928932d9d695ad019888c5abeabd0fb18baa`  
**Validation date:** 2026-07-26  
**Status:** In progress; owner-performed input and audible checks remain pending

## Purpose

This walkthrough is the human-visible acceptance record for the Windows
release candidate. Automated tests, screenshots, player logs, and performance
captures support it, but they cannot prove physical keyboard, mouse, speaker,
or perceived-motion behavior.

Record each check as `Pass`, `Fail`, or `Not available`. A failure must include
what happened, the expected behavior, and enough detail to reproduce it.

## Preconditions

- Use the generated Windows x86-64 release player, not Unity Play Mode.
- Begin in a `1920x1080` window when the display permits it.
- Confirm Windows is using the intended speakers or headphones.
- Leave the approved stored mix intact unless a step explicitly changes it:
  `65%` master, `18%` music, `45%` interface, and `22%` celestial ambience.
- Close unrelated high-load applications before judging smoothness.

## WIN-ACC-01 — Launch and Baseline Audio

Actions:

1. Launch `Solar System Simulation.exe`.
2. Wait for the overview to appear.
3. Listen without selecting a body.

Expected:

- The application opens without a crash.
- The Solar System animates smoothly.
- Ambient music is audible but does not dominate the presentation.
- No harsh click, pop, or sudden volume jump occurs during startup.

**Owner result:** Pass — Tanvir confirmed the simulation is clearly visible
and the ambient music is clearly audible on 2026-07-26.

## WIN-ACC-02 — Selection and Information

Actions:

1. Left-click Earth.
2. Left-click the Sun.
3. Left-click a small body such as Mercury.

Expected:

- The selection reticle follows the selected body.
- The status panel and selected-body panel show the same target.
- The facts and scale disclosure remain readable.
- Each selection produces a restrained interface response.

**Owner result:** Partial failure — Tanvir confirmed on 2026-07-27 that Earth,
Sun, and Mercury selection, information, readability, and scale disclosure all
pass. A subsequent Titan focus check exposed rapid one-frame reticle movement
on fast small bodies. The corrective candidate orders HUD projection after
camera tracking and locks a focused body's reticle to the viewport center.
Rebuilt-player retest remains pending.

## WIN-ACC-03 — Focus and Zoom

Actions:

1. Select Earth.
2. Press `F`.
3. Roll the mouse wheel toward Earth and then away from Earth.
4. Press `Esc` to leave focus.

Expected:

- `F` transitions the adaptive camera to Earth.
- The wheel changes viewing distance without crossing through Earth or
  becoming stuck.
- Earth remains framed while focused.
- `Esc` returns to free flight without opening Help on the same press.

**Owner result:** Pass after correction — Tanvir confirmed on 2026-07-26 that
`F` focuses the selected body and that mouse-wheel zoom now works. The visible
state also confirmed that `Esc` ended focus and cleared the target. The
original fixed world-space offset changed the Sun focus distance by only about
`0.13%` per standard Windows wheel notch; bounded proportional zoom now makes
the same input perceptible across every body scale while preserving safe
body-relative limits. A final confirmation against the rebuilt Windows player
will be recorded with the release artifact.

## WIN-ACC-04 — Camera Movement

Actions:

1. In free flight, hold the right mouse button and move the mouse.
2. Use `W`, `A`, `S`, and `D`.
3. Use `Q` and `E`.
4. Select a body, press `F`, and orbit with the right mouse button.

Expected:

- Right-mouse look is smooth and predictable.
- `WASD` moves horizontally/forward-back; `Q/E` moves vertically.
- Focus orbit keeps the selected body as the visual subject.
- No sudden inversion, uncontrollable drift, clipping, or camera jump occurs.

**Owner result:** Pending

## WIN-ACC-05 — Simulation Time

Actions:

1. Press `Space`.
2. Confirm motion stops, then press `Space` again.
3. Press `[` to slow the simulation.
4. Press `]` to speed it up.

Expected:

- The HUD changes between `RUNNING` and `PAUSED`.
- Simulation motion stops while UI and camera interaction remain available.
- Slow and fast controls change the displayed rate and visible motion.
- Resume continues cleanly without a positional jump.

**Owner result:** Partial pass — Tanvir confirmed on 2026-07-26 that `Space`
pauses and resumes the simulation as intended. Slower/faster rate controls
remain pending.

## WIN-ACC-06 — Navigation and Keyboard Focus

Actions:

1. Press `N` to open celestial navigation.
2. Move through entries with the documented keyboard controls.
3. Activate one entry.
4. Press `Esc`.

Expected:

- The navigator opens with a visible focused entry.
- Focus order is understandable and focus indicators remain visible.
- Activating an entry selects or focuses the intended body.
- `Esc` closes the navigator before affecting a deeper interaction state.

**Owner result:** Pending

## WIN-ACC-07 — Learning Modes and Guides

Actions:

1. Press `L` twice.
2. Press `O` twice.
3. Press `M` twice.
4. Press `C` and complete or exit the guided scale comparison.
5. Press `T` and advance through or exit the cinematic tour.

Expected:

- `L` toggles projected body labels.
- `O` toggles orbit guides.
- `M` changes between Full Motion and Reduced Motion.
- Scale comparison clearly explains the presentation/physical-scale
  difference and exits cleanly.
- The tour frames its subjects, advances correctly, and restores the prior
  camera state when finished or cancelled.

**Owner result:** Pending

## WIN-ACC-08 — Menu, Settings, and Persistence

Actions:

1. Press `H`.
2. Inspect Help, Settings, and Credits & Sources.
3. Toggle mute on and off.
4. Move each audio slider slightly, confirm the audible change, then restore
   the approved values.
5. Toggle Reduced Motion on and off.
6. Close and relaunch the player.

Expected:

- `H` opens the menu and `Esc` closes it.
- Keyboard focus order and indicators are visible throughout the menu.
- Mute silences all channels and unmute restores them.
- Master, music, interface, and celestial-ambience controls affect only their
  intended mix contribution.
- Reduced Motion replaces guided camera animation with immediate cuts.
- Restored values persist after relaunch.

**Owner result:** Pending

## WIN-ACC-09 — Spatial and Interface Audio

Actions:

1. Focus the Sun and listen while changing the camera angle.
2. Focus Earth and move toward and away from it.
3. Trigger selections and menu interactions.

Expected:

- The Sun's burning ambience behaves as a stable, non-directional 2D bed.
- Earth's ambience changes naturally with 3D distance and position.
- Interface sounds are clear, short, and quieter than the presentation mix.
- No channel clips, loops with a noticeable seam, or persists incorrectly
  after leaving its context.

**Owner result:** Pending

## WIN-ACC-10 — Responsive Presentation

Codex-assisted exact-client-area inspection is complete:

- `960x540`: Pass
- `1280x720`: Pass
- `1920x1080`: Pass

The status panel, Help launcher, complete quick-controls panel, selected-body
panel, Help, Settings, and Credits & Sources remained within frame. Compact
mode activated at the two smaller cases. The owner still confirms that manual
resize dragging feels stable and does not leave a panel stranded.

**Owner resize result:** Pending

## WIN-ACC-11 — Perceived Performance and Exit

Actions:

1. Use free flight, body focus, projected labels, scale comparison, and tour
   for several minutes.
2. Watch for stutter when changing modes or opening UI.
3. Close the player normally.

Expected:

- Interaction feels smooth on the available system.
- No recurring hitch, visual corruption, runaway comet population, or audio
  breakup appears.
- The player closes without hanging or displaying an error.

Automated available-hardware evidence already passes frame, CPU, GPU, process
memory, dedicated GPU memory, and cold-launch budgets. Managed-allocation and
approved mid-range-hardware certification remain explicitly incomplete.

**Owner result:** Pending

## Final Owner Decision

**Decision:** Pending  
**Owner:** Tanvir  
**Date:** Pending  
**Notes:** Pending

If the final release-source commit changes runtime behavior, repeat the
affected checks against the regenerated Windows player before packaging.
