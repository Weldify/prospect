using Prospect.Engine;
using System.Diagnostics.CodeAnalysis;

public struct Rotation : IEquatable<Rotation> {
	public static readonly Rotation Identity = new Rotation( 0f, 0f, 0f, 1f );

	public static Rotation LookAt( Vector3 forward ) => LookAt( forward, Vector3.Up );
	public static Rotation LookAt( Vector3 forward, Vector3 up ) {
		forward.OrthoNormalize( up );
		var right = up.Cross( forward );

		var w = MathF.Sqrt( 1f + right.X + up.Y + forward.Z ) / 2f;
		var recip = 1f / (4f * w);

		return new(
			(up.Z - forward.Y) * recip,
			(forward.X - right.Z) * recip,
			(right.Y - up.X) * recip,
			w
		);
	}

	public static Rotation From( Angles angles ) {
		var (yaw, pitch, roll) = angles.Wrapped * MathX.DEG_TO_RAD;
		return System.Numerics.Quaternion.CreateFromYawPitchRoll( yaw, pitch, roll );
	}

	public float X {
		get => _quat.X;
		set => _quat.X = value;
	}

	public float Y {
		get => _quat.Y;
		set => _quat.Y = value;
	}

	public float Z {
		get => _quat.Z;
		set => _quat.Z = value;
	}

	public float W {
		get => _quat.W;
		set => _quat.W = value;
	}

	public Angles Angles {
		get {
			float x = 2f * W * W + 2f * X * X - 1f;
			float y = 2f * X * Y + 2f * W * Z;
			float num = 2f * X * Z - 2f * W * Y;
			float y2 = 2f * Y * Z + 2f * W * X;
			float x2 = 2f * W * W + 2f * Z * Z - 1f;

			Angles angles = new(
				MathF.Asin( 0f - num ).ToDegrees(),
				MathF.Atan2( y, x ).ToDegrees(),
				MathF.Atan2( y2, x2 ).ToDegrees()
			);

			return angles.Wrapped;
		}
	}

	public Rotation Normal => System.Numerics.Quaternion.Normalize( _quat );
	public Vector3 Forward => (this * Vector3.Forward).Normal;
	public Vector3 Up => (this * Vector3.Up).Normal;
	public Vector3 Right => (this * Vector3.Right).Normal;

	System.Numerics.Quaternion _quat;

	public Rotation( float x, float y, float z, float w ) {
		_quat.X = x;
		_quat.Y = y;
		_quat.Z = z;
		_quat.W = w;
	}

	public static bool operator ==( Rotation a, Rotation b ) => a._quat == b._quat;
	public static bool operator !=( Rotation a, Rotation b ) => a._quat != b._quat;
	public static Rotation operator +( Rotation a, Rotation b ) => a._quat + b._quat;
	public static Rotation operator -( Rotation a, Rotation b ) => a._quat - b._quat;
	public static Rotation operator -( Rotation r ) => -r._quat;
	public static Rotation operator *( Rotation a, Rotation b ) => a._quat * b._quat;

	public static Vector3 operator *( Rotation r, Vector3 v ) {
		float x = r.X * 2f;
		float y = r.Y * 2f;
		float z = r.Z * 2f;

		float xx = r.X * x;
		float yy = r.Y * y;
		float zz = r.Z * z;

		float xy = r.X * y;
		float xz = r.X * z;
		float yz = r.Y * z;

		float wx = r.W * x;
		float wy = r.W * y;
		float wz = r.W * z;

		Vector3 result = new(
			(1f - (yy + zz)) * v.X + (xy - wz) * v.Y + (xz + wy) * v.Z,
			(xy + wz) * v.X + (1f - (xx + zz)) * v.Y + (yz - wx) * v.Z,
			(xz - wy) * v.X + (yz + wx) * v.Y + (1f - (xx + yy)) * v.Z
		);

		return result;
	}

	public bool Equals( Rotation r ) => this == r;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Rotation r && this == r;
	public override int GetHashCode() => _quat.GetHashCode();

	public static implicit operator System.Numerics.Quaternion( Rotation r ) => new( r._quat.X, r._quat.Y, r._quat.Z, r._quat.W );
	public static implicit operator Rotation( System.Numerics.Quaternion quat ) => new( quat.X, quat.Y, quat.Z, quat.W );
}
