using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    internal static class CelestialTestFactory
    {
        internal static CelestialBodyModel CreateSun(double rotationPeriodSeconds = 100d)
        {
            return new CelestialBodyModel(
                new CelestialBodyId("sun"),
                "Sun",
                CelestialBodyCategory.Star,
                null,
                696_340d,
                1.98847e30d,
                rotationPeriodSeconds,
                7.25d,
                null,
                "TEST-SUN");
        }

        internal static CelestialBodyModel CreateOrbitingBody(
            string id,
            string parentId,
            CelestialBodyCategory category = CelestialBodyCategory.Planet,
            double semiMajorAxisKm = 100d,
            double eccentricity = 0d,
            double inclinationDeg = 0d,
            double longitudeAscendingNodeDeg = 0d,
            double argumentPeriapsisDeg = 0d,
            double meanAnomalyAtEpochDeg = 0d,
            double orbitalPeriodSeconds = 40d,
            double rotationPeriodSeconds = 10d)
        {
            return new CelestialBodyModel(
                new CelestialBodyId(id),
                id,
                category,
                new CelestialBodyId(parentId),
                1d,
                null,
                rotationPeriodSeconds,
                0d,
                new OrbitalElements(
                    semiMajorAxisKm,
                    eccentricity,
                    inclinationDeg,
                    longitudeAscendingNodeDeg,
                    argumentPeriapsisDeg,
                    meanAnomalyAtEpochDeg,
                    orbitalPeriodSeconds),
                $"TEST-{id}");
        }

        internal static CelestialCatalog BuildCatalog(params CelestialBodyModel[] bodies)
        {
            return new CelestialCatalogBuilder().Build(bodies);
        }
    }
}
