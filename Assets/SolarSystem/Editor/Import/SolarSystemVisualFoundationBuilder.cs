using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Applies the project-owned visual foundation without recreating scene identities.</summary>
    public static class SolarSystemVisualFoundationBuilder
    {
        private const string ScenePath = "Assets/SolarSystem/Scenes/SolarSystem.unity";

        /// <summary>Builds visual assets and updates the authored scene in place.</summary>
        [MenuItem("Tools/Solar System/Apply Visual Foundation")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                throw new InvalidOperationException(
                    "Visual foundation authoring is unavailable during Play Mode.");
            }

            SolarSystemSlice2Content content = SolarSystemSlice2AssetBuilder.Build();
            Scene scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.path != ScenePath)
            {
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }

            SolarSystemSlice2SceneBuilder.ApplyVisualFoundation(content);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.Refresh();
            Debug.Log(
                "VISUAL_FOUNDATION_COMPLETE|skybox=M_SpaceSkybox|profile=VP_SolarSystem|materials=4");
        }
    }
}
