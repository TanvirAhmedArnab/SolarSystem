using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Authoring;
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
        private const string SaturnRingTexturePath =
            CelestialTextureRoot + "/Saturn/T_Saturn_RingsAlpha_2K.png";
        private const string SpaceTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/Environment/T_Space_MilkyWay_2K.jpg";
        private const float EarthNormalStrength = 0.28f;
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
        private const float SaturnRingSmoothness = 0.18f;
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
        private static readonly Color SunTint = new Color(2.6f, 1.55f, 0.5f, 1f);
        private static readonly Color MercuryTint = new Color(0.9f, 0.88f, 0.84f, 1f);
        private static readonly Color VenusTint = new Color(1f, 0.9f, 0.72f, 1f);
        private static readonly Color EarthTint = new Color(0.9f, 0.94f, 1f, 1f);
        private static readonly Color MoonTint = new Color(0.82f, 0.82f, 0.8f, 1f);
        private static readonly Color MarsTint = new Color(1f, 0.78f, 0.68f, 1f);
        private static readonly Color JupiterTint = new Color(0.96f, 0.9f, 0.84f, 1f);
        private static readonly Color SaturnTint = new Color(1f, 0.93f, 0.78f, 1f);
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
                    6371d,
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

            return new SolarSystemSlice2Content
            {
                Bodies = bodies,
                Catalog = catalog,
                Scale = scale,
                SaturnRingMesh = CreateSaturnRingMesh(),
                SaturnRingMaterial = CreateSaturnRingMaterial(),
                OrbitMaterial = orbitMaterial,
                SkyboxMaterial = skyboxMaterial,
                VisualProfile = visualProfile
            };
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
                ConfigureLitMaterial(
                    material,
                    AssetDatabase.LoadAssetAtPath<Texture2D>(EarthNormalPath),
                    EarthNormalStrength);
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
            Material material = CreateMaterial(
                $"{MaterialRoot}/CelestialBodies/M_{bodyName}.mat",
                unlit
                    ? "Universal Render Pipeline/Unlit"
                    : "Universal Render Pipeline/Lit",
                $"{CelestialTextureRoot}/{bodyName}/{textureName}",
                tint,
                smoothness);
            if (unlit)
            {
                ConfigureSunMaterial(material);
            }
            else if (bodyName != "Earth")
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
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
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
            Material material = CreateMaterial(
                path,
                "Universal Render Pipeline/Lit",
                SaturnRingTexturePath,
                Color.white,
                SaturnRingSmoothness);
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_AlphaClip", 0f);
            material.SetFloat("_Cull", (float)CullMode.Off);
            material.SetFloat("_ZWrite", 0f);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetOverrideTag("RenderType", "Transparent");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHATEST_ON");
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
            serialized.FindProperty("distanceReferenceKm").doubleValue = 1000000d;
            serialized.FindProperty("unitsPerDistanceDecade").doubleValue = 15d;
            serialized.FindProperty("radiusReferenceKm").doubleValue = 6371d;
            serialized.FindProperty("referenceDisplayRadius").doubleValue = 0.8d;
            serialized.FindProperty("radiusExponent").doubleValue = 0.4d;
            serialized.FindProperty("minimumDisplayRadius").doubleValue = 0.18d;
            serialized.FindProperty("maximumDisplayRadius").doubleValue = 4.8d;
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
