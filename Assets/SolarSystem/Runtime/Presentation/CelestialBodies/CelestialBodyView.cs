using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Applies projected state to one visual body without owning simulation state.</summary>
    [DisallowMultipleComponent]
    public sealed class CelestialBodyView : MonoBehaviour
    {
        [SerializeField] private CelestialBodyDefinition definition;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private SphereCollider selectionCollider;
        [SerializeField] private CelestialLayeredBodyView layeredBodyView;
        [SerializeField] private SolarVisualView solarVisualView;
        [SerializeField] private GasGiantVisualView gasGiantVisualView;

        private CelestialBodyModel model;
        private float currentDisplayRadius;

        /// <summary>Gets the assigned authoring definition.</summary>
        public CelestialBodyDefinition Definition => definition;

        /// <summary>Gets the stable ID assigned to this view.</summary>
        public string StableId => definition == null ? string.Empty : definition.StableId;

        /// <summary>Gets whether this view has received a validated runtime model.</summary>
        public bool IsInitialized => model != null;

        /// <summary>Gets the body's latest projected visual radius in world units.</summary>
        public float CurrentDisplayRadius => currentDisplayRadius;

        /// <summary>Gets the transform containing the rendered body and optional layers.</summary>
        public Transform VisualRoot => visualRoot;

        /// <summary>Gets the optional layered-body presentation adapter.</summary>
        public CelestialLayeredBodyView LayeredBodyView => layeredBodyView;

        /// <summary>Gets the optional deterministic solar presentation adapter.</summary>
        public SolarVisualView SolarVisualView => solarVisualView;

        /// <summary>Gets the optional deterministic gas-giant presentation adapter.</summary>
        public GasGiantVisualView GasGiantVisualView => gasGiantVisualView;

        /// <summary>Initializes the view against its immutable runtime model.</summary>
        public void Initialize(CelestialBodyModel runtimeModel)
        {
            if (runtimeModel == null)
            {
                throw new ArgumentNullException(nameof(runtimeModel));
            }

            if (definition == null)
            {
                throw new InvalidOperationException($"View '{name}' has no body definition.");
            }

            if (visualRoot == null)
            {
                throw new InvalidOperationException($"View '{name}' has no visual root.");
            }

            if (selectionCollider == null)
            {
                throw new InvalidOperationException($"View '{name}' has no selection collider.");
            }

            if (definition.StableId != runtimeModel.Id.Value)
            {
                throw new InvalidOperationException(
                    $"View '{name}' expects '{definition.StableId}' but received '{runtimeModel.Id}'.");
            }

            model = runtimeModel;
            layeredBodyView?.Initialize(runtimeModel);
            solarVisualView?.Initialize(runtimeModel);
            gasGiantVisualView?.Initialize(runtimeModel);
        }

        /// <summary>Applies one projected snapshot to the transform hierarchy.</summary>
        public void Apply(CelestialPresentationState state)
        {
            Apply(state, 0d);
        }

        /// <summary>Applies projected state and absolute authoritative simulation time.</summary>
        public void Apply(
            CelestialPresentationState state,
            double simulationTimeSeconds)
        {
            if (model == null)
            {
                throw new InvalidOperationException($"View '{name}' must be initialized before use.");
            }

            if (state.Id != model.Id)
            {
                throw new InvalidOperationException(
                    $"Projected state '{state.Id}' does not match view '{model.Id}'.");
            }

            transform.position = state.Position;
            currentDisplayRadius = state.DisplayRadius;
            visualRoot.localScale = Vector3.one * (currentDisplayRadius * 2f);
            selectionCollider.radius = Mathf.Max(
                currentDisplayRadius,
                ReadableOverviewScaleContract.MinimumSelectionRadius);

            Quaternion axialTilt = Quaternion.AngleAxis((float)model.AxialTiltDeg, Vector3.forward);
            Quaternion siderealSpin = Quaternion.AngleAxis(-state.RotationAngleDeg, Vector3.up);
            visualRoot.localRotation = axialTilt * siderealSpin;
            layeredBodyView?.Apply(state.RotationAngleDeg);
            solarVisualView?.Apply(simulationTimeSeconds);
            gasGiantVisualView?.Apply(simulationTimeSeconds);
        }
    }
}
