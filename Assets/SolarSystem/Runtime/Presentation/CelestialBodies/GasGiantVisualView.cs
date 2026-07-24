using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Applies deterministic simulation-time presentation to a gas giant.</summary>
    [DisallowMultipleComponent]
    public sealed class GasGiantVisualView : MonoBehaviour
    {
        private static readonly int SimulationPhaseId =
            Shader.PropertyToID("_SimulationPhase");

        [SerializeField] private GasGiantVisualDefinition definition;
        [SerializeField] private Transform atmosphereShell;
        [SerializeField] private MeshRenderer surfaceRenderer;
        [SerializeField] private MeshRenderer atmosphereRenderer;

        private MaterialPropertyBlock surfaceProperties;
        private GasGiantVisualModel model;
        private double signedRotationPeriodSeconds;

        /// <summary>Gets whether the view owns validated immutable gas-giant data.</summary>
        public bool IsInitialized => model != null;

        /// <summary>Gets the authored definition.</summary>
        public GasGiantVisualDefinition Definition => definition;

        /// <summary>Gets the atmosphere-shell transform.</summary>
        public Transform AtmosphereShell => atmosphereShell;

        /// <summary>Gets the physical surface renderer.</summary>
        public MeshRenderer SurfaceRenderer => surfaceRenderer;

        /// <summary>Gets the atmosphere renderer.</summary>
        public MeshRenderer AtmosphereRenderer => atmosphereRenderer;

        /// <summary>Gets the last deterministic band phase in the range [0, 1).</summary>
        public float BandPhase { get; private set; }

        /// <summary>Initializes the presentation against its immutable body.</summary>
        public void Initialize(CelestialBodyModel body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (definition == null ||
                atmosphereShell == null ||
                surfaceRenderer == null ||
                atmosphereRenderer == null)
            {
                throw new InvalidOperationException(
                    $"Gas-giant visual '{name}' has incomplete serialized dependencies.");
            }

            GasGiantVisualModel runtimeModel = definition.ToModel();
            if (!string.Equals(
                    runtimeModel.BodyStableId,
                    body.Id.Value,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Gas-giant visual '{name}' expects '{runtimeModel.BodyStableId}' " +
                    $"but received '{body.Id.Value}'.");
            }

            model = runtimeModel;
            signedRotationPeriodSeconds = body.RotationPeriodSeconds;
            surfaceProperties ??= new MaterialPropertyBlock();
            atmosphereShell.localScale =
                Vector3.one * model.AtmosphereShellRadiusMultiplier;
            surfaceRenderer.shadowCastingMode = ShadowCastingMode.Off;
            surfaceRenderer.receiveShadows = false;
            atmosphereRenderer.shadowCastingMode = ShadowCastingMode.Off;
            atmosphereRenderer.receiveShadows = false;
            atmosphereRenderer.lightProbeUsage = LightProbeUsage.Off;
            atmosphereRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            surfaceRenderer.GetPropertyBlock(surfaceProperties);
            Apply(0d);
        }

        /// <summary>Applies the absolute deterministic band phase.</summary>
        public void Apply(double simulationTimeSeconds)
        {
            if (model == null)
            {
                throw new InvalidOperationException(
                    $"Gas-giant visual '{name}' must be initialized before use.");
            }

            BandPhase = model.EvaluateBandPhase(
                simulationTimeSeconds,
                signedRotationPeriodSeconds);
            surfaceProperties.SetFloat(SimulationPhaseId, BandPhase);
            surfaceRenderer.SetPropertyBlock(surfaceProperties);
        }
    }
}
