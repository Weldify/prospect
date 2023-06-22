using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public readonly struct Vector2u : IEquatable<Vector2u> {
	public static Vector2u Zero => new( 0, 0 );

	public readonly uint X = 0;
	public readonly uint Y = 0;

	public Vector2u( uint x, uint y ) {
		X = x;
		Y = y;
	}

	public bool Equals( Vector2u other ) => X == other.X && Y == other.Y;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2u other && Equals( other );
	public override int GetHashCode() => HashCode.Combine( X, Y );

	public static bool operator ==( Vector2u left, Vector2u right ) => left.Equals( right );
	public static bool operator !=( Vector2u left, Vector2u right ) => !left.Equals( right );
}