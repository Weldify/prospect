using System;
using System.Diagnostics.CodeAnalysis;

namespace Prospect;

public readonly struct Vector2i : IEquatable<Vector2i> {
	public readonly int X = 0;
	public readonly int Y = 0;

	public Vector2i( int x, int y ) {
		X = x;
		Y = y;
	}

	public bool Equals( Vector2i other ) => X == other.X && Y == other.Y;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2i other && Equals( other );
	public override int GetHashCode() => HashCode.Combine( X, Y );

	public static bool operator ==( Vector2i left, Vector2i right ) => left.Equals( right );
	public static bool operator !=( Vector2i left, Vector2i right ) => !left.Equals( right );
}