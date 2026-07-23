using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class Slice2ContentAssetTests
    {
        private const string CatalogPath =
            "Assets/SolarSystem/Content/Data/Catalog_SolarSystem.asset";
        private const string JupiterMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Jupiter.mat";
        private const string JupiterTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Jupiter/" +
            "T_Jupiter_Surface_2K.jpg";

        [Test]
        public void CatalogAsset_ContainsVerifiedJupiterDefinition()
        {
            CelestialCatalogDefinition catalog =
                AssetDatabase.LoadAssetAtPath<CelestialCatalogDefinition>(CatalogPath);

            Assert.That(catalog, Is.Not.Null);
            Assert.That(catalog.Bodies, Has.Count.EqualTo(4));
            Assert.That(
                catalog.TryGetDefinition("jupiter", out CelestialBodyDefinition jupiter),
                Is.True);
            Assert.That(jupiter.DisplayName, Is.EqualTo("Jupiter"));
            Assert.That(jupiter.Category, Is.EqualTo(CelestialBodyCategory.Planet));
            Assert.That(jupiter.ParentId, Is.EqualTo("sun"));
            Assert.That(
                jupiter.ScientificSourceId,
                Is.EqualTo(
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_JUPITER_FACTS"));
            Assert.That(jupiter.MeanRadiusKm, Is.EqualTo(69911d));
            Assert.That(jupiter.MassKg, Is.EqualTo(1.898125e27d));
            Assert.That(jupiter.RotationPeriodSeconds, Is.EqualTo(35729.856d).Within(0.000001d));
            Assert.That(jupiter.AxialTiltDeg, Is.EqualTo(3d));
            Assert.That(jupiter.HasOrbit, Is.True);
            Assert.That(
                jupiter.Orbit.SemiMajorAxisKm,
                Is.EqualTo(778340816.6927109d).Within(0.001d));
            Assert.That(jupiter.Orbit.Eccentricity, Is.EqualTo(0.04838624d).Within(0.000000000001d));
            Assert.That(jupiter.Orbit.InclinationDeg, Is.EqualTo(1.30439695d).Within(0.000000001d));
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

        [Test]
        public void JupiterMaterial_UsesAuditedTexture()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(JupiterMaterialPath);
            Texture expectedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(JupiterTexturePath);

            Assert.That(material, Is.Not.Null);
            Assert.That(expectedTexture, Is.Not.Null);
            Assert.That(material.GetTexture("_BaseMap"), Is.SameAs(expectedTexture));
            Assert.That(material.GetFloat("_Smoothness"), Is.EqualTo(0.35f).Within(0.0001f));
        }
    }
}
