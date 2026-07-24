using System;
using System.Linq;
using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class Slice2ContentAssetTests
    {
        private const string CatalogPath =
            "Assets/SolarSystem/Content/Data/Catalog_SolarSystem.asset";
        private const string MaterialRoot =
            "Assets/SolarSystem/Content/Materials/CelestialBodies";
        private const string TextureRoot =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies";

        [Test]
        public void CatalogAsset_ContainsTheSunEightPlanetsAndEarthMoon()
        {
            CelestialCatalogDefinition catalog = LoadCatalog();

            Assert.That(catalog.Bodies, Has.Count.EqualTo(10));
            Assert.That(
                catalog.Bodies.Count(body => body.Category == CelestialBodyCategory.Star),
                Is.EqualTo(1));
            Assert.That(
                catalog.Bodies.Count(body => body.Category == CelestialBodyCategory.Planet),
                Is.EqualTo(8));
            Assert.That(
                catalog.Bodies.Count(body => body.Category == CelestialBodyCategory.Moon),
                Is.EqualTo(1));
            CollectionAssert.AreEqual(
                new[]
                {
                    "sun",
                    "mercury",
                    "venus",
                    "earth",
                    "moon",
                    "mars",
                    "jupiter",
                    "saturn",
                    "uranus",
                    "neptune"
                },
                catalog.Bodies.Select(body => body.StableId));
        }

        [TestCase("mercury", "Mercury", 2439.4d, 3.30103e23d, 58.6462d, 2d)]
        [TestCase("venus", "Venus", 6051.8d, 4.86731e24d, -243.018d, 3d)]
        [TestCase("earth", "Earth", 6371d, 5.9722e24d, 0.9972708333333333d, 23.44d)]
        [TestCase("mars", "Mars", 3389.5d, 6.41691e23d, 1.02595676d, 25.2d)]
        [TestCase("jupiter", "Jupiter", 69911d, 1.898125e27d, 0.41354d, 3d)]
        [TestCase("saturn", "Saturn", 58232d, 5.68317e26d, 0.44401d, 26.73d)]
        [TestCase("uranus", "Uranus", 25362d, 8.68099e25d, -0.71833d, 97.77d)]
        [TestCase("neptune", "Neptune", 24622d, 1.024092e26d, 0.67125d, 28d)]
        public void PlanetDefinition_UsesVerifiedPhysicalBaseline(
            string stableId,
            string displayName,
            double radiusKm,
            double massKg,
            double rotationPeriodDays,
            double axialTiltDeg)
        {
            CelestialCatalogDefinition catalog = LoadCatalog();

            Assert.That(
                catalog.TryGetDefinition(stableId, out CelestialBodyDefinition planet),
                Is.True);
            Assert.That(planet.DisplayName, Is.EqualTo(displayName));
            Assert.That(planet.Category, Is.EqualTo(CelestialBodyCategory.Planet));
            Assert.That(planet.ParentId, Is.EqualTo("sun"));
            Assert.That(planet.ScientificSourceId, Does.Contain("JPL"));
            Assert.That(
                planet.ScientificSourceId,
                Does.Contain(stableId.ToUpperInvariant()));
            Assert.That(planet.MeanRadiusKm, Is.EqualTo(radiusKm).Within(0.000001d));
            Assert.That(planet.MassKg, Is.EqualTo(massKg).Within(Math.Abs(massKg) * 1e-12d));
            Assert.That(
                planet.RotationPeriodSeconds,
                Is.EqualTo(rotationPeriodDays * 86400d).Within(0.000001d));
            Assert.That(planet.AxialTiltDeg, Is.EqualTo(axialTiltDeg).Within(0.000001d));
            Assert.That(planet.HasOrbit, Is.True);
            Assert.That(planet.Orbit.SemiMajorAxisKm, Is.GreaterThan(0d));
            Assert.That(planet.Orbit.OrbitalPeriodSeconds, Is.GreaterThan(0d));
        }

        [Test]
        public void JupiterDefinition_PreservesApprovedJ2000OrbitalBaseline()
        {
            CelestialCatalogDefinition catalog = LoadCatalog();
            Assert.That(
                catalog.TryGetDefinition("jupiter", out CelestialBodyDefinition jupiter),
                Is.True);

            Assert.That(
                jupiter.Orbit.SemiMajorAxisKm,
                Is.EqualTo(778340816.6927109d).Within(0.001d));
            Assert.That(
                jupiter.Orbit.Eccentricity,
                Is.EqualTo(0.04838624d).Within(0.000000000001d));
            Assert.That(
                jupiter.Orbit.InclinationDeg,
                Is.EqualTo(1.30439695d).Within(0.000000001d));
            Assert.That(
                jupiter.Orbit.LongitudeAscendingNodeDeg,
                Is.EqualTo(100.47390909d).Within(0.000000001d));
            Assert.That(
                jupiter.Orbit.ArgumentPeriapsisDeg,
                Is.EqualTo(-85.74542926d).Within(0.000000001d));
            Assert.That(
                jupiter.Orbit.MeanAnomalyAtEpochDeg,
                Is.EqualTo(19.66796068d).Within(0.000000001d));
            Assert.That(
                jupiter.Orbit.OrbitalPeriodSeconds,
                Is.EqualTo(374355659.124d).Within(0.001d));
        }

        [TestCase("Mercury", "T_Mercury_Surface_2K.jpg", 0.08f)]
        [TestCase("Venus", "T_Venus_Surface_2K.jpg", 0.24f)]
        [TestCase("Mars", "T_Mars_Surface_2K.jpg", 0.1f)]
        [TestCase("Jupiter", "T_Jupiter_Surface_2K.jpg", 0.18f)]
        [TestCase("Saturn", "T_Saturn_Surface_2K.jpg", 0.2f)]
        [TestCase("Uranus", "T_Uranus_Surface_2K.jpg", 0.28f)]
        [TestCase("Neptune", "T_Neptune_Surface_2K.jpg", 0.3f)]
        public void PlanetMaterial_UsesAuditedTextureAndVisualBaseline(
            string bodyName,
            string textureName,
            float smoothness)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(
                $"{MaterialRoot}/M_{bodyName}.mat");
            Texture expectedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                $"{TextureRoot}/{bodyName}/{textureName}");

            Assert.That(material, Is.Not.Null);
            Assert.That(expectedTexture, Is.Not.Null);
            Assert.That(material.GetTexture("_BaseMap"), Is.SameAs(expectedTexture));
            Assert.That(material.GetFloat("_Smoothness"), Is.EqualTo(smoothness).Within(0.0001f));
            Assert.That(material.enableInstancing, Is.True);
        }

        [Test]
        public void SaturnRingAssets_UseAuditedAlphaTextureAndGeneratedAnnulus()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(
                $"{MaterialRoot}/M_Saturn_Rings.mat");
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                $"{TextureRoot}/Saturn/T_Saturn_RingsAlpha_2K.png");
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(
                "Assets/SolarSystem/Content/Models/Generated/SM_Saturn_Rings.asset");

            Assert.That(material, Is.Not.Null);
            Assert.That(texture, Is.Not.Null);
            Assert.That(material.GetTexture("_BaseMap"), Is.SameAs(texture));
            Assert.That(material.renderQueue, Is.EqualTo((int)RenderQueue.Transparent));
            Assert.That(material.GetFloat("_Cull"), Is.EqualTo((float)CullMode.Off));
            Assert.That(mesh, Is.Not.Null);
            Assert.That(mesh.vertexCount, Is.EqualTo(258));
            Assert.That(mesh.triangles, Has.Length.EqualTo(768));
            Assert.That(mesh.bounds.extents.x, Is.EqualTo(1.15f).Within(0.001f));
            Assert.That(mesh.bounds.extents.z, Is.EqualTo(1.15f).Within(0.001f));
        }

        private static CelestialCatalogDefinition LoadCatalog()
        {
            CelestialCatalogDefinition catalog =
                AssetDatabase.LoadAssetAtPath<CelestialCatalogDefinition>(CatalogPath);
            Assert.That(catalog, Is.Not.Null);
            return catalog;
        }
    }
}
