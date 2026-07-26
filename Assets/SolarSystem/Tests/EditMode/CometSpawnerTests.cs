using System;
using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.TransientBodies;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CometSpawnerTests
    {
        private const string DefinitionPath =
            "Assets/SolarSystem/Content/Data/TransientBodies/DEF_CometSpawner.asset";
        private const string PrefabPath =
            "Assets/SolarSystem/Content/Prefabs/TransientBodies/PF_Comet.prefab";

        [Test]
        public void AuthoredDefinition_BuildsBoundedPoolAndLaunchSettings()
        {
            CometSpawnerDefinition definition =
                AssetDatabase.LoadAssetAtPath<CometSpawnerDefinition>(DefinitionPath);
            Assert.That(definition, Is.Not.Null);

            CometSpawnerModel model = definition.ToModel();
            Assert.That(model.PoolSize, Is.EqualTo(6));
            Assert.That(model.OrbitRadius, Is.EqualTo(680f).Within(0.001f));
            Assert.That(model.SolarDespawnRadius, Is.GreaterThan(model.OrbitRadius));
            Assert.That(
                model.MaximumSpawnIntervalSeconds,
                Is.GreaterThanOrEqualTo(model.MinimumSpawnIntervalSeconds));
            Assert.That(model.TargetRadius, Is.LessThan(model.OrbitRadius));
        }

        [Test]
        public void DeterministicSequence_RepeatsIntervalsAndLaunchPlans()
        {
            CometSpawnerModel model = CreateModel();
            var first = new DeterministicCometSpawnSequence(model.RandomSeed);
            var second = new DeterministicCometSpawnSequence(model.RandomSeed);
            Vector3 spawn = new Vector3(680f, 0f, 0f);

            for (int index = 0; index < 12; index++)
            {
                Assert.That(
                    first.NextInterval(model),
                    Is.EqualTo(second.NextInterval(model)).Within(0.000001f));
                CometSpawnPlan firstPlan =
                    first.NextPlan(model, spawn, Vector3.zero);
                CometSpawnPlan secondPlan =
                    second.NextPlan(model, spawn, Vector3.zero);
                Assert.That(firstPlan.Position, Is.EqualTo(secondPlan.Position));
                Assert.That(firstPlan.Velocity, Is.EqualTo(secondPlan.Velocity));
                Assert.That(firstPlan.Radius, Is.EqualTo(secondPlan.Radius));
                Assert.That(firstPlan.SpinAxis, Is.EqualTo(secondPlan.SpinAxis));
                Assert.That(
                    firstPlan.SpinDegreesPerSecond,
                    Is.EqualTo(secondPlan.SpinDegreesPerSecond));
            }
        }

        [Test]
        public void DeterministicSequence_StaysInsideAuthoredRanges()
        {
            CometSpawnerModel model = CreateModel();
            var sequence = new DeterministicCometSpawnSequence(model.RandomSeed);
            Vector3 spawn = new Vector3(680f, 0f, 0f);

            for (int index = 0; index < 128; index++)
            {
                float interval = sequence.NextInterval(model);
                Assert.That(
                    interval,
                    Is.InRange(
                        model.MinimumSpawnIntervalSeconds,
                        model.MaximumSpawnIntervalSeconds));
                CometSpawnPlan plan =
                    sequence.NextPlan(model, spawn, Vector3.zero);
                Assert.That(
                    plan.Velocity.magnitude,
                    Is.InRange(model.MinimumSpeed, model.MaximumSpeed));
                Assert.That(
                    plan.Radius,
                    Is.InRange(model.MinimumRadius, model.MaximumRadius));
                Assert.That(plan.SpinAxis.magnitude, Is.EqualTo(1f).Within(0.0001f));
                Assert.That(
                    plan.SpinDegreesPerSecond,
                    Is.InRange(
                        model.MinimumSpinDegreesPerSecond,
                        model.MaximumSpinDegreesPerSecond));

                Vector3 closestPoint =
                    ClosestPointOnRayToOrigin(spawn, plan.Velocity.normalized);
                Assert.That(
                    closestPoint.magnitude,
                    Is.LessThanOrEqualTo(model.TargetRadius + 0.001f));
            }
        }

        [Test]
        public void DespawnPolicy_RequiresExpiryOrOffscreenRangeExit()
        {
            Vector3 visible = new Vector3(0.5f, 0.5f, 100f);
            Vector3 offscreen = new Vector3(1.5f, 0.5f, 100f);

            Assert.That(
                CometDespawnPolicy.ShouldDespawn(
                    1f,
                    45f,
                    visible,
                    1900f,
                    2000f,
                    950f,
                    900f,
                    0.15f),
                Is.False,
                "A visible comet remains active even near a distance boundary.");
            Assert.That(
                CometDespawnPolicy.ShouldDespawn(
                    1f,
                    45f,
                    offscreen,
                    1000f,
                    2000f,
                    950f,
                    900f,
                    0.15f),
                Is.True,
                "An offscreen comet beyond the authored system envelope despawns.");
            Assert.That(
                CometDespawnPolicy.ShouldDespawn(
                    45f,
                    45f,
                    visible,
                    100f,
                    2000f,
                    100f,
                    900f,
                    0.15f),
                Is.True,
                "The lifetime bound prevents pooled instances from remaining active forever.");
        }

        [Test]
        public void AuthoredPrefab_HasNucleusTrailAndNoCollider()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.That(prefab, Is.Not.Null);
            CometView view = prefab.GetComponent<CometView>();
            Assert.That(view, Is.Not.Null);
            Assert.That(view.NucleusRenderer, Is.Not.Null);
            Assert.That(view.TrailRenderer, Is.Not.Null);
            Assert.That(
                view.NucleusRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Transient/Comet Nucleus"));
            Assert.That(view.TrailRenderer.sharedMaterial, Is.Not.Null);
            Assert.That(
                view.TrailRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Transient/Comet Trail"));
            Assert.That(view.TrailRenderer.time, Is.EqualTo(2.8f).Within(0.001f));
            Assert.That(
                view.TrailRenderer.widthMultiplier,
                Is.EqualTo(1.3f).Within(0.001f));
            Assert.That(prefab.GetComponentsInChildren<Collider>(true), Is.Empty);
        }

        [Test]
        public void InvalidModel_RejectsSolarBoundaryInsideSpawnerOrbit()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CometSpawnerModel(
                    1,
                    2,
                    0f,
                    1f,
                    2f,
                    100f,
                    10f,
                    0f,
                    20f,
                    10f,
                    20f,
                    1f,
                    2f,
                    10f,
                    20f,
                    30f,
                    90f,
                    0.1f));
        }

        private static CometSpawnerModel CreateModel()
        {
            return new CometSpawnerModel(
                20260726,
                6,
                2f,
                8f,
                14f,
                680f,
                240f,
                14f,
                100f,
                40f,
                58f,
                1.2f,
                2f,
                30f,
                90f,
                45f,
                900f,
                0.15f);
        }

        private static Vector3 ClosestPointOnRayToOrigin(
            Vector3 origin,
            Vector3 direction)
        {
            float distance = Mathf.Max(0f, Vector3.Dot(-origin, direction));
            return origin + (direction * distance);
        }
    }
}
