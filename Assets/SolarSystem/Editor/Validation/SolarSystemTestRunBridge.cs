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
        private const string PendingKey = "Tanvir.SolarSystem.Tests.PlayMode.Pending";
        private const string ResultKey = "Tanvir.SolarSystem.Tests.PlayMode.Result";
        private const string PendingValue = "PENDING";

        private static TestRunnerApi api;
        private static PlayModeCallbacks callbacks;

        static SolarSystemTestRunBridge()
        {
            if (IsPlayModeRunPending)
            {
                EditorApplication.delayCall += AttachCallbacks;
            }
        }

        /// <summary>Gets whether a bridge-started Play Mode run is awaiting completion.</summary>
        public static bool IsPlayModeRunPending =>
            SessionState.GetBool(PendingKey, false);

        /// <summary>Gets the persisted result text, or PENDING while the run is active.</summary>
        public static string PlayModeResult =>
            SessionState.GetString(ResultKey, string.Empty);

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
