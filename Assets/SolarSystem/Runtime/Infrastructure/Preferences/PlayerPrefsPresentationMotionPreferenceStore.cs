using Tanvir.SolarSystem.Application;
using UnityEngine;

namespace Tanvir.SolarSystem.Infrastructure.Preferences
{
    /// <summary>Persists presentation-motion accessibility through PlayerPrefs.</summary>
    public sealed class PlayerPrefsPresentationMotionPreferenceStore :
        IPresentationMotionPreferenceStore
    {
        /// <summary>Stable project-owned persistence key.</summary>
        public const string PreferenceKey =
            "Tanvir.SolarSystem.PresentationMotion.v1";

        /// <inheritdoc />
        public bool TryLoad(out PresentationMotionMode mode)
        {
            if (!PlayerPrefs.HasKey(PreferenceKey))
            {
                mode = default;
                return false;
            }

            int saved = PlayerPrefs.GetInt(PreferenceKey);
            if (saved < (int)PresentationMotionMode.FullMotion ||
                saved > (int)PresentationMotionMode.ReducedMotion)
            {
                mode = default;
                return false;
            }

            mode = (PresentationMotionMode)saved;
            return true;
        }

        /// <inheritdoc />
        public void Save(PresentationMotionMode mode)
        {
            PlayerPrefs.SetInt(PreferenceKey, (int)mode);
            PlayerPrefs.Save();
        }
    }
}
