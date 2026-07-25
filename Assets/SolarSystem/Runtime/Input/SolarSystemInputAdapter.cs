using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanvir.SolarSystem.Input
{
    /// <summary>Translates the project-owned Input System map into interaction intent.</summary>
    [DisallowMultipleComponent]
    public sealed class SolarSystemInputAdapter : MonoBehaviour
    {
        private InputActionMap explorer;
        private InputAction move;
        private InputAction elevate;
        private InputAction look;
        private InputAction lookModifier;
        private InputAction boost;
        private InputAction pointerPosition;
        private InputAction zoom;

        /// <summary>Raised when the user requests selection at the pointer.</summary>
        public event Action SelectPerformed;

        /// <summary>Raised when the user requests focus on the selected body.</summary>
        public event Action FocusPerformed;

        /// <summary>Raised when the user requests a return to free flight.</summary>
        public event Action CancelPerformed;

        /// <summary>Raised when the user requests pause or resume.</summary>
        public event Action TogglePausePerformed;

        /// <summary>Raised when the user requests the next slower simulation rate.</summary>
        public event Action DecreaseTimeSpeedPerformed;

        /// <summary>Raised when the user requests the next faster simulation rate.</summary>
        public event Action IncreaseTimeSpeedPerformed;

        /// <summary>Raised when the user advances the guided scale comparison.</summary>
        public event Action ScaleComparisonPerformed;

        /// <summary>Raised when the user starts or advances the cinematic tour.</summary>
        public event Action CinematicTourPerformed;

        /// <summary>Raised when the user toggles reduced presentation motion.</summary>
        public event Action ToggleReducedMotionPerformed;

        /// <summary>Raised when the user opens or closes the celestial navigator.</summary>
        public event Action ToggleNavigatorPerformed;

        /// <summary>Raised when the user toggles projected celestial labels.</summary>
        public event Action ToggleWorldLabelsPerformed;

        /// <summary>Raised when the user opens or closes contextual Help.</summary>
        public event Action ToggleHelpPerformed;

        /// <summary>Raised when the user toggles overview orbit guides.</summary>
        public event Action ToggleOrbitGuidesPerformed;

        /// <summary>Gets planar movement intent.</summary>
        public Vector2 Move => IsExplorerInteractionEnabled
            ? move?.ReadValue<Vector2>() ?? Vector2.zero
            : Vector2.zero;

        /// <summary>Gets vertical movement intent.</summary>
        public float Elevate => IsExplorerInteractionEnabled
            ? elevate?.ReadValue<float>() ?? 0f
            : 0f;

        /// <summary>Gets pointer-look delta.</summary>
        public Vector2 LookDelta => IsExplorerInteractionEnabled
            ? look?.ReadValue<Vector2>() ?? Vector2.zero
            : Vector2.zero;

        /// <summary>Gets whether pointer look is currently active.</summary>
        public bool IsLookActive =>
            IsExplorerInteractionEnabled && lookModifier?.IsPressed() == true;

        /// <summary>Gets whether the temporary speed boost is active.</summary>
        public bool IsBoostActive =>
            IsExplorerInteractionEnabled && boost?.IsPressed() == true;

        /// <summary>Gets the current pointer position in screen coordinates.</summary>
        public Vector2 PointerPosition =>
            pointerPosition?.ReadValue<Vector2>() ?? Vector2.zero;

        /// <summary>Gets scroll-wheel focus zoom intent.</summary>
        public float Zoom => IsExplorerInteractionEnabled
            ? zoom?.ReadValue<float>() ?? 0f
            : 0f;

        /// <summary>Gets whether an action map has been resolved and enabled.</summary>
        public bool IsInitialized => explorer != null && explorer.enabled;

        /// <summary>Gets whether non-modal exploration commands are accepted.</summary>
        public bool IsExplorerInteractionEnabled { get; private set; } = true;

        /// <summary>Resolves, subscribes, and enables the project interaction map.</summary>
        public void Initialize(InputActionAsset actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            Release();
            explorer = actions.FindActionMap("Explorer", true);
            move = explorer.FindAction("Move", true);
            elevate = explorer.FindAction("Elevate", true);
            look = explorer.FindAction("Look", true);
            lookModifier = explorer.FindAction("LookModifier", true);
            boost = explorer.FindAction("Boost", true);
            pointerPosition = explorer.FindAction("PointerPosition", true);
            zoom = explorer.FindAction("Zoom", true);
            explorer.FindAction("Select", true).performed += OnSelect;
            explorer.FindAction("Focus", true).performed += OnFocus;
            explorer.FindAction("Cancel", true).performed += OnCancel;
            explorer.FindAction("TogglePause", true).performed += OnTogglePause;
            explorer.FindAction("DecreaseTimeSpeed", true).performed += OnDecreaseTimeSpeed;
            explorer.FindAction("IncreaseTimeSpeed", true).performed += OnIncreaseTimeSpeed;
            explorer.FindAction("ScaleComparison", true).performed += OnScaleComparison;
            explorer.FindAction("CinematicTour", true).performed += OnCinematicTour;
            explorer.FindAction("ToggleReducedMotion", true).performed +=
                OnToggleReducedMotion;
            explorer.FindAction("ToggleNavigator", true).performed += OnToggleNavigator;
            explorer.FindAction("ToggleWorldLabels", true).performed += OnToggleWorldLabels;
            explorer.FindAction("ToggleHelp", true).performed += OnToggleHelp;
            explorer.FindAction("ToggleOrbitGuides", true).performed +=
                OnToggleOrbitGuides;
            IsExplorerInteractionEnabled = true;
            explorer.Enable();
        }

        /// <summary>
        /// Enables or suppresses every exploration command except Help and Escape.
        /// </summary>
        public void SetExplorerInteractionEnabled(bool enabled)
        {
            IsExplorerInteractionEnabled = enabled;
        }

        private void OnDestroy()
        {
            Release();
        }

        private void OnSelect(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(SelectPerformed);

        private void OnFocus(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(FocusPerformed);

        private void OnCancel(InputAction.CallbackContext context) => CancelPerformed?.Invoke();

        private void OnTogglePause(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(TogglePausePerformed);

        private void OnDecreaseTimeSpeed(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(DecreaseTimeSpeedPerformed);

        private void OnIncreaseTimeSpeed(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(IncreaseTimeSpeedPerformed);

        private void OnScaleComparison(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(ScaleComparisonPerformed);

        private void OnCinematicTour(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(CinematicTourPerformed);

        private void OnToggleReducedMotion(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(ToggleReducedMotionPerformed);

        private void OnToggleNavigator(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(ToggleNavigatorPerformed);

        private void OnToggleWorldLabels(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(ToggleWorldLabelsPerformed);

        private void OnToggleHelp(InputAction.CallbackContext context) =>
            ToggleHelpPerformed?.Invoke();

        private void OnToggleOrbitGuides(InputAction.CallbackContext context) =>
            InvokeWhenExplorerEnabled(ToggleOrbitGuidesPerformed);

        private void InvokeWhenExplorerEnabled(Action intent)
        {
            if (IsExplorerInteractionEnabled)
            {
                intent?.Invoke();
            }
        }

        private void Release()
        {
            if (explorer == null)
            {
                return;
            }

            explorer.FindAction("Select", true).performed -= OnSelect;
            explorer.FindAction("Focus", true).performed -= OnFocus;
            explorer.FindAction("Cancel", true).performed -= OnCancel;
            explorer.FindAction("TogglePause", true).performed -= OnTogglePause;
            explorer.FindAction("DecreaseTimeSpeed", true).performed -= OnDecreaseTimeSpeed;
            explorer.FindAction("IncreaseTimeSpeed", true).performed -= OnIncreaseTimeSpeed;
            explorer.FindAction("ScaleComparison", true).performed -= OnScaleComparison;
            explorer.FindAction("CinematicTour", true).performed -= OnCinematicTour;
            explorer.FindAction("ToggleReducedMotion", true).performed -=
                OnToggleReducedMotion;
            explorer.FindAction("ToggleNavigator", true).performed -= OnToggleNavigator;
            explorer.FindAction("ToggleWorldLabels", true).performed -= OnToggleWorldLabels;
            explorer.FindAction("ToggleHelp", true).performed -= OnToggleHelp;
            explorer.FindAction("ToggleOrbitGuides", true).performed -=
                OnToggleOrbitGuides;
            explorer.Disable();
            explorer = null;
            IsExplorerInteractionEnabled = true;
        }
    }
}
