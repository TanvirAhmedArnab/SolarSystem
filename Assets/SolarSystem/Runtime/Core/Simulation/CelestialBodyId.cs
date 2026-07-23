using System;

namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Identifies a celestial body with a stable, case-sensitive serialized value.</summary>
    public readonly struct CelestialBodyId : IEquatable<CelestialBodyId>, IComparable<CelestialBodyId>
    {
        /// <summary>Initializes a new identifier.</summary>
        /// <exception cref="ArgumentException">Thrown when the value is empty or padded with whitespace.</exception>
        public CelestialBodyId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A celestial body ID cannot be empty.", nameof(value));
            }

            if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            {
                throw new ArgumentException("A celestial body ID cannot contain leading or trailing whitespace.", nameof(value));
            }

            Value = value;
        }

        /// <summary>Gets the serialized identifier value.</summary>
        public string Value { get; }

        /// <inheritdoc />
        public int CompareTo(CelestialBodyId other) => string.Compare(Value, other.Value, StringComparison.Ordinal);

        /// <inheritdoc />
        public bool Equals(CelestialBodyId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is CelestialBodyId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);

        /// <inheritdoc />
        public override string ToString() => Value ?? string.Empty;

        /// <summary>Returns whether two IDs are equal.</summary>
        public static bool operator ==(CelestialBodyId left, CelestialBodyId right) => left.Equals(right);

        /// <summary>Returns whether two IDs are not equal.</summary>
        public static bool operator !=(CelestialBodyId left, CelestialBodyId right) => !left.Equals(right);
    }
}
