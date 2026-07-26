using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanvir.SolarSystem.Presentation.TransientBodies
{
    /// <summary>
    /// Moves one collider-free comet and returns it to its owner when culled.
    /// </summary>
    [DefaultExecutionOrder(110)]
    [DisallowMultipleComponent]
    public sealed class CometView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer nucleusRenderer;
        [SerializeField] private TrailRenderer trailRenderer;

        private CometSpawner owner;
        private Vector3 velocity;
        private Vector3 spinAxis;
        private float spinDegreesPerSecond;
        private float ageSeconds;

        /// <summary>Gets the authored nucleus renderer.</summary>
        public MeshRenderer NucleusRenderer => nucleusRenderer;

        /// <summary>Gets the authored trail renderer.</summary>
        public TrailRenderer TrailRenderer => trailRenderer;

        /// <summary>Gets whether this pooled instance is currently active.</summary>
        public bool IsActive { get; private set; }

        /// <summary>Gets the current world-space velocity.</summary>
        public Vector3 Velocity => velocity;

        /// <summary>Gets the active age in real seconds.</summary>
        public float AgeSeconds => ageSeconds;

        /// <summary>Associates this pooled instance with its single owner.</summary>
        public void Initialize(CometSpawner cometSpawner)
        {
            owner = cometSpawner ??
                throw new ArgumentNullException(nameof(cometSpawner));
            if (nucleusRenderer == null || trailRenderer == null)
            {
                throw new InvalidOperationException(
                    "Comet view requires a nucleus renderer and trail renderer.");
            }

            if (GetComponentInChildren<Collider>(true) != null)
            {
                throw new InvalidOperationException(
                    "Transient comets must remain collider-free.");
            }

            nucleusRenderer.shadowCastingMode = ShadowCastingMode.Off;
            nucleusRenderer.receiveShadows = false;
            trailRenderer.shadowCastingMode = ShadowCastingMode.Off;
            trailRenderer.receiveShadows = false;
            Deactivate();
        }

        /// <summary>Activates this pooled view from an immutable launch plan.</summary>
        public void Activate(CometSpawnPlan plan)
        {
            if (owner == null)
            {
                throw new InvalidOperationException(
                    "Comet view must be initialized before activation.");
            }

            transform.position = plan.Position;
            transform.localScale = Vector3.one * (plan.Radius * 2f);
            velocity = plan.Velocity;
            spinAxis = plan.SpinAxis.sqrMagnitude > 0f
                ? plan.SpinAxis.normalized
                : Vector3.up;
            spinDegreesPerSecond = plan.SpinDegreesPerSecond;
            ageSeconds = 0f;
            gameObject.SetActive(true);
            trailRenderer.Clear();
            trailRenderer.emitting = true;
            IsActive = true;
        }

        /// <summary>Returns this view to its inactive pooled state.</summary>
        public void Deactivate()
        {
            IsActive = false;
            velocity = Vector3.zero;
            ageSeconds = 0f;
            if (trailRenderer != null)
            {
                trailRenderer.emitting = false;
                trailRenderer.Clear();
            }

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!IsActive || owner == null || owner.IsSimulationPaused)
            {
                return;
            }

            float deltaTime = Time.unscaledDeltaTime;
            ageSeconds += deltaTime;
            transform.position += velocity * deltaTime;
            transform.Rotate(
                spinAxis,
                spinDegreesPerSecond * deltaTime,
                Space.World);

            if (owner.ShouldDespawn(this))
            {
                owner.Despawn(this);
            }
        }
    }
}
