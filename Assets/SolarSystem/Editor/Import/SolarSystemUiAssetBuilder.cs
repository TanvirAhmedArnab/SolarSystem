using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Creates and validates project-owned runtime UI Toolkit assets.</summary>
    internal static class SolarSystemUiAssetBuilder
    {
        internal const string PanelSettingsPath =
            "Assets/SolarSystem/Settings/UI/PanelSettings_SolarSystem.asset";
        internal const string VisualTreePath =
            "Assets/SolarSystem/Content/UI/SolarSystemHud.uxml";
        internal const string StyleSheetPath =
            "Assets/SolarSystem/Content/UI/SolarSystemHud.uss";
        internal const string ThemePath =
            "Assets/SolarSystem/Settings/UI/ToolkitTheme/UnityThemes/" +
            "UnityDefaultRuntimeTheme.tss";
        internal const string RegularFontPath =
            "Assets/SolarSystem/Content/UI/Typography/Inter-Regular.ttf";
        internal const string SemiBoldFontPath =
            "Assets/SolarSystem/Content/UI/Typography/Inter-SemiBold.ttf";
        internal const string RegularFontAssetPath =
            "Assets/SolarSystem/Content/UI/Typography/FA_Inter_Regular.asset";
        internal const string SemiBoldFontAssetPath =
            "Assets/SolarSystem/Content/UI/Typography/FA_Inter_SemiBold.asset";

        private const int FontSamplingPointSize = 90;
        private const int FontAtlasPadding = 9;
        private const int FontAtlasSize = 1024;

        internal static void Build()
        {
            EnsureFolder("Assets/SolarSystem/Settings/UI");
            EnsureFolder("Assets/SolarSystem/Content/UI/Typography");
            EnsureFontAsset(RegularFontPath, RegularFontAssetPath, "FA_Inter_Regular");
            EnsureFontAsset(SemiBoldFontPath, SemiBoldFontAssetPath, "FA_Inter_SemiBold");

            PanelSettings panel =
                AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panel == null)
            {
                panel = ScriptableObject.CreateInstance<PanelSettings>();
                panel.name = "PanelSettings_SolarSystem";
                AssetDatabase.CreateAsset(panel, PanelSettingsPath);
            }

            panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panel.referenceResolution = new Vector2Int(1920, 1080);
            panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panel.match = 0.5f;
            panel.themeStyleSheet = RequireAsset<ThemeStyleSheet>(ThemePath);
            EditorUtility.SetDirty(panel);

            RequireAsset<VisualTreeAsset>(VisualTreePath);
            RequireAsset<StyleSheet>(StyleSheetPath);
        }

        private static FontAsset EnsureFontAsset(
            string sourcePath,
            string assetPath,
            string assetName)
        {
            Font source = RequireAsset<Font>(sourcePath);
            FontAsset asset = AssetDatabase.LoadAssetAtPath<FontAsset>(assetPath);
            if (asset != null)
            {
                if (asset.sourceFontFile != source)
                {
                    throw new InvalidOperationException(
                        $"Font asset '{assetPath}' does not use its approved source font.");
                }

                ConfigureFontAsset(asset);
                return asset;
            }

            asset = FontAsset.CreateFontAsset(
                source,
                FontSamplingPointSize,
                FontAtlasPadding,
                GlyphRenderMode.SDFAA,
                FontAtlasSize,
                FontAtlasSize,
                AtlasPopulationMode.Dynamic,
                true);
            if (asset == null)
            {
                throw new InvalidOperationException(
                    $"Could not create the runtime font asset '{assetPath}'.");
            }

            asset.name = assetName;
            ConfigureFontAsset(asset);
            AssetDatabase.CreateAsset(asset, assetPath);
            AddSubAssetIfNeeded(asset.material, asset, $"{assetName} Material");
            foreach (Texture2D atlasTexture in asset.atlasTextures)
            {
                AddSubAssetIfNeeded(atlasTexture, asset, $"{assetName} Atlas");
            }

            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void ConfigureFontAsset(FontAsset asset)
        {
            asset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            asset.isMultiAtlasTexturesEnabled = true;
            asset.getFontFeatures = true;
            EditorUtility.SetDirty(asset);
        }

        private static void AddSubAssetIfNeeded(
            UnityEngine.Object child,
            UnityEngine.Object parent,
            string name)
        {
            if (child == null || AssetDatabase.Contains(child))
            {
                return;
            }

            child.name = name;
            AssetDatabase.AddObjectToAsset(child, parent);
        }

        private static T RequireAsset<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset != null
                ? asset
                : throw new InvalidOperationException(
                    $"Required runtime UI asset is missing at '{path}'.");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            int separator = path.LastIndexOf('/');
            string parent = path.Substring(0, separator);
            string name = path.Substring(separator + 1);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
