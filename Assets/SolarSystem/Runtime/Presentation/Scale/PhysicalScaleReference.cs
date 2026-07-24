using System;

namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>
    /// Converts physical lengths into one documented reference scale without
    /// exponent, clamp, or body-specific exaggeration.
    /// </summary>
    public readonly struct PhysicalScaleReference
    {
        /// <summary>Initializes a validated physical-length reference.</summary>
        public PhysicalScaleReference(
            double referenceRadiusKm,
            double referenceDisplayRadius)
        {
            RequirePositiveFinite(referenceRadiusKm, nameof(referenceRadiusKm));
            RequirePositiveFinite(
                referenceDisplayRadius,
                nameof(referenceDisplayRadius));
            ReferenceRadiusKm = referenceRadiusKm;
            ReferenceDisplayRadius = referenceDisplayRadius;
        }

        /// <summary>Gets the physical radius represented by the reference body.</summary>
        public double ReferenceRadiusKm { get; }

        /// <summary>Gets the display radius assigned to the reference body.</summary>
        public double ReferenceDisplayRadius { get; }

        /// <summary>
        /// Converts any physical length to exact reference-radius display units.
        /// </summary>
        public double ToDisplayUnits(double physicalLengthKm)
        {
            RequireNonNegativeFinite(physicalLengthKm, nameof(physicalLengthKm));
            return ReferenceDisplayRadius * physicalLengthKm / ReferenceRadiusKm;
        }

        private static void RequirePositiveFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0d)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Physical-scale reference values must be positive and finite.");
            }
        }

        private static void RequireNonNegativeFinite(
            double value,
            string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Physical length must be finite and non-negative.");
            }
        }
    }
}
