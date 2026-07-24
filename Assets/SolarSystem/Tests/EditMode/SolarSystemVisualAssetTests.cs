using NUnit.Framework;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
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
        private const string SunTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Sun/T_Sun_Surface_2K.jpg";
        private const string SunMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Sun.mat";
        private const string SunCoronaMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Sun_Corona.mat";
        private const string SunVisualDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Sun.asset";
        private const string JupiterTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Jupiter/T_Jupiter_Surface_2K.jpg";
        private const string JupiterMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Jupiter.mat";
        private const string JupiterAtmosphereMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Jupiter_Atmosphere.mat";
        private const string JupiterVisualDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Jupiter.asset";
        private const string EarthMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Earth.mat";
        private const string EarthNormalPath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_Normal_2K.tif";
        private const string EarthSpecularPath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_Specular_2K.tif";
        private const string EarthNightPath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_NightEmission_2K.jpg";
        private const string EarthCloudPath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_Clouds_2K.jpg";
        private const string EarthCloudMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Earth_Clouds.mat";
        private const string EarthAtmosphereMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Earth_Atmosphere.mat";

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
        public void EarthMaterials_UseAuditedLayeredRenderingInputs()
        {
            Material earth = AssetDatabase.LoadAssetAtPath<Material>(EarthMaterialPath);
            Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(EarthNormalPath);
            Texture2D specular = AssetDatabase.LoadAssetAtPath<Texture2D>(EarthSpecularPath);
            Texture2D night = AssetDatabase.LoadAssetAtPath<Texture2D>(EarthNightPath);
            Texture2D clouds = AssetDatabase.LoadAssetAtPath<Texture2D>(EarthCloudPath);
            Material cloudMaterial =
                AssetDatabase.LoadAssetAtPath<Material>(EarthCloudMaterialPath);
            Material atmosphereMaterial =
                AssetDatabase.LoadAssetAtPath<Material>(EarthAtmosphereMaterialPath);
            var importer = AssetImporter.GetAtPath(EarthNormalPath) as TextureImporter;
            var specularImporter =
                AssetImporter.GetAtPath(EarthSpecularPath) as TextureImporter;
            var cloudImporter =
                AssetImporter.GetAtPath(EarthCloudPath) as TextureImporter;

            Assert.That(earth, Is.Not.Null);
            Assert.That(
                earth.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Earth Surface"));
            Assert.That(earth.GetTexture("_BumpMap"), Is.SameAs(normal));
            Assert.That(earth.GetFloat("_BumpScale"), Is.EqualTo(0.28f).Within(0.001f));
            Assert.That(earth.GetTexture("_SpecularMap"), Is.SameAs(specular));
            Assert.That(earth.GetTexture("_NightMap"), Is.SameAs(night));
            Assert.That(earth.GetFloat("_OceanSpecular"), Is.GreaterThan(
                earth.GetFloat("_LandSpecular")));
            Assert.That(earth.GetFloat("_OceanSmoothness"), Is.GreaterThan(
                earth.GetFloat("_LandSmoothness")));
            Assert.That(earth.enableInstancing, Is.True);
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.NormalMap));
            Assert.That(importer.sRGBTexture, Is.False);
            Assert.That(specularImporter, Is.Not.Null);
            Assert.That(specularImporter.sRGBTexture, Is.False);

            Assert.That(cloudMaterial, Is.Not.Null);
            Assert.That(
                cloudMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Earth Cloud Layer"));
            Assert.That(cloudMaterial.GetTexture("_CloudMap"), Is.SameAs(clouds));
            Assert.That(cloudMaterial.renderQueue, Is.EqualTo((int)RenderQueue.Transparent));
            Assert.That(cloudMaterial.enableInstancing, Is.True);
            Assert.That(cloudImporter, Is.Not.Null);
            Assert.That(cloudImporter.sRGBTexture, Is.False);

            Assert.That(atmosphereMaterial, Is.Not.Null);
            Assert.That(
                atmosphereMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Atmosphere Rim"));
            Assert.That(
                atmosphereMaterial.renderQueue,
                Is.EqualTo((int)RenderQueue.Transparent + 10));
            Assert.That(atmosphereMaterial.GetFloat("_RimIntensity"), Is.LessThan(0.5f));
        }

        [Test]
        public void SunMaterials_UseAuditedDeterministicHeroTreatment()
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(SunTexturePath);
            Material surface = AssetDatabase.LoadAssetAtPath<Material>(SunMaterialPath);
            Material corona =
                AssetDatabase.LoadAssetAtPath<Material>(SunCoronaMaterialPath);
            SolarVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<SolarVisualDefinition>(
                    SunVisualDefinitionPath);
            var importer = AssetImporter.GetAtPath(SunTexturePath) as TextureImporter;

            Assert.That(texture, Is.Not.Null);
            Assert.That(surface, Is.Not.Null);
            Assert.That(
                surface.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Solar Surface"));
            Assert.That(surface.GetTexture("_BaseMap"), Is.SameAs(texture));
            Assert.That(
                surface.GetFloat("_FlowStrength"),
                Is.EqualTo(SolarVisualRenderingContract.SurfaceFlowStrength)
                    .Within(0.0001f));
            Assert.That(surface.enableInstancing, Is.True);

            Assert.That(corona, Is.Not.Null);
            Assert.That(
                corona.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Solar Corona"));
            Assert.That(corona.GetTexture("_SolarMap"), Is.SameAs(texture));
            Assert.That(
                corona.GetFloat("_Intensity"),
                Is.EqualTo(SolarVisualRenderingContract.CoronaIntensity)
                    .Within(0.0001f));
            Assert.That(
                corona.renderQueue,
                Is.EqualTo((int)RenderQueue.Transparent + 20));
            Assert.That(corona.enableInstancing, Is.True);

            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.BodyStableId, Is.EqualTo("sun"));
            Assert.That(
                definition.CoronaShellRadiusMultiplier,
                Is.EqualTo(SolarVisualRenderingContract.CoronaShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.mipmapEnabled, Is.True);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }

        [Test]
        public void JupiterMaterials_UseAnchoredTextureAndGasGiantContract()
        {
            Texture2D texture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(JupiterTexturePath);
            Material surface =
                AssetDatabase.LoadAssetAtPath<Material>(JupiterMaterialPath);
            Material atmosphere =
                AssetDatabase.LoadAssetAtPath<Material>(
                    JupiterAtmosphereMaterialPath);
            GasGiantVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<GasGiantVisualDefinition>(
                    JupiterVisualDefinitionPath);
            var importer =
                AssetImporter.GetAtPath(JupiterTexturePath) as TextureImporter;

            Assert.That(texture, Is.Not.Null);
            Assert.That(surface, Is.Not.Null);
            Assert.That(
                surface.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Gas Giant Surface"));
            Assert.That(surface.GetTexture("_BaseMap"), Is.SameAs(texture));
            Assert.That(
                surface.GetFloat("_FlowStrength"),
                Is.EqualTo(GasGiantVisualRenderingContract.BandFlowStrength)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_AnimatedDetailStrength"),
                Is.EqualTo(
                    GasGiantVisualRenderingContract.AnimatedDetailStrength)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_BandNormalStrength"),
                Is.EqualTo(GasGiantVisualRenderingContract.BandNormalStrength)
                    .Within(0.0001f));
            Assert.That(surface.enableInstancing, Is.True);

            Assert.That(atmosphere, Is.Not.Null);
            Assert.That(
                atmosphere.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Gas Giant Atmosphere"));
            Assert.That(
                atmosphere.GetFloat("_RimIntensity"),
                Is.EqualTo(GasGiantVisualRenderingContract.AtmosphereIntensity)
                    .Within(0.0001f));
            Assert.That(
                atmosphere.renderQueue,
                Is.EqualTo((int)RenderQueue.Transparent + 12));
            Assert.That(atmosphere.enableInstancing, Is.True);

            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.BodyStableId, Is.EqualTo("jupiter"));
            Assert.That(
                definition.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(
                    GasGiantVisualRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.mipmapEnabled, Is.True);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }
    }
}
