using System;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Immutable PBR presentation parameters for one airless rocky body.</summary>
    public sealed class AirlessRockyVisualModel
    {
        /// <summary>Creates a validated airless rocky-body visual model.</summary>
        public AirlessRockyVisualModel(
            string bodyStableId,
            float reliefStrength,
            float reliefSampleDistance,
            float surfaceSpecular,
            float surfaceSmoothness,
            float nightsideReadability)
        {
            BodyStableId = string.IsNullOrWhiteSpace(bodyStableId)
                ? throw new ArgumentException(
                    "An airless rocky visual requires a stable body ID.",
                    nameof(bodyStableId))
                : bodyStableId.Trim();
            ReliefStrength = RequireRange(
                reliefStrength,
                0f,
                1f,
                nameof(reliefStrength));
            ReliefSampleDistance = RequireRange(
                reliefSampleDistance,
                0.5f,
                4f,
                nameof(reliefSampleDistance));
            SurfaceSpecular = RequireRange(
                surfaceSpecular,
                0f,
                0.2f,
                nameof(surfaceSpecular));
            SurfaceSmoothness = RequireRange(
                surfaceSmoothness,
                0f,
                0.3f,
                nameof(surfaceSmoothness));
            NightsideReadability = RequireRange(
                nightsideReadability,
                0f,
                0.1f,
                nameof(nightsideReadability));
        }

        /// <summary>Gets the stable body ID this treatment belongs to.</summary>
        public string BodyStableId { get; }

        /// <summary>Gets source-derived relief strength.</summary>
        public float ReliefStrength { get; }

        /// <summary>Gets the source-texel relief sample distance.</summary>
        public float ReliefSampleDistance { get; }

        /// <summary>Gets the non-metallic specular response.</summary>
        public float SurfaceSpecular { get; }

        /// <summary>Gets the dry-surface smoothness.</summary>
        public float SurfaceSmoothness { get; }

        /// <summary>Gets the bounded unlit-hemisphere readability floor.</summary>
        public float NightsideReadability { get; }

        private static float RequireRange(
            float value,
            float inclusiveMinimum,
            float inclusiveMaximum,
            string parameterName)
        {
            if (!float.IsFinite(value) ||
                value < inclusiveMinimum ||
                value > inclusiveMaximum)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    $"Value must be between {inclusiveMinimum} and {inclusiveMaximum}.");
            }

            return value;
        }
    }
}
