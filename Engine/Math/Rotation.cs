using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Prospect.Engine;
using YamlDotNet.Core.Tokens;

public struct Rotation : IEquatable<Rotation> {
	const float _SLERP_EPSILON = 1e-6f;

	public static readonly Rotation Identity = new( 0f, 0f, 0f, 1f );
	public static readonly Rotation Zero = new( 0f, 0f, 0f, 0f );

	public static Rotation LookAt( Vector3f sourcePoint, Vector3f destPoint ) {
		Vector3f forwardVector = (destPoint - sourcePoint).Normal;

		float dot = Vector3.Dot( Vector3f.Forward, forwardVector );

		if ( Math.Abs( dot - (-1.0f) ) < 0.000001f ) {
			return new Rotation( Vector3f.Up.X, Vector3f.Up.Y, Vector3f.Up.Z, 3.1415926535897932f );
		}
		if ( Math.Abs( dot - (1.0f) ) < 0.000001f ) {
			return Identity;
		}

		float rotAngle = (float)Math.Acos( dot );
		Vector3f rotAxis = Vector3f.Forward.Cross( forwardVector );

		return FromAxisAngle( rotAxis.Normal, rotAngle );
	}

	public static Rotation FromYawPitchRoll( float yaw, float pitch, float roll ) {
		//  Roll first, about axis the object is facing, then
		//  pitch upward, then yaw to face into the new heading
		float sr, cr, sp, cp, sy, cy;

		float halfRoll = roll * 0.5f;
		sr = MathF.Sin( halfRoll );
		cr = MathF.Cos( halfRoll );

		float halfPitch = pitch * 0.5f;
		sp = MathF.Sin( halfPitch );
		cp = MathF.Cos( halfPitch );

		float halfYaw = yaw * 0.5f;
		sy = MathF.Sin( halfYaw );
		cy = MathF.Cos( halfYaw );

		return new(
			cy * sp * cr + sy * cp * sr,
			sy * cp * cr - cy * sp * sr,
			cy * cp * sr - sy * sp * cr,
			cy * cp * cr + sy * sp * sr
		);
	}

	public static Rotation FromAxisAngle( Vector3f axis, float angle ) {
		float halfAngle = angle * 0.5f;
		float s = MathF.Sin( halfAngle );
		float c = MathF.Cos( halfAngle );

		return new(
			axis.X * s,
			axis.Y * s,
			axis.Z * s,
			c
		);
	}

	public float X;
	public float Y;
	public float Z;
	public float W;

	public Rotation Normal {
		get {
			float ls = X * X + Y * Y + Z * Z + W * W;
			float invNorm = 1f / MathF.Sqrt( ls );

			return this * invNorm;
		}
	}

	public Rotation Inverse {
		get {
			float ls = X * X + Y * Y + Z * Z + W * W;
			float invNorm = 1f / ls;

			return Normal.Conjugate * invNorm;
		}
	}

	public Vector3f Forward => new(
		2f * (X * Z + W * Y),
		2f * (Y * Z - W * X),
		1f - 2f * (X * X + Y * Y)
	);

	public Vector3f Up => new(
		2f * (X * Y + W * Z),
		1f - 2f * (X * X + Z * Z),
		2f * (Y * Z + W * X)
	);

	public Vector3f Right => new(
		-(1f - 2f * (Y * Y + Z * Z)), // Negative because usually its Left. But Right is a cooler direction so fuck the world
		2f * (X * Y + W * Z),
		2f * (X * Z - W * Y)
	);

	public Rotation Conjugate => new( -X, -Y, -Z, W );
	public float SquareLength => X * X + Y * Y + Z * Z + W * W;
	public float Length => MathF.Sqrt( SquareLength );

	public Rotation( float x, float y, float z, float w ) {
		X = x;
		Y = y;
		Z = z;
		W = w;
	}
	public Rotation( Vector3 vectorPart, float scalarPart ) {
		X = vectorPart.X;
		Y = vectorPart.Y;
		Z = vectorPart.Z;
		W = scalarPart;
	}

	public static Rotation operator /( Rotation r1, Rotation r2 ) {
		float q1x = r1.X;
		float q1y = r1.Y;
		float q1z = r1.Z;
		float q1w = r1.W;

		//-------------------------------------
		// Inverse part.
		float ls = r2.X * r2.X + r2.Y * r2.Y +
				   r2.Z * r2.Z + r2.W * r2.W;
		float invNorm = 1f / ls;

		float q2x = -r2.X * invNorm;
		float q2y = -r2.Y * invNorm;
		float q2z = -r2.Z * invNorm;
		float q2w = r2.W * invNorm;

		//-------------------------------------
		// Multiply part.

		// cross(av, bv)
		float cx = q1y * q2z - q1z * q2y;
		float cy = q1z * q2x - q1x * q2z;
		float cz = q1x * q2y - q1y * q2x;

		float dot = q1x * q2x + q1y * q2y + q1z * q2z;

		return new(
			q1x * q2w + q2x * q1w + cx,
			q1y * q2w + q2y * q1w + cy,
			q1z * q2w + q2z * q1w + cz,
			q1w * q2w - dot
		);
	}

	public static Rotation operator *( Rotation r1, Rotation r2 ) {
		float q1x = r1.X;
		float q1y = r1.Y;
		float q1z = r1.Z;
		float q1w = r1.W;

		float q2x = r2.X;
		float q2y = r2.Y;
		float q2z = r2.Z;
		float q2w = r2.W;

		// cross(av, bv)
		float cx = q1y * q2z - q1z * q2y;
		float cy = q1z * q2x - q1x * q2z;
		float cz = q1x * q2y - q1y * q2x;

		float dot = q1x * q2x + q1y * q2y + q1z * q2z;

		return new(
			q1x * q2w + q2x * q1w + cx,
			q1y * q2w + q2y * q1w + cy,
			q1z * q2w + q2z * q1w + cz,
			q1w * q2w - dot
		);
	}

	public static bool operator ==( Rotation value1, Rotation value2 ) =>
		(value1.X == value2.X)
		&& (value1.Y == value2.Y)
		&& (value1.Z == value2.Z)
		&& (value1.W == value2.W);

	public static bool operator !=( Rotation value1, Rotation value2 ) => !(value1 == value2);

	public static Rotation operator +( Rotation r1, Rotation r2 ) => new( r1.X + r2.X, r1.Y + r2.Y, r1.Z + r2.Z, r1.W + r2.W );
	public static Rotation operator *( Rotation r, float f ) => new( r.X * f, r.Y * f, r.Z * f, r.W * f );
	public static Rotation operator -( Rotation r1, Rotation r2 ) => new( r1.X - r2.X, r1.Y - r2.Y, r1.Z - r2.Z, r1.W - r2.W );
	public static Rotation operator -( Rotation r ) => new( -r.X, -r.Y, -r.Z, -r.W );

	public Rotation Concatenated( Rotation r ) {
		// Concatenate rotation is actually q2 * q1 instead of q1 * q2.
		// So that's why value2 goes q1 and value1 goes q2.
		float q1x = r.X;
		float q1y = r.Y;
		float q1z = r.Z;
		float q1w = r.W;

		float q2x = X;
		float q2y = Y;
		float q2z = Z;
		float q2w = W;

		// cross(av, bv)
		float cx = q1y * q2z - q1z * q2y;
		float cy = q1z * q2x - q1x * q2z;
		float cz = q1x * q2y - q1y * q2x;

		float dot = q1x * q2x + q1y * q2y + q1z * q2z;

		return new(
			q1x * q2w + q2x * q1w + cx,
			q1y * q2w + q2y * q1w + cy,
			q1z * q2w + q2z * q1w + cz,
			q1w * q2w - dot
		);
	}

	public float Dot( Quaternion other ) {
		return X * other.X +
			   Y * other.Y +
			   Z * other.Z +
			   W * other.W;
	}

	public override readonly string ToString() =>
		$"{{X:{X} Y:{Y} Z:{Z} W:{W}}}";

	public static implicit operator Quaternion( Rotation r ) => new( r.X, r.Y, r.Z, r.W );

	public bool Equals( Rotation other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Rotation other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y, Z, W );
}
