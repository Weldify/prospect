using System.Diagnostics.CodeAnalysis;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Prospect.Engine;

public struct Transform : IEquatable<Transform> {
	public static readonly Transform Zero = new( Vector3.Zero );

	public Matrix4x4 Matrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion( Rotation ) * Matrix4x4.CreateScale( Scale ) * Matrix4x4.CreateTranslation( Position );

	public Vector3 Position;
	public Rotation Rotation;
	public float Scale;

	public Transform( Vector3 position, Rotation rotation, float scale ) {
		Position = position;
		Rotation = rotation;
		Scale = scale;
	}

	public Transform( Vector3 position, Rotation rotation ) {
		Position = position;
		Rotation = rotation;
		Scale = 1f;
	}

	public Transform( Vector3 position ) {
		Position = position;
		Rotation = Rotation.Identity;
		Scale = 1f;
	}

	public static bool operator ==( Transform a, Transform b ) => a.Position == b.Position && a.Rotation == b.Rotation && a.Scale == b.Scale;
	public static bool operator !=( Transform a, Transform b ) => a.Position != b.Position || a.Rotation != b.Rotation || a.Scale != b.Scale;

	public static Transform operator +( Transform t, Vector3 v ) => t with { Position = t.Position + v };
	public static Transform operator -( Transform t, Vector3 v ) => t with { Position = t.Position - v };

	public bool Equals( Transform other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Transform other && this == other;
	public override int GetHashCode() => HashCode.Combine( Position.GetHashCode(), Rotation.GetHashCode(), Scale );
}
