using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public readonly struct Vector2u : IEquatable<Vector2u> {
	public static Vector2u Zero => new( 0, 0 );

	public readonly int X = 0;
	public readonly int Y = 0;

	public Vector2u( int x, int y ) {
		X = x;
		Y = y;
	}

	public static bool operator ==( Vector2u v1, Vector2u v2 ) => v1.X == v2.X && v1.Y == v2.Y;
	public static bool operator !=( Vector2u v1, Vector2u v2 ) => !(v1 == v2);

	public bool Equals( Vector2u other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2u other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y );

	public static implicit operator Vector2( Vector2u v ) => new( v.X, v.Y );
}
