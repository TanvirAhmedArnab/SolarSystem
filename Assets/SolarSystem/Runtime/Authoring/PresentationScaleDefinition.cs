using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Defines a tunable, explicitly non-physical presentation scale.</summary>
    [CreateAssetMenu(
        fileName = "Scale_PresentationGraybox",
        menuName = "Solar System/Data/Presentation Scale")]
    public sealed class PresentationScaleDefinition : ScriptableObject
    {
        [Header("Distance compression")]
        [Tooltip("Physical kilometers represented by the first logarithmic interval.")]
        [SerializeField] private double distanceReferenceKm =
            ReadableOverviewScaleContract.DistanceReferenceKm;
        [Tooltip("Unity units allocated to each base-10 distance interval.")]
        [SerializeField] private double unitsPerDistanceDecade =
            ReadableOverviewScaleContract.UnitsPerDistanceDecade;

        [Header("Earth-referenced proportional size")]
        [Tooltip("Verified Earth mean radius used as the shared size reference.")]
        [SerializeField] private double radiusReferenceKm =
            CelestialReferenceUnits.EarthMeanRadiusKm;
        [Tooltip("Earth display radius. Every other body uses the same linear ratio.")]
        [SerializeField] private double referenceDisplayRadius =
            ReadableOverviewScaleContract.EarthDisplayRadius;

        [Header("Readable-overview acceptance")]
        [Tooltip("Minimum conservative surface gap, measured in Earth-radius display units.")]
        [SerializeField] private double minimumSurfaceClearance =
            ReadableOverviewScaleContract.MinimumSurfaceClearance;

        /// <summary>Creates immutable, validated runtime scale parameters.</summary>
        public PresentationScaleParameters ToParameters()
        {
            return new PresentationScaleParameters(
                distanceReferenceKm,
                unitsPerDistanceDecade,
                radiusReferenceKm,
                referenceDisplayRadius,
                minimumSurfaceClearance);
        }
    }
}
