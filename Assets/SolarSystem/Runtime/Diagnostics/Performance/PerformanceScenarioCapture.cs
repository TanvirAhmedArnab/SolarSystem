using System;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    internal readonly struct PerformanceScenarioDescriptor
    {
        public PerformanceScenarioDescriptor(
            string id,
            string name,
            PerformanceScenarioKind kind,
            int ordinal)
        {
            Id = id;
            Name = name;
            Kind = kind;
            Ordinal = ordinal;
        }

        public string Id { get; }
        public string Name { get; }
        public PerformanceScenarioKind Kind { get; }
        public int Ordinal { get; }
    }

    internal enum PerformanceScenarioKind
    {
        Overview = 0,
        EarthFocus = 1,
        CreditsMenu = 2,
        ScaleComparison = 3,
        CinematicChapter = 4
    }

    internal sealed class PerformanceScenarioCapture
    {
        private readonly long[][] samples;
        private readonly string[] observations;
        private int observationCount;

        public PerformanceScenarioCapture(
            PerformanceScenarioDescriptor descriptor,
            PerformanceMetricSource[] sources,
            int capacity)
        {
            Descriptor = descriptor;
            if (sources == null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            samples = new long[sources.Length][];
            for (int index = 0; index < sources.Length; index++)
            {
                samples[index] = new long[capacity];
            }

            observations = new string[4];
            Capacity = capacity;
        }

        public PerformanceScenarioDescriptor Descriptor { get; }
        public int Capacity { get; }
        public int Count { get; private set; }
        public float ElapsedSeconds { get; private set; }

        public bool Capture(
            PerformanceMetricSource[] sources,
            float unscaledDeltaTime)
        {
            if (sources == null || sources.Length != samples.Length)
            {
                throw new ArgumentException(
                    "Scenario capture requires its original metric set.",
                    nameof(sources));
            }

            if (Count >= Capacity)
            {
                AddObservation(
                    "The sample buffer reached capacity before both requested " +
                    "sampling minima completed.");
                return false;
            }

            for (int index = 0; index < sources.Length; index++)
            {
                samples[index][Count] = sources[index].TryRead(out long value)
                    ? value
                    : long.MinValue;
            }

            Count++;
            ElapsedSeconds += unscaledDeltaTime;
            return true;
        }

        public PerformanceScenarioResult CreateResult(
            PerformanceMetricSource[] sources,
            bool editorDiagnostic)
        {
            var metricResults = new PerformanceMetricResult[sources.Length];
            bool hasIncompleteGate = false;
            bool passesBudgets = true;

            for (int sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                PerformanceMetricSource source = sources[sourceIndex];
                PerformanceStatisticsResult statistics =
                    PerformanceStatistics.Calculate(
                        samples[sourceIndex],
                        Count);
                bool gated =
                    source.HasPercentile95Budget ||
                    source.HasPercentile99Budget;
                if (gated && statistics.SampleCount == 0)
                {
                    hasIncompleteGate = true;
                }

                if (source.HasPercentile95Budget &&
                    statistics.SampleCount > 0 &&
                    statistics.Percentile95 > source.Percentile95Budget)
                {
                    passesBudgets = false;
                }

                if (source.HasPercentile99Budget &&
                    statistics.SampleCount > 0 &&
                    statistics.Percentile99 > source.Percentile99Budget)
                {
                    passesBudgets = false;
                }

                var trimmedSamples = new long[Count];
                Array.Copy(
                    samples[sourceIndex],
                    trimmedSamples,
                    Count);
                metricResults[sourceIndex] = new PerformanceMetricResult
                {
                    id = source.Id,
                    source = source.Source,
                    unit = source.Unit,
                    available =
                        source.IsAvailable && statistics.SampleCount > 0,
                    budget = source.HasPercentile95Budget
                        ? source.Percentile95Budget
                        : 0L,
                    statistics = PerformanceStatisticsRecord.From(statistics),
                    rawSamples = trimmedSamples
                };
            }

            string status;
            if (editorDiagnostic)
            {
                status = "DiagnosticOnly";
            }
            else if (hasIncompleteGate)
            {
                status = "Incomplete";
            }
            else
            {
                status = passesBudgets ? "Pass" : "Fail";
            }

            var resultObservations = new string[observationCount];
            Array.Copy(observations, resultObservations, observationCount);
            return new PerformanceScenarioResult
            {
                id = Descriptor.Id,
                name = Descriptor.Name,
                status = status,
                sampleCount = Count,
                sampledSeconds = ElapsedSeconds,
                metrics = metricResults,
                observations = resultObservations
            };
        }

        private void AddObservation(string observation)
        {
            if (observationCount < observations.Length)
            {
                observations[observationCount++] = observation;
            }
        }
    }
}
