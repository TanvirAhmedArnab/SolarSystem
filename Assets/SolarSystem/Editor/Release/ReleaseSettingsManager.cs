using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Release;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Tanvir.SolarSystem.Editor.Release
{
    /// <summary>
    /// Applies and validates the owner-approved release-facing Player Settings.
    /// </summary>
    public static class ReleaseSettingsManager
    {
        private const string ApplyMenuPath =
            "Tools/Solar System/Release/Apply Approved Player Settings";
        private const string ValidateMenuPath =
            "Tools/Solar System/Release/Validate Release Settings";

        /// <summary>Applies the approved shared, Windows, and WebGL settings.</summary>
        [MenuItem(ApplyMenuPath)]
        public static void ApplyApprovedSettings()
        {
            PlayerSettings.companyName = ReleaseBuildContract.CompanyName;
            PlayerSettings.productName = ReleaseBuildContract.ProductName;
            PlayerSettings.bundleVersion = ReleaseBuildContract.Version;
            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.Standalone,
                ReleaseBuildContract.ApplicationIdentifier);
            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.WebGL,
                ReleaseBuildContract.ApplicationIdentifier);

            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = ReleaseBuildContract.WindowsWidth;
            PlayerSettings.defaultScreenHeight = ReleaseBuildContract.WindowsHeight;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.allowFullscreenSwitch = true;
            PlayerSettings.runInBackground = false;
            PlayerSettings.usePlayerLog = true;
            PlayerSettings.SetScriptingBackend(
                NamedBuildTarget.Standalone,
                ScriptingImplementation.IL2CPP);

            PlayerSettings.defaultWebScreenWidth = ReleaseBuildContract.WebWidth;
            PlayerSettings.defaultWebScreenHeight = ReleaseBuildContract.WebHeight;
            PlayerSettings.WebGL.compressionFormat =
                WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.dataCaching = true;

            AssetDatabase.SaveAssets();
            string[] issues = CollectIssues(includeModuleAvailability: false);
            if (issues.Length > 0)
            {
                throw new InvalidOperationException(
                    "Approved settings did not apply cleanly:\n- " +
                    string.Join("\n- ", issues));
            }

            Debug.Log(
                "Applied the approved Solar System release Player Settings.");
        }

        /// <summary>Reports release-setting and module drift to the Console.</summary>
        [MenuItem(ValidateMenuPath)]
        public static void ValidateFromMenu()
        {
            string[] issues = CollectIssues(includeModuleAvailability: true);
            if (issues.Length > 0)
            {
                Debug.LogError(
                    "Release settings validation failed:\n- " +
                    string.Join("\n- ", issues));
                return;
            }

            Debug.Log("Release settings and platform modules are valid.");
        }

        /// <summary>Returns every detected release-settings issue.</summary>
        public static string[] CollectIssues(bool includeModuleAvailability)
        {
            var issues = new List<string>();
            RequireEqual(
                issues,
                "Company name",
                ReleaseBuildContract.CompanyName,
                PlayerSettings.companyName);
            RequireEqual(
                issues,
                "Product name",
                ReleaseBuildContract.ProductName,
                PlayerSettings.productName);
            RequireEqual(
                issues,
                "Version",
                ReleaseBuildContract.Version,
                PlayerSettings.bundleVersion);
            RequireEqual(
                issues,
                "Standalone application identifier",
                ReleaseBuildContract.ApplicationIdentifier,
                PlayerSettings.GetApplicationIdentifier(
                    NamedBuildTarget.Standalone));
            RequireEqual(
                issues,
                "WebGL application identifier",
                ReleaseBuildContract.ApplicationIdentifier,
                PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.WebGL));

            if (PlayerSettings.fullScreenMode != FullScreenMode.Windowed)
            {
                issues.Add("Windows first launch must be windowed.");
            }

            if (PlayerSettings.defaultScreenWidth != ReleaseBuildContract.WindowsWidth ||
                PlayerSettings.defaultScreenHeight != ReleaseBuildContract.WindowsHeight)
            {
                issues.Add("Windows default resolution must be 1280x720.");
            }

            if (!PlayerSettings.resizableWindow)
            {
                issues.Add("The desktop window must be resizable.");
            }

            if (!PlayerSettings.allowFullscreenSwitch)
            {
                issues.Add("The desktop fullscreen switch must be enabled.");
            }

            if (PlayerSettings.runInBackground)
            {
                issues.Add("Run In Background must remain disabled.");
            }

            if (!PlayerSettings.usePlayerLog)
            {
                issues.Add("Player logging must remain enabled for release QA.");
            }

            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone) !=
                ScriptingImplementation.IL2CPP)
            {
                issues.Add("The serialized Windows release backend must be IL2CPP.");
            }

            if (PlayerSettings.defaultWebScreenWidth != ReleaseBuildContract.WebWidth ||
                PlayerSettings.defaultWebScreenHeight != ReleaseBuildContract.WebHeight)
            {
                issues.Add("WebGL default canvas must be 960x540.");
            }

            if (PlayerSettings.WebGL.compressionFormat !=
                WebGLCompressionFormat.Brotli)
            {
                issues.Add("WebGL compression must be Brotli.");
            }

            if (!PlayerSettings.WebGL.decompressionFallback)
            {
                issues.Add("WebGL Decompression Fallback must be enabled.");
            }

            if (!PlayerSettings.WebGL.dataCaching)
            {
                issues.Add("WebGL data caching must be enabled.");
            }

            ValidateScenes(issues);
            if (includeModuleAvailability)
            {
                ValidateModule(
                    issues,
                    "Windows x86-64",
                    BuildTargetGroup.Standalone,
                    BuildTarget.StandaloneWindows64);
                ValidateModule(
                    issues,
                    "macOS",
                    BuildTargetGroup.Standalone,
                    BuildTarget.StandaloneOSX);
                ValidateModule(
                    issues,
                    "WebGL",
                    BuildTargetGroup.WebGL,
                    BuildTarget.WebGL);
            }

            return issues.ToArray();
        }

        /// <summary>Throws when the release contract is not currently satisfied.</summary>
        public static void ThrowIfInvalid(bool includeModuleAvailability)
        {
            string[] issues = CollectIssues(includeModuleAvailability);
            if (issues.Length > 0)
            {
                throw new InvalidOperationException(
                    "Release validation failed:\n- " +
                    string.Join("\n- ", issues));
            }
        }

        private static void ValidateScenes(ICollection<string> issues)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes.Length != 1 ||
                !scenes[0].enabled ||
                !string.Equals(
                    scenes[0].path,
                    ReleaseBuildContract.ProductionScenePath,
                    StringComparison.Ordinal))
            {
                issues.Add(
                    "The production SolarSystem scene must be the sole enabled build scene.");
            }
        }

        private static void ValidateModule(
            ICollection<string> issues,
            string label,
            BuildTargetGroup group,
            BuildTarget target)
        {
            if (!BuildPipeline.IsBuildTargetSupported(group, target))
            {
                issues.Add(
                    $"{label} build support is not installed for this Unity Editor.");
            }
        }

        private static void RequireEqual(
            ICollection<string> issues,
            string label,
            string expected,
            string actual)
        {
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                issues.Add($"{label} must be '{expected}', not '{actual}'.");
            }
        }
    }
}
