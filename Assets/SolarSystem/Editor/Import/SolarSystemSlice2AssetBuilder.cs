using System;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Creates or updates Slice 2 scientific definitions, scale data, and materials.</summary>
    internal static class SolarSystemSlice2AssetBuilder
    {
        private const string DataRoot = "Assets/SolarSystem/Content/Data";
        private const string MaterialRoot = "Assets/SolarSystem/Content/Materials";
        private const double SecondsPerDay = 86400d;
        private const double AstronomicalUnitKm = 149597870.7d;

        internal static SolarSystemSlice2Content Build()
        {
            EnsureFolder($"{DataRoot}/CelestialBodies");
            EnsureFolder($"{DataRoot}/Scale");
            EnsureFolder($"{MaterialRoot}/CelestialBodies");
            EnsureFolder($"{MaterialRoot}/Environment");
            EnsureFolder("Assets/SolarSystem/Scenes");

            CelestialBodyDefinition sun = CreateBody(new SolarSystemSlice2BodyData(
                "Body_Sun",
                "sun",
                "Sun",
                CelestialBodyCategory.Star,
                string.Empty,
                "NASA_NSSDC_SUN_EARTH_FACT_SHEET",
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

            CelestialCatalogDefinition catalog = CreateOrLoad<CelestialCatalogDefinition>(
                $"{DataRoot}/Catalog_SunEarthMoon.asset");
            SetObjectArray(catalog, "bodies", sun, earth, moon);
            PresentationScaleDefinition scale = CreateOrLoad<PresentationScaleDefinition>(
                $"{DataRoot}/Scale/Scale_PresentationGraybox.asset");
            ConfigureScale(scale);

            return new SolarSystemSlice2Content
            {
                Sun = sun,
                Earth = earth,
                Moon = moon,
                Catalog = catalog,
                Scale = scale,
                SunMaterial = CreateMaterial(
                    $"{MaterialRoot}/CelestialBodies/M_Sun.mat",
                    "Universal Render Pipeline/Unlit",
                    "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Sun/T_Sun_Surface_2K.jpg",
                    new Color(2.2f, 1.15f, 0.3f, 1f),
                    0f),
                EarthMaterial = CreateMaterial(
                    $"{MaterialRoot}/CelestialBodies/M_Earth.mat",
                    "Universal Render Pipeline/Lit",
                    "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Earth/T_Earth_DayAlbedo_2K.jpg",
                    Color.white,
                    0.25f),
                MoonMaterial = CreateMaterial(
                    $"{MaterialRoot}/CelestialBodies/M_Moon.mat",
                    "Universal Render Pipeline/Lit",
                    "Assets/SolarSystem/Content/Art/Textures/CelestialBodies/Moon/T_Moon_Surface_2K.jpg",
                    Color.white,
                    0.1f),
                OrbitMaterial = CreateMaterial(
                    $"{MaterialRoot}/Environment/M_OrbitPath.mat",
                    "Universal Render Pipeline/Unlit",
                    null,
                    new Color(0.2f, 0.52f, 0.9f, 1f),
                    0f)
            };
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
