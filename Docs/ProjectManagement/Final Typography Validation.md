# Final UI Typography Validation

**Owner:** Tanvir  
**Technical steward:** Codex  
**Status:** Final typography candidate validated; repository commit pending owner approval  
**Validation date:** 2026-07-25  
**Unity baseline:** Unity 6000.5.3f1, URP 17.5.0

## Approved Decision

Tanvir approved Inter Regular and Inter SemiBold as the project's final UI
typefaces. The implementation uses the official Inter v4.1 static TTF files:

- Regular owns explanatory copy, summaries, numeric values, and source text.
- SemiBold owns headings, status labels, tabs, buttons, keycaps, and other
  emphasis roles.
- USS assigns the actual SemiBold asset to semantic selectors, so the interface
  does not rely on synthetic bold.

## Runtime Architecture

`SolarSystemUiAssetBuilder` validates or creates two project-owned TextCore font
assets:

- `FA_Inter_Regular.asset`
- `FA_Inter_SemiBold.asset`

Both use Unity 6's supported Dynamic atlas population mode, a 1024-square SDFAA
atlas contract, multi-atlas fallback, and font-feature discovery. The raw TTFs,
TextCore assets, UXML/USS, runtime credit, and complete license are separate and
auditable.

Live Play Mode style inspection proved:

| Element role | Representative element | Resolved asset |
|---|---|---|
| Explanatory copy | `body-summary` | `FA_Inter_Regular` |
| Numeric/status copy | `simulation-rate` | `FA_Inter_Regular` |
| Primary heading | `body-name` | `FA_Inter_SemiBold` |
| Control | `menu-button` | `FA_Inter_SemiBold` |

## Source and License Verification

- Official release:
  https://github.com/rsms/inter/releases/tag/v4.1
- Release archive SHA-256:
  `9883FDD4A49D4FB66BD8177BA6625EF9A64AA45899767DDE3D36AA425756B11E`
- Inter Regular SHA-256:
  `40D692FCE188E4471E2B3CBA937BE967878F631AD3EBBBDCD587687C7EBE0C82`
- Inter SemiBold SHA-256:
  `78A843FADE9D4612A5567302FB595B56976EB5FCEBF4FEA5A5912D638BAFCDE3`
- OFL text SHA-256:
  `262481E844521B326F5ECD053E59B98C8B2DA78C8EE1BDBB6E8174305E54935A`

The retained source and Unity TTF copies are byte-identical. The manifest
contains exactly three verified Inter rows. The complete SIL Open Font License
1.1 is retained at `SourceAssets/ThirdParty/Fonts/Inter/LICENSE.txt`, and the
runtime Credits & Sources page identifies the family, weights, designer, and
license. The 33 MB release archive remains in the ignored download cache.

## Automated Validation

| Check | Result |
|---|---|
| Unity compilation | Pass |
| Edit Mode suite | 185 passed, 0 failed |
| Play Mode suite | 25 passed, 0 failed |
| Console after final run | 0 warnings, 0 errors |
| Font-fetch PowerShell parse | Pass |
| Manifest size/hash verification | Pass |
| Source-to-runtime TTF hash equality | Pass |

Dynamic atlas initialization exposed a test-only weakness in the previous
two-second transition waits: the deadline could begin before the first
initialized frame. The Play Mode helpers now yield one frame before starting a
five-second wall-clock deadline. Production camera durations and runtime
transition behavior are unchanged; the revised tests still fail bounded hangs.

## Visual Validation

Fresh exact-resolution Game View captures were reviewed for both the explorer
overview and the densest Credits & Sources surface.

| Resolution | Overview HUD | Credits & Sources |
|---|---|---|
| 1280x720 | Pass; panels, labels, keycaps, and menu control remain inside safe areas | Pass; all sections and the Inter credit remain visible without clipping |
| 2560x1440 | Pass; type hierarchy, labels, and control grouping remain clear | Pass; headings, long credit lines, tabs, close control, and license note remain legible and contained |

Temporary validation captures remain under ignored `Temp/` and are not release
media. The final portfolio capture pass will create separately curated assets.

## Candidate Result

The final typography decision is implemented, licensed, reproducible,
responsive, and regression-tested. No repository commit or push has been made;
Tanvir's explicit approval remains required.
