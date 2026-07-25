using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Read-only authoring definition for optional celestial visual layers.</summary>
    [CreateAssetMenu(
        fileName = "VisualLayers_Body",
        menuName = "Solar System/Celestial Layer Visual Definition")]
    public sealed class CelestialLayerVisualDefinition : ScriptableObject
    {
        [SerializeField] private string bodyStableId;
        [SerializeField] private bool hasCloudLayer = true;
        [SerializeField, Min(1.0001f)]
        private float cloudShellRadiusMultiplier =
            EarthLayerRenderingContract.CloudShellRadiusMultiplier;
        [SerializeField, Min(1.0002f)]
        private float atmosphereShellRadiusMultiplier =
            EarthLayerRenderingContract.AtmosphereShellRadiusMultiplier;
        [SerializeField, Min(0.0001f)]
        private float cloudRotationMultiplier =
            EarthLayerRenderingContract.CloudRotationMultiplier;
        [SerializeField, Min(0f)]
        private float atmosphereCyclesPerRotation;

        /// <summary>Gets the stable body ID assigned to this definition.</summary>
        public string BodyStableId => bodyStableId;

        /// <summary>Gets whether this body presents a separate visible cloud shell.</summary>
        public bool HasCloudLayer => hasCloudLayer;

        /// <summary>Gets the authored cloud-shell radius multiplier.</summary>
        public float CloudShellRadiusMultiplier => cloudShellRadiusMultiplier;

        /// <summary>Gets the authored atmosphere-shell radius multiplier.</summary>
        public float AtmosphereShellRadiusMultiplier => atmosphereShellRadiusMultiplier;

        /// <summary>Gets the authored total cloud rotation multiplier.</summary>
        public float CloudRotationMultiplier => cloudRotationMultiplier;

        /// <summary>Gets the authored atmosphere-detail cycles per signed body rotation.</summary>
        public float AtmosphereCyclesPerRotation => atmosphereCyclesPerRotation;

        /// <summary>Creates a validated immutable runtime model without mutating this asset.</summary>
        public CelestialLayerVisualModel ToModel()
        {
            return new CelestialLayerVisualModel(
                bodyStableId,
                hasCloudLayer,
                cloudShellRadiusMultiplier,
                atmosphereShellRadiusMultiplier,
                cloudRotationMultiplier,
                atmosphereCyclesPerRotation);
        }
    }
}
