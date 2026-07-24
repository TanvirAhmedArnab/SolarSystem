using System;
using Tanvir.SolarSystem.Input;
using UnityEngine;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Routes discrete input intent into simulation-time application commands.</summary>
    [DisallowMultipleComponent]
    public sealed class SimulationTimeInputController : MonoBehaviour
    {
        private SolarSystemInputAdapter input;

        /// <summary>Gets the active time-control service.</summary>
        public SimulationTimeControlService Service { get; private set; }

        /// <summary>Gets whether input and application dependencies are assigned.</summary>
        public bool IsInitialized => input != null && Service != null;

        /// <summary>Gets whether user time commands are currently accepted.</summary>
        public bool IsInteractionEnabled { get; private set; } = true;

        /// <summary>Initializes command routing and replaces any previous subscriptions.</summary>
        public void Initialize(
            SolarSystemInputAdapter inputAdapter,
            SimulationTimeControlService service)
        {
            Release();
            input = inputAdapter != null
                ? inputAdapter
                : throw new ArgumentNullException(nameof(inputAdapter));
            Service = service ?? throw new ArgumentNullException(nameof(service));
            IsInteractionEnabled = true;
            input.TogglePausePerformed += OnTogglePaused;
            input.DecreaseTimeSpeedPerformed += OnDecreaseSpeed;
            input.IncreaseTimeSpeedPerformed += OnIncreaseSpeed;
        }

        private void OnDestroy()
        {
            Release();
        }

        private void OnDecreaseSpeed()
        {
            if (IsInteractionEnabled)
            {
                Service.DecreaseSpeed();
            }
        }

        private void OnIncreaseSpeed()
        {
            if (IsInteractionEnabled)
            {
                Service.IncreaseSpeed();
            }
        }

        private void OnTogglePaused()
        {
            if (IsInteractionEnabled)
            {
                Service.TogglePaused();
            }
        }

        /// <summary>Enables or suppresses user time commands without changing time state.</summary>
        public void SetInteractionEnabled(bool enabled)
        {
            IsInteractionEnabled = enabled;
        }

        private void Release()
        {
            if (input != null && Service != null)
            {
                input.TogglePausePerformed -= OnTogglePaused;
                input.DecreaseTimeSpeedPerformed -= OnDecreaseSpeed;
                input.IncreaseTimeSpeedPerformed -= OnIncreaseSpeed;
            }

            input = null;
            Service = null;
            IsInteractionEnabled = true;
        }
    }
}
