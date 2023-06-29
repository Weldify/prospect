using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public struct Vector3f : IEquatable<Vector3f> {
	public static readonly Vector3f Zero = new( 0f, 0f, 0f );
	public static readonly Vector3f One = new( 1f, 1f, 1f );
	public static readonly Vector3f Forward = new( 0f, 0f, 1f );
	public static readonly Vector3f Right = new( 1f, 0f, 0f );
	public static readonly Vector3f Up = new( 0f, 1f, 0f );

	public float X = 0f;
	public float Y = 0f;
	public float Z = 0f;

	public Vector3f Normal => SquareLength switch {
		0f => Zero,
		_ => this / Length
	};

	public float SquareLength => X * X + Y * Y + Z * Z;
	public float Length => MathF.Sqrt( SquareLength );

	public Vector3f( float x, float y, float z ) {
		X = x;
		Y = y;
		Z = z;
	}

	public Vector3f Cross( Vector3f v ) => new(
		(Y * v.Z) - (Z * v.Y),
		(Z * v.X) - (X * v.Z),
		(X * v.Y) - (Y * v.X)
	);

	public static bool operator ==( Vector3f v1, Vector3f v2 ) => v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
	public static bool operator !=( Vector3f v1, Vector3f v2 ) => !(v1 == v2);

	public static Vector3f operator +( Vector3f v1, Vector3f v2 ) => new( v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z );
	public static Vector3f operator -( Vector3f v1, Vector3f v2 ) => new( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );
	public static Vector3f operator *( Vector3f v1, float f ) => new( v1.X * f, v1.Y * f, v1.Z * f );
	public static Vector3f operator /( Vector3f v1, float f ) => new( v1.X / f, v1.Y / f, v1.Z / f );

	public bool Equals( Vector3f other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector3f other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y, Z );

	public static implicit operator Vector3( Vector3f v ) => new( v.X, v.Y, v.Z );
	public static implicit operator Vector3f( Vector3 v ) => new( v.X, v.Y, v.Z );
}
