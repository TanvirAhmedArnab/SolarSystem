using System;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    [Serializable]
    internal sealed class PerformanceCaptureDocument
    {
        public string schemaVersion;
        public string capturedAtUtc;
        public string overallStatus;
        public string productName;
        public string productVersion;
        public string unityVersion;
        public string buildGuid;
        public string commitSha;
        public bool editorDiagnostic;
        public double runtimeInitializationToInteractiveSeconds;
        public PerformanceBudgetRecord budget;
        public PerformanceEnvironmentRecord environment;
        public PerformanceCaptureSettingsRecord settings;
        public PerformanceMetricAvailabilityRecord[] metricAvailability;
        public PerformanceScenarioResult[] scenarios;
        public string[] limitations;
    }

    [Serializable]
    internal sealed class PerformanceBudgetRecord
    {
        public long frameTimeP95Nanoseconds;
        public long frameTimeP99Nanoseconds;
        public long cpuMainThreadP95Nanoseconds;
        public long gpuP95Nanoseconds;
        public long steadyGcAllocationP95Bytes;
        public long steadyProcessMemoryBytes;
        public long peakProcessMemoryBytes;
        public long dedicatedGpuMemoryBytes;
        public double coldLaunchSeconds;
    }

    [Serializable]
    internal sealed class PerformanceEnvironmentRecord
    {
        public string operatingSystem;
        public string processorType;
        public int processorCount;
        public int processorFrequencyMhz;
        public string graphicsDeviceName;
        public string graphicsDeviceVendor;
        public string graphicsDeviceVersion;
        public int graphicsMemoryCapacityMb;
        public int systemMemoryCapacityMb;
        public int screenWidth;
        public int screenHeight;
        public int refreshRateNumerator;
        public int refreshRateDenominator;
        public string graphicsApi;
        public string qualityLevel;
        public int vSyncCountDuringCapture;
        public int targetFrameRateDuringCapture;
    }

    [Serializable]
    internal sealed class PerformanceCaptureSettingsRecord
    {
        public int warmupFrames;
        public float warmupSeconds;
        public int sampleFrames;
        public float sampleSeconds;
        public int maximumSamples;
        public string resultPath;
    }

    [Serializable]
    internal sealed class PerformanceMetricAvailabilityRecord
    {
        public string id;
        public string source;
        public string unit;
        public bool available;
    }

    [Serializable]
    internal sealed class PerformanceScenarioResult
    {
        public string id;
        public string name;
        public string status;
        public int sampleCount;
        public float sampledSeconds;
        public PerformanceMetricResult[] metrics;
        public string[] observations;
    }

    [Serializable]
    internal sealed class PerformanceMetricResult
    {
        public string id;
        public string source;
        public string unit;
        public bool available;
        public long budget;
        public PerformanceStatisticsRecord statistics;
        public long[] rawSamples;
    }

    [Serializable]
    internal sealed class PerformanceStatisticsRecord
    {
        public int sampleCount;
        public long median;
        public long percentile95;
        public long percentile99;
        public long maximum;
        public int nonZeroCount;

        public static PerformanceStatisticsRecord From(
            PerformanceStatisticsResult result)
        {
            return new PerformanceStatisticsRecord
            {
                sampleCount = result.SampleCount,
                median = result.Median,
                percentile95 = result.Percentile95,
                percentile99 = result.Percentile99,
                maximum = result.Maximum,
                nonZeroCount = result.NonZeroCount
            };
        }
    }
}
