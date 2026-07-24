using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Audio;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Presentation.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
        private const string SolarRadialLightName = "Solar Radial Light";
        private const string LegacySolarKeyLightName = "Sun Key Light";
        private const string SunStableId = "sun";
        private const string EarthStableId = "earth";
        private const float EarthAmbienceMinimumDistance = 1.5f;
        private const float EarthAmbienceMaximumDistance = 12f;
        private const float SolarRadialIntensityCandela = 165000f;
        private const float SolarRadialRange = 620f;
        private const float SolarRadialTemperature = 5600f;
        private const float ReflectionIntensity = 0.18f;
        private static readonly Color AmbientFill =
            new Color(0.012f, 0.017f, 0.03f, 1f);

        internal static GameObject Build(SolarSystemSlice2Content content)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject sceneRoot = new GameObject("SolarSystem");
            Transform applicationRoot = CreateGroup("_Application", sceneRoot.transform);
            Transform simulationRoot = CreateGroup("_Simulation", sceneRoot.transform);
            Transform bodyRoot = CreateGroup("CelestialBodies", simulationRoot);
            Transform orbitRoot = CreateGroup("OrbitPaths", simulationRoot);
            Transform environmentRoot = CreateGroup("_Environment", sceneRoot.transform);
            Transform audioRoot = CreateGroup("_Audio", sceneRoot.transform);
            Transform interfaceRoot = CreateGroup("_Interface", sceneRoot.transform);
            CreateGroup("_Diagnostics", sceneRoot.transform);

            GameObject compositionObject = new GameObject("SolarSystemCompositionRoot");
            compositionObject.transform.SetParent(applicationRoot, false);
            SolarSystemSimulationController controller =
                compositionObject.AddComponent<SolarSystemSimulationController>();
            SolarSystemCompositionRoot composition =
                compositionObject.AddComponent<SolarSystemCompositionRoot>();

            var bodyViews = new List<CelestialBodyView>(content.Bodies.Length);
            var orbitPaths = new List<CelestialOrbitPathView>(content.Bodies.Length - 1);
            CelestialBodyView sunView = null;
            CelestialBodyView earthView = null;
            foreach (SolarSystemSlice2BodyContent body in content.Bodies)
            {
                bool isSaturn = body.Definition.StableId == "saturn";
                CelestialBodyView view = CreateBodyView(
                    body.Definition,
                    body.Material,
                    bodyRoot,
                    isSaturn ? content.SaturnRingMesh : null,
                    isSaturn ? content.SaturnRingMaterial : null);
                bodyViews.Add(view);
                if (body.Definition.StableId == SunStableId)
                {
                    sunView = view;
                }
                else if (body.Definition.StableId == EarthStableId)
                {
                    earthView = view;
                }

                if (body.Definition.HasOrbit)
                {
                    orbitPaths.Add(CreateOrbitPath(
                        body.Definition,
                        content.OrbitMaterial,
                        orbitRoot,
                        body.OrbitSampleCount,
                        body.OrbitWidth));
                }
            }

            if (sunView == null)
            {
                throw new InvalidOperationException("The authored content requires the Sun root.");
            }

            if (earthView == null)
            {
                throw new InvalidOperationException("The authored content requires Earth.");
            }

            ConfigureSimulationComposition(
                composition,
                controller,
                content.Catalog,
                content.Scale,
                bodyViews.ToArray(),
                orbitPaths.ToArray());

            Camera camera = CreateCamera(environmentRoot);
            SolarSystemHudPresenter hudPresenter =
                SolarSystemHudSceneBuilder.Create(interfaceRoot);
            AudioDirector audioDirector =
                CreateAudioSystem(audioRoot, sunView, earthView, content);
            CreateInteractionComposition(
                applicationRoot,
                camera,
                controller,
                hudPresenter,
                audioDirector);
            CreateLighting(sunView.transform);
            CreateGlobalVolume(environmentRoot, content.VisualProfile);
            ConfigureRenderSettings(content.SkyboxMaterial);

            composition.RebuildRuntimeGraph();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            return sceneRoot;
        }

        internal static void ApplyVisualFoundation(SolarSystemSlice2Content content)
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                throw new InvalidOperationException(
                    "The SolarSystem scene requires a Main Camera.");
            }

            ConfigureCamera(camera);

            GameObject lightObject =
                GameObject.Find(SolarRadialLightName) ??
                GameObject.Find(LegacySolarKeyLightName);
            Light radialLight = lightObject != null ? lightObject.GetComponent<Light>() : null;
            if (radialLight == null)
            {
                throw new InvalidOperationException(
                    "The SolarSystem scene requires its project-owned solar light.");
            }

            CelestialBodyView sunView = FindBodyView(SunStableId);
            ConfigureLighting(radialLight, sunView.transform);

            GameObject volumeObject = GameObject.Find("Global Volume");
            Volume volume = volumeObject != null ? volumeObject.GetComponent<Volume>() : null;
            if (volume == null)
            {
                throw new InvalidOperationException(
                    "The SolarSystem scene requires the Global Volume.");
            }

            ConfigureGlobalVolume(volume, content.VisualProfile);
            ConfigureRenderSettings(content.SkyboxMaterial);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private static CelestialBodyView CreateBodyView(
            CelestialBodyDefinition definition,
            Material material,
            Transform parent,
            Mesh ringMesh,
            Material ringMaterial)
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
            if (ringMesh != null && ringMaterial != null)
            {
                CreateRingView(visual.transform, ringMesh, ringMaterial);
            }

            var serializedView = new SerializedObject(view);
            serializedView.FindProperty("definition").objectReferenceValue = definition;
            serializedView.FindProperty("visualRoot").objectReferenceValue = visual.transform;
            serializedView.FindProperty("selectionCollider").objectReferenceValue =
                selectionCollider;
            serializedView.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        private static void CreateRingView(
            Transform visualRoot,
            Mesh mesh,
            Material material)
        {
            GameObject ringObject = new GameObject("Rings");
            ringObject.transform.SetParent(visualRoot, false);
            MeshFilter filter = ringObject.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            MeshRenderer renderer = ringObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
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
            line.widthMultiplier =
                width * ReadableOverviewScaleContract.OrbitPathWidthMultiplier;
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
            cameraObject.transform.position = new Vector3(0f, 600f, -900f);
            cameraObject.transform.LookAt(Vector3.zero);
            Camera camera = cameraObject.AddComponent<Camera>();
            ConfigureCamera(camera);
            cameraObject.AddComponent<AudioListener>();
            return camera;
        }

        private static void ConfigureCamera(Camera camera)
        {
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.backgroundColor = new Color(0.001f, 0.003f, 0.01f, 1f);
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 2000f;
            camera.fieldOfView = 50f;
            camera.allowHDR = true;
            camera.allowMSAA = true;

            UniversalAdditionalCameraData cameraData =
                camera.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
            cameraData.stopNaN = true;
            cameraData.dithering = true;
        }

        private static void CreateInteractionComposition(
            Transform parent,
            Camera camera,
            SolarSystemSimulationController simulationController,
            SolarSystemHudPresenter hudPresenter,
            AudioDirector audioDirector)
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
            serialized.FindProperty("audioDirector").objectReferenceValue = audioDirector;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static AudioDirector CreateAudioSystem(
            Transform audioRoot,
            CelestialBodyView sun,
            CelestialBodyView earth,
            SolarSystemSlice2Content content)
        {
            AudioDirector director = audioRoot.gameObject.AddComponent<AudioDirector>();
            AudioSource music = CreateAudioSource(
                "Music",
                audioRoot,
                content.MusicClip,
                true,
                0f);
            AudioSource ui = CreateAudioSource(
                "Interface Feedback",
                audioRoot,
                null,
                false,
                0f);
            AudioSource sunAmbience = CreateAudioSource(
                "Burning Ambience",
                sun.transform,
                content.SunAmbienceClip,
                true,
                0f);
            AudioSource earthAmbience = CreateAudioSource(
                "Forest Ambience",
                earth.transform,
                content.EarthAmbienceClip,
                true,
                1f);
            earthAmbience.rolloffMode = AudioRolloffMode.Logarithmic;
            earthAmbience.minDistance = EarthAmbienceMinimumDistance;
            earthAmbience.maxDistance = EarthAmbienceMaximumDistance;

            var serialized = new SerializedObject(director);
            serialized.FindProperty("musicSource").objectReferenceValue = music;
            serialized.FindProperty("sunAmbienceSource").objectReferenceValue = sunAmbience;
            serialized.FindProperty("earthAmbienceSource").objectReferenceValue = earthAmbience;
            serialized.FindProperty("uiSource").objectReferenceValue = ui;
            serialized.FindProperty("selectionClip").objectReferenceValue =
                content.SelectionClip;
            serialized.FindProperty("focusClip").objectReferenceValue = content.FocusClip;
            serialized.FindProperty("timeControlClip").objectReferenceValue =
                content.TimeControlClip;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return director;
        }

        private static AudioSource CreateAudioSource(
            string name,
            Transform parent,
            AudioClip clip,
            bool loop,
            float spatialBlend)
        {
            GameObject sourceObject = new GameObject(name);
            sourceObject.transform.SetParent(parent, false);
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = loop;
            source.playOnAwake = clip != null;
            source.spatialBlend = spatialBlend;
            source.dopplerLevel = 0f;
            source.reverbZoneMix = 0f;
            return source;
        }

        private static void CreateLighting(Transform sun)
        {
            GameObject lightObject = new GameObject(SolarRadialLightName);
            Light light = lightObject.AddComponent<Light>();
            ConfigureLighting(light, sun);
        }

        private static void ConfigureLighting(Light light, Transform sun)
        {
            light.gameObject.name = SolarRadialLightName;
            light.transform.SetParent(sun, false);
            light.transform.localPosition = Vector3.zero;
            light.transform.localRotation = Quaternion.identity;
            light.type = LightType.Point;
            light.lightUnit = LightUnit.Candela;
            light.intensity = SolarRadialIntensityCandela;
            light.range = SolarRadialRange;
            light.color = Color.white;
            light.useColorTemperature = true;
            light.colorTemperature = SolarRadialTemperature;
            light.shadows = LightShadows.None;
            light.bounceIntensity = 0f;
            RenderSettings.sun = null;
        }

        private static CelestialBodyView FindBodyView(string stableId)
        {
            CelestialBodyView[] views = Object.FindObjectsByType<CelestialBodyView>();
            foreach (CelestialBodyView view in views)
            {
                if (view.StableId == stableId)
                {
                    return view;
                }
            }

            throw new InvalidOperationException(
                $"The SolarSystem scene requires celestial body '{stableId}'.");
        }

        private static void CreateGlobalVolume(
            Transform parent,
            VolumeProfile profile)
        {
            GameObject volumeObject = new GameObject("Global Volume");
            volumeObject.transform.SetParent(parent, false);
            Volume volume = volumeObject.AddComponent<Volume>();
            ConfigureGlobalVolume(volume, profile);
        }

        private static void ConfigureGlobalVolume(
            Volume volume,
            VolumeProfile profile)
        {
            volume.isGlobal = true;
            volume.priority = 0f;
            volume.weight = 1f;
            volume.sharedProfile = profile;
        }

        private static void ConfigureRenderSettings(Material skybox)
        {
            RenderSettings.skybox = skybox;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = AmbientFill;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
            RenderSettings.defaultReflectionResolution = 128;
            RenderSettings.reflectionIntensity = ReflectionIntensity;
            RenderSettings.fog = false;
            DynamicGI.UpdateEnvironment();
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
                SimulationTimeControlService.BaselineSecondsPerRealSecond;
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
