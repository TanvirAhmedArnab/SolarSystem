using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class GuidedScaleComparisonServiceTests
    {
        private FakeScaleController scale;
        private FakeTimeController timeController;
        private SimulationTimeControlService time;
        private GuidedScaleComparisonService service;

        [SetUp]
        public void SetUp()
        {
            scale = new FakeScaleController();
            timeController = new FakeTimeController();
            time = new SimulationTimeControlService(timeController);
            service = new GuidedScaleComparisonService(scale, time);
        }

        [Test]
        public void Advance_TraversesThreeStagesAndRestoresReadableRunningState()
        {
            service.Advance();
            Assert.That(service.Stage, Is.EqualTo(
                GuidedScaleComparisonStage.ReadableOverview));
            Assert.That(service.CurrentStep, Is.EqualTo(1));
            Assert.That(time.IsPaused, Is.True);

            service.Advance();
            Assert.That(service.Stage, Is.EqualTo(
                GuidedScaleComparisonStage.NormalizedOrbits));
            Assert.That(scale.ScaleMode, Is.EqualTo(
                CelestialScaleMode.NormalizedOrbits));

            service.Advance();
            Assert.That(service.Stage, Is.EqualTo(
                GuidedScaleComparisonStage.LiteralEarthReference));
            Assert.That(scale.ScaleMode, Is.EqualTo(
                CelestialScaleMode.LiteralEarthReference));

            service.Advance();
            Assert.That(service.Stage, Is.EqualTo(
                GuidedScaleComparisonStage.Inactive));
            Assert.That(scale.ScaleMode, Is.EqualTo(
                CelestialScaleMode.ReadableOverview));
            Assert.That(time.IsPaused, Is.False);
        }

        [Test]
        public void Cancel_FromAnyActiveStage_RestoresPriorPauseState()
        {
            time.SetPaused(true);
            service.Advance();
            service.Advance();

            Assert.That(service.Cancel(), Is.True);

            Assert.That(service.IsActive, Is.False);
            Assert.That(time.IsPaused, Is.True);
            Assert.That(scale.ScaleMode, Is.EqualTo(
                CelestialScaleMode.ReadableOverview));
        }

        [Test]
        public void Cancel_WhileInactive_IsIdempotent()
        {
            int changes = 0;
            service.Changed += () => changes++;

            Assert.That(service.Cancel(), Is.False);

            Assert.That(changes, Is.Zero);
            Assert.That(service.CurrentStep, Is.Zero);
        }

        [Test]
        public void EveryEffectiveAdvance_RaisesOneComparisonChange()
        {
            int changes = 0;
            service.Changed += () => changes++;

            service.Advance();
            service.Advance();
            service.Advance();
            service.Advance();

            Assert.That(changes, Is.EqualTo(4));
        }

        private sealed class FakeScaleController : IScaleModeController
        {
            public CelestialScaleMode ScaleMode { get; private set; } =
                CelestialScaleMode.ReadableOverview;

            public void SetScaleMode(CelestialScaleMode mode)
            {
                ScaleMode = mode;
            }
        }

        private sealed class FakeTimeController : ISimulationTimeController
        {
            private bool paused;

            public SimulationClockSnapshot ClockSnapshot =>
                new SimulationClockSnapshot(
                    0d,
                    paused,
                    SimulationTimeControlService.BaselineSecondsPerRealSecond);

            public void SetPaused(bool isPaused)
            {
                paused = isPaused;
            }

            public void SetSpeedMultiplier(double simulationSecondsPerRealSecond)
            {
            }
        }
    }
}
