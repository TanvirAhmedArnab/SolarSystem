using System;
using Unity.Profiling;
using UnityEngine;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    internal abstract class PerformanceMetricSource : IDisposable
    {
        protected PerformanceMetricSource(
            string id,
            string source,
            string unit,
            bool hasPercentile95Budget,
            long percentile95Budget,
            bool hasPercentile99Budget,
            long percentile99Budget)
        {
            Id = id;
            Source = source;
            Unit = unit;
            HasPercentile95Budget = hasPercentile95Budget;
            Percentile95Budget = percentile95Budget;
            HasPercentile99Budget = hasPercentile99Budget;
            Percentile99Budget = percentile99Budget;
        }

        public string Id { get; }
        public string Source { get; protected set; }
        public string Unit { get; protected set; }
        public bool HasPercentile95Budget { get; }
        public long Percentile95Budget { get; }
        public bool HasPercentile99Budget { get; }
        public long Percentile99Budget { get; }
        public abstract bool IsAvailable { get; }
        public abstract bool TryRead(out long value);
        public abstract void Dispose();
    }

    internal sealed class FrameTimeMetricSource : PerformanceMetricSource
    {
        public FrameTimeMetricSource()
            : base(
                "totalFrameTime",
                "UnityEngine.Time.unscaledDeltaTime",
                "Nanoseconds",
                true,
                SolarSystemPerformanceHarness.FrameTimeP95BudgetNanoseconds,
                true,
                SolarSystemPerformanceHarness.FrameTimeP99BudgetNanoseconds)
        {
        }

        public override bool IsAvailable => true;

        public override bool TryRead(out long value)
        {
            value = (long)Math.Round(
                Time.unscaledDeltaTime * 1_000_000_000d,
                MidpointRounding.AwayFromZero);
            return true;
        }

        public override void Dispose()
        {
        }
    }

    internal readonly struct ProfilerMetricCandidate
    {
        public ProfilerMetricCandidate(
            ProfilerCategory category,
            string categoryName,
            string counterName)
        {
            Category = category;
            CategoryName = categoryName;
            CounterName = counterName;
        }

        public ProfilerCategory Category { get; }
        public string CategoryName { get; }
        public string CounterName { get; }
    }

    internal sealed class ProfilerMetricSource : PerformanceMetricSource
    {
        private ProfilerRecorder recorder;

        public ProfilerMetricSource(
            string id,
            bool hasPercentile95Budget,
            long percentile95Budget,
            bool hasPercentile99Budget,
            long percentile99Budget,
            params ProfilerMetricCandidate[] candidates)
            : base(
                id,
                "Unavailable",
                "Unavailable",
                hasPercentile95Budget,
                percentile95Budget,
                hasPercentile99Budget,
                percentile99Budget)
        {
            if (candidates == null || candidates.Length == 0)
            {
                throw new ArgumentException(
                    "At least one Profiler counter candidate is required.",
                    nameof(candidates));
            }

            for (int index = 0; index < candidates.Length; index++)
            {
                ProfilerMetricCandidate candidate = candidates[index];
                ProfilerRecorder attempt = default;
                try
                {
                    attempt = ProfilerRecorder.StartNew(
                        candidate.Category,
                        candidate.CounterName,
                        1);
                    if (!attempt.Valid)
                    {
                        attempt.Dispose();
                        continue;
                    }

                    recorder = attempt;
                    Source =
                        $"{candidate.CategoryName}/{candidate.CounterName}";
                    Unit = recorder.UnitType.ToString();
                    return;
                }
                catch (ArgumentException)
                {
                    attempt.Dispose();
                }
            }
        }

        public override bool IsAvailable => recorder.Valid;

        public override bool TryRead(out long value)
        {
            if (!recorder.Valid || recorder.Count == 0)
            {
                value = long.MinValue;
                return false;
            }

            value = recorder.LastValue;
            return true;
        }

        public override void Dispose()
        {
            recorder.Dispose();
            recorder = default;
        }
    }
}
