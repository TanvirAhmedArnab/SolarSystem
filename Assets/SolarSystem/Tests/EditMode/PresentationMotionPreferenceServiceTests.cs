using NUnit.Framework;
using Tanvir.SolarSystem.Application;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class PresentationMotionPreferenceServiceTests
    {
        [Test]
        public void Constructor_LoadsValidPersistedPreference()
        {
            var store = new MemoryStore
            {
                HasValue = true,
                Value = PresentationMotionMode.ReducedMotion
            };

            var service = new PresentationMotionPreferenceService(store);

            Assert.That(service.IsReducedMotion, Is.True);
            Assert.That(store.SaveCount, Is.Zero);
        }

        [Test]
        public void Toggle_PersistsAndRaisesOneEffectiveChange()
        {
            var store = new MemoryStore();
            var service = new PresentationMotionPreferenceService(store);
            int changeCount = 0;
            service.Changed += () => changeCount++;

            service.Toggle();

            Assert.That(service.Mode, Is.EqualTo(PresentationMotionMode.ReducedMotion));
            Assert.That(store.Value, Is.EqualTo(PresentationMotionMode.ReducedMotion));
            Assert.That(store.SaveCount, Is.EqualTo(1));
            Assert.That(changeCount, Is.EqualTo(1));
        }

        [Test]
        public void SetMode_WithCurrentValue_DoesNotWriteOrPublish()
        {
            var store = new MemoryStore();
            var service = new PresentationMotionPreferenceService(store);
            int changeCount = 0;
            service.Changed += () => changeCount++;

            service.SetMode(PresentationMotionMode.FullMotion);

            Assert.That(store.SaveCount, Is.Zero);
            Assert.That(changeCount, Is.Zero);
        }

        private sealed class MemoryStore : IPresentationMotionPreferenceStore
        {
            internal bool HasValue;
            internal PresentationMotionMode Value;
            internal int SaveCount;

            public bool TryLoad(out PresentationMotionMode mode)
            {
                mode = Value;
                return HasValue;
            }

            public void Save(PresentationMotionMode mode)
            {
                HasValue = true;
                Value = mode;
                SaveCount++;
            }
        }
    }
}
