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
            GameObject sceneRoot = SolarSystemSlice2SceneBuilder.Build(content);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeGameObject = sceneRoot;
            SceneView.lastActiveSceneView?.FrameSelected();
            Debug.Log(
                "PROJECT_BUILD_COMPLETE|scene=SolarSystem|bodies=10|planets=8|orbits=9|interaction=ready");
        }
    }
}
