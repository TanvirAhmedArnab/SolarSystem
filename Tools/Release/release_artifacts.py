#!/usr/bin/env python3
"""Validate and package Solar System release artifacts deterministically."""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import shutil
import stat
import sys
import tempfile
import zipfile
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path, PurePosixPath
from typing import Iterable


BUILD_REPORT_NAME = "release-build-report.json"
ARCHIVE_MANIFEST_NAME = "release-manifest.json"
FIXED_ZIP_TIMESTAMP = (1980, 1, 1, 0, 0, 0)
UNITY_DO_NOT_SHIP_DIRECTORY_SUFFIXES = (
    "_BackUpThisFolder_ButDontShipItWithYourGame",
    "_BurstDebugInformation_DoNotShip",
)


@dataclass(frozen=True)
class PlatformContract:
    key: str
    directory_suffix: str
    archive_suffix: str
    unity_target: str


PLATFORMS = {
    "windows": PlatformContract(
        "windows",
        "Windows-x86_64",
        "Windows-x86_64",
        "StandaloneWindows64",
    ),
    "webgl": PlatformContract("webgl", "WebGL", "WebGL", "WebGL"),
    "macos": PlatformContract(
        "macos",
        "macOS-Universal",
        "macOS-Universal",
        "StandaloneOSX",
    ),
}


class ReleaseArtifactError(RuntimeError):
    """Raised when a release artifact violates the approved contract."""


def require(condition: bool, message: str, issues: list[str]) -> None:
    if not condition:
        issues.append(message)


def require_file(root: Path, relative_path: str, issues: list[str]) -> Path:
    path = root / relative_path
    require(path.is_file(), f"Missing required file: {relative_path}", issues)
    return path


def require_directory(root: Path, relative_path: str, issues: list[str]) -> Path:
    path = root / relative_path
    require(
        path.is_dir(),
        f"Missing required directory: {relative_path}",
        issues,
    )
    return path


def read_report(root: Path, issues: list[str]) -> dict[str, object]:
    report_path = require_file(root, BUILD_REPORT_NAME, issues)
    if not report_path.is_file():
        return {}

    try:
        return json.loads(report_path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as error:
        issues.append(f"Invalid {BUILD_REPORT_NAME}: {error}")
        return {}


def require_glob(
    root: Path,
    pattern: str,
    description: str,
    issues: list[str],
) -> None:
    require(
        any(path.is_file() for path in root.glob(pattern)),
        f"Missing {description}; expected pattern: {pattern}",
        issues,
    )


def read_magic(path: Path, byte_count: int = 4) -> str:
    try:
        with path.open("rb") as stream:
            return stream.read(byte_count).hex().upper()
    except OSError:
        return ""


def validate_build(
    platform_key: str,
    root: Path,
    expected_commit: str,
    expected_version: str,
) -> dict[str, object]:
    contract = PLATFORMS[platform_key]
    root = root.resolve()
    issues: list[str] = []

    require(root.is_dir(), f"Build directory does not exist: {root}", issues)
    if not root.is_dir():
        raise ReleaseArtifactError("\n".join(issues))

    report = read_report(root, issues)
    require(
        report.get("sourceCommit") == expected_commit,
        "Build report sourceCommit does not match the approved release commit.",
        issues,
    )
    require(
        report.get("releaseVersion") == expected_version,
        "Build report releaseVersion does not match the requested release.",
        issues,
    )
    require(
        report.get("target") == contract.unity_target,
        f"Build report target must be {contract.unity_target}.",
        issues,
    )
    require(
        report.get("result") == "Succeeded",
        "Build report result must be Succeeded.",
        issues,
    )
    require(
        report.get("errorCount") == 0,
        "Build report errorCount must be zero.",
        issues,
    )

    if platform_key == "windows":
        require_file(root, "Solar System Simulation.exe", issues)
        require_file(root, "UnityPlayer.dll", issues)
        require_file(root, "GameAssembly.dll", issues)
        require_directory(root, "Solar System Simulation_Data", issues)
    elif platform_key == "webgl":
        require_file(root, "index.html", issues)
        build_directory_path = require_directory(root, "Build", issues)
        require_directory(root, "TemplateData", issues)
        if build_directory_path.is_dir():
            require_glob(
                build_directory_path,
                "*.loader.js",
                "WebGL loader",
                issues,
            )
            require_glob(
                build_directory_path,
                "*.data.unityweb",
                "fallback-compressed WebGL data",
                issues,
            )
            require_glob(
                build_directory_path,
                "*.framework.js.unityweb",
                "fallback-compressed WebGL framework",
                issues,
            )
            require_glob(
                build_directory_path,
                "*.wasm.unityweb",
                "fallback-compressed WebAssembly player",
                issues,
            )
    elif platform_key == "macos":
        application = require_directory(
            root,
            "Solar System Simulation.app",
            issues,
        )
        if application.is_dir():
            require_file(application, "Contents/Info.plist", issues)
            launcher = require_file(
                application,
                "Contents/MacOS/Solar System Simulation",
                issues,
            )
            require_directory(application, "Contents/Resources/Data", issues)
            if launcher.is_file():
                require(
                    read_magic(launcher)
                    in {"CAFEBABE", "BEBAFECA", "CAFEBABF", "BFBAFECA"},
                    "macOS launcher is not a Universal Mach-O binary.",
                    issues,
                )

    if issues:
        formatted = "\n- ".join(issues)
        raise ReleaseArtifactError(
            f"{platform_key} release validation failed:\n- {formatted}"
        )

    files = [path for path in root.rglob("*") if path.is_file()]
    return {
        "platform": platform_key,
        "path": str(root),
        "sourceCommit": report["sourceCommit"],
        "releaseVersion": report["releaseVersion"],
        "buildUtc": report.get("utcTimestamp"),
        "unityVersion": report.get("unityVersion"),
        "target": report["target"],
        "result": report["result"],
        "warningCount": report.get("warningCount"),
        "errorCount": report["errorCount"],
        "fileCount": len(files),
        "uncompressedBytes": sum(path.stat().st_size for path in files),
    }


def build_directory(
    release_root: Path,
    version: str,
    contract: PlatformContract,
) -> Path:
    return release_root / f"SolarSystem-{version}-{contract.directory_suffix}"


def archive_path(
    archive_root: Path,
    version: str,
    contract: PlatformContract,
) -> Path:
    return archive_root / f"SolarSystem-{version}-{contract.archive_suffix}.zip"


def is_excluded(relative_path: PurePosixPath) -> bool:
    return any(
        part.endswith(suffix)
        for part in relative_path.parts
        for suffix in UNITY_DO_NOT_SHIP_DIRECTORY_SUFFIXES
    )


def is_macos_executable(relative_path: PurePosixPath) -> bool:
    parts = relative_path.parts
    in_macos_directory = (
        "Contents" in parts
        and "MacOS" in parts
        and parts.index("MacOS") > parts.index("Contents")
    )
    return in_macos_directory or relative_path.suffix.lower() in {
        ".dylib",
        ".so",
        ".bundle",
    }


def zip_info(
    relative_path: PurePosixPath,
    *,
    is_directory: bool,
    executable: bool,
) -> zipfile.ZipInfo:
    name = relative_path.as_posix()
    if is_directory and not name.endswith("/"):
        name += "/"

    info = zipfile.ZipInfo(name, FIXED_ZIP_TIMESTAMP)
    info.create_system = 3
    if is_directory:
        mode = stat.S_IFDIR | 0o755
        info.external_attr = (mode << 16) | 0x10
        info.compress_type = zipfile.ZIP_STORED
    else:
        mode = stat.S_IFREG | (0o755 if executable else 0o644)
        info.external_attr = mode << 16
        info.compress_type = (
            zipfile.ZIP_STORED
            if name.endswith(".unityweb")
            else zipfile.ZIP_DEFLATED
        )
    return info


def iter_archive_entries(
    platform_key: str,
    root: Path,
) -> Iterable[tuple[Path, PurePosixPath]]:
    for path in sorted(root.rglob("*"), key=lambda item: item.as_posix()):
        relative = PurePosixPath(path.relative_to(root).as_posix())
        if relative.as_posix() == BUILD_REPORT_NAME:
            continue
        if is_excluded(relative):
            continue
        yield path, relative


def write_archive(
    platform_key: str,
    root: Path,
    destination: Path,
    force: bool,
    release_manifest: dict[str, object],
) -> None:
    if destination.exists() and not force:
        raise ReleaseArtifactError(
            f"Archive already exists: {destination}. Use --force to replace it."
        )

    destination.parent.mkdir(parents=True, exist_ok=True)
    handle, temporary_name = tempfile.mkstemp(
        prefix=f".{destination.stem}-",
        suffix=".tmp",
        dir=destination.parent,
    )
    os.close(handle)
    temporary = Path(temporary_name)

    try:
        with zipfile.ZipFile(
            temporary,
            mode="w",
            compression=zipfile.ZIP_DEFLATED,
            compresslevel=9,
            allowZip64=True,
        ) as archive:
            archive.writestr(
                zip_info(
                    PurePosixPath(ARCHIVE_MANIFEST_NAME),
                    is_directory=False,
                    executable=False,
                ),
                (
                    json.dumps(
                        release_manifest,
                        indent=2,
                        sort_keys=True,
                    )
                    + "\n"
                ).encode("utf-8"),
            )
            for path, relative in iter_archive_entries(platform_key, root):
                if path.is_dir():
                    archive.writestr(
                        zip_info(
                            relative,
                            is_directory=True,
                            executable=False,
                        ),
                        b"",
                    )
                    continue

                executable = (
                    platform_key == "macos"
                    and is_macos_executable(relative)
                )
                info = zip_info(
                    relative,
                    is_directory=False,
                    executable=executable,
                )
                with path.open("rb") as source, archive.open(info, "w") as sink:
                    shutil.copyfileobj(source, sink, length=1024 * 1024)

        validate_archive(
            platform_key,
            temporary,
            str(release_manifest["sourceCommit"]),
        )
        os.replace(temporary, destination)
    finally:
        temporary.unlink(missing_ok=True)


def validate_archive(
    platform_key: str,
    archive_path_value: Path,
    expected_commit: str,
) -> None:
    issues: list[str] = []
    with zipfile.ZipFile(archive_path_value, "r") as archive:
        entries = archive.infolist()
        entry_names = [entry.filename for entry in entries]
        names = {entry.filename for entry in entries}
        unsafe_names = [
            name
            for name in entry_names
            if (
                "\\" in name
                or PurePosixPath(name).is_absolute()
                or ".." in PurePosixPath(name).parts
                or (
                    PurePosixPath(name).parts
                    and PurePosixPath(name).parts[0].endswith(":")
                )
            )
        ]
        require(
            len(entry_names) == len(names),
            "Archive contains duplicate entry names.",
            issues,
        )
        require(
            not unsafe_names,
            "Archive contains an absolute or traversal-capable entry path.",
            issues,
        )
        require(
            ARCHIVE_MANIFEST_NAME in names,
            f"Archive root must contain {ARCHIVE_MANIFEST_NAME}.",
            issues,
        )
        require(
            BUILD_REPORT_NAME not in names,
            f"Public archive must not contain private {BUILD_REPORT_NAME}.",
            issues,
        )
        if ARCHIVE_MANIFEST_NAME in names:
            try:
                release_manifest = json.loads(
                    archive.read(ARCHIVE_MANIFEST_NAME).decode("utf-8")
                )
                require(
                    release_manifest.get("sourceCommit") == expected_commit,
                    "Sanitized release manifest commit is incorrect.",
                    issues,
                )
                require(
                    release_manifest.get("platform") == platform_key,
                    "Sanitized release manifest platform is incorrect.",
                    issues,
                )
            except (UnicodeDecodeError, json.JSONDecodeError) as error:
                issues.append(
                    f"Invalid {ARCHIVE_MANIFEST_NAME}: {error}"
                )
        require(
            not any(name.startswith("SolarSystem-") for name in names),
            "Archive contains an extra top-level build directory.",
            issues,
        )
        require(
            not any(
                part.endswith(suffix)
                for name in names
                for part in PurePosixPath(name).parts
                for suffix in UNITY_DO_NOT_SHIP_DIRECTORY_SUFFIXES
            ),
            "Archive contains a Unity do-not-ship directory.",
            issues,
        )

        if platform_key == "windows":
            require(
                "Solar System Simulation.exe" in names,
                "Windows archive is missing the executable at its root.",
                issues,
            )
        elif platform_key == "webgl":
            require(
                "index.html" in names,
                "WebGL archive is missing index.html at its root.",
                issues,
            )
            require(
                any(name.startswith("Build/") for name in names),
                "WebGL archive is missing its Build directory.",
                issues,
            )
            require(
                any(name.startswith("TemplateData/") for name in names),
                "WebGL archive is missing its TemplateData directory.",
                issues,
            )
        elif platform_key == "macos":
            launcher_name = (
                "Solar System Simulation.app/Contents/MacOS/"
                "Solar System Simulation"
            )
            launcher = next(
                (entry for entry in entries if entry.filename == launcher_name),
                None,
            )
            require(
                launcher is not None,
                "macOS archive is missing the application launcher.",
                issues,
            )
            if launcher is not None:
                unix_mode = (launcher.external_attr >> 16) & 0xFFFF
                require(
                    launcher.create_system == 3,
                    "macOS archive launcher lacks Unix creator metadata.",
                    issues,
                )
                require(
                    unix_mode & 0o111 != 0,
                    "macOS archive launcher lacks executable permissions.",
                    issues,
                )

    if issues:
        formatted = "\n- ".join(issues)
        raise ReleaseArtifactError(
            f"{platform_key} archive validation failed:\n- {formatted}"
        )


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        while chunk := stream.read(1024 * 1024):
            digest.update(chunk)
    return digest.hexdigest().upper()


def package_all(
    release_root: Path,
    version: str,
    expected_commit: str,
    force: bool,
) -> dict[str, object]:
    release_root = release_root.resolve()
    archive_root = release_root / "Archives"
    validations: dict[str, dict[str, object]] = {}

    for platform_key, contract in PLATFORMS.items():
        root = build_directory(release_root, version, contract)
        validations[platform_key] = validate_build(
            platform_key,
            root,
            expected_commit,
            version,
        )

    destinations = [
        archive_path(archive_root, version, contract)
        for contract in PLATFORMS.values()
    ]
    if not force:
        existing = [path for path in destinations if path.exists()]
        if existing:
            formatted = "\n- ".join(str(path) for path in existing)
            raise ReleaseArtifactError(
                "Protected release archives already exist; no archive was "
                f"written:\n- {formatted}\nUse --force to replace them."
            )

    artifact_records: list[dict[str, object]] = []
    for platform_key, contract in PLATFORMS.items():
        root = build_directory(release_root, version, contract)
        destination = archive_path(archive_root, version, contract)
        limitation = (
            "Unsigned, unnotarized, and not runtime-tested on macOS."
            if platform_key == "macos"
            else None
        )
        sanitized_manifest = {
            "productName": "Solar System Simulation",
            "releaseVersion": version,
            "sourceCommit": expected_commit,
            "platform": platform_key,
            "unityTarget": validations[platform_key]["target"],
            "unityVersion": validations[platform_key]["unityVersion"],
            "buildUtc": validations[platform_key]["buildUtc"],
            "limitations": limitation,
        }
        write_archive(
            platform_key,
            root,
            destination,
            force,
            sanitized_manifest,
        )
        with zipfile.ZipFile(destination, "r") as archive:
            file_count = sum(
                1 for entry in archive.infolist() if not entry.is_dir()
            )
            packaged_uncompressed_bytes = sum(
                entry.file_size
                for entry in archive.infolist()
                if not entry.is_dir()
            )
        artifact_records.append(
            {
                "platform": platform_key,
                "sourceDirectory": str(root),
                "archive": str(destination),
                "sha256": sha256(destination),
                "bytes": destination.stat().st_size,
                "fileCount": file_count,
                "uncompressedBytes": packaged_uncompressed_bytes,
                "sourceBuildBytes": validations[platform_key][
                    "uncompressedBytes"
                ],
                "limitations": limitation,
            }
        )

    manifest = {
        "generatedUtc": datetime.now(timezone.utc).isoformat(),
        "releaseVersion": version,
        "sourceCommit": expected_commit,
        "artifacts": artifact_records,
    }
    manifest_path = archive_root / "release-artifact-manifest.json"
    manifest_path.write_text(
        json.dumps(manifest, indent=2) + "\n",
        encoding="utf-8",
    )
    checksums_path = archive_root / "SHA256SUMS.txt"
    checksums_path.write_text(
        "".join(
            f"{record['sha256']}  {Path(str(record['archive'])).name}\n"
            for record in artifact_records
        ),
        encoding="utf-8",
    )
    return manifest


def add_release_arguments(parser: argparse.ArgumentParser) -> None:
    parser.add_argument("--release-root", type=Path, required=True)
    parser.add_argument("--version", required=True)
    parser.add_argument("--expected-commit", required=True)


def parse_arguments() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=(
            "Validate or package Solar System release builds without adding "
            "an extra archive-root directory."
        )
    )
    subparsers = parser.add_subparsers(dest="command", required=True)

    validate_parser = subparsers.add_parser(
        "validate",
        help="Validate one generated build directory.",
    )
    validate_parser.add_argument(
        "--platform",
        choices=PLATFORMS,
        required=True,
    )
    validate_parser.add_argument("--path", type=Path, required=True)
    validate_parser.add_argument("--expected-commit", required=True)
    validate_parser.add_argument("--expected-version", required=True)

    validate_all_parser = subparsers.add_parser(
        "validate-all",
        help="Validate all three generated build directories.",
    )
    add_release_arguments(validate_all_parser)

    package_parser = subparsers.add_parser(
        "package",
        help="Validate and package all three release artifacts.",
    )
    add_release_arguments(package_parser)
    package_parser.add_argument(
        "--force",
        action="store_true",
        help="Replace existing ZIP files only after new archives validate.",
    )
    return parser.parse_args()


def main() -> int:
    arguments = parse_arguments()
    try:
        if arguments.command == "validate":
            result: object = validate_build(
                arguments.platform,
                arguments.path,
                arguments.expected_commit,
                arguments.expected_version,
            )
        elif arguments.command == "validate-all":
            release_root = arguments.release_root.resolve()
            result = {
                platform_key: validate_build(
                    platform_key,
                    build_directory(
                        release_root,
                        arguments.version,
                        contract,
                    ),
                    arguments.expected_commit,
                    arguments.version,
                )
                for platform_key, contract in PLATFORMS.items()
            }
        else:
            result = package_all(
                arguments.release_root,
                arguments.version,
                arguments.expected_commit,
                arguments.force,
            )
    except ReleaseArtifactError as error:
        print(str(error), file=sys.stderr)
        return 1

    print(json.dumps(result, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
