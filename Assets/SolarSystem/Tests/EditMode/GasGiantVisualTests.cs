using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class GasGiantVisualTests
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
        public void Definition_CreatesImmutableValidatedGasGiantModel()
        {
            GasGiantVisualDefinition definition = CreateDefinition();

            GasGiantVisualModel model = definition.ToModel();

            Assert.That(model.BodyStableId, Is.EqualTo("jupiter"));
            Assert.That(
                model.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(
                    GasGiantVisualRenderingContract.AtmosphereShellRadiusMultiplier));
            Assert.That(
                model.BandFlowCyclesPerRotation,
                Is.EqualTo(
                    GasGiantVisualRenderingContract.BandFlowCyclesPerRotation));
        }

        [Test]
        public void Model_RejectsInvalidShellAndMotionConfiguration()
        {
            Assert.Throws<ArgumentException>(
                () => new GasGiantVisualModel(" ", 1.01f, 0.0015f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new GasGiantVisualModel("jupiter", 1f, 0.0015f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new GasGiantVisualModel("jupiter", 1.01f, 0f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new GasGiantVisualModel("jupiter", 1.01f, float.NaN));
        }

        [Test]
        public void Model_EvaluatesSignedAbsoluteBandPhaseWithoutFrameDelta()
        {
            GasGiantVisualModel model = CreateDefinition().ToModel();

            float first = model.EvaluateBandPhase(200d, 100d);
            float repeated = model.EvaluateBandPhase(200d, 100d);
            float later = model.EvaluateBandPhase(400d, 100d);
            float retrograde = model.EvaluateBandPhase(200d, -100d);

            Assert.That(first, Is.EqualTo(0.003f).Within(0.000001f));
            Assert.That(repeated, Is.EqualTo(first).Within(0.000001f));
            Assert.That(later, Is.EqualTo(0.006f).Within(0.000001f));
            Assert.That(retrograde, Is.EqualTo(0.997f).Within(0.000001f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => model.EvaluateBandPhase(double.PositiveInfinity, 100d));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => model.EvaluateBandPhase(100d, 0d));
        }

        [Test]
        public void View_AppliesShellScalePhaseAndRendererPolicy()
        {
            GasGiantVisualDefinition definition = CreateDefinition();
            GameObject root = CreateObject("Jupiter");
            GameObject surfaceObject = CreateObject("Surface", root.transform);
            GameObject atmosphereObject =
                CreateObject("Atmosphere", root.transform);
            MeshRenderer surface = surfaceObject.AddComponent<MeshRenderer>();
            MeshRenderer atmosphere =
                atmosphereObject.AddComponent<MeshRenderer>();
            GasGiantVisualView view = root.AddComponent<GasGiantVisualView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("definition").objectReferenceValue = definition;
            serialized.FindProperty("atmosphereShell").objectReferenceValue =
                atmosphereObject.transform;
            serialized.FindProperty("surfaceRenderer").objectReferenceValue =
                surface;
            serialized.FindProperty("atmosphereRenderer").objectReferenceValue =
                atmosphere;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            CelestialBodyModel jupiter = CelestialTestFactory.CreateOrbitingBody(
                "jupiter",
                "sun",
                rotationPeriodSeconds: 100d);

            view.Initialize(jupiter);
            view.Apply(200d);

            Assert.That(view.IsInitialized, Is.True);
            Assert.That(
                view.AtmosphereShell.localScale.x,
                Is.EqualTo(
                    GasGiantVisualRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(view.BandPhase, Is.EqualTo(0.003f).Within(0.000001f));
            Assert.That(
                surface.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(surface.receiveShadows, Is.False);
            Assert.That(
                atmosphere.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(atmosphere.receiveShadows, Is.False);
            Assert.That(
                atmosphere.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                atmosphere.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));

            var properties = new MaterialPropertyBlock();
            surface.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(view.BandPhase).Within(0.000001f));
        }

        private GasGiantVisualDefinition CreateDefinition()
        {
            GasGiantVisualDefinition definition =
                ScriptableObject.CreateInstance<GasGiantVisualDefinition>();
            createdObjects.Add(definition);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "jupiter";
            serialized.FindProperty("atmosphereShellRadiusMultiplier").floatValue =
                GasGiantVisualRenderingContract.AtmosphereShellRadiusMultiplier;
            serialized.FindProperty("bandFlowCyclesPerRotation").floatValue =
                GasGiantVisualRenderingContract.BandFlowCyclesPerRotation;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        private GameObject CreateObject(string name, Transform parent = null)
        {
            var gameObject = new GameObject(name);
            if (parent == null)
            {
                createdObjects.Add(gameObject);
            }
            else
            {
                gameObject.transform.SetParent(parent, false);
            }

            return gameObject;
        }
    }
}
