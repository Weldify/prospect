namespace Prospect.Engine;

public static class Camera {
	public static Transform Transform { get; set; } = new( new( 0f, 0f, -1f ), Rotation.LookAt( new( 0f, 0f, -1f ), Vector3f.Zero ) );

	/// <summary> Camera's field of view in degrees </summary>
	public static float FieldOfView { get; set; } = 70f;
}
