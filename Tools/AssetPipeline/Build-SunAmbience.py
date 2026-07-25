"""Build the Unity-ready Sun ambience from the retained licensed master."""

from __future__ import annotations

import argparse
import array
import hashlib
import math
from pathlib import Path
import sys
import wave


EXPECTED_SOURCE_SHA256 = (
    "85ca0cc60d0c037fff8b185e31ad1fcdbda6ce45eee17c3ee1318d1b8f59e330"
)
DEFAULT_CROSSFADE_MS = 100
DEFAULT_PEAK_DBFS = -3.0


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def read_pcm32_stereo(path: Path) -> tuple[int, list[float]]:
    with wave.open(str(path), "rb") as source:
        if source.getcomptype() != "NONE":
            raise ValueError("The source must be an uncompressed PCM WAV.")
        if source.getnchannels() != 2 or source.getsampwidth() != 4:
            raise ValueError("Expected a 32-bit stereo PCM source WAV.")

        sample_rate = source.getframerate()
        samples = array.array("i")
        samples.frombytes(source.readframes(source.getnframes()))

    if sys.byteorder != "little":
        samples.byteswap()

    scale = float(1 << 31)
    mono = [
        (samples[index] + samples[index + 1]) / (2.0 * scale)
        for index in range(0, len(samples), 2)
    ]
    return sample_rate, mono


def crossfade_loop(samples: list[float], frame_count: int) -> list[float]:
    if frame_count <= 0:
        raise ValueError("Crossfade duration must contain at least one frame.")
    if frame_count * 2 >= len(samples):
        raise ValueError("Crossfade duration must be shorter than half the clip.")

    head = samples[:frame_count]
    tail = samples[-frame_count:]
    blend = []
    for index, (tail_sample, head_sample) in enumerate(zip(tail, head)):
        phase = (index + 1) / (frame_count + 1)
        weight = 0.5 - (0.5 * math.cos(math.pi * phase))
        blend.append((tail_sample * (1.0 - weight)) + (head_sample * weight))

    return samples[frame_count:-frame_count] + blend


def normalize_peak(samples: list[float], peak_dbfs: float) -> list[float]:
    measured_peak = max(abs(sample) for sample in samples)
    if measured_peak <= 0.0:
        raise ValueError("The source contains no audible samples.")

    target_peak = 10.0 ** (peak_dbfs / 20.0)
    gain = target_peak / measured_peak
    return [sample * gain for sample in samples]


def write_pcm16_mono(path: Path, sample_rate: int, samples: list[float]) -> None:
    encoded = array.array(
        "h",
        (
            max(-32767, min(32767, round(sample * 32767.0)))
            for sample in samples
        ),
    )
    if sys.byteorder != "little":
        encoded.byteswap()

    path.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(path), "wb") as destination:
        destination.setnchannels(1)
        destination.setsampwidth(2)
        destination.setframerate(sample_rate)
        destination.writeframes(encoded.tobytes())


def main() -> int:
    project_root = Path(__file__).resolve().parents[2]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--source",
        type=Path,
        default=project_root
        / "SourceAssets/ThirdParty/Audio/OpenGameArt/PagDev/fire.wav",
    )
    parser.add_argument(
        "--destination",
        type=Path,
        default=project_root
        / (
            "Assets/SolarSystem/Content/Audio/Ambience/"
            "CelestialBodies/Sun/A_Sun_BurningLoop.wav"
        ),
    )
    parser.add_argument("--crossfade-ms", type=int, default=DEFAULT_CROSSFADE_MS)
    parser.add_argument("--peak-dbfs", type=float, default=DEFAULT_PEAK_DBFS)
    arguments = parser.parse_args()

    source_hash = sha256(arguments.source)
    if source_hash != EXPECTED_SOURCE_SHA256:
        raise ValueError(
            f"Source SHA-256 mismatch: expected {EXPECTED_SOURCE_SHA256}, "
            f"found {source_hash}."
        )

    sample_rate, samples = read_pcm32_stereo(arguments.source)
    crossfade_frames = round(sample_rate * arguments.crossfade_ms / 1000.0)
    processed = crossfade_loop(samples, crossfade_frames)
    processed = normalize_peak(processed, arguments.peak_dbfs)
    write_pcm16_mono(arguments.destination, sample_rate, processed)

    print(f"Source SHA-256:      {source_hash.upper()}")
    print(f"Derivative SHA-256:  {sha256(arguments.destination).upper()}")
    print(f"Output: {len(processed)} mono PCM16 frames at {sample_rate} Hz")
    print(
        f"Processing: {arguments.crossfade_ms} ms loop crossfade, "
        f"{arguments.peak_dbfs:.1f} dBFS peak"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
