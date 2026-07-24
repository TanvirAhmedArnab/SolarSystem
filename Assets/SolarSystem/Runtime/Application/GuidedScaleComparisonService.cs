using System;
using Tanvir.SolarSystem.Presentation.Scale;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>
    /// Owns deterministic guided-comparison state while preserving simulation
    /// pause state and keeping physical data untouched.
    /// </summary>
    public sealed class GuidedScaleComparisonService
    {
        private readonly IScaleModeController scaleController;
        private readonly SimulationTimeControlService timeControls;
        private bool wasPausedBeforeComparison;

        /// <summary>Initializes the guided comparison against explicit application services.</summary>
        public GuidedScaleComparisonService(
            IScaleModeController presentationScaleController,
            SimulationTimeControlService simulationTimeControls)
        {
            scaleController = presentationScaleController ??
                throw new ArgumentNullException(nameof(presentationScaleController));
            timeControls = simulationTimeControls ??
                throw new ArgumentNullException(nameof(simulationTimeControls));
            Stage = GuidedScaleComparisonStage.Inactive;
        }

        /// <summary>Raised after an effective stage transition or exit.</summary>
        public event Action Changed;

        /// <summary>Gets the active guided-comparison stage.</summary>
        public GuidedScaleComparisonStage Stage { get; private set; }

        /// <summary>Gets whether the controlled comparison currently owns presentation.</summary>
        public bool IsActive => Stage != GuidedScaleComparisonStage.Inactive;

        /// <summary>Gets the one-based educational step, or zero while inactive.</summary>
        public int CurrentStep => Stage switch
        {
            GuidedScaleComparisonStage.ReadableOverview => 1,
            GuidedScaleComparisonStage.NormalizedOrbits => 2,
            GuidedScaleComparisonStage.LiteralEarthReference => 3,
            _ => 0
        };

        /// <summary>Gets the fixed number of comparison steps.</summary>
        public int StepCount => 3;

        /// <summary>Starts, advances, finishes, or restarts the deterministic sequence.</summary>
        public void Advance()
        {
            switch (Stage)
            {
                case GuidedScaleComparisonStage.Inactive:
                    wasPausedBeforeComparison = timeControls.IsPaused;
                    Stage = GuidedScaleComparisonStage.ReadableOverview;
                    scaleController.SetScaleMode(CelestialScaleMode.ReadableOverview);
                    timeControls.SetPaused(true);
                    Changed?.Invoke();
                    break;
                case GuidedScaleComparisonStage.ReadableOverview:
                    SetStage(
                        GuidedScaleComparisonStage.NormalizedOrbits,
                        CelestialScaleMode.NormalizedOrbits);
                    break;
                case GuidedScaleComparisonStage.NormalizedOrbits:
                    SetStage(
                        GuidedScaleComparisonStage.LiteralEarthReference,
                        CelestialScaleMode.LiteralEarthReference);
                    break;
                case GuidedScaleComparisonStage.LiteralEarthReference:
                    Exit();
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported comparison stage '{Stage}'.");
            }
        }

        /// <summary>Returns immediately to the readable overview when active.</summary>
        public bool Cancel()
        {
            if (!IsActive)
            {
                return false;
            }

            Exit();
            return true;
        }

        private void SetStage(
            GuidedScaleComparisonStage stage,
            CelestialScaleMode scaleMode)
        {
            scaleController.SetScaleMode(scaleMode);
            Stage = stage;
            Changed?.Invoke();
        }

        private void Exit()
        {
            scaleController.SetScaleMode(CelestialScaleMode.ReadableOverview);
            timeControls.SetPaused(wasPausedBeforeComparison);
            Stage = GuidedScaleComparisonStage.Inactive;
            Changed?.Invoke();
        }
    }
}
