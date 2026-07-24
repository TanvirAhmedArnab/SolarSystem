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
    public sealed class SolarVisualTests
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
        public void Definition_CreatesImmutableValidatedSolarModel()
        {
            SolarVisualDefinition definition = CreateDefinition();

            SolarVisualModel model = definition.ToModel();

            Assert.That(model.BodyStableId, Is.EqualTo("sun"));
            Assert.That(
                model.CoronaShellRadiusMultiplier,
                Is.EqualTo(SolarVisualRenderingContract.CoronaShellRadiusMultiplier));
            Assert.That(
                model.SurfaceFlowCyclesPerRotation,
                Is.EqualTo(SolarVisualRenderingContract.SurfaceFlowCyclesPerRotation));
            Assert.That(
                model.CoronaFlowCyclesPerRotation,
                Is.EqualTo(SolarVisualRenderingContract.CoronaFlowCyclesPerRotation));
        }

        [Test]
        public void Model_RejectsInvalidShellAndMotionConfiguration()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new SolarVisualModel("sun", 1f, 0.125f, 0.05f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new SolarVisualModel("sun", 1.065f, 0f, 0.05f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new SolarVisualModel("sun", 1.065f, 0.125f, float.NaN));
        }

        [Test]
        public void Model_EvaluatesSmoothAbsolutePhasesWithoutFrameDelta()
        {
            SolarVisualModel model = CreateDefinition().ToModel();

            float surface = model.EvaluateSurfacePhase(200d, 100d);
            float corona = model.EvaluateCoronaPhase(200d, 100d);
            float repeatedSurface = model.EvaluateSurfacePhase(1000d, 100d);

            Assert.That(surface, Is.EqualTo(0.25f).Within(0.000001f));
            Assert.That(corona, Is.EqualTo(0.1f).Within(0.000001f));
            Assert.That(repeatedSurface, Is.EqualTo(surface).Within(0.000001f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => model.EvaluateSurfacePhase(double.NaN, 100d));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => model.EvaluateSurfacePhase(100d, 0d));
        }

        [Test]
        public void View_AppliesCoronaScalePhasesAndNonShadowingPolicy()
        {
            SolarVisualDefinition definition = CreateDefinition();
            GameObject root = CreateObject("Sun");
            GameObject surfaceObject = CreateObject("Surface", root.transform);
            GameObject coronaObject = CreateObject("Corona", root.transform);
            MeshRenderer surface = surfaceObject.AddComponent<MeshRenderer>();
            MeshRenderer corona = coronaObject.AddComponent<MeshRenderer>();
            SolarVisualView view = root.AddComponent<SolarVisualView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("definition").objectReferenceValue = definition;
            serialized.FindProperty("coronaShell").objectReferenceValue =
                coronaObject.transform;
            serialized.FindProperty("surfaceRenderer").objectReferenceValue = surface;
            serialized.FindProperty("coronaRenderer").objectReferenceValue = corona;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            CelestialBodyModel sun = CelestialTestFactory.CreateSun(100d);

            view.Initialize(sun);
            view.Apply(200d);

            Assert.That(view.IsInitialized, Is.True);
            Assert.That(
                view.CoronaShell.localScale.x,
                Is.EqualTo(SolarVisualRenderingContract.CoronaShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(view.SurfacePhase, Is.EqualTo(0.25f).Within(0.000001f));
            Assert.That(view.CoronaPhase, Is.EqualTo(0.1f).Within(0.000001f));
            Assert.That(surface.shadowCastingMode, Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(surface.receiveShadows, Is.False);
            Assert.That(corona.shadowCastingMode, Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(corona.receiveShadows, Is.False);

            var properties = new MaterialPropertyBlock();
            surface.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(view.SurfacePhase).Within(0.000001f));
            corona.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(view.CoronaPhase).Within(0.000001f));
        }

        private SolarVisualDefinition CreateDefinition()
        {
            SolarVisualDefinition definition =
                ScriptableObject.CreateInstance<SolarVisualDefinition>();
            createdObjects.Add(definition);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "sun";
            serialized.FindProperty("coronaShellRadiusMultiplier").floatValue =
                SolarVisualRenderingContract.CoronaShellRadiusMultiplier;
            serialized.FindProperty("surfaceFlowCyclesPerRotation").floatValue =
                SolarVisualRenderingContract.SurfaceFlowCyclesPerRotation;
            serialized.FindProperty("coronaFlowCyclesPerRotation").floatValue =
                SolarVisualRenderingContract.CoronaFlowCyclesPerRotation;
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
