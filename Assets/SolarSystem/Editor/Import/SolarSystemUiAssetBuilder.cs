using System;
using UnityEditor;
using UnityEngine;
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

        internal static void Build()
        {
            EnsureFolder("Assets/SolarSystem/Settings/UI");
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
