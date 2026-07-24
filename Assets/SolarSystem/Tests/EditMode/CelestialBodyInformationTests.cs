using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.UI;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CelestialBodyInformationTests
    {
        private CelestialBodyDefinition definition;

        [TearDown]
        public void TearDown()
        {
            if (definition != null)
            {
                Object.DestroyImmediate(definition);
            }
        }

        [Test]
        public void From_FormatsVerifiedEarthDefinitionForEducationalPresentation()
        {
            definition = CreateDefinition(hasOrbit: true);

            CelestialBodyInformation information =
                CelestialBodyInformation.From(definition);

            Assert.That(information.DisplayName, Is.EqualTo("Earth"));
            Assert.That(information.Category, Is.EqualTo("Planet"));
            Assert.That(information.Summary, Does.Contain("rocky world"));
            Assert.That(information.Parent, Is.EqualTo("Sun"));
            Assert.That(information.Radius, Is.EqualTo("6,371.0 km"));
            Assert.That(information.Mass, Is.EqualTo("5.972E+24 kg"));
            Assert.That(information.Rotation, Is.EqualTo("23.93 hours"));
            Assert.That(information.AxialTilt, Is.EqualTo("23.44°"));
            Assert.That(information.OrbitDistance, Is.EqualTo("149.60 million km"));
            Assert.That(information.OrbitPeriod, Is.EqualTo("365.26 days"));
            Assert.That(information.SourceRecord, Is.EqualTo("TEST-SOURCE"));
        }

        [Test]
        public void From_LabelsCatalogRootOrbitFieldsAsNotApplicable()
        {
            definition = CreateDefinition(hasOrbit: false);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("stableId").stringValue = "sun";
            serialized.FindProperty("displayName").stringValue = "Sun";
            serialized.FindProperty("category").enumValueIndex =
                (int)CelestialBodyCategory.Star;
            serialized.FindProperty("parentId").stringValue = string.Empty;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CelestialBodyInformation information =
                CelestialBodyInformation.From(definition);

            Assert.That(information.Category, Is.EqualTo("Star"));
            Assert.That(information.Parent, Is.EqualTo("None — catalog root"));
            Assert.That(information.OrbitDistance, Is.EqualTo("Not applicable"));
            Assert.That(information.OrbitPeriod, Is.EqualTo("Not applicable"));
        }

        private static CelestialBodyDefinition CreateDefinition(bool hasOrbit)
        {
            CelestialBodyDefinition created =
                ScriptableObject.CreateInstance<CelestialBodyDefinition>();
            var serialized = new SerializedObject(created);
            serialized.FindProperty("stableId").stringValue = "earth";
            serialized.FindProperty("displayName").stringValue = "Earth";
            serialized.FindProperty("category").enumValueIndex =
                (int)CelestialBodyCategory.Planet;
            serialized.FindProperty("parentId").stringValue = "sun";
            serialized.FindProperty("scientificSourceId").stringValue = "TEST-SOURCE";
            serialized.FindProperty("educationalSummary").stringValue =
                "A rocky world with global oceans.";
            serialized.FindProperty("meanRadiusKm").doubleValue = 6371d;
            serialized.FindProperty("hasMass").boolValue = true;
            serialized.FindProperty("massKg").doubleValue = 5.9722e24d;
            serialized.FindProperty("rotationPeriodSeconds").doubleValue = 86164.2d;
            serialized.FindProperty("axialTiltDeg").doubleValue = 23.44d;
            serialized.FindProperty("hasOrbit").boolValue = hasOrbit;

            SerializedProperty orbit = serialized.FindProperty("orbit");
            orbit.FindPropertyRelative("semiMajorAxisKm").doubleValue = 149597870.7d;
            orbit.FindPropertyRelative("orbitalPeriodSeconds").doubleValue = 31558149.8d;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return created;
        }
    }
}
