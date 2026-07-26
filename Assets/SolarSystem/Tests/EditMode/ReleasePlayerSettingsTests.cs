using NUnit.Framework;
using Tanvir.SolarSystem.Release;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class ReleasePlayerSettingsTests
    {
        [Test]
        public void ApprovedReleaseIdentityAndDesktopWindow_AreSerialized()
        {
            Assert.That(
                PlayerSettings.companyName,
                Is.EqualTo(ReleaseBuildContract.CompanyName));
            Assert.That(
                PlayerSettings.productName,
                Is.EqualTo(ReleaseBuildContract.ProductName));
            Assert.That(
                PlayerSettings.bundleVersion,
                Is.EqualTo(ReleaseBuildContract.Version));
            Assert.That(
                PlayerSettings.GetApplicationIdentifier(
                    NamedBuildTarget.Standalone),
                Is.EqualTo(ReleaseBuildContract.ApplicationIdentifier));
            Assert.That(
                PlayerSettings.fullScreenMode,
                Is.EqualTo(FullScreenMode.Windowed));
            Assert.That(
                PlayerSettings.defaultScreenWidth,
                Is.EqualTo(ReleaseBuildContract.WindowsWidth));
            Assert.That(
                PlayerSettings.defaultScreenHeight,
                Is.EqualTo(ReleaseBuildContract.WindowsHeight));
            Assert.That(PlayerSettings.resizableWindow, Is.True);
            Assert.That(PlayerSettings.allowFullscreenSwitch, Is.True);
            Assert.That(PlayerSettings.runInBackground, Is.False);
            Assert.That(PlayerSettings.usePlayerLog, Is.True);
            Assert.That(
                PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone),
                Is.EqualTo(ScriptingImplementation.IL2CPP));
        }

        [Test]
        public void ApprovedWebGlPublishingSettings_AreSerialized()
        {
            Assert.That(
                PlayerSettings.defaultWebScreenWidth,
                Is.EqualTo(ReleaseBuildContract.WebWidth));
            Assert.That(
                PlayerSettings.defaultWebScreenHeight,
                Is.EqualTo(ReleaseBuildContract.WebHeight));
            Assert.That(
                PlayerSettings.WebGL.compressionFormat,
                Is.EqualTo(WebGLCompressionFormat.Brotli));
            Assert.That(PlayerSettings.WebGL.decompressionFallback, Is.True);
            Assert.That(PlayerSettings.WebGL.dataCaching, Is.True);
        }

        [Test]
        public void ReleaseContract_IncludesUnsignedUniversalMacOsTarget()
        {
            Assert.That(
                ReleaseBuildContract.MacOsUniversalArchitecture,
                Is.EqualTo(2));
            Assert.That(
                ReleaseBuildContract.MacOsDirectory,
                Does.Contain("macOS-Universal"));
            Assert.That(
                ReleaseBuildContract.MacOsApplication,
                Does.EndWith(".app"));
        }

        [Test]
        public void ProductionScene_IsTheOnlyEnabledReleaseScene()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            Assert.That(scenes, Has.Length.EqualTo(1));
            Assert.That(scenes[0].enabled, Is.True);
            Assert.That(
                scenes[0].path,
                Is.EqualTo(ReleaseBuildContract.ProductionScenePath));
        }
    }
}
