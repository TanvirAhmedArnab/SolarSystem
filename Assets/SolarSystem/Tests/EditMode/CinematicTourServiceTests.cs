using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Presentation.Camera;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CinematicTourServiceTests
    {
        private GuidedPresentationCoordinator coordinator;
        private CinematicTourService service;

        [SetUp]
        public void SetUp()
        {
            coordinator = new GuidedPresentationCoordinator();
            service = new CinematicTourService(
                new[]
                {
                    Chapter("sun", 2f),
                    Chapter("earth-moon", 3f),
                    Chapter("outer-system", 4f)
                },
                coordinator);
        }

        [Test]
        public void Start_AcquiresExclusivePresentationAndSelectsFirstChapter()
        {
            Assert.That(service.Start(), Is.True);

            Assert.That(service.IsActive, Is.True);
            Assert.That(service.CurrentChapter.StableId, Is.EqualTo("sun"));
            Assert.That(service.CurrentChapterNumber, Is.EqualTo(1));
            Assert.That(
                coordinator.Owner,
                Is.EqualTo(GuidedPresentationOwner.CinematicTour));
        }

        [Test]
        public void Tick_CarriesExcessTimeAcrossDeterministicChapterBoundaries()
        {
            service.Start();

            service.Tick(2.5f);

            Assert.That(service.CurrentChapter.StableId, Is.EqualTo("earth-moon"));
            Assert.That(service.ElapsedSeconds, Is.EqualTo(0.5f).Within(0.0001f));
        }

        [Test]
        public void Tick_PastFinalChapter_CompletesAndReleasesOwnership()
        {
            service.Start();

            service.Tick(9f);

            Assert.That(service.IsActive, Is.False);
            Assert.That(
                coordinator.Owner,
                Is.EqualTo(GuidedPresentationOwner.None));
        }

        [Test]
        public void Cancel_ReleasesOwnershipAndIsIdempotent()
        {
            service.Start();

            Assert.That(service.Cancel(), Is.True);
            Assert.That(service.Cancel(), Is.False);
            Assert.That(
                coordinator.Owner,
                Is.EqualTo(GuidedPresentationOwner.None));
        }

        [Test]
        public void Start_WhenScaleComparisonOwnsPresentation_IsRejected()
        {
            coordinator.TryAcquire(GuidedPresentationOwner.ScaleComparison);

            Assert.That(service.Start(), Is.False);
            Assert.That(service.IsActive, Is.False);
            Assert.That(
                coordinator.Owner,
                Is.EqualTo(GuidedPresentationOwner.ScaleComparison));
        }

        [Test]
        public void Advance_SkipsChaptersAndCompletesFromFinalChapter()
        {
            service.Start();

            Assert.That(service.Advance(), Is.True);
            Assert.That(service.CurrentChapter.StableId, Is.EqualTo("earth-moon"));
            Assert.That(service.Advance(), Is.True);
            Assert.That(service.CurrentChapter.StableId, Is.EqualTo("outer-system"));
            Assert.That(service.Advance(), Is.True);
            Assert.That(service.IsActive, Is.False);
        }

        [Test]
        public void Chapter_PreservesValidatedAuthoredShotContract()
        {
            var chapter = new CinematicTourChapter(
                "earth-moon",
                "A Paired World",
                "Earth and Moon",
                "Educational description.",
                new[] { "earth", "moon" },
                12f,
                1.1f,
                new Vector3(0.2f, 0.3f, -1f),
                CinematicTourFramingSpace.SunlitTargetAxis,
                new Vector2(0.22f, 0.18f),
                1.25f,
                GuidedCameraEasing.SmootherStep);

            Assert.That(
                chapter.FramingSpace,
                Is.EqualTo(CinematicTourFramingSpace.SunlitTargetAxis));
            Assert.That(chapter.ScreenOffset, Is.EqualTo(new Vector2(0.22f, 0.18f)));
            Assert.That(chapter.TransitionDurationSeconds, Is.EqualTo(1.25f));
            Assert.That(
                chapter.TransitionEasing,
                Is.EqualTo(GuidedCameraEasing.SmootherStep));
            Assert.That(chapter.FramingDirection.magnitude, Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void Chapter_RejectsUnsafeScreenOffset()
        {
            Assert.That(
                () => new CinematicTourChapter(
                    "sun",
                    "Sun",
                    "The Sun",
                    "Educational description.",
                    new[] { "sun" },
                    10f,
                    1.2f,
                    Vector3.forward,
                    CinematicTourFramingSpace.World,
                    new Vector2(0.8f, 0f),
                    1f,
                    GuidedCameraEasing.SmoothStep),
                Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }

        private static CinematicTourChapter Chapter(string id, float duration)
        {
            return new CinematicTourChapter(
                id,
                id,
                "SUBTITLE",
                "Educational description.",
                new[] { "sun" },
                duration,
                1.2f,
                new Vector3(0.2f, 0.3f, -1f));
        }
    }
}
