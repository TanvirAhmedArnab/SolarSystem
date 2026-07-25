using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Applies validated presentation data to one airless rocky body.</summary>
    [DisallowMultipleComponent]
    public sealed class AirlessRockyVisualView : MonoBehaviour
    {
        private static readonly int ReliefStrengthId =
            Shader.PropertyToID("_ReliefStrength");
        private static readonly int ReliefSampleDistanceId =
            Shader.PropertyToID("_ReliefSampleDistance");
        private static readonly int SpecularId =
            Shader.PropertyToID("_Specular");
        private static readonly int SmoothnessId =
            Shader.PropertyToID("_Smoothness");
        private static readonly int NightsideReadabilityId =
            Shader.PropertyToID("_NightsideReadability");

        [SerializeField] private AirlessRockyVisualDefinition definition;
        [SerializeField] private MeshRenderer surfaceRenderer;

        private AirlessRockyVisualModel model;
        private MaterialPropertyBlock surfaceProperties;

        /// <summary>Gets whether the view owns validated immutable presentation data.</summary>
        public bool IsInitialized => model != null;

        /// <summary>Gets the authored definition.</summary>
        public AirlessRockyVisualDefinition Definition => definition;

        /// <summary>Gets the physical surface renderer.</summary>
        public MeshRenderer SurfaceRenderer => surfaceRenderer;

        /// <summary>Initializes the adapter against its immutable body model.</summary>
        public void Initialize(CelestialBodyModel body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (definition == null || surfaceRenderer == null)
            {
                throw new InvalidOperationException(
                    $"Airless rocky visual '{name}' has incomplete serialized dependencies.");
            }

            AirlessRockyVisualModel runtimeModel = definition.ToModel();
            if (!string.Equals(
                    runtimeModel.BodyStableId,
                    body.Id.Value,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Airless rocky visual '{name}' expects " +
                    $"'{runtimeModel.BodyStableId}' but received '{body.Id.Value}'.");
            }

            model = runtimeModel;
            surfaceProperties ??= new MaterialPropertyBlock();
            surfaceRenderer.shadowCastingMode = ShadowCastingMode.Off;
            surfaceRenderer.receiveShadows = false;
            surfaceRenderer.lightProbeUsage = LightProbeUsage.Off;
            surfaceRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            surfaceRenderer.GetPropertyBlock(surfaceProperties);
            surfaceProperties.SetFloat(ReliefStrengthId, model.ReliefStrength);
            surfaceProperties.SetFloat(
                ReliefSampleDistanceId,
                model.ReliefSampleDistance);
            surfaceProperties.SetFloat(SpecularId, model.SurfaceSpecular);
            surfaceProperties.SetFloat(SmoothnessId, model.SurfaceSmoothness);
            surfaceProperties.SetFloat(
                NightsideReadabilityId,
                model.NightsideReadability);
            surfaceRenderer.SetPropertyBlock(surfaceProperties);
        }
    }
}
