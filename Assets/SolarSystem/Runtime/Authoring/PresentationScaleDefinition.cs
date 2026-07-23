using Tanvir.SolarSystem.Presentation.Scale;
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
        [SerializeField] private double distanceReferenceKm = 1000000d;
        [Tooltip("Unity units allocated to each base-10 distance interval.")]
        [SerializeField] private double unitsPerDistanceDecade = 15d;

        [Header("Radius exaggeration")]
        [SerializeField] private double radiusReferenceKm = 6371d;
        [SerializeField] private double referenceDisplayRadius = 0.8d;
        [SerializeField] private double radiusExponent = 0.4d;
        [SerializeField] private double minimumDisplayRadius = 0.18d;
        [SerializeField] private double maximumDisplayRadius = 4.8d;

        /// <summary>Creates immutable, validated runtime scale parameters.</summary>
        public PresentationScaleParameters ToParameters()
        {
            return new PresentationScaleParameters(
                distanceReferenceKm,
                unitsPerDistanceDecade,
                radiusReferenceKm,
                referenceDisplayRadius,
                radiusExponent,
                minimumDisplayRadius,
                maximumDisplayRadius);
        }
    }
}
