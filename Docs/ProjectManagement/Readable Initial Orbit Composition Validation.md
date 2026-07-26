# Readable Initial Orbit Composition Validation

**Date:** 2026-07-26  
**Owner:** Tanvir  
**Status:** Implemented and validated

## Purpose

The authored J2000 state placed several planets in similar directions from the
Sun, so their silhouettes appeared visually crowded even though their orbital
radii were distinct. This slice improves the opening composition without
altering any planet's scientific source data.

## Decision

The explorer starts at one shared analytical-clock offset:

- J2000 offset: `4,904 days`
- Initial simulation time: `423,705,600 seconds`
- Minimum reviewed opening angle between any two planet directions:
  `30 degrees`

The offset is applied once by the scene composition root. Individual planet
mean anomalies, semi-major axes, eccentricities, inclinations, periods, sizes,
rotation rates, and rotation directions remain unchanged.

## Opening distribution

At the selected shared epoch, the evaluated planet directions are
approximately:

| Planet | Direction around the Sun |
|---|---:|
| Mercury | 184.73 degrees |
| Venus | 118.60 degrees |
| Earth | 254.80 degrees |
| Mars | 55.68 degrees |
| Jupiter | 87.08 degrees |
| Saturn | 219.19 degrees |
| Uranus | 9.03 degrees |
| Neptune | 333.27 degrees |

The closest pair is Mars and Jupiter at approximately `31.40 degrees`, above
the reviewed `30-degree` contract. These are presentation-space directions
derived from the same deterministic Kepler evaluator used during play.

## Scientific interpretation

This project is an educational visualization, not a date-exact ephemeris. The
chosen epoch is a coherent analytical state rather than eight independently
posed transforms. Planets may naturally appear aligned again later as time
advances; permanently forcing them apart would misrepresent their motion.

## Automated evidence

- Edit Mode tests evaluate all 28 unique planet pairs at the shared opening
  epoch and require at least `30 degrees` of angular separation.
- The existing complete-synodic-cycle tests continue to verify adjacent
  presentation clearances.
- A Play Mode scene assertion verifies that the production composition root
  starts at or after the centralized epoch contract.

## Unity validation

- Production scene rebuilt successfully in Unity `6000.5.3f1`.
- Edit Mode: `204 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Play Mode: `26 passed`, `0 failed`, `0 skipped`, `0 inconclusive`.
- Unity Console: `0 errors`, `0 warnings`.
- Production camera: inspected at `1920 x 1080`; planets open on distributed
  sides of the Sun rather than in the prior clustered row.
