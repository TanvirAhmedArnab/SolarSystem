using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Read-only authoring data for one deterministic ice-giant treatment.</summary>
    [CreateAssetMenu(
        fileName = "VisualLayers_IceGiant",
        menuName = "Solar System/Presentation/Ice Giant Visual Definition")]
    public sealed class IceGiantVisualDefinition : ScriptableObject
    {
        [SerializeField] private string bodyStableId = "uranus";
        [SerializeField, Min(1.0001f)]
        private float atmosphereShellRadiusMultiplier =
            IceGiantVisualRenderingContract.UranusAtmosphereShellRadiusMultiplier;
        [SerializeField, Min(0.000001f)]
        private float detailCyclesPerRotation =
            IceGiantVisualRenderingContract.UranusDetailCyclesPerRotation;

        /// <summary>Gets the stable body ID this presentation belongs to.</summary>
        public string BodyStableId => bodyStableId;

        /// <summary>Gets the authored atmosphere radius multiplier.</summary>
        public float AtmosphereShellRadiusMultiplier =>
            atmosphereShellRadiusMultiplier;

        /// <summary>Gets the authored presentation-detail cycles per signed rotation.</summary>
        public float DetailCyclesPerRotation => detailCyclesPerRotation;

        /// <summary>Creates the immutable validated runtime model.</summary>
        public IceGiantVisualModel ToModel()
        {
            return new IceGiantVisualModel(
                bodyStableId,
                atmosphereShellRadiusMultiplier,
                detailCyclesPerRotation);
        }
    }
}
