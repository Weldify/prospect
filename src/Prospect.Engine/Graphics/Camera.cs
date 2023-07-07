using System.Numerics;

namespace Prospect.Engine;

public static class Camera {
	public static Transform Transform {
		get => _transform;
		set {
			_transform = value;

			// Recompute ViewMatrix when changing transform so the graphics backend doesn't have to do that
			ViewMatrix = Matrix4x4.CreateLookAt(
				_transform.Position,
				_transform.Position + _transform.Rotation.Forward,
				_transform.Rotation.Up
			);
		}
	}

	/// <summary> Camera's field of view in degrees </summary>
	public static float FieldOfView { get; set; } = 70f;
    public static float Near { get; set; } = 0.01f;
    public static float Far { get; set; } = 100f;

	internal static Matrix4x4 ViewMatrix { get; private set; }

	static Transform _transform;

	static Camera() {
		var startPosition = -Vector3.Forward * 5f;
		Transform = new Transform( startPosition, Rotation.Identity );
	}
}
