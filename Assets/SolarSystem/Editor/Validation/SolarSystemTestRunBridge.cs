using System;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Tanvir.SolarSystem.Editor.Validation
{
    /// <summary>Persists automated Play Mode test completion across Unity domain reloads.</summary>
    [InitializeOnLoad]
    public static class SolarSystemTestRunBridge
    {
        private const string EditModePendingKey =
            "Tanvir.SolarSystem.Tests.EditMode.Pending";
        private const string EditModeResultKey =
            "Tanvir.SolarSystem.Tests.EditMode.Result";
        private const string PendingKey = "Tanvir.SolarSystem.Tests.PlayMode.Pending";
        private const string ResultKey = "Tanvir.SolarSystem.Tests.PlayMode.Result";
        private const string PendingValue = "PENDING";

        private static TestRunnerApi editModeApi;
        private static EditModeCallbacks editModeCallbacks;
        private static TestRunnerApi api;
        private static PlayModeCallbacks callbacks;

        static SolarSystemTestRunBridge()
        {
            if (IsEditModeRunPending)
            {
                EditorApplication.delayCall += AttachEditModeCallbacks;
            }

            if (IsPlayModeRunPending)
            {
                EditorApplication.delayCall += AttachCallbacks;
            }
        }

        /// <summary>Gets whether a bridge-started Edit Mode run is awaiting completion.</summary>
        public static bool IsEditModeRunPending =>
            SessionState.GetBool(EditModePendingKey, false);

        /// <summary>Gets the persisted Edit Mode result text, or PENDING while active.</summary>
        public static string EditModeResult =>
            SessionState.GetString(EditModeResultKey, string.Empty);

        /// <summary>Gets whether a bridge-started Play Mode run is awaiting completion.</summary>
        public static bool IsPlayModeRunPending =>
            SessionState.GetBool(PendingKey, false);

        /// <summary>Gets the persisted result text, or PENDING while the run is active.</summary>
        public static string PlayModeResult =>
            SessionState.GetString(ResultKey, string.Empty);

        /// <summary>Starts the project Edit Mode test assembly and persists its final counts.</summary>
        [MenuItem("Tools/Solar System/Validation/Run Edit Mode Tests")]
        public static void RunEditModeTests()
        {
            if (IsEditModeRunPending)
            {
                throw new InvalidOperationException("An Edit Mode test run is already pending.");
            }

            SessionState.SetBool(EditModePendingKey, true);
            SessionState.SetString(EditModeResultKey, PendingValue);
            AttachEditModeCallbacks();

            editModeApi.Execute(new ExecutionSettings(new Filter
            {
                testMode = TestMode.EditMode,
                assemblyNames = new[] { "Tanvir.SolarSystem.Tests.EditMode" }
            }));
        }

        /// <summary>Starts the project Play Mode test assembly and persists its final counts.</summary>
        [MenuItem("Tools/Solar System/Validation/Run Play Mode Tests")]
        public static void RunPlayModeTests()
        {
            if (IsPlayModeRunPending)
            {
                throw new InvalidOperationException("A Play Mode test run is already pending.");
            }

            SessionState.SetBool(PendingKey, true);
            SessionState.SetString(ResultKey, PendingValue);
            AttachCallbacks();

            api.Execute(new ExecutionSettings(new Filter
            {
                testMode = TestMode.PlayMode,
                assemblyNames = new[] { "Tanvir.SolarSystem.Tests.PlayMode" }
            }));
        }

        private static void AttachEditModeCallbacks()
        {
            if (!IsEditModeRunPending)
            {
                return;
            }

            editModeApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            editModeCallbacks = new EditModeCallbacks();
            editModeApi.RegisterCallbacks(editModeCallbacks);
        }

        /// <summary>Clears a completed bridge result without affecting Test Runner history.</summary>
        public static void ClearResult()
        {
            if (IsPlayModeRunPending)
            {
                throw new InvalidOperationException("Cannot clear a pending Play Mode test run.");
            }

            SessionState.EraseString(ResultKey);
        }

        private static void AttachCallbacks()
        {
            if (!IsPlayModeRunPending)
            {
                return;
            }

            api = ScriptableObject.CreateInstance<TestRunnerApi>();
            callbacks = new PlayModeCallbacks();
            api.RegisterCallbacks(callbacks);
        }

        private sealed class EditModeCallbacks : ICallbacks
        {
            private readonly StringBuilder failureDetails = new StringBuilder();

            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                string summary =
                    $"passed={result.PassCount}|failed={result.FailCount}|" +
                    $"skipped={result.SkipCount}|inconclusive={result.InconclusiveCount}|" +
                    $"duration={result.Duration:F3}|failures={failureDetails}";
                SessionState.SetString(EditModeResultKey, summary);
                SessionState.SetBool(EditModePendingKey, false);
                Debug.Log($"SOLAR_SYSTEM_EDITMODE_RESULTS|{summary}");
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.HasChildren || result.FailCount == 0)
                {
                    return;
                }

                if (failureDetails.Length > 0)
                {
                    failureDetails.Append(" || ");
                }

                failureDetails.Append(result.Test.FullName);
                failureDetails.Append(": ");
                failureDetails.Append(result.Message);
            }
        }

        private sealed class PlayModeCallbacks : ICallbacks
        {
            private readonly StringBuilder failureDetails = new StringBuilder();

            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                string summary =
                    $"passed={result.PassCount}|failed={result.FailCount}|" +
                    $"skipped={result.SkipCount}|inconclusive={result.InconclusiveCount}|" +
                    $"duration={result.Duration:F3}|failures={failureDetails}";
                SessionState.SetString(ResultKey, summary);
                SessionState.SetBool(PendingKey, false);
                Debug.Log($"SLICE2_PLAYMODE_RESULTS|{summary}");
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.HasChildren || result.FailCount == 0)
                {
                    return;
                }

                if (failureDetails.Length > 0)
                {
                    failureDetails.Append(" || ");
                }

                failureDetails.Append(result.Test.FullName);
                failureDetails.Append(": ");
                failureDetails.Append(result.Message);
            }
        }
    }
}
