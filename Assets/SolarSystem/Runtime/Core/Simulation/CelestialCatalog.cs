using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Provides validated celestial definitions in deterministic parent-first order.</summary>
    public sealed class CelestialCatalog
    {
        private readonly IReadOnlyDictionary<CelestialBodyId, CelestialBodyModel> bodiesById;

        internal CelestialCatalog(IList<CelestialBodyModel> orderedBodies)
        {
            var bodyList = new List<CelestialBodyModel>(orderedBodies);
            var bodyMap = new Dictionary<CelestialBodyId, CelestialBodyModel>(bodyList.Count);
            foreach (CelestialBodyModel body in bodyList)
            {
                bodyMap.Add(body.Id, body);
            }

            OrderedBodies = new ReadOnlyCollection<CelestialBodyModel>(bodyList);
            bodiesById = new ReadOnlyDictionary<CelestialBodyId, CelestialBodyModel>(bodyMap);
            Root = bodyList[0];
        }

        /// <summary>Gets all bodies in deterministic parent-first order.</summary>
        public IReadOnlyList<CelestialBodyModel> OrderedBodies { get; }

        /// <summary>Gets the single root body.</summary>
        public CelestialBodyModel Root { get; }

        /// <summary>Gets the number of catalog bodies.</summary>
        public int Count => OrderedBodies.Count;

        /// <summary>Gets a body by its stable ID.</summary>
        /// <exception cref="KeyNotFoundException">Thrown when the ID is not present.</exception>
        public CelestialBodyModel GetBody(CelestialBodyId id)
        {
            if (!bodiesById.TryGetValue(id, out CelestialBodyModel body))
            {
                throw new KeyNotFoundException($"Celestial body '{id}' is not in the catalog.");
            }

            return body;
        }

        /// <summary>Attempts to get a body by its stable ID.</summary>
        public bool TryGetBody(CelestialBodyId id, out CelestialBodyModel body)
        {
            return bodiesById.TryGetValue(id, out body);
        }
    }
}
