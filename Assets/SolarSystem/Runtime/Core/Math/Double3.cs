using System;

namespace Tanvir.SolarSystem.Mathematics
{
    /// <summary>Represents a double-precision three-dimensional vector in the simulation domain.</summary>
    public readonly struct Double3 : IEquatable<Double3>
    {
        /// <summary>A vector whose components are all zero.</summary>
        public static readonly Double3 Zero = new Double3(0d, 0d, 0d);

        /// <summary>Initializes a new vector from its Cartesian components.</summary>
        public Double3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>Gets the X component.</summary>
        public double X { get; }

        /// <summary>Gets the Y component.</summary>
        public double Y { get; }

        /// <summary>Gets the Z component.</summary>
        public double Z { get; }

        /// <summary>Gets the squared vector magnitude.</summary>
        public double SqrMagnitude => (X * X) + (Y * Y) + (Z * Z);

        /// <summary>Gets the vector magnitude.</summary>
        public double Magnitude => Math.Sqrt(SqrMagnitude);

        /// <summary>Gets whether every component is finite.</summary>
        public bool IsFinite => IsFiniteValue(X) && IsFiniteValue(Y) && IsFiniteValue(Z);

        /// <inheritdoc />
        public bool Equals(Double3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Double3 other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + X.GetHashCode();
                hash = (hash * 31) + Y.GetHashCode();
                hash = (hash * 31) + Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>Adds two vectors.</summary>
        public static Double3 operator +(Double3 left, Double3 right) =>
            new Double3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        /// <summary>Subtracts one vector from another.</summary>
        public static Double3 operator -(Double3 left, Double3 right) =>
            new Double3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        /// <summary>Scales a vector.</summary>
        public static Double3 operator *(Double3 value, double scalar) =>
            new Double3(value.X * scalar, value.Y * scalar, value.Z * scalar);

        /// <summary>Returns whether two vectors are exactly equal.</summary>
        public static bool operator ==(Double3 left, Double3 right) => left.Equals(right);

        /// <summary>Returns whether two vectors are not exactly equal.</summary>
        public static bool operator !=(Double3 left, Double3 right) => !left.Equals(right);

        /// <inheritdoc />
        public override string ToString() => $"({X:R}, {Y:R}, {Z:R})";

        private static bool IsFiniteValue(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
