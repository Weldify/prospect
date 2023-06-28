using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public readonly struct Transform : IEquatable<Transform> {
	public readonly Vector3f Position;
	public readonly float Scale;
	public readonly Rotation Rotation;

	public static bool operator ==( Transform t1, Transform t2 ) => t1.Position == t2.Position;
	public static bool operator !=( Transform t1, Transform t2 ) => !(t1 == t2);

	public bool Equals( Transform other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Transform other && this == other;
	public override int GetHashCode() => HashCode.Combine( Position.GetHashCode(), Rotation.GetHashCode(), Scale );
}
