using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public struct Vector2f : IEquatable<Vector2f> {
	public static readonly Vector2f Zero = new( 0f, 0f );

	public float X = 0;
	public float Y = 0;

	public Vector2f( float x, float y ) {
		X = x;
		Y = y;
	}

	public static bool operator ==( Vector2f v1, Vector2f v2 ) => v1.X == v2.X && v1.Y == v2.Y;
	public static bool operator !=( Vector2f v1, Vector2f v2 ) => !(v1 == v2);
	public static Vector2f operator +( Vector2f v1, Vector2f v2 ) => new( v1.X + v2.X, v1.Y + v2.Y );
	public static Vector2f operator -( Vector2f v1, Vector2f v2 ) => new( v1.X - v2.X, v1.Y - v2.Y );
	public static Vector2f operator -( Vector2f v ) => new( -v.X, -v.Y );
	public static Vector2f operator *( Vector2f v1, float f ) => new( v1.X * f, v1.Y * f );
	public static Vector2f operator /( Vector2f v1, float f ) => new( v1.X / f, v1.Y / f );

	public bool Equals( Vector2f other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2f other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y );

	public static implicit operator Vector2( Vector2f v ) => new( v.X, v.Y );
}
