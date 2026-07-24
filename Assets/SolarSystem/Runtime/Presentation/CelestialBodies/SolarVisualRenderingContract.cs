namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Reviewed numeric contract shared by solar authoring and validation.</summary>
    public static class SolarVisualRenderingContract
    {
        /// <summary>Corona-shell radius relative to the Sun's physical surface.</summary>
        public const float CoronaShellRadiusMultiplier = 1.045f;

        /// <summary>Surface-flow cycles completed during one signed solar rotation.</summary>
        public const float SurfaceFlowCyclesPerRotation = 0.125f;

        /// <summary>Corona-flow cycles completed during one signed solar rotation.</summary>
        public const float CoronaFlowCyclesPerRotation = 0.05f;

        /// <summary>Small periodic UV displacement used by the surface shader.</summary>
        public const float SurfaceFlowStrength = 0.006f;

        /// <summary>Small periodic UV displacement used by the corona shader.</summary>
        public const float CoronaFlowStrength = 0.012f;

        /// <summary>Restrained authored opacity/intensity ceiling for the corona.</summary>
        public const float CoronaIntensity = 0.22f;
    }
}
