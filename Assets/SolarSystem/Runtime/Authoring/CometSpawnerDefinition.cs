using Tanvir.SolarSystem.Presentation.TransientBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>
    /// Stores bounded, non-scientific transient-comet presentation settings.
    /// </summary>
    [CreateAssetMenu(
        fileName = "CometSpawner",
        menuName = "Solar System/Presentation/Comet Spawner")]
    public sealed class CometSpawnerDefinition : ScriptableObject
    {
        [Header("Determinism and pooling")]
        [SerializeField] private int randomSeed = 20260726;
        [SerializeField, Min(1)] private int poolSize = 6;

        [Header("Timing")]
        [SerializeField, Min(0f)] private float initialSpawnDelaySeconds = 2f;
        [SerializeField] private Vector2 spawnIntervalSeconds = new Vector2(8f, 14f);

        [Header("Spawner orbit")]
        [SerializeField, Min(0.01f)] private float orbitRadius = 680f;
        [SerializeField, Min(0.01f)] private float orbitPeriodSimulationDays = 240f;
        [SerializeField] private float orbitInclinationDegrees = 14f;

        [Header("Launch envelope")]
        [SerializeField, Min(0f)] private float targetRadius = 100f;
        [SerializeField] private Vector2 speedRange = new Vector2(40f, 58f);
        [SerializeField] private Vector2 radiusRange = new Vector2(1.2f, 2f);
        [SerializeField] private Vector2 spinRange = new Vector2(30f, 90f);

        [Header("Culling")]
        [SerializeField, Min(0.01f)] private float maximumLifetimeSeconds = 45f;
        [SerializeField, Min(0.01f)] private float solarDespawnRadius = 900f;
        [SerializeField, Min(0f)] private float viewportMargin = 0.15f;

        /// <summary>Builds an immutable validated runtime model.</summary>
        public CometSpawnerModel ToModel()
        {
            return new CometSpawnerModel(
                randomSeed,
                poolSize,
                initialSpawnDelaySeconds,
                spawnIntervalSeconds.x,
                spawnIntervalSeconds.y,
                orbitRadius,
                orbitPeriodSimulationDays,
                orbitInclinationDegrees,
                targetRadius,
                speedRange.x,
                speedRange.y,
                radiusRange.x,
                radiusRange.y,
                spinRange.x,
                spinRange.y,
                maximumLifetimeSeconds,
                solarDespawnRadius,
                viewportMargin);
        }
    }
}
