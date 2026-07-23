using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Exposes authoritative clock state and settings commands to application services.</summary>
    public interface ISimulationTimeController
    {
        /// <summary>Gets the current immutable simulation-clock snapshot.</summary>
        SimulationClockSnapshot ClockSnapshot { get; }

        /// <summary>Sets whether authoritative simulation advancement is paused.</summary>
        void SetPaused(bool paused);

        /// <summary>Sets simulated seconds advanced per real second.</summary>
        void SetSpeedMultiplier(double simulationSecondsPerRealSecond);
    }
}
