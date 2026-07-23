using System;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Stores unit-explicit orbital elements in a Unity-serializable form.</summary>
    [Serializable]
    public struct OrbitalElementsDefinition
    {
        [SerializeField] private double semiMajorAxisKm;
        [SerializeField] private double eccentricity;
        [SerializeField] private double inclinationDeg;
        [SerializeField] private double longitudeAscendingNodeDeg;
        [SerializeField] private double argumentPeriapsisDeg;
        [SerializeField] private double meanAnomalyAtEpochDeg;
        [SerializeField] private double orbitalPeriodSeconds;

        /// <summary>Gets the semi-major axis in kilometers.</summary>
        public double SemiMajorAxisKm => semiMajorAxisKm;

        /// <summary>Gets the dimensionless eccentricity.</summary>
        public double Eccentricity => eccentricity;

        /// <summary>Gets the inclination in degrees.</summary>
        public double InclinationDeg => inclinationDeg;

        /// <summary>Gets the longitude of the ascending node in degrees.</summary>
        public double LongitudeAscendingNodeDeg => longitudeAscendingNodeDeg;

        /// <summary>Gets the argument of periapsis in degrees.</summary>
        public double ArgumentPeriapsisDeg => argumentPeriapsisDeg;

        /// <summary>Gets the mean anomaly at the simulation epoch in degrees.</summary>
        public double MeanAnomalyAtEpochDeg => meanAnomalyAtEpochDeg;

        /// <summary>Gets the orbital period in seconds.</summary>
        public double OrbitalPeriodSeconds => orbitalPeriodSeconds;

        /// <summary>Creates the immutable Core representation without mutating this definition.</summary>
        public OrbitalElements ToModel()
        {
            return new OrbitalElements(
                semiMajorAxisKm,
                eccentricity,
                inclinationDeg,
                longitudeAscendingNodeDeg,
                argumentPeriapsisDeg,
                meanAnomalyAtEpochDeg,
                orbitalPeriodSeconds);
        }
    }
}
