using NUnit.Framework;
using Tanvir.SolarSystem.Presentation.Camera;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class GuidedCameraTransitionTests
    {
        [TestCase(GuidedCameraEasing.SmoothStep)]
        [TestCase(GuidedCameraEasing.SmootherStep)]
        public void Evaluate_IsDeterministicBoundedAndMonotonic(
            GuidedCameraEasing easing)
        {
            var transition = new GuidedCameraTransition(1.25f, easing);
            float previous = 0f;

            for (int step = 0; step <= 100; step++)
            {
                float value = transition.Evaluate(step / 100f);
                Assert.That(value, Is.InRange(0f, 1f));
                Assert.That(value, Is.GreaterThanOrEqualTo(previous));
                previous = value;
            }

            Assert.That(transition.Evaluate(0f), Is.Zero);
            Assert.That(transition.Evaluate(1f), Is.EqualTo(1f));
        }

        [Test]
        public void Instant_UsesZeroDuration()
        {
            Assert.That(GuidedCameraTransition.Instant.IsInstant, Is.True);
            Assert.That(GuidedCameraTransition.Instant.DurationSeconds, Is.Zero);
        }
    }
}
