using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Defines one celestial body as read-only Unity authoring data.</summary>
    [CreateAssetMenu(
        fileName = "Body_NewCelestialBody",
        menuName = "Solar System/Data/Celestial Body")]
    public sealed class CelestialBodyDefinition : ScriptableObject
    {
        [Header("Identity and provenance")]
        [SerializeField] private string stableId;
        [SerializeField] private string displayName;
        [SerializeField] private CelestialBodyCategory category;
        [SerializeField] private string parentId;
        [SerializeField] private string scientificSourceId;
        [TextArea(2, 4)]
        [SerializeField] private string educationalSummary;

        [Header("Physical properties")]
        [SerializeField] private double meanRadiusKm;
        [SerializeField] private bool hasMass;
        [SerializeField] private double massKg;
        [SerializeField] private double rotationPeriodSeconds;
        [SerializeField] private double axialTiltDeg;

        [Header("Orbit relative to parent")]
        [SerializeField] private bool hasOrbit;
        [SerializeField] private OrbitalElementsDefinition orbit;

        /// <summary>Gets the stable serialized body ID.</summary>
        public string StableId => stableId;

        /// <summary>Gets the human-readable name.</summary>
        public string DisplayName => displayName;

        /// <summary>Gets the body category.</summary>
        public CelestialBodyCategory Category => category;

        /// <summary>Gets the stable parent ID, or an empty value for the catalog root.</summary>
        public string ParentId => parentId;

        /// <summary>Gets the scientific provenance record ID.</summary>
        public string ScientificSourceId => scientificSourceId;

        /// <summary>Gets the concise educational description shown in the explorer UI.</summary>
        public string EducationalSummary => educationalSummary;

        /// <summary>Gets the mean radius in kilometers.</summary>
        public double MeanRadiusKm => meanRadiusKm;

        /// <summary>Gets whether a mass value is authored.</summary>
        public bool HasMass => hasMass;

        /// <summary>Gets the authored mass in kilograms.</summary>
        public double MassKg => massKg;

        /// <summary>Gets the signed sidereal rotation period in seconds.</summary>
        public double RotationPeriodSeconds => rotationPeriodSeconds;

        /// <summary>Gets the axial tilt in degrees.</summary>
        public double AxialTiltDeg => axialTiltDeg;

        /// <summary>Gets whether this body has a parent-relative orbit.</summary>
        public bool HasOrbit => hasOrbit;

        /// <summary>Gets the serialized orbital elements.</summary>
        public OrbitalElementsDefinition Orbit => orbit;

        /// <summary>Creates an immutable Core model without mutating the authoring asset.</summary>
        public CelestialBodyModel ToModel()
        {
            CelestialBodyId? runtimeParentId = string.IsNullOrWhiteSpace(parentId)
                ? (CelestialBodyId?)null
                : new CelestialBodyId(parentId);

            double? runtimeMassKg = hasMass ? massKg : (double?)null;
            OrbitalElements? runtimeOrbit = hasOrbit
                ? orbit.ToModel()
                : (OrbitalElements?)null;

            return new CelestialBodyModel(
                new CelestialBodyId(stableId),
                displayName,
                category,
                runtimeParentId,
                meanRadiusKm,
                runtimeMassKg,
                rotationPeriodSeconds,
                axialTiltDeg,
                runtimeOrbit,
                scientificSourceId);
        }
    }
}
