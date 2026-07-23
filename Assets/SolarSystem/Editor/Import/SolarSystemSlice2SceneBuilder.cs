using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Constructs the reproducible Slice 2 scene from authored content.</summary>
    internal static class SolarSystemSlice2SceneBuilder
    {
        private const string ScenePath = "Assets/SolarSystem/Scenes/SolarSystem.unity";

        internal static GameObject Build(SolarSystemSlice2Content content)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject sceneRoot = new GameObject("SolarSystem");
            Transform applicationRoot = CreateGroup("_Application", sceneRoot.transform);
            Transform simulationRoot = CreateGroup("_Simulation", sceneRoot.transform);
            Transform bodyRoot = CreateGroup("CelestialBodies", simulationRoot);
            Transform orbitRoot = CreateGroup("OrbitPaths", simulationRoot);
            Transform environmentRoot = CreateGroup("_Environment", sceneRoot.transform);
            CreateGroup("_Diagnostics", sceneRoot.transform);

            GameObject compositionObject = new GameObject("SolarSystemCompositionRoot");
            compositionObject.transform.SetParent(applicationRoot, false);
            SolarSystemSimulationController controller =
                compositionObject.AddComponent<SolarSystemSimulationController>();
            SolarSystemCompositionRoot composition =
                compositionObject.AddComponent<SolarSystemCompositionRoot>();

            CelestialBodyView sunView =
                CreateBodyView(content.Sun, content.SunMaterial, bodyRoot);
            CelestialBodyView earthView =
                CreateBodyView(content.Earth, content.EarthMaterial, bodyRoot);
            CelestialBodyView moonView =
                CreateBodyView(content.Moon, content.MoonMaterial, bodyRoot);
            CelestialOrbitPathView earthOrbit =
                CreateOrbitPath(content.Earth, content.OrbitMaterial, orbitRoot, 192, 0.055f);
            CelestialOrbitPathView moonOrbit =
                CreateOrbitPath(content.Moon, content.OrbitMaterial, orbitRoot, 128, 0.025f);

            ConfigureComposition(
                composition,
                controller,
                content.Catalog,
                content.Scale,
                new[] { sunView, earthView, moonView },
                new[] { earthOrbit, moonOrbit });

            CreateCamera(environmentRoot);
            CreateLighting(environmentRoot);
            CreateGlobalVolume(environmentRoot);

            composition.RebuildRuntimeGraph();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            return sceneRoot;
        }

        private static CelestialBodyView CreateBodyView(
            CelestialBodyDefinition definition,
            Material material,
            Transform parent)
        {
            GameObject bodyObject = new GameObject(definition.DisplayName);
            bodyObject.transform.SetParent(parent, false);
            CelestialBodyView view = bodyObject.AddComponent<CelestialBodyView>();

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Visual";
            visual.transform.SetParent(bodyObject.transform, false);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            visual.GetComponent<MeshRenderer>().sharedMaterial = material;

            var serializedView = new SerializedObject(view);
            serializedView.FindProperty("definition").objectReferenceValue = definition;
            serializedView.FindProperty("visualRoot").objectReferenceValue = visual.transform;
            serializedView.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        private static CelestialOrbitPathView CreateOrbitPath(
            CelestialBodyDefinition definition,
            Material material,
            Transform parent,
            int samples,
            float width)
        {
            GameObject orbitObject = new GameObject($"{definition.DisplayName} Orbit");
            orbitObject.transform.SetParent(parent, false);
            LineRenderer line = orbitObject.AddComponent<LineRenderer>();
            line.sharedMaterial = material;
            line.widthMultiplier = width;
            line.numCornerVertices = 2;
            line.numCapVertices = 0;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;

            CelestialOrbitPathView path = orbitObject.AddComponent<CelestialOrbitPathView>();
            var serializedPath = new SerializedObject(path);
            serializedPath.FindProperty("definition").objectReferenceValue = definition;
            serializedPath.FindProperty("lineRenderer").objectReferenceValue = line;
            serializedPath.FindProperty("sampleCount").intValue = samples;
            serializedPath.ApplyModifiedPropertiesWithoutUndo();
            return path;
        }

        private static void CreateCamera(Transform parent)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(parent, false);
            cameraObject.transform.position = new Vector3(0f, 38f, -55f);
            cameraObject.transform.LookAt(new Vector3(0f, 0f, 15f));
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.001f, 0.003f, 0.01f, 1f);
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 250f;
            camera.fieldOfView = 50f;
            cameraObject.AddComponent<AudioListener>();
        }

        private static void CreateLighting(Transform parent)
        {
            GameObject lightObject = new GameObject("Sun Key Light");
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.6f;
            light.color = new Color(1f, 0.94f, 0.84f);
            light.shadows = LightShadows.Soft;
        }

        private static void CreateGlobalVolume(Transform parent)
        {
            GameObject volumeObject = new GameObject("Global Volume");
            volumeObject.transform.SetParent(parent, false);
            Volume volume = volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 0f;
            volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(
                "Assets/Settings/SampleSceneProfile.asset");
        }

        private static void ConfigureComposition(
            SolarSystemCompositionRoot composition,
            SolarSystemSimulationController controller,
            CelestialCatalogDefinition catalog,
            PresentationScaleDefinition scale,
            CelestialBodyView[] views,
            CelestialOrbitPathView[] paths)
        {
            var serialized = new SerializedObject(composition);
            serialized.FindProperty("catalogDefinition").objectReferenceValue = catalog;
            serialized.FindProperty("scaleDefinition").objectReferenceValue = scale;
            serialized.FindProperty("simulationController").objectReferenceValue = controller;
            SetArray(serialized.FindProperty("bodyViews"), views);
            SetArray(serialized.FindProperty("orbitPaths"), paths);
            serialized.FindProperty("simulationSecondsPerRealSecond").doubleValue = 604800d;
            serialized.FindProperty("beginPaused").boolValue = false;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetArray<T>(SerializedProperty array, T[] values) where T : Object
        {
            array.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++)
            {
                array.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }

        private static Transform CreateGroup(string name, Transform parent)
        {
            GameObject group = new GameObject(name);
            group.transform.SetParent(parent, false);
            return group.transform;
        }
    }
}
