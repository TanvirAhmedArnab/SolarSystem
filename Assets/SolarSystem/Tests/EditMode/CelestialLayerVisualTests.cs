using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CelestialLayerVisualTests
    {
        private readonly List<UnityEngine.Object> createdObjects =
            new List<UnityEngine.Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (UnityEngine.Object createdObject in createdObjects)
            {
                UnityEngine.Object.DestroyImmediate(createdObject);
            }

            createdObjects.Clear();
        }

        [Test]
        public void Definition_CreatesImmutableValidatedEarthLayerModel()
        {
            CelestialLayerVisualDefinition definition = CreateDefinition();

            CelestialLayerVisualModel model = definition.ToModel();

            Assert.That(model.BodyStableId, Is.EqualTo("earth"));
            Assert.That(
                model.CloudShellRadiusMultiplier,
                Is.EqualTo(EarthLayerRenderingContract.CloudShellRadiusMultiplier));
            Assert.That(
                model.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(EarthLayerRenderingContract.AtmosphereShellRadiusMultiplier));
            Assert.That(
                model.CloudRotationMultiplier,
                Is.EqualTo(EarthLayerRenderingContract.CloudRotationMultiplier));
            Assert.That(definition.BodyStableId, Is.EqualTo("earth"));
        }

        [Test]
        public void Model_RejectsInvalidOrInvertedShellConfiguration()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CelestialLayerVisualModel("earth", 1f, 1.02f, 1.025f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CelestialLayerVisualModel("earth", 1.02f, 1.01f, 1.025f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CelestialLayerVisualModel("earth", 1.004f, 1.018f, 0f));
        }

        [Test]
        public void LayeredView_AppliesRelativeShellScaleAndDeterministicCloudDrift()
        {
            CelestialLayerVisualDefinition definition = CreateDefinition();
            GameObject root = CreateObject("Earth");
            Transform cloud = CreateRendererObject("Cloud Layer", root.transform);
            Transform atmosphere =
                CreateRendererObject("Atmosphere Layer", root.transform);
            MeshRenderer surface = root.AddComponent<MeshRenderer>();
            CelestialLayeredBodyView view =
                root.AddComponent<CelestialLayeredBodyView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("definition").objectReferenceValue = definition;
            serialized.FindProperty("cloudShell").objectReferenceValue = cloud;
            serialized.FindProperty("atmosphereShell").objectReferenceValue = atmosphere;
            serialized.FindProperty("surfaceRenderer").objectReferenceValue = surface;
            serialized.FindProperty("cloudRenderer").objectReferenceValue =
                cloud.GetComponent<MeshRenderer>();
            serialized.FindProperty("atmosphereRenderer").objectReferenceValue =
                atmosphere.GetComponent<MeshRenderer>();
            serialized.ApplyModifiedPropertiesWithoutUndo();
            CelestialBodyModel earth =
                CelestialTestFactory.CreateOrbitingBody("earth", "sun");

            view.Initialize(earth);
            view.Apply(120f);

            Assert.That(view.IsInitialized, Is.True);
            Assert.That(
                cloud.localScale.x,
                Is.EqualTo(EarthLayerRenderingContract.CloudShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                atmosphere.localScale.x,
                Is.EqualTo(EarthLayerRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            float expectedAngle =
                -120f * (EarthLayerRenderingContract.CloudRotationMultiplier - 1f);
            Assert.That(view.CloudRelativeRotationDeg, Is.EqualTo(expectedAngle).Within(0.0001f));
            Assert.That(
                Mathf.DeltaAngle(0f, cloud.localEulerAngles.y),
                Is.EqualTo(expectedAngle).Within(0.0001f));
        }

        [Test]
        public void NightWeight_IsZeroOnDaysideAndOneOnOpposedHemisphere()
        {
            Assert.That(EarthLayerRenderingContract.EvaluateNightWeight(1f), Is.Zero);
            Assert.That(EarthLayerRenderingContract.EvaluateNightWeight(0f), Is.Zero);
            Assert.That(EarthLayerRenderingContract.EvaluateNightWeight(-1f), Is.EqualTo(1f));
        }

        private CelestialLayerVisualDefinition CreateDefinition()
        {
            CelestialLayerVisualDefinition definition =
                ScriptableObject.CreateInstance<CelestialLayerVisualDefinition>();
            createdObjects.Add(definition);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "earth";
            serialized.FindProperty("cloudShellRadiusMultiplier").floatValue =
                EarthLayerRenderingContract.CloudShellRadiusMultiplier;
            serialized.FindProperty("atmosphereShellRadiusMultiplier").floatValue =
                EarthLayerRenderingContract.AtmosphereShellRadiusMultiplier;
            serialized.FindProperty("cloudRotationMultiplier").floatValue =
                EarthLayerRenderingContract.CloudRotationMultiplier;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        private GameObject CreateObject(string name)
        {
            var gameObject = new GameObject(name);
            createdObjects.Add(gameObject);
            return gameObject;
        }

        private Transform CreateRendererObject(string name, Transform parent)
        {
            GameObject gameObject = CreateObject(name);
            gameObject.transform.SetParent(parent, false);
            gameObject.AddComponent<MeshRenderer>();
            return gameObject.transform;
        }
    }
}
