using System;
using NUnit.Framework;
using Tanvir.SolarSystem.Diagnostics.Performance;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class PerformanceStatisticsTests
    {
        [Test]
        public void Calculate_UnorderedSamples_UsesNearestRankPercentiles()
        {
            long[] samples = { 10L, 1L, 8L, 3L, 5L, 7L, 2L, 9L, 4L, 6L };

            PerformanceStatisticsResult result =
                PerformanceStatistics.Calculate(samples, samples.Length);

            Assert.That(result.SampleCount, Is.EqualTo(10));
            Assert.That(result.Median, Is.EqualTo(5L));
            Assert.That(result.Percentile95, Is.EqualTo(10L));
            Assert.That(result.Percentile99, Is.EqualTo(10L));
            Assert.That(result.Maximum, Is.EqualTo(10L));
            Assert.That(result.NonZeroCount, Is.EqualTo(10));
        }

        [Test]
        public void Calculate_UnavailableSentinels_ExcludesThem()
        {
            long[] samples =
            {
                long.MinValue,
                0L,
                4L,
                long.MinValue,
                2L,
                0L
            };

            PerformanceStatisticsResult result =
                PerformanceStatistics.Calculate(samples, samples.Length);

            Assert.That(result.SampleCount, Is.EqualTo(4));
            Assert.That(result.Median, Is.EqualTo(0L));
            Assert.That(result.Percentile95, Is.EqualTo(4L));
            Assert.That(result.NonZeroCount, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_NoAvailableSamples_ReturnsExplicitEmptySummary()
        {
            PerformanceStatisticsResult result =
                PerformanceStatistics.Calculate(
                    new[] { long.MinValue, long.MinValue },
                    2);

            Assert.That(result.SampleCount, Is.Zero);
            Assert.That(result.Median, Is.Zero);
            Assert.That(result.Percentile95, Is.Zero);
            Assert.That(result.Percentile99, Is.Zero);
            Assert.That(result.Maximum, Is.Zero);
            Assert.That(result.NonZeroCount, Is.Zero);
        }

        [Test]
        public void Calculate_UsesOnlyRequestedPrefix()
        {
            PerformanceStatisticsResult result =
                PerformanceStatistics.Calculate(
                    new[] { 1L, 2L, 3L, 1000L },
                    3);

            Assert.That(result.Maximum, Is.EqualTo(3L));
        }

        [Test]
        public void Calculate_InvalidCount_RejectsInput()
        {
            Assert.That(
                () => PerformanceStatistics.Calculate(
                    Array.Empty<long>(),
                    1),
                Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => PerformanceStatistics.Calculate(
                    Array.Empty<long>(),
                    -1),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ApprovedTimingBudgets_AreMillisecondsExpressedAsNanoseconds()
        {
            Assert.That(
                SolarSystemPerformanceHarness
                    .FrameTimeP95BudgetNanoseconds,
                Is.EqualTo(16_670_000L));
            Assert.That(
                SolarSystemPerformanceHarness
                    .FrameTimeP99BudgetNanoseconds,
                Is.EqualTo(25_000_000L));
            Assert.That(
                SolarSystemPerformanceHarness
                    .CpuMainThreadP95BudgetNanoseconds,
                Is.EqualTo(13_330_000L));
            Assert.That(
                SolarSystemPerformanceHarness
                    .GpuP95BudgetNanoseconds,
                Is.EqualTo(13_330_000L));
            Assert.That(
                SolarSystemPerformanceHarness
                    .GcAllocationP95BudgetBytes,
                Is.Zero);
        }
    }
}
