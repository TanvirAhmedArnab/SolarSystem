using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Orchestrates the reproducible Sun-Earth-Moon Slice 2 build.</summary>
    public static class SolarSystemSlice2Builder
    {
        /// <summary>Rebuilds all Slice 2 authored assets and the visible scene.</summary>
        [MenuItem("Tools/Solar System/Rebuild Slice 2 Graybox")]
        public static void Build()
        {
            SolarSystemSlice2Content content = SolarSystemSlice2AssetBuilder.Build();
            GameObject sceneRoot = SolarSystemSlice2SceneBuilder.Build(content);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeGameObject = sceneRoot;
            SceneView.lastActiveSceneView?.FrameSelected();
            Debug.Log("SLICE2_BUILD_COMPLETE|scene=SolarSystem|bodies=3|orbits=2");
        }
    }
}
