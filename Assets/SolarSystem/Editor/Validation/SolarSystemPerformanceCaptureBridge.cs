using System;
using System.IO;
using Tanvir.SolarSystem.Diagnostics.Performance;
using UnityEditor;
using UnityEngine;
using UnityApplication = UnityEngine.Application;

namespace Tanvir.SolarSystem.Editor.Validation
{
    /// <summary>Starts and closes the reproducible Editor-only diagnostic run.</summary>
    [InitializeOnLoad]
    internal static class SolarSystemPerformanceCaptureBridge
    {
        private const string RequestedSessionKey =
            "Tanvir.SolarSystem.PerformanceCaptureRequested";
        private const string MenuPath =
            "Tools/Solar System/Validation/Run Performance Diagnostic";

        static SolarSystemPerformanceCaptureBridge()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            SolarSystemPerformanceHarness.CaptureCompleted -= OnCaptureCompleted;
            SolarSystemPerformanceHarness.CaptureCompleted += OnCaptureCompleted;
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            if (EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                Debug.LogWarning(
                    "Wait for Unity compilation/import to finish before " +
                    "starting the performance diagnostic.");
                return;
            }

            if (EditorApplication.isPlaying)
            {
                StartInPlayMode();
                return;
            }

            SessionState.SetBool(RequestedSessionKey, true);
            EditorApplication.EnterPlaymode();
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isCompiling &&
                !EditorApplication.isUpdating;
        }

        private static void OnPlayModeStateChanged(
            PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode &&
                SessionState.GetBool(RequestedSessionKey, false))
            {
                SessionState.SetBool(RequestedSessionKey, false);
                EditorApplication.delayCall += StartInPlayMode;
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                SessionState.SetBool(RequestedSessionKey, false);
            }
        }

        private static void StartInPlayMode()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            string projectRoot = Directory.GetParent(UnityApplication.dataPath)
                ?.FullName;
            if (string.IsNullOrEmpty(projectRoot))
            {
                throw new InvalidOperationException(
                    "Cannot resolve the Unity project root.");
            }

            string resultPath = Path.Combine(
                projectRoot,
                "Temp",
                "Performance",
                "solar-system-editor-diagnostic.json");
            SolarSystemPerformanceHarness.StartCapture(
                PerformanceCaptureOptions.CreateEditorDiagnostic(resultPath));
        }

        private static void OnCaptureCompleted(string resultPath)
        {
            if (!string.IsNullOrEmpty(resultPath))
            {
                Debug.Log(
                    "Editor performance diagnostic completed: " + resultPath);
            }

            EditorApplication.delayCall += StopPlayMode;
        }

        private static void StopPlayMode()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.ExitPlaymode();
            }
        }
    }
}
