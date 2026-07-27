using NUnit.Framework;
using Tanvir.SolarSystem.Editor.Release;
using UnityEditor;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class ReleaseTargetRestorationTests
    {
        [Test]
        public void GetNextStep_WhenRequestedTargetIsActive_Completes()
        {
            StandaloneTargetRestorationStep step =
                StandaloneTargetRestorationCoordinator.GetNextStep(
                    BuildTarget.StandaloneWindows64,
                    BuildTarget.StandaloneOSX,
                    BuildTarget.StandaloneWindows64,
                    editorBusy: true,
                    builtTargetObserved: true);

            Assert.That(
                step,
                Is.EqualTo(StandaloneTargetRestorationStep.Complete));
        }

        [Test]
        public void GetNextStep_WhenEditorIsBusy_Waits()
        {
            StandaloneTargetRestorationStep step =
                StandaloneTargetRestorationCoordinator.GetNextStep(
                    BuildTarget.StandaloneWindows64,
                    BuildTarget.StandaloneOSX,
                    BuildTarget.StandaloneOSX,
                    editorBusy: true,
                    builtTargetObserved: true);

            Assert.That(
                step,
                Is.EqualTo(StandaloneTargetRestorationStep.Wait));
        }

        [Test]
        public void GetNextStep_WhenEditorIsIdleAndTargetDiffers_RequestsSwitch()
        {
            StandaloneTargetRestorationStep step =
                StandaloneTargetRestorationCoordinator.GetNextStep(
                    BuildTarget.StandaloneWindows64,
                    BuildTarget.StandaloneOSX,
                    BuildTarget.StandaloneOSX,
                    editorBusy: false,
                    builtTargetObserved: true);

            Assert.That(
                step,
                Is.EqualTo(StandaloneTargetRestorationStep.RequestSwitch));
        }

        [Test]
        public void GetNextStep_BeforeBuildTargetActivation_DoesNotCompleteEarly()
        {
            StandaloneTargetRestorationStep step =
                StandaloneTargetRestorationCoordinator.GetNextStep(
                    BuildTarget.StandaloneWindows64,
                    BuildTarget.StandaloneOSX,
                    BuildTarget.StandaloneWindows64,
                    editorBusy: false,
                    builtTargetObserved: false);

            Assert.That(
                step,
                Is.EqualTo(StandaloneTargetRestorationStep.Wait));
        }

        [Test]
        public void GetNextStep_WhenBuildTargetBecomesActive_ObservesItFirst()
        {
            StandaloneTargetRestorationStep step =
                StandaloneTargetRestorationCoordinator.GetNextStep(
                    BuildTarget.StandaloneWindows64,
                    BuildTarget.StandaloneOSX,
                    BuildTarget.StandaloneOSX,
                    editorBusy: false,
                    builtTargetObserved: false);

            Assert.That(
                step,
                Is.EqualTo(
                    StandaloneTargetRestorationStep.ObserveBuiltTarget));
        }
    }
}
