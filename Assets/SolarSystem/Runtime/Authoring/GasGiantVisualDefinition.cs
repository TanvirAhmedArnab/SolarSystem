using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Read-only authoring data for a deterministic gas-giant treatment.</summary>
    [CreateAssetMenu(
        fileName = "VisualLayers_GasGiant",
        menuName = "Solar System/Presentation/Gas Giant Visual Definition")]
    public sealed class GasGiantVisualDefinition : ScriptableObject
    {
        [SerializeField] private string bodyStableId = "jupiter";
        [SerializeField, Min(1.0001f)]
        private float atmosphereShellRadiusMultiplier =
            GasGiantVisualRenderingContract.AtmosphereShellRadiusMultiplier;
        [SerializeField, Min(0.000001f)]
        private float bandFlowCyclesPerRotation =
            GasGiantVisualRenderingContract.BandFlowCyclesPerRotation;

        /// <summary>Gets the stable body ID this presentation belongs to.</summary>
        public string BodyStableId => bodyStableId;

        /// <summary>Gets the authored atmosphere radius multiplier.</summary>
        public float AtmosphereShellRadiusMultiplier =>
            atmosphereShellRadiusMultiplier;

        /// <summary>Gets the authored band-flow cycles per signed rotation.</summary>
        public float BandFlowCyclesPerRotation => bandFlowCyclesPerRotation;

        /// <summary>Creates the immutable validated runtime model.</summary>
        public GasGiantVisualModel ToModel()
        {
            return new GasGiantVisualModel(
                bodyStableId,
                atmosphereShellRadiusMultiplier,
                bandFlowCyclesPerRotation);
        }
    }
}
