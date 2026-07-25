using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Presentation.Camera;
using UnityEngine;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Defines the authored basis used to resolve a tour viewing direction.</summary>
    public enum CinematicTourFramingSpace
    {
        World = 0,
        SolarRadial = 1,
        TargetAxis = 2,
        SunlitTargetAxis = 3
    }

    /// <summary>Immutable authored content and framing data for one tour chapter.</summary>
    public sealed class CinematicTourChapter
    {
        private readonly string[] targetIds;

        /// <summary>Creates one validated deterministic chapter.</summary>
        public CinematicTourChapter(
            string stableId,
            string title,
            string subtitle,
            string description,
            IReadOnlyList<string> targets,
            float durationSeconds,
            float framingPadding,
            Vector3 framingDirection)
            : this(
                stableId,
                title,
                subtitle,
                description,
                targets,
                durationSeconds,
                framingPadding,
                framingDirection,
                CinematicTourFramingSpace.World,
                Vector2.zero,
                GuidedCameraTransition.Default.DurationSeconds,
                GuidedCameraEasing.SmoothStep)
        {
        }

        /// <summary>
        /// Creates one validated deterministic chapter with authored shot composition.
        /// </summary>
        public CinematicTourChapter(
            string stableId,
            string title,
            string subtitle,
            string description,
            IReadOnlyList<string> targets,
            float durationSeconds,
            float framingPadding,
            Vector3 framingDirection,
            CinematicTourFramingSpace framingSpace,
            Vector2 screenOffset,
            float transitionDurationSeconds,
            GuidedCameraEasing transitionEasing)
        {
            StableId = RequireText(stableId, nameof(stableId));
            Title = RequireText(title, nameof(title));
            Subtitle = RequireText(subtitle, nameof(subtitle));
            Description = RequireText(description, nameof(description));
            if (targets == null || targets.Count == 0)
            {
                throw new ArgumentException(
                    "A cinematic chapter requires at least one target.",
                    nameof(targets));
            }

            targetIds = new string[targets.Count];
            for (int index = 0; index < targets.Count; index++)
            {
                targetIds[index] = RequireText(targets[index], nameof(targets));
                for (int prior = 0; prior < index; prior++)
                {
                    if (string.Equals(
                        targetIds[prior],
                        targetIds[index],
                        StringComparison.Ordinal))
                    {
                        throw new ArgumentException(
                            $"Duplicate target ID '{targetIds[index]}'.",
                            nameof(targets));
                    }
                }
            }

            if (!float.IsFinite(durationSeconds) || durationSeconds < 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            }

            if (!float.IsFinite(framingPadding) || framingPadding < 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(framingPadding));
            }

            if (!float.IsFinite(framingDirection.x) ||
                !float.IsFinite(framingDirection.y) ||
                !float.IsFinite(framingDirection.z) ||
                framingDirection.sqrMagnitude < 0.001f)
            {
                throw new ArgumentOutOfRangeException(nameof(framingDirection));
            }

            if (framingSpace != CinematicTourFramingSpace.World &&
                framingSpace != CinematicTourFramingSpace.SolarRadial &&
                framingSpace != CinematicTourFramingSpace.TargetAxis &&
                framingSpace != CinematicTourFramingSpace.SunlitTargetAxis)
            {
                throw new ArgumentOutOfRangeException(nameof(framingSpace));
            }

            if (!float.IsFinite(screenOffset.x) ||
                !float.IsFinite(screenOffset.y) ||
                Mathf.Abs(screenOffset.x) > 0.75f ||
                Mathf.Abs(screenOffset.y) > 0.75f)
            {
                throw new ArgumentOutOfRangeException(nameof(screenOffset));
            }

            var transition = new GuidedCameraTransition(
                transitionDurationSeconds,
                transitionEasing);
            DurationSeconds = durationSeconds;
            FramingPadding = framingPadding;
            FramingDirection = framingDirection.normalized;
            FramingSpace = framingSpace;
            ScreenOffset = screenOffset;
            TransitionDurationSeconds = transition.DurationSeconds;
            TransitionEasing = transition.Easing;
        }

        public string StableId { get; }
        public string Title { get; }
        public string Subtitle { get; }
        public string Description { get; }
        public IReadOnlyList<string> TargetIds => targetIds;
        public float DurationSeconds { get; }
        public float FramingPadding { get; }
        public Vector3 FramingDirection { get; }
        public CinematicTourFramingSpace FramingSpace { get; }
        public Vector2 ScreenOffset { get; }
        public float TransitionDurationSeconds { get; }
        public GuidedCameraEasing TransitionEasing { get; }

        private static string RequireText(string value, string parameter)
        {
            return !string.IsNullOrWhiteSpace(value)
                ? value.Trim()
                : throw new ArgumentException("Value cannot be empty.", parameter);
        }
    }
}
