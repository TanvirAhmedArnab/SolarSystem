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
            double radiusExponent,
            double minimumDisplayRadius,
            double maximumDisplayRadius)
        {
            RequirePositiveFinite(distanceReferenceKm, nameof(distanceReferenceKm));
            RequirePositiveFinite(unitsPerDistanceDecade, nameof(unitsPerDistanceDecade));
            RequirePositiveFinite(radiusReferenceKm, nameof(radiusReferenceKm));
            RequirePositiveFinite(referenceDisplayRadius, nameof(referenceDisplayRadius));
            RequirePositiveFinite(radiusExponent, nameof(radiusExponent));
            RequirePositiveFinite(minimumDisplayRadius, nameof(minimumDisplayRadius));
            RequirePositiveFinite(maximumDisplayRadius, nameof(maximumDisplayRadius));

            if (maximumDisplayRadius < minimumDisplayRadius)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumDisplayRadius),
                    maximumDisplayRadius,
                    "Maximum display radius cannot be smaller than the minimum.");
            }

            DistanceReferenceKm = distanceReferenceKm;
            UnitsPerDistanceDecade = unitsPerDistanceDecade;
            RadiusReferenceKm = radiusReferenceKm;
            ReferenceDisplayRadius = referenceDisplayRadius;
            RadiusExponent = radiusExponent;
            MinimumDisplayRadius = minimumDisplayRadius;
            MaximumDisplayRadius = maximumDisplayRadius;
        }

        /// <summary>Gets the physical distance represented by the first logarithmic interval.</summary>
        public double DistanceReferenceKm { get; }

        /// <summary>Gets display units allocated to each base-10 distance interval.</summary>
        public double UnitsPerDistanceDecade { get; }

        /// <summary>Gets the physical radius used as the visual reference.</summary>
        public double RadiusReferenceKm { get; }

        /// <summary>Gets the display radius assigned to the reference body.</summary>
        public double ReferenceDisplayRadius { get; }

        /// <summary>Gets the exponent used for monotonic radius exaggeration.</summary>
        public double RadiusExponent { get; }

        /// <summary>Gets the smallest visible display radius.</summary>
        public double MinimumDisplayRadius { get; }

        /// <summary>Gets the largest allowed display radius.</summary>
        public double MaximumDisplayRadius { get; }

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
