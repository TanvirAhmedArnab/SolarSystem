using System;
using System.Collections.Generic;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Runs deterministic unscaled chapter timing without Unity scene state.</summary>
    public sealed class CinematicTourService
    {
        private readonly CinematicTourChapter[] chapters;
        private readonly GuidedPresentationCoordinator coordinator;

        public CinematicTourService(
            IReadOnlyList<CinematicTourChapter> authoredChapters,
            GuidedPresentationCoordinator presentationCoordinator)
        {
            if (authoredChapters == null || authoredChapters.Count == 0)
            {
                throw new ArgumentException(
                    "The cinematic tour requires at least one chapter.",
                    nameof(authoredChapters));
            }

            chapters = new CinematicTourChapter[authoredChapters.Count];
            for (int index = 0; index < authoredChapters.Count; index++)
            {
                chapters[index] = authoredChapters[index] ??
                    throw new ArgumentException(
                        "Tour chapters cannot contain null entries.",
                        nameof(authoredChapters));
            }

            coordinator = presentationCoordinator ??
                throw new ArgumentNullException(nameof(presentationCoordinator));
            CurrentChapterIndex = -1;
        }

        public event Action Changed;
        public bool IsActive { get; private set; }
        public int ChapterCount => chapters.Length;
        public int CurrentChapterIndex { get; private set; }
        public int CurrentChapterNumber => IsActive ? CurrentChapterIndex + 1 : 0;
        public float ElapsedSeconds { get; private set; }
        public CinematicTourChapter CurrentChapter =>
            IsActive ? chapters[CurrentChapterIndex] : null;
        public float NormalizedChapterProgress =>
            IsActive ? Math.Clamp(
                ElapsedSeconds / CurrentChapter.DurationSeconds,
                0f,
                1f) : 0f;

        public bool Start()
        {
            if (IsActive ||
                !coordinator.TryAcquire(GuidedPresentationOwner.CinematicTour))
            {
                return false;
            }

            IsActive = true;
            CurrentChapterIndex = 0;
            ElapsedSeconds = 0f;
            Changed?.Invoke();
            return true;
        }

        public void Tick(float unscaledDeltaTime)
        {
            if (!IsActive || unscaledDeltaTime <= 0f)
            {
                return;
            }

            if (!float.IsFinite(unscaledDeltaTime))
            {
                throw new ArgumentOutOfRangeException(nameof(unscaledDeltaTime));
            }

            float remaining = unscaledDeltaTime;
            while (IsActive && remaining > 0f)
            {
                float chapterRemaining =
                    CurrentChapter.DurationSeconds - ElapsedSeconds;
                if (remaining < chapterRemaining)
                {
                    ElapsedSeconds += remaining;
                    remaining = 0f;
                    continue;
                }

                remaining -= chapterRemaining;
                Advance();
            }
        }

        public bool Advance()
        {
            if (!IsActive)
            {
                return false;
            }

            if (CurrentChapterIndex + 1 >= chapters.Length)
            {
                Exit();
                return true;
            }

            CurrentChapterIndex++;
            ElapsedSeconds = 0f;
            Changed?.Invoke();
            return true;
        }

        public bool Cancel()
        {
            if (!IsActive)
            {
                return false;
            }

            Exit();
            return true;
        }

        private void Exit()
        {
            IsActive = false;
            CurrentChapterIndex = -1;
            ElapsedSeconds = 0f;
            coordinator.Release(GuidedPresentationOwner.CinematicTour);
            Changed?.Invoke();
        }
    }
}
