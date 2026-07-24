namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>
    /// Owns reviewed constants for the guided comparison between readable and
    /// physically linear scale presentations.
    /// </summary>
    public static class GuidedScaleComparisonContract
    {
        /// <summary>
        /// JPL-derived Venus perihelion minus Mercury aphelion, in kilometers.
        /// </summary>
        public const double MercuryVenusEnvelopeGapKm = 37658725.03012079d;

        /// <summary>Stable Earth body identifier used as the literal-mode render origin.</summary>
        public const string LiteralRenderOriginStableId = "earth";

        /// <summary>Diagrammatic line width in normalized orbit units.</summary>
        public const float NormalizedOrbitLineWidth = 0.08f;

        /// <summary>Diagrammatic line width in literal Earth-radius units.</summary>
        public const float LiteralOrbitLineWidth = 20f;
    }
}
