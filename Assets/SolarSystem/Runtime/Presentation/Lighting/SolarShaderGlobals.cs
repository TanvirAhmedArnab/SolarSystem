using System;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Lighting
{
    /// <summary>Publishes the presentation-space Sun position once for all solar-aware shaders.</summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(900)]
    public sealed class SolarShaderGlobals : MonoBehaviour
    {
        /// <summary>Global shader property used by solar-aware materials.</summary>
        public const string SunPositionProperty = "_SolarSystemSunPositionWS";

        private static readonly int SunPositionId =
            Shader.PropertyToID(SunPositionProperty);

        [SerializeField] private Transform sunSource;

        /// <summary>Gets the transform supplying the presentation-space Sun position.</summary>
        public Transform SunSource => sunSource;

        /// <summary>Gets whether the publisher has a valid source.</summary>
        public bool IsInitialized => sunSource != null;

        /// <summary>Assigns the source explicitly for runtime composition or tests.</summary>
        public void Initialize(Transform source)
        {
            sunSource = source != null
                ? source
                : throw new ArgumentNullException(nameof(source));
            Publish();
        }

        /// <summary>Publishes the current Sun position immediately.</summary>
        public void Publish()
        {
            if (sunSource == null)
            {
                throw new InvalidOperationException(
                    $"Solar shader globals on '{name}' require a Sun source.");
            }

            Vector3 position = sunSource.position;
            Shader.SetGlobalVector(
                SunPositionId,
                new Vector4(position.x, position.y, position.z, 1f));
        }

        private void OnEnable()
        {
            if (sunSource != null)
            {
                Publish();
            }
        }

        private void LateUpdate()
        {
            Publish();
        }
    }
}
