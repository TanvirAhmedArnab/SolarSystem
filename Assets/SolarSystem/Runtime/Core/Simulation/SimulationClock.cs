using System;

namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Owns authoritative simulation time, pause state, and speed multiplier.</summary>
    public sealed class SimulationClock
    {
        private double elapsedSimulationTimeSeconds;
        private bool isPaused;
        private double speedMultiplier;

        /// <summary>Initializes a simulation clock.</summary>
        public SimulationClock(double initialTimeSeconds = 0d, double initialSpeedMultiplier = 1d)
        {
            RequireFinite(initialTimeSeconds, nameof(initialTimeSeconds));
            RequirePositiveFinite(initialSpeedMultiplier, nameof(initialSpeedMultiplier));
            elapsedSimulationTimeSeconds = initialTimeSeconds;
            speedMultiplier = initialSpeedMultiplier;
        }

        /// <summary>Raised when pause or speed settings change, but not on ordinary time advancement.</summary>
        public event Action Changed;

        /// <summary>Gets an immutable snapshot of the clock state.</summary>
        public SimulationClockSnapshot Snapshot =>
            new SimulationClockSnapshot(elapsedSimulationTimeSeconds, isPaused, speedMultiplier);

        /// <summary>Advances authoritative time using an unscaled real-time delta.</summary>
        public void Advance(double unscaledDeltaTimeSeconds)
        {
            RequireFinite(unscaledDeltaTimeSeconds, nameof(unscaledDeltaTimeSeconds));
            if (unscaledDeltaTimeSeconds < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unscaledDeltaTimeSeconds),
                    unscaledDeltaTimeSeconds,
                    "Clock advancement cannot be negative.");
            }

            if (isPaused || unscaledDeltaTimeSeconds == 0d)
            {
                return;
            }

            double nextTime = elapsedSimulationTimeSeconds + (unscaledDeltaTimeSeconds * speedMultiplier);
            if (double.IsNaN(nextTime) || double.IsInfinity(nextTime))
            {
                throw new InvalidOperationException("Simulation time overflowed its finite range.");
            }

            elapsedSimulationTimeSeconds = nextTime;
        }

        /// <summary>Sets whether simulation-time advancement is paused.</summary>
        public void SetPaused(bool paused)
        {
            if (isPaused == paused)
            {
                return;
            }

            isPaused = paused;
            Changed?.Invoke();
        }

        /// <summary>Sets the positive simulation-time multiplier.</summary>
        public void SetSpeedMultiplier(double multiplier)
        {
            RequirePositiveFinite(multiplier, nameof(multiplier));
            if (speedMultiplier.Equals(multiplier))
            {
                return;
            }

            speedMultiplier = multiplier;
            Changed?.Invoke();
        }

        private static void RequirePositiveFinite(double value, string parameterName)
        {
            RequireFinite(value, parameterName);
            if (value <= 0d)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, "Value must be positive.");
            }
        }

        private static void RequireFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
            }
        }
    }
}
