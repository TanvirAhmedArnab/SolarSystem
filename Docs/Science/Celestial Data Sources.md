# Celestial Data Sources

**Project:** Solar System Simulation  
**Dataset slice:** Sun-Earth-Moon graybox  
**Verification date:** 2026-07-22  
**Authoring epoch:** J2000.0 where orbital elements provide an epoch  
**Accuracy tier:** Educational visualization; not a date-exact ephemeris

## Purpose

This record maps every scientific value in the first serialized catalog to an authoritative source and documents conversions or approximations. The Unity assets contain stable source-record IDs that resolve to the sections below.

## Sun

**Source record ID:** `NASA_NSSDC_SUN_EARTH_FACT_SHEET`

Authored values:

- Volumetric mean radius: `695,700 km`
- Mass: `1.9884 × 10^30 kg`
- Adopted sidereal rotation period at 16 degrees latitude: `609.12 hours`
- Obliquity to the ecliptic: `7.25 degrees`

The Sun is the catalog root and therefore has no parent orbit in this simulation.

Primary source:

- [NASA NSSDCA Sun Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/sunfact.html)

## Earth

**Source record ID:** `NASA_NSSDC_EARTH_AND_JPL_APPROX_POS_J2000`

Authored physical values:

- Volumetric mean radius: `6,371 km`
- Mass: `5.9722 × 10^24 kg`
- Sidereal rotation period: `23.9345 hours`
- Obliquity to the ecliptic: `23.44 degrees`

Authored J2000 orbital values:

- Semi-major axis: `1.00000261 AU`, converted with `1 AU = 149,597,870.7 km` to `149,598,261.150442527 km`
- Eccentricity: `0.01671123`
- Inclination: `-0.00001531 degrees`
- Mean longitude: `100.46457166 degrees`
- Longitude of perihelion: `102.93768193 degrees`
- Longitude of ascending node: `0 degrees`
- Argument of periapsis: `102.93768193 degrees`
- Mean anomaly: `-2.47311027 degrees`, calculated as mean longitude minus longitude of perihelion
- Sidereal period used by the graybox: `365.256363004 days`

The JPL row represents the Earth-Moon barycenter. Using it as Earth's heliocentric educational orbit is an explicit approximation suitable for this project’s approved accuracy tier, not a claim of date-exact Earth ephemeris.

Primary sources:

- [NASA NSSDCA Sun/Earth comparison](https://nssdc.gsfc.nasa.gov/planetary/factsheet/sunfact.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)

## Moon

**Source record ID:** `NASA_MOON_BY_NUMBERS_AND_JPL_DE405_LE405`

Authored physical values:

- Mean radius: `1,737.4 km`
- Mass: `7.34767309245735 × 10^22 kg`
- Sidereal rotation period: `27.322 days`
- Equatorial inclination used for the visual tilt: `6.68 degrees`

Authored J2000 mean ecliptic orbital values:

- Semi-major axis: `384,400 km`
- Eccentricity: `0.0554`
- Inclination: `5.16 degrees`
- Longitude of ascending node: `125.08 degrees`
- Argument of periapsis: `318.15 degrees`
- Mean anomaly: `135.27 degrees`
- Sidereal orbital period: `27.322 days`

Primary sources:

- [NASA Moon by the Numbers](https://science.nasa.gov/moon/by-the-numbers/)
- [JPL Planetary Satellite Physical Parameters](https://ssd.jpl.nasa.gov/sats/phys_par/)
- [JPL Planetary Satellite Mean Elements](https://ssd.jpl.nasa.gov/sats/elem/sep.html)

## Presentation Transformations

Physical values remain unchanged in the authoring assets and immutable runtime models. The graybox presentation uses a separate provisional scale asset:

- Parent-relative distance is projected with  
  `displayDistance = 15 × log10(1 + physicalDistanceKm / 1,000,000)`.
- Radius is projected with  
  `displayRadius = 0.8 × (physicalRadiusKm / 6,371)^0.4`.
- Display radius is clamped to `[0.18, 4.8]` Unity units.

These values are intentionally transformed for legibility. They remain a tunable graybox proposal under `TDD-OPEN-004` and must not be presented as literal physical scale.

## Known Scientific Limitations

- Orbital elements are fixed at their authoring epoch; secular element rates and perturbations are not evaluated.
- The Earth follows the JPL Earth-Moon barycenter approximation.
- The Moon uses fixed mean elements and does not model nodal or apsidal precession, libration, or solar perturbations.
- The Sun uses one adopted rotation period even though it rotates differentially by latitude.
- The simulation epoch is an educational reference state, not the current real-world date.
