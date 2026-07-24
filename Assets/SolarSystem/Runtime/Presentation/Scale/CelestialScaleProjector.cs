using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Mathematics;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>Projects physical hierarchy-relative states into a readable, stable Unity scale.</summary>
    public sealed class CelestialScaleProjector
    {
        private readonly PresentationScaleParameters parameters;

        /// <summary>Initializes a projector with immutable, validated scale parameters.</summary>
        public CelestialScaleProjector(PresentationScaleParameters parameters)
        {
            this.parameters = parameters;
            CurrentMode = CelestialScaleMode.ReadableOverview;
        }

        /// <summary>Gets the active, explicitly disclosed presentation scale.</summary>
        public CelestialScaleMode CurrentMode { get; private set; }

        /// <summary>
        /// Gets the proportional reference used by body radii and future guided comparison.
        /// </summary>
        public PhysicalScaleReference PhysicalReference => parameters.PhysicalReference;

        /// <summary>Gets the conservative minimum surface-clearance target.</summary>
        public double MinimumSurfaceClearance => parameters.MinimumSurfaceClearance;

        /// <summary>Changes projection policy without mutating physical state.</summary>
        public void SetMode(CelestialScaleMode mode)
        {
            if (!Enum.IsDefined(typeof(CelestialScaleMode), mode))
            {
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            CurrentMode = mode;
        }

        /// <summary>Projects one parent-relative physical offset in the active scale.</summary>
        public Vector3 ProjectRelativePosition(Double3 parentRelativePositionKm)
        {
            if (!parentRelativePositionKm.IsFinite)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(parentRelativePositionKm),
                    "Physical offset must be finite.");
            }

            double physicalDistanceKm = parentRelativePositionKm.Magnitude;
            if (physicalDistanceKm == 0d)
            {
                return Vector3.zero;
            }

            double displayDistance = CurrentMode switch
            {
                CelestialScaleMode.ReadableOverview =>
                    Math.Log10(1d + (physicalDistanceKm / parameters.DistanceReferenceKm)) *
                    parameters.UnitsPerDistanceDecade,
                CelestialScaleMode.NormalizedOrbits =>
                    physicalDistanceKm /
                    GuidedScaleComparisonContract.MercuryVenusEnvelopeGapKm,
                CelestialScaleMode.LiteralEarthReference =>
                    parameters.PhysicalReference.ToDisplayUnits(physicalDistanceKm),
                _ => throw new InvalidOperationException(
                    $"Unsupported scale mode '{CurrentMode}'.")
            };
            var coreDirection = new Double3(
                parentRelativePositionKm.X / physicalDistanceKm,
                parentRelativePositionKm.Y / physicalDistanceKm,
                parentRelativePositionKm.Z / physicalDistanceKm);
            return UnityCoordinateAdapter.ToUnityVector(coreDirection) * ToFiniteFloat(displayDistance);
        }

        /// <summary>Projects one physical radius in the active common scale.</summary>
        public float ProjectRadius(double physicalRadiusKm)
        {
            if (double.IsNaN(physicalRadiusKm) ||
                double.IsInfinity(physicalRadiusKm) ||
                physicalRadiusKm <= 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(physicalRadiusKm),
                    physicalRadiusKm,
                    "Physical radius must be positive and finite.");
            }

            double displayRadius = CurrentMode switch
            {
                CelestialScaleMode.ReadableOverview =>
                    parameters.PhysicalReference.ToDisplayUnits(physicalRadiusKm),
                CelestialScaleMode.NormalizedOrbits =>
                    physicalRadiusKm /
                    GuidedScaleComparisonContract.MercuryVenusEnvelopeGapKm,
                CelestialScaleMode.LiteralEarthReference =>
                    parameters.PhysicalReference.ToDisplayUnits(physicalRadiusKm),
                _ => throw new InvalidOperationException(
                    $"Unsupported scale mode '{CurrentMode}'.")
            };
            return ToFiniteFloat(displayRadius);
        }

        /// <summary>Projects a complete parent-first catalog into a caller-owned allocation-free buffer.</summary>
        public void Project(
            CelestialCatalog catalog,
            IReadOnlyList<CelestialState> physicalStates,
            CelestialPresentationState[] destination)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            if (physicalStates == null)
            {
                throw new ArgumentNullException(nameof(physicalStates));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (physicalStates.Count != catalog.Count || destination.Length < catalog.Count)
            {
                throw new ArgumentException("Projection buffers must match the validated catalog count.");
            }

            for (int index = 0; index < catalog.Count; index++)
            {
                CelestialBodyModel body = catalog.OrderedBodies[index];
                CelestialState physicalState = physicalStates[index];
                if (body.Id != physicalState.Id)
                {
                    throw new InvalidOperationException(
                        $"Physical state '{physicalState.Id}' does not match catalog body '{body.Id}'.");
                }

                Vector3 parentPosition = Vector3.zero;
                if (body.ParentId.HasValue)
                {
                    int parentIndex = FindPriorBodyIndex(catalog, body.ParentId.Value, index);
                    parentPosition = destination[parentIndex].Position;
                }

                destination[index] = new CelestialPresentationState(
                    body.Id,
                    parentPosition + ProjectRelativePosition(physicalState.ParentRelativePositionKm),
                    ProjectRadius(body.MeanRadiusKm),
                    ToFiniteFloat(physicalState.RotationAngleDeg));
            }

            if (CurrentMode == CelestialScaleMode.LiteralEarthReference)
            {
                ApplyLiteralRenderOrigin(catalog, destination);
            }
        }

        private static void ApplyLiteralRenderOrigin(
            CelestialCatalog catalog,
            CelestialPresentationState[] destination)
        {
            var originId = new CelestialBodyId(
                GuidedScaleComparisonContract.LiteralRenderOriginStableId);
            int originIndex = FindBodyIndex(catalog, originId);
            Vector3 renderOrigin = destination[originIndex].Position;
            for (int index = 0; index < catalog.Count; index++)
            {
                CelestialPresentationState state = destination[index];
                destination[index] = new CelestialPresentationState(
                    state.Id,
                    state.Position - renderOrigin,
                    state.DisplayRadius,
                    state.RotationAngleDeg);
            }
        }

        private static int FindBodyIndex(
            CelestialCatalog catalog,
            CelestialBodyId bodyId)
        {
            for (int index = 0; index < catalog.Count; index++)
            {
                if (catalog.OrderedBodies[index].Id == bodyId)
                {
                    return index;
                }
            }

            throw new InvalidOperationException(
                $"Render-origin body '{bodyId}' is not present in the catalog.");
        }

        private static int FindPriorBodyIndex(
            CelestialCatalog catalog,
            CelestialBodyId parentId,
            int childIndex)
        {
            for (int index = 0; index < childIndex; index++)
            {
                if (catalog.OrderedBodies[index].Id == parentId)
                {
                    return index;
                }
            }

            throw new InvalidOperationException(
                $"Catalog order placed a body before parent '{parentId}'.");
        }

        private static float ToFiniteFloat(double value)
        {
            if (double.IsNaN(value) ||
                double.IsInfinity(value) ||
                value > float.MaxValue ||
                value < -float.MaxValue)
            {
                throw new OverflowException("Projected value exceeded Unity's finite float range.");
            }

            return (float)value;
        }
    }
}
