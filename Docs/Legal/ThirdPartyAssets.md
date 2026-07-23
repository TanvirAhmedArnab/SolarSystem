# Third-Party Assets and Licensing Record

**Owner:** Tanvir  
**Status:** Living licensing ledger  
**Version:** 0.3.1  
**Last reviewed:** 2026-07-23

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
| TEX-SSS-001-017 | Sun, planets, Earth layers, Moon, ring, starfield | Solar System Scope | CC BY 4.0 | Imported working set |
| TEX-USGS-001 | Io browse mosaic | USGS / Galileo and Voyager | Public domain | Imported evaluation source |
| TEX-USGS-002 | Europa browse mosaic | USGS / Galileo and Voyager | No use constraints listed | Imported evaluation source |
| TEX-USGS-003 | Ganymede browse mosaic | USGS / Galileo and Voyager | Public domain | Imported evaluation source |
| TEX-USGS-004 | Callisto browse mosaic | USGS / Galileo and Voyager | USGS source | Imported evaluation source |
| TEX-USGS-005 | Titan browse mosaic | USGS / Cassini ISS | Cite authors | Imported evaluation source |
| TEX-USGS-006 | Triton color browse mosaic | USGS / Voyager 2 | Public domain; cite authors | Imported evaluation source |
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
| TEX-SSS-001-016 | `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/<Body>` | Imported; Jupiter material and orientation validated, remaining material review pending |
| TEX-SSS-017 | `Assets/SolarSystem/Content/Art/Textures/Environment/T_Space_MilkyWay_2K.jpg` | Imported; sky presentation review pending |
| TEX-USGS-001-006 | `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/<Moon>` | Imported as visibly named `Browse` evaluation textures |
| AUD-KEN-001 selected subset | `Assets/SolarSystem/Content/Audio/SFX/UI` | Imported; audition and loudness review pending |
| AUD-OGA-MUS-001 | `Assets/SolarSystem/Content/Audio/Music/A_Music_OuterSpaceLoop.mp3` | Imported; 2D music mix review pending |
| AUD-OGA-SUN-001 | `Assets/SolarSystem/Content/Audio/Ambience/CelestialBodies/Sun/A_Sun_BurningLoop.wav` | Imported; intended 2D loop and mix review pending |
| AUD-OGA-EARTH-001 | `Assets/SolarSystem/Content/Audio/Ambience/CelestialBodies/Earth/A_Earth_ForestAmbienceLoop.mp3` | Imported; intended 3D rolloff and mix review pending |

### Active Jupiter texture record

- Source ID: `TEX-SSS-011`
- Source: `SourceAssets/ThirdParty/Textures/SolarSystemScope/2k_jupiter.jpg`
- Unity derivative: `Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Jupiter/T_Jupiter_Surface_2K.jpg`
- Active material: `Assets/SolarSystem/Content/Materials/CelestialBodies/M_Jupiter.mat`
- SHA-256 for source and Unity derivative: `B0F04D005350252636B0E3396FC592548CBD9E9126B269D32D5C6ABD4B0E4F2B`
- License: Solar System Scope, CC BY 4.0; release attribution remains required.

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

## Release Audit

- Verify every shipped binary against the SHA-256 manifest.
- Record each derivative's source ID and modifications.
- Include CC BY license links and attribution.
- Do not imply endorsement or use NASA branding.
- Re-check source terms immediately before release.
- Keep unused files out of the distributable build.
