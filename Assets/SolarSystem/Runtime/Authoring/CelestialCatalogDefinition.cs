using System.Collections.Generic;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Defines the complete authored celestial catalog and converts it into validated Core models.</summary>
    [CreateAssetMenu(
        fileName = "Catalog_SolarSystem",
        menuName = "Solar System/Data/Celestial Catalog")]
    public sealed class CelestialCatalogDefinition : ScriptableObject
    {
        [SerializeField] private List<CelestialBodyDefinition> bodies = new List<CelestialBodyDefinition>();

        /// <summary>Gets the authored definitions in their serialized order.</summary>
        public IReadOnlyList<CelestialBodyDefinition> Bodies => bodies;

        /// <summary>Builds a validated, deterministically ordered runtime catalog.</summary>
        public CelestialCatalog BuildCatalog()
        {
            var runtimeModels = new List<CelestialBodyModel>(bodies.Count);
            foreach (CelestialBodyDefinition body in bodies)
            {
                runtimeModels.Add(body == null ? null : body.ToModel());
            }

            return new CelestialCatalogBuilder().Build(runtimeModels);
        }

        /// <summary>Attempts to find an authored body by its stable, case-sensitive ID.</summary>
        public bool TryGetDefinition(string stableId, out CelestialBodyDefinition definition)
        {
            foreach (CelestialBodyDefinition candidate in bodies)
            {
                if (candidate != null && candidate.StableId == stableId)
                {
                    definition = candidate;
                    return true;
                }
            }

            definition = null;
            return false;
        }
    }
}
