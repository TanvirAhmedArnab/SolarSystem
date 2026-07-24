# Celestial Data Sources

**Project:** Solar System Simulation  
**Dataset slice:** Sun, eight planets, and Earth's Moon baseline  
**Verification date:** 2026-07-23  
**Authoring epoch:** J2000.0 where orbital elements provide an epoch  
**Accuracy tier:** Educational visualization; not a date-exact ephemeris

## Purpose

This record maps every scientific value in the serialized graybox catalog to an authoritative source and documents conversions or approximations. The Unity assets contain stable source-record IDs that resolve to the sections below.

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

## Jupiter

**Source record ID:** `JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_JUPITER_FACTS`

Authored physical values:

- Volumetric mean radius: `69,911 km`
- Mass: `1.898125 × 10^27 kg`
- Sidereal rotation period: `0.41354 days`, converted to `35,729.856 seconds`
- Axial tilt: `3 degrees`

Authored J2000 orbital values:

- Semi-major axis: `5.202887 AU`, converted with `1 AU = 149,597,870.7 km` to `778,340,816.6927109 km`
- Eccentricity: `0.04838624`
- Inclination: `1.30439695 degrees`
- Mean longitude: `34.39644051 degrees`
- Longitude of perihelion: `14.72847983 degrees`
- Longitude of ascending node: `100.47390909 degrees`
- Argument of periapsis: `-85.74542926 degrees`, calculated as longitude of perihelion minus longitude of ascending node
- Mean anomaly: `19.66796068 degrees`, calculated as mean longitude minus longitude of perihelion
- Sidereal orbital period: `11.862615 Julian years`, converted to `4,332.82012875 days`

Primary sources:

- [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)
- [NASA Jupiter Facts](https://science.nasa.gov/jupiter/jupiter-facts/)

## Mercury

**Source record ID:** `JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_MERCURY_FACTS`

Authored physical values:

- Mean radius: `2,439.4 km`
- Mass: `3.30103 × 10^23 kg`
- Sidereal rotation period: `58.6462 days`
- Educational axial tilt: `2 degrees`

Authored J2000 orbital values:

- Semi-major axis: `0.38709927 AU` = `57,909,226.54152438 km`
- Eccentricity: `0.20563593`
- Inclination: `7.00497902 degrees`
- Longitude of ascending node: `48.33076593 degrees`
- Argument of periapsis: `29.12703035 degrees`
- Mean anomaly: `174.79252722 degrees`
- Sidereal orbital period: `0.2408467 Julian years` = `87.969257175 days`

Primary sources:

- [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)
- [NASA Mercury Facts](https://science.nasa.gov/mercury/facts/)

## Venus

**Source record ID:** `JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_VENUS_FACTS`

Authored physical values:

- Mean radius: `6,051.8 km`
- Mass: `4.86731 × 10^24 kg`
- Signed sidereal rotation period: `-243.018 days`; the negative sign records retrograde rotation
- Educational axial tilt: `3 degrees`, paired with the signed rotation direction

Authored J2000 orbital values:

- Semi-major axis: `0.72333566 AU` = `108,209,474.53737916 km`
- Eccentricity: `0.00677672`
- Inclination: `3.39467605 degrees`
- Longitude of ascending node: `76.67984255 degrees`
- Argument of periapsis: `54.92262463 degrees`
- Mean anomaly: `50.37663232 degrees`
- Sidereal orbital period: `0.61519726 Julian years` = `224.700799215 days`

Primary sources:

- [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)
- [NASA Venus Facts](https://science.nasa.gov/venus/venus-facts/)

## Mars

**Source record ID:** `JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_MARS_FACTS`

Authored physical values:

- Mean radius: `3,389.5 km`
- Mass: `6.41691 × 10^23 kg`
- Sidereal rotation period: `1.02595676 days`
- Axial tilt: `25.2 degrees`

Authored J2000 orbital values:

- Semi-major axis: `1.52371034 AU` = `227,943,822.42757303 km`
- Eccentricity: `0.09339410`
- Inclination: `1.84969142 degrees`
- Longitude of ascending node: `49.55953891 degrees`
- Argument of periapsis: `-73.50316850 degrees`
- Mean anomaly: `19.39019754 degrees`
- Sidereal orbital period: `1.8808476 Julian years` = `686.9795859 days`

Primary sources:

- [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)
- [NASA Mars Quick Facts](https://science.nasa.gov/resource/mars-quick-facts/)

## Saturn

**Source record ID:** `JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_SATURN_FACTS`

Authored physical values:

- Mean radius: `58,232 km`
- Mass: `5.68317 × 10^26 kg`
- Sidereal rotation period: `0.44401 days`
- Axial tilt: `26.73 degrees`

Authored J2000 orbital values:

- Semi-major axis: `9.53667594 AU` = `1,426,666,414.1799209 km`
- Eccentricity: `0.05386179`
- Inclination: `2.48599187 degrees`
- Longitude of ascending node: `113.66242448 degrees`
- Argument of periapsis: `-21.06354617 degrees`
- Mean anomaly: `-42.64463408 degrees`
- Sidereal orbital period: `29.447498 Julian years` = `10,755.6986445 days`

Primary sources:

- [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)
- [NASA Saturn Facts](https://science.nasa.gov/saturn/facts/)

## Uranus

**Source record ID:** `JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_URANUS_FACTS`

Authored physical values:

- Mean radius: `25,362 km`
- Mass: `8.68099 × 10^25 kg`
- Signed sidereal rotation period: `-0.71833 days`; the negative sign records retrograde rotation
- Axial tilt: `97.77 degrees`

Authored J2000 orbital values:

- Semi-major axis: `19.18916464 AU` = `2,870,658,170.6557322 km`
- Eccentricity: `0.04725744`
- Inclination: `0.77263783 degrees`
- Longitude of ascending node: `74.01692503 degrees`
- Argument of periapsis: `96.93735127 degrees`
- Mean anomaly: `142.28382821 degrees`
- Sidereal orbital period: `84.016846 Julian years` = `30,687.1530015 days`

Primary sources:

- [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)
- [NASA Uranus Facts](https://science.nasa.gov/uranus/facts/)

## Neptune

**Source record ID:** `JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_NEPTUNE_FACTS`

Authored physical values:

- Mean radius: `24,622 km`
- Mass: `1.024092 × 10^26 kg`
- Sidereal rotation period: `0.67125 days`
- Educational axial tilt: `28 degrees`

Authored J2000 orbital values:

- Semi-major axis: `30.06992276 AU` = `4,498,396,417.0094671 km`
- Eccentricity: `0.00859048`
- Inclination: `1.77004347 degrees`
- Longitude of ascending node: `131.78422574 degrees`
- Argument of periapsis: `-86.81946347 degrees`
- Mean anomaly: `-100.08479196 degrees`
- Sidereal orbital period: `164.79132 Julian years` = `60,190.02963 days`

Primary sources:

- [JPL Planetary Physical Parameters](https://ssd.jpl.nasa.gov/planets/phys_par.html)
- [JPL Approximate Positions of the Planets](https://ssd.jpl.nasa.gov/planets/approx_pos.html)
- [NASA Neptune Facts](https://science.nasa.gov/neptune/neptune-facts/)

## Presentation Transformations

Physical values remain unchanged in the authoring assets and immutable runtime
models. The reviewed readable-overview presentation uses a separate scale
asset and shared constants:

- Parent-relative distance is projected with  
  `displayDistance = 160 × log10(1 + physicalDistanceKm / 1,000,000)`.
- Radius is projected with  
  `displayRadius = physicalRadiusKm / 6,371`.
- Earth therefore has radius `1`; every other visible body uses the same linear
  ratio with no exponent, clamp, or per-body exaggeration.
- Adjacent planet orbit envelopes retain at least `2.5` Earth-radius display
  units of tested surface clearance. Saturn's ring envelope is included.
- Sub-pixel bodies may use an invisible selection radius of `1.5` units. This
  affects raycast accessibility, not rendered size.

Body-size ratios are physically proportional; orbital distance is intentionally
compressed for legibility and must not be presented as literal physical scale.
For the approved guided comparison, the exact linear reference makes the
average Earth-Moon distance about `60.34` Earth radii and the average Earth-Sun
distance about `23,481.13` Earth radii.

## Known Scientific Limitations

- Orbital elements are fixed at their authoring epoch; secular element rates and perturbations are not evaluated.
- The Earth follows the JPL Earth-Moon barycenter approximation.
- The Moon uses fixed mean elements and does not model nodal or apsidal precession, libration, or solar perturbations.
- All eight planets use the fixed JPL 1800-2050 approximation rows without
  secular rates or perturbations.
- Jupiter is represented by its volumetric mean radius; equatorial bulge is not yet modeled.
- Saturn, Uranus, and Neptune also use volumetric mean radii; oblateness is not
  yet modeled.
- Venus and Uranus use signed negative sidereal periods to preserve retrograde
  spin; the visual axial-tilt values are documented presentation conventions,
  not a full IAU pole-orientation model.
- The Sun uses one adopted rotation period even though it rotates differentially by latitude.
- The simulation epoch is an educational reference state, not the current real-world date.
