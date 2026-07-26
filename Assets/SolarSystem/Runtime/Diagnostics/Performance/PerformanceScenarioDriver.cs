using System;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.UI;
using UnityEngine;

namespace Tanvir.SolarSystem.Diagnostics.Performance
{
    /// <summary>
    /// Resolves production services and drives each approved capture state.
    /// </summary>
    internal sealed class PerformanceScenarioDriver
    {
        private static readonly PerformanceScenarioDescriptor[] Catalog =
        {
            new PerformanceScenarioDescriptor(
                "PERF-01",
                "Readable overview",
                PerformanceScenarioKind.Overview,
                0),
            new PerformanceScenarioDescriptor(
                "PERF-02",
                "Earth close focus",
                PerformanceScenarioKind.EarthFocus,
                0),
            new PerformanceScenarioDescriptor(
                "PERF-03",
                "Credits and sources menu",
                PerformanceScenarioKind.CreditsMenu,
                0),
            new PerformanceScenarioDescriptor(
                "PERF-04",
                "Scale comparison - readable overview",
                PerformanceScenarioKind.ScaleComparison,
                1),
            new PerformanceScenarioDescriptor(
                "PERF-05",
                "Scale comparison - normalized orbits",
                PerformanceScenarioKind.ScaleComparison,
                2),
            new PerformanceScenarioDescriptor(
                "PERF-06",
                "Scale comparison - literal Earth reference",
                PerformanceScenarioKind.ScaleComparison,
                3),
            new PerformanceScenarioDescriptor(
                "PERF-07",
                "Cinematic chapter 1",
                PerformanceScenarioKind.CinematicChapter,
                1),
            new PerformanceScenarioDescriptor(
                "PERF-08",
                "Cinematic chapter 2",
                PerformanceScenarioKind.CinematicChapter,
                2),
            new PerformanceScenarioDescriptor(
                "PERF-09",
                "Cinematic chapter 3",
                PerformanceScenarioKind.CinematicChapter,
                3),
            new PerformanceScenarioDescriptor(
                "PERF-10",
                "Cinematic chapter 4",
                PerformanceScenarioKind.CinematicChapter,
                4),
            new PerformanceScenarioDescriptor(
                "PERF-11",
                "Cinematic chapter 5",
                PerformanceScenarioKind.CinematicChapter,
                5)
        };

        private SolarSystemInteractionCompositionRoot compositionRoot;
        private CelestialBodyView earth;

        public static int ScenarioCount => Catalog.Length;

        public int Count => ScenarioCount;

        public PerformanceScenarioDescriptor GetDescriptor(int index)
        {
            if (index < 0 || index >= Catalog.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Catalog[index];
        }

        public bool TryResolveDependencies(out string pendingTarget)
        {
            compositionRoot ??=
                UnityEngine.Object.FindAnyObjectByType<
                    SolarSystemInteractionCompositionRoot>();
            if (compositionRoot == null || !compositionRoot.IsInitialized)
            {
                pendingTarget = "production interaction composition root";
                return false;
            }

            if (earth == null)
            {
                CelestialBodyView[] views =
                    UnityEngine.Object.FindObjectsByType<CelestialBodyView>();
                for (int index = 0; index < views.Length; index++)
                {
                    if (string.Equals(
                        views[index].StableId,
                        "earth",
                        StringComparison.Ordinal))
                    {
                        earth = views[index];
                        break;
                    }
                }
            }

            if (earth == null)
            {
                pendingTarget = "Earth celestial view";
                return false;
            }

            pendingTarget = null;
            return true;
        }

        public bool TryPrepare(PerformanceScenarioDescriptor descriptor)
        {
            return descriptor.Kind switch
            {
                PerformanceScenarioKind.Overview => TryPrepareOverview(),
                PerformanceScenarioKind.EarthFocus => TryPrepareEarthFocus(),
                PerformanceScenarioKind.CreditsMenu => TryPrepareCredits(),
                PerformanceScenarioKind.ScaleComparison =>
                    TryPrepareScaleComparison(descriptor.Ordinal),
                PerformanceScenarioKind.CinematicChapter =>
                    TryPrepareCinematicChapter(descriptor.Ordinal),
                _ => throw new InvalidOperationException(
                    $"Unsupported scenario kind '{descriptor.Kind}'.")
            };
        }

        public void EnsureStable(PerformanceScenarioDescriptor descriptor)
        {
            bool stable = descriptor.Kind switch
            {
                PerformanceScenarioKind.Overview =>
                    compositionRoot.CameraController.Mode ==
                    SolarSystemCameraMode.FreeFlight &&
                    !compositionRoot.ExplorerMenu.Service.IsOpen,
                PerformanceScenarioKind.EarthFocus =>
                    compositionRoot.CameraController.Mode ==
                    SolarSystemCameraMode.Focused &&
                    compositionRoot.CameraController.FocusedTarget == earth,
                PerformanceScenarioKind.CreditsMenu =>
                    compositionRoot.ExplorerMenu.Service.IsOpen &&
                    compositionRoot.ExplorerMenu.Service.ActivePage ==
                    ExplorerMenuPage.CreditsAndSources,
                PerformanceScenarioKind.ScaleComparison =>
                    compositionRoot.ScaleComparison.CurrentStep ==
                    descriptor.Ordinal &&
                    compositionRoot.CameraController.Mode ==
                    SolarSystemCameraMode.GuidedComparison,
                PerformanceScenarioKind.CinematicChapter =>
                    compositionRoot.CinematicTour.CurrentChapterNumber ==
                    descriptor.Ordinal &&
                    compositionRoot.CameraController.Mode ==
                    SolarSystemCameraMode.CinematicTour,
                _ => false
            };

            if (!stable)
            {
                throw new InvalidOperationException(
                    $"Scenario '{descriptor.Id}' changed state while sampling.");
            }
        }

        private bool TryPrepareOverview()
        {
            compositionRoot.ExplorerMenu.Close();
            if (compositionRoot.CinematicTour.IsActive)
            {
                compositionRoot.CinematicTour.Cancel();
                return false;
            }

            if (compositionRoot.ScaleComparison.IsActive)
            {
                compositionRoot.ScaleComparison.Cancel();
                return false;
            }

            if (compositionRoot.CameraController.IsGuidedPresentationActive)
            {
                return false;
            }

            if (compositionRoot.CameraController.Mode !=
                SolarSystemCameraMode.FreeFlight)
            {
                compositionRoot.CameraController.ReturnToFreeFlight();
            }

            compositionRoot.SelectionController.ClearSelection();
            return compositionRoot.CameraController.Mode ==
                SolarSystemCameraMode.FreeFlight &&
                !compositionRoot.ExplorerMenu.Service.IsOpen;
        }

        private bool TryPrepareEarthFocus()
        {
            compositionRoot.ExplorerMenu.Close();
            if (compositionRoot.SelectionController.SelectedView != earth)
            {
                compositionRoot.SelectionController.Select(earth);
            }

            if (compositionRoot.CameraController.FocusedTarget != earth ||
                compositionRoot.CameraController.Mode ==
                SolarSystemCameraMode.FreeFlight)
            {
                compositionRoot.CameraController.Focus(earth);
            }

            return compositionRoot.CameraController.FocusedTarget == earth &&
                compositionRoot.CameraController.Mode ==
                SolarSystemCameraMode.Focused;
        }

        private bool TryPrepareCredits()
        {
            compositionRoot.ExplorerMenu.Open(
                ExplorerMenuPage.CreditsAndSources);
            return compositionRoot.ExplorerMenu.Service.IsOpen &&
                compositionRoot.ExplorerMenu.Service.ActivePage ==
                ExplorerMenuPage.CreditsAndSources;
        }

        private bool TryPrepareScaleComparison(int oneBasedStage)
        {
            compositionRoot.ExplorerMenu.Close();
            if (compositionRoot.CinematicTour.IsActive)
            {
                compositionRoot.CinematicTour.Cancel();
                return false;
            }

            if (compositionRoot.CameraController.IsCinematicTourActive)
            {
                return false;
            }

            if (!compositionRoot.ScaleComparison.IsActive)
            {
                if (compositionRoot.CameraController.Mode !=
                    SolarSystemCameraMode.FreeFlight)
                {
                    compositionRoot.CameraController.ReturnToFreeFlight();
                }

                compositionRoot.ScaleComparison.Advance();
                return false;
            }

            while (compositionRoot.ScaleComparison.CurrentStep < oneBasedStage)
            {
                compositionRoot.ScaleComparison.Advance();
                return false;
            }

            if (compositionRoot.ScaleComparison.CurrentStep > oneBasedStage)
            {
                throw new InvalidOperationException(
                    "Scale-comparison scenario advanced beyond its " +
                    "requested stage.");
            }

            return compositionRoot.CameraController.Mode ==
                SolarSystemCameraMode.GuidedComparison;
        }

        private bool TryPrepareCinematicChapter(int oneBasedChapter)
        {
            compositionRoot.ExplorerMenu.Close();
            if (compositionRoot.ScaleComparison.IsActive)
            {
                compositionRoot.ScaleComparison.Cancel();
                return false;
            }

            if (compositionRoot.CameraController.IsGuidedPresentationActive &&
                !compositionRoot.CameraController.IsCinematicTourActive)
            {
                return false;
            }

            if (!compositionRoot.CinematicTour.IsActive)
            {
                if (!compositionRoot.CinematicTour.Start())
                {
                    return false;
                }

                return false;
            }

            while (compositionRoot.CinematicTour.CurrentChapterNumber <
                oneBasedChapter)
            {
                compositionRoot.CinematicTour.Advance();
                return false;
            }

            if (compositionRoot.CinematicTour.CurrentChapterNumber >
                oneBasedChapter)
            {
                throw new InvalidOperationException(
                    "Cinematic scenario advanced beyond its requested " +
                    "chapter.");
            }

            return compositionRoot.CameraController.Mode ==
                SolarSystemCameraMode.CinematicTour;
        }
    }
}
