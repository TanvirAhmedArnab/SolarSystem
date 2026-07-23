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

        /// <summary>Gets planar movement intent.</summary>
        public Vector2 Move => move?.ReadValue<Vector2>() ?? Vector2.zero;

        /// <summary>Gets vertical movement intent.</summary>
        public float Elevate => elevate?.ReadValue<float>() ?? 0f;

        /// <summary>Gets pointer-look delta.</summary>
        public Vector2 LookDelta => look?.ReadValue<Vector2>() ?? Vector2.zero;

        /// <summary>Gets whether pointer look is currently active.</summary>
        public bool IsLookActive => lookModifier?.IsPressed() == true;

        /// <summary>Gets whether the temporary speed boost is active.</summary>
        public bool IsBoostActive => boost?.IsPressed() == true;

        /// <summary>Gets the current pointer position in screen coordinates.</summary>
        public Vector2 PointerPosition =>
            pointerPosition?.ReadValue<Vector2>() ?? Vector2.zero;

        /// <summary>Gets scroll-wheel focus zoom intent.</summary>
        public float Zoom => zoom?.ReadValue<float>() ?? 0f;

        /// <summary>Gets whether an action map has been resolved and enabled.</summary>
        public bool IsInitialized => explorer != null && explorer.enabled;

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
            explorer.Enable();
        }

        private void OnDestroy()
        {
            Release();
        }

        private void OnSelect(InputAction.CallbackContext context) => SelectPerformed?.Invoke();

        private void OnFocus(InputAction.CallbackContext context) => FocusPerformed?.Invoke();

        private void OnCancel(InputAction.CallbackContext context) => CancelPerformed?.Invoke();

        private void Release()
        {
            if (explorer == null)
            {
                return;
            }

            explorer.FindAction("Select", true).performed -= OnSelect;
            explorer.FindAction("Focus", true).performed -= OnFocus;
            explorer.FindAction("Cancel", true).performed -= OnCancel;
            explorer.Disable();
            explorer = null;
        }
    }
}
