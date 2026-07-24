using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class SolarSystemVisualAssetTests
    {
        private const string ProfilePath =
            "Assets/SolarSystem/Settings/Rendering/VP_SolarSystem.asset";
        private const string SkyboxMaterialPath =
            "Assets/SolarSystem/Content/Materials/Environment/M_SpaceSkybox.mat";
        private const string SpaceTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/Environment/T_Space_MilkyWay_2K.jpg";
        private const string EarthMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Earth.mat";
        private const string EarthNormalPath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_Normal_2K.tif";

        [Test]
        public void VisualProfile_UsesApprovedRestrainedPostProcessing()
        {
            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(ProfilePath);

            Assert.That(profile, Is.Not.Null);
            Assert.That(profile.TryGet(out Tonemapping tonemapping), Is.True);
            Assert.That(tonemapping.mode.value, Is.EqualTo(TonemappingMode.ACES));
            Assert.That(profile.TryGet(out Bloom bloom), Is.True);
            Assert.That(bloom.threshold.value, Is.EqualTo(1.1f).Within(0.001f));
            Assert.That(bloom.intensity.value, Is.EqualTo(0.32f).Within(0.001f));
            Assert.That(bloom.highQualityFiltering.value, Is.False);
            Assert.That(profile.TryGet(out ColorAdjustments color), Is.True);
            Assert.That(color.postExposure.value, Is.EqualTo(-0.1f).Within(0.001f));
            Assert.That(color.contrast.value, Is.EqualTo(6f).Within(0.001f));
            Assert.That(profile.TryGet(out Vignette vignette), Is.True);
            Assert.That(vignette.intensity.value, Is.EqualTo(0.12f).Within(0.001f));
            Assert.That(profile.components.Count, Is.EqualTo(4));
        }

        [Test]
        public void SpaceSkybox_UsesApprovedPanoramicTexture()
        {
            Material skybox = AssetDatabase.LoadAssetAtPath<Material>(SkyboxMaterialPath);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(SpaceTexturePath);
            var importer = AssetImporter.GetAtPath(SpaceTexturePath) as TextureImporter;

            Assert.That(skybox, Is.Not.Null);
            Assert.That(skybox.shader.name, Is.EqualTo("Skybox/Panoramic"));
            Assert.That(skybox.GetTexture("_MainTex"), Is.SameAs(texture));
            Assert.That(skybox.GetFloat("_Exposure"), Is.EqualTo(0.62f).Within(0.001f));
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.mipmapEnabled, Is.True);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }

        [Test]
        public void EarthMaterial_UsesSubtleLinearNormalDetail()
        {
            Material earth = AssetDatabase.LoadAssetAtPath<Material>(EarthMaterialPath);
            Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(EarthNormalPath);
            var importer = AssetImporter.GetAtPath(EarthNormalPath) as TextureImporter;

            Assert.That(earth, Is.Not.Null);
            Assert.That(earth.shader.name, Is.EqualTo("Universal Render Pipeline/Lit"));
            Assert.That(earth.GetTexture("_BumpMap"), Is.SameAs(normal));
            Assert.That(earth.GetFloat("_BumpScale"), Is.EqualTo(0.28f).Within(0.001f));
            Assert.That(earth.IsKeywordEnabled("_NORMALMAP"), Is.True);
            Assert.That(earth.enableInstancing, Is.True);
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.NormalMap));
            Assert.That(importer.sRGBTexture, Is.False);
        }
    }
}
