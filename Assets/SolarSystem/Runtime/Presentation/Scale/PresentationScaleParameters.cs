using System;

namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>Stores immutable parameters for the readable graybox scale projection.</summary>
    public readonly struct PresentationScaleParameters
    {
        /// <summary>Initializes validated presentation-scale parameters.</summary>
        public PresentationScaleParameters(
            double distanceReferenceKm,
            double unitsPerDistanceDecade,
            double radiusReferenceKm,
            double referenceDisplayRadius,
            double minimumSurfaceClearance)
        {
            RequirePositiveFinite(distanceReferenceKm, nameof(distanceReferenceKm));
            RequirePositiveFinite(unitsPerDistanceDecade, nameof(unitsPerDistanceDecade));
            RequirePositiveFinite(radiusReferenceKm, nameof(radiusReferenceKm));
            RequirePositiveFinite(referenceDisplayRadius, nameof(referenceDisplayRadius));
            RequirePositiveFinite(minimumSurfaceClearance, nameof(minimumSurfaceClearance));

            DistanceReferenceKm = distanceReferenceKm;
            UnitsPerDistanceDecade = unitsPerDistanceDecade;
            RadiusReferenceKm = radiusReferenceKm;
            ReferenceDisplayRadius = referenceDisplayRadius;
            MinimumSurfaceClearance = minimumSurfaceClearance;
            PhysicalReference = new PhysicalScaleReference(
                radiusReferenceKm,
                referenceDisplayRadius);
        }

        /// <summary>Gets the physical distance represented by the first logarithmic interval.</summary>
        public double DistanceReferenceKm { get; }

        /// <summary>Gets display units allocated to each base-10 distance interval.</summary>
        public double UnitsPerDistanceDecade { get; }

        /// <summary>Gets the physical radius used as the visual reference.</summary>
        public double RadiusReferenceKm { get; }

        /// <summary>Gets the display radius assigned to the reference body.</summary>
        public double ReferenceDisplayRadius { get; }

        /// <summary>Gets the required conservative surface clearance in display units.</summary>
        public double MinimumSurfaceClearance { get; }

        /// <summary>Gets the shared proportional reference for body size and guided comparison.</summary>
        public PhysicalScaleReference PhysicalReference { get; }

        private static void RequirePositiveFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0d)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Scale parameter must be positive and finite.");
            }
        }
    }
}
