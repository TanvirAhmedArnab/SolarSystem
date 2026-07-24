using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Creates or updates Slice 2 scientific definitions, scale data, and materials.</summary>
    internal static class SolarSystemSlice2AssetBuilder
    {
        private const string DataRoot = "Assets/SolarSystem/Content/Data";
        private const string MaterialRoot = "Assets/SolarSystem/Content/Materials";
        private const string RenderingSettingsRoot = "Assets/SolarSystem/Settings/Rendering";
        private const string CelestialTextureRoot =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies";
        private const string EarthNormalPath =
            CelestialTextureRoot + "/Earth/T_Earth_Normal_2K.tif";
        private const string EarthSpecularPath =
            CelestialTextureRoot + "/Earth/T_Earth_Specular_2K.tif";
        private const string EarthNightPath =
            CelestialTextureRoot + "/Earth/T_Earth_NightEmission_2K.jpg";
        private const string EarthCloudPath =
            CelestialTextureRoot + "/Earth/T_Earth_Clouds_2K.jpg";
        private const string SunTexturePath =
            CelestialTextureRoot + "/Sun/T_Sun_Surface_2K.jpg";
        private const string JupiterTexturePath =
            CelestialTextureRoot + "/Jupiter/T_Jupiter_Surface_2K.jpg";
        private const string SaturnTexturePath =
            CelestialTextureRoot + "/Saturn/T_Saturn_Surface_2K.jpg";
        private const string SolarSurfaceShader =
            "SolarSystem/Celestial/Solar Surface";
        private const string SolarCoronaShader =
            "SolarSystem/Celestial/Solar Corona";
        private const string GasGiantSurfaceShader =
            "SolarSystem/Celestial/Gas Giant Surface";
        private const string GasGiantAtmosphereShader =
            "SolarSystem/Celestial/Gas Giant Atmosphere";
        private const string SaturnRingShader =
            "SolarSystem/Celestial/Saturn Rings";
        private const string EarthSurfaceShader =
            "SolarSystem/Celestial/Earth Surface";
        private const string EarthCloudShader =
            "SolarSystem/Celestial/Earth Cloud Layer";
        private const string AtmosphereShader =
            "SolarSystem/Celestial/Atmosphere Rim";
        private const string SaturnRingTexturePath =
            CelestialTextureRoot + "/Saturn/T_Saturn_RingsAlpha_2K.png";
        private const string SpaceTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/Environment/T_Space_MilkyWay_2K.jpg";
        private const string MusicClipPath =
            "Assets/SolarSystem/Content/Audio/Music/A_Music_OuterSpaceLoop.mp3";
        private const string SunAmbienceClipPath =
            "Assets/SolarSystem/Content/Audio/Ambience/CelestialBodies/Sun/A_Sun_BurningLoop.wav";
        private const string EarthAmbienceClipPath =
            "Assets/SolarSystem/Content/Audio/Ambience/CelestialBodies/Earth/A_Earth_ForestAmbienceLoop.mp3";
        private const string SelectionClipPath =
            "Assets/SolarSystem/Content/Audio/SFX/UI/A_UI_Select.ogg";
        private const string FocusClipPath =
            "Assets/SolarSystem/Content/Audio/SFX/UI/A_UI_FocusConfirmation.ogg";
        private const string TimeControlClipPath =
            "Assets/SolarSystem/Content/Audio/SFX/UI/A_UI_TimeTick.ogg";
        private const string ScaleComparisonClipPath =
            "Assets/SolarSystem/Content/Audio/SFX/UI/A_UI_ToggleScale.ogg";
        private const float SunSmoothness = 0f;
        private const float MercurySmoothness = 0.08f;
        private const float VenusSmoothness = 0.24f;
        private const float EarthSmoothness = 0.22f;
        private const float MoonSmoothness = 0.08f;
        private const float MarsSmoothness = 0.1f;
        private const float JupiterSmoothness = 0.18f;
        private const float SaturnSmoothness = 0.2f;
        private const float UranusSmoothness = 0.28f;
        private const float NeptuneSmoothness = 0.3f;
        private const int SaturnRingSegments = 128;
        private const float SaturnRingInnerRadius = 0.62f;
        private const float SaturnRingOuterRadius = 1.15f;
        private const float OrbitSmoothness = 0f;
        private const float SkyboxExposure = 0.62f;
        private const float BloomThreshold = 1.1f;
        private const float BloomIntensity = 0.32f;
        private const float BloomScatter = 0.55f;
        private const float PostExposure = -0.1f;
        private const float ColorContrast = 6f;
        private const float ColorSaturation = -2f;
        private const float VignetteIntensity = 0.12f;
        private const float VignetteSmoothness = 0.32f;
        private const double SecondsPerDay = 86400d;
        private const double AstronomicalUnitKm = 149597870.7d;
        private static readonly Color SunTint = new Color(1.55f, 0.78f, 0.18f, 1f);
        private static readonly Color SunHotTint = new Color(0.55f, 0.16f, 0.02f, 1f);
        private static readonly Color CoronaTint = new Color(6f, 2f, 0.2f, 1f);
        private static readonly Color MercuryTint = new Color(0.9f, 0.88f, 0.84f, 1f);
        private static readonly Color VenusTint = new Color(1f, 0.9f, 0.72f, 1f);
        private static readonly Color EarthTint = new Color(0.9f, 0.94f, 1f, 1f);
        private static readonly Color MoonTint = new Color(0.82f, 0.82f, 0.8f, 1f);
        private static readonly Color MarsTint = new Color(1f, 0.78f, 0.68f, 1f);
        private static readonly Color JupiterTint = new Color(0.96f, 0.9f, 0.84f, 1f);
        private static readonly Color JupiterAtmosphereTint =
            new Color(0.76f, 0.58f, 0.42f, 1f);
        private static readonly Color SaturnTint = new Color(1f, 0.93f, 0.78f, 1f);
        private static readonly Color SaturnAtmosphereTint =
            new Color(0.82f, 0.7f, 0.5f, 1f);
        private static readonly Color UranusTint = new Color(0.78f, 0.95f, 1f, 1f);
        private static readonly Color NeptuneTint = new Color(0.72f, 0.84f, 1f, 1f);
        private static readonly Color OrbitTint = new Color(0.16f, 0.45f, 0.78f, 1f);
        private static readonly Color SkyboxTint = new Color(0.72f, 0.78f, 0.9f, 1f);
        private static readonly Color ColorFilter = new Color(0.98f, 0.99f, 1f, 1f);
        private static readonly Color VignetteColor =
            new Color(0.005f, 0.008f, 0.015f, 1f);

        internal static SolarSystemSlice2Content Build()
        {
            EnsureFolder($"{DataRoot}/CelestialBodies");
            EnsureFolder($"{DataRoot}/Scale");
            EnsureFolder($"{DataRoot}/VisualLayers");
            EnsureFolder($"{MaterialRoot}/CelestialBodies");
            EnsureFolder($"{MaterialRoot}/Environment");
            EnsureFolder(RenderingSettingsRoot);
            EnsureFolder("Assets/SolarSystem/Scenes");
            ConfigureTextureImports();

            SolarSystemSlice2BodyContent[] bodies =
            {
                CreateBodyContent(
                    CreateBody(new SolarSystemSlice2BodyData(
                        "Body_Sun",
                        "sun",
                        "Sun",
                        CelestialBodyCategory.Star,
                        string.Empty,
                        "NASA_NSSDC_SUN_EARTH_FACT_SHEET",
                        "The star at the center of our Solar System and the source of nearly all its light and heat.",
                        695700d,
                        1.9884e30d,
                        609.12d * 3600d,
                        7.25d,
                        null)),
                    CreateBodyMaterial("Sun", "T_Sun_Surface_2K.jpg", SunTint, SunSmoothness, true),
                    0,
                    0f),
                CreatePlanetContent(
                    "Mercury",
                    "mercury",
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_MERCURY_FACTS",
                    "The smallest and fastest planet, a cratered rocky world closest to the Sun.",
                    2439.4d,
                    3.30103e23d,
                    58.6462d,
                    2d,
                    0.38709927d,
                    0.20563593d,
                    7.00497902d,
                    48.33076593d,
                    77.45779628d,
                    252.25032350d,
                    0.2408467d,
                    MercuryTint,
                    MercurySmoothness,
                    192,
                    0.035f),
                CreatePlanetContent(
                    "Venus",
                    "venus",
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_VENUS_FACTS",
                    "A cloud-covered rocky planet with extreme surface heat and slow retrograde rotation.",
                    6051.8d,
                    4.86731e24d,
                    -243.018d,
                    3d,
                    0.72333566d,
                    0.00677672d,
                    3.39467605d,
                    76.67984255d,
                    131.60246718d,
                    181.97909950d,
                    0.61519726d,
                    VenusTint,
                    VenusSmoothness,
                    192,
                    0.045f),
                CreatePlanetContent(
                    "Earth",
                    "earth",
                    "NASA_NSSDC_EARTH_AND_JPL_APPROX_POS_J2000",
                    "A rocky world with global oceans and the only known environment that supports life.",
                    CelestialReferenceUnits.EarthMeanRadiusKm,
                    5.9722e24d,
                    23.9345d / 24d,
                    23.44d,
                    1.00000261d,
                    0.01671123d,
                    -0.00001531d,
                    0d,
                    102.93768193d,
                    100.46457166d,
                    365.256363004d / 365.25d,
                    EarthTint,
                    EarthSmoothness,
                    192,
                    0.055f),
                CreateBodyContent(
                    CreateBody(new SolarSystemSlice2BodyData(
                        "Body_Moon",
                        "moon",
                        "Moon",
                        CelestialBodyCategory.Moon,
                        "earth",
                        "NASA_MOON_BY_NUMBERS_AND_JPL_DE405_LE405",
                        "Earth's natural satellite; its gravity drives most ocean tides and its orbit is shown around Earth.",
                        1737.4d,
                        7.34767309245735e22d,
                        27.322d * SecondsPerDay,
                        6.68d,
                        new SolarSystemSlice2OrbitData(
                            384400d,
                            0.0554d,
                            5.16d,
                            125.08d,
                            318.15d,
                            135.27d,
                            27.322d * SecondsPerDay))),
                    CreateBodyMaterial(
                        "Moon",
                        "T_Moon_Surface_2K.jpg",
                        MoonTint,
                        MoonSmoothness),
                    128,
                    0.025f),
                CreatePlanetContent(
                    "Mars",
                    "mars",
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_MARS_FACTS",
                    "A cold desert world whose iron-rich surface, giant volcanoes, and ancient river valleys record a dynamic past.",
                    3389.5d,
                    6.41691e23d,
                    1.02595676d,
                    25.2d,
                    1.52371034d,
                    0.09339410d,
                    1.84969142d,
                    49.55953891d,
                    -23.94362959d,
                    -4.55343205d,
                    1.8808476d,
                    MarsTint,
                    MarsSmoothness,
                    224,
                    0.055f),
                CreatePlanetContent(
                    "Jupiter",
                    "jupiter",
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_JUPITER_FACTS",
                    "The Solar System's largest planet, a hydrogen-helium giant with rapid rotation and powerful storms.",
                    69911d,
                    1.898125e27d,
                    0.41354d,
                    3d,
                    5.202887d,
                    0.04838624d,
                    1.30439695d,
                    100.47390909d,
                    14.72847983d,
                    34.39644051d,
                    11.862615d,
                    JupiterTint,
                    JupiterSmoothness,
                    256,
                    0.065f),
                CreatePlanetContent(
                    "Saturn",
                    "saturn",
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_SATURN_FACTS",
                    "A low-density gas giant encircled by an extensive system of ice-rich rings.",
                    58232d,
                    5.68317e26d,
                    0.44401d,
                    26.73d,
                    9.53667594d,
                    0.05386179d,
                    2.48599187d,
                    113.66242448d,
                    92.59887831d,
                    49.95424423d,
                    29.447498d,
                    SaturnTint,
                    SaturnSmoothness,
                    288,
                    0.07f),
                CreatePlanetContent(
                    "Uranus",
                    "uranus",
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_URANUS_FACTS",
                    "An ice giant with a blue-green atmosphere and an extreme sideways axial tilt.",
                    25362d,
                    8.68099e25d,
                    -0.71833d,
                    97.77d,
                    19.18916464d,
                    0.04725744d,
                    0.77263783d,
                    74.01692503d,
                    170.95427630d,
                    313.23810451d,
                    84.016846d,
                    UranusTint,
                    UranusSmoothness,
                    320,
                    0.075f),
                CreatePlanetContent(
                    "Neptune",
                    "neptune",
                    "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_NEPTUNE_FACTS",
                    "A distant blue ice giant with supersonic winds and a long, season-bearing orbit.",
                    24622d,
                    1.024092e26d,
                    0.67125d,
                    28d,
                    30.06992276d,
                    0.00859048d,
                    1.77004347d,
                    131.78422574d,
                    44.96476227d,
                    -55.12002969d,
                    164.79132d,
                    NeptuneTint,
                    NeptuneSmoothness,
                    352,
                    0.08f)
            };

            CelestialCatalogDefinition catalog = CreateOrLoad<CelestialCatalogDefinition>(
                $"{DataRoot}/Catalog_SolarSystem.asset");
            SetCatalogBodies(catalog, bodies);
            PresentationScaleDefinition scale = CreateOrLoad<PresentationScaleDefinition>(
                $"{DataRoot}/Scale/Scale_PresentationGraybox.asset");
            ConfigureScale(scale);
            Material orbitMaterial = CreateMaterial(
                $"{MaterialRoot}/Environment/M_OrbitPath.mat",
                "Universal Render Pipeline/Unlit",
                null,
                OrbitTint,
                OrbitSmoothness);
            Material skyboxMaterial = CreateSkyboxMaterial();
            VolumeProfile visualProfile = CreateVisualProfile();
            ConfigureAudioImporters();

            return new SolarSystemSlice2Content
            {
                Bodies = bodies,
                Catalog = catalog,
                Scale = scale,
                SaturnRingMesh = CreateSaturnRingMesh(),
                SaturnRingMaterial = CreateSaturnRingMaterial(),
                SunVisualDefinition = CreateSunVisualDefinition(),
                SunCoronaMaterial = CreateSunCoronaMaterial(),
                EarthLayerDefinition = CreateEarthLayerDefinition(),
                EarthCloudMaterial = CreateEarthCloudMaterial(),
                EarthAtmosphereMaterial = CreateEarthAtmosphereMaterial(),
                JupiterVisualDefinition = CreateJupiterVisualDefinition(),
                JupiterAtmosphereMaterial = CreateJupiterAtmosphereMaterial(),
                SaturnVisualDefinition = CreateSaturnVisualDefinition(),
                SaturnAtmosphereMaterial = CreateSaturnAtmosphereMaterial(),
                OrbitMaterial = orbitMaterial,
                SkyboxMaterial = skyboxMaterial,
                VisualProfile = visualProfile,
                MusicClip = LoadRequiredAsset<AudioClip>(MusicClipPath),
                SunAmbienceClip = LoadRequiredAsset<AudioClip>(SunAmbienceClipPath),
                EarthAmbienceClip = LoadRequiredAsset<AudioClip>(EarthAmbienceClipPath),
                SelectionClip = LoadRequiredAsset<AudioClip>(SelectionClipPath),
                FocusClip = LoadRequiredAsset<AudioClip>(FocusClipPath),
                TimeControlClip = LoadRequiredAsset<AudioClip>(TimeControlClipPath),
                ScaleComparisonClip =
                    LoadRequiredAsset<AudioClip>(ScaleComparisonClipPath)
            };
        }

        private static void ConfigureAudioImporters()
        {
            ConfigureAudioImporter(MusicClipPath, true, false);
            ConfigureAudioImporter(SunAmbienceClipPath, true, true);
            ConfigureAudioImporter(EarthAmbienceClipPath, true, true);
            ConfigureAudioImporter(SelectionClipPath, false, true);
            ConfigureAudioImporter(FocusClipPath, false, true);
            ConfigureAudioImporter(TimeControlClipPath, false, true);
            ConfigureAudioImporter(ScaleComparisonClipPath, false, true);
        }

        private static void ConfigureAudioImporter(
            string path,
            bool streaming,
            bool forceToMono)
        {
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null)
            {
                throw new InvalidOperationException(
                    $"Required audio importer '{path}' is missing.");
            }

            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            AudioClipLoadType expectedLoadType =
                streaming ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
            bool changed =
                importer.forceToMono != forceToMono ||
                importer.loadInBackground != streaming ||
                settings.loadType != expectedLoadType ||
                settings.preloadAudioData == streaming ||
                settings.compressionFormat != AudioCompressionFormat.Vorbis ||
                Math.Abs(settings.quality - 0.7f) > 0.0001f ||
                settings.sampleRateSetting != AudioSampleRateSetting.OptimizeSampleRate;

            if (!changed)
            {
                return;
            }

            importer.forceToMono = forceToMono;
            importer.loadInBackground = streaming;
            settings.loadType = expectedLoadType;
            settings.preloadAudioData = !streaming;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            settings.quality = 0.7f;
            settings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            importer.defaultSampleSettings = settings;
            importer.SaveAndReimport();
        }

        private static SolarSystemSlice2BodyContent CreatePlanetContent(
            string displayName,
            string stableId,
            string sourceId,
            string educationalSummary,
            double radiusKm,
            double massKg,
            double rotationPeriodDays,
            double axialTiltDeg,
            double semiMajorAxisAu,
            double eccentricity,
            double inclinationDeg,
            double ascendingNodeDeg,
            double longitudePerihelionDeg,
            double meanLongitudeDeg,
            double orbitalPeriodYears,
            Color tint,
            float smoothness,
            int orbitSampleCount,
            float orbitWidth)
        {
            var bodyData = new SolarSystemSlice2BodyData(
                $"Body_{displayName}",
                stableId,
                displayName,
                CelestialBodyCategory.Planet,
                "sun",
                sourceId,
                educationalSummary,
                radiusKm,
                massKg,
                rotationPeriodDays * SecondsPerDay,
                axialTiltDeg,
                new SolarSystemSlice2OrbitData(
                    AstronomicalUnitKm * semiMajorAxisAu,
                    eccentricity,
                    inclinationDeg,
                    ascendingNodeDeg,
                    longitudePerihelionDeg - ascendingNodeDeg,
                    meanLongitudeDeg - longitudePerihelionDeg,
                    orbitalPeriodYears * 365.25d * SecondsPerDay));
            string textureName = displayName == "Earth"
                ? "T_Earth_DayAlbedo_2K.jpg"
                : $"T_{displayName}_Surface_2K.jpg";
            Material material = CreateBodyMaterial(
                displayName,
                textureName,
                tint,
                smoothness);
            if (displayName == "Earth")
            {
                ConfigureEarthSurfaceMaterial(material);
            }

            return CreateBodyContent(
                CreateBody(bodyData),
                material,
                orbitSampleCount,
                orbitWidth);
        }

        private static SolarSystemSlice2BodyContent CreateBodyContent(
            CelestialBodyDefinition definition,
            Material material,
            int orbitSampleCount,
            float orbitWidth)
        {
            return new SolarSystemSlice2BodyContent
            {
                Definition = definition,
                Material = material,
                OrbitSampleCount = orbitSampleCount,
                OrbitWidth = orbitWidth
            };
        }

        private static Material CreateBodyMaterial(
            string bodyName,
            string textureName,
            Color tint,
            float smoothness,
            bool unlit = false)
        {
            if (bodyName == "Sun")
            {
                Material sunMaterial = CreateOrUpdateMaterial(
                    $"{MaterialRoot}/CelestialBodies/M_Sun.mat",
                    SolarSurfaceShader);
                ConfigureSunMaterial(sunMaterial);
                return sunMaterial;
            }

            if (bodyName == "Earth")
            {
                Material earthMaterial = CreateOrUpdateMaterial(
                    $"{MaterialRoot}/CelestialBodies/M_Earth.mat",
                    EarthSurfaceShader);
                earthMaterial.SetTexture(
                    "_BaseMap",
                    LoadRequiredAsset<Texture2D>(
                        $"{CelestialTextureRoot}/Earth/{textureName}"));
                earthMaterial.SetColor("_BaseColor", tint);
                return earthMaterial;
            }

            if (bodyName == "Jupiter")
            {
                Material jupiterMaterial = CreateOrUpdateMaterial(
                    $"{MaterialRoot}/CelestialBodies/M_Jupiter.mat",
                    GasGiantSurfaceShader);
                ConfigureJupiterSurfaceMaterial(jupiterMaterial);
                return jupiterMaterial;
            }

            if (bodyName == "Saturn")
            {
                Material saturnMaterial = CreateOrUpdateMaterial(
                    $"{MaterialRoot}/CelestialBodies/M_Saturn.mat",
                    GasGiantSurfaceShader);
                ConfigureSaturnSurfaceMaterial(saturnMaterial);
                return saturnMaterial;
            }

            Material material = CreateMaterial(
                $"{MaterialRoot}/CelestialBodies/M_{bodyName}.mat",
                unlit
                    ? "Universal Render Pipeline/Unlit"
                    : "Universal Render Pipeline/Lit",
                $"{CelestialTextureRoot}/{bodyName}/{textureName}",
                tint,
                smoothness);
            if (!unlit && bodyName != "Earth")
            {
                ConfigureLitMaterial(material, null, 0f);
            }

            return material;
        }

        private static void ConfigureTextureImports()
        {
            string[] colorTextures =
            {
                $"{CelestialTextureRoot}/Sun/T_Sun_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Mercury/T_Mercury_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Venus/T_Venus_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Earth/T_Earth_DayAlbedo_2K.jpg",
                EarthNightPath,
                $"{CelestialTextureRoot}/Moon/T_Moon_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Mars/T_Mars_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Jupiter/T_Jupiter_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Saturn/T_Saturn_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Uranus/T_Uranus_Surface_2K.jpg",
                $"{CelestialTextureRoot}/Neptune/T_Neptune_Surface_2K.jpg"
            };
            foreach (string path in colorTextures)
            {
                ConfigureTextureImporter(
                    path,
                    TextureImporterType.Default,
                    true,
                    TextureWrapMode.Repeat);
            }

            ConfigureTextureImporter(
                SpaceTexturePath,
                TextureImporterType.Default,
                true,
                TextureWrapMode.Repeat);
            ConfigureTextureImporter(
                EarthNormalPath,
                TextureImporterType.NormalMap,
                false,
                TextureWrapMode.Repeat);
            ConfigureTextureImporter(
                EarthSpecularPath,
                TextureImporterType.Default,
                false,
                TextureWrapMode.Repeat);
            ConfigureTextureImporter(
                EarthCloudPath,
                TextureImporterType.Default,
                false,
                TextureWrapMode.Repeat);
            ConfigureTextureImporter(
                SaturnRingTexturePath,
                TextureImporterType.Default,
                true,
                TextureWrapMode.Clamp);
        }

        private static void ConfigureTextureImporter(
            string path,
            TextureImporterType textureType,
            bool sRgb,
            TextureWrapMode wrapMode)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException(
                    $"Required texture importer is unavailable at '{path}'.");
            }

            bool changed =
                importer.textureType != textureType ||
                importer.sRGBTexture != sRgb ||
                importer.wrapMode != wrapMode ||
                !importer.mipmapEnabled;
            if (!changed)
            {
                return;
            }

            importer.textureType = textureType;
            importer.sRGBTexture = sRgb;
            importer.wrapMode = wrapMode;
            importer.mipmapEnabled = true;
            importer.SaveAndReimport();
        }

        private static void ConfigureSunMaterial(Material material)
        {
            material.SetTexture(
                "_BaseMap",
                LoadRequiredAsset<Texture2D>(SunTexturePath));
            material.SetColor("_BaseColor", SunTint);
            material.SetColor("_HotColor", SunHotTint);
            material.SetFloat(
                "_FlowStrength",
                SolarVisualRenderingContract.SurfaceFlowStrength);
            material.SetFloat("_SecondaryBlend", 0.16f);
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
        }

        private static SolarVisualDefinition CreateSunVisualDefinition()
        {
            const string path =
                DataRoot + "/VisualLayers/VisualLayers_Sun.asset";
            SolarVisualDefinition definition =
                CreateOrLoad<SolarVisualDefinition>(path);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "sun";
            serialized.FindProperty("coronaShellRadiusMultiplier").floatValue =
                SolarVisualRenderingContract.CoronaShellRadiusMultiplier;
            serialized.FindProperty("surfaceFlowCyclesPerRotation").floatValue =
                SolarVisualRenderingContract.SurfaceFlowCyclesPerRotation;
            serialized.FindProperty("coronaFlowCyclesPerRotation").floatValue =
                SolarVisualRenderingContract.CoronaFlowCyclesPerRotation;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static Material CreateSunCoronaMaterial()
        {
            const string path =
                MaterialRoot + "/CelestialBodies/M_Sun_Corona.mat";
            Material material = CreateOrUpdateMaterial(path, SolarCoronaShader);
            material.SetTexture(
                "_SolarMap",
                LoadRequiredAsset<Texture2D>(SunTexturePath));
            material.SetColor("_CoronaColor", CoronaTint);
            material.SetFloat("_RimPower", 3.2f);
            material.SetFloat(
                "_Intensity",
                SolarVisualRenderingContract.CoronaIntensity);
            material.SetFloat("_PulseAmplitude", 0.025f);
            material.SetFloat(
                "_FlowStrength",
                SolarVisualRenderingContract.CoronaFlowStrength);
            material.renderQueue = (int)RenderQueue.Transparent + 20;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ConfigureEarthSurfaceMaterial(Material material)
        {
            material.SetTexture(
                "_BaseMap",
                LoadRequiredAsset<Texture2D>(
                    $"{CelestialTextureRoot}/Earth/T_Earth_DayAlbedo_2K.jpg"));
            material.SetColor("_BaseColor", EarthTint);
            material.SetTexture(
                "_BumpMap",
                LoadRequiredAsset<Texture2D>(EarthNormalPath));
            material.SetFloat(
                "_BumpScale",
                EarthLayerRenderingContract.NormalStrength);
            material.SetTexture(
                "_SpecularMap",
                LoadRequiredAsset<Texture2D>(EarthSpecularPath));
            material.SetFloat("_LandSpecular", 0.025f);
            material.SetFloat("_OceanSpecular", 0.34f);
            material.SetFloat("_LandSmoothness", 0.18f);
            material.SetFloat("_OceanSmoothness", 0.64f);
            material.SetTexture(
                "_NightMap",
                LoadRequiredAsset<Texture2D>(EarthNightPath));
            material.SetColor(
                "_NightColor",
                new Color(1.2f, 0.68f, 0.28f, 1f));
            material.SetFloat("_NightIntensity", 1.1f);
            material.SetFloat(
                "_NightFadeStart",
                EarthLayerRenderingContract.NightFadeStart);
            material.SetFloat(
                "_NightFadeEnd",
                EarthLayerRenderingContract.NightFadeEnd);
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
        }

        private static void ConfigureJupiterSurfaceMaterial(Material material)
        {
            material.SetTexture(
                "_BaseMap",
                LoadRequiredAsset<Texture2D>(JupiterTexturePath));
            material.SetColor("_BaseColor", JupiterTint);
            material.SetFloat(
                "_BandNormalStrength",
                GasGiantVisualRenderingContract.BandNormalStrength);
            material.SetFloat("_BandSampleDistance", 1.5f);
            material.SetFloat(
                "_FlowStrength",
                GasGiantVisualRenderingContract.BandFlowStrength);
            material.SetFloat(
                "_AnimatedDetailStrength",
                GasGiantVisualRenderingContract.AnimatedDetailStrength);
            material.SetFloat("_Specular", 0.08f);
            material.SetFloat("_Smoothness", JupiterSmoothness);
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
        }

        private static GasGiantVisualDefinition CreateJupiterVisualDefinition()
        {
            const string path =
                DataRoot + "/VisualLayers/VisualLayers_Jupiter.asset";
            GasGiantVisualDefinition definition =
                CreateOrLoad<GasGiantVisualDefinition>(path);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "jupiter";
            serialized.FindProperty("atmosphereShellRadiusMultiplier").floatValue =
                GasGiantVisualRenderingContract.AtmosphereShellRadiusMultiplier;
            serialized.FindProperty("bandFlowCyclesPerRotation").floatValue =
                GasGiantVisualRenderingContract.BandFlowCyclesPerRotation;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static Material CreateJupiterAtmosphereMaterial()
        {
            const string path =
                MaterialRoot + "/CelestialBodies/M_Jupiter_Atmosphere.mat";
            Material material =
                CreateOrUpdateMaterial(path, GasGiantAtmosphereShader);
            material.SetColor("_AtmosphereColor", JupiterAtmosphereTint);
            material.SetFloat("_RimPower", 4.8f);
            material.SetFloat(
                "_RimIntensity",
                GasGiantVisualRenderingContract.AtmosphereIntensity);
            material.SetFloat("_NightsideVisibility", 0.04f);
            material.renderQueue = (int)RenderQueue.Transparent + 12;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ConfigureSaturnSurfaceMaterial(Material material)
        {
            material.SetTexture(
                "_BaseMap",
                LoadRequiredAsset<Texture2D>(SaturnTexturePath));
            material.SetColor("_BaseColor", SaturnTint);
            material.SetFloat(
                "_BandNormalStrength",
                GasGiantVisualRenderingContract.SaturnBandNormalStrength);
            material.SetFloat("_BandSampleDistance", 1.5f);
            material.SetFloat(
                "_FlowStrength",
                GasGiantVisualRenderingContract.SaturnBandFlowStrength);
            material.SetFloat(
                "_AnimatedDetailStrength",
                GasGiantVisualRenderingContract.SaturnAnimatedDetailStrength);
            material.SetFloat("_Specular", 0.06f);
            material.SetFloat("_Smoothness", SaturnSmoothness);
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
        }

        private static GasGiantVisualDefinition CreateSaturnVisualDefinition()
        {
            const string path =
                DataRoot + "/VisualLayers/VisualLayers_Saturn.asset";
            GasGiantVisualDefinition definition =
                CreateOrLoad<GasGiantVisualDefinition>(path);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "saturn";
            serialized.FindProperty("atmosphereShellRadiusMultiplier").floatValue =
                GasGiantVisualRenderingContract.SaturnAtmosphereShellRadiusMultiplier;
            serialized.FindProperty("bandFlowCyclesPerRotation").floatValue =
                GasGiantVisualRenderingContract.SaturnBandFlowCyclesPerRotation;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static Material CreateSaturnAtmosphereMaterial()
        {
            const string path =
                MaterialRoot + "/CelestialBodies/M_Saturn_Atmosphere.mat";
            Material material =
                CreateOrUpdateMaterial(path, GasGiantAtmosphereShader);
            material.SetColor("_AtmosphereColor", SaturnAtmosphereTint);
            material.SetFloat("_RimPower", 5.2f);
            material.SetFloat(
                "_RimIntensity",
                GasGiantVisualRenderingContract.SaturnAtmosphereIntensity);
            material.SetFloat("_NightsideVisibility", 0.035f);
            material.renderQueue = (int)RenderQueue.Transparent + 11;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static CelestialLayerVisualDefinition CreateEarthLayerDefinition()
        {
            const string path =
                DataRoot + "/VisualLayers/VisualLayers_Earth.asset";
            CelestialLayerVisualDefinition definition =
                CreateOrLoad<CelestialLayerVisualDefinition>(path);
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("bodyStableId").stringValue = "earth";
            serialized.FindProperty("cloudShellRadiusMultiplier").floatValue =
                EarthLayerRenderingContract.CloudShellRadiusMultiplier;
            serialized.FindProperty("atmosphereShellRadiusMultiplier").floatValue =
                EarthLayerRenderingContract.AtmosphereShellRadiusMultiplier;
            serialized.FindProperty("cloudRotationMultiplier").floatValue =
                EarthLayerRenderingContract.CloudRotationMultiplier;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static Material CreateEarthCloudMaterial()
        {
            const string path =
                MaterialRoot + "/CelestialBodies/M_Earth_Clouds.mat";
            Material material = CreateOrUpdateMaterial(path, EarthCloudShader);
            material.SetTexture(
                "_CloudMap",
                LoadRequiredAsset<Texture2D>(EarthCloudPath));
            material.SetColor(
                "_CloudColor",
                new Color(0.95f, 0.98f, 1f, 1f));
            material.SetFloat("_CoverageThreshold", 0.16f);
            material.SetFloat("_CoverageContrast", 2.4f);
            material.SetFloat("_Opacity", 0.62f);
            material.SetFloat("_AmbientBrightness", 0.08f);
            material.SetFloat("_SunBrightness", 1f);
            material.renderQueue = (int)RenderQueue.Transparent;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateEarthAtmosphereMaterial()
        {
            const string path =
                MaterialRoot + "/CelestialBodies/M_Earth_Atmosphere.mat";
            Material material = CreateOrUpdateMaterial(path, AtmosphereShader);
            material.SetColor(
                "_AtmosphereColor",
                new Color(0.12f, 0.46f, 1f, 1f));
            material.SetFloat("_RimPower", 4.2f);
            material.SetFloat("_RimIntensity", 0.38f);
            material.SetFloat("_NightsideVisibility", 0.12f);
            material.renderQueue = (int)RenderQueue.Transparent + 10;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ConfigureLitMaterial(
            Material material,
            Texture2D normalMap,
            float normalScale)
        {
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_EnvironmentReflections", 1f);
            material.SetFloat("_SpecularHighlights", 1f);
            material.enableInstancing = true;
            material.SetTexture("_BumpMap", normalMap);
            material.SetFloat("_BumpScale", normalMap != null ? normalScale : 0f);
            if (normalMap != null)
            {
                material.EnableKeyword("_NORMALMAP");
            }
            else
            {
                material.DisableKeyword("_NORMALMAP");
            }

            EditorUtility.SetDirty(material);
        }

        private static Material CreateSaturnRingMaterial()
        {
            const string path = MaterialRoot + "/CelestialBodies/M_Saturn_Rings.mat";
            Material material = CreateOrUpdateMaterial(path, SaturnRingShader);
            material.SetTexture(
                "_BaseMap",
                LoadRequiredAsset<Texture2D>(SaturnRingTexturePath));
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_Opacity", SaturnRingRenderingContract.Opacity);
            material.SetFloat(
                "_AmbientBrightness",
                SaturnRingRenderingContract.AmbientBrightness);
            material.SetFloat(
                "_DayBrightness",
                SaturnRingRenderingContract.DayBrightness);
            material.SetFloat(
                "_ScatteringStrength",
                SaturnRingRenderingContract.ScatteringStrength);
            material.renderQueue = (int)RenderQueue.Transparent;
            material.doubleSidedGI = true;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Mesh CreateSaturnRingMesh()
        {
            const string meshRoot = "Assets/SolarSystem/Content/Models/Generated";
            const string path = meshRoot + "/SM_Saturn_Rings.asset";
            EnsureFolder(meshRoot);
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh == null)
            {
                mesh = new Mesh();
                AssetDatabase.CreateAsset(mesh, path);
            }

            int vertexCount = (SaturnRingSegments + 1) * 2;
            var vertices = new Vector3[vertexCount];
            var normals = new Vector3[vertexCount];
            var uv = new Vector2[vertexCount];
            var triangles = new int[SaturnRingSegments * 6];
            for (int segment = 0; segment <= SaturnRingSegments; segment++)
            {
                float ratio = (float)segment / SaturnRingSegments;
                float angle = ratio * Mathf.PI * 2f;
                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);
                int inner = segment * 2;
                int outer = inner + 1;
                vertices[inner] = new Vector3(
                    x * SaturnRingInnerRadius,
                    0f,
                    z * SaturnRingInnerRadius);
                vertices[outer] = new Vector3(
                    x * SaturnRingOuterRadius,
                    0f,
                    z * SaturnRingOuterRadius);
                normals[inner] = Vector3.up;
                normals[outer] = Vector3.up;
                uv[inner] = new Vector2(0f, ratio);
                uv[outer] = new Vector2(1f, ratio);
            }

            for (int segment = 0; segment < SaturnRingSegments; segment++)
            {
                int triangle = segment * 6;
                int inner = segment * 2;
                int outer = inner + 1;
                int nextInner = inner + 2;
                int nextOuter = outer + 2;
                triangles[triangle] = inner;
                triangles[triangle + 1] = nextOuter;
                triangles[triangle + 2] = outer;
                triangles[triangle + 3] = inner;
                triangles[triangle + 4] = nextInner;
                triangles[triangle + 5] = nextOuter;
            }

            mesh.Clear();
            mesh.name = "SM_Saturn_Rings";
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            EditorUtility.SetDirty(mesh);
            return mesh;
        }

        private static Material CreateSkyboxMaterial()
        {
            const string path = MaterialRoot + "/Environment/M_SpaceSkybox.mat";
            Shader shader = Shader.Find("Skybox/Panoramic");
            if (shader == null)
            {
                throw new InvalidOperationException(
                    "Required shader 'Skybox/Panoramic' is unavailable.");
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.shader = shader;
            }

            material.SetTexture(
                "_MainTex",
                AssetDatabase.LoadAssetAtPath<Texture2D>(SpaceTexturePath));
            material.SetColor("_Tint", SkyboxTint);
            material.SetFloat("_Exposure", SkyboxExposure);
            material.SetFloat("_Rotation", 0f);
            material.DisableKeyword("_MAPPING_6_FRAMES_LAYOUT");
            material.DisableKeyword("_MAPPING_LATITUDE_LONGITUDE_LAYOUT");
            EditorUtility.SetDirty(material);
            return material;
        }

        private static VolumeProfile CreateVisualProfile()
        {
            const string path = RenderingSettingsRoot + "/VP_SolarSystem.asset";
            VolumeProfile profile = CreateOrLoad<VolumeProfile>(path);
            RemoveUnexpectedOrDuplicateVolumeComponents(profile);

            Tonemapping tonemapping = GetOrAddVolumeComponent<Tonemapping>(profile);
            tonemapping.mode.Override(TonemappingMode.ACES);

            Bloom bloom = GetOrAddVolumeComponent<Bloom>(profile);
            bloom.threshold.Override(BloomThreshold);
            bloom.intensity.Override(BloomIntensity);
            bloom.scatter.Override(BloomScatter);
            bloom.highQualityFiltering.Override(false);

            ColorAdjustments color = GetOrAddVolumeComponent<ColorAdjustments>(profile);
            color.postExposure.Override(PostExposure);
            color.contrast.Override(ColorContrast);
            color.colorFilter.Override(ColorFilter);
            color.hueShift.Override(0f);
            color.saturation.Override(ColorSaturation);

            Vignette vignette = GetOrAddVolumeComponent<Vignette>(profile);
            vignette.color.Override(VignetteColor);
            vignette.center.Override(new Vector2(0.5f, 0.5f));
            vignette.intensity.Override(VignetteIntensity);
            vignette.smoothness.Override(VignetteSmoothness);
            vignette.rounded.Override(false);

            profile.components.Clear();
            profile.components.Add(tonemapping);
            profile.components.Add(bloom);
            profile.components.Add(color);
            profile.components.Add(vignette);
            EditorUtility.SetDirty(profile);
            return profile;
        }

        private static T GetOrAddVolumeComponent<T>(VolumeProfile profile)
            where T : VolumeComponent
        {
            return profile.TryGet(out T component)
                ? component
                : AddVolumeComponent<T>(profile);
        }

        private static T AddVolumeComponent<T>(VolumeProfile profile)
            where T : VolumeComponent
        {
            T component = profile.Add<T>(false);
            component.name = typeof(T).Name;
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(component, profile);
            return component;
        }

        private static void RemoveUnexpectedOrDuplicateVolumeComponents(
            VolumeProfile profile)
        {
            var retainedTypes = new HashSet<Type>();
            for (int index = profile.components.Count - 1; index >= 0; index--)
            {
                VolumeComponent component = profile.components[index];
                Type componentType = component != null ? component.GetType() : null;
                bool expectedType =
                    componentType == typeof(Tonemapping) ||
                    componentType == typeof(Bloom) ||
                    componentType == typeof(ColorAdjustments) ||
                    componentType == typeof(Vignette);
                if (expectedType && retainedTypes.Add(componentType))
                {
                    continue;
                }

                profile.components.RemoveAt(index);
                if (component != null)
                {
                    Object.DestroyImmediate(component, true);
                }
            }
        }

        private static CelestialBodyDefinition CreateBody(SolarSystemSlice2BodyData data)
        {
            CelestialBodyDefinition body = CreateOrLoad<CelestialBodyDefinition>(
                $"{DataRoot}/CelestialBodies/{data.AssetName}.asset");
            var serialized = new SerializedObject(body);
            serialized.FindProperty("stableId").stringValue = data.StableId;
            serialized.FindProperty("displayName").stringValue = data.DisplayName;
            serialized.FindProperty("category").enumValueIndex = (int)data.Category;
            serialized.FindProperty("parentId").stringValue = data.ParentId;
            serialized.FindProperty("scientificSourceId").stringValue = data.SourceId;
            serialized.FindProperty("educationalSummary").stringValue = data.EducationalSummary;
            serialized.FindProperty("meanRadiusKm").doubleValue = data.RadiusKm;
            serialized.FindProperty("hasMass").boolValue = true;
            serialized.FindProperty("massKg").doubleValue = data.MassKg;
            serialized.FindProperty("rotationPeriodSeconds").doubleValue = data.RotationPeriodSeconds;
            serialized.FindProperty("axialTiltDeg").doubleValue = data.AxialTiltDeg;
            serialized.FindProperty("hasOrbit").boolValue = data.Orbit.HasValue;
            if (data.Orbit.HasValue)
            {
                data.Orbit.Value.Apply(serialized.FindProperty("orbit"));
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(body);
            return body;
        }

        private static void ConfigureScale(PresentationScaleDefinition scale)
        {
            var serialized = new SerializedObject(scale);
            serialized.FindProperty("distanceReferenceKm").doubleValue =
                ReadableOverviewScaleContract.DistanceReferenceKm;
            serialized.FindProperty("unitsPerDistanceDecade").doubleValue =
                ReadableOverviewScaleContract.UnitsPerDistanceDecade;
            serialized.FindProperty("radiusReferenceKm").doubleValue =
                CelestialReferenceUnits.EarthMeanRadiusKm;
            serialized.FindProperty("referenceDisplayRadius").doubleValue =
                ReadableOverviewScaleContract.EarthDisplayRadius;
            serialized.FindProperty("minimumSurfaceClearance").doubleValue =
                ReadableOverviewScaleContract.MinimumSurfaceClearance;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(scale);
        }

        private static Material CreateMaterial(
            string path,
            string shaderName,
            string texturePath,
            Color color,
            float smoothness)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                throw new InvalidOperationException($"Required shader '{shaderName}' is unavailable.");
            }

            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.shader = shader;
            }

            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", smoothness);
            if (!string.IsNullOrEmpty(texturePath))
            {
                material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateOrUpdateMaterial(
            string path,
            string shaderName)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                throw new InvalidOperationException(
                    $"Required shader '{shaderName}' is unavailable.");
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.shader = shader;
            }

            return material;
        }

        private static T CreateOrLoad<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static T LoadRequiredAsset<T>(string path) where T : Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                throw new InvalidOperationException(
                    $"Required authored asset '{path}' is missing or failed to import.");
            }

            return asset;
        }

        private static void SetCatalogBodies(
            CelestialCatalogDefinition catalog,
            SolarSystemSlice2BodyContent[] bodies)
        {
            var serialized = new SerializedObject(catalog);
            SerializedProperty array = serialized.FindProperty("bodies");
            array.arraySize = bodies.Length;
            for (int index = 0; index < bodies.Length; index++)
            {
                array.GetArrayElementAtIndex(index).objectReferenceValue =
                    bodies[index].Definition;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];
            for (int index = 1; index < segments.Length; index++)
            {
                string next = $"{current}/{segments[index]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }

    }
}
