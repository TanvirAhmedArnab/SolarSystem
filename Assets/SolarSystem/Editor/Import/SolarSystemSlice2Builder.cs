using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Orchestrates the reproducible Solar System project graybox build.</summary>
    public static class SolarSystemSlice2Builder
    {
        /// <summary>Rebuilds project-authored assets and the visible scene.</summary>
        [MenuItem("Tools/Solar System/Rebuild Project Content")]
        public static void Build()
        {
            SolarSystemSlice2Content content = SolarSystemSlice2AssetBuilder.Build();
            InputActionAsset inputActions = SolarSystemInputAssetBuilder.Build();
            if (inputActions == null)
            {
                throw new InvalidOperationException(
                    "Project input asset did not load after deterministic authoring.");
            }

            SolarSystemUiAssetBuilder.Build();
            SolarSystemSlice2AssetBuilder.ReloadCometContent(content);
            GameObject sceneRoot = SolarSystemSlice2SceneBuilder.Build(content);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeGameObject = sceneRoot;
            SceneView.lastActiveSceneView?.FrameSelected();
            Debug.Log(
                "PROJECT_BUILD_COMPLETE|scene=SolarSystem|bodies=16|planets=8|moons=7|orbits=15|interaction=ready");
        }

        /// <summary>
        /// Rebuilds only the project-owned input and UI contracts without replacing the scene.
        /// </summary>
        [MenuItem("Tools/Solar System/Rebuild Interface Contracts")]
        public static void BuildInterfaceContracts()
        {
            InputActionAsset inputActions = SolarSystemInputAssetBuilder.Build();
            if (inputActions == null)
            {
                throw new InvalidOperationException(
                    "Project input asset did not load after deterministic authoring.");
            }

            SolarSystemUiAssetBuilder.Build();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("INTERFACE_CONTRACT_BUILD_COMPLETE|input=v7|ui=validated");
        }
    }
}
