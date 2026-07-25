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
        private const string SaturnTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Saturn/T_Saturn_Surface_2K.jpg";
        private const string SaturnMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Saturn.mat";
        private const string SaturnAtmosphereMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Saturn_Atmosphere.mat";
        private const string SaturnVisualDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Saturn.asset";
        private const string UranusTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Uranus/T_Uranus_Surface_2K.jpg";
        private const string UranusMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Uranus.mat";
        private const string UranusAtmosphereMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Uranus_Atmosphere.mat";
        private const string UranusVisualDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Uranus.asset";
        private const string NeptuneTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Neptune/T_Neptune_Surface_2K.jpg";
        private const string NeptuneMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Neptune.mat";
        private const string NeptuneAtmosphereMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Neptune_Atmosphere.mat";
        private const string NeptuneVisualDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Neptune.asset";
        private const string VenusSurfaceTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Venus/T_Venus_Surface_2K.jpg";
        private const string VenusAtmosphereTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Venus/T_Venus_Atmosphere_2K.jpg";
        private const string VenusSurfaceMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Venus.mat";
        private const string VenusCloudMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Venus_CloudDeck.mat";
        private const string VenusAtmosphereMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Venus_Atmosphere.mat";
        private const string VenusLayerDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Venus.asset";
        private const string MarsTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Mars/T_Mars_Surface_2K.jpg";
        private const string MarsMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Mars.mat";
        private const string MarsAtmosphereMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Mars_Atmosphere.mat";
        private const string MarsLayerDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Mars.asset";
        private const string TitanTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Titan/T_Titan_Surface_Browse.jpg";
        private const string TitanMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Titan.mat";
        private const string TitanHazeMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Titan_Haze.mat";
        private const string TitanLayerDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Titan.asset";
        private const string MercuryTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Mercury/T_Mercury_Surface_2K.jpg";
        private const string MercuryMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Mercury.mat";
        private const string MercuryVisualDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Mercury.asset";
        private const string MoonTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Moon/T_Moon_Surface_2K.jpg";
        private const string MoonMaterialPath =
            "Assets/SolarSystem/Content/Materials/CelestialBodies/M_Moon.mat";
        private const string MoonVisualDefinitionPath =
            "Assets/SolarSystem/Content/Data/VisualLayers/VisualLayers_Moon.asset";
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
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Surface"));
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
            Assert.That(
                surface.GetFloat("_NightsideReadability"),
                Is.EqualTo(GasGiantVisualRenderingContract.NightsideReadability)
                    .Within(0.0001f));
            Assert.That(surface.enableInstancing, Is.True);

            Assert.That(atmosphere, Is.Not.Null);
            Assert.That(
                atmosphere.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Atmosphere"));
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

        [Test]
        public void SaturnMaterials_UseDistinctAnchoredGasGiantContract()
        {
            Texture2D texture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(SaturnTexturePath);
            Material surface =
                AssetDatabase.LoadAssetAtPath<Material>(SaturnMaterialPath);
            Material atmosphere =
                AssetDatabase.LoadAssetAtPath<Material>(
                    SaturnAtmosphereMaterialPath);
            GasGiantVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<GasGiantVisualDefinition>(
                    SaturnVisualDefinitionPath);
            var importer =
                AssetImporter.GetAtPath(SaturnTexturePath) as TextureImporter;

            Assert.That(texture, Is.Not.Null);
            Assert.That(surface, Is.Not.Null);
            Assert.That(
                surface.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Surface"));
            Assert.That(surface.GetTexture("_BaseMap"), Is.SameAs(texture));
            Assert.That(
                surface.GetFloat("_FlowStrength"),
                Is.EqualTo(
                    GasGiantVisualRenderingContract.SaturnBandFlowStrength)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_AnimatedDetailStrength"),
                Is.EqualTo(
                    GasGiantVisualRenderingContract.SaturnAnimatedDetailStrength)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_BandNormalStrength"),
                Is.EqualTo(
                    GasGiantVisualRenderingContract.SaturnBandNormalStrength)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_NightsideReadability"),
                Is.EqualTo(
                    GasGiantVisualRenderingContract.SaturnNightsideReadability)
                    .Within(0.0001f));
            Assert.That(surface.enableInstancing, Is.True);

            Assert.That(atmosphere, Is.Not.Null);
            Assert.That(
                atmosphere.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Atmosphere"));
            Assert.That(
                atmosphere.GetFloat("_RimIntensity"),
                Is.EqualTo(
                    GasGiantVisualRenderingContract.SaturnAtmosphereIntensity)
                    .Within(0.0001f));
            Assert.That(
                atmosphere.renderQueue,
                Is.EqualTo((int)RenderQueue.Transparent + 11));
            Assert.That(atmosphere.enableInstancing, Is.True);

            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.BodyStableId, Is.EqualTo("saturn"));
            Assert.That(
                definition.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(
                    GasGiantVisualRenderingContract
                        .SaturnAtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                definition.BandFlowCyclesPerRotation,
                Is.EqualTo(
                    GasGiantVisualRenderingContract
                        .SaturnBandFlowCyclesPerRotation)
                    .Within(0.0001f));
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.mipmapEnabled, Is.True);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }

        [Test]
        public void IceGiantMaterials_PreserveDistinctAnchoredSourcesAndContracts()
        {
            AssertIceGiantMaterial(
                "uranus",
                UranusTexturePath,
                UranusMaterialPath,
                UranusAtmosphereMaterialPath,
                UranusVisualDefinitionPath,
                IceGiantVisualRenderingContract.UranusDetailFlowStrength,
                IceGiantVisualRenderingContract.UranusAnimatedDetailStrength,
                IceGiantVisualRenderingContract.UranusBandNormalStrength,
                IceGiantVisualRenderingContract.UranusNightsideReadability,
                IceGiantVisualRenderingContract.UranusAtmosphereIntensity,
                IceGiantVisualRenderingContract
                    .UranusAtmosphereShellRadiusMultiplier,
                IceGiantVisualRenderingContract.UranusDetailCyclesPerRotation,
                (int)RenderQueue.Transparent + 10);
            AssertIceGiantMaterial(
                "neptune",
                NeptuneTexturePath,
                NeptuneMaterialPath,
                NeptuneAtmosphereMaterialPath,
                NeptuneVisualDefinitionPath,
                IceGiantVisualRenderingContract.NeptuneDetailFlowStrength,
                IceGiantVisualRenderingContract.NeptuneAnimatedDetailStrength,
                IceGiantVisualRenderingContract.NeptuneBandNormalStrength,
                IceGiantVisualRenderingContract.NeptuneNightsideReadability,
                IceGiantVisualRenderingContract.NeptuneAtmosphereIntensity,
                IceGiantVisualRenderingContract
                    .NeptuneAtmosphereShellRadiusMultiplier,
                IceGiantVisualRenderingContract.NeptuneDetailCyclesPerRotation,
                (int)RenderQueue.Transparent + 11);
        }

        [Test]
        public void VenusMaterials_UseAnchoredOpaqueCloudDeckAndLayerContract()
        {
            Texture2D surfaceTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(VenusSurfaceTexturePath);
            Texture2D atmosphereTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    VenusAtmosphereTexturePath);
            Material surface =
                AssetDatabase.LoadAssetAtPath<Material>(VenusSurfaceMaterialPath);
            Material clouds =
                AssetDatabase.LoadAssetAtPath<Material>(VenusCloudMaterialPath);
            Material atmosphere =
                AssetDatabase.LoadAssetAtPath<Material>(
                    VenusAtmosphereMaterialPath);
            CelestialLayerVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<CelestialLayerVisualDefinition>(
                    VenusLayerDefinitionPath);
            var surfaceImporter =
                AssetImporter.GetAtPath(VenusSurfaceTexturePath) as TextureImporter;
            var atmosphereImporter =
                AssetImporter.GetAtPath(VenusAtmosphereTexturePath) as TextureImporter;

            Assert.That(surfaceTexture, Is.Not.Null);
            Assert.That(atmosphereTexture, Is.Not.Null);
            Assert.That(surface, Is.Not.Null);
            Assert.That(surface.GetTexture("_BaseMap"), Is.SameAs(surfaceTexture));
            Assert.That(clouds, Is.Not.Null);
            Assert.That(
                clouds.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Venus Cloud Deck"));
            Assert.That(
                clouds.GetTexture("_CloudMap"),
                Is.SameAs(atmosphereTexture));
            Assert.That(
                clouds.GetFloat("_ReliefStrength"),
                Is.EqualTo(VenusLayerRenderingContract.CloudReliefStrength)
                    .Within(0.0001f));
            Assert.That(
                clouds.GetFloat("_AmbientBrightness"),
                Is.EqualTo(
                    VenusLayerRenderingContract.CloudAmbientBrightness)
                    .Within(0.0001f));
            Assert.That(
                clouds.GetFloat("_SunBrightness"),
                Is.EqualTo(VenusLayerRenderingContract.CloudSunBrightness)
                    .Within(0.0001f));
            Assert.That(
                clouds.renderQueue,
                Is.EqualTo((int)RenderQueue.Geometry + 1));
            Assert.That(clouds.enableInstancing, Is.True);

            Assert.That(atmosphere, Is.Not.Null);
            Assert.That(
                atmosphere.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Atmosphere Rim"));
            Assert.That(
                atmosphere.GetFloat("_RimIntensity"),
                Is.EqualTo(VenusLayerRenderingContract.AtmosphereIntensity)
                    .Within(0.0001f));
            Assert.That(
                atmosphere.renderQueue,
                Is.EqualTo((int)RenderQueue.Transparent + 10));
            Assert.That(atmosphere.enableInstancing, Is.True);

            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.BodyStableId, Is.EqualTo("venus"));
            Assert.That(
                definition.CloudShellRadiusMultiplier,
                Is.EqualTo(VenusLayerRenderingContract.CloudShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                definition.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(
                    VenusLayerRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                definition.CloudRotationMultiplier,
                Is.EqualTo(VenusLayerRenderingContract.CloudRotationMultiplier)
                    .Within(0.0001f));
            Assert.That(surfaceImporter, Is.Not.Null);
            Assert.That(surfaceImporter.sRGBTexture, Is.True);
            Assert.That(surfaceImporter.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
            Assert.That(atmosphereImporter, Is.Not.Null);
            Assert.That(atmosphereImporter.sRGBTexture, Is.True);
            Assert.That(atmosphereImporter.mipmapEnabled, Is.True);
            Assert.That(
                atmosphereImporter.wrapMode,
                Is.EqualTo(TextureWrapMode.Repeat));
        }

        [Test]
        public void MarsMaterials_UseAnchoredRockySurfaceAndAtmosphereOnlyContract()
        {
            Texture2D texture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(MarsTexturePath);
            Material surface =
                AssetDatabase.LoadAssetAtPath<Material>(MarsMaterialPath);
            Material atmosphere =
                AssetDatabase.LoadAssetAtPath<Material>(
                    MarsAtmosphereMaterialPath);
            CelestialLayerVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<CelestialLayerVisualDefinition>(
                    MarsLayerDefinitionPath);
            var importer =
                AssetImporter.GetAtPath(MarsTexturePath) as TextureImporter;

            Assert.That(texture, Is.Not.Null);
            Assert.That(surface, Is.Not.Null);
            Assert.That(
                surface.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(surface.GetTexture("_BaseMap"), Is.SameAs(texture));
            Assert.That(
                surface.GetFloat("_ReliefStrength"),
                Is.EqualTo(MarsLayerRenderingContract.ReliefStrength)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_ReliefSampleDistance"),
                Is.EqualTo(MarsLayerRenderingContract.ReliefSampleDistance)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_Specular"),
                Is.EqualTo(MarsLayerRenderingContract.SurfaceSpecular)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_Smoothness"),
                Is.EqualTo(MarsLayerRenderingContract.SurfaceSmoothness)
                    .Within(0.0001f));
            Assert.That(surface.enableInstancing, Is.True);

            Assert.That(atmosphere, Is.Not.Null);
            Assert.That(
                atmosphere.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Atmosphere Rim"));
            Assert.That(
                atmosphere.GetFloat("_RimPower"),
                Is.EqualTo(MarsLayerRenderingContract.AtmosphereRimPower)
                    .Within(0.0001f));
            Assert.That(
                atmosphere.GetFloat("_RimIntensity"),
                Is.EqualTo(MarsLayerRenderingContract.AtmosphereIntensity)
                    .Within(0.0001f));
            Assert.That(
                atmosphere.GetFloat("_NightsideVisibility"),
                Is.EqualTo(MarsLayerRenderingContract.AtmosphereNightsideVisibility)
                    .Within(0.0001f));
            Assert.That(
                atmosphere.renderQueue,
                Is.EqualTo((int)RenderQueue.Transparent + 10));
            Assert.That(atmosphere.enableInstancing, Is.True);

            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.BodyStableId, Is.EqualTo("mars"));
            Assert.That(definition.HasCloudLayer, Is.False);
            Assert.That(
                definition.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(MarsLayerRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.mipmapEnabled, Is.True);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }

        [Test]
        public void TitanMaterials_KeepAnchoredSurfaceSubordinateToDenseHazeContract()
        {
            Texture2D texture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(TitanTexturePath);
            Material surface =
                AssetDatabase.LoadAssetAtPath<Material>(TitanMaterialPath);
            Material haze =
                AssetDatabase.LoadAssetAtPath<Material>(TitanHazeMaterialPath);
            CelestialLayerVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<CelestialLayerVisualDefinition>(
                    TitanLayerDefinitionPath);
            var importer =
                AssetImporter.GetAtPath(TitanTexturePath) as TextureImporter;

            Assert.That(texture, Is.Not.Null);
            Assert.That(surface, Is.Not.Null);
            Assert.That(
                surface.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Titan Surface"));
            Assert.That(surface.GetTexture("_BaseMap"), Is.SameAs(texture));
            Assert.That(
                surface.GetFloat("_DetailStrength"),
                Is.EqualTo(TitanHazeRenderingContract.SurfaceDetailStrength)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_AmbientBrightness"),
                Is.EqualTo(TitanHazeRenderingContract.SurfaceAmbientBrightness)
                    .Within(0.0001f));
            Assert.That(
                surface.GetFloat("_SunBrightness"),
                Is.EqualTo(TitanHazeRenderingContract.SurfaceSunBrightness)
                    .Within(0.0001f));
            Assert.That(surface.renderQueue, Is.EqualTo((int)RenderQueue.Geometry));
            Assert.That(surface.enableInstancing, Is.True);

            Assert.That(haze, Is.Not.Null);
            Assert.That(
                haze.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Titan Haze"));
            Assert.That(
                haze.GetFloat("_DiskOpacity"),
                Is.EqualTo(TitanHazeRenderingContract.HazeDiskOpacity)
                    .Within(0.0001f));
            Assert.That(
                haze.GetFloat("_RimIntensity"),
                Is.EqualTo(TitanHazeRenderingContract.HazeRimIntensity)
                    .Within(0.0001f));
            Assert.That(
                haze.GetFloat("_RimPower"),
                Is.EqualTo(TitanHazeRenderingContract.HazeRimPower)
                    .Within(0.0001f));
            Assert.That(
                haze.GetFloat("_NightsideVisibility"),
                Is.EqualTo(TitanHazeRenderingContract.HazeNightsideVisibility)
                    .Within(0.0001f));
            Assert.That(
                haze.GetFloat("_ForwardScatter"),
                Is.EqualTo(TitanHazeRenderingContract.HazeForwardScatter)
                    .Within(0.0001f));
            Assert.That(
                haze.GetFloat("_VariationStrength"),
                Is.EqualTo(TitanHazeRenderingContract.HazeVariationStrength)
                    .Within(0.0001f));
            Assert.That(
                haze.renderQueue,
                Is.EqualTo((int)RenderQueue.Transparent + 12));
            Assert.That(haze.enableInstancing, Is.True);

            Assert.That(definition, Is.Not.Null);
            Assert.That(definition.BodyStableId, Is.EqualTo("titan"));
            Assert.That(definition.HasCloudLayer, Is.False);
            Assert.That(
                definition.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(TitanHazeRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                definition.AtmosphereCyclesPerRotation,
                Is.EqualTo(TitanHazeRenderingContract.HazeCyclesPerRotation)
                    .Within(0.0001f));
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.mipmapEnabled, Is.True);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }

        [Test]
        public void AirlessRockyMaterials_UseDistinctAnchoredSourcesAndContracts()
        {
            AssertAirlessRockyMaterial(
                "mercury",
                MercuryTexturePath,
                MercuryMaterialPath,
                MercuryVisualDefinitionPath,
                AirlessRockyVisualRenderingContract.MercuryReliefStrength,
                AirlessRockyVisualRenderingContract.MercuryReliefSampleDistance,
                AirlessRockyVisualRenderingContract.MercurySurfaceSpecular,
                AirlessRockyVisualRenderingContract.MercurySurfaceSmoothness,
                AirlessRockyVisualRenderingContract.MercuryNightsideReadability);
            AssertAirlessRockyMaterial(
                "moon",
                MoonTexturePath,
                MoonMaterialPath,
                MoonVisualDefinitionPath,
                AirlessRockyVisualRenderingContract.MoonReliefStrength,
                AirlessRockyVisualRenderingContract.MoonReliefSampleDistance,
                AirlessRockyVisualRenderingContract.MoonSurfaceSpecular,
                AirlessRockyVisualRenderingContract.MoonSurfaceSmoothness,
                AirlessRockyVisualRenderingContract.MoonNightsideReadability);
        }

        private static void AssertAirlessRockyMaterial(
            string stableId,
            string texturePath,
            string materialPath,
            string definitionPath,
            float reliefStrength,
            float reliefSampleDistance,
            float surfaceSpecular,
            float surfaceSmoothness,
            float nightsideReadability)
        {
            Texture2D texture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            Material material =
                AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            AirlessRockyVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<AirlessRockyVisualDefinition>(
                    definitionPath);
            var importer =
                AssetImporter.GetAtPath(texturePath) as TextureImporter;

            Assert.That(texture, Is.Not.Null, stableId);
            Assert.That(material, Is.Not.Null, stableId);
            Assert.That(
                material.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"),
                stableId);
            Assert.That(material.GetTexture("_BaseMap"), Is.SameAs(texture));
            Assert.That(
                material.GetFloat("_ReliefStrength"),
                Is.EqualTo(reliefStrength).Within(0.0001f));
            Assert.That(
                material.GetFloat("_ReliefSampleDistance"),
                Is.EqualTo(reliefSampleDistance).Within(0.0001f));
            Assert.That(
                material.GetFloat("_Specular"),
                Is.EqualTo(surfaceSpecular).Within(0.0001f));
            Assert.That(
                material.GetFloat("_Smoothness"),
                Is.EqualTo(surfaceSmoothness).Within(0.0001f));
            Assert.That(
                material.GetFloat("_NightsideReadability"),
                Is.EqualTo(nightsideReadability).Within(0.0001f));
            Assert.That(material.enableInstancing, Is.True);

            Assert.That(definition, Is.Not.Null, stableId);
            Assert.That(definition.BodyStableId, Is.EqualTo(stableId));
            Assert.That(
                definition.ReliefStrength,
                Is.EqualTo(reliefStrength).Within(0.0001f));
            Assert.That(
                definition.ReliefSampleDistance,
                Is.EqualTo(reliefSampleDistance).Within(0.0001f));
            Assert.That(
                definition.SurfaceSpecular,
                Is.EqualTo(surfaceSpecular).Within(0.0001f));
            Assert.That(
                definition.SurfaceSmoothness,
                Is.EqualTo(surfaceSmoothness).Within(0.0001f));
            Assert.That(
                definition.NightsideReadability,
                Is.EqualTo(nightsideReadability).Within(0.0001f));

            Assert.That(importer, Is.Not.Null, stableId);
            Assert.That(importer.sRGBTexture, Is.True);
            Assert.That(importer.mipmapEnabled, Is.True);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }

        private static void AssertIceGiantMaterial(
            string stableId,
            string texturePath,
            string surfacePath,
            string atmospherePath,
            string definitionPath,
            float flowStrength,
            float animatedDetailStrength,
            float normalStrength,
            float nightsideReadability,
            float atmosphereIntensity,
            float shellRadius,
            float cyclesPerRotation,
            int renderQueue)
        {
            Texture2D texture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            Material surface =
                AssetDatabase.LoadAssetAtPath<Material>(surfacePath);
            Material atmosphere =
                AssetDatabase.LoadAssetAtPath<Material>(atmospherePath);
            IceGiantVisualDefinition definition =
                AssetDatabase.LoadAssetAtPath<IceGiantVisualDefinition>(
                    definitionPath);
            var importer =
                AssetImporter.GetAtPath(texturePath) as TextureImporter;

            Assert.That(texture, Is.Not.Null, stableId);
            Assert.That(surface, Is.Not.Null, stableId);
            Assert.That(
                surface.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Surface"),
                stableId);
            Assert.That(surface.GetTexture("_BaseMap"), Is.SameAs(texture), stableId);
            Assert.That(
                surface.GetFloat("_FlowStrength"),
                Is.EqualTo(flowStrength).Within(0.0001f),
                stableId);
            Assert.That(
                surface.GetFloat("_AnimatedDetailStrength"),
                Is.EqualTo(animatedDetailStrength).Within(0.0001f),
                stableId);
            Assert.That(
                surface.GetFloat("_BandNormalStrength"),
                Is.EqualTo(normalStrength).Within(0.0001f),
                stableId);
            Assert.That(
                surface.GetFloat("_NightsideReadability"),
                Is.EqualTo(nightsideReadability).Within(0.0001f),
                stableId);
            Assert.That(surface.enableInstancing, Is.True, stableId);

            Assert.That(atmosphere, Is.Not.Null, stableId);
            Assert.That(
                atmosphere.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Atmosphere"),
                stableId);
            Assert.That(
                atmosphere.GetFloat("_RimIntensity"),
                Is.EqualTo(atmosphereIntensity).Within(0.0001f),
                stableId);
            Assert.That(atmosphere.renderQueue, Is.EqualTo(renderQueue), stableId);
            Assert.That(atmosphere.enableInstancing, Is.True, stableId);

            Assert.That(definition, Is.Not.Null, stableId);
            Assert.That(definition.BodyStableId, Is.EqualTo(stableId));
            Assert.That(
                definition.AtmosphereShellRadiusMultiplier,
                Is.EqualTo(shellRadius).Within(0.0001f),
                stableId);
            Assert.That(
                definition.DetailCyclesPerRotation,
                Is.EqualTo(cyclesPerRotation).Within(0.0001f),
                stableId);
            Assert.That(importer, Is.Not.Null, stableId);
            Assert.That(importer.sRGBTexture, Is.True, stableId);
            Assert.That(importer.mipmapEnabled, Is.True, stableId);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Repeat), stableId);
        }
    }
}
