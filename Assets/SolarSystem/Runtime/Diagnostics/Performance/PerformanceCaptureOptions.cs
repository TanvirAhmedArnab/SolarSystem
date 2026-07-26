using System;
using System.Globalization;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    /// <summary>
    /// Defines validated activation and sampling options for a reproducible
    /// Solar System performance capture.
    /// </summary>
    public sealed class PerformanceCaptureOptions
    {
        /// <summary>Command-line switch that activates the dormant harness.</summary>
        public const string ActivationArgument = "-solarSystemPerformance";

        /// <summary>Optional absolute or process-relative JSON output path.</summary>
        public const string OutputArgument = "-solarSystemPerformanceOutput";

        /// <summary>Optional full Git commit recorded in the result document.</summary>
        public const string CommitArgument = "-solarSystemPerformanceCommit";

        /// <summary>Optional warmup-frame override.</summary>
        public const string WarmupFramesArgument =
            "-solarSystemPerformanceWarmupFrames";

        /// <summary>Optional minimum warmup-duration override.</summary>
        public const string WarmupSecondsArgument =
            "-solarSystemPerformanceWarmupSeconds";

        /// <summary>Optional sampled-frame override.</summary>
        public const string SampleFramesArgument =
            "-solarSystemPerformanceSampleFrames";

        /// <summary>Optional minimum sampled-duration override.</summary>
        public const string SampleSecondsArgument =
            "-solarSystemPerformanceSampleSeconds";

        /// <summary>Prevents an automated standalone capture from quitting.</summary>
        public const string NoQuitArgument = "-solarSystemPerformanceNoQuit";

        /// <summary>Default warmup frames per scenario.</summary>
        public const int DefaultWarmupFrames = 60;

        /// <summary>Default minimum warmup duration per scenario.</summary>
        public const float DefaultWarmupSeconds = 1f;

        /// <summary>Default sampled frames per scenario.</summary>
        public const int DefaultSampleFrames = 240;

        /// <summary>Default minimum sampled duration per scenario.</summary>
        public const float DefaultSampleSeconds = 3f;

        /// <summary>Maximum samples retained for one scenario and metric.</summary>
        public const int MaximumSamples = 8192;

        private PerformanceCaptureOptions(
            bool isEnabled,
            string outputPath,
            string commitSha,
            int warmupFrames,
            float warmupSeconds,
            int sampleFrames,
            float sampleSeconds,
            bool quitOnComplete)
        {
            IsEnabled = isEnabled;
            OutputPath = outputPath;
            CommitSha = commitSha;
            WarmupFrames = warmupFrames;
            WarmupSeconds = warmupSeconds;
            SampleFrames = sampleFrames;
            SampleSeconds = sampleSeconds;
            QuitOnComplete = quitOnComplete;
        }

        /// <summary>Gets whether the harness was explicitly requested.</summary>
        public bool IsEnabled { get; }

        /// <summary>Gets the optional JSON output path.</summary>
        public string OutputPath { get; }

        /// <summary>Gets the supplied build commit or an explicit unknown marker.</summary>
        public string CommitSha { get; }

        /// <summary>Gets the minimum warmup frames per scenario.</summary>
        public int WarmupFrames { get; }

        /// <summary>Gets the minimum warmup duration per scenario.</summary>
        public float WarmupSeconds { get; }

        /// <summary>Gets the minimum sampled frames per scenario.</summary>
        public int SampleFrames { get; }

        /// <summary>Gets the minimum sampled duration per scenario.</summary>
        public float SampleSeconds { get; }

        /// <summary>Gets whether a standalone process should quit after capture.</summary>
        public bool QuitOnComplete { get; }

        /// <summary>Parses project-owned arguments while ignoring unrelated Unity arguments.</summary>
        public static PerformanceCaptureOptions Parse(string[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            bool enabled = Contains(arguments, ActivationArgument);
            string outputPath = ReadOptionalValue(arguments, OutputArgument);
            string commitSha =
                ReadOptionalValue(arguments, CommitArgument) ?? "UNSPECIFIED";
            int warmupFrames = ReadOptionalInt(
                arguments,
                WarmupFramesArgument,
                DefaultWarmupFrames,
                0,
                MaximumSamples);
            float warmupSeconds = ReadOptionalFloat(
                arguments,
                WarmupSecondsArgument,
                DefaultWarmupSeconds,
                0f,
                30f);
            int sampleFrames = ReadOptionalInt(
                arguments,
                SampleFramesArgument,
                DefaultSampleFrames,
                60,
                MaximumSamples);
            float sampleSeconds = ReadOptionalFloat(
                arguments,
                SampleSecondsArgument,
                DefaultSampleSeconds,
                0.5f,
                30f);

            return new PerformanceCaptureOptions(
                enabled,
                outputPath,
                commitSha,
                warmupFrames,
                warmupSeconds,
                sampleFrames,
                sampleSeconds,
                !Contains(arguments, NoQuitArgument));
        }

        /// <summary>Creates explicit options for the project-owned Editor diagnostic.</summary>
        public static PerformanceCaptureOptions CreateEditorDiagnostic(
            string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException(
                    "An Editor diagnostic requires a result path.",
                    nameof(outputPath));
            }

            return new PerformanceCaptureOptions(
                true,
                outputPath,
                "WORKTREE-EDITOR-DIAGNOSTIC",
                DefaultWarmupFrames,
                DefaultWarmupSeconds,
                DefaultSampleFrames,
                DefaultSampleSeconds,
                false);
        }

        private static bool Contains(string[] arguments, string expected)
        {
            for (int index = 0; index < arguments.Length; index++)
            {
                if (string.Equals(
                    arguments[index],
                    expected,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ReadOptionalValue(
            string[] arguments,
            string option)
        {
            for (int index = 0; index < arguments.Length; index++)
            {
                if (!string.Equals(
                    arguments[index],
                    option,
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (index + 1 >= arguments.Length ||
                    string.IsNullOrWhiteSpace(arguments[index + 1]) ||
                    arguments[index + 1].StartsWith(
                        "-",
                        StringComparison.Ordinal))
                {
                    throw new ArgumentException(
                        $"Performance option '{option}' requires a value.",
                        nameof(arguments));
                }

                return arguments[index + 1];
            }

            return null;
        }

        private static int ReadOptionalInt(
            string[] arguments,
            string option,
            int fallback,
            int minimum,
            int maximum)
        {
            string value = ReadOptionalValue(arguments, option);
            if (value == null)
            {
                return fallback;
            }

            if (!int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int parsed) ||
                parsed < minimum ||
                parsed > maximum)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(arguments),
                    $"Performance option '{option}' must be between " +
                    $"{minimum} and {maximum}.");
            }

            return parsed;
        }

        private static float ReadOptionalFloat(
            string[] arguments,
            string option,
            float fallback,
            float minimum,
            float maximum)
        {
            string value = ReadOptionalValue(arguments, option);
            if (value == null)
            {
                return fallback;
            }

            if (!float.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float parsed) ||
                !float.IsFinite(parsed) ||
                parsed < minimum ||
                parsed > maximum)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(arguments),
                    $"Performance option '{option}' must be between " +
                    $"{minimum.ToString(CultureInfo.InvariantCulture)} and " +
                    $"{maximum.ToString(CultureInfo.InvariantCulture)}.");
            }

            return parsed;
        }
    }
}
