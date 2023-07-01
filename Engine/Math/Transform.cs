using Silk.NET.Input;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Prospect.Engine;

public struct Transform : IEquatable<Transform> {
	public static readonly Transform Zero = new();

	public Vector3f Position;
	public Rotation Rotation;
	public float Scale;

	public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion( Rotation ) * Matrix4x4.CreateScale( Scale ) * Matrix4x4.CreateTranslation( Position );

	public Transform( Vector3f position, Rotation rotation, float scale ) {
		Position = position;
		Rotation = rotation;
		Scale = scale;
	}

	public Transform( Vector3f position, Rotation rotation ) {
		Position = position;
		Rotation = rotation;
		Scale = 1f;
	}

	public Transform( Vector3f position ) {
		Position = position;
		Rotation = Rotation.Identity;
		Scale = 1f;
	}

	public Transform() {
		Position = Vector3f.Zero;
		Rotation = Rotation.Identity;
		Scale = 1f;
	}

	public static bool operator ==( Transform t1, Transform t2 ) => t1.Position == t2.Position;
	public static bool operator !=( Transform t1, Transform t2 ) => !(t1 == t2);

	public static Transform operator +( Transform t, Vector3f v ) => t with { Position = t.Position + v };
	public static Transform operator -( Transform t, Vector3f v ) => t with { Position = t.Position - v };

	public bool Equals( Transform other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Transform other && this == other;
	public override int GetHashCode() => HashCode.Combine( Position.GetHashCode(), Rotation.GetHashCode(), Scale );
}
