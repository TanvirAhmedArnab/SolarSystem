using System;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.UI;
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
        private const string InputAssetPath =
            "Assets/SolarSystem/Settings/Input/IA_SolarSystem.asset";
        private const int EarthOrbitSampleCount = 192;
        private const int MoonOrbitSampleCount = 128;
        private const int JupiterOrbitSampleCount = 256;
        private const float EarthOrbitWidth = 0.055f;
        private const float MoonOrbitWidth = 0.025f;
        private const float JupiterOrbitWidth = 0.065f;

        internal static GameObject Build(SolarSystemSlice2Content content)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject sceneRoot = new GameObject("SolarSystem");
            Transform applicationRoot = CreateGroup("_Application", sceneRoot.transform);
            Transform simulationRoot = CreateGroup("_Simulation", sceneRoot.transform);
            Transform bodyRoot = CreateGroup("CelestialBodies", simulationRoot);
            Transform orbitRoot = CreateGroup("OrbitPaths", simulationRoot);
            Transform environmentRoot = CreateGroup("_Environment", sceneRoot.transform);
            Transform interfaceRoot = CreateGroup("_Interface", sceneRoot.transform);
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
            CelestialBodyView jupiterView =
                CreateBodyView(content.Jupiter, content.JupiterMaterial, bodyRoot);
            CelestialOrbitPathView earthOrbit =
                CreateOrbitPath(
                    content.Earth,
                    content.OrbitMaterial,
                    orbitRoot,
                    EarthOrbitSampleCount,
                    EarthOrbitWidth);
            CelestialOrbitPathView moonOrbit =
                CreateOrbitPath(
                    content.Moon,
                    content.OrbitMaterial,
                    orbitRoot,
                    MoonOrbitSampleCount,
                    MoonOrbitWidth);
            CelestialOrbitPathView jupiterOrbit =
                CreateOrbitPath(
                    content.Jupiter,
                    content.OrbitMaterial,
                    orbitRoot,
                    JupiterOrbitSampleCount,
                    JupiterOrbitWidth);

            ConfigureSimulationComposition(
                composition,
                controller,
                content.Catalog,
                content.Scale,
                new[] { sunView, earthView, moonView, jupiterView },
                new[] { earthOrbit, moonOrbit, jupiterOrbit });

            Camera camera = CreateCamera(environmentRoot);
            SolarSystemHudPresenter hudPresenter =
                SolarSystemHudSceneBuilder.Create(interfaceRoot);
            CreateInteractionComposition(
                applicationRoot,
                camera,
                controller,
                hudPresenter);
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
            SphereCollider selectionCollider = bodyObject.AddComponent<SphereCollider>();

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Visual";
            visual.transform.SetParent(bodyObject.transform, false);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            visual.GetComponent<MeshRenderer>().sharedMaterial = material;

            var serializedView = new SerializedObject(view);
            serializedView.FindProperty("definition").objectReferenceValue = definition;
            serializedView.FindProperty("visualRoot").objectReferenceValue = visual.transform;
            serializedView.FindProperty("selectionCollider").objectReferenceValue =
                selectionCollider;
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

        private static Camera CreateCamera(Transform parent)
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
            return camera;
        }

        private static void CreateInteractionComposition(
            Transform parent,
            Camera camera,
            SolarSystemSimulationController simulationController,
            SolarSystemHudPresenter hudPresenter)
        {
            UnityEngine.InputSystem.InputActionAsset inputActions =
                AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(
                    InputAssetPath);
            if (inputActions == null)
            {
                throw new ArgumentNullException(nameof(inputActions));
            }

            GameObject interactionObject = new GameObject("SolarSystemInteractionRoot");
            interactionObject.transform.SetParent(parent, false);
            SolarSystemInputAdapter input =
                interactionObject.AddComponent<SolarSystemInputAdapter>();
            CelestialSelectionController selection =
                interactionObject.AddComponent<CelestialSelectionController>();
            SimulationTimeInputController timeInput =
                interactionObject.AddComponent<SimulationTimeInputController>();
            SolarSystemCameraController cameraController =
                camera.gameObject.AddComponent<SolarSystemCameraController>();
            SolarSystemInteractionCompositionRoot composition =
                interactionObject.AddComponent<SolarSystemInteractionCompositionRoot>();

            var serialized = new SerializedObject(composition);
            serialized.FindProperty("inputActions").objectReferenceValue = inputActions;
            serialized.FindProperty("explorerCamera").objectReferenceValue = camera;
            serialized.FindProperty("inputAdapter").objectReferenceValue = input;
            serialized.FindProperty("selectionController").objectReferenceValue = selection;
            serialized.FindProperty("cameraController").objectReferenceValue = cameraController;
            serialized.FindProperty("simulationController").objectReferenceValue =
                simulationController;
            serialized.FindProperty("timeInputController").objectReferenceValue = timeInput;
            serialized.FindProperty("hudPresenter").objectReferenceValue = hudPresenter;
            serialized.ApplyModifiedPropertiesWithoutUndo();
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

        private static void ConfigureSimulationComposition(
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
            serialized.FindProperty("simulationSecondsPerRealSecond").doubleValue =
                SimulationTimeControlService.BaselineSecondsPerRealSecond * 10d;
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
