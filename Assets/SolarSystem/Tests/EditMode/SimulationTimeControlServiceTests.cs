using System;
using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class SimulationTimeControlServiceTests
    {
        [Test]
        public void Baseline_UsesOneEarthSiderealRotationPerRealSecond()
        {
            Assert.That(
                SimulationTimeControlService.BaselineSecondsPerRealSecond,
                Is.EqualTo(CelestialReferenceUnits.EarthSiderealRotationPeriodSeconds)
                    .Within(0.000001d));
        }

        [Test]
        public void Constructor_ReadsSupportedInitialPreset()
        {
            var controller = new FakeTimeController(
                SimulationTimeControlService.BaselineSecondsPerRealSecond * 10d);

            var service = new SimulationTimeControlService(controller);

            Assert.That(service.CurrentMultiplier, Is.EqualTo(10));
            Assert.That(service.IsPaused, Is.False);
        }

        [Test]
        public void TogglePaused_PreservesSpeedAndRaisesChange()
        {
            var controller = new FakeTimeController(
                SimulationTimeControlService.BaselineSecondsPerRealSecond * 10d);
            var service = new SimulationTimeControlService(controller);
            int changeCount = 0;
            service.Changed += () => changeCount++;

            service.TogglePaused();

            Assert.That(service.IsPaused, Is.True);
            Assert.That(service.CurrentMultiplier, Is.EqualTo(10));
            Assert.That(changeCount, Is.EqualTo(1));
        }

        [Test]
        public void IncreaseAndDecreaseSpeed_ApplyAdjacentPresets()
        {
            var controller = new FakeTimeController(
                SimulationTimeControlService.BaselineSecondsPerRealSecond * 10d);
            var service = new SimulationTimeControlService(controller);

            Assert.That(service.IncreaseSpeed(), Is.True);
            Assert.That(service.CurrentMultiplier, Is.EqualTo(100));
            Assert.That(
                controller.ClockSnapshot.SpeedMultiplier,
                Is.EqualTo(SimulationTimeControlService.BaselineSecondsPerRealSecond * 100d));

            Assert.That(service.DecreaseSpeed(), Is.True);
            Assert.That(service.CurrentMultiplier, Is.EqualTo(10));
        }

        [Test]
        public void SpeedCommands_AtBounds_DoNotWrapOrRaiseChange()
        {
            var controller = new FakeTimeController(
                SimulationTimeControlService.BaselineSecondsPerRealSecond);
            var service = new SimulationTimeControlService(controller);
            int changeCount = 0;
            service.Changed += () => changeCount++;

            Assert.That(service.DecreaseSpeed(), Is.False);
            service.SetPresetIndex(service.PresetCount - 1);
            changeCount = 0;
            Assert.That(service.IncreaseSpeed(), Is.False);

            Assert.That(service.CurrentMultiplier, Is.EqualTo(10000));
            Assert.That(changeCount, Is.Zero);
        }

        [TestCase(-1)]
        [TestCase(5)]
        public void SetPresetIndex_OutsideSupportedRange_Throws(int index)
        {
            var controller = new FakeTimeController(
                SimulationTimeControlService.BaselineSecondsPerRealSecond);
            var service = new SimulationTimeControlService(controller);

            Assert.Throws<ArgumentOutOfRangeException>(() => service.SetPresetIndex(index));
        }

        [Test]
        public void Constructor_WithUnapprovedInitialRate_Throws()
        {
            var controller = new FakeTimeController(12345d);

            Assert.Throws<InvalidOperationException>(
                () => new SimulationTimeControlService(controller));
        }

        private sealed class FakeTimeController : ISimulationTimeController
        {
            private double speed;
            private bool paused;

            internal FakeTimeController(double initialSpeed)
            {
                speed = initialSpeed;
            }

            public SimulationClockSnapshot ClockSnapshot =>
                new SimulationClockSnapshot(0d, paused, speed);

            public void SetPaused(bool isPaused)
            {
                paused = isPaused;
            }

            public void SetSpeedMultiplier(double simulationSecondsPerRealSecond)
            {
                speed = simulationSecondsPerRealSecond;
            }
        }
    }
}
