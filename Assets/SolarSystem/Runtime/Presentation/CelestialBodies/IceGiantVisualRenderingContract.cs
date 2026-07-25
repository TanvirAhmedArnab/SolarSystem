namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>
    /// Reviewed presentation constants for the Uranus and Neptune ice-giant treatment.
    /// </summary>
    /// <remarks>
    /// Detail motion is a bounded readability treatment tied to authoritative signed
    /// body rotation. It is not a wind-speed or atmospheric-fluid simulation.
    /// </remarks>
    public static class IceGiantVisualRenderingContract
    {
        /// <summary>Uranus atmosphere-shell radius relative to its physical surface.</summary>
        public const float UranusAtmosphereShellRadiusMultiplier = 1.009f;

        /// <summary>Uranus presentation-detail cycles per signed rotation.</summary>
        public const float UranusDetailCyclesPerRotation = 0.0002f;

        /// <summary>Uranus maximum longitudinal sample displacement.</summary>
        public const float UranusDetailFlowStrength = 0.0008f;

        /// <summary>Uranus moving-detail contribution to anchored source color.</summary>
        public const float UranusAnimatedDetailStrength = 0.012f;

        /// <summary>Uranus source-derived latitudinal normal strength.</summary>
        public const float UranusBandNormalStrength = 0.07f;

        /// <summary>Uranus restrained atmospheric limb intensity.</summary>
        public const float UranusAtmosphereIntensity = 0.12f;

        /// <summary>Anchored-color visibility floor on Uranus's unlit hemisphere.</summary>
        public const float UranusNightsideReadability = 0.035f;

        /// <summary>Neptune atmosphere-shell radius relative to its physical surface.</summary>
        public const float NeptuneAtmosphereShellRadiusMultiplier = 1.01f;

        /// <summary>Neptune presentation-detail cycles per signed rotation.</summary>
        public const float NeptuneDetailCyclesPerRotation = 0.0009f;

        /// <summary>Neptune maximum longitudinal sample displacement.</summary>
        public const float NeptuneDetailFlowStrength = 0.0018f;

        /// <summary>Neptune moving-detail contribution to anchored source color.</summary>
        public const float NeptuneAnimatedDetailStrength = 0.035f;

        /// <summary>Neptune source-derived latitudinal normal strength.</summary>
        public const float NeptuneBandNormalStrength = 0.16f;

        /// <summary>Neptune restrained atmospheric limb intensity.</summary>
        public const float NeptuneAtmosphereIntensity = 0.17f;

        /// <summary>Anchored-color visibility floor on Neptune's unlit hemisphere.</summary>
        public const float NeptuneNightsideReadability = 0.04f;
    }
}
