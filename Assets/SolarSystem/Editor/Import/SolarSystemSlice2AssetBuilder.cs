using System;
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
        private const string EarthNormalPath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_Normal_2K.tif";
        private const string EarthSpecularPath =
            "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_Specular_2K.tif";
        private const string SpaceTexturePath =
            "Assets/SolarSystem/Content/Art/Textures/Environment/T_Space_MilkyWay_2K.jpg";
        private const float EarthNormalStrength = 0.28f;
        private const float SunSmoothness = 0f;
        private const float EarthSmoothness = 0.22f;
        private const float MoonSmoothness = 0.08f;
        private const float JupiterSmoothness = 0.18f;
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
        private static readonly Color EarthTint = new Color(0.9f, 0.94f, 1f, 1f);
        private static readonly Color MoonTint = new Color(0.82f, 0.82f, 0.8f, 1f);
        private static readonly Color JupiterTint = new Color(0.96f, 0.9f, 0.84f, 1f);
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

            CelestialBodyDefinition sun = CreateBody(new SolarSystemSlice2BodyData(
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
                null));
            CelestialBodyDefinition earth = CreateBody(new SolarSystemSlice2BodyData(
                "Body_Earth",
                "earth",
                "Earth",
                CelestialBodyCategory.Planet,
                "sun",
                "NASA_NSSDC_EARTH_AND_JPL_APPROX_POS_J2000",
                "A rocky world with global oceans and the only known environment that supports life.",
                6371d,
                5.9722e24d,
                23.9345d * 3600d,
                23.44d,
                new SolarSystemSlice2OrbitData(
                    AstronomicalUnitKm * 1.00000261d,
                    0.01671123d,
                    -0.00001531d,
                    0d,
                    102.93768193d,
                    100.46457166d - 102.93768193d,
                    365.256363004d * SecondsPerDay)));
            CelestialBodyDefinition moon = CreateBody(new SolarSystemSlice2BodyData(
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
                    27.322d * SecondsPerDay)));
            CelestialBodyDefinition jupiter = CreateBody(new SolarSystemSlice2BodyData(
                "Body_Jupiter",
                "jupiter",
                "Jupiter",
                CelestialBodyCategory.Planet,
                "sun",
                "JPL_PLANETARY_PHYSICAL_AND_APPROX_POS_J2000_NASA_JUPITER_FACTS",
                "The Solar System's largest planet, a hydrogen-helium giant with rapid rotation and powerful storms.",
                69911d,
                1.898125e27d,
                0.41354d * SecondsPerDay,
                3d,
                new SolarSystemSlice2OrbitData(
                    AstronomicalUnitKm * 5.202887d,
                    0.04838624d,
                    1.30439695d,
                    100.47390909d,
                    14.72847983d - 100.47390909d,
                    34.39644051d - 14.72847983d,
                    11.862615d * 365.25d * SecondsPerDay)));

            CelestialCatalogDefinition catalog = CreateOrLoad<CelestialCatalogDefinition>(
                $"{DataRoot}/Catalog_SolarSystem.asset");
            SetObjectArray(catalog, "bodies", sun, earth, moon, jupiter);
            PresentationScaleDefinition scale = CreateOrLoad<PresentationScaleDefinition>(
                $"{DataRoot}/Scale/Scale_PresentationGraybox.asset");
            ConfigureScale(scale);
            Material sunMaterial = CreateMaterial(
                $"{MaterialRoot}/CelestialBodies/M_Sun.mat",
                "Universal Render Pipeline/Unlit",
                "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Sun/T_Sun_Surface_2K.jpg",
                SunTint,
                SunSmoothness);
            ConfigureSunMaterial(sunMaterial);
            Material earthMaterial = CreateMaterial(
                $"{MaterialRoot}/CelestialBodies/M_Earth.mat",
                "Universal Render Pipeline/Lit",
                "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_DayAlbedo_2K.jpg",
                EarthTint,
                EarthSmoothness);
            ConfigureLitMaterial(
                earthMaterial,
                AssetDatabase.LoadAssetAtPath<Texture2D>(EarthNormalPath),
                EarthNormalStrength);
            Material moonMaterial = CreateMaterial(
                $"{MaterialRoot}/CelestialBodies/M_Moon.mat",
                "Universal Render Pipeline/Lit",
                "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Moon/T_Moon_Surface_2K.jpg",
                MoonTint,
                MoonSmoothness);
            ConfigureLitMaterial(moonMaterial, null, 0f);
            Material jupiterMaterial = CreateMaterial(
                $"{MaterialRoot}/CelestialBodies/M_Jupiter.mat",
                "Universal Render Pipeline/Lit",
                "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Jupiter/T_Jupiter_Surface_2K.jpg",
                JupiterTint,
                JupiterSmoothness);
            ConfigureLitMaterial(jupiterMaterial, null, 0f);
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
                Sun = sun,
                Earth = earth,
                Moon = moon,
                Jupiter = jupiter,
                Catalog = catalog,
                Scale = scale,
                SunMaterial = sunMaterial,
                EarthMaterial = earthMaterial,
                MoonMaterial = moonMaterial,
                JupiterMaterial = jupiterMaterial,
                OrbitMaterial = orbitMaterial,
                SkyboxMaterial = skyboxMaterial,
                VisualProfile = visualProfile
            };
        }

        private static void ConfigureTextureImports()
        {
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
            for (int index = profile.components.Count - 1; index >= 0; index--)
            {
                Object.DestroyImmediate(profile.components[index], true);
            }

            profile.components.Clear();

            Tonemapping tonemapping = AddVolumeComponent<Tonemapping>(profile);
            tonemapping.mode.Override(TonemappingMode.ACES);

            Bloom bloom = AddVolumeComponent<Bloom>(profile);
            bloom.threshold.Override(BloomThreshold);
            bloom.intensity.Override(BloomIntensity);
            bloom.scatter.Override(BloomScatter);
            bloom.highQualityFiltering.Override(false);

            ColorAdjustments color = AddVolumeComponent<ColorAdjustments>(profile);
            color.postExposure.Override(PostExposure);
            color.contrast.Override(ColorContrast);
            color.colorFilter.Override(ColorFilter);
            color.hueShift.Override(0f);
            color.saturation.Override(ColorSaturation);

            Vignette vignette = AddVolumeComponent<Vignette>(profile);
            vignette.color.Override(VignetteColor);
            vignette.center.Override(new Vector2(0.5f, 0.5f));
            vignette.intensity.Override(VignetteIntensity);
            vignette.smoothness.Override(VignetteSmoothness);
            vignette.rounded.Override(false);

            EditorUtility.SetDirty(profile);
            return profile;
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

        private static void SetObjectArray(
            CelestialCatalogDefinition catalog,
            string propertyName,
            params Object[] values)
        {
            var serialized = new SerializedObject(catalog);
            SerializedProperty array = serialized.FindProperty(propertyName);
            array.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++)
            {
                array.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
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
