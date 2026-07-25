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
    public sealed class AirlessRockyVisualTests
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
        public void Definition_CreatesImmutableValidatedMercuryModel()
        {
            AirlessRockyVisualDefinition definition =
                CreateMercuryDefinition();

            AirlessRockyVisualModel model = definition.ToModel();

            Assert.That(model.BodyStableId, Is.EqualTo("mercury"));
            Assert.That(
                model.ReliefStrength,
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract.MercuryReliefStrength));
            Assert.That(
                model.ReliefSampleDistance,
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .MercuryReliefSampleDistance));
            Assert.That(
                model.NightsideReadability,
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .MercuryNightsideReadability));
        }

        [Test]
        public void Model_RejectsInvalidIdentityAndPresentationRanges()
        {
            Assert.Throws<ArgumentException>(
                () => new AirlessRockyVisualModel(
                    " ",
                    0.2f,
                    1f,
                    0.02f,
                    0.1f,
                    0.02f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new AirlessRockyVisualModel(
                    "moon",
                    1.1f,
                    1f,
                    0.02f,
                    0.1f,
                    0.02f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new AirlessRockyVisualModel(
                    "moon",
                    0.2f,
                    0.4f,
                    0.02f,
                    0.1f,
                    0.02f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new AirlessRockyVisualModel(
                    "moon",
                    0.2f,
                    1f,
                    float.NaN,
                    0.1f,
                    0.02f));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new AirlessRockyVisualModel(
                    "moon",
                    0.2f,
                    1f,
                    0.02f,
                    0.1f,
                    0.11f));
        }

        [Test]
        public void View_AppliesValidatedPropertiesAndRendererPolicy()
        {
            AirlessRockyVisualDefinition definition =
                CreateMercuryDefinition();
            GameObject root = CreateObject("Mercury");
            GameObject surfaceObject = CreateObject("Surface", root.transform);
            MeshRenderer surface = surfaceObject.AddComponent<MeshRenderer>();
            AirlessRockyVisualView view =
                root.AddComponent<AirlessRockyVisualView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("definition").objectReferenceValue =
                definition;
            serialized.FindProperty("surfaceRenderer").objectReferenceValue =
                surface;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            CelestialBodyModel mercury = CelestialTestFactory.CreateOrbitingBody(
                "mercury",
                "sun");

            view.Initialize(mercury);

            Assert.That(view.IsInitialized, Is.True);
            Assert.That(view.Definition, Is.SameAs(definition));
            Assert.That(view.SurfaceRenderer, Is.SameAs(surface));
            Assert.That(
                surface.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(surface.receiveShadows, Is.False);
            Assert.That(
                surface.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                surface.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));

            var properties = new MaterialPropertyBlock();
            surface.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .MercuryReliefStrength));
            Assert.That(
                properties.GetFloat(
                    Shader.PropertyToID("_NightsideReadability")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .MercuryNightsideReadability));
        }

        [Test]
        public void View_RejectsMismatchedBodyIdentity()
        {
            AirlessRockyVisualDefinition definition =
                CreateMercuryDefinition();
            GameObject root = CreateObject("Mercury");
            GameObject surfaceObject = CreateObject("Surface", root.transform);
            AirlessRockyVisualView view =
                root.AddComponent<AirlessRockyVisualView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("definition").objectReferenceValue =
                definition;
            serialized.FindProperty("surfaceRenderer").objectReferenceValue =
                surfaceObject.AddComponent<MeshRenderer>();
            serialized.ApplyModifiedPropertiesWithoutUndo();
            CelestialBodyModel moon = CelestialTestFactory.CreateOrbitingBody(
                "moon",
                "earth");

            Assert.Throws<InvalidOperationException>(
                () => view.Initialize(moon));
        }

        [TestCase(
            "io",
            AirlessRockyVisualRenderingContract.IoReliefStrength,
            AirlessRockyVisualRenderingContract.IoReliefSampleDistance,
            AirlessRockyVisualRenderingContract.IoSurfaceSpecular,
            AirlessRockyVisualRenderingContract.IoSurfaceSmoothness,
            AirlessRockyVisualRenderingContract.IoNightsideReadability)]
        [TestCase(
            "europa",
            AirlessRockyVisualRenderingContract.EuropaReliefStrength,
            AirlessRockyVisualRenderingContract.EuropaReliefSampleDistance,
            AirlessRockyVisualRenderingContract.EuropaSurfaceSpecular,
            AirlessRockyVisualRenderingContract.EuropaSurfaceSmoothness,
            AirlessRockyVisualRenderingContract.EuropaNightsideReadability)]
        [TestCase(
            "ganymede",
            AirlessRockyVisualRenderingContract.GanymedeReliefStrength,
            AirlessRockyVisualRenderingContract.GanymedeReliefSampleDistance,
            AirlessRockyVisualRenderingContract.GanymedeSurfaceSpecular,
            AirlessRockyVisualRenderingContract.GanymedeSurfaceSmoothness,
            AirlessRockyVisualRenderingContract.GanymedeNightsideReadability)]
        [TestCase(
            "callisto",
            AirlessRockyVisualRenderingContract.CallistoReliefStrength,
            AirlessRockyVisualRenderingContract.CallistoReliefSampleDistance,
            AirlessRockyVisualRenderingContract.CallistoSurfaceSpecular,
            AirlessRockyVisualRenderingContract.CallistoSurfaceSmoothness,
            AirlessRockyVisualRenderingContract.CallistoNightsideReadability)]
        public void MajorMoonContract_CreatesValidatedImmutableModel(
            string stableId,
            float reliefStrength,
            float reliefSampleDistance,
            float surfaceSpecular,
            float surfaceSmoothness,
            float nightsideReadability)
        {
            var model = new AirlessRockyVisualModel(
                stableId,
                reliefStrength,
                reliefSampleDistance,
                surfaceSpecular,
                surfaceSmoothness,
                nightsideReadability);

            Assert.That(model.BodyStableId, Is.EqualTo(stableId));
            Assert.That(model.ReliefStrength, Is.EqualTo(reliefStrength));
            Assert.That(
                model.ReliefSampleDistance,
                Is.EqualTo(reliefSampleDistance));
            Assert.That(model.SurfaceSpecular, Is.EqualTo(surfaceSpecular));
            Assert.That(
                model.SurfaceSmoothness,
                Is.EqualTo(surfaceSmoothness));
            Assert.That(
                model.NightsideReadability,
                Is.EqualTo(nightsideReadability));
        }

        private AirlessRockyVisualDefinition CreateMercuryDefinition()
        {
            AirlessRockyVisualDefinition definition =
                ScriptableObject.CreateInstance<AirlessRockyVisualDefinition>();
            createdObjects.Add(definition);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "mercury";
            serialized.FindProperty("reliefStrength").floatValue =
                AirlessRockyVisualRenderingContract.MercuryReliefStrength;
            serialized.FindProperty("reliefSampleDistance").floatValue =
                AirlessRockyVisualRenderingContract
                    .MercuryReliefSampleDistance;
            serialized.FindProperty("surfaceSpecular").floatValue =
                AirlessRockyVisualRenderingContract.MercurySurfaceSpecular;
            serialized.FindProperty("surfaceSmoothness").floatValue =
                AirlessRockyVisualRenderingContract.MercurySurfaceSmoothness;
            serialized.FindProperty("nightsideReadability").floatValue =
                AirlessRockyVisualRenderingContract
                    .MercuryNightsideReadability;
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
