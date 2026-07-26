using System;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.TransientBodies
{
    /// <summary>
    /// Orbits a presentation spawn point around the Sun and launches a
    /// deterministic sequence of collider-free pooled comets.
    /// </summary>
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    public sealed class CometSpawner : MonoBehaviour
    {
        private const double SecondsPerDay = 86400d;

        [SerializeField] private CometSpawnerDefinition definition;
        [SerializeField] private Transform sun;
        [SerializeField] private UnityEngine.Camera visibilityCamera;
        [SerializeField] private SolarSystemSimulationController simulationController;
        [SerializeField] private CometView cometPrefab;
        [SerializeField] private Transform poolRoot;

        private CometSpawnerModel model;
        private DeterministicCometSpawnSequence sequence;
        private CometView[] pool = Array.Empty<CometView>();
        private float spawnCountdownSeconds;

        /// <summary>Gets whether all dependencies and pooled instances are ready.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Gets the number of reusable comet instances.</summary>
        public int PoolCount => pool.Length;

        /// <summary>Gets the number of currently active comets.</summary>
        public int ActiveCount
        {
            get
            {
                int count = 0;
                foreach (CometView comet in pool)
                {
                    if (comet != null && comet.IsActive)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>Gets the number of successful launches this session.</summary>
        public int TotalSpawnedCount { get; private set; }

        /// <summary>Gets whether authoritative simulation time is paused.</summary>
        public bool IsSimulationPaused =>
            !IsInitialized || simulationController.ClockSnapshot.IsPaused;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>Builds the reusable pool and initializes the spawn sequence.</summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            if (definition == null)
            {
                throw new InvalidOperationException(
                    "Comet spawner requires an authored definition.");
            }

            if (sun == null ||
                visibilityCamera == null ||
                simulationController == null ||
                cometPrefab == null ||
                poolRoot == null)
            {
                throw new InvalidOperationException(
                    "Comet spawner requires the Sun, camera, simulation, prefab, and pool root.");
            }

            if (cometPrefab.GetComponentInChildren<Collider>(true) != null)
            {
                throw new InvalidOperationException(
                    "Comet prefab must not contain a collider.");
            }

            model = definition.ToModel();
            sequence = new DeterministicCometSpawnSequence(model.RandomSeed);
            pool = new CometView[model.PoolSize];
            for (int index = 0; index < pool.Length; index++)
            {
                CometView comet = Instantiate(cometPrefab, poolRoot);
                comet.name = $"Comet {index + 1:00}";
                comet.Initialize(this);
                pool[index] = comet;
            }

            spawnCountdownSeconds = model.InitialSpawnDelaySeconds;
            TotalSpawnedCount = 0;
            IsInitialized = true;
            UpdateOrbit(simulationController.ClockSnapshot);
        }

        /// <summary>
        /// Launches one comet immediately when a pooled instance is available.
        /// </summary>
        public bool TrySpawnComet()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    "Comet spawner must be initialized before spawning.");
            }

            CometView available = null;
            foreach (CometView comet in pool)
            {
                if (!comet.IsActive)
                {
                    available = comet;
                    break;
                }
            }

            if (available == null)
            {
                return false;
            }

            CometSpawnPlan plan = sequence.NextPlan(
                model,
                transform.position,
                sun.position);
            available.Activate(plan);
            TotalSpawnedCount++;
            return true;
        }

        /// <summary>Returns all active comets to the reusable pool.</summary>
        public void DespawnAll()
        {
            foreach (CometView comet in pool)
            {
                if (comet != null && comet.IsActive)
                {
                    comet.Deactivate();
                }
            }
        }

        internal bool ShouldDespawn(CometView comet)
        {
            Vector3 position = comet.transform.position;
            Vector3 viewport = visibilityCamera.WorldToViewportPoint(position);
            return CometDespawnPolicy.ShouldDespawn(
                comet.AgeSeconds,
                model.MaximumLifetimeSeconds,
                viewport,
                Vector3.Distance(visibilityCamera.transform.position, position),
                visibilityCamera.farClipPlane,
                Vector3.Distance(sun.position, position),
                model.SolarDespawnRadius,
                model.ViewportMargin);
        }

        internal void Despawn(CometView comet)
        {
            if (comet != null)
            {
                comet.Deactivate();
            }
        }

        private void Update()
        {
            if (!IsInitialized)
            {
                return;
            }

            SimulationClockSnapshot snapshot = simulationController.ClockSnapshot;
            UpdateOrbit(snapshot);
            if (snapshot.IsPaused)
            {
                return;
            }

            spawnCountdownSeconds -= Time.unscaledDeltaTime;
            if (spawnCountdownSeconds > 0f)
            {
                return;
            }

            TrySpawnComet();
            spawnCountdownSeconds = sequence.NextInterval(model);
        }

        private void UpdateOrbit(SimulationClockSnapshot snapshot)
        {
            double periodSeconds =
                model.OrbitPeriodSimulationDays * SecondsPerDay;
            float angleDegrees = (float)(
                (snapshot.ElapsedSimulationTimeSeconds / periodSeconds) *
                360d);
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            Vector3 flatOffset = new Vector3(
                Mathf.Cos(angleRadians) * model.OrbitRadius,
                0f,
                Mathf.Sin(angleRadians) * model.OrbitRadius);
            Quaternion inclination = Quaternion.AngleAxis(
                model.OrbitInclinationDegrees,
                Vector3.forward);
            Vector3 offset = inclination * flatOffset;
            transform.position = sun.position + offset;

            Vector3 tangent = inclination * new Vector3(
                -Mathf.Sin(angleRadians),
                0f,
                Mathf.Cos(angleRadians));
            transform.rotation = Quaternion.LookRotation(
                tangent,
                offset.normalized);
        }
    }
}
