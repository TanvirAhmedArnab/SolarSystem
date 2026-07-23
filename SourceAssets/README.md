# Source Assets

This folder stores unmodified third-party downloads and a generated SHA-256 manifest.

- Never edit originals in place.
- Create Unity-ready derivatives under `Assets/SolarSystem/Content` only after review.
- Record modifications in `Docs/Legal/ThirdPartyAssets.md`.
- Binary assets follow repository Git LFS rules.
- Presence here does not mean final-build approval.
- Re-run `Tools/AssetPipeline/Fetch-ApprovedAssets.ps1` to reproduce the set.

## Repository inclusion policy

- `_Downloads` is a reproducible local cache and is not versioned.
- Only source files used by the approved Unity working set are committed.
- The complete Kenney Interface Sounds archive may be regenerated locally, but only the seven selected source sounds, the provider link, and the bundled license are versioned.
- Unused extracted files remain ignored until an approved feature requires them.

See `Docs/Art/ArtBible.md` and `Docs/Legal/ThirdPartyAssets.md`.
