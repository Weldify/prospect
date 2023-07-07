using System;
using System.Diagnostics.CodeAnalysis;

namespace Prospect.Engine;

public struct Vector2 : IEquatable<Vector2> {
	public static readonly Vector2 Zero = new( 0f, 0f );
	public static readonly Vector2 One = new( 1f, 1f );

	public float X {
		get => _vec.X;
		set => _vec.X = value;
	}

	public float Y {
		get => _vec.Y;
		set => _vec.Y = value;
	}

	System.Numerics.Vector2 _vec;

	public Vector2( float x, float y ) {
		_vec.X = x;
		_vec.Y = y;
	}

	public static bool operator ==( Vector2 a, Vector2 b ) => a._vec == b._vec;
	public static bool operator !=( Vector2 a, Vector2 b ) => a._vec != b._vec;
	public static Vector2 operator +( Vector2 a, Vector2 b ) => a._vec + b._vec;
	public static Vector2 operator -( Vector2 a, Vector2 b ) => a._vec - b._vec;
	public static Vector2 operator -( Vector2 v ) => -v._vec;
	public static Vector2 operator *( Vector2 a, Vector2 b ) => a._vec * b._vec;
	public static Vector2 operator *( Vector2 v, float n ) => v._vec * n;
	public static Vector2 operator /( Vector2 a, Vector2 b ) => a._vec / b._vec;
	public static Vector2 operator /( Vector2 v, float n ) => v._vec / n;

	public bool Equals( Vector2 other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector2 other && this == other;
	public override int GetHashCode() => _vec.GetHashCode();

	public static implicit operator System.Numerics.Vector2( Vector2 v ) => new( v._vec.X, v._vec.Y );
	public static implicit operator Vector2( System.Numerics.Vector2 v ) => new( v.X, v.Y );
}
