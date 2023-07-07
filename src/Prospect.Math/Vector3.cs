using System;
using System.Diagnostics.CodeAnalysis;

namespace Prospect.Engine;

public struct Vector3 : IEquatable<Vector3> {
	public static readonly Vector3 One = new( 1f, 1f, 1f );
	public static readonly Vector3 Zero = new( 0f, 0f, 0f );
	public static readonly Vector3 Forward = new( 0f, 0f, -1f ); // OpenGL is Z-
	public static readonly Vector3 Up = new( 0f, 1f, 0f );
	public static readonly Vector3 Right = new( 1f, 0f, 0f );

	public float X {
		get => _vec.X;
		set => _vec.X = value;
	}

	public float Y {
		get => _vec.Y;
		set => _vec.Y = value;
	}

	public float Z {
		get => _vec.Z;
		set => _vec.Z = value;
	}

	public float Length => _vec.Length();
	public float LengthSquared => _vec.LengthSquared();
	public Vector3 Normal => (LengthSquared == 0f) ? Zero : (this / Length);

	System.Numerics.Vector3 _vec;

	public Vector3( float x, float y, float z ) {
		_vec.X = x;
		_vec.Y = y;
		_vec.Z = z;
	}

	public float Dot( Vector3 v ) => System.Numerics.Vector3.Dot( _vec, v._vec );
	public Vector3 Cross( Vector3 v ) => System.Numerics.Vector3.Cross( _vec, v._vec );
	public void OrthoNormalize( Vector3 tangent ) => this = tangent.Normal.Cross( Normal );

	public static bool operator ==( Vector3 a, Vector3 b ) => a._vec == b._vec;
	public static bool operator !=( Vector3 a, Vector3 b ) => a._vec != b._vec;
	public static Vector3 operator +( Vector3 a, Vector3 b ) => a._vec + b._vec;
	public static Vector3 operator -( Vector3 a, Vector3 b ) => a._vec - b._vec;
	public static Vector3 operator -( Vector3 v ) => -v._vec;
	public static Vector3 operator *( Vector3 a, Vector3 b ) => a._vec * b._vec;
	public static Vector3 operator *( Vector3 v, float n ) => v._vec * n;
	public static Vector3 operator /( Vector3 a, Vector3 b ) => a._vec / b._vec;
	public static Vector3 operator /( Vector3 v, float n ) => v._vec / n;

	public bool Equals( Vector3 other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Vector3 other && this == other;
	public override int GetHashCode() => _vec.GetHashCode();

	public static implicit operator System.Numerics.Vector3( Vector3 v ) => new( v._vec.X, v._vec.Y, v._vec.Z );
	public static implicit operator Vector3( System.Numerics.Vector3 v ) => new( v.X, v.Y, v.Z );
}
