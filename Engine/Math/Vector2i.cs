using Silk.NET.Maths;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public struct Vector2i : IEquatable<Vector2i> {
	public static readonly Vector2i Zero = new( 0, 0 );

	public int X = 0;
	public int Y = 0;

	public float Aspect => X / Y;

	public Vector2i( int x, int y ) {
		X = x;
		Y = y;
	}

	public static bool operator ==( Vector2i v1, Vector2i v2 ) => v1.X == v2.X && v1.Y == v2.Y;
	public static bool operator !=( Vector2i v1, Vector2i v2 ) => !(v1 == v2);

	public bool Equals( Vector2i other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2i other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y );

	public static implicit operator Vector2( Vector2i v ) => new( v.X, v.Y );
	public static implicit operator Vector2D<int>( Vector2i v ) => new( v.X, v.Y );
}
