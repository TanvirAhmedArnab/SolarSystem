# Release Artifact Tooling

`release_artifacts.py` validates the generated Windows, WebGL, and macOS
release folders and creates upload-ready ZIP archives from one approved source
commit.

The tool:

- requires every Unity build report to identify the expected commit;
- checks the platform-specific player structure;
- verifies WebGL fallback-compressed `.unityweb` payloads;
- verifies that the macOS launcher is a Universal Mach-O binary;
- removes Unity's two Windows do-not-ship directories from the public ZIP;
- places each platform's playable files directly at its archive root;
- preserves Unix executable metadata for the macOS launcher and libraries;
- validates every completed ZIP before replacing an earlier archive; and
- writes `SHA256SUMS.txt` plus `release-artifact-manifest.json`.

Raw Unity build reports remain beside ignored build folders because their
diagnostic `outputPath` can contain a local user path. Public ZIPs exclude the
raw report and instead include a sanitized deterministic
`release-manifest.json` containing only release identity, platform, Unity
version, source commit, build timestamp, and applicable limitations.

## Validate One Build

```powershell
python Tools/Release/release_artifacts.py validate `
  --platform webgl `
  --path Builds/Release/SolarSystem-1.0.0-WebGL `
  --expected-commit <FULL_COMMIT_SHA> `
  --expected-version 1.0.0
```

Valid platform values are `windows`, `webgl`, and `macos`.

## Validate All Builds

```powershell
python Tools/Release/release_artifacts.py validate-all `
  --release-root Builds/Release `
  --version 1.0.0 `
  --expected-commit <FULL_COMMIT_SHA>
```

## Create All Upload Archives

```powershell
python Tools/Release/release_artifacts.py package `
  --release-root Builds/Release `
  --version 1.0.0 `
  --expected-commit <FULL_COMMIT_SHA>
```

Existing ZIPs are protected by default. Add `--force` only when intentionally
replacing artifacts after a new same-commit build:

```powershell
python Tools/Release/release_artifacts.py package `
  --release-root Builds/Release `
  --version 1.0.0 `
  --expected-commit <FULL_COMMIT_SHA> `
  --force
```

Generated outputs remain ignored under `Builds/Release/Archives`.

The packager validates the embedded release version before writing any
archive, rejects unsafe or duplicate ZIP entry paths, and aborts before
creating a partial archive set when a protected destination already exists.
