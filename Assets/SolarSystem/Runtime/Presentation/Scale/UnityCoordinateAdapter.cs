using System;
using Tanvir.SolarSystem.Mathematics;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>Maps the right-handed Core XY plane and +Z normal into Unity's XZ plane and +Y normal.</summary>
    public static class UnityCoordinateAdapter
    {
        /// <summary>Converts one finite Core vector into Unity coordinates exactly once at the Runtime boundary.</summary>
        public static Vector3 ToUnityVector(Double3 coreVector)
        {
            if (!coreVector.IsFinite)
            {
                throw new ArgumentOutOfRangeException(nameof(coreVector), "Core vector must be finite.");
            }

            return new Vector3(
                ToFiniteFloat(coreVector.X),
                ToFiniteFloat(coreVector.Z),
                ToFiniteFloat(coreVector.Y));
        }

        private static float ToFiniteFloat(double value)
        {
            if (value > float.MaxValue || value < -float.MaxValue)
            {
                throw new OverflowException("Projected coordinate exceeded Unity's finite float range.");
            }

            return (float)value;
        }
    }
}
