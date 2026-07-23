using System;
using System.Collections.Generic;
using System.Linq;

namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Validates runtime celestial models and creates a deterministic immutable catalog.</summary>
    public sealed class CelestialCatalogBuilder
    {
        /// <summary>Builds a validated catalog from programmatic or authoring-converted models.</summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bodies"/> is null.</exception>
        /// <exception cref="CelestialCatalogValidationException">Thrown when any model or relationship is invalid.</exception>
        public CelestialCatalog Build(IEnumerable<CelestialBodyModel> bodies)
        {
            if (bodies == null)
            {
                throw new ArgumentNullException(nameof(bodies));
            }

            List<CelestialBodyModel> models = bodies.ToList();
            if (models.Count == 0)
            {
                throw Invalid("The celestial catalog cannot be empty.");
            }

            var byId = new Dictionary<CelestialBodyId, CelestialBodyModel>(models.Count);
            foreach (CelestialBodyModel body in models)
            {
                ValidateBody(body);
                if (!byId.TryAdd(body.Id, body))
                {
                    throw Invalid($"Celestial body ID '{body.Id}' is duplicated.");
                }
            }

            List<CelestialBodyModel> roots = models.Where(body => !body.ParentId.HasValue).ToList();
            if (roots.Count != 1)
            {
                throw Invalid($"The celestial catalog must contain exactly one root body; found {roots.Count}.");
            }

            CelestialBodyModel root = roots[0];
            if (root.Category != CelestialBodyCategory.Star)
            {
                throw Invalid($"Root body '{root.Id}' must use the Star category.");
            }

            if (root.Orbit.HasValue)
            {
                throw Invalid($"Root body '{root.Id}' cannot define an orbit.");
            }

            foreach (CelestialBodyModel body in models)
            {
                if (!body.ParentId.HasValue)
                {
                    continue;
                }

                CelestialBodyId parentId = body.ParentId.Value;
                if (parentId == body.Id)
                {
                    throw Invalid($"Celestial body '{body.Id}' cannot parent itself.");
                }

                if (!byId.ContainsKey(parentId))
                {
                    throw Invalid($"Celestial body '{body.Id}' references missing parent '{parentId}'.");
                }

                if (!body.Orbit.HasValue)
                {
                    throw Invalid($"Non-root body '{body.Id}' must define an orbit.");
                }
            }

            return new CelestialCatalog(CreateParentFirstOrder(models, root));
        }

        private static IList<CelestialBodyModel> CreateParentFirstOrder(
            IEnumerable<CelestialBodyModel> models,
            CelestialBodyModel root)
        {
            var remaining = models
                .Where(body => body.Id != root.Id)
                .ToDictionary(body => body.Id, body => body);
            var ordered = new List<CelestialBodyModel> { root };
            var resolved = new HashSet<CelestialBodyId> { root.Id };

            while (remaining.Count > 0)
            {
                List<CelestialBodyModel> next = remaining.Values
                    .Where(body => body.ParentId.HasValue && resolved.Contains(body.ParentId.Value))
                    .OrderBy(body => body.Id)
                    .ToList();

                if (next.Count == 0)
                {
                    string unresolved = string.Join(", ", remaining.Keys.OrderBy(id => id));
                    throw Invalid($"The celestial catalog contains a parent cycle involving: {unresolved}.");
                }

                foreach (CelestialBodyModel body in next)
                {
                    ordered.Add(body);
                    resolved.Add(body.Id);
                    remaining.Remove(body.Id);
                }
            }

            return ordered;
        }

        private static void ValidateBody(CelestialBodyModel body)
        {
            if (body == null)
            {
                throw Invalid("The celestial catalog contains a null body.");
            }

            if (string.IsNullOrWhiteSpace(body.Id.Value))
            {
                throw Invalid("A celestial body has an empty ID.");
            }

            if (string.IsNullOrWhiteSpace(body.DisplayName))
            {
                throw Invalid($"Celestial body '{body.Id}' has an empty display name.");
            }

            RequirePositiveFinite(body.MeanRadiusKm, body.Id, nameof(body.MeanRadiusKm));
            if (body.MassKg.HasValue)
            {
                RequirePositiveFinite(body.MassKg.Value, body.Id, nameof(body.MassKg));
            }

            RequireNonZeroFinite(body.RotationPeriodSeconds, body.Id, nameof(body.RotationPeriodSeconds));
            RequireFinite(body.AxialTiltDeg, body.Id, nameof(body.AxialTiltDeg));

            if (string.IsNullOrWhiteSpace(body.ScientificSourceId))
            {
                throw Invalid($"Celestial body '{body.Id}' has no scientific source ID.");
            }

            if (body.Orbit.HasValue)
            {
                ValidateOrbit(body.Id, body.Orbit.Value);
            }
        }

        private static void ValidateOrbit(CelestialBodyId bodyId, OrbitalElements orbit)
        {
            RequirePositiveFinite(orbit.SemiMajorAxisKm, bodyId, nameof(orbit.SemiMajorAxisKm));
            RequirePositiveFinite(orbit.OrbitalPeriodSeconds, bodyId, nameof(orbit.OrbitalPeriodSeconds));
            RequireFinite(orbit.Eccentricity, bodyId, nameof(orbit.Eccentricity));
            if (orbit.Eccentricity < 0d || orbit.Eccentricity >= 1d)
            {
                throw Invalid($"Celestial body '{bodyId}' has eccentricity outside [0, 1).");
            }

            RequireFinite(orbit.InclinationDeg, bodyId, nameof(orbit.InclinationDeg));
            RequireFinite(orbit.LongitudeAscendingNodeDeg, bodyId, nameof(orbit.LongitudeAscendingNodeDeg));
            RequireFinite(orbit.ArgumentPeriapsisDeg, bodyId, nameof(orbit.ArgumentPeriapsisDeg));
            RequireFinite(orbit.MeanAnomalyAtEpochDeg, bodyId, nameof(orbit.MeanAnomalyAtEpochDeg));
        }

        private static void RequirePositiveFinite(double value, CelestialBodyId id, string field)
        {
            RequireFinite(value, id, field);
            if (value <= 0d)
            {
                throw Invalid($"Celestial body '{id}' requires positive {field}.");
            }
        }

        private static void RequireNonZeroFinite(double value, CelestialBodyId id, string field)
        {
            RequireFinite(value, id, field);
            if (value == 0d)
            {
                throw Invalid($"Celestial body '{id}' requires non-zero {field}.");
            }
        }

        private static void RequireFinite(double value, CelestialBodyId id, string field)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw Invalid($"Celestial body '{id}' requires finite {field}.");
            }
        }

        private static CelestialCatalogValidationException Invalid(string message) =>
            new CelestialCatalogValidationException(message);
    }
}
