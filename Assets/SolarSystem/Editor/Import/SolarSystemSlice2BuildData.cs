using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Groups authored assets consumed by the Slice 2 scene builder.</summary>
    internal sealed class SolarSystemSlice2Content
    {
        internal SolarSystemSlice2BodyContent[] Bodies { get; set; }
        internal CelestialCatalogDefinition Catalog { get; set; }
        internal PresentationScaleDefinition Scale { get; set; }
        internal Mesh SaturnRingMesh { get; set; }
        internal Material SaturnRingMaterial { get; set; }
        internal Material OrbitMaterial { get; set; }
        internal Material SkyboxMaterial { get; set; }
        internal VolumeProfile VisualProfile { get; set; }
    }

    /// <summary>Pairs one authored body with its scene presentation assets.</summary>
    internal sealed class SolarSystemSlice2BodyContent
    {
        internal CelestialBodyDefinition Definition { get; set; }
        internal Material Material { get; set; }
        internal int OrbitSampleCount { get; set; }
        internal float OrbitWidth { get; set; }
    }

    /// <summary>Describes one celestial-body asset for deterministic editor authoring.</summary>
    internal readonly struct SolarSystemSlice2BodyData
    {
        internal SolarSystemSlice2BodyData(
            string assetName,
            string stableId,
            string displayName,
            CelestialBodyCategory category,
            string parentId,
            string sourceId,
            string educationalSummary,
            double radiusKm,
            double massKg,
            double rotationPeriodSeconds,
            double axialTiltDeg,
            SolarSystemSlice2OrbitData? orbit)
        {
            AssetName = assetName;
            StableId = stableId;
            DisplayName = displayName;
            Category = category;
            ParentId = parentId;
            SourceId = sourceId;
            EducationalSummary = educationalSummary;
            RadiusKm = radiusKm;
            MassKg = massKg;
            RotationPeriodSeconds = rotationPeriodSeconds;
            AxialTiltDeg = axialTiltDeg;
            Orbit = orbit;
        }

        internal string AssetName { get; }
        internal string StableId { get; }
        internal string DisplayName { get; }
        internal CelestialBodyCategory Category { get; }
        internal string ParentId { get; }
        internal string SourceId { get; }
        internal string EducationalSummary { get; }
        internal double RadiusKm { get; }
        internal double MassKg { get; }
        internal double RotationPeriodSeconds { get; }
        internal double AxialTiltDeg { get; }
        internal SolarSystemSlice2OrbitData? Orbit { get; }
    }

    /// <summary>Describes serialized orbital elements for deterministic editor authoring.</summary>
    internal readonly struct SolarSystemSlice2OrbitData
    {
        private readonly double semiMajorAxisKm;
        private readonly double eccentricity;
        private readonly double inclinationDeg;
        private readonly double ascendingNodeDeg;
        private readonly double periapsisDeg;
        private readonly double meanAnomalyDeg;
        private readonly double periodSeconds;

        internal SolarSystemSlice2OrbitData(
            double semiMajorAxisKm,
            double eccentricity,
            double inclinationDeg,
            double ascendingNodeDeg,
            double periapsisDeg,
            double meanAnomalyDeg,
            double periodSeconds)
        {
            this.semiMajorAxisKm = semiMajorAxisKm;
            this.eccentricity = eccentricity;
            this.inclinationDeg = inclinationDeg;
            this.ascendingNodeDeg = ascendingNodeDeg;
            this.periapsisDeg = periapsisDeg;
            this.meanAnomalyDeg = meanAnomalyDeg;
            this.periodSeconds = periodSeconds;
        }

        internal void Apply(SerializedProperty property)
        {
            property.FindPropertyRelative("semiMajorAxisKm").doubleValue = semiMajorAxisKm;
            property.FindPropertyRelative("eccentricity").doubleValue = eccentricity;
            property.FindPropertyRelative("inclinationDeg").doubleValue = inclinationDeg;
            property.FindPropertyRelative("longitudeAscendingNodeDeg").doubleValue =
                ascendingNodeDeg;
            property.FindPropertyRelative("argumentPeriapsisDeg").doubleValue = periapsisDeg;
            property.FindPropertyRelative("meanAnomalyAtEpochDeg").doubleValue = meanAnomalyDeg;
            property.FindPropertyRelative("orbitalPeriodSeconds").doubleValue = periodSeconds;
        }
    }
}
