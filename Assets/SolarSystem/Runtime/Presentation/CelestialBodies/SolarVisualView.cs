using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Applies deterministic simulation-time phases to solar render layers.</summary>
    [DisallowMultipleComponent]
    public sealed class SolarVisualView : MonoBehaviour
    {
        private static readonly int SimulationPhaseId =
            Shader.PropertyToID("_SimulationPhase");

        [SerializeField] private SolarVisualDefinition definition;
        [SerializeField] private Transform coronaShell;
        [SerializeField] private MeshRenderer surfaceRenderer;
        [SerializeField] private MeshRenderer coronaRenderer;

        private MaterialPropertyBlock surfaceProperties;
        private MaterialPropertyBlock coronaProperties;
        private SolarVisualModel model;
        private double signedRotationPeriodSeconds;

        /// <summary>Gets whether the view owns validated immutable solar data.</summary>
        public bool IsInitialized => model != null;

        /// <summary>Gets the authored definition.</summary>
        public SolarVisualDefinition Definition => definition;

        /// <summary>Gets the corona-shell transform.</summary>
        public Transform CoronaShell => coronaShell;

        /// <summary>Gets the physical surface renderer.</summary>
        public MeshRenderer SurfaceRenderer => surfaceRenderer;

        /// <summary>Gets the corona renderer.</summary>
        public MeshRenderer CoronaRenderer => coronaRenderer;

        /// <summary>Gets the last deterministic surface phase in the range [0, 1).</summary>
        public float SurfacePhase { get; private set; }

        /// <summary>Gets the last deterministic corona phase in the range [0, 1).</summary>
        public float CoronaPhase { get; private set; }

        /// <summary>Initializes the visual against its owning immutable body.</summary>
        public void Initialize(CelestialBodyModel body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (definition == null ||
                coronaShell == null ||
                surfaceRenderer == null ||
                coronaRenderer == null)
            {
                throw new InvalidOperationException(
                    $"Solar visual '{name}' has incomplete serialized dependencies.");
            }

            SolarVisualModel runtimeModel = definition.ToModel();
            if (!string.Equals(
                    runtimeModel.BodyStableId,
                    body.Id.Value,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Solar visual '{name}' expects '{runtimeModel.BodyStableId}' " +
                    $"but received '{body.Id.Value}'.");
            }

            model = runtimeModel;
            signedRotationPeriodSeconds = body.RotationPeriodSeconds;
            surfaceProperties ??= new MaterialPropertyBlock();
            coronaProperties ??= new MaterialPropertyBlock();
            coronaShell.localScale =
                Vector3.one * model.CoronaShellRadiusMultiplier;
            surfaceRenderer.shadowCastingMode = ShadowCastingMode.Off;
            surfaceRenderer.receiveShadows = false;
            coronaRenderer.shadowCastingMode = ShadowCastingMode.Off;
            coronaRenderer.receiveShadows = false;
            surfaceRenderer.GetPropertyBlock(surfaceProperties);
            coronaRenderer.GetPropertyBlock(coronaProperties);
            Apply(0d);
        }

        /// <summary>Applies absolute deterministic animation phases.</summary>
        public void Apply(double simulationTimeSeconds)
        {
            if (model == null)
            {
                throw new InvalidOperationException(
                    $"Solar visual '{name}' must be initialized before use.");
            }

            SurfacePhase = model.EvaluateSurfacePhase(
                simulationTimeSeconds,
                signedRotationPeriodSeconds);
            CoronaPhase = model.EvaluateCoronaPhase(
                simulationTimeSeconds,
                signedRotationPeriodSeconds);

            surfaceProperties.SetFloat(SimulationPhaseId, SurfacePhase);
            surfaceRenderer.SetPropertyBlock(surfaceProperties);
            coronaProperties.SetFloat(SimulationPhaseId, CoronaPhase);
            coronaRenderer.SetPropertyBlock(coronaProperties);
        }
    }
}
