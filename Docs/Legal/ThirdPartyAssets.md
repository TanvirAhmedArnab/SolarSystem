# Third-Party Assets and Licensing Record

**Owner:** Tanvir  
**Status:** Living licensing ledger  
**Version:** 0.12.0  
**Last reviewed:** 2026-07-25

**Live-source verification:** Rechecked 2026-07-22 against the Solar System Scope texture page, Kenney Interface Sounds page, the three OpenGameArt asset pages, and the linked USGS product records.

> Operational provenance record, not legal advice. Re-check source pages before release and reject assets with ambiguous rights.

## Release Credits and Provenance

### Solar System Scope

> Planet and space textures by Solar System Scope (https://www.solarsystemscope.com/textures/), licensed under Creative Commons Attribution 4.0 International (https://creativecommons.org/licenses/by/4.0/). Textures may include enhanced color and fictional fill for unmapped regions. Used and adapted for Solar System Simulation.

### OpenGameArt CC0 audio

> “Outer Space Loop” by wipics, “Fireplace Sound Loop” by PagDev, and “Forest Ambience” by TinyWorlds. Released under CC0 1.0 through OpenGameArt.

Attribution is not legally required under CC0, but this optional credit is retained as professional provenance.

### Kenney

> Interface sounds by Kenney (https://kenney.nl), dedicated to the public domain under CC0 1.0.

Credit is optional under CC0 but retained as professional provenance.

### USGS/NASA

> Selected planetary mosaics courtesy of the U.S. Geological Survey Astrogeology Science Center and source NASA missions. Individual product pages and citation requirements are linked below.

## Provider Findings

### Solar System Scope

- Source: https://www.solarsystemscope.com/textures/
- Provider: Solar System Scope / INOVE
- License: CC BY 4.0
- License: https://creativecommons.org/licenses/by/4.0/
- Commercial use, adaptation, and redistribution are allowed with attribution.
- Link the license and indicate modifications.
- The provider discloses enhanced saturation and fictional fill.

### USGS Astrogeology

- Catalog: https://astrogeology.usgs.gov/
- Provider: U.S. Geological Survey Astrogeology Science Center
- Selected pages list public-domain, no-use-constraint, or cite-authors status.
- Product pages remain authoritative.
- Disclose incomplete coverage, synthesized color, interpolation, or seams.

### NASA

- Guidelines: https://www.nasa.gov/nasa-brand-center/images-and-media/
- NASA content generally is not subject to U.S. copyright, but third-party items may be protected.
- Acknowledge NASA, inspect item credits, do not imply endorsement, and do not use NASA insignia as project branding.

### OpenGameArt CC0 audio

- Music: https://opengameart.org/content/outer-space-loop — wipics — CC0 1.0.
- Sun ambience source: https://opengameart.org/content/fireplace-sound-loop — PagDev — CC0 1.0.
- Earth ambience source: https://opengameart.org/content/forest-ambience — TinyWorlds — CC0 1.0.
- License: https://creativecommons.org/publicdomain/zero/1.0/
- CC0 permits copying, modification, redistribution, performance, and commercial use without permission or required attribution.

### Kenney

- Official: https://kenney.nl/assets/interface-sounds
- Mirror: https://opengameart.org/content/interface-sounds
- License: CC0 1.0
- License: https://creativecommons.org/publicdomain/zero/1.0/

## Asset Ledger

The fetch script records byte size and SHA-256 in `SourceAssets/asset-download-manifest.csv`.

| IDs | Content | Provider | License | Status |
|---|---|---|---|---|
| TEX-SSS-001-017 | Sun, planets, Earth layers, Moon, ring, starfield | Solar System Scope | CC BY 4.0 | Imported; solar hero, required planet surfaces, layered Earth, Saturn ring, and starfield active |
| TEX-USGS-001 | Io browse mosaic | USGS / Galileo and Voyager | Public domain | Approved runtime baseline; attribution retained |
| TEX-USGS-002 | Europa browse mosaic | USGS / Galileo and Voyager | No use constraints listed | Approved runtime baseline; source attribution retained |
| TEX-USGS-003 | Ganymede browse mosaic | USGS / Galileo and Voyager | Public domain | Approved runtime baseline; attribution retained |
| TEX-USGS-004 | Callisto browse mosaic | USGS / Galileo and Voyager | USGS source | Approved runtime baseline; source attribution retained |
| TEX-USGS-005 | Titan browse mosaic | USGS / Cassini ISS | Cite authors | Approved runtime baseline; author citation required |
| TEX-USGS-006 | Triton color browse mosaic | USGS / Voyager 2 | Public domain; cite authors | Approved runtime baseline; author citation required |
| AUD-OGA-MUS-001 | Outer Space Loop | wipics / OpenGameArt | CC0 1.0 | Imported music loop |
| AUD-OGA-SUN-001 | Fireplace Sound Loop | PagDev / OpenGameArt | CC0 1.0 | Imported Sun ambience source |
| AUD-OGA-EARTH-001 | Forest Ambience | TinyWorlds / OpenGameArt | CC0 1.0 | Imported Earth ambience source |
| AUD-KEN-001 | Interface Sounds | Kenney | CC0 1.0 | Seven-file runtime subset imported |

## Repository Inclusion Policy

- Commit every Unity-ready asset used by the project through Git LFS.
- Commit the corresponding source original when it is part of the approved working set.
- Treat `SourceAssets/_Downloads` as a reproducible, ignored cache rather than permanent repository content.
- For the 100-file Kenney pack, commit only the seven selected originals plus the bundled license and provider link; leave 93 unused sounds and the archive out of Git.
- Keep the manifest and retrieval script so the complete source package can be reproduced and its archive hash verified.

## Unity Import Record

**Imported:** 2026-07-22  
**Unity root:** `Assets/SolarSystem/Content`  
**Method:** Byte-identical copies renamed to the project naming convention; originals and SHA-256 values remain in `SourceAssets`.

| Source IDs | Unity destination | Import status |
|---|---|---|
| TEX-SSS-001-016 | `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/<Body>` | Imported; Sun, eight planet surfaces, layered Earth and Venus maps, Moon, and Saturn ring active |
| TEX-SSS-017 | `Assets/SolarSystem/Content/Art/Textures/Environment/T_Space_MilkyWay_2K.jpg` | Imported; sky presentation review pending |
| TEX-USGS-001-006 | `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/<Moon>` | Active byte-identical `Browse` runtime baselines; source limitations disclosed |
| AUD-KEN-001 selected subset | `Assets/SolarSystem/Content/Audio/SFX/UI` | Imported; audition and loudness review pending |
| AUD-OGA-MUS-001 | `Assets/SolarSystem/Content/Audio/Music/A_Music_OuterSpaceLoop.mp3` | Imported; 2D music mix review pending |
| AUD-OGA-SUN-001 | `Assets/SolarSystem/Content/Audio/Ambience/CelestialBodies/Sun/A_Sun_BurningLoop.wav` | Imported; intended 2D loop and mix review pending |
| AUD-OGA-EARTH-001 | `Assets/SolarSystem/Content/Audio/Ambience/CelestialBodies/Earth/A_Earth_ForestAmbienceLoop.mp3` | Imported; intended 3D rolloff and mix review pending |

### Active Jupiter texture record

- Source ID: `TEX-SSS-011`
- Source: `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_jupiter.jpg`
- Unity derivative: `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Jupiter/T_Jupiter_Surface_2K.jpg`
- Active materials: `Assets/SolarSystem/Content/Materials/CelestialBodies/M_Jupiter.mat`
  and `M_Jupiter_Atmosphere.mat`
- SHA-256 for source and Unity derivative: `B0F04D005350252636B0E3396FC592548CBD9E9126B269D32D5C6ABD4B0E4F2B`
- License: Solar System Scope, CC BY 4.0; release attribution remains required.
- Modification status: source and Unity texture pixels are unchanged. The
  project-owned surface shader keeps the source map anchored as the primary
  color sample and adds low-amplitude procedural detail; the atmosphere shader
  uses no additional texture.

The gas-giant authoring/model/view code, surface shader, atmosphere shader, and
shell wiring are project-authored. They introduce no new third-party media or
license. Their use of TEX-SSS-011 continues the existing Solar System Scope
CC BY 4.0 attribution obligation.

### Active Saturn hero texture records

- Source IDs: `TEX-SSS-012` (surface) and `TEX-SSS-013` (ring alpha).
- Unity derivatives:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Saturn/T_Saturn_Surface_2K.jpg`
  and `T_Saturn_RingsAlpha_2K.png`.
- Active materials: `M_Saturn.mat`, `M_Saturn_Atmosphere.mat`, and
  `M_Saturn_Rings.mat`.
- License: Solar System Scope, CC BY 4.0; release attribution remains required.
- Modification status: texture pixels are unchanged. Project-authored shaders
  keep the surface map anchored and the ring alpha radially anchored to the
  generated annulus.

The Saturn definition/profile, atmosphere material, ring shader, renderer
policy, and annulus geometry are project-authored and introduce no new
third-party media or license. The rendering changes do not alter the existing
CC BY 4.0 attribution obligation for `TEX-SSS-012` and `TEX-SSS-013`.

### Active Venus atmosphere texture records

- Source IDs: `TEX-SSS-002` (surface) and `TEX-SSS-003` (atmosphere).
- Sources:
  `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_venus_surface.jpg`
  and `2k_venus_atmosphere.jpg`.
- Unity derivatives:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Venus/T_Venus_Surface_2K.jpg`
  and `T_Venus_Atmosphere_2K.jpg`.
- Active materials: `M_Venus.mat`, `M_Venus_CloudDeck.mat`, and
  `M_Venus_Atmosphere.mat`.
- SHA-256:
  `DBE5DB1C794A8AB4CBF7DD6BF193540C400FC833CE1E6CC399318AA68026278B`
  for the surface and
  `225012AD4911730605C4E189CA2A3BF674FCE50CC48AAB4102B936B47D6991AC`
  for the atmosphere source and matching Unity derivative.
- License: Solar System Scope, CC BY 4.0; release attribution remains required.
- Modification status: texture pixels are unchanged. The project-owned opaque
  cloud shader keeps the atmosphere map anchored and samples neighboring
  source texels for restrained relief; the outer atmosphere shader uses no
  additional texture.

The Venus layer definition/model/view reuse, cloud shader, atmosphere
material, and reproducible scene wiring are project-authored. They introduce
no new third-party media or license and do not alter the existing Solar System
Scope CC BY 4.0 attribution obligation.

### Active Mars hero-rendering record

- Source ID: `TEX-SSS-010`.
- Source:
  `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_mars.jpg`.
- Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Mars/T_Mars_Surface_2K.jpg`.
- Active materials: `M_Mars.mat` and project-authored
  `M_Mars_Atmosphere.mat`.
- Source and Unity derivative SHA-256:
  `2D187F3E77A98EAA8CEA5F4CC722F633C122EF170B9E94ACE6B5FB6CBC3F8E01`.
- License: Solar System Scope, CC BY 4.0; release attribution remains required.
- Modification status: texture pixels are unchanged. The project-owned rocky
  shader keeps the source map anchored, applies an authored color multiplier,
  and samples four neighboring source texels for restrained relief. The
  atmosphere shader uses no additional texture.

The atmosphere-only definition/model/view extension, rocky shader, atmosphere
material, tests, and reproducible scene wiring are project-authored. They add
no third-party media or license and do not change the existing attribution
obligation for `TEX-SSS-010`.

### Active Uranus and Neptune hero-rendering records

- Uranus source ID: `TEX-SSS-014`.
- Uranus source:
  `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_uranus.jpg`.
- Uranus Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Uranus/T_Uranus_Surface_2K.jpg`.
- Uranus active materials: `M_Uranus.mat` and project-authored
  `M_Uranus_Atmosphere.mat`.
- Uranus source and derivative SHA-256:
  `D15239D46F82D3EA13D2B260B5B29B2A382F42F2916DAE0694D0387B1204A09D`.
- Neptune source ID: `TEX-SSS-015`.
- Neptune source:
  `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_neptune.jpg`.
- Neptune Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Neptune/T_Neptune_Surface_2K.jpg`.
- Neptune active materials: `M_Neptune.mat` and project-authored
  `M_Neptune_Atmosphere.mat`.
- Neptune source and derivative SHA-256:
  `CB42EA82709741D28B0AF44D8B283CBC6DBD0C521A7F0E1E1E010ADE00977DF6`.
- License: Solar System Scope, CC BY 4.0; release attribution remains required.
- Modification status: all source and derivative hashes match. Texture pixels
  are unchanged. Project-authored shaders apply reviewed tints, restrained
  source-derived relief, bounded shifted-sample detail, a live-Sun nightside
  readability mask, and atmosphere colors without introducing another media
  source.

The ice-giant definition/model/view architecture, generic giant-planet shader
extension, atmosphere materials, tests, and reproducible scene wiring are
project-authored. They add no third-party media or license and do not change
the existing CC BY 4.0 attribution obligations for `TEX-SSS-014` or
`TEX-SSS-015`.

### Active Mercury and Moon airless-rocky hero-rendering records

- Mercury source ID: `TEX-SSS-001`.
- Mercury source:
  `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_mercury.jpg`.
- Mercury Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Mercury/T_Mercury_Surface_2K.jpg`.
- Mercury active material: `M_Mercury.mat`.
- Mercury source and derivative SHA-256:
  `5A5C80607F643496BAC9A631E71957DEF35ED788895F18B678AC849C2B38E48A`.
- Moon source ID: `TEX-SSS-016`.
- Moon source:
  `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_moon.jpg`.
- Moon Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Moon/T_Moon_Surface_2K.jpg`.
- Moon active material: `M_Moon.mat`.
- Moon source and derivative SHA-256:
  `2764BA6535EA0481A062846EE033CC7A909DAE05B31A8FD13F3E98F3A7FD92BD`.
- License: Solar System Scope, CC BY 4.0; release attribution remains required.
- Modification status: both source and derivative hashes match. Texture pixels
  are unchanged. The project-authored rocky shader applies reviewed tints,
  estimates restrained relief from neighboring source luminance, uses
  body-specific dry PBR values, and applies a small live-Sun nightside
  readability mask without adding another media source.

The airless-rocky definition/model/view architecture, shader extension, tests,
and reproducible scene wiring are project-authored. They add no third-party
media or license and do not change the CC BY 4.0 attribution obligations for
`TEX-SSS-001` or `TEX-SSS-016`.

### Active eight-planet material records

The baseline activates the following byte-identical Unity derivatives from the
already approved Solar System Scope CC BY 4.0 working set:

| Source ID | Body/purpose | Unity derivative | Active material |
|---|---|---|---|
| TEX-SSS-001 | Mercury surface | `CelestialBodies/Mercury/T_Mercury_Surface_2K.jpg` | `M_Mercury.mat` |
| TEX-SSS-002 | Venus surface | `CelestialBodies/Venus/T_Venus_Surface_2K.jpg` | `M_Venus.mat` |
| TEX-SSS-003 | Venus atmosphere | `CelestialBodies/Venus/T_Venus_Atmosphere_2K.jpg` | `M_Venus_CloudDeck.mat` |
| TEX-SSS-004 | Earth day surface | `CelestialBodies/Earth/T_Earth_DayAlbedo_2K.jpg` | `M_Earth.mat` |
| TEX-SSS-005 | Earth night emission | `CelestialBodies/Earth/T_Earth_NightEmission_2K.jpg` | `M_Earth.mat` |
| TEX-SSS-006 | Earth clouds | `CelestialBodies/Earth/T_Earth_Clouds_2K.jpg` | `M_Earth_Clouds.mat` |
| TEX-SSS-007 | Earth normal detail | `CelestialBodies/Earth/T_Earth_Normal_2K.tif` | `M_Earth.mat` |
| TEX-SSS-008 | Earth ocean specular mask | `CelestialBodies/Earth/T_Earth_Specular_2K.tif` | `M_Earth.mat` |
| TEX-SSS-010 | Mars surface | `CelestialBodies/Mars/T_Mars_Surface_2K.jpg` | `M_Mars.mat` |
| TEX-SSS-011 | Jupiter surface | `CelestialBodies/Jupiter/T_Jupiter_Surface_2K.jpg` | `M_Jupiter.mat` |
| TEX-SSS-012 | Saturn surface | `CelestialBodies/Saturn/T_Saturn_Surface_2K.jpg` | `M_Saturn.mat` |
| TEX-SSS-013 | Saturn ring alpha | `CelestialBodies/Saturn/T_Saturn_RingsAlpha_2K.png` | `M_Saturn_Rings.mat` |
| TEX-SSS-014 | Uranus surface | `CelestialBodies/Uranus/T_Uranus_Surface_2K.jpg` | `M_Uranus.mat`, `M_Uranus_Atmosphere.mat` |
| TEX-SSS-015 | Neptune surface | `CelestialBodies/Neptune/T_Neptune_Surface_2K.jpg` | `M_Neptune.mat`, `M_Neptune_Atmosphere.mat` |

The generated `SM_Saturn_Rings.asset` contains project-authored geometry only;
its appearance remains a derivative use of TEX-SSS-013 and therefore retains
the source texture's CC BY 4.0 attribution obligation. No new third-party asset
or license was introduced by the eight-planet baseline.

The layered-Earth shaders, shell geometry, and deterministic motion code are
project-authored. They activate the already approved TEX-SSS-004 through
TEX-SSS-008 derivatives without modifying their pixels, so the existing Solar
System Scope CC BY 4.0 attribution obligation continues and no new third-party
license is introduced.

### Active solar hero record

- Source ID: `TEX-SSS-016`
- Source: `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_sun.jpg`
- Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Sun/T_Sun_Surface_2K.jpg`
- Active materials: `M_Sun.mat` and `M_Sun_Corona.mat`
- License: Solar System Scope, CC BY 4.0; release attribution remains required.
- Modification status: the source and Unity texture pixels are unchanged.
  Project-authored shaders sample the approved texture for the opaque surface
  and transparent corona shell.

The solar surface/corona shaders, shell wiring, immutable authoring, and
deterministic motion code are project-authored. They introduce no new
third-party asset or license. Their use of TEX-SSS-016 continues the existing
Solar System Scope CC BY 4.0 attribution obligation.

### Active Io and Europa hero records

#### Io

- Source ID: `TEX-USGS-001`
- Product: Voyager/Galileo SSI global mosaic, `1 km/pixel` source product.
- Product page:
  https://astrogeology.usgs.gov/search/map/io_voyager_galileo_ssi_global_mosaic_1km
- Retained source:
  `SourceAssets/ThirdParty/Textures/USGS/io_global_mosaic_browse.jpg`
- Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Io/T_Io_Surface_Browse.jpg`
- Active material: `M_Io.mat`.
- Integrity: both repository JPEGs are byte-identical with SHA-256
  `DE69759452F5479B6F56FF5C72A90ED402AB8D7F11219524C26E5B60610B9597`.
- Usage status: USGS lists the product as public domain with no use
  constraints; attribution is retained and no endorsement is implied.
- Coverage limitations: the mosaic combines Galileo and Voyager imagery;
  some coverage and control geometry vary, and the browse derivative is only
  `512 x 256`.
- Modification status: source pixels are unchanged. The project-authored
  shader adds a global sulfur/ochre presentation tint and bounded
  luminance-derived normal response; it does not add source data.

#### Europa

- Source ID: `TEX-USGS-002`
- Product: Voyager/Galileo SSI global mosaic, `500 m/pixel` final map.
- Product page:
  https://astrogeology.usgs.gov/search/map/europa_voyager_galileo_ssi_global_mosaic_500m
- Retained source:
  `SourceAssets/ThirdParty/Textures/USGS/europa_global_mosaic_browse.jpg`
- Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Europa/T_Europa_Surface_Browse.jpg`
- Active material: `M_Europa.mat`.
- Integrity: both repository JPEGs are byte-identical with SHA-256
  `3369BA56CBFA447347B5AFC003B80B69FBF87DD90A724D59F09ABAD8691A9819`.
- Usage status: USGS lists no use constraints; source attribution is retained
  and no endorsement is implied.
- Coverage limitations: Galileo and Voyager inputs span approximately
  `20 km/pixel` gap fill to `200 m/pixel` detail before assembly into the
  `500 m/pixel` map; the browse derivative is `512 x 256` and preserves
  incomplete/dark source regions.
- Modification status: source pixels are unchanged. The project-authored
  shader adds a pale presentation tint and bounded luminance-derived normal
  response; it does not reconstruct missing imagery.

### Active Titan hero record

- Source ID: `TEX-USGS-005`
- Product: Cassini ISS Titan near-global mosaic, `450 m/pixel` source product.
- Product page:
  https://astrogeology.usgs.gov/search/map/titan_cassini_iss_near_global_mosaic_450m
- Retained source:
  `SourceAssets/ThirdParty/Textures/USGS/titan_near_global_mosaic_browse.jpg`
- Unity derivative:
  `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Titan/T_Titan_Surface_Browse.jpg`
- Active materials: `M_Titan.mat` and project-authored `M_Titan_Haze.mat`.
- Integrity: both repository JPEGs are byte-identical with SHA-256
  `0F967976320C91444D5CBF7E5A0BEAD56C4A38FE41F497D85E00A22D1F119774`.
- Usage status: approved with required author citation; no endorsement implied.
- Coverage and processing: the browse mosaic covers all longitudes from
  approximately `45 N` to `65 S`; Cassini ISS methane-window observations
  were processed and haze-corrected. It is not raw natural-color,
  globally complete, or date-specific visible-light imagery.
- Modifications: the source pixels remain unchanged. Project-authored shaders
  suppress surface contrast and add an amber presentation haze; those
  rendering choices introduce no new third-party asset or license.

Release credits must cite the authors named on the USGS product record and
identify USGS Astrogeology/Cassini ISS as the imagery source. Reconfirm the
product page's citation wording during the final release audit.

### Kenney selected-subset mapping

| Source file | Unity asset | Intended event |
|---|---|---|
| `select_001.ogg` | `A_UI_Select.ogg` | Selection |
| `confirmation_001.ogg` | `A_UI_FocusConfirmation.ogg` | Focus confirmation |
| `open_001.ogg` | `A_UI_Open.ogg` | Panel open |
| `close_001.ogg` | `A_UI_Close.ogg` | Panel close |
| `switch_002.ogg` | `A_UI_ToggleScale.ogg` | Scale-mode toggle |
| `tick_002.ogg` | `A_UI_TimeTick.ogg` | Time control tick |
| `error_004.ogg` | `A_UI_InvalidAction.ogg` | Invalid action |

## USGS Product Pages

- Io: https://astrogeology.usgs.gov/search/map/io_voyager_galileo_ssi_global_mosaic_1km
- Europa: https://astrogeology.usgs.gov/search/map/europa_voyager_galileo_ssi_global_mosaic_500m
- Ganymede: https://astrogeology.usgs.gov/search/map/ganymede_voyager_galileo_ssi_global_mosaic_1km
- Callisto: https://astrogeology.usgs.gov/search/map/callisto_galileo_voyager_simple_cylindrical_global_map
- Titan: https://astrogeology.usgs.gov/search/map/titan_cassini_iss_near_global_mosaic_450m
- Triton: https://astrogeology.usgs.gov/search/map/triton_voyager_2_global_color_mosaic_600m

### Active USGS derivative integrity

The runtime files are byte-identical copies of the retained source downloads;
Unity import settings do not modify the repository binaries.

| ID | Body | Dimensions | SHA-256 |
|---|---|---:|---|
| TEX-USGS-001 | Io | 512 x 256 | `DE69759452F5479B6F56FF5C72A90ED402AB8D7F11219524C26E5B60610B9597` |
| TEX-USGS-002 | Europa | 512 x 256 | `3369BA56CBFA447347B5AFC003B80B69FBF87DD90A724D59F09ABAD8691A9819` |
| TEX-USGS-003 | Ganymede | 512 x 256 | `465673D0D789658CE63275C8CCC9EBBDF6B1AEC0A148CAA41052FEBC314A1616` |
| TEX-USGS-004 | Callisto | 512 x 249 | `FA60F8305E1B000E4FBC4446CECDD5DF919A778841D9D7354E606889BBAC856F` |
| TEX-USGS-005 | Titan | 512 x 156 | `0F967976320C91444D5CBF7E5A0BEAD56C4A38FE41F497D85E00A22D1F119774` |
| TEX-USGS-006 | Triton | 512 x 256 | `A71DF5E3DE28BA755200E0E9DB2633E0529CD2CDF344299715660F3DA37D1FCE` |

## Release Audit

- Verify every shipped binary against the SHA-256 manifest.
- Record each derivative's source ID and modifications.
- Include CC BY license links and attribution.
- Do not imply endorsement or use NASA branding.
- Re-check source terms immediately before release.
- Keep unused files out of the distributable build.
