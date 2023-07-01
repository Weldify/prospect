﻿using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public struct Angles : IEquatable<Angles> {
	public readonly static Angles Zero = new( 0f, 0f, 0f );

	public float Yaw;
	public float Pitch;
	public float Roll;

	public Angles Wrapped => new(
		Yaw % 180f,
		Pitch % 180f,
		Roll % 180f
	);

	public Angles( float yaw, float pitch, float roll ) {
		Yaw = yaw;
		Pitch = pitch;
		Roll = roll;
	}

	public static bool operator ==( Angles a1, Angles a2 ) => a1.Pitch == a2.Pitch && a1.Roll == a2.Roll && a1.Yaw == a2.Yaw;
	public static bool operator !=( Angles a1, Angles a2 ) => !(a1 == a2);
	public static Angles operator +( Angles a1, Angles a2 ) => new( a1.Yaw + a2.Yaw, a1.Pitch + a2.Pitch, a1.Roll + a2.Roll );
	public static Angles operator -( Angles a1, Angles a2 ) => new( a1.Yaw - a2.Yaw, a1.Pitch - a2.Pitch, a1.Roll - a2.Roll );
	public static Angles operator -( Angles a ) => new( -a.Yaw, -a.Pitch, -a.Roll );
	public static Angles operator *( Angles a1, float n ) => new( a1.Yaw * n, a1.Pitch * n, a1.Roll * n );
	public static Angles operator /( Angles a1, float n ) => new( a1.Yaw / n, a1.Pitch / n, a1.Roll / n );

	public bool Equals( Angles other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Angles other && this == other;
	public override int GetHashCode() => HashCode.Combine( Pitch, Roll, Yaw );

	public static explicit operator Rotation( Angles a ) {
		var clamped = a.Wrapped;
		return Rotation.FromYawPitchRoll( clamped.Yaw, clamped.Pitch, clamped.Roll );
	}
}