using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>
    /// Applies deterministic, body-relative presentation to optional cloud and atmosphere shells.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CelestialLayeredBodyView : MonoBehaviour
    {
        [SerializeField] private CelestialLayerVisualDefinition definition;
        [SerializeField] private Transform cloudShell;
        [SerializeField] private Transform atmosphereShell;
        [SerializeField] private MeshRenderer surfaceRenderer;
        [SerializeField] private MeshRenderer cloudRenderer;
        [SerializeField] private MeshRenderer atmosphereRenderer;

        private CelestialLayerVisualModel model;
        private double signedRotationPeriodSeconds;

        /// <summary>Gets whether the view owns a validated immutable layer model.</summary>
        public bool IsInitialized => model != null;

        /// <summary>Gets the authored definition.</summary>
        public CelestialLayerVisualDefinition Definition => definition;

        /// <summary>Gets the cloud shell transform.</summary>
        public Transform CloudShell => cloudShell;

        /// <summary>Gets the atmosphere shell transform.</summary>
        public Transform AtmosphereShell => atmosphereShell;

        /// <summary>Gets the physical surface renderer.</summary>
        public MeshRenderer SurfaceRenderer => surfaceRenderer;

        /// <summary>Gets the cloud-shell renderer.</summary>
        public MeshRenderer CloudRenderer => cloudRenderer;

        /// <summary>Gets the atmosphere-shell renderer.</summary>
        public MeshRenderer AtmosphereRenderer => atmosphereRenderer;

        /// <summary>Gets the current deterministic cloud drift angle relative to the surface.</summary>
        public float CloudRelativeRotationDeg { get; private set; }

        /// <summary>Initializes the layer view for its owning immutable body.</summary>
        public void Initialize(CelestialBodyModel body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (definition == null ||
                cloudShell == null ||
                atmosphereShell == null ||
                surfaceRenderer == null ||
                cloudRenderer == null ||
                atmosphereRenderer == null)
            {
                throw new InvalidOperationException(
                    $"Layered view '{name}' has incomplete serialized dependencies.");
            }

            CelestialLayerVisualModel runtimeModel = definition.ToModel();
            if (!string.Equals(
                    runtimeModel.BodyStableId,
                    body.Id.Value,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Layered view '{name}' expects '{runtimeModel.BodyStableId}' " +
                    $"but received '{body.Id.Value}'.");
            }

            model = runtimeModel;
            signedRotationPeriodSeconds = body.RotationPeriodSeconds;
            cloudShell.localScale = Vector3.one * model.CloudShellRadiusMultiplier;
            atmosphereShell.localScale =
                Vector3.one * model.AtmosphereShellRadiusMultiplier;
        }

        /// <summary>Applies deterministic layer motion from authoritative absolute time.</summary>
        public void Apply(double simulationTimeSeconds)
        {
            if (model == null)
            {
                throw new InvalidOperationException(
                    $"Layered view '{name}' must be initialized before use.");
            }

            if (double.IsNaN(simulationTimeSeconds) ||
                double.IsInfinity(simulationTimeSeconds))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(simulationTimeSeconds),
                    "Layer simulation time must be finite.");
            }

            double relativeAngle =
                -360d *
                simulationTimeSeconds /
                signedRotationPeriodSeconds *
                (model.CloudRotationMultiplier - 1f);
            CloudRelativeRotationDeg =
                Mathf.Repeat((float)(relativeAngle % 360d), 360f);
            cloudShell.localRotation =
                Quaternion.AngleAxis(CloudRelativeRotationDeg, Vector3.up);
        }
    }
}
