# Slice 4 Proportional Scale Calibration Validation

**Project:** Solar System Simulation  
**Owner:** Tanvir  
**Validation date:** 2026-07-24  
**Unity:** 6000.5.3f1  
**URP:** 17.5.0  
**Status:** Implemented and validated; awaiting owner commit approval

## Scope

This candidate replaces the provisional exaggerated-radius graybox with a
reviewed educational presentation contract:

- Earth is the shared body-size reference at one display-radius unit.
- Every rendered body radius is its exact mean-radius ratio to Earth.
- Orbital distance remains logarithmically compressed for a readable overview.
- Adjacent orbit envelopes retain a documented minimum surface clearance.
- Every body's spin rate and direction derive from its signed sidereal period.
- The exact linear length conversion needed by the approved guided
  physical-scale comparison is available independently of overview scaling.

The guided comparison's player-facing transition, captions, narration, and
camera choreography are prepared but not implemented in this candidate.

## Scale contract

| Quantity | Reviewed value |
|---|---:|
| Earth physical reference radius | 6,371 km |
| Earth displayed radius | 1 unit |
| Radius conversion | `physicalRadiusKm / 6,371` |
| Distance reference | 1,000,000 km |
| Readable distance conversion | `160 × log10(1 + distanceKm / 1,000,000)` |
| Minimum adjacent surface clearance | 2.5 Earth-radius units |
| Minimum invisible selection radius | 1.5 units |
| Saturn outer ring envelope | 2.3 Saturn radii |

There is no radius exponent, minimum visual clamp, maximum visual clamp, or
body-specific visible exaggeration. The minimum selection radius affects only
raycast accessibility.

Representative rendered radii are:

| Body | Earth-relative radius |
|---|---:|
| Moon | 0.2727 |
| Earth | 1.0000 |
| Saturn | 9.1402 |
| Jupiter | 10.9733 |
| Sun | 109.1979 |

## Deterministic clearance evidence

Each adjacent planet pair is tested in two ways:

1. A conservative envelope uses the inner body's apoapsis, the outer body's
   periapsis, both effective body radii, and Saturn's ring envelope.
2. A complete deterministic synodic cycle is sampled at 4,096 intervals using
   the analytical Kepler evaluator.

The conservative envelope clearances are:

| Pair | Minimum surface clearance |
|---|---:|
| Sun-Mercury | 157.956 |
| Mercury-Venus | 28.298 |
| Venus-Earth | 18.751 |
| Earth-Mars | 19.648 |
| Mars-Jupiter | 63.994 |
| Jupiter-Saturn, including rings | 2.944 |
| Saturn-Uranus, including rings | 16.552 |
| Uranus-Neptune | 19.551 |
| Earth-Moon | 20.252 |

Jupiter-Saturn is intentionally the tightest reviewed pair and still exceeds
the 2.5-unit requirement. The Earth-Moon hierarchy and Sun-Mercury root pair
have dedicated regression coverage.

## Rotation and time contract

- Earth's verified sidereal rotation period is `86,164.2` seconds.
- At `1x`, one real second advances exactly `86,164.2` simulated seconds.
- Earth therefore completes one sidereal rotation per real second.
- Every other body evaluates its proportional angle from the same elapsed
  simulation time and its own signed source period.
- Venus and Uranus retain negative sidereal periods and rotate retrograde.
- All remaining authored bodies rotate in the positive convention.

This is a presentation time rate, not a claim that the scene represents the
current date.

## Guided physical-scale readiness

`PhysicalScaleReference` converts any physical length linearly using the same
`Earth radius = 1` reference. It demonstrates the core educational problem:

- Average Earth-Moon distance: approximately `60.336` Earth radii.
- Average Earth-Sun distance: approximately `23,481.127` Earth radii.

A literal system overview would make planets effectively invisible or require
an enormous camera range. The approved guided experience can now transition
between the readable overview and this exact reference without changing the
underlying scientific data.

## Automated validation

| Suite | Passed | Failed | Skipped | Inconclusive |
|---|---:|---:|---:|---:|
| Edit Mode | 101 | 0 | 0 | 0 |
| Play Mode | 7 | 0 | 0 | 0 |

New and updated coverage verifies:

- Exact Earth-relative radii for every catalog body.
- Conservative and sampled adjacent-orbit clearances.
- Sun-Mercury and Earth-Moon hierarchy clearances.
- The exact physical-comparison reference.
- Baseline simulation-time units.
- Signed sidereal spin rate and direction for every body.
- Real-scene proportional geometry, selection colliders, radial illumination,
  scene default rate, and preserved interaction behavior.

## Visual inspection

- The Sun is correctly dominant in the full-system overview.
- Giant planets remain larger than terrestrial planets in the correct ratios.
- Small rocky planets are intentionally small at overview scale.
- Invisible selection hit areas preserve mouse accessibility without changing
  visible geometry.
- Expanded orbit paths remain readable and subordinate to the bodies and HUD.
- The responsive HUD remains within the tested Game-view envelope.

## Final Unity result

- Compilation: passed
- Edit Mode: `101 passed`, `0 failed`, `0 skipped`, `0 inconclusive`
- Play Mode: `7 passed`, `0 failed`, `0 skipped`, `0 inconclusive`
- Final validation produced no new Console errors or warnings.
- Historical Console entries from earlier interrupted diagnostic runs were
  retained rather than cleared through editor internals.
- Editor state: Edit Mode

## Repository preflight

- Staged files: `30`
- Generated paths: `0`
- Staged files over 5 MB: `0`
- Repository files over 5 MB: `2`; both copies of the approved Sun ambience
  are correctly governed by Git LFS.
- Missing Unity `.meta` files: `0`
- Orphan Unity `.meta` files: `0`
- Secret-pattern matches: `0`
- Merge-conflict markers: `0`
- `git diff --cached --check`: passed
- `git lfs fsck --pointers`: passed

The unrelated Unity serialization changes in
`ProjectSettings/PackageManagerSettings.asset`,
`ProjectSettings/ProjectSettings.asset`, and
`ProjectSettings/URPProjectSettings.asset` will remain deliberately unstaged.

The candidate is staged for owner review. No commit or push has been performed
for this goal.
