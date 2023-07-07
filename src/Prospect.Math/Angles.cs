using System;
using System.Diagnostics.CodeAnalysis;

namespace Prospect.Engine;

public struct Angles : IEquatable<Angles> {
	public static readonly Angles Zero = new( 0f, 0f, 0f );

	public float Yaw {
		get => _vec.X;
		set => _vec.X = value;
	}

	public float Pitch {
		get => _vec.Y;
		set => _vec.Y = value;
	}

	public float Roll {
		get => _vec.Z;
		set => _vec.Z = value;
	}

	public Angles Wrapped => new(
		_vec.X % 360f,
		_vec.Y % 360f,
		_vec.Z % 360f
	);

	System.Numerics.Vector3 _vec;

	public Angles( float yaw, float pitch, float roll ) {
		_vec.X = yaw;
		_vec.Y = pitch;
		_vec.Z = roll;
	}

	public void Deconstruct( out float yaw, out float pitch, out float roll ) {
		yaw = _vec.X;
		pitch = _vec.Y;
		roll = _vec.Z;
	}

	public static bool operator ==( Angles a, Angles b ) => a._vec == b._vec;
	public static bool operator !=( Angles a, Angles b ) => a._vec != b._vec;
	public static Angles operator +( Angles a, Angles b ) => a._vec + b._vec;
	public static Angles operator -( Angles a, Angles b ) => a._vec - b._vec;
	public static Angles operator -( Angles v ) => -v._vec;
	public static Angles operator *( Angles a, Angles b ) => a._vec * b._vec;
	public static Angles operator *( Angles v, float n ) => v._vec * n;
	public static Angles operator /( Angles a, Angles b ) => a._vec / b._vec;
	public static Angles operator /( Angles v, float n ) => v._vec / n;

	public bool Equals( Angles other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Angles other && this == other;
	public override int GetHashCode() => _vec.GetHashCode();

	public static implicit operator Angles( System.Numerics.Vector3 v ) => new( v.X, v.Y, v.Z );
}
