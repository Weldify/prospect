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

	internal static Matrix4x4 ViewMatrix { get; private set; }

	static Transform _transform;

	static Camera() {
		var startPosition = -Vector3f.Forward * 5f;
		Transform = new Transform( startPosition, Rotation.Identity );
	}
}
