using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Owns the single ordered per-frame simulation and presentation update.</summary>
    [DisallowMultipleComponent]
    public sealed class SolarSystemSimulationController :
        MonoBehaviour,
        ISimulationTimeController,
        IScaleModeController
    {
        private readonly Dictionary<CelestialBodyId, int> bodyIndices =
            new Dictionary<CelestialBodyId, int>();
        private readonly Dictionary<CelestialBodyId, CelestialBodyView> views =
            new Dictionary<CelestialBodyId, CelestialBodyView>();

        private CelestialCatalog catalog;
        private KeplerOrbitEvaluator evaluator;
        private SimulationClock clock;
        private CelestialScaleProjector projector;
        private CelestialState[] physicalStates;
        private CelestialPresentationState[] presentationStates;
        private CelestialOrbitPathView[] orbitPaths;

        /// <summary>Gets whether dependencies and views have been validated and initialized.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Gets the number of bodies in the active catalog.</summary>
        public int CatalogCount => catalog == null ? 0 : catalog.Count;

        /// <summary>Gets the number of explicitly registered views.</summary>
        public int ViewCount => views.Count;

        /// <summary>Gets the active presentation scale.</summary>
        public CelestialScaleMode ScaleMode =>
            projector?.CurrentMode ?? CelestialScaleMode.ReadableOverview;

        /// <summary>Gets the current immutable clock snapshot.</summary>
        public SimulationClockSnapshot ClockSnapshot
        {
            get
            {
                if (clock == null)
                {
                    throw new InvalidOperationException("Simulation controller is not initialized.");
                }

                return clock.Snapshot;
            }
        }

        /// <summary>Constructs the runtime update graph from explicit dependencies.</summary>
        public void Initialize(
            CelestialCatalog runtimeCatalog,
            KeplerOrbitEvaluator runtimeEvaluator,
            SimulationClock runtimeClock,
            CelestialScaleProjector runtimeProjector,
            CelestialBodyView[] bodyViews,
            CelestialOrbitPathView[] bodyOrbitPaths)
        {
            catalog = runtimeCatalog ?? throw new ArgumentNullException(nameof(runtimeCatalog));
            evaluator = runtimeEvaluator ?? throw new ArgumentNullException(nameof(runtimeEvaluator));
            clock = runtimeClock ?? throw new ArgumentNullException(nameof(runtimeClock));
            projector = runtimeProjector ?? throw new ArgumentNullException(nameof(runtimeProjector));
            orbitPaths = bodyOrbitPaths ?? Array.Empty<CelestialOrbitPathView>();

            if (bodyViews == null)
            {
                throw new ArgumentNullException(nameof(bodyViews));
            }

            bodyIndices.Clear();
            views.Clear();
            physicalStates = new CelestialState[catalog.Count];
            presentationStates = new CelestialPresentationState[catalog.Count];

            for (int index = 0; index < catalog.Count; index++)
            {
                CelestialBodyModel body = catalog.OrderedBodies[index];
                bodyIndices.Add(body.Id, index);
            }

            foreach (CelestialBodyView view in bodyViews)
            {
                if (view == null)
                {
                    throw new InvalidOperationException("Registered body views cannot contain null entries.");
                }

                var id = new CelestialBodyId(view.StableId);
                CelestialBodyModel model = catalog.GetBody(id);
                view.Initialize(model);
                if (!views.TryAdd(id, view))
                {
                    throw new InvalidOperationException($"Multiple views are registered for '{id}'.");
                }
            }

            if (views.Count != catalog.Count)
            {
                throw new InvalidOperationException(
                    $"Catalog contains {catalog.Count} bodies but {views.Count} views are registered.");
            }

            foreach (CelestialOrbitPathView orbitPath in orbitPaths)
            {
                if (orbitPath == null)
                {
                    throw new InvalidOperationException("Registered orbit paths cannot contain null entries.");
                }

                CelestialBodyModel model = catalog.GetBody(new CelestialBodyId(orbitPath.StableId));
                orbitPath.Initialize(model, evaluator, projector);
            }

            IsInitialized = true;
            RenderCurrentState();
        }

        /// <summary>Sets the simulation pause state.</summary>
        public void SetPaused(bool paused)
        {
            RequireInitialized();
            clock.SetPaused(paused);
        }

        /// <summary>Sets simulated seconds advanced per real second.</summary>
        public void SetSpeedMultiplier(double simulationSecondsPerRealSecond)
        {
            RequireInitialized();
            clock.SetSpeedMultiplier(simulationSecondsPerRealSecond);
        }

        /// <summary>Changes only presentation projection and rerenders current state.</summary>
        public void SetScaleMode(CelestialScaleMode mode)
        {
            RequireInitialized();
            if (projector.CurrentMode == mode)
            {
                return;
            }

            projector.SetMode(mode);
            foreach (CelestialOrbitPathView orbitPath in orbitPaths)
            {
                CelestialBodyModel model =
                    catalog.GetBody(new CelestialBodyId(orbitPath.StableId));
                orbitPath.Initialize(model, evaluator, projector);
            }

            RenderCurrentState();
        }

        /// <summary>Gets the largest body-or-position extent from the render origin.</summary>
        public float GetMaximumPresentationExtent()
        {
            RequireInitialized();
            float maximum = 0f;
            foreach (CelestialBodyView view in views.Values)
            {
                maximum = Mathf.Max(
                    maximum,
                    view.transform.position.magnitude + view.CurrentDisplayRadius);
            }

            return maximum;
        }

        /// <summary>Attempts to obtain a registered body view by stable ID.</summary>
        public bool TryGetView(string stableId, out CelestialBodyView view)
        {
            return views.TryGetValue(new CelestialBodyId(stableId), out view);
        }

        /// <summary>Evaluates and renders the current authoritative time without advancing it.</summary>
        public void RenderCurrentState()
        {
            RequireInitialized();
            double timeSeconds = clock.Snapshot.ElapsedSimulationTimeSeconds;
            evaluator.Evaluate(catalog, timeSeconds, physicalStates);
            projector.Project(catalog, physicalStates, presentationStates);

            for (int index = 0; index < catalog.Count; index++)
            {
                CelestialPresentationState state = presentationStates[index];
                views[state.Id].Apply(state, timeSeconds);
            }

            foreach (CelestialOrbitPathView orbitPath in orbitPaths)
            {
                CelestialBodyModel body = catalog.GetBody(new CelestialBodyId(orbitPath.StableId));
                Vector3 parentPosition = Vector3.zero;
                if (body.ParentId.HasValue)
                {
                    parentPosition = presentationStates[bodyIndices[body.ParentId.Value]].Position;
                }

                orbitPath.ApplyParentPosition(parentPosition);
            }
        }

        private void Update()
        {
            if (!IsInitialized)
            {
                return;
            }

            clock.Advance(Time.unscaledDeltaTime);
            RenderCurrentState();
        }

        private void RequireInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Simulation controller is not initialized.");
            }
        }
    }
}
