using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Read-only authoring data for one airless rocky-body treatment.</summary>
    [CreateAssetMenu(
        fileName = "VisualLayers_AirlessRocky",
        menuName = "Solar System/Presentation/Airless Rocky Visual Definition")]
    public sealed class AirlessRockyVisualDefinition : ScriptableObject
    {
        [SerializeField] private string bodyStableId = "mercury";
        [SerializeField, Range(0f, 1f)]
        private float reliefStrength =
            AirlessRockyVisualRenderingContract.MercuryReliefStrength;
        [SerializeField, Range(0.5f, 4f)]
        private float reliefSampleDistance =
            AirlessRockyVisualRenderingContract.MercuryReliefSampleDistance;
        [SerializeField, Range(0f, 0.2f)]
        private float surfaceSpecular =
            AirlessRockyVisualRenderingContract.MercurySurfaceSpecular;
        [SerializeField, Range(0f, 0.3f)]
        private float surfaceSmoothness =
            AirlessRockyVisualRenderingContract.MercurySurfaceSmoothness;
        [SerializeField, Range(0f, 0.1f)]
        private float nightsideReadability =
            AirlessRockyVisualRenderingContract.MercuryNightsideReadability;

        /// <summary>Gets the stable body ID this presentation belongs to.</summary>
        public string BodyStableId => bodyStableId;

        /// <summary>Gets the source-derived relief strength.</summary>
        public float ReliefStrength => reliefStrength;

        /// <summary>Gets the source-texel relief sample distance.</summary>
        public float ReliefSampleDistance => reliefSampleDistance;

        /// <summary>Gets the non-metallic specular response.</summary>
        public float SurfaceSpecular => surfaceSpecular;

        /// <summary>Gets the dry-surface smoothness.</summary>
        public float SurfaceSmoothness => surfaceSmoothness;

        /// <summary>Gets the bounded unlit-hemisphere readability floor.</summary>
        public float NightsideReadability => nightsideReadability;

        /// <summary>Creates the immutable validated runtime model.</summary>
        public AirlessRockyVisualModel ToModel()
        {
            return new AirlessRockyVisualModel(
                bodyStableId,
                reliefStrength,
                reliefSampleDistance,
                surfaceSpecular,
                surfaceSmoothness,
                nightsideReadability);
        }
    }
}
