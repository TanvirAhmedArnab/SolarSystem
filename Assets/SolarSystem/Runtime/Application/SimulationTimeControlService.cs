using System;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Owns the bounded, user-facing simulation-time control policy.</summary>
    public sealed class SimulationTimeControlService
    {
        /// <summary>
        /// Defines 1x as one verified Earth sidereal rotation per real second.
        /// </summary>
        public const double BaselineSecondsPerRealSecond =
            CelestialReferenceUnits.EarthSiderealRotationPeriodSeconds;

        private static readonly int[] PresetMultipliers = { 1, 10, 100, 1000, 10000 };
        private readonly ISimulationTimeController controller;
        private int currentPresetIndex;

        /// <summary>Initializes controls from an authoritative controller at a supported preset.</summary>
        public SimulationTimeControlService(ISimulationTimeController timeController)
        {
            controller = timeController ??
                throw new ArgumentNullException(nameof(timeController));
            currentPresetIndex = FindPresetIndex(controller.ClockSnapshot.SpeedMultiplier);
        }

        /// <summary>Raised after an effective pause or speed command.</summary>
        public event Action Changed;

        /// <summary>Gets a read-only snapshot for presentation.</summary>
        public SimulationClockSnapshot Snapshot => controller.ClockSnapshot;

        /// <summary>Gets whether the simulation is paused.</summary>
        public bool IsPaused => Snapshot.IsPaused;

        /// <summary>Gets the current user-facing multiplier.</summary>
        public int CurrentMultiplier => PresetMultipliers[currentPresetIndex];

        /// <summary>Gets the current simulated seconds advanced per real second.</summary>
        public double SimulationSecondsPerRealSecond => Snapshot.SpeedMultiplier;

        /// <summary>Gets the number of supported rate presets.</summary>
        public int PresetCount => PresetMultipliers.Length;

        /// <summary>Toggles pause while preserving the current speed preset.</summary>
        public void TogglePaused()
        {
            controller.SetPaused(!IsPaused);
            Changed?.Invoke();
        }

        /// <summary>Moves to the next slower preset without wrapping.</summary>
        public bool DecreaseSpeed()
        {
            return SetPresetIndex(Math.Max(0, currentPresetIndex - 1));
        }

        /// <summary>Moves to the next faster preset without wrapping.</summary>
        public bool IncreaseSpeed()
        {
            return SetPresetIndex(
                Math.Min(PresetMultipliers.Length - 1, currentPresetIndex + 1));
        }

        /// <summary>Applies a preset by zero-based index.</summary>
        public bool SetPresetIndex(int presetIndex)
        {
            if (presetIndex < 0 || presetIndex >= PresetMultipliers.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(presetIndex));
            }

            if (presetIndex == currentPresetIndex)
            {
                return false;
            }

            currentPresetIndex = presetIndex;
            controller.SetSpeedMultiplier(
                BaselineSecondsPerRealSecond * PresetMultipliers[currentPresetIndex]);
            Changed?.Invoke();
            return true;
        }

        private static int FindPresetIndex(double speed)
        {
            for (int index = 0; index < PresetMultipliers.Length; index++)
            {
                double preset = BaselineSecondsPerRealSecond * PresetMultipliers[index];
                if (Math.Abs(speed - preset) <= 0.000001d)
                {
                    return index;
                }
            }

            throw new InvalidOperationException(
                $"Initial simulation rate '{speed}' is not a supported time-control preset.");
        }
    }
}
