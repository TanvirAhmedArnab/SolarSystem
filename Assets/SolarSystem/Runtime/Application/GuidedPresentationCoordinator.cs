using System;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Identifies the feature that currently owns guided presentation.</summary>
    public enum GuidedPresentationOwner
    {
        None,
        ScaleComparison,
        CinematicTour
    }

    /// <summary>
    /// Arbitrates exclusive ownership of the shared camera and guided UI surface.
    /// </summary>
    public sealed class GuidedPresentationCoordinator
    {
        /// <summary>Raised after guided ownership changes.</summary>
        public event Action Changed;

        /// <summary>Gets the current owner.</summary>
        public GuidedPresentationOwner Owner { get; private set; }

        /// <summary>Gets whether any guided presentation is active.</summary>
        public bool IsActive => Owner != GuidedPresentationOwner.None;

        /// <summary>Attempts to acquire guided presentation for one feature.</summary>
        public bool TryAcquire(GuidedPresentationOwner requestedOwner)
        {
            if (requestedOwner == GuidedPresentationOwner.None)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedOwner));
            }

            if (Owner == requestedOwner)
            {
                return true;
            }

            if (IsActive)
            {
                return false;
            }

            Owner = requestedOwner;
            Changed?.Invoke();
            return true;
        }

        /// <summary>Releases guided presentation only when called by its owner.</summary>
        public bool Release(GuidedPresentationOwner releasingOwner)
        {
            if (Owner != releasingOwner)
            {
                return false;
            }

            Owner = GuidedPresentationOwner.None;
            Changed?.Invoke();
            return true;
        }
    }
}
