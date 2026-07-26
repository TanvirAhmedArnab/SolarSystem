using System;
using NUnit.Framework;
using Tanvir.SolarSystem.Diagnostics.Performance;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class PerformanceCaptureOptionsTests
    {
        [Test]
        public void Parse_WithoutActivation_RemainsDormant()
        {
            PerformanceCaptureOptions options =
                PerformanceCaptureOptions.Parse(
                    new[] { "-batchmode", "-projectPath", "C:\\Project" });

            Assert.That(options.IsEnabled, Is.False);
            Assert.That(
                options.WarmupFrames,
                Is.EqualTo(PerformanceCaptureOptions.DefaultWarmupFrames));
            Assert.That(
                options.SampleFrames,
                Is.EqualTo(PerformanceCaptureOptions.DefaultSampleFrames));
            Assert.That(options.QuitOnComplete, Is.True);
        }

        [Test]
        public void Parse_ExplicitValues_UsesInvariantValidatedContract()
        {
            PerformanceCaptureOptions options =
                PerformanceCaptureOptions.Parse(
                    new[]
                    {
                        PerformanceCaptureOptions.ActivationArgument,
                        PerformanceCaptureOptions.OutputArgument,
                        "C:\\Results\\capture.json",
                        PerformanceCaptureOptions.CommitArgument,
                        "0123456789abcdef",
                        PerformanceCaptureOptions.WarmupFramesArgument,
                        "90",
                        PerformanceCaptureOptions.WarmupSecondsArgument,
                        "1.5",
                        PerformanceCaptureOptions.SampleFramesArgument,
                        "360",
                        PerformanceCaptureOptions.SampleSecondsArgument,
                        "4.5",
                        PerformanceCaptureOptions.NoQuitArgument
                    });

            Assert.That(options.IsEnabled, Is.True);
            Assert.That(
                options.OutputPath,
                Is.EqualTo("C:\\Results\\capture.json"));
            Assert.That(
                options.CommitSha,
                Is.EqualTo("0123456789abcdef"));
            Assert.That(options.WarmupFrames, Is.EqualTo(90));
            Assert.That(options.WarmupSeconds, Is.EqualTo(1.5f));
            Assert.That(options.SampleFrames, Is.EqualTo(360));
            Assert.That(options.SampleSeconds, Is.EqualTo(4.5f));
            Assert.That(options.QuitOnComplete, Is.False);
        }

        [Test]
        public void Parse_MissingOptionValue_RejectsAmbiguousCapture()
        {
            Assert.That(
                () => PerformanceCaptureOptions.Parse(
                    new[]
                    {
                        PerformanceCaptureOptions.ActivationArgument,
                        PerformanceCaptureOptions.OutputArgument
                    }),
                Throws.ArgumentException);
        }

        [Test]
        public void Parse_OutOfRangeSampleFrames_RejectsCapture()
        {
            Assert.That(
                () => PerformanceCaptureOptions.Parse(
                    new[]
                    {
                        PerformanceCaptureOptions.ActivationArgument,
                        PerformanceCaptureOptions.SampleFramesArgument,
                        (
                            PerformanceCaptureOptions.MaximumSamples + 1
                        ).ToString()
                    }),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void CreateEditorDiagnostic_IsExplicitAndDoesNotQuit()
        {
            PerformanceCaptureOptions options =
                PerformanceCaptureOptions.CreateEditorDiagnostic(
                    "C:\\Project\\Temp\\Performance\\capture.json");

            Assert.That(options.IsEnabled, Is.True);
            Assert.That(options.QuitOnComplete, Is.False);
            Assert.That(
                options.CommitSha,
                Is.EqualTo("WORKTREE-EDITOR-DIAGNOSTIC"));
        }

        [Test]
        public void CreateEditorDiagnostic_WithoutPath_RejectsCapture()
        {
            Assert.That(
                () => PerformanceCaptureOptions.CreateEditorDiagnostic(" "),
                Throws.ArgumentException);
        }
    }
}
