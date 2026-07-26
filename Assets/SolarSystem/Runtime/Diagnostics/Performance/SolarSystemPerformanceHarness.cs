using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityApplication = UnityEngine.Application;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    /// <summary>
    /// Runs an explicitly activated, allocation-aware scenario capture against
    /// the production scene and writes machine-readable evidence after sampling.
    /// </summary>
    [DefaultExecutionOrder(32000)]
    [DisallowMultipleComponent]
    public sealed class SolarSystemPerformanceHarness : MonoBehaviour
    {
        /// <summary>Approved total-frame P95 budget at 1920x1080.</summary>
        public const long FrameTimeP95BudgetNanoseconds = 16_670_000L;

        /// <summary>Approved total-frame P99 budget at 1920x1080.</summary>
        public const long FrameTimeP99BudgetNanoseconds = 25_000_000L;

        /// <summary>Approved CPU main-thread P95 headroom budget.</summary>
        public const long CpuMainThreadP95BudgetNanoseconds = 13_330_000L;

        /// <summary>Approved GPU P95 headroom budget.</summary>
        public const long GpuP95BudgetNanoseconds = 13_330_000L;

        /// <summary>Approved steady-state managed-allocation P95 budget.</summary>
        public const long GcAllocationP95BudgetBytes = 0L;

        private const int PreparationTimeoutFrames = 3600;
        private const int MinimumScenarioSampleCapacity = 1024;
        private const int SampleCapacityMultiplier = 4;

        private static double runtimeInitializationStartSeconds;
        private static bool hasStarted;

        private readonly PerformanceScenarioDriver scenarioDriver =
            new PerformanceScenarioDriver();
        private readonly List<PerformanceScenarioCapture> completedScenarios =
            new List<PerformanceScenarioCapture>(
                PerformanceScenarioDriver.ScenarioCount);

        private PerformanceCaptureOptions options;
        private PerformanceMetricSource[] metricSources;
        private PerformanceScenarioCapture activeScenario;
        private CapturePhase phase;
        private int scenarioIndex;
        private int phaseFrames;
        private float phaseSeconds;
        private int preparationFrames;
        private int previousVSyncCount;
        private int previousTargetFrameRate;
        private double interactiveSeconds;
        private bool settingsRestored;
        private bool completionStarted;

        /// <summary>
        /// Raised after the evidence file is closed. Editor tooling uses this to
        /// stop Play Mode without introducing an Editor dependency into runtime.
        /// </summary>
        public static event Action<string> CaptureCompleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RecordRuntimeInitializationStart()
        {
            runtimeInitializationStartSeconds = Time.realtimeSinceStartupAsDouble;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void TryStartFromCommandLine()
        {
            try
            {
                PerformanceCaptureOptions parsed =
                    PerformanceCaptureOptions.Parse(
                        Environment.GetCommandLineArgs());
                if (parsed.IsEnabled)
                {
                    StartCapture(parsed);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "Solar System performance arguments are invalid.\n" +
                    exception);
            }
        }

        /// <summary>Starts one explicit capture in the currently playing scene.</summary>
        public static void StartCapture(PerformanceCaptureOptions captureOptions)
        {
            if (captureOptions == null)
            {
                throw new ArgumentNullException(nameof(captureOptions));
            }

            if (!captureOptions.IsEnabled)
            {
                throw new ArgumentException(
                    "Performance capture options are not enabled.",
                    nameof(captureOptions));
            }

            if (!UnityApplication.isPlaying)
            {
                throw new InvalidOperationException(
                    "The performance harness requires Play Mode or a Player.");
            }

            if (hasStarted)
            {
                throw new InvalidOperationException(
                    "A Solar System performance capture is already active.");
            }

            hasStarted = true;
            var host = new GameObject("Solar System Performance Harness");
            DontDestroyOnLoad(host);
            host.AddComponent<SolarSystemPerformanceHarness>()
                .Initialize(captureOptions);
        }

        private void Initialize(PerformanceCaptureOptions captureOptions)
        {
            options = captureOptions;
            previousVSyncCount = QualitySettings.vSyncCount;
            previousTargetFrameRate = UnityApplication.targetFrameRate;
            QualitySettings.vSyncCount = 0;
            UnityApplication.targetFrameRate = -1;
            metricSources = PerformanceEvidenceFactory.CreateMetricSources();
            phase = CapturePhase.WaitingForScene;
            Debug.Log(
                "Solar System performance capture started. Normal gameplay " +
                "does not activate this harness.");
        }

        private void Update()
        {
            if (completionStarted)
            {
                return;
            }

            try
            {
                switch (phase)
                {
                    case CapturePhase.WaitingForScene:
                        WaitForScene();
                        break;
                    case CapturePhase.Preparing:
                        PrepareScenario();
                        break;
                    case CapturePhase.WarmingUp:
                        WarmUpScenario();
                        break;
                    case CapturePhase.Sampling:
                        SampleScenario();
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Unsupported capture phase '{phase}'.");
                }
            }
            catch (Exception exception)
            {
                Complete("HarnessError", exception.ToString(), 2);
            }
        }

        private void WaitForScene()
        {
            preparationFrames++;
            if (!scenarioDriver.TryResolveDependencies(
                out string pendingTarget))
            {
                ThrowIfPreparationTimedOut(pendingTarget);
                return;
            }

            interactiveSeconds =
                Time.realtimeSinceStartupAsDouble -
                runtimeInitializationStartSeconds;
            scenarioIndex = 0;
            preparationFrames = 0;
            phase = CapturePhase.Preparing;
        }

        private void PrepareScenario()
        {
            preparationFrames++;
            PerformanceScenarioDescriptor descriptor =
                scenarioDriver.GetDescriptor(scenarioIndex);
            if (!scenarioDriver.TryPrepare(descriptor))
            {
                ThrowIfPreparationTimedOut(descriptor.Name);
                return;
            }

            int capacity = Math.Max(
                options.SampleFrames * SampleCapacityMultiplier,
                MinimumScenarioSampleCapacity);
            capacity = Math.Min(
                capacity,
                PerformanceCaptureOptions.MaximumSamples);
            activeScenario = new PerformanceScenarioCapture(
                descriptor,
                metricSources,
                capacity);
            phaseFrames = 0;
            phaseSeconds = 0f;
            phase = CapturePhase.WarmingUp;
        }

        private void WarmUpScenario()
        {
            scenarioDriver.EnsureStable(
                scenarioDriver.GetDescriptor(scenarioIndex));
            phaseFrames++;
            phaseSeconds += Time.unscaledDeltaTime;
            if (phaseFrames < options.WarmupFrames ||
                phaseSeconds < options.WarmupSeconds)
            {
                return;
            }

            phaseFrames = 0;
            phaseSeconds = 0f;
            phase = CapturePhase.Sampling;
        }

        private void SampleScenario()
        {
            scenarioDriver.EnsureStable(
                scenarioDriver.GetDescriptor(scenarioIndex));
            bool hasCapacity = activeScenario.Capture(
                metricSources,
                Time.unscaledDeltaTime);
            phaseFrames++;
            phaseSeconds += Time.unscaledDeltaTime;
            if (hasCapacity &&
                (phaseFrames < options.SampleFrames ||
                 phaseSeconds < options.SampleSeconds))
            {
                return;
            }

            FinishScenario();
        }

        private void FinishScenario()
        {
            completedScenarios.Add(activeScenario);
            Debug.Log(
                activeScenario.Descriptor.Id +
                " captured " +
                activeScenario.Count.ToString(
                    CultureInfo.InvariantCulture) +
                " frames across " +
                activeScenario.ElapsedSeconds.ToString(
                    "F3",
                    CultureInfo.InvariantCulture) +
                " seconds.");

            activeScenario = null;
            scenarioIndex++;
            preparationFrames = 0;
            if (scenarioIndex < scenarioDriver.Count)
            {
                phase = CapturePhase.Preparing;
                return;
            }

            Complete("Captured", null, 0);
        }

        private void ThrowIfPreparationTimedOut(string target)
        {
            if (preparationFrames >= PreparationTimeoutFrames)
            {
                throw new TimeoutException(
                    $"Timed out preparing performance target '{target}'.");
            }
        }

        private void Complete(
            string requestedStatus,
            string error,
            int exitCode)
        {
            if (completionStarted)
            {
                return;
            }

            completionStarted = true;
            string resultPath = null;
            try
            {
                RestoreFrameSettings();
                resultPath =
                    PerformanceEvidenceFactory.ResolveResultPath(options);
                PerformanceCaptureDocument document =
                    PerformanceEvidenceFactory.CreateDocument(
                        completedScenarios,
                        scenarioDriver.Count,
                        metricSources,
                        options,
                        interactiveSeconds,
                        requestedStatus,
                        error,
                        resultPath);
                string directory = Path.GetDirectoryName(resultPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(
                    resultPath,
                    JsonUtility.ToJson(document, true));
                Debug.Log(
                    "Solar System performance evidence written to '" +
                    resultPath +
                    "'. Overall status: " +
                    document.overallStatus +
                    ".");
            }
            catch (Exception writeException)
            {
                exitCode = 3;
                Debug.LogError(
                    "Solar System performance evidence could not be written.\n" +
                    writeException);
            }
            finally
            {
                DisposeMetrics();
                hasStarted = false;
                CaptureCompleted?.Invoke(resultPath);
                if (!UnityApplication.isEditor && options.QuitOnComplete)
                {
                    UnityApplication.Quit(exitCode);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        private void RestoreFrameSettings()
        {
            if (settingsRestored)
            {
                return;
            }

            QualitySettings.vSyncCount = previousVSyncCount;
            UnityApplication.targetFrameRate = previousTargetFrameRate;
            settingsRestored = true;
        }

        private void DisposeMetrics()
        {
            if (metricSources == null)
            {
                return;
            }

            for (int index = 0; index < metricSources.Length; index++)
            {
                metricSources[index]?.Dispose();
            }

            metricSources = null;
        }

        private void OnDestroy()
        {
            RestoreFrameSettings();
            DisposeMetrics();
            hasStarted = false;
        }

        private enum CapturePhase
        {
            WaitingForScene = 0,
            Preparing = 1,
            WarmingUp = 2,
            Sampling = 3
        }
    }
}
