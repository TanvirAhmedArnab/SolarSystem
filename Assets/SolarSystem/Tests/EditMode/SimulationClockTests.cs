using System;
using NUnit.Framework;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class SimulationClockTests
    {
        [Test]
        public void Advance_WithSpeedMultiplier_AdvancesAuthoritativeTime()
        {
            var clock = new SimulationClock(initialTimeSeconds: 5d, initialSpeedMultiplier: 4d);

            clock.Advance(2.5d);

            Assert.That(clock.Snapshot.ElapsedSimulationTimeSeconds, Is.EqualTo(15d).Within(1e-12d));
        }

        [Test]
        public void Advance_WhilePaused_DoesNotAdvanceTime()
        {
            var clock = new SimulationClock(initialTimeSeconds: 5d);
            clock.SetPaused(true);

            clock.Advance(10d);

            Assert.That(clock.Snapshot.ElapsedSimulationTimeSeconds, Is.EqualTo(5d));
        }

        [Test]
        public void Advance_DoesNotRaiseSettingsChangedEvent()
        {
            var clock = new SimulationClock();
            int eventCount = 0;
            clock.Changed += () => eventCount++;

            clock.Advance(1d);

            Assert.That(eventCount, Is.Zero);
        }

        [Test]
        public void PauseAndSpeedChanges_RaiseOneEventPerEffectiveChange()
        {
            var clock = new SimulationClock();
            int eventCount = 0;
            clock.Changed += () => eventCount++;

            clock.SetPaused(true);
            clock.SetPaused(true);
            clock.SetSpeedMultiplier(10d);
            clock.SetSpeedMultiplier(10d);

            Assert.That(eventCount, Is.EqualTo(2));
        }

        [TestCase(0d)]
        [TestCase(-1d)]
        [TestCase(double.PositiveInfinity)]
        public void SetSpeedMultiplier_InvalidValue_Throws(double multiplier)
        {
            var clock = new SimulationClock();

            Assert.Throws<ArgumentOutOfRangeException>(() => clock.SetSpeedMultiplier(multiplier));
        }

        [Test]
        public void Advance_NegativeDelta_Throws()
        {
            var clock = new SimulationClock();

            Assert.Throws<ArgumentOutOfRangeException>(() => clock.Advance(-0.01d));
        }
    }
}
