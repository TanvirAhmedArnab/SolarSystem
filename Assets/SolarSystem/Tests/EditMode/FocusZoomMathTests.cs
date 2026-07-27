using System;
using NUnit.Framework;
using Tanvir.SolarSystem.Presentation.Camera;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class FocusZoomMathTests
    {
        private const float ScrollNotch = 120f;

        [Test]
        public void CalculateDistance_PositiveAndNegativeScrollMoveInOppositeDirections()
        {
            const float currentDistance = 10f;

            float zoomedIn = FocusZoomMath.CalculateDistance(
                currentDistance,
                ScrollNotch,
                1f,
                100f);
            float zoomedOut = FocusZoomMath.CalculateDistance(
                currentDistance,
                -ScrollNotch,
                1f,
                100f);

            Assert.That(zoomedIn, Is.LessThan(currentDistance));
            Assert.That(zoomedOut, Is.GreaterThan(currentDistance));
        }

        [Test]
        public void CalculateDistance_UsesTheSameFractionForSmallAndLargeBodies()
        {
            const float earthFocusDistance = 3.5f;
            const float sunFocusDistance = 382f;

            float earthResult = FocusZoomMath.CalculateDistance(
                earthFocusDistance,
                ScrollNotch,
                0.1f,
                1000f);
            float sunResult = FocusZoomMath.CalculateDistance(
                sunFocusDistance,
                ScrollNotch,
                0.1f,
                1000f);

            Assert.That(
                earthResult / earthFocusDistance,
                Is.EqualTo(sunResult / sunFocusDistance).Within(0.00001f));
        }

        [Test]
        public void CalculateDistance_ZeroScrollPreservesCurrentDistance()
        {
            Assert.That(
                FocusZoomMath.CalculateDistance(10f, 0f, 1f, 100f),
                Is.EqualTo(10f));
        }

        [Test]
        public void CalculateDistance_ExtremeScrollClampsToBothLimits()
        {
            Assert.That(
                FocusZoomMath.CalculateDistance(10f, 100000f, 2f, 20f),
                Is.EqualTo(2f));
            Assert.That(
                FocusZoomMath.CalculateDistance(10f, -100000f, 2f, 20f),
                Is.EqualTo(20f));
        }

        [TestCase(0f, 0f, 1f, 10f)]
        [TestCase(1f, float.NaN, 1f, 10f)]
        [TestCase(1f, 0f, 0f, 10f)]
        [TestCase(1f, 0f, 10f, 1f)]
        public void CalculateDistance_InvalidContractThrows(
            float currentDistance,
            float scrollDelta,
            float minimumDistance,
            float maximumDistance)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => FocusZoomMath.CalculateDistance(
                    currentDistance,
                    scrollDelta,
                    minimumDistance,
                    maximumDistance));
        }
    }
}
