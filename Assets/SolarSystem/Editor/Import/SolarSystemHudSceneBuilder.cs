using System;
using Tanvir.SolarSystem.Presentation.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Builds the runtime UI Toolkit document and presenter scene objects.</summary>
    internal static class SolarSystemHudSceneBuilder
    {
        internal static SolarSystemHudPresenter Create(Transform parent)
        {
            PanelSettings panel = RequireAsset<PanelSettings>(
                SolarSystemUiAssetBuilder.PanelSettingsPath);
            VisualTreeAsset visualTree = RequireAsset<VisualTreeAsset>(
                SolarSystemUiAssetBuilder.VisualTreePath);
            StyleSheet styleSheet = RequireAsset<StyleSheet>(
                SolarSystemUiAssetBuilder.StyleSheetPath);

            GameObject hudObject = new GameObject("ExplorerHUD");
            hudObject.transform.SetParent(parent, false);
            UIDocument document = hudObject.AddComponent<UIDocument>();
            document.panelSettings = panel;
            document.visualTreeAsset = visualTree;
            document.sortingOrder = 100;

            SolarSystemHudPresenter presenter =
                hudObject.AddComponent<SolarSystemHudPresenter>();
            var serialized = new SerializedObject(presenter);
            serialized.FindProperty("document").objectReferenceValue = document;
            serialized.FindProperty("styleSheet").objectReferenceValue = styleSheet;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return presenter;
        }

        private static T RequireAsset<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset != null
                ? asset
                : throw new InvalidOperationException(
                    $"Required HUD asset is missing at '{path}'.");
        }
    }
}
