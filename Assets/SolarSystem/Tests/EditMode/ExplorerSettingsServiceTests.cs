using NUnit.Framework;
using Tanvir.SolarSystem.Application;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class ExplorerSettingsServiceTests
    {
        [Test]
        public void Service_LoadsChangesPersistsAndRestoresApprovedDefaults()
        {
            var store = new MemoryStore
            {
                HasValue = true,
                Value = new ExplorerSettingsSnapshot(
                    0.8f,
                    0.3f,
                    0.4f,
                    0.5f,
                    true,
                    PresentationMotionMode.ReducedMotion,
                    false,
                    false,
                    true)
            };
            var service = new ExplorerSettingsService(store);
            int changes = 0;
            service.Changed += () => changes++;

            Assert.That(service.Current, Is.EqualTo(store.Value));

            service.SetMasterVolume(0.7f);
            service.SetMuted(false);
            service.SetOrbitGuidesEnabled(true);
            service.SetWorldLabelsEnabled(true);
            service.ResetToDefaults();

            ExplorerSettingsSnapshot current = service.Current;
            Assert.That(current.MasterVolume, Is.EqualTo(0.65f));
            Assert.That(current.MusicVolume, Is.EqualTo(0.18f));
            Assert.That(current.UiVolume, Is.EqualTo(0.45f));
            Assert.That(current.CelestialVolume, Is.EqualTo(0.22f));
            Assert.That(current.IsMuted, Is.False);
            Assert.That(current.MotionMode, Is.EqualTo(PresentationMotionMode.FullMotion));
            Assert.That(current.AreOrbitGuidesEnabled, Is.True);
            Assert.That(current.AreWorldLabelsEnabled, Is.True);
            Assert.That(current.HasCompletedOnboarding, Is.True);
            Assert.That(store.SaveCount, Is.EqualTo(changes));
        }

        [Test]
        public void Service_DefaultsAndCompletesOnboardingWithoutDuplicateWrites()
        {
            var store = new MemoryStore();
            var service = new ExplorerSettingsService(store);

            Assert.That(service.Current, Is.EqualTo(
                ExplorerSettingsSnapshot.CreateDefaults()));
            service.CompleteOnboarding();
            service.CompleteOnboarding();

            Assert.That(service.Current.HasCompletedOnboarding, Is.True);
            Assert.That(store.SaveCount, Is.EqualTo(1));
        }

        private sealed class MemoryStore : IExplorerSettingsStore
        {
            public bool HasValue;
            public ExplorerSettingsSnapshot Value;
            public int SaveCount;

            public bool TryLoad(out ExplorerSettingsSnapshot settings)
            {
                settings = Value;
                return HasValue;
            }

            public void Save(ExplorerSettingsSnapshot settings)
            {
                HasValue = true;
                Value = settings;
                SaveCount++;
            }
        }
    }
}
