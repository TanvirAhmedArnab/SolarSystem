"""Tests for the deterministic release artifact validator and packager."""

from __future__ import annotations

import importlib.util
import json
import sys
import tempfile
import unittest
import warnings
import zipfile
from pathlib import Path


MODULE_PATH = Path(__file__).parents[1] / "release_artifacts.py"
SPEC = importlib.util.spec_from_file_location("release_artifacts", MODULE_PATH)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"Could not load {MODULE_PATH}")
release_artifacts = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = release_artifacts
SPEC.loader.exec_module(release_artifacts)


class ReleaseArtifactTests(unittest.TestCase):
    commit = "0123456789abcdef0123456789abcdef01234567"
    version = "1.0.0"

    def setUp(self) -> None:
        self.temporary_directory = tempfile.TemporaryDirectory()
        self.release_root = Path(self.temporary_directory.name)
        self._create_windows_fixture()
        self._create_webgl_fixture()
        self._create_macos_fixture()

    def tearDown(self) -> None:
        self.temporary_directory.cleanup()

    def test_validate_all_accepts_complete_same_commit_fixtures(self) -> None:
        for platform_key, contract in release_artifacts.PLATFORMS.items():
            result = release_artifacts.validate_build(
                platform_key,
                release_artifacts.build_directory(
                    self.release_root,
                    self.version,
                    contract,
                ),
                self.commit,
                self.version,
            )
            self.assertEqual(result["sourceCommit"], self.commit)
            self.assertEqual(result["releaseVersion"], self.version)
            self.assertEqual(result["errorCount"], 0)

    def test_commit_mismatch_is_rejected(self) -> None:
        windows = release_artifacts.build_directory(
            self.release_root,
            self.version,
            release_artifacts.PLATFORMS["windows"],
        )
        with self.assertRaises(release_artifacts.ReleaseArtifactError):
            release_artifacts.validate_build(
                "windows",
                windows,
                "ffffffffffffffffffffffffffffffffffffffff",
                self.version,
            )

    def test_version_mismatch_is_rejected(self) -> None:
        windows = release_artifacts.build_directory(
            self.release_root,
            self.version,
            release_artifacts.PLATFORMS["windows"],
        )
        with self.assertRaises(release_artifacts.ReleaseArtifactError):
            release_artifacts.validate_build(
                "windows",
                windows,
                self.commit,
                "9.9.9",
            )

    def test_existing_archive_aborts_before_any_archive_is_written(self) -> None:
        archive_root = self.release_root / "Archives"
        archive_root.mkdir()
        webgl_archive = archive_root / (
            f"SolarSystem-{self.version}-WebGL.zip"
        )
        webgl_archive.write_bytes(b"protected")

        with self.assertRaises(release_artifacts.ReleaseArtifactError):
            release_artifacts.package_all(
                self.release_root,
                self.version,
                self.commit,
                force=False,
            )

        windows_archive = archive_root / (
            f"SolarSystem-{self.version}-Windows-x86_64.zip"
        )
        self.assertFalse(windows_archive.exists())
        self.assertEqual(webgl_archive.read_bytes(), b"protected")

    def test_archive_with_traversal_entry_is_rejected(self) -> None:
        archive_path = self.release_root / "unsafe.zip"
        manifest = {
            "sourceCommit": self.commit,
            "platform": "windows",
        }
        with zipfile.ZipFile(archive_path, "w") as archive:
            archive.writestr(
                release_artifacts.ARCHIVE_MANIFEST_NAME,
                json.dumps(manifest),
            )
            archive.writestr("Solar System Simulation.exe", b"fixture")
            archive.writestr("../escape.txt", b"unsafe")

        with self.assertRaises(release_artifacts.ReleaseArtifactError):
            release_artifacts.validate_archive(
                "windows",
                archive_path,
                self.commit,
            )

    def test_archive_with_duplicate_entry_is_rejected(self) -> None:
        archive_path = self.release_root / "duplicate.zip"
        manifest = {
            "sourceCommit": self.commit,
            "platform": "windows",
        }
        with warnings.catch_warnings():
            warnings.simplefilter("ignore", UserWarning)
            with zipfile.ZipFile(archive_path, "w") as archive:
                archive.writestr(
                    release_artifacts.ARCHIVE_MANIFEST_NAME,
                    json.dumps(manifest),
                )
                archive.writestr("Solar System Simulation.exe", b"first")
                archive.writestr("Solar System Simulation.exe", b"second")

        with self.assertRaises(release_artifacts.ReleaseArtifactError):
            release_artifacts.validate_archive(
                "windows",
                archive_path,
                self.commit,
            )

    def test_package_all_is_root_correct_filtered_and_deterministic(self) -> None:
        first_manifest = release_artifacts.package_all(
            self.release_root,
            self.version,
            self.commit,
            force=False,
        )
        first_hashes = {
            record["platform"]: record["sha256"]
            for record in first_manifest["artifacts"]
        }
        second_manifest = release_artifacts.package_all(
            self.release_root,
            self.version,
            self.commit,
            force=True,
        )
        second_hashes = {
            record["platform"]: record["sha256"]
            for record in second_manifest["artifacts"]
        }
        self.assertEqual(first_hashes, second_hashes)

        archive_root = self.release_root / "Archives"
        windows_archive = archive_root / (
            f"SolarSystem-{self.version}-Windows-x86_64.zip"
        )
        with zipfile.ZipFile(windows_archive) as archive:
            names = set(archive.namelist())
            windows_zip_uncompressed_bytes = sum(
                entry.file_size
                for entry in archive.infolist()
                if not entry.is_dir()
            )
        self.assertIn("Solar System Simulation.exe", names)
        self.assertIn(release_artifacts.ARCHIVE_MANIFEST_NAME, names)
        self.assertNotIn(release_artifacts.BUILD_REPORT_NAME, names)
        self.assertFalse(
            any(
                excluded in Path(name).parts
                for name in names
                for excluded in release_artifacts.WINDOWS_EXCLUDED_DIRECTORIES
            )
        )
        self.assertFalse(
            any(name.startswith("SolarSystem-") for name in names)
        )
        windows_record = next(
            record
            for record in first_manifest["artifacts"]
            if record["platform"] == "windows"
        )
        self.assertEqual(
            windows_record["uncompressedBytes"],
            windows_zip_uncompressed_bytes,
        )

        webgl_archive = archive_root / (
            f"SolarSystem-{self.version}-WebGL.zip"
        )
        with zipfile.ZipFile(webgl_archive) as archive:
            webgl_names = set(archive.namelist())
        self.assertIn("index.html", webgl_names)
        self.assertIn("Build/game.data.unityweb", webgl_names)
        self.assertIn("TemplateData/style.css", webgl_names)

        macos_archive = archive_root / (
            f"SolarSystem-{self.version}-macOS-Universal.zip"
        )
        launcher_name = (
            "Solar System Simulation.app/Contents/MacOS/"
            "Solar System Simulation"
        )
        with zipfile.ZipFile(macos_archive) as archive:
            launcher = archive.getinfo(launcher_name)
        self.assertEqual(launcher.create_system, 3)
        self.assertNotEqual((launcher.external_attr >> 16) & 0o111, 0)

        checksum_lines = (
            archive_root / "SHA256SUMS.txt"
        ).read_text(encoding="utf-8").splitlines()
        self.assertEqual(len(checksum_lines), 3)
        self.assertTrue(
            (archive_root / "release-artifact-manifest.json").is_file()
        )

    def _create_report(self, root: Path, target: str) -> None:
        root.mkdir(parents=True, exist_ok=True)
        report = {
            "sourceCommit": self.commit,
            "releaseVersion": self.version,
            "target": target,
            "result": "Succeeded",
            "warningCount": 0,
            "errorCount": 0,
        }
        (root / release_artifacts.BUILD_REPORT_NAME).write_text(
            json.dumps(report),
            encoding="utf-8",
        )

    def _create_windows_fixture(self) -> None:
        root = release_artifacts.build_directory(
            self.release_root,
            self.version,
            release_artifacts.PLATFORMS["windows"],
        )
        self._create_report(root, "StandaloneWindows64")
        for file_name in (
            "Solar System Simulation.exe",
            "UnityPlayer.dll",
            "GameAssembly.dll",
        ):
            (root / file_name).write_bytes(b"fixture")
        data = root / "Solar System Simulation_Data"
        data.mkdir()
        (data / "globalgamemanagers").write_bytes(b"fixture")
        for excluded in release_artifacts.WINDOWS_EXCLUDED_DIRECTORIES:
            directory = root / excluded
            directory.mkdir()
            (directory / "must-not-ship.txt").write_text(
                "excluded",
                encoding="utf-8",
            )

    def _create_webgl_fixture(self) -> None:
        root = release_artifacts.build_directory(
            self.release_root,
            self.version,
            release_artifacts.PLATFORMS["webgl"],
        )
        self._create_report(root, "WebGL")
        (root / "index.html").write_text("<html></html>", encoding="utf-8")
        build = root / "Build"
        build.mkdir()
        for file_name in (
            "game.loader.js",
            "game.data.unityweb",
            "game.framework.js.unityweb",
            "game.wasm.unityweb",
        ):
            (build / file_name).write_bytes(b"fixture")
        template_data = root / "TemplateData"
        template_data.mkdir()
        (template_data / "style.css").write_text("", encoding="utf-8")

    def _create_macos_fixture(self) -> None:
        root = release_artifacts.build_directory(
            self.release_root,
            self.version,
            release_artifacts.PLATFORMS["macos"],
        )
        self._create_report(root, "StandaloneOSX")
        contents = root / "Solar System Simulation.app" / "Contents"
        contents.mkdir(parents=True)
        (contents / "Info.plist").write_text(
            "<plist></plist>",
            encoding="utf-8",
        )
        macos = contents / "MacOS"
        macos.mkdir()
        (macos / "Solar System Simulation").write_bytes(
            bytes.fromhex("CAFEBABE") + b"fixture"
        )
        data = contents / "Resources" / "Data"
        data.mkdir(parents=True)
        (data / "globalgamemanagers").write_bytes(b"fixture")


if __name__ == "__main__":
    unittest.main()
