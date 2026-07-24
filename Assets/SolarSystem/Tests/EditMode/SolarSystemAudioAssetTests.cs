using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class SolarSystemAudioAssetTests
    {
        private const string AudioRoot = "Assets/SolarSystem/Content/Audio";

        [TestCase("Music/A_Music_OuterSpaceLoop.mp3", false)]
        [TestCase("Ambience/CelestialBodies/Sun/A_Sun_BurningLoop.wav", true)]
        [TestCase("Ambience/CelestialBodies/Earth/A_Earth_ForestAmbienceLoop.mp3", true)]
        public void LoopingAudio_UsesStreamingImportPolicy(
            string relativePath,
            bool expectedMono)
        {
            AssertImporter(
                $"{AudioRoot}/{relativePath}",
                AudioClipLoadType.Streaming,
                expectedMono,
                false,
                true);
        }

        [TestCase("SFX/UI/A_UI_Select.ogg")]
        [TestCase("SFX/UI/A_UI_FocusConfirmation.ogg")]
        [TestCase("SFX/UI/A_UI_TimeTick.ogg")]
        [TestCase("SFX/UI/A_UI_ToggleScale.ogg")]
        public void InterfaceCue_UsesPreloadedMonoImportPolicy(string relativePath)
        {
            AssertImporter(
                $"{AudioRoot}/{relativePath}",
                AudioClipLoadType.DecompressOnLoad,
                true,
                true,
                false);
        }

        private static void AssertImporter(
            string path,
            AudioClipLoadType expectedLoadType,
            bool expectedMono,
            bool expectedPreload,
            bool expectedBackgroundLoad)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;

            Assert.That(clip, Is.Not.Null, path);
            Assert.That(importer, Is.Not.Null, path);
            Assert.That(importer.forceToMono, Is.EqualTo(expectedMono));
            Assert.That(importer.loadInBackground, Is.EqualTo(expectedBackgroundLoad));
            Assert.That(
                importer.defaultSampleSettings.loadType,
                Is.EqualTo(expectedLoadType));
            Assert.That(
                importer.defaultSampleSettings.preloadAudioData,
                Is.EqualTo(expectedPreload));
            Assert.That(
                importer.defaultSampleSettings.compressionFormat,
                Is.EqualTo(AudioCompressionFormat.Vorbis));
            Assert.That(importer.defaultSampleSettings.quality, Is.EqualTo(0.7f).Within(0.0001f));
            Assert.That(
                importer.defaultSampleSettings.sampleRateSetting,
                Is.EqualTo(AudioSampleRateSetting.OptimizeSampleRate));
        }
    }
}
