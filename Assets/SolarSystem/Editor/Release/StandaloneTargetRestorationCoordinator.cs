using System;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Editor.Release
{
    /// <summary>
    /// Restores a standalone editor target after Unity finishes deferred build
    /// imports and preserves the request across domain reloads.
    /// </summary>
    [InitializeOnLoad]
    public static class StandaloneTargetRestorationCoordinator
    {
        private const string PendingKey =
            "Tanvir.SolarSystem.Release.TargetRestoration.Pending";
        private const string TargetKey =
            "Tanvir.SolarSystem.Release.TargetRestoration.Target";
        private const string BuiltTargetKey =
            "Tanvir.SolarSystem.Release.TargetRestoration.BuiltTarget";
        private const string BuiltTargetObservedKey =
            "Tanvir.SolarSystem.Release.TargetRestoration.BuiltTargetObserved";

        static StandaloneTargetRestorationCoordinator()
        {
            if (IsPending)
            {
                ScheduleNextAttempt();
            }
        }

        /// <summary>Gets whether an editor-target restoration is pending.</summary>
        public static bool IsPending =>
            SessionState.GetBool(PendingKey, false);

        /// <summary>
        /// Records and schedules restoration of the supplied standalone target
        /// after Unity has completed activation of the build target.
        /// </summary>
        /// <param name="target">
        /// Standalone target to restore after the build.
        /// </param>
        /// <param name="builtTarget">
        /// Final target used by the build sequence.
        /// </param>
        public static void Request(BuildTarget target, BuildTarget builtTarget)
        {
            if (!IsStandaloneTarget(target))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(target),
                    target,
                    "Only standalone desktop targets can be restored.");
            }

            if (builtTarget == BuildTarget.NoTarget)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(builtTarget),
                    builtTarget,
                    "A concrete final build target is required.");
            }

            SessionState.SetInt(TargetKey, (int)target);
            SessionState.SetInt(BuiltTargetKey, (int)builtTarget);
            SessionState.SetBool(
                BuiltTargetObservedKey,
                target == builtTarget);
            SessionState.SetBool(PendingKey, true);
            ScheduleNextAttempt();
        }

        /// <summary>
        /// Resolves a safe standalone restoration target for individual builds
        /// and ordered multi-platform build sequences.
        /// </summary>
        /// <param name="previousTarget">
        /// Editor target active before the release command.
        /// </param>
        /// <returns>
        /// The previous standalone target, or the approved Windows baseline
        /// when the previous target was not a standalone desktop platform.
        /// </returns>
        public static BuildTarget ResolveRestorationTarget(
            BuildTarget previousTarget)
        {
            return IsStandaloneTarget(previousTarget)
                ? previousTarget
                : BuildTarget.StandaloneWindows64;
        }

        /// <summary>
        /// Determines the next restoration action without mutating editor
        /// state, allowing the transition policy to be regression-tested.
        /// </summary>
        /// <param name="target">Requested standalone target.</param>
        /// <param name="activeTarget">Currently active editor target.</param>
        /// <param name="editorBusy">
        /// Whether Unity is compiling or importing assets.
        /// </param>
        /// <returns>The next coordinator action.</returns>
        public static StandaloneTargetRestorationStep GetNextStep(
            BuildTarget target,
            BuildTarget builtTarget,
            BuildTarget activeTarget,
            bool editorBusy,
            bool builtTargetObserved)
        {
            if (!builtTargetObserved)
            {
                return activeTarget == builtTarget
                    ? StandaloneTargetRestorationStep.ObserveBuiltTarget
                    : StandaloneTargetRestorationStep.Wait;
            }

            if (activeTarget == target)
            {
                return StandaloneTargetRestorationStep.Complete;
            }

            return editorBusy
                ? StandaloneTargetRestorationStep.Wait
                : StandaloneTargetRestorationStep.RequestSwitch;
        }

        private static void ProcessPendingRequest()
        {
            if (!IsPending)
            {
                return;
            }

            BuildTarget target = (BuildTarget)SessionState.GetInt(
                TargetKey,
                (int)BuildTarget.StandaloneWindows64);
            BuildTarget builtTarget = (BuildTarget)SessionState.GetInt(
                BuiltTargetKey,
                (int)target);
            bool builtTargetObserved = SessionState.GetBool(
                BuiltTargetObservedKey,
                target == builtTarget);
            bool editorBusy =
                EditorApplication.isCompiling || EditorApplication.isUpdating;
            StandaloneTargetRestorationStep step = GetNextStep(
                target,
                builtTarget,
                EditorUserBuildSettings.activeBuildTarget,
                editorBusy,
                builtTargetObserved);

            switch (step)
            {
                case StandaloneTargetRestorationStep.ObserveBuiltTarget:
                    SessionState.SetBool(BuiltTargetObservedKey, true);
                    ScheduleNextAttempt();
                    return;
                case StandaloneTargetRestorationStep.Complete:
                    Complete(target);
                    return;
                case StandaloneTargetRestorationStep.Wait:
                    ScheduleNextAttempt();
                    return;
                case StandaloneTargetRestorationStep.RequestSwitch:
                    if (!EditorUserBuildSettings.SwitchActiveBuildTargetAsync(
                            BuildTargetGroup.Standalone,
                            target))
                    {
                        ScheduleNextAttempt();
                    }

                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Complete(BuildTarget target)
        {
            SessionState.SetBool(PendingKey, false);
            SessionState.EraseInt(TargetKey);
            SessionState.EraseInt(BuiltTargetKey);
            SessionState.SetBool(BuiltTargetObservedKey, false);
            Debug.Log(
                $"Release editor target restoration completed: {target}.");
        }

        private static void ScheduleNextAttempt()
        {
            EditorApplication.delayCall -= ProcessPendingRequest;
            EditorApplication.delayCall += ProcessPendingRequest;
        }

        private static bool IsStandaloneTarget(BuildTarget target)
        {
            return target == BuildTarget.StandaloneWindows64 ||
                   target == BuildTarget.StandaloneWindows ||
                   target == BuildTarget.StandaloneOSX ||
                   target == BuildTarget.StandaloneLinux64;
        }
    }

    /// <summary>Describes the coordinator's next editor-target action.</summary>
    public enum StandaloneTargetRestorationStep
    {
        /// <summary>Wait until Unity finishes compiling or importing.</summary>
        Wait,

        /// <summary>
        /// Record that Unity has activated the target used by the build.
        /// </summary>
        ObserveBuiltTarget,

        /// <summary>The requested target is already active.</summary>
        Complete,

        /// <summary>Request an asynchronous active-target switch.</summary>
        RequestSwitch
    }
}
