namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Reviewed numeric contract for Saturn's presentation-scale ring rendering.</summary>
    public static class SaturnRingRenderingContract
    {
        /// <summary>Maximum authored ring opacity before source-alpha modulation.</summary>
        public const float Opacity = 0.9f;

        /// <summary>Minimum visible brightness on either ring face.</summary>
        public const float AmbientBrightness = 0.12f;

        /// <summary>Brightness when a ring face is aligned with the Sun.</summary>
        public const float DayBrightness = 1.05f;

        /// <summary>Restrained grazing-angle visibility contribution.</summary>
        public const float ScatteringStrength = 0.14f;
    }
}
