using Tanvir.SolarSystem.Presentation.Scale;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Applies one reviewed presentation scale without changing physical state.</summary>
    public interface IScaleModeController
    {
        /// <summary>Gets the active presentation scale.</summary>
        CelestialScaleMode ScaleMode { get; }

        /// <summary>Applies a presentation scale and rerenders the current physical state.</summary>
        void SetScaleMode(CelestialScaleMode mode);
    }
}
