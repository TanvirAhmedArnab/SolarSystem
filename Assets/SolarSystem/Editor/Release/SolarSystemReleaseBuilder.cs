using System;
using System.Diagnostics;
using System.IO;
using Tanvir.SolarSystem.Release;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Tanvir.SolarSystem.Editor.Release
{
    /// <summary>
    /// Produces deterministic non-development release artifacts for approved platforms.
    /// </summary>
    public static class SolarSystemReleaseBuilder
    {
        private const string MenuRoot = "Tools/Solar System/Release/Build/";

        /// <summary>Builds the Windows x86-64 IL2CPP release artifact.</summary>
        [MenuItem(MenuRoot + "Windows x86-64")]
        public static void BuildWindows()
        {
            BuildStandalone(
                BuildTarget.StandaloneWindows64,
                ScriptingImplementation.IL2CPP,
                architecture: null,
                Path.Combine(
                    ReleaseBuildContract.ReleaseRoot,
                    ReleaseBuildContract.WindowsDirectory,
                    ReleaseBuildContract.WindowsExecutable));
        }

        /// <summary>Builds the unsigned Universal macOS Mono artifact.</summary>
        [MenuItem(MenuRoot + "macOS Universal (Unsigned)")]
        public static void BuildMacOs()
        {
            BuildStandalone(
                BuildTarget.StandaloneOSX,
                ScriptingImplementation.Mono2x,
                ReleaseBuildContract.MacOsUniversalArchitecture,
                Path.Combine(
                    ReleaseBuildContract.ReleaseRoot,
                    ReleaseBuildContract.MacOsDirectory,
                    ReleaseBuildContract.MacOsApplication));
        }

        /// <summary>Builds the Brotli-compressed WebGL release artifact.</summary>
        [MenuItem(MenuRoot + "WebGL")]
        public static void BuildWebGl()
        {
            ReleaseSettingsManager.ThrowIfInvalid(
                includeModuleAvailability: true);
            EnsureCleanPushedSource();
            Build(
                BuildTarget.WebGL,
                Path.Combine(
                    ReleaseBuildContract.ReleaseRoot,
                    ReleaseBuildContract.WebGlDirectory),
                BuildOptions.None);
        }

        /// <summary>Builds Windows, macOS, and WebGL in approved order.</summary>
        [MenuItem(MenuRoot + "All Three Platforms")]
        public static void BuildAll()
        {
            ReleaseSettingsManager.ThrowIfInvalid(
                includeModuleAvailability: true);
            EnsureCleanPushedSource();

            BuildTarget restorationTarget =
                StandaloneTargetRestorationCoordinator
                    .ResolveRestorationTarget(
                        EditorUserBuildSettings.activeBuildTarget);
            ScriptingImplementation previousBackend =
                PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone);
            int previousArchitecture =
                PlayerSettings.GetArchitecture(NamedBuildTarget.Standalone);
            BuildTarget finalBuildTarget = BuildTarget.NoTarget;
            try
            {
                ConfigureStandalone(
                    ScriptingImplementation.IL2CPP,
                    architecture: null);
                finalBuildTarget = BuildTarget.StandaloneWindows64;
                Build(
                    finalBuildTarget,
                    Path.Combine(
                        ReleaseBuildContract.ReleaseRoot,
                        ReleaseBuildContract.WindowsDirectory,
                        ReleaseBuildContract.WindowsExecutable),
                    BuildOptions.CompressWithLz4HC);

                ConfigureStandalone(
                    ScriptingImplementation.Mono2x,
                    ReleaseBuildContract.MacOsUniversalArchitecture);
                finalBuildTarget = BuildTarget.StandaloneOSX;
                Build(
                    finalBuildTarget,
                    Path.Combine(
                        ReleaseBuildContract.ReleaseRoot,
                        ReleaseBuildContract.MacOsDirectory,
                        ReleaseBuildContract.MacOsApplication),
                    BuildOptions.CompressWithLz4HC);

                finalBuildTarget = BuildTarget.WebGL;
                Build(
                    finalBuildTarget,
                    Path.Combine(
                        ReleaseBuildContract.ReleaseRoot,
                        ReleaseBuildContract.WebGlDirectory),
                    BuildOptions.None);
            }
            finally
            {
                try
                {
                    RestoreStandaloneSettings(
                        previousBackend,
                        previousArchitecture);
                }
                finally
                {
                    if (finalBuildTarget != BuildTarget.NoTarget)
                    {
                        StandaloneTargetRestorationCoordinator.Request(
                            restorationTarget,
                            finalBuildTarget);
                    }
                }
            }
        }

        private static void BuildStandalone(
            BuildTarget target,
            ScriptingImplementation scriptingBackend,
            int? architecture,
            string outputPath)
        {
            ReleaseSettingsManager.ThrowIfInvalid(
                includeModuleAvailability: true);
            EnsureCleanPushedSource();
            BuildTarget restorationTarget =
                StandaloneTargetRestorationCoordinator
                    .ResolveRestorationTarget(
                        EditorUserBuildSettings.activeBuildTarget);
            ScriptingImplementation previousBackend =
                PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone);
            int previousArchitecture =
                PlayerSettings.GetArchitecture(NamedBuildTarget.Standalone);
            try
            {
                ConfigureStandalone(scriptingBackend, architecture);

                Build(target, outputPath, BuildOptions.CompressWithLz4HC);
            }
            finally
            {
                try
                {
                    RestoreStandaloneSettings(
                        previousBackend,
                        previousArchitecture);
                }
                finally
                {
                    StandaloneTargetRestorationCoordinator.Request(
                        restorationTarget,
                        target);
                }
            }
        }

        private static void ConfigureStandalone(
            ScriptingImplementation scriptingBackend,
            int? architecture)
        {
            PlayerSettings.SetScriptingBackend(
                NamedBuildTarget.Standalone,
                scriptingBackend);
            if (architecture.HasValue)
            {
                PlayerSettings.SetArchitecture(
                    NamedBuildTarget.Standalone,
                    architecture.Value);
            }
        }

        private static void RestoreStandaloneSettings(
            ScriptingImplementation scriptingBackend,
            int architecture)
        {
            PlayerSettings.SetArchitecture(
                NamedBuildTarget.Standalone,
                architecture);
            PlayerSettings.SetScriptingBackend(
                NamedBuildTarget.Standalone,
                scriptingBackend);
            AssetDatabase.SaveAssets();
        }

        private static void Build(
            BuildTarget target,
            string relativeOutputPath,
            BuildOptions options)
        {
            string projectRoot =
                Directory.GetParent(UnityEngine.Application.dataPath).FullName;
            string absoluteOutputPath =
                Path.GetFullPath(Path.Combine(projectRoot, relativeOutputPath));
            Directory.CreateDirectory(
                target == BuildTarget.WebGL
                    ? absoluteOutputPath
                    : Path.GetDirectoryName(absoluteOutputPath));

            var buildOptions = new BuildPlayerOptions
            {
                scenes = ReleaseBuildContract.CreateScenePaths(),
                locationPathName = absoluteOutputPath,
                target = target,
                options = options
            };
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            WriteReport(projectRoot, target, absoluteOutputPath, report.summary);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"{target} release build ended with {report.summary.result}.");
            }
        }

        private static void WriteReport(
            string projectRoot,
            BuildTarget target,
            string outputPath,
            BuildSummary summary)
        {
            string reportDirectory =
                target == BuildTarget.WebGL
                    ? outputPath
                    : Path.GetDirectoryName(outputPath);
            var evidence = new ReleaseBuildEvidence
            {
                utcTimestamp = DateTime.UtcNow.ToString("O"),
                unityVersion = UnityEngine.Application.unityVersion,
                releaseVersion = ReleaseBuildContract.Version,
                sourceCommit = ReadGitRevision(projectRoot),
                target = target.ToString(),
                result = summary.result.ToString(),
                warningCount = summary.totalWarnings,
                errorCount = summary.totalErrors,
                durationSeconds = summary.totalTime.TotalSeconds,
                outputBytes = summary.totalSize,
                outputPath = outputPath
            };
            File.WriteAllText(
                Path.Combine(
                    reportDirectory,
                    ReleaseBuildContract.BuildReportFileName),
                JsonUtility.ToJson(evidence, prettyPrint: true));
        }

        private static string ReadGitRevision(string projectRoot)
        {
            return RunGit(projectRoot, "rev-parse HEAD");
        }

        private static void EnsureCleanPushedSource()
        {
            string projectRoot =
                Directory.GetParent(UnityEngine.Application.dataPath).FullName;
            string changes = RunGit(projectRoot, "status --porcelain");
            if (!string.IsNullOrEmpty(changes))
            {
                throw new BuildFailedException(
                    "Release builds require a clean working tree.");
            }

            string head = RunGit(projectRoot, "rev-parse HEAD");
            string upstream = RunGit(projectRoot, "rev-parse @{u}");
            if (head == "unavailable" ||
                upstream == "unavailable" ||
                !string.Equals(head, upstream, StringComparison.Ordinal))
            {
                throw new BuildFailedException(
                    "Release builds require HEAD to match its pushed upstream.");
            }
        }

        private static string RunGit(string projectRoot, string arguments)
        {
            try
            {
                var startInfo = new ProcessStartInfo("git", arguments)
                {
                    WorkingDirectory = projectRoot,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    return process.ExitCode == 0 ? output : "unavailable";
                }
            }
            catch (Exception)
            {
                return "unavailable";
            }
        }

        [Serializable]
        private sealed class ReleaseBuildEvidence
        {
            public string utcTimestamp;
            public string unityVersion;
            public string releaseVersion;
            public string sourceCommit;
            public string target;
            public string result;
            public int warningCount;
            public int errorCount;
            public double durationSeconds;
            public ulong outputBytes;
            public string outputPath;
        }
    }
}
