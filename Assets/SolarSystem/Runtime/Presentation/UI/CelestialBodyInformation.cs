using System;
using System.Globalization;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Presentation.UI
{
    /// <summary>Contains display-ready, read-only facts for one selected celestial body.</summary>
    public sealed class CelestialBodyInformation
    {
        internal CelestialBodyInformation(
            string displayName,
            string category,
            string summary,
            string parent,
            string radius,
            string mass,
            string rotation,
            string axialTilt,
            string orbitDistance,
            string orbitPeriod,
            string sourceRecord)
        {
            DisplayName = displayName;
            Category = category;
            Summary = summary;
            Parent = parent;
            Radius = radius;
            Mass = mass;
            Rotation = rotation;
            AxialTilt = axialTilt;
            OrbitDistance = orbitDistance;
            OrbitPeriod = orbitPeriod;
            SourceRecord = sourceRecord;
        }

        /// <summary>Gets the authored display name.</summary>
        public string DisplayName { get; }

        /// <summary>Gets the human-readable body category.</summary>
        public string Category { get; }

        /// <summary>Gets the concise educational description.</summary>
        public string Summary { get; }

        /// <summary>Gets the stable parent-body relationship.</summary>
        public string Parent { get; }

        /// <summary>Gets the formatted mean radius.</summary>
        public string Radius { get; }

        /// <summary>Gets the formatted mass.</summary>
        public string Mass { get; }

        /// <summary>Gets the formatted sidereal rotation period.</summary>
        public string Rotation { get; }

        /// <summary>Gets the formatted axial tilt.</summary>
        public string AxialTilt { get; }

        /// <summary>Gets the formatted average orbital distance.</summary>
        public string OrbitDistance { get; }

        /// <summary>Gets the formatted sidereal orbital period.</summary>
        public string OrbitPeriod { get; }

        /// <summary>Gets the authored scientific-source record identifier.</summary>
        public string SourceRecord { get; }

        /// <summary>Creates display-ready facts from the selected authoring definition.</summary>
        public static CelestialBodyInformation From(CelestialBodyDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            string rotation = FormatDuration(Math.Abs(definition.RotationPeriodSeconds));
            if (definition.RotationPeriodSeconds < 0d)
            {
                rotation += " (retrograde)";
            }

            return new CelestialBodyInformation(
                definition.DisplayName,
                FormatCategory(definition.Category),
                definition.EducationalSummary,
                string.IsNullOrWhiteSpace(definition.ParentId)
                    ? "None — catalog root"
                    : ToDisplayName(definition.ParentId),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:N1} km",
                    definition.MeanRadiusKm),
                definition.HasMass
                    ? definition.MassKg.ToString("0.###E+0", CultureInfo.InvariantCulture) + " kg"
                    : "Not authored",
                rotation,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:N2}°",
                    definition.AxialTiltDeg),
                definition.HasOrbit
                    ? FormatDistance(definition.Orbit.SemiMajorAxisKm)
                    : "Not applicable",
                definition.HasOrbit
                    ? FormatDuration(definition.Orbit.OrbitalPeriodSeconds)
                    : "Not applicable",
                definition.ScientificSourceId);
        }

        private static string FormatCategory(CelestialBodyCategory category)
        {
            switch (category)
            {
                case CelestialBodyCategory.Star:
                    return "Star";
                case CelestialBodyCategory.Planet:
                    return "Planet";
                case CelestialBodyCategory.Moon:
                    return "Natural satellite";
                case CelestialBodyCategory.DwarfPlanet:
                    return "Dwarf planet";
                case CelestialBodyCategory.Asteroid:
                    return "Asteroid";
                case CelestialBodyCategory.Comet:
                    return "Comet";
                default:
                    return category.ToString();
            }
        }

        private static string FormatDistance(double kilometers)
        {
            if (kilometers >= 1000000d)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:N2} million km",
                    kilometers / 1000000d);
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:N0} km",
                kilometers);
        }

        private static string FormatDuration(double seconds)
        {
            const double secondsPerHour = 3600d;
            const double secondsPerDay = 86400d;
            const double daysPerYear = 365.25d;

            if (seconds < 2d * secondsPerDay)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:N2} hours",
                    seconds / secondsPerHour);
            }

            double days = seconds / secondsPerDay;
            if (days < 2d * daysPerYear)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:N2} days",
                    days);
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:N2} years",
                days / daysPerYear);
        }

        private static string ToDisplayName(string stableId)
        {
            if (string.IsNullOrEmpty(stableId))
            {
                return string.Empty;
            }

            return char.ToUpperInvariant(stableId[0]) + stableId.Substring(1);
        }
    }
}
