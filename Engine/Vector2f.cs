using System;
using System.Diagnostics.CodeAnalysis;

namespace Prospect.Engine;

public readonly struct Vector2f : IEquatable<Vector2f> {
	public readonly float X = 0;
	public readonly float Y = 0;

	public Vector2f( float x, float y ) {
		X = x;
		Y = y;
	}

	public bool Equals( Vector2f other ) => X == other.X && Y == other.Y;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2f other && Equals( other );
	public override int GetHashCode() => HashCode.Combine( X, Y );

	public static bool operator ==( Vector2f left, Vector2f right ) => left.Equals( right );
	public static bool operator !=( Vector2f left, Vector2f right ) => !left.Equals( right );
}