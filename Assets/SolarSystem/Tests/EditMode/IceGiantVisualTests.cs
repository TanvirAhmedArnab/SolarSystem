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
    public sealed class IceGiantVisualTests
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
        public void Definition_CreatesImmutableValidatedIceGiantModel()
        {
            IceGiantVisualDefinition definition = CreateDefinition();

            IceGiantVisualModel model = definition.ToModel();

            Assert.That(model.BodyStableId, Is.EqualTo("uranus"));
            Assert.That(
                model.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(
                    IceGiantVisualRenderingContract
                        .UranusAtmosphereShellRadiusMultiplier));
            Assert.That(
                model.DetailCyclesPerRotation,
                Is.EqualTo(
                    IceGiantVisualRenderingContract
                        .UranusDetailCyclesPerRotation));
        }

        [Test]
        public void Model_RejectsInvalidShellAndMotionConfiguration()
        {
            Assert.Throws<ArgumentException>(
                () => new IceGiantVisualModel(" ", 1.009f, 0.0002f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new IceGiantVisualModel("uranus", 1f, 0.0002f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new IceGiantVisualModel("uranus", 1.009f, 0f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new IceGiantVisualModel("uranus", 1.009f, float.NaN));
        }

        [Test]
        public void Model_UsesAbsoluteTimeAndPreservesRetrogradeDirection()
        {
            IceGiantVisualModel model = CreateDefinition().ToModel();

            float prograde = model.EvaluateDetailPhase(200d, 100d);
            float repeated = model.EvaluateDetailPhase(200d, 100d);
            float retrograde = model.EvaluateDetailPhase(200d, -100d);

            Assert.That(prograde, Is.EqualTo(0.0004f).Within(0.000001f));
            Assert.That(repeated, Is.EqualTo(prograde).Within(0.000001f));
            Assert.That(retrograde, Is.EqualTo(0.9996f).Within(0.000001f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => model.EvaluateDetailPhase(double.PositiveInfinity, 100d));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => model.EvaluateDetailPhase(100d, 0d));
        }

        [Test]
        public void View_AppliesShellPhaseAndRendererPolicy()
        {
            IceGiantVisualDefinition definition = CreateDefinition();
            GameObject root = CreateObject("Uranus");
            GameObject surfaceObject = CreateObject("Surface", root.transform);
            GameObject atmosphereObject =
                CreateObject("Atmosphere", root.transform);
            MeshRenderer surface = surfaceObject.AddComponent<MeshRenderer>();
            MeshRenderer atmosphere =
                atmosphereObject.AddComponent<MeshRenderer>();
            IceGiantVisualView view = root.AddComponent<IceGiantVisualView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("definition").objectReferenceValue = definition;
            serialized.FindProperty("atmosphereShell").objectReferenceValue =
                atmosphereObject.transform;
            serialized.FindProperty("surfaceRenderer").objectReferenceValue =
                surface;
            serialized.FindProperty("atmosphereRenderer").objectReferenceValue =
                atmosphere;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            CelestialBodyModel uranus = CelestialTestFactory.CreateOrbitingBody(
                "uranus",
                "sun",
                rotationPeriodSeconds: -100d);

            view.Initialize(uranus);
            view.Apply(200d);

            Assert.That(view.IsInitialized, Is.True);
            Assert.That(
                view.AtmosphereShell.localScale.x,
                Is.EqualTo(
                    IceGiantVisualRenderingContract
                        .UranusAtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(view.DetailPhase, Is.EqualTo(0.9996f).Within(0.000001f));
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
                Is.EqualTo(view.DetailPhase).Within(0.000001f));
        }

        private IceGiantVisualDefinition CreateDefinition()
        {
            IceGiantVisualDefinition definition =
                ScriptableObject.CreateInstance<IceGiantVisualDefinition>();
            createdObjects.Add(definition);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "uranus";
            serialized.FindProperty("atmosphereShellRadiusMultiplier").floatValue =
                IceGiantVisualRenderingContract
                    .UranusAtmosphereShellRadiusMultiplier;
            serialized.FindProperty("detailCyclesPerRotation").floatValue =
                IceGiantVisualRenderingContract.UranusDetailCyclesPerRotation;
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
