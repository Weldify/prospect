namespace Prospect.Engine.OpenGL;

struct Vertex {
	public const uint MAX_BONE_INFLUENCE = 4;

	public Vector3 Position;
	public Vector2 TexCoords;

	public int[] BoneIds;
	public float[] Weights;
}
