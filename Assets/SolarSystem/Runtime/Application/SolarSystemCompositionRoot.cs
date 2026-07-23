using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Validates serialized references and constructs the explicit Solar System runtime graph.</summary>
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class SolarSystemCompositionRoot : MonoBehaviour
    {
        [Header("Authoring data")]
        [SerializeField] private CelestialCatalogDefinition catalogDefinition;
        [SerializeField] private PresentationScaleDefinition scaleDefinition;

        [Header("Runtime presentation")]
        [SerializeField] private SolarSystemSimulationController simulationController;
        [SerializeField] private CelestialBodyView[] bodyViews = Array.Empty<CelestialBodyView>();
        [SerializeField] private CelestialOrbitPathView[] orbitPaths =
            Array.Empty<CelestialOrbitPathView>();

        [Header("Graybox simulation")]
        [SerializeField] private double initialSimulationTimeSeconds;
        [SerializeField] private double simulationSecondsPerRealSecond = 604800d;
        [SerializeField] private bool beginPaused;

        /// <summary>Gets the centralized runtime controller after bootstrap.</summary>
        public SolarSystemSimulationController SimulationController => simulationController;

        /// <summary>Gets whether the complete runtime graph initialized successfully.</summary>
        public bool IsInitialized =>
            simulationController != null && simulationController.IsInitialized;

        private void Awake()
        {
            RebuildRuntimeGraph();
        }

        /// <summary>Rebuilds the runtime graph and renders its initial state for Play Mode or authoring preview.</summary>
        [ContextMenu("Rebuild Runtime Graph")]
        public void RebuildRuntimeGraph()
        {
            if (catalogDefinition == null)
            {
                throw new InvalidOperationException("Composition root requires a catalog definition.");
            }

            if (scaleDefinition == null)
            {
                throw new InvalidOperationException("Composition root requires a scale definition.");
            }

            if (simulationController == null)
            {
                throw new InvalidOperationException("Composition root requires a simulation controller.");
            }

            CelestialCatalog catalog = catalogDefinition.BuildCatalog();
            var evaluator = new KeplerOrbitEvaluator();
            var clock = new SimulationClock(
                initialSimulationTimeSeconds,
                simulationSecondsPerRealSecond);
            clock.SetPaused(beginPaused);
            var projector = new CelestialScaleProjector(scaleDefinition.ToParameters());

            simulationController.Initialize(
                catalog,
                evaluator,
                clock,
                projector,
                bodyViews,
                orbitPaths);
        }
    }
}
