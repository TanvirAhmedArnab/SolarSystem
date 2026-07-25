using System;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Presentation.Camera;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>
    /// Routes Help and contextual Escape intent through one ordered modal boundary.
    /// </summary>
    public sealed class ExplorerMenuController : IDisposable
    {
        private readonly SolarSystemInputAdapter input;
        private readonly ExplorerSettingsController settings;
        private readonly SolarSystemCameraController cameraController;
        private readonly GuidedScaleComparisonService scaleComparison;
        private readonly CinematicTourController tourController;
        private bool isDisposed;

        public ExplorerMenuController(
            SolarSystemInputAdapter inputAdapter,
            ExplorerSettingsController settingsController,
            SolarSystemCameraController explorerCameraController,
            GuidedScaleComparisonService guidedScaleComparison,
            CinematicTourController cinematicTourController)
        {
            input = inputAdapter ?? throw new ArgumentNullException(nameof(inputAdapter));
            settings = settingsController ??
                throw new ArgumentNullException(nameof(settingsController));
            cameraController = explorerCameraController ??
                throw new ArgumentNullException(nameof(explorerCameraController));
            scaleComparison = guidedScaleComparison ??
                throw new ArgumentNullException(nameof(guidedScaleComparison));
            tourController = cinematicTourController ??
                throw new ArgumentNullException(nameof(cinematicTourController));

            Service = new ExplorerMenuService();
            Service.Changed += OnMenuChanged;
            input.ToggleHelpPerformed += ToggleHelp;
            input.CancelPerformed += HandleCancel;

            if (!settings.Service.Current.HasCompletedOnboarding)
            {
                Service.Open(ExplorerMenuPage.Help);
            }
        }

        public ExplorerMenuService Service { get; }

        public void Open(ExplorerMenuPage page) => Service.Open(page);
        public void SetPage(ExplorerMenuPage page) => Service.SetPage(page);

        public void ToggleHelp()
        {
            if (Service.IsOpen && Service.ActivePage == ExplorerMenuPage.Help)
            {
                Close();
            }
            else
            {
                Service.Open(ExplorerMenuPage.Help);
            }
        }

        public void Close()
        {
            if (Service.Close())
            {
                settings.CompleteOnboarding();
            }
        }

        /// <summary>Applies the approved modal, guided, focus, then menu priority.</summary>
        public void HandleCancel()
        {
            if (Service.IsOpen)
            {
                Close();
                return;
            }

            if (tourController.Service?.IsActive == true)
            {
                tourController.Cancel();
                return;
            }

            if (scaleComparison.IsActive)
            {
                scaleComparison.Cancel();
                return;
            }

            if (cameraController.Mode != SolarSystemCameraMode.FreeFlight)
            {
                cameraController.ReturnToFreeFlight();
                return;
            }

            Service.Open(ExplorerMenuPage.Help);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            Service.Changed -= OnMenuChanged;
            input.ToggleHelpPerformed -= ToggleHelp;
            input.CancelPerformed -= HandleCancel;
            input.SetExplorerInteractionEnabled(true);
            isDisposed = true;
        }

        private void OnMenuChanged()
        {
            input.SetExplorerInteractionEnabled(!Service.IsOpen);
        }
    }
}
