namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Reviewed numeric contract for reusable gas-giant presentation.</summary>
    public static class GasGiantVisualRenderingContract
    {
        /// <summary>Atmosphere-shell radius relative to the physical surface.</summary>
        public const float AtmosphereShellRadiusMultiplier = 1.01f;

        /// <summary>Animated band-detail cycles completed per signed body rotation.</summary>
        public const float BandFlowCyclesPerRotation = 0.0015f;

        /// <summary>Maximum periodic longitudinal sample displacement.</summary>
        public const float BandFlowStrength = 0.0035f;

        /// <summary>Contribution of the moving detail sample to anchored source color.</summary>
        public const float AnimatedDetailStrength = 0.08f;

        /// <summary>Source-derived latitudinal normal strength.</summary>
        public const float BandNormalStrength = 0.42f;

        /// <summary>Restrained atmospheric limb intensity.</summary>
        public const float AtmosphereIntensity = 0.2f;

        /// <summary>Saturn atmosphere-shell radius relative to its physical surface.</summary>
        public const float SaturnAtmosphereShellRadiusMultiplier = 1.008f;

        /// <summary>Saturn band-detail cycles completed per signed body rotation.</summary>
        public const float SaturnBandFlowCyclesPerRotation = 0.0008f;

        /// <summary>Saturn's maximum periodic longitudinal sample displacement.</summary>
        public const float SaturnBandFlowStrength = 0.0018f;

        /// <summary>Saturn's moving-detail contribution to anchored source color.</summary>
        public const float SaturnAnimatedDetailStrength = 0.035f;

        /// <summary>Saturn's source-derived latitudinal normal strength.</summary>
        public const float SaturnBandNormalStrength = 0.22f;

        /// <summary>Saturn's restrained atmospheric limb intensity.</summary>
        public const float SaturnAtmosphereIntensity = 0.14f;
    }
}
