namespace Prospect.Engine.OpenGl;

struct Vertex {
	public const uint MAX_BONE_INFLUENCE = 4;

	public Vector3f Position;
	public Vector2f TexCoords;

	public int[] BoneIds;
	public float[] Weights;
}
