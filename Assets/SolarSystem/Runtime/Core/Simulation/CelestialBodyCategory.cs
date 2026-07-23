namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Describes a celestial body's scientific role in the catalog.</summary>
    public enum CelestialBodyCategory
    {
        /// <summary>A star and catalog root.</summary>
        Star = 0,
        /// <summary>A major planet.</summary>
        Planet = 1,
        /// <summary>A natural satellite.</summary>
        Moon = 2,
        /// <summary>A dwarf planet.</summary>
        DwarfPlanet = 3,
        /// <summary>An asteroid or minor body.</summary>
        Asteroid = 4,
        /// <summary>A comet.</summary>
        Comet = 5,
    }
}
