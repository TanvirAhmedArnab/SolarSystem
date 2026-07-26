using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Profiling;
using UnityEngine;
using UnityApplication = UnityEngine.Application;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    /// <summary>
    /// Creates metric sources and immutable capture evidence outside the
    /// frame-driven harness coordinator.
    /// </summary>
    internal static class PerformanceEvidenceFactory
    {
        private const long SteadyMemoryBudgetBytes = 1_610_612_736L;
        private const long PeakMemoryBudgetBytes = 2_147_483_648L;
        private const long GpuMemoryBudgetBytes = 2_147_483_648L;
        private const double ColdLaunchBudgetSeconds = 10d;

        public static PerformanceMetricSource[] CreateMetricSources()
        {
            return new PerformanceMetricSource[]
            {
                new FrameTimeMetricSource(),
                new ProfilerMetricSource(
                    "cpuMainThread",
                    true,
                    SolarSystemPerformanceHarness
                        .CpuMainThreadP95BudgetNanoseconds,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "CPU Main Thread Frame Time"),
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Internal,
                        "Internal",
                        "Main Thread")),
                new ProfilerMetricSource(
                    "cpuRenderThread",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "CPU Render Thread Frame Time")),
                new ProfilerMetricSource(
                    "gpuFrameTime",
                    true,
                    SolarSystemPerformanceHarness.GpuP95BudgetNanoseconds,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "GPU Frame Time"),
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "FrameTime.GPU")),
                new ProfilerMetricSource(
                    "gcAllocatedInFrame",
                    true,
                    SolarSystemPerformanceHarness.GcAllocationP95BudgetBytes,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Memory,
                        "Memory",
                        "GC Allocated In Frame")),
                new ProfilerMetricSource(
                    "systemUsedMemory",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Memory,
                        "Memory",
                        "System Used Memory")),
                new ProfilerMetricSource(
                    "totalReservedMemory",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Memory,
                        "Memory",
                        "Total Reserved Memory")),
                new ProfilerMetricSource(
                    "batches",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "Batches Count")),
                new ProfilerMetricSource(
                    "standardDrawCalls",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "Standard Draw Calls Count")),
                new ProfilerMetricSource(
                    "srpBatcherDrawCalls",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "SRP Batcher Draw Calls Count")),
                new ProfilerMetricSource(
                    "setPassCalls",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "SetPass Calls Count")),
                new ProfilerMetricSource(
                    "triangles",
                    false,
                    0L,
                    false,
                    0L,
                    new ProfilerMetricCandidate(
                        ProfilerCategory.Render,
                        "Render",
                        "Triangles Count"))
            };
        }

        public static string ResolveResultPath(
            PerformanceCaptureOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.OutputPath))
            {
                return Path.GetFullPath(options.OutputPath);
            }

            string timestamp = DateTime.UtcNow.ToString(
                "yyyyMMdd-HHmmss",
                CultureInfo.InvariantCulture);
            return Path.Combine(
                UnityApplication.persistentDataPath,
                "Performance",
                $"solar-system-performance-{timestamp}.json");
        }

        public static PerformanceCaptureDocument CreateDocument(
            IReadOnlyList<PerformanceScenarioCapture> completedScenarios,
            int expectedScenarioCount,
            PerformanceMetricSource[] metricSources,
            PerformanceCaptureOptions options,
            double interactiveSeconds,
            string requestedStatus,
            string error,
            string resultPath)
        {
            var results =
                new PerformanceScenarioResult[completedScenarios.Count];
            bool allPass = results.Length == expectedScenarioCount;
            bool hasIncomplete = results.Length != expectedScenarioCount;
            for (int index = 0; index < completedScenarios.Count; index++)
            {
                results[index] = completedScenarios[index].CreateResult(
                    metricSources,
                    UnityApplication.isEditor);
                allPass &= results[index].status == "Pass";
                hasIncomplete |= results[index].status == "Incomplete";
            }

            return new PerformanceCaptureDocument
            {
                schemaVersion = "1.0.0",
                capturedAtUtc = DateTime.UtcNow.ToString("O"),
                overallStatus = ResolveOverallStatus(
                    requestedStatus,
                    allPass,
                    hasIncomplete),
                productName = UnityApplication.productName,
                productVersion = UnityApplication.version,
                unityVersion = UnityApplication.unityVersion,
                buildGuid = UnityApplication.buildGUID,
                commitSha = options.CommitSha,
                editorDiagnostic = UnityApplication.isEditor,
                runtimeInitializationToInteractiveSeconds =
                    interactiveSeconds,
                budget = CreateBudgetRecord(),
                environment = CreateEnvironmentRecord(),
                settings = new PerformanceCaptureSettingsRecord
                {
                    warmupFrames = options.WarmupFrames,
                    warmupSeconds = options.WarmupSeconds,
                    sampleFrames = options.SampleFrames,
                    sampleSeconds = options.SampleSeconds,
                    maximumSamples = PerformanceCaptureOptions.MaximumSamples,
                    resultPath = resultPath
                },
                metricAvailability =
                    CreateAvailabilityRecords(metricSources),
                scenarios = results,
                limitations = CreateLimitations(error)
            };
        }

        private static string ResolveOverallStatus(
            string requestedStatus,
            bool allPass,
            bool hasIncomplete)
        {
            if (!string.Equals(
                requestedStatus,
                "Captured",
                StringComparison.Ordinal))
            {
                return requestedStatus;
            }

            if (UnityApplication.isEditor)
            {
                return "DiagnosticOnly";
            }

            if (hasIncomplete)
            {
                return "Incomplete";
            }

            return allPass ? "RequiresExternalValidation" : "Fail";
        }

        private static string[] CreateLimitations(string error)
        {
            if (error != null)
            {
                return new[]
                {
                    "The harness terminated before completing every scenario.",
                    error
                };
            }

            return new[]
            {
                UnityApplication.isEditor
                    ? "Editor timing, memory, and allocation results include " +
                      "Editor overhead and cannot certify the release Player."
                    : "Reference-hardware equivalence requires owner review.",
                "Dedicated application GPU-memory usage requires an external " +
                "operating-system or vendor capture.",
                "Cold process launch requires an external process-timing run.",
                "System Used Memory and Unity reserved memory are recorded as " +
                "different concepts and must not be conflated."
            };
        }

        private static PerformanceEnvironmentRecord CreateEnvironmentRecord()
        {
            Resolution resolution = Screen.currentResolution;
            RefreshRate refreshRate = resolution.refreshRateRatio;
            return new PerformanceEnvironmentRecord
            {
                operatingSystem = SystemInfo.operatingSystem,
                processorType = SystemInfo.processorType,
                processorCount = SystemInfo.processorCount,
                processorFrequencyMhz = SystemInfo.processorFrequency,
                graphicsDeviceName = SystemInfo.graphicsDeviceName,
                graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor,
                graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion,
                graphicsMemoryCapacityMb = SystemInfo.graphicsMemorySize,
                systemMemoryCapacityMb = SystemInfo.systemMemorySize,
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                refreshRateNumerator = (int)refreshRate.numerator,
                refreshRateDenominator = (int)refreshRate.denominator,
                graphicsApi = SystemInfo.graphicsDeviceType.ToString(),
                qualityLevel =
                    QualitySettings.names[QualitySettings.GetQualityLevel()],
                vSyncCountDuringCapture = 0,
                targetFrameRateDuringCapture = -1
            };
        }

        private static PerformanceBudgetRecord CreateBudgetRecord()
        {
            return new PerformanceBudgetRecord
            {
                frameTimeP95Nanoseconds =
                    SolarSystemPerformanceHarness
                        .FrameTimeP95BudgetNanoseconds,
                frameTimeP99Nanoseconds =
                    SolarSystemPerformanceHarness
                        .FrameTimeP99BudgetNanoseconds,
                cpuMainThreadP95Nanoseconds =
                    SolarSystemPerformanceHarness
                        .CpuMainThreadP95BudgetNanoseconds,
                gpuP95Nanoseconds =
                    SolarSystemPerformanceHarness.GpuP95BudgetNanoseconds,
                steadyGcAllocationP95Bytes =
                    SolarSystemPerformanceHarness
                        .GcAllocationP95BudgetBytes,
                steadyProcessMemoryBytes = SteadyMemoryBudgetBytes,
                peakProcessMemoryBytes = PeakMemoryBudgetBytes,
                dedicatedGpuMemoryBytes = GpuMemoryBudgetBytes,
                coldLaunchSeconds = ColdLaunchBudgetSeconds
            };
        }

        private static PerformanceMetricAvailabilityRecord[]
            CreateAvailabilityRecords(PerformanceMetricSource[] sources)
        {
            var records =
                new PerformanceMetricAvailabilityRecord[sources.Length];
            for (int index = 0; index < sources.Length; index++)
            {
                PerformanceMetricSource source = sources[index];
                records[index] = new PerformanceMetricAvailabilityRecord
                {
                    id = source.Id,
                    source = source.Source,
                    unit = source.Unit,
                    available = source.IsAvailable
                };
            }

            return records;
        }
    }
}
