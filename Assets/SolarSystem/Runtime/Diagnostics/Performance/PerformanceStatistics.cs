using System;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    /// <summary>Immutable nearest-rank statistics for one sampled metric.</summary>
    public readonly struct PerformanceStatisticsResult
    {
        /// <summary>Initializes a complete metric summary.</summary>
        public PerformanceStatisticsResult(
            int sampleCount,
            long median,
            long percentile95,
            long percentile99,
            long maximum,
            int nonZeroCount)
        {
            SampleCount = sampleCount;
            Median = median;
            Percentile95 = percentile95;
            Percentile99 = percentile99;
            Maximum = maximum;
            NonZeroCount = nonZeroCount;
        }

        /// <summary>Gets the number of valid samples.</summary>
        public int SampleCount { get; }

        /// <summary>Gets the nearest-rank 50th percentile.</summary>
        public long Median { get; }

        /// <summary>Gets the nearest-rank 95th percentile.</summary>
        public long Percentile95 { get; }

        /// <summary>Gets the nearest-rank 99th percentile.</summary>
        public long Percentile99 { get; }

        /// <summary>Gets the largest sample.</summary>
        public long Maximum { get; }

        /// <summary>Gets how many samples were non-zero.</summary>
        public int NonZeroCount { get; }
    }

    /// <summary>Calculates deterministic percentile evidence without LINQ.</summary>
    public static class PerformanceStatistics
    {
        /// <summary>
        /// Calculates a summary from the first <paramref name="sampleCount"/>
        /// entries. Values equal to <see cref="long.MinValue"/> are unavailable
        /// samples and are excluded.
        /// </summary>
        public static PerformanceStatisticsResult Calculate(
            long[] samples,
            int sampleCount)
        {
            if (samples == null)
            {
                throw new ArgumentNullException(nameof(samples));
            }

            if (sampleCount < 0 || sampleCount > samples.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleCount));
            }

            int validCount = 0;
            int nonZeroCount = 0;
            for (int index = 0; index < sampleCount; index++)
            {
                long value = samples[index];
                if (value == long.MinValue)
                {
                    continue;
                }

                validCount++;
                if (value != 0L)
                {
                    nonZeroCount++;
                }
            }

            if (validCount == 0)
            {
                return new PerformanceStatisticsResult(0, 0L, 0L, 0L, 0L, 0);
            }

            var ordered = new long[validCount];
            int targetIndex = 0;
            for (int index = 0; index < sampleCount; index++)
            {
                long value = samples[index];
                if (value != long.MinValue)
                {
                    ordered[targetIndex++] = value;
                }
            }

            Array.Sort(ordered);
            return new PerformanceStatisticsResult(
                validCount,
                Percentile(ordered, 0.50d),
                Percentile(ordered, 0.95d),
                Percentile(ordered, 0.99d),
                ordered[ordered.Length - 1],
                nonZeroCount);
        }

        private static long Percentile(long[] ordered, double percentile)
        {
            int rank = (int)Math.Ceiling(percentile * ordered.Length);
            int index = Math.Clamp(rank - 1, 0, ordered.Length - 1);
            return ordered[index];
        }
    }
}
