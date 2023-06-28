using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Prospect.Engine;

public readonly struct Rotation : IEquatable<Rotation> {
	const float _SLERP_EPSILON = 1e-6f;

	public static Rotation Identity => new( 0, 0, 0, 1 );
	public static Rotation Zero => new( 0, 0, 0, 0 );

	public readonly float X;
	public readonly float Y;
	public readonly float Z;
	public readonly float W;

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
		float invNorm = 1.0f / ls;

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

	public static Quaternion Concatenate( Quaternion value1, Quaternion value2 ) {
		Quaternion ans;

		// Concatenate rotation is actually q2 * q1 instead of q1 * q2.
		// So that's why value2 goes q1 and value1 goes q2.
		float q1x = value2.X;
		float q1y = value2.Y;
		float q1z = value2.Z;
		float q1w = value2.W;

		float q2x = value1.X;
		float q2y = value1.Y;
		float q2z = value1.Z;
		float q2w = value1.W;

		// cross(av, bv)
		float cx = q1y * q2z - q1z * q2y;
		float cy = q1z * q2x - q1x * q2z;
		float cz = q1x * q2y - q1y * q2x;

		float dot = q1x * q2x + q1y * q2y + q1z * q2z;

		ans.X = q1x * q2w + q2x * q1w + cx;
		ans.Y = q1y * q2w + q2y * q1w + cy;
		ans.Z = q1z * q2w + q2z * q1w + cz;
		ans.W = q1w * q2w - dot;

		return ans;
	}

	public static Quaternion Conjugate( Quaternion value ) {
		Quaternion ans;

		ans.X = -value.X;
		ans.Y = -value.Y;
		ans.Z = -value.Z;
		ans.W = value.W;

		return ans;
	}

	public static Quaternion FromAxisAngle( Vector3 axis, float angle ) {
		Quaternion ans;

		float halfAngle = angle * 0.5f;
		float s = MathF.Sin( halfAngle );
		float c = MathF.Cos( halfAngle );

		ans.X = axis.X * s;
		ans.Y = axis.Y * s;
		ans.Z = axis.Z * s;
		ans.W = c;

		return ans;
	}

	public static Quaternion FromRotationMatrix( Matrix4x4 matrix ) {
		float trace = matrix.M11 + matrix.M22 + matrix.M33;

		Quaternion q = default;

		if ( trace > 0.0f ) {
			float s = MathF.Sqrt( trace + 1.0f );
			q.W = s * 0.5f;
			s = 0.5f / s;
			q.X = (matrix.M23 - matrix.M32) * s;
			q.Y = (matrix.M31 - matrix.M13) * s;
			q.Z = (matrix.M12 - matrix.M21) * s;
		} else {
			if ( matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33 ) {
				float s = MathF.Sqrt( 1.0f + matrix.M11 - matrix.M22 - matrix.M33 );
				float invS = 0.5f / s;
				q.X = 0.5f * s;
				q.Y = (matrix.M12 + matrix.M21) * invS;
				q.Z = (matrix.M13 + matrix.M31) * invS;
				q.W = (matrix.M23 - matrix.M32) * invS;
			} else if ( matrix.M22 > matrix.M33 ) {
				float s = MathF.Sqrt( 1.0f + matrix.M22 - matrix.M11 - matrix.M33 );
				float invS = 0.5f / s;
				q.X = (matrix.M21 + matrix.M12) * invS;
				q.Y = 0.5f * s;
				q.Z = (matrix.M32 + matrix.M23) * invS;
				q.W = (matrix.M31 - matrix.M13) * invS;
			} else {
				float s = MathF.Sqrt( 1.0f + matrix.M33 - matrix.M11 - matrix.M22 );
				float invS = 0.5f / s;
				q.X = (matrix.M31 + matrix.M13) * invS;
				q.Y = (matrix.M32 + matrix.M23) * invS;
				q.Z = 0.5f * s;
				q.W = (matrix.M12 - matrix.M21) * invS;
			}
		}

		return q;
	}

	public static Quaternion FromYawPitchRoll( float yaw, float pitch, float roll ) {
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

		Quaternion result;

		result.X = cy * sp * cr + sy * cp * sr;
		result.Y = sy * cp * cr - cy * sp * sr;
		result.Z = cy * cp * sr - sy * sp * cr;
		result.W = cy * cp * cr + sy * sp * sr;

		return result;
	}

	public static float Dot( Quaternion quaternion1, Quaternion quaternion2 ) {
		return quaternion1.X * quaternion2.X +
			   quaternion1.Y * quaternion2.Y +
			   quaternion1.Z * quaternion2.Z +
			   quaternion1.W * quaternion2.W;
	}

	public static Quaternion Inverse( Quaternion value ) {
		//  -1   (       a              -v       )
		// q   = ( -------------   ------------- )
		//       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

		Quaternion ans;

		float ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
		float invNorm = 1.0f / ls;

		ans.X = -value.X * invNorm;
		ans.Y = -value.Y * invNorm;
		ans.Z = -value.Z * invNorm;
		ans.W = value.W * invNorm;

		return ans;
	}

	public static Quaternion Lerp( Quaternion quaternion1, Quaternion quaternion2, float amount ) {
		float t = amount;
		float t1 = 1.0f - t;

		Quaternion r = default;

		float dot = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
					quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

		if ( dot >= 0.0f ) {
			r.X = t1 * quaternion1.X + t * quaternion2.X;
			r.Y = t1 * quaternion1.Y + t * quaternion2.Y;
			r.Z = t1 * quaternion1.Z + t * quaternion2.Z;
			r.W = t1 * quaternion1.W + t * quaternion2.W;
		} else {
			r.X = t1 * quaternion1.X - t * quaternion2.X;
			r.Y = t1 * quaternion1.Y - t * quaternion2.Y;
			r.Z = t1 * quaternion1.Z - t * quaternion2.Z;
			r.W = t1 * quaternion1.W - t * quaternion2.W;
		}

		// Normalize it.
		float ls = r.X * r.X + r.Y * r.Y + r.Z * r.Z + r.W * r.W;
		float invNorm = 1.0f / MathF.Sqrt( ls );

		r.X *= invNorm;
		r.Y *= invNorm;
		r.Z *= invNorm;
		r.W *= invNorm;

		return r;
	}

	public static Quaternion Normalize( Quaternion value ) {
		Quaternion ans;

		float ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;

		float invNorm = 1.0f / MathF.Sqrt( ls );

		ans.X = value.X * invNorm;
		ans.Y = value.Y * invNorm;
		ans.Z = value.Z * invNorm;
		ans.W = value.W * invNorm;

		return ans;
	}

	public static Quaternion Slerp( Quaternion quaternion1, Quaternion quaternion2, float amount ) {
		float t = amount;

		float cosOmega = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
						 quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

		bool flip = false;

		if ( cosOmega < 0.0f ) {
			flip = true;
			cosOmega = -cosOmega;
		}

		float s1, s2;

		if ( cosOmega > (1.0f - _SLERP_EPSILON) ) {
			// Too close, do straight linear interpolation.
			s1 = 1.0f - t;
			s2 = (flip) ? -t : t;
		} else {
			float omega = MathF.Acos( cosOmega );
			float invSinOmega = 1 / MathF.Sin( omega );

			s1 = MathF.Sin( (1.0f - t) * omega ) * invSinOmega;
			s2 = (flip)
				? -MathF.Sin( t * omega ) * invSinOmega
				: MathF.Sin( t * omega ) * invSinOmega;
		}

		Quaternion ans;

		ans.X = s1 * quaternion1.X + s2 * quaternion2.X;
		ans.Y = s1 * quaternion1.Y + s2 * quaternion2.Y;
		ans.Z = s1 * quaternion1.Z + s2 * quaternion2.Z;
		ans.W = s1 * quaternion1.W + s2 * quaternion2.W;

		return ans;
	}

	public bool Equals( Rotation other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Rotation other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y, Z, W );

	public readonly float Length() {
		float lengthSquared = LengthSquared();
		return MathF.Sqrt( lengthSquared );
	}

	public readonly float LengthSquared() {
		return X * X + Y * Y + Z * Z + W * W;
	}

	public override readonly string ToString() =>
		$"{{X:{X} Y:{Y} Z:{Z} W:{W}}}";
}
