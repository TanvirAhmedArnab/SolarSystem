using System.Collections.Generic;
using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CelestialAuthoringTests
    {
        private readonly List<Object> createdObjects = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (Object createdObject in createdObjects)
            {
                Object.DestroyImmediate(createdObject);
            }

            createdObjects.Clear();
        }

        [Test]
        public void ToModel_CopiesUnitExplicitSerializedValues()
        {
            CelestialBodyDefinition earth = CreateDefinition(
                "earth",
                "Earth",
                CelestialBodyCategory.Planet,
                "sun",
                true);

            CelestialBodyModel model = earth.ToModel();

            Assert.That(model.Id.Value, Is.EqualTo("earth"));
            Assert.That(model.ParentId.Value.Value, Is.EqualTo("sun"));
            Assert.That(model.MeanRadiusKm, Is.EqualTo(6371d));
            Assert.That(model.MassKg.Value, Is.EqualTo(5.9722e24d));
            Assert.That(model.Orbit.Value.SemiMajorAxisKm, Is.EqualTo(149597870.7d));
            Assert.That(model.ScientificSourceId, Is.EqualTo("TEST-SOURCE"));
        }

        [Test]
        public void ToModel_DoesNotMutateAuthoringAsset()
        {
            CelestialBodyDefinition earth = CreateDefinition(
                "earth",
                "Earth",
                CelestialBodyCategory.Planet,
                "sun",
                true);
            string originalId = earth.StableId;
            double originalAxis = earth.Orbit.SemiMajorAxisKm;

            _ = earth.ToModel();

            Assert.That(earth.StableId, Is.EqualTo(originalId));
            Assert.That(earth.Orbit.SemiMajorAxisKm, Is.EqualTo(originalAxis));
        }

        [Test]
        public void CatalogBuild_OrdersShuffledDefinitionsParentFirst()
        {
            CelestialBodyDefinition sun = CreateDefinition(
                "sun",
                "Sun",
                CelestialBodyCategory.Star,
                string.Empty,
                false);
            CelestialBodyDefinition earth = CreateDefinition(
                "earth",
                "Earth",
                CelestialBodyCategory.Planet,
                "sun",
                true);
            CelestialBodyDefinition moon = CreateDefinition(
                "moon",
                "Moon",
                CelestialBodyCategory.Moon,
                "earth",
                true);
            CelestialCatalogDefinition catalogDefinition =
                ScriptableObject.CreateInstance<CelestialCatalogDefinition>();
            createdObjects.Add(catalogDefinition);
            SetCatalogBodies(catalogDefinition, moon, earth, sun);

            CelestialCatalog catalog = catalogDefinition.BuildCatalog();

            Assert.That(catalog.OrderedBodies[0].Id.Value, Is.EqualTo("sun"));
            Assert.That(catalog.OrderedBodies[1].Id.Value, Is.EqualTo("earth"));
            Assert.That(catalog.OrderedBodies[2].Id.Value, Is.EqualTo("moon"));
        }

        [Test]
        public void CatalogBuild_RejectsMissingDefinitionReference()
        {
            CelestialCatalogDefinition catalogDefinition =
                ScriptableObject.CreateInstance<CelestialCatalogDefinition>();
            createdObjects.Add(catalogDefinition);
            SetCatalogBodies(
                catalogDefinition,
                new CelestialBodyDefinition[] { null });

            Assert.Throws<CelestialCatalogValidationException>(
                () => catalogDefinition.BuildCatalog());
        }

        private CelestialBodyDefinition CreateDefinition(
            string id,
            string displayName,
            CelestialBodyCategory category,
            string parentId,
            bool hasOrbit)
        {
            CelestialBodyDefinition definition =
                ScriptableObject.CreateInstance<CelestialBodyDefinition>();
            createdObjects.Add(definition);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("stableId").stringValue = id;
            serialized.FindProperty("displayName").stringValue = displayName;
            serialized.FindProperty("category").enumValueIndex = (int)category;
            serialized.FindProperty("parentId").stringValue = parentId;
            serialized.FindProperty("scientificSourceId").stringValue = "TEST-SOURCE";
            serialized.FindProperty("educationalSummary").stringValue =
                "A concise educational summary.";
            serialized.FindProperty("meanRadiusKm").doubleValue = 6371d;
            serialized.FindProperty("hasMass").boolValue = true;
            serialized.FindProperty("massKg").doubleValue = 5.9722e24d;
            serialized.FindProperty("rotationPeriodSeconds").doubleValue = 86164.2d;
            serialized.FindProperty("axialTiltDeg").doubleValue = 23.44d;
            serialized.FindProperty("hasOrbit").boolValue = hasOrbit;
            if (hasOrbit)
            {
                SerializedProperty orbit = serialized.FindProperty("orbit");
                orbit.FindPropertyRelative("semiMajorAxisKm").doubleValue = 149597870.7d;
                orbit.FindPropertyRelative("eccentricity").doubleValue = 0.0167d;
                orbit.FindPropertyRelative("inclinationDeg").doubleValue = 0d;
                orbit.FindPropertyRelative("longitudeAscendingNodeDeg").doubleValue = 0d;
                orbit.FindPropertyRelative("argumentPeriapsisDeg").doubleValue = 0d;
                orbit.FindPropertyRelative("meanAnomalyAtEpochDeg").doubleValue = 0d;
                orbit.FindPropertyRelative("orbitalPeriodSeconds").doubleValue = 31558149.8d;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        private static void SetCatalogBodies(
            CelestialCatalogDefinition catalog,
            params CelestialBodyDefinition[] definitions)
        {
            var serialized = new SerializedObject(catalog);
            SerializedProperty bodies = serialized.FindProperty("bodies");
            bodies.arraySize = definitions.Length;
            for (int index = 0; index < definitions.Length; index++)
            {
                bodies.GetArrayElementAtIndex(index).objectReferenceValue = definitions[index];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
