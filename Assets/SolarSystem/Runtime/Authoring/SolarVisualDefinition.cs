using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Read-only authoring data for a deterministic solar hero treatment.</summary>
    [CreateAssetMenu(
        fileName = "VisualLayers_Sun",
        menuName = "Solar System/Presentation/Solar Visual Definition")]
    public sealed class SolarVisualDefinition : ScriptableObject
    {
        [SerializeField] private string bodyStableId = "sun";
        [SerializeField] private float coronaShellRadiusMultiplier =
            SolarVisualRenderingContract.CoronaShellRadiusMultiplier;
        [SerializeField] private float surfaceFlowCyclesPerRotation =
            SolarVisualRenderingContract.SurfaceFlowCyclesPerRotation;
        [SerializeField] private float coronaFlowCyclesPerRotation =
            SolarVisualRenderingContract.CoronaFlowCyclesPerRotation;

        /// <summary>Gets the stable body ID this presentation belongs to.</summary>
        public string BodyStableId => bodyStableId;

        /// <summary>Gets the authored corona radius multiplier.</summary>
        public float CoronaShellRadiusMultiplier => coronaShellRadiusMultiplier;

        /// <summary>Creates the immutable validated runtime model.</summary>
        public SolarVisualModel ToModel()
        {
            return new SolarVisualModel(
                bodyStableId,
                coronaShellRadiusMultiplier,
                surfaceFlowCyclesPerRotation,
                coronaFlowCyclesPerRotation);
        }
    }
}
