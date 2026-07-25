using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>
    /// Applies a reversible renderer-only spotlight to authored tour targets.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CinematicTourBodyVisibilityController : MonoBehaviour
    {
        private sealed class BodyRenderers
        {
            internal BodyRenderers(CelestialBodyView view)
            {
                StableId = view.StableId;
                Renderers = view.GetComponentsInChildren<Renderer>(true);
                SavedEnabledStates = new bool[Renderers.Length];
            }

            internal string StableId { get; }
            internal Renderer[] Renderers { get; }
            internal bool[] SavedEnabledStates { get; }
        }

        private BodyRenderers[] bodies = Array.Empty<BodyRenderers>();

        /// <summary>Gets whether body renderer references have been cached.</summary>
        public bool IsInitialized => bodies.Length > 0;

        /// <summary>Gets whether a tour currently owns the renderer spotlight.</summary>
        public bool IsTourOverrideActive { get; private set; }

        /// <summary>Caches renderer arrays once for allocation-stable chapter changes.</summary>
        public void Initialize(CelestialBodyView[] bodyViews)
        {
            EndTour();
            if (bodyViews == null || bodyViews.Length == 0)
            {
                throw new ArgumentException(
                    "Tour body visibility requires celestial views.",
                    nameof(bodyViews));
            }

            bodies = new BodyRenderers[bodyViews.Length];
            for (int index = 0; index < bodyViews.Length; index++)
            {
                CelestialBodyView view = bodyViews[index];
                if (view == null)
                {
                    throw new ArgumentException(
                        "Tour body visibility cannot cache a null view.",
                        nameof(bodyViews));
                }

                bodies[index] = new BodyRenderers(view);
            }
        }

        /// <summary>Captures every renderer's exact enabled state at tour entry.</summary>
        public void BeginTour()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    "Tour body visibility is not initialized.");
            }

            if (IsTourOverrideActive)
            {
                return;
            }

            for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
            {
                BodyRenderers body = bodies[bodyIndex];
                for (int rendererIndex = 0;
                     rendererIndex < body.Renderers.Length;
                     rendererIndex++)
                {
                    Renderer renderer = body.Renderers[rendererIndex];
                    body.SavedEnabledStates[rendererIndex] =
                        renderer != null && renderer.enabled;
                }
            }

            IsTourOverrideActive = true;
        }

        /// <summary>Shows only renderers belonging to the authored chapter targets.</summary>
        public void ShowOnly(IReadOnlyList<string> targetIds)
        {
            if (!IsTourOverrideActive)
            {
                throw new InvalidOperationException(
                    "Tour body visibility must begin before applying targets.");
            }

            if (targetIds == null || targetIds.Count == 0)
            {
                throw new ArgumentException(
                    "A renderer spotlight requires at least one target.",
                    nameof(targetIds));
            }

            for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
            {
                BodyRenderers body = bodies[bodyIndex];
                bool isTarget = Contains(targetIds, body.StableId);
                for (int rendererIndex = 0;
                     rendererIndex < body.Renderers.Length;
                     rendererIndex++)
                {
                    Renderer renderer = body.Renderers[rendererIndex];
                    if (renderer != null)
                    {
                        renderer.enabled =
                            isTarget && body.SavedEnabledStates[rendererIndex];
                    }
                }
            }
        }

        /// <summary>Restores every renderer to its exact state captured at entry.</summary>
        public void EndTour()
        {
            if (!IsTourOverrideActive)
            {
                return;
            }

            for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
            {
                BodyRenderers body = bodies[bodyIndex];
                for (int rendererIndex = 0;
                     rendererIndex < body.Renderers.Length;
                     rendererIndex++)
                {
                    Renderer renderer = body.Renderers[rendererIndex];
                    if (renderer != null)
                    {
                        renderer.enabled = body.SavedEnabledStates[rendererIndex];
                    }
                }
            }

            IsTourOverrideActive = false;
        }

        /// <summary>Gets whether any renderer for one body is currently enabled.</summary>
        public bool IsBodyVisible(string stableId)
        {
            for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
            {
                BodyRenderers body = bodies[bodyIndex];
                if (!string.Equals(
                    body.StableId,
                    stableId,
                    StringComparison.Ordinal))
                {
                    continue;
                }

                for (int rendererIndex = 0;
                     rendererIndex < body.Renderers.Length;
                     rendererIndex++)
                {
                    if (body.Renderers[rendererIndex]?.enabled == true)
                    {
                        return true;
                    }
                }

                return false;
            }

            throw new ArgumentException(
                $"Unknown tour body '{stableId}'.",
                nameof(stableId));
        }

        private void OnDestroy()
        {
            EndTour();
        }

        private static bool Contains(
            IReadOnlyList<string> targetIds,
            string stableId)
        {
            for (int index = 0; index < targetIds.Count; index++)
            {
                if (string.Equals(
                    targetIds[index],
                    stableId,
                    StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
