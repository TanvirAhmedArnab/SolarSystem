using System;

namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Reports invalid celestial catalog data that prevents deterministic startup.</summary>
    public sealed class CelestialCatalogValidationException : Exception
    {
        /// <summary>Initializes a validation exception with a concise diagnostic.</summary>
        public CelestialCatalogValidationException(string message)
            : base(message)
        {
        }
    }
}
