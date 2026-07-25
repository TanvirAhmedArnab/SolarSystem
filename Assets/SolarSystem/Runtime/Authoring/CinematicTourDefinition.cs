using System;
using Tanvir.SolarSystem.Application;
using UnityEngine;

namespace Tanvir.SolarSystem.Authoring
{
    /// <summary>Project-authored deterministic cinematic tour content.</summary>
    [CreateAssetMenu(
        fileName = "Tour_Cinematic",
        menuName = "Solar System/Presentation/Cinematic Tour")]
    public sealed class CinematicTourDefinition : ScriptableObject
    {
        [Serializable]
        private sealed class ChapterDefinition
        {
            [SerializeField] private string stableId;
            [SerializeField] private string title;
            [SerializeField] private string subtitle;
            [SerializeField, TextArea(2, 4)] private string description;
            [SerializeField] private string[] targetIds = Array.Empty<string>();
            [SerializeField, Min(1f)] private float durationSeconds = 10f;
            [SerializeField, Min(1f)] private float framingPadding = 1.2f;
            [SerializeField] private Vector3 framingDirection =
                new Vector3(0.18f, 0.34f, -1f);

            internal CinematicTourChapter ToRuntime()
            {
                return new CinematicTourChapter(
                    stableId,
                    title,
                    subtitle,
                    description,
                    targetIds,
                    durationSeconds,
                    framingPadding,
                    framingDirection);
            }
        }

        [SerializeField] private ChapterDefinition[] chapters =
            Array.Empty<ChapterDefinition>();

        /// <summary>Creates a validated immutable runtime copy.</summary>
        public CinematicTourChapter[] CreateRuntimeChapters()
        {
            if (chapters == null || chapters.Length == 0)
            {
                throw new InvalidOperationException(
                    "Cinematic tour definition has no authored chapters.");
            }

            var runtime = new CinematicTourChapter[chapters.Length];
            for (int index = 0; index < chapters.Length; index++)
            {
                runtime[index] = chapters[index]?.ToRuntime() ??
                    throw new InvalidOperationException(
                        $"Cinematic tour chapter {index} is null.");
            }

            return runtime;
        }
    }
}
