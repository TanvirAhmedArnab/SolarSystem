using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Mathematics;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Caches one projected parent-relative orbit path for a LineRenderer.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public sealed class CelestialOrbitPathView : MonoBehaviour
    {
        private const int MinimumSampleCount = 16;

        [SerializeField] private CelestialBodyDefinition definition;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int sampleCount = 128;
        private bool hasReadableWidth;
        private float readableWidth;

        /// <summary>Gets the stable ID whose orbit this path represents.</summary>
        public string StableId => definition == null ? string.Empty : definition.StableId;

        /// <summary>Initializes and caches the complete orbit shape.</summary>
        public void Initialize(
            CelestialBodyModel model,
            KeplerOrbitEvaluator evaluator,
            CelestialScaleProjector projector)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            if (definition == null || definition.StableId != model.Id.Value)
            {
                throw new InvalidOperationException(
                    $"Orbit path '{name}' is not assigned to runtime body '{model.Id}'.");
            }

            if (lineRenderer == null)
            {
                throw new InvalidOperationException($"Orbit path '{name}' has no LineRenderer.");
            }

            if (!model.Orbit.HasValue)
            {
                lineRenderer.positionCount = 0;
                lineRenderer.enabled = false;
                return;
            }

            if (!hasReadableWidth)
            {
                readableWidth = lineRenderer.widthMultiplier;
                hasReadableWidth = true;
            }

            lineRenderer.widthMultiplier = projector.CurrentMode switch
            {
                CelestialScaleMode.ReadableOverview => readableWidth,
                CelestialScaleMode.NormalizedOrbits =>
                    GuidedScaleComparisonContract.NormalizedOrbitLineWidth,
                CelestialScaleMode.LiteralEarthReference =>
                    GuidedScaleComparisonContract.LiteralOrbitLineWidth,
                _ => throw new InvalidOperationException(
                    $"Unsupported scale mode '{projector.CurrentMode}'.")
            };
            lineRenderer.enabled = true;
            lineRenderer.useWorldSpace = false;
            int resolvedSampleCount = Math.Max(MinimumSampleCount, sampleCount);
            lineRenderer.positionCount = resolvedSampleCount + 1;
            OrbitalElements orbit = model.Orbit.Value;

            for (int index = 0; index <= resolvedSampleCount; index++)
            {
                double timeSeconds = orbit.OrbitalPeriodSeconds * index / resolvedSampleCount;
                CelestialState state = evaluator.Evaluate(model, Double3.Zero, timeSeconds);
                lineRenderer.SetPosition(
                    index,
                    projector.ProjectRelativePosition(state.ParentRelativePositionKm));
            }
        }

        /// <summary>Moves the cached parent-relative path to its current parent position.</summary>
        public void ApplyParentPosition(Vector3 parentPosition)
        {
            transform.position = parentPosition;
        }
    }
}
