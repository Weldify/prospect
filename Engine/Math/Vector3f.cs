using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public readonly struct Vector3f : IEquatable<Vector3f> {
	public static Vector3f Zero => new( 0f, 0f, 0f );

	public readonly float X = 0f;
	public readonly float Y = 0f;
	public readonly float Z = 0f;

	public Vector3f( float x, float y, float z ) {
		X = x;
		Y = y;
		Z = z;
	}

	public static bool operator ==( Vector3f v1, Vector3f v2 ) => v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
	public static bool operator !=( Vector3f v1, Vector3f v2 ) => !(v1 == v2);

	public bool Equals( Vector3f other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector3f other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y, Z );
}
