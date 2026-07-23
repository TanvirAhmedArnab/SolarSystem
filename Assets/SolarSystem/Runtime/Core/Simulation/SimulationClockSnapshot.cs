namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Provides an immutable view of authoritative simulation-clock state.</summary>
    public readonly struct SimulationClockSnapshot
    {
        /// <summary>Initializes a clock snapshot.</summary>
        public SimulationClockSnapshot(
            double elapsedSimulationTimeSeconds,
            bool isPaused,
            double speedMultiplier)
        {
            ElapsedSimulationTimeSeconds = elapsedSimulationTimeSeconds;
            IsPaused = isPaused;
            SpeedMultiplier = speedMultiplier;
        }

        /// <summary>Gets authoritative elapsed simulation time in seconds.</summary>
        public double ElapsedSimulationTimeSeconds { get; }
        /// <summary>Gets whether simulation advancement is paused.</summary>
        public bool IsPaused { get; }
        /// <summary>Gets the positive simulation-time multiplier.</summary>
        public double SpeedMultiplier { get; }
    }
}
